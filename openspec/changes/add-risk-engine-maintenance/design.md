## Context

`CreditSystem.Frontend` es una Blazor Web App (Interactive Server) con una arquitectura estricta de 3 capas (`Pages → I<Grupo>Service → IApiClient`) descrita en `CLAUDE.md` y `Src/CreditSystem.Frontend.Web/README/frontend-overview.md`. Hasta ahora consume un único backend, `CreditSystem.Api` (`open_api.json`, 61 rutas), a través de un único `IApiClient`/`HttpClient` nombrado "CreditSystemApi".

Este change agrega la primera pantalla que consume un contrato distinto: `Crm.WebApi` (`openapi_crm.txt`), específicamente el tag `RiskEngine` (12 endpoints: `risk-rules`, `risk-matrices`, `risk-evaluations`). El swagger CRM, igual que el de CreditSystem, no declara `securityScheme` ni `servers`, y varios campos (`ruleType`, `targetField`, `status` de la matriz) viajan como `string` libre sin `enum`/`x-enumNames`.

Decisión de negocio ya tomada con el usuario: por ahora se asume que `Crm.WebApi` está detrás del mismo gateway que `CreditSystem.Api` (mismo `Api:BaseUrl`/`ApiKey`), así que no se introduce un segundo `HttpClient`. Esto es un supuesto, no un hecho confirmado — ver Riesgos.

## Goals / Non-Goals

**Goals:**
- Pantallas de mantenimiento (listar/crear/editar/eliminar) para reglas de riesgo y matrices de riesgo, siguiendo exactamente el patrón de 3 capas y las convenciones de UI (`PageHeader`, `DataTable`, `ConfirmDialog`, `_isLoading`/`_error`/`_data`) ya usadas en el resto del proyecto.
- Activación de una matriz de riesgo (`POST .../activate`).
- Pantalla de prueba para disparar una evaluación de riesgo y visualizar su resultado (ScoreCard), como herramienta de validación de la matriz activa.
- Un único punto de entrada HTTP reutilizado (`IApiClient` existente) — cero nueva infraestructura de red.

**Non-Goals:**
- No se cubre ningún otro tag del swagger CRM (`Prospects`, `Customers`, `CreditApplications`, `Workflow`, etc.) — quedan fuera de este change.
- No se construye un catálogo dinámico (backend-driven) para `ruleType`/`targetField`/`status`; se usan enums estáticos en el frontend con la expectativa explícita de migrarlos después.
- No se introduce Modo Datos Dummy específico más allá de lo que ya cubre `DummyDataFactory` por reflexión (no requiere cambios, ya generaliza cualquier `Response`).
- No se resuelve la incertidumbre de si `Crm.WebApi` vive detrás del mismo gateway — se documenta como supuesto.

## Decisions

**1. Reutilizar el `IApiClient` existente en vez de un segundo cliente HTTP.**
Alternativa considerada: crear `ICrmApiClient`/`HttpClient` "CrmApi" con su propio `BaseUrl`/`ApiKey` en `ApiSettings`. Se descarta por ahora porque el usuario confirmó que el backend CRM está detrás del mismo gateway que el resto de la app; agregar una segunda config sin necesidad real sería especular. Si en la práctica el gateway resulta distinto, el cambio está acotado a `ApiSettings`/`Program.cs`/`RiskEngineService`, sin tocar páginas.

**2. Nuevo servicio `IRiskEngineService`, no extender `IRiskService`.**
`IRiskService` ya existe y cubre el tag "Risk Classification" de `CreditSystem.Api` (`loans/risk-summary`, no relacionado). Mezclar ambos en un servicio violaría la regla "un servicio por tag del swagger" y generaría confusión entre dos dominios de riesgo distintos (clasificación de cartera vencida vs. motor de reglas del CRM). Se crea `IRiskEngineService`/`RiskEngineService` como par nuevo.

**3. Modelar `ruleType`, `targetField` y `status` (matriz) como enums C#, con `JsonStringEnumConverter` a nivel de tipo.**
El resto del proyecto usa el patrón "string libre + `StatusBadge.razor`" para campos `status` sin catálogo confirmado (ver `ProductResponse.Status`, etc.). Para este módulo el usuario pidió explícitamente usar enums desde ahora (mantenimiento de datos estructurados, no texto libre en un formulario de alta), sabiendo que la migración a catálogos reales del backend vendrá después.
Como el wire format de estos campos es `string` (no `int`, a diferencia de `GuaranteeType`/`AmortizationMethod`), cada enum lleva `[JsonConverter(typeof(JsonStringEnumConverter))]` declarado en el propio tipo — un cambio acotado a esos 3 enums, sin tocar el `JsonSerializerOptions` global de `ApiClient.cs` (que sigue sirviendo a los enums enteros existentes sin modificación).
Alternativa considerada: dejarlos como `string` libre + un `<select>` armado a mano en la página (sin tipo fuerte). Se descarta porque el usuario prefirió el enum explícito por ahora.
Valores elegidos (inferidos, sin `x-enumNames` en el swagger — mismo criterio de `DOCUMENTACION INSUFICIENTE` que `GuaranteeType.cs`):
- `RiskRuleType`: `MinThreshold`, `MaxThreshold`, `Range`, `Blacklist`, `Whitelist`.
- `RiskRuleTargetField`: `CreditScore`, `MonthlyIncome`, `MonthlyDebt`, `Salary` — únicos campos financieros confirmados en DTOs del swagger CRM (`UpdateCustomerFinancialsDto`, `CustomerWorkInfoDto`).
- `RiskMatrixStatus`: `Draft`, `Active`, `Archived` — inferido de que existe `POST .../activate` y un campo `version` (sugiere que solo una versión está activa a la vez).

**4. `PricingBand` como tipo único en `Models/Common/`, reusado en request y response.**
A diferencia de `GuaranteeInput`/`CreateGuaranteeRequest` (que divergen — el request omite `ExpirationDate`), el shape de `PricingBandDto` es idéntico en `CreateRiskMatrixDto`, `UpdateRiskMatrixDto` y `RiskMatrixDetailDto` (siempre `minScore`/`maxScore`/`interestRate`/`maxAmount`). Un solo tipo compartido evita duplicación sin romper la convención 1:1 con el swagger (el swagger también define un único `PricingBandDto` reusado por `$ref`).

**5. Rutas bajo `/crm/riesgo/...` y carpeta `Components/Pages/Crm/Riesgo/`.**
Ya existe `/riesgo` (Clasificación de Riesgo de préstamos, `IRiskService`). Usar el mismo prefijo confundiría dos módulos de dominios distintos. El prefijo `/crm/...` además deja espacio para que futuros módulos del swagger CRM (Prospects, Customers, etc.) se agreguen bajo el mismo grupo de sidebar sin reabrir esta decisión.

**6. Matriz de riesgo referencia reglas existentes por `ruleIds[]`, no las crea inline.**
El swagger confirma esto (`CreateRiskMatrixDto.ruleIds: uuid[]`), así que `MatrizNueva.razor`/`MatrizDetalle.razor` deben cargar `ListRiskRulesAsync()` primero y ofrecer una selección múltiple — no un formulario de reglas embebido.

**7. Edición inline en la pantalla de detalle, sin ruta `/editar` separada.**
Sigue la convención de rutas documentada (`/modulo`, `/modulo/nuevo`, `/modulo/{id}` — nada más) y el precedente de `Productos/Listado.razor`/`Riesgo/Index.razor`, que hacen `PUT` inline en vez de una pantalla de edición dedicada.

## Risks / Trade-offs

- **[Riesgo] `Crm.WebApi` termina NO estando detrás del mismo gateway/`BaseUrl`/`ApiKey`** → Mitigación: el `RiskEngineService` es el único punto que arma rutas `api/v1/risk-*`; si falla en producción, el fix es acotado (agregar `ApiSettings.Crm` + segundo `HttpClient`) sin tocar páginas ni modelos.
- **[Riesgo] Los valores reales de `ruleType`/`targetField`/`status` en el backend no coinciden en casing o en el set de valores con los enums inferidos aquí** → Mitigación: cada enum queda marcado `DOCUMENTACION INSUFICIENTE` (igual que `GuaranteeType`) y agregado a la tabla de incertidumbres de `frontend-overview.md`; el `JsonStringEnumConverter` fallará de forma visible (error de deserialización → `ApiError`) en vez de silenciosa si el backend manda un valor no mapeado, así que el problema se detecta rápido al probar contra el backend real.
- **[Trade-off] El editor de `parameters` (diccionario `string`→`string` libre) es una tabla dinámica de pares clave/valor sin validación de esquema** → Aceptado: el swagger no da más estructura que `additionalProperties: string`, así que cualquier validación adicional sería inventar contrato no documentado.
- **[Riesgo] `TriggerRiskEvaluationDto` solo recibe `creditApplicationId` — no hay forma de elegir qué matriz se usa para la evaluación de prueba** → Aceptado como limitación del contrato: la pantalla `EvaluacionPrueba.razor` debe dejar claro en la UI que la evaluación usa la matriz `Active` vigente (no seleccionable), y mostrar `riskMatrixId`/`riskMatrixVersion` del resultado para que quede trazable cuál se usó.

## Migration Plan

No aplica migración de datos (no hay tabla/almacenamiento local nuevo). Despliegue es el flujo normal del proyecto: build, verificar con `DummyData:Enabled = true`, luego contra backend real. No hay rollback especial — es código nuevo aditivo (nuevas rutas, nuevo servicio, nuevos NavLink); no modifica pantallas ni servicios existentes.

## Open Questions

- ¿`Crm.WebApi` está realmente detrás del mismo `Api:BaseUrl`/`ApiKey` que `CreditSystem.Api`, o necesita config propia? (bloqueante solo en tiempo de integración contra backend real, no bloquea el desarrollo con Dummy Data Mode)
- ¿Cuál es el catálogo real (y casing exacto) de `ruleType`, `targetField` y `status` de matriz? Confirmar con el equipo de backend/CRM antes de ir a producción.
- ¿El header de autenticación (`ApiKey`) es el mismo esquema ya asumido en `ApiSettings` (`X-Api-Key`) para `Crm.WebApi`, o distinto?
