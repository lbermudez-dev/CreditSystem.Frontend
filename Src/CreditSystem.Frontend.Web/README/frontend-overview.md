# Frontend Architecture Overview

> Documento de referencia del Frontend (Blazor). Léelo antes de tocar código: describe la arquitectura, el patrón de diseño y las convenciones que **todas** las pantallas deben seguir para mantener consistencia. Actualízalo cada vez que se agregue una pantalla, un servicio o se confirme algo que hoy está marcado como "documentación insuficiente".

---

## Stack

| Capa | Tecnología |
|---|---|
| Framework | Blazor Web App (.NET 9), render mode **Interactive Server** |
| UI | Bootstrap 5.3.3 (local, `wwwroot/lib/bootstrap`) + iconos Material Symbols (Google Fonts CDN) |
| Estilos propios | CSS custom properties (`wwwroot/css/app.css`, prefijo `cace-*`) traducidos de los HTML de referencia (Tailwind) |
| Comunicación con backend | `HttpClientFactory` vía un único `IApiClient` centralizado — sin librerías de terceros (no MudBlazor/Radzen) |
| Autenticación | ApiKey de servicio a servicio (header configurable), sin login de usuario |

No hay dependencias NuGet adicionales fuera del SDK Web — decisión explícita para no traer una librería de componentes que choque con Bootstrap.

---

## Por qué existe este documento

El Frontend se construyó **exclusivamente** a partir de tres fuentes de verdad:

1. `Backend/open_api.json` — el contrato real de los 58 endpoints (rutas, verbos, shapes).
2. `Backend/*.md` — la documentación funcional (`backend-overview.md`, `payment-flow.md`, `credit-system-presentacion-ejecutiva.md`).
3. `Frontend/Referencia/*.html` — el diseño visual objetivo (`Dash.html`, `Creditos.html`, `ExpedientePersona.html`), maquetados en Tailwind.

Regla de oro que gobierna todo el proyecto: **no inventar funcionalidad que no esté respaldada por un endpoint documentado**. Cuando el swagger no confirma algo (un catálogo de valores, un formato, una moneda), el código lo señala explícitamente con un comentario `DOCUMENTACION INSUFICIENTE` en vez de adivinar en silencio. Ver la sección [Incertidumbres pendientes](#incertidumbres-pendientes-de-confirmar-con-el-backend).

---

## Estructura de carpetas

```
Frontend/
├── frontend-overview.md          ← este documento
├── Referencia/                   ← HTML de diseño objetivo (Tailwind, solo de referencia visual)
│   ├── Dash.html
│   ├── Creditos.html
│   └── ExpedientePersona.html
└── Frontend/Src/CreditSystem.Frontend.Web/
    ├── Program.cs                 ← composición de servicios (DI), incluye el switch dummy/real
    ├── appsettings.json / appsettings.Development.json
    ├── Components/
    │   ├── App.razor              ← shell HTML, referencias a Bootstrap/fuentes
    │   ├── Routes.razor           ← router + página 404
    │   ├── Layout/                ← AppHeader, AppSidebar, AppFooter, MainLayout
    │   ├── Common/                ← componentes reutilizables (ver tabla abajo)
    │   ├── Tables/DataTable.razor ← tabla genérica con loading/empty/error
    │   └── Pages/                 ← una carpeta por módulo de negocio (Dashboard, Socios, Creditos, ...)
    ├── Models/
    │   ├── Requests/               ← 1 clase por request body del swagger
    │   ├── Responses/              ← 1 clase por response body del swagger
    │   ├── Enums/                  ← enums del swagger (valor entero = contrato real)
    │   └── Common/                 ← ApiResult<T>, ApiError, DocumentFile, tipos compartidos
    ├── Services/Api/
    │   ├── IApiClient.cs / ApiClient.cs         ← único punto que habla HTTP
    │   ├── I<Grupo>Service.cs / <Grupo>Service.cs ← uno por grupo de endpoints del swagger
    │   └── Dummy/                                ← DummyApiClient + DummyDataFactory (modo demo)
    ├── Helpers/CurrencyFormatter.cs
    └── wwwroot/
        ├── css/app.css             ← design tokens `cace-*`
        └── lib/bootstrap/          ← Bootstrap local (restaurado manualmente, ver nota abajo)
```

> **Nota operativa:** `libman.json` declara Bootstrap pero **no se restaura solo** en este entorno (no hay `libman restore` automático al compilar). Si `wwwroot/lib` aparece vacío y el layout se ve sin estilos, es la causa: volver a descargar `bootstrap.min.css[.map]` y `bootstrap.bundle.min.js[.map]` (versión 5.3.3) a `wwwroot/lib/bootstrap/{css,js}/`.

---

## Patrón de arquitectura: 3 capas estrictas

```
Components/Pages/*.razor
        │  solo conoce interfaces I<Grupo>Service (inyectadas por DI)
        ▼
Services/Api/<Grupo>Service.cs
        │  solo conoce IApiClient — arma la ruta, llama Get/Post/Put, mapea Request→Response
        ▼
Services/Api/ApiClient.cs (o Dummy/DummyApiClient.cs)
        │  único lugar que serializa JSON, agrega headers, interpreta códigos de error HTTP
        ▼
HttpClient nombrado "CreditSystemApi" → backend real
```

**Reglas que no se rompen:**

1. **Una página nunca inyecta `HttpClient` ni `IApiClient` directamente** — siempre una interfaz de servicio (`ILoanService`, `IMemberService`, etc.). Esto es lo que permite que el modo demo funcione sin tocar páginas (ver más abajo).
2. **Un servicio por grupo de endpoints del swagger**, no un servicio gigante. El nombre del grupo en `open_api.json` (tags) define el nombre del servicio: grupo "Loan Contracts" → `ILoanService`, grupo "Cooperative Members" → `IMemberService`, etc.
3. **Todo servicio devuelve `ApiResult<T>` o `ApiResult`** (nunca lanza excepciones de red hacia la página). La página solo distingue `IsSuccess` / `Data` / `Error` — nunca hace `try/catch` de HTTP.
4. **Los modelos (`Models/Requests`, `Models/Responses`, `Models/Enums`) son un espejo 1:1 del swagger.** Llevan el comentario `// <auto-generated> ... no editar a mano sin resincronizar` porque conceptualmente están generados desde `open_api.json`. Si el swagger cambia, estos archivos son los primeros en actualizarse.
5. **Cada página sigue la misma forma interna**: `_isLoading` / `ApiError? _error` / `TData? _data` por cada fuente de datos que carga, un `LoadXxxAsync()` por cada una, y renderizado condicional `LoadingIndicator → ErrorState → contenido`. Ver [receta para nuevas pantallas](#receta-para-agregar-una-pantalla-nueva).

---

## Componentes comunes (no los reinventes)

| Componente | Uso |
|---|---|
| `PageHeader` | Encabezado de página: `Title`, `Icon`, `Breadcrumb`, `Subtitle` (normalmente la ruta del endpoint que consume) y `Actions` (botones a la derecha) |
| `KpiCard` | Tarjeta de indicador numérico. `Accent`: `secondary` \| `success` \| `warning` \| `danger` |
| `StatusBadge` | Traduce el campo `status` (string libre del backend) a una etiqueta en español con color. Editar su `switch` si aparece un estado nuevo |
| `DataTable<TItem>` | Tabla genérica: recibe `Items`, `IsLoading`, `Error`, `OnRetry`, y templates `HeaderTemplate`/`RowTemplate`. Maneja loading/empty/error sin que la página repita esa lógica |
| `ConfirmDialog` | Modal de confirmación para acciones destructivas/irreversibles (`IsVisible`, `OnConfirm`, `OnCancel`, `IsBusy`) |
| `LoadingIndicator` / `ErrorState` / `EmptyState` | Estados de página fuera de una `DataTable` (ej. una card de detalle) |
| `Pagination` | Paginación client-side (el swagger no confirma paginación server-side — ver `PagedResponse.cs`) |

Clases CSS del sistema de diseño (`wwwroot/css/app.css`): `cace-card`, `cace-kpi-card`/`cace-accent-*`, `cace-badge`/`cace-badge-{neutral,info,success,warning,danger}`, `cace-table`/`cace-table-mono` (para IDs y montos), `cace-text-primary`, `cace-bg-primary`. **Usa siempre estas clases en vez de estilos inline** para que un cambio de paleta se haga en un solo archivo.

---

## Convención de rutas

```
/<módulo>                    → listado / buscador          (ej. /creditos, /morosos, /productos)
/<módulo>/nuevo               → formulario de creación       (ej. /creditos/nuevo, /socios/nuevo)
/<módulo>/{Id:guid}           → detalle                      (ej. /creditos/{LoanId:guid})
```

El `AppSidebar` solo muestra como **enlace activo** los módulos con endpoint real. Los módulos sin endpoint documentado (Ahorros, Caja, Reportes) se dejan visibles pero deshabilitados, por fidelidad visual con `Referencia/Dash.html` — **no se les agrega ruta ni página hasta que exista el endpoint**.

---

## Modo Datos Dummy (para maquetar sin backend)

Problema que resuelve: iterar el layout/diseño de una pantalla sin depender de que el backend real esté arriba.

**Cómo funciona:**

- `Services/Api/Dummy/DummyDataFactory.cs` — genera por **reflexión** una instancia plausible de cualquier `Response` (o lista de ellas) leyendo sus propiedades: si el nombre sugiere su significado (`Amount`, `Rate`, `Status`, `Email`, `Currency`, `Overdue`...) usa un valor coherente con ese significado; si no, un valor genérico del tipo correcto. No hace falta escribir un fixture por endpoint.
- `Services/Api/Dummy/DummyApiClient.cs` — implementa `IApiClient` (la misma interfaz que `ApiClient`) y responde con `DummyDataFactory` en vez de llamar HTTP, simulando 150–400ms de latencia.
- **Interruptor único**: `appsettings*.json → "DummyData": { "Enabled": true|false }`. En `Program.cs`, una sola condición decide si el contenedor de DI resuelve `IApiClient` como `DummyApiClient` o como `ApiClient` real.
- Cuando está activo, el header muestra el badge **"Modo Demo · Datos de Prueba"** (`AppHeader.razor`) para que nunca se confunda con datos reales.

**Por diseño, ninguna página ni servicio de dominio sabe que existe el modo demo** — todos dependen de `I<Grupo>Service` → `IApiClient`, nunca de la implementación concreta. Pasar a producción es cambiar `DummyData:Enabled` a `false`; cero cambios de código.

**Limitación aceptada:** cada llamada genera datos independientes (sin "base de datos" simulada detrás), así que un préstamo en un listado no tendrá el mismo saldo que en su pantalla de detalle. Es una herramienta para ajustar layout, no para simular reglas de negocio de extremo a extremo. Si en algún momento se necesita coherencia referencial entre pantallas, hay que reemplazar `DummyDataFactory` por fixtures explícitos por escenario (ese es un cambio localizado, no toca el resto de la arquitectura).

---

## Receta para agregar una pantalla nueva

Seguir este orden mantiene el patrón consistente:

1. **Modelos**: si el endpoint no tiene `Request`/`Response` en `Models/`, crearlos ahí (mismo shape que `open_api.json`, comentario `<auto-generated>`).
2. **Servicio**: agregar el método a la interfaz `I<Grupo>Service` existente (o crear el par interfaz/implementación si es un grupo nuevo) devolviendo `ApiResult<T>`/`ApiResult`. La implementación solo arma la ruta (`api/v1/...`) y delega en `IApiClient`.
3. **Registro en DI**: agregar `builder.Services.AddScoped<I<Grupo>Service, <Grupo>Service>();` en `Program.cs` si es un servicio nuevo.
4. **Página**: crear `Components/Pages/<Módulo>/<Nombre>.razor` con `@page "/ruta"`, inyectar la interfaz de servicio, y componer con `PageHeader` + `DataTable`/`KpiCard`/`ConfirmDialog` según corresponda. Seguir el patrón `_isLoading`/`_error`/`_data` + `LoadAsync()` de cualquier página existente (ej. `Riesgo/Index.razor` es un buen ejemplo corto).
5. **Navegación**: agregar el `NavLink` en `AppSidebar.razor` (o un botón "Ir a..." desde la pantalla relacionada, como se hizo con `/socios/nuevo` desde `/socios`).
6. **Si algo del swagger no está claro** (catálogo de valores, formato, unidad): no adivinar en silencio — dejar un comentario `DOCUMENTACION INSUFICIENTE` explicando la suposición tomada y qué hay que confirmar.
7. **Verificar**: `dotnet build` sin errores, y probar la pantalla con `DummyData:Enabled = true` para validar el layout antes de que el endpoint real esté disponible.

---

## Mapa de pantallas → endpoints

| Ruta | Página | Endpoints del swagger |
|---|---|---|
| `/` | `Dashboard/Index.razor` | `GET delinquent-loans`, `GET loans/defaulted`, `GET loans/paid-off` |
| `/socios` | `Socios/Buscar.razor` | (buscador por GUID — no hay listado general) |
| `/socios/nuevo` | `Socios/Nuevo.razor` | `POST members/enroll` |
| `/socios/{id}` | `Socios/Expediente.razor` | `GET members/{id}`, `/shares`, `/social-capital`, `GET loans/customer/{id}`, `GET revolving-credits/customer/{id}` |
| `/creditos` | `Creditos/Listado.razor` | `GET loans/customer/{externalCustomerId}` |
| `/creditos/nuevo` | `Creditos/Simulador.razor` | `POST loans`, `GET products` |
| `/creditos/{id}` | `Creditos/Detalle.razor` | `GET loans/{id}`, `/payments`, `/guarantees`, `POST disburse/payments/default/restructure/payoff`, `GET payoff-amount`, `POST guarantees`, `PUT guarantees/{id}/status`, `GET documents/*` |
| `/revolventes` | `Revolventes/Listado.razor` | `GET revolving-credits/customer/{id}`, `POST revolving-credits` |
| `/revolventes/{id}` | `Revolventes/Detalle.razor` | `GET revolving-credits/{id}`, `/transactions`, `/statements`, `POST activate/draw/payments/freeze/unfreeze/close` |
| `/morosos` | `Morosos/Listado.razor` | `GET delinquent-loans` (filtros `minDaysOverdue`, `collectionStatus`) |
| `/riesgo` | `Riesgo/Index.razor` | `GET loans/risk-summary`, `/risk-summary/{category}`, `PUT loans/{id}/risk-category` |
| `/productos` | `Productos/Listado.razor` | `GET/POST products`, `PUT products/{id}/status` |
| `/pagos` | `Pagos/Seguimiento.razor` | `POST payments`, `POST payments/revolving`, `GET payments/{id}/status` |
| `/admin` | `Admin/Index.razor` | `POST admin/jobs/*`, `admin/loans/{id}/accrue-interest`, `admin/revolving-credits/{id}/*` |
| `/tasas` | `Configuracion/Tasas.razor` | `GET/PUT reference-rates/{id}` |
| `/webhooks` | `Configuracion/Webhooks.razor` | `POST webhooks/subscribe` |

Cobertura actual: **58/58 endpoints documentados en `open_api.json` tienen pantalla.**

---

## Incertidumbres pendientes de confirmar con el backend

Todas están marcadas en el código con `DOCUMENTACION INSUFICIENTE` para que sean fáciles de encontrar (`grep -r "DOCUMENTACION INSUFICIENTE"`). Resumen:

| Tema | Dónde | Supuesto actual |
|---|---|---|
| Nombres de los enums (`AmortizationMethod`, `ProductStatus`, `GuaranteeType`, `GuaranteeStatus`) | `Models/Enums/*.cs` | El swagger solo expone el valor entero, sin `x-enumNames`. Los nombres son inferencia funcional — **el entero sí es el contrato real** |
| Valores posibles de `status` (string libre, sin enum) en `LoanSummaryResponse`, `RevolvingCreditSummaryResponse`, etc. | `StatusBadge.razor` | Se infirió el catálogo (`Approved`, `Active`, `Delinquent`, `Default`, `Restructured`, `PaidOff`, `Pending`, `Frozen`, `Closed`) de la documentación funcional |
| Header de autenticación (ApiKey) | `ApiSettings.cs` | Se asume `X-Api-Key`; podría ser `Authorization: ApiKey ...` u otro esquema |
| Moneda por defecto cuando el response no trae campo `currency` | `CurrencyFormatter.cs` | Se usa `NIO` (córdoba) por defecto |
| Paginación server-side en listados | `PagedResponse.cs` | Se asume arreglo plano (`IList<T>`), tal como documenta `backend-overview.md`; el wrapper paginado existe por si acaso |
| Formato válido del parámetro `format` en `GET .../documents/amortization-table` | `IDocumentService.cs` | Se asume `pdf` / `xlsx` |
| Catálogo de `EventType` para webhooks | `Configuracion/Webhooks.razor` | Inferido de la lista de eventos de `credit-system-presentacion-ejecutiva.md` (§12) |
| Literales exactos de `ProductResponse.Status` (`"Active"`/`"Inactive"` vs `"0"`/`"1"`) | `Productos/Listado.razor` | Se comparan ambas formas antes de alternar el estado |

Cuando el backend real confirme cualquiera de estos puntos, actualizar el archivo correspondiente **y borrar la fila de esta tabla**.

---

## Cómo correr en local

```bash
cd "Frontend/Frontend/Src/CreditSystem.Frontend.Web"
dotnet build
dotnet run
```

- Con `appsettings.Development.json` (`DummyData:Enabled = true`), la app arranca en modo demo sin necesitar backend.
- Para apuntar al backend real: poner `DummyData:Enabled = false` y completar `Api:BaseUrl` / `Api:ApiKey` (por `dotnet user-secrets` en desarrollo, no hardcodeado).
