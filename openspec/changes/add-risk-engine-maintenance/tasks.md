## 1. Enums

- [x] 1.1 Crear `Models/Enums/RiskRuleType.cs` (`MinThreshold`, `MaxThreshold`, `Range`, `Blacklist`, `Whitelist`) con `[JsonConverter(typeof(JsonStringEnumConverter))]` y comentario `DOCUMENTACION INSUFICIENTE` (mismo formato que `GuaranteeType.cs`)
- [x] 1.2 Crear `Models/Enums/RiskRuleTargetField.cs` (`CreditScore`, `MonthlyIncome`, `MonthlyDebt`, `Salary`) con el mismo patrón
- [x] 1.3 Crear `Models/Enums/RiskMatrixStatus.cs` (`Draft`, `Active`, `Archived`) con el mismo patrón

## 2. Modelos: Common y Requests

- [x] 2.1 Crear `Models/Common/PricingBand.cs` (`MinScore`, `MaxScore`, `InterestRate`, `MaxAmount`)
- [x] 2.2 Crear `Models/Requests/CreateRiskRuleRequest.cs` (`Name`, `RuleType`, `TargetField`, `Parameters: Dictionary<string,string>?`, `Weight`)
- [x] 2.3 Crear `Models/Requests/UpdateRiskRuleRequest.cs` (mismo shape que create)
- [x] 2.4 Crear `Models/Requests/CreateRiskMatrixRequest.cs` (`Name`, `AutoApproveThreshold`, `AutoRejectThreshold`, `RuleIds: List<Guid>?`, `PricingBands: List<PricingBand>?`)
- [x] 2.5 Crear `Models/Requests/UpdateRiskMatrixRequest.cs` (mismo shape que create)
- [x] 2.6 Crear `Models/Requests/TriggerRiskEvaluationRequest.cs` (`CreditApplicationId: Guid`)

## 3. Modelos: Responses

- [x] 3.1 Crear `Models/Responses/RiskRuleResponse.cs` (`Id`, `Name`, `RuleType`, `TargetField`, `Parameters`, `Weight`, `CreatedAt`)
- [x] 3.2 Crear `Models/Responses/RiskMatrixResponse.cs` (`Id`, `Name`, `Version`, `Status`, `AutoApproveThreshold`, `AutoRejectThreshold`, `CreatedAt`)
- [x] 3.3 Crear `Models/Responses/RiskMatrixRuleDetailResponse.cs` (`RuleId`, `RuleName`, `RuleType`, `TargetField`, `Weight`, `Order`)
- [x] 3.4 Crear `Models/Responses/RiskMatrixDetailResponse.cs` (todo lo de `RiskMatrixResponse` + `Rules: List<RiskMatrixRuleDetailResponse>?` + `PricingBands: List<PricingBand>?`)
- [x] 3.5 Crear `Models/Responses/ScoreCardEntryResponse.cs` (`RuleId`, `RuleName`, `TargetField`, `ObservedValue`, `Passed`, `WeightedContribution`)
- [x] 3.6 Crear `Models/Responses/RiskEvaluationResponse.cs` (`Id`, `CreditApplicationId`, `RiskMatrixId`, `RiskMatrixVersion`, `TotalScore`, `Outcome`, `SuggestedInterestRate`, `SuggestedMaxAmount`, `EvaluatedAt`, `Entries: List<ScoreCardEntryResponse>?`)

## 4. Servicio

- [x] 4.0 (Descubierta durante implementación) `IApiClient` no tenía soporte para `DELETE` — se agregó `DeleteAsync(string requestUri, CancellationToken ct)` a `IApiClient`, `ApiClient` (via `HttpClient.DeleteAsync` + `ReadResponseAsync`) y `DummyApiClient` (mismo patrón que `PostAsync`/`PutAsync` sin body)
- [x] 4.1 Crear `Services/Api/IRiskEngineService.cs` con los 12 métodos (ver design.md §Decisions) devolviendo `ApiResult<T>`/`ApiResult`
- [x] 4.2 Implementar `Services/Api/RiskEngineService.cs` delegando en `IApiClient` existente, armando rutas `api/v1/risk-rules*`, `api/v1/risk-matrices*`, `api/v1/risk-evaluations`
- [x] 4.3 Registrar `builder.Services.AddScoped<IRiskEngineService, RiskEngineService>();` en `Program.cs`

## 5. Pantallas: Reglas de riesgo

- [x] 5.1 Crear `Components/Pages/Crm/Riesgo/ReglasListado.razor` (`@page "/crm/riesgo/reglas"`) — `DataTable` + botón "Nueva regla"
- [x] 5.2 Crear `Components/Pages/Crm/Riesgo/ReglaNueva.razor` (`@page "/crm/riesgo/reglas/nuevo"`) — formulario con editor dinámico de `parameters` (agregar/quitar filas clave/valor)
- [x] 5.3 Crear `Components/Pages/Crm/Riesgo/ReglaDetalle.razor` (`@page "/crm/riesgo/reglas/{Id:guid}"`) — detalle editable inline (`PUT`) + eliminar con `ConfirmDialog`

## 6. Pantallas: Matrices de riesgo

- [x] 6.1 Crear `Components/Pages/Crm/Riesgo/MatricesListado.razor` (`@page "/crm/riesgo/matrices"`) — `DataTable` con `StatusBadge` de `RiskMatrixStatus`
- [x] 6.2 Crear `Components/Pages/Crm/Riesgo/MatrizNueva.razor` (`@page "/crm/riesgo/matrices/nuevo"`) — carga `ListRiskRulesAsync` para selección múltiple de `ruleIds`, tabla dinámica de `pricingBands`
- [x] 6.3 Crear `Components/Pages/Crm/Riesgo/MatrizDetalle.razor` (`@page "/crm/riesgo/matrices/{Id:guid}"`) — detalle con reglas asociadas y bandas de precio, edición inline (`PUT`), botón "Activar" (`ConfirmDialog` + `POST .../activate`), eliminar (`ConfirmDialog` + `DELETE`)

## 7. Pantalla: Evaluación de prueba

- [x] 7.1 Crear `Components/Pages/Crm/Riesgo/EvaluacionPrueba.razor` (`@page "/crm/riesgo/evaluaciones"`) — formulario `creditApplicationId` → `POST risk-evaluations` → resultado + tabla ScoreCard, mostrando `riskMatrixId`/`riskMatrixVersion` usados

## 8. Navegación

- [x] 8.1 Agregar separador + 3 `NavLink` nuevos en `Components/Layout/AppSidebar.razor` agrupados visualmente como "CRM" (Reglas de Riesgo → `/crm/riesgo/reglas`, Matrices de Riesgo → `/crm/riesgo/matrices`, Evaluación de Riesgo → `/crm/riesgo/evaluaciones`)

## 9. Documentación y verificación

- [x] 9.1 Actualizar `Src/CreditSystem.Frontend.Web/README/frontend-overview.md`: agregar las 3 rutas nuevas al "Mapa de pantallas → endpoints" y una nota sobre el supuesto de gateway compartido con `Crm.WebApi`
- [x] 9.2 Agregar a la tabla "Incertidumbres pendientes de confirmar con el backend" las 3 entradas de catálogo de enums (`RiskRuleType`, `RiskRuleTargetField`, `RiskMatrixStatus`) y el supuesto del gateway compartido
- [x] 9.3 Verificar `dotnet build` sin errores
- [~] 9.4 Probar las 7 pantallas nuevas con `DummyData:Enabled = true` — verificado por `curl` que las 7 rutas cargan (200) y renderizan el contenido esperado (tablas, badges, selects, checkboxes), más regresión en `/`, `/riesgo`, `/productos`. **Pendiente**: click-through interactivo real (crear/editar/eliminar/activar/evaluar) — no se pudo probar en navegador en esta sesión (Claude in Chrome no disponible). Servidor dev quedó corriendo en `http://localhost:5266` con `DummyData:Enabled=true` para que el usuario lo verifique manualmente
