# Handoff a frontend: mantenimiento del RiskEngine (CRM)

Este documento reemplaza el recorte de la **Task 0.2** (`IRiskEngineService` con solo 4 endpoints). El backend ya expone el mantenimiento completo. Todo lo de aquí está verificado contra el swagger vivo y contra el gateway.

## Regla principal: las matrices no se editan, se versionan

- **No existe `PUT /risk-matrices/{id}`** (responde 405). No modelar "editar matriz".
- "Editar" una matriz = `POST /risk-matrices/{id}/new-version`. Crea un **Draft** nuevo con `version = origen + 1` y un `id` distinto. El origen no cambia hasta que ese Draft se **active**; en ese momento el origen pasa a `Superseded`.
- Formulario de edición: prellenarlo con `GET /risk-matrices/{id}`, enviar solo lo que cambió a `new-version`, y usar el `id` del Draft devuelto para seguir (activar o borrar).
- Estados: `Draft` → `Active` → `Superseded`. Solo un `Draft` se puede activar (`activate`) o borrar.
- Las reglas se pueden modificar o borrar **solo si ninguna matriz vigente las usa** (ver abajo). Para cambiar una regla usada por una matriz `Active`: crear una regla nueva y una nueva versión de la matriz que la use.

## Acceso

- Base: el **gateway**, no el servicio directo. Local: `http://localhost:5000`.
- Prefijo: `/api/v1/...`
- Swagger/Scalar del CRM: `/swagger/crm/v1/swagger.json` (documento "CRM" en Scalar).
- Todas las rutas van bajo el tag `RiskEngine`.

## Rutas

| Método | Ruta | Body | Éxito | Errores |
|---|---|---|---|---|
| GET | `/risk-rules` | – | 200 `RiskRuleDto[]` | – |
| GET | `/risk-rules/{id}` | – | 200 `RiskRuleDto` | 404 |
| POST | `/risk-rules` | `CreateRiskRuleDto` | 201 `RiskRuleDto` | 400 |
| PUT | `/risk-rules/{id}` | `UpdateRiskRuleDto` | 200 `RiskRuleDto` | 400, 404, **409** |
| DELETE | `/risk-rules/{id}` | – | 204 | 404, **409** |
| GET | `/risk-matrices?status=` | – | 200 `RiskMatrixDto[]` | 400 (status inválido) |
| GET | `/risk-matrices/{id}` | – | 200 `RiskMatrixDetailDto` | 404 |
| POST | `/risk-matrices` | `CreateRiskMatrixDto` | 201 `RiskMatrixDto` | 400, **404**, 422 |
| POST | `/risk-matrices/{id}/new-version` | `CreateRiskMatrixVersionDto` (opcional) | 201 `RiskMatrixDetailDto` + `Location` | 400, 404 |
| POST | `/risk-matrices/{id}/activate` | – | 204 | 404, **422** |
| DELETE | `/risk-matrices/{id}` | – | 204 | 404, **422** |
| POST | `/risk-evaluations` | `TriggerRiskEvaluationDto` | 201 `RiskEvaluationDto` | 400, 422 (sin cambios en este change) |

`status` acepta `Draft`, `Active` o `Superseded` (sin filtro = todas).

## DTOs

```ts
type RiskRuleDto = {
  id: string; name: string; ruleType: string; targetField: string;
  parameters: Record<string, string>;   // valores siempre string
  weight: number; createdAt: string;
};
// ruleType: "RangeCheck" | "ThresholdCheck" | "EnumCheck"

type CreateRiskRuleDto = {              // igual que UpdateRiskRuleDto
  name: string; ruleType: string; targetField: string;
  parameters: Record<string, string>; weight: number;   // weight > 0
};

type RiskMatrixDto = {                  // listado (sin reglas ni bandas)
  id: string; name: string; version: number;
  status: "Draft" | "Active" | "Superseded";
  autoApproveThreshold: number; autoRejectThreshold: number; createdAt: string;
};

type PricingBandDto = { minScore: number; maxScore: number; interestRate: number; maxAmount: number };

type RiskMatrixRuleDto = {
  ruleId: string; order: number; name: string;
  ruleType: string; targetField: string; weight: number;
};

type RiskMatrixDetailDto = RiskMatrixDto & {
  rules: RiskMatrixRuleDto[];           // ordenadas por `order`
  pricingBands: PricingBandDto[];
};

type CreateRiskMatrixDto = {
  name: string; autoApproveThreshold: number; autoRejectThreshold: number;
  ruleIds: string[]; pricingBands?: PricingBandDto[];   // pricingBands es nuevo y opcional
};

// Todo opcional: lo omitido (null) se copia de la matriz origen.
type CreateRiskMatrixVersionDto = {
  name?: string; autoApproveThreshold?: number; autoRejectThreshold?: number;
  ruleIds?: string[]; pricingBands?: PricingBandDto[];
};
```

`POST .../new-version` acepta body vacío (crea una copia exacta con `version + 1`).

## Formato de errores

Todos los errores son `ProblemDetails`:

```json
{
  "title": "RiskRule.InUse",
  "status": 409,
  "code": "RiskRule.InUse",
  "detail": "Risk rule '019ec957-…' cannot be deleted because it is referenced by 1 risk matrix(es): 'Matriz v1' (version 1, Active)",
  "traceId": "00-…"
}
```

- Usar `code` para la lógica y `detail` como mensaje al usuario.
- Los errores de validación (400 por campos) traen además `errors`.
- **Cambio**: `POST /risk-matrices` y `POST /risk-matrices/{id}/activate` antes devolvían el objeto de error crudo (`{code, description, type}`) con 400 para todo. Ahora usan este formato: `activate` responde **404** si la matriz no existe y **422** (`RiskMatrix.NotDraft`) si no está en `Draft`; `POST /risk-matrices` puede responder **404** si un `ruleId` no existe. El mensaje va en `detail`, ya no en `description`.

### Códigos

| `code` | HTTP | Cuándo |
|---|---|---|
| `RiskRule.NotFound` | 404 | regla inexistente (también en `ruleIds` de matrices) |
| `RiskRule.InvalidWeight` | 400 | `weight <= 0` |
| `RiskRule.UnknownType` | 400 | `ruleType` no soportado |
| `RiskRule.InUse` | **409** | `DELETE` de una regla referenciada por **cualquier** matriz (Draft, Active o Superseded) |
| `RiskRule.NotEditableInUse` | **409** | `PUT` de una regla que está en una matriz `Active` o `Superseded` |
| `RiskMatrix.NotFound` | 404 | matriz inexistente |
| `RiskMatrix.OverlappingThresholds` | 400 | `autoApproveThreshold <= autoRejectThreshold` |
| `RiskMatrix.NoRules` | 400 | matriz sin reglas |
| `RiskMatrix.InvalidPricingBand` | 400 | una banda con `minScore > maxScore` |
| `RiskMatrix.NotEditable` | **422** | `DELETE` de una matriz `Active` o `Superseded` |
| `RiskMatrix.NotDraft` | **422** | `activate` de una matriz que no está en `Draft` (incluida una ya `Active`) |

### Reglas de bloqueo de reglas

| Acción | Bloquea si la regla está en… |
|---|---|
| `PUT /risk-rules/{id}` | matriz `Active` o `Superseded` (solo `Draft` o sin matriz: permitido) |
| `DELETE /risk-rules/{id}` | **cualquier** matriz, incluso `Draft` |

El `detail` del 409 de `DELETE` lista cada matriz con nombre, versión y estado. Mostrarlo tal cual en un diálogo. Sugerencia de UX: deshabilitar "Eliminar" y mostrar el motivo cuando se sepa que la regla está en uso, aunque el backend es la fuente de verdad.

## Estado actual de los datos (BD de desarrollo)

- Las 5 reglas existentes pertenecen a **"Matriz v1" (Active)**. Hoy todas responden 409 a `PUT` y `DELETE`.
- Para probar un borrado exitoso: crear una regla nueva (`POST /risk-rules`) y borrarla sin asociarla a ninguna matriz.
- `DELETE` de una matriz `Draft` elimina sus asociaciones pero **conserva las reglas**.

## Flujo típico de "editar matriz"

1. `GET /risk-matrices?status=Active` → elegir la matriz.
2. `GET /risk-matrices/{id}` → prellenar el formulario.
3. `POST /risk-matrices/{id}/new-version` con los cambios → `201`, nuevo `Draft` (`version + 1`).
4. (Opcional) revisar el Draft con `GET /risk-matrices/{draftId}`; si se equivocó, `DELETE /risk-matrices/{draftId}` y repetir.
5. `POST /risk-matrices/{draftId}/activate` → `204`. La anterior queda `Superseded`.
