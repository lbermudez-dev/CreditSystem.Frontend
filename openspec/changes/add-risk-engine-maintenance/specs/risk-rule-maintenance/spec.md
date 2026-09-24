## ADDED Requirements

### Requirement: Listar reglas de riesgo
El sistema SHALL mostrar en `/crm/riesgo/reglas` el listado completo de reglas de riesgo obtenido de `GET /api/v1/risk-rules`, con nombre, tipo de regla, campo objetivo, peso y fecha de creación, usando el componente `DataTable` con sus estados de carga/vacío/error estándar.

#### Scenario: Listado carga correctamente
- **WHEN** el usuario navega a `/crm/riesgo/reglas` y el backend responde `200 OK` con una lista de reglas
- **THEN** la tabla muestra una fila por regla con nombre, tipo, campo objetivo, peso y fecha de creación

#### Scenario: Listado vacío
- **WHEN** el backend responde `200 OK` con una lista vacía
- **THEN** la pantalla muestra el estado vacío del `DataTable` en vez de una tabla sin filas

#### Scenario: Error al listar
- **WHEN** la llamada a `GET /api/v1/risk-rules` falla (error de red o HTTP no exitoso)
- **THEN** la pantalla muestra `ErrorState` con opción de reintentar, sin lanzar una excepción no controlada

### Requirement: Crear regla de riesgo
El sistema SHALL permitir crear una regla de riesgo desde `/crm/riesgo/reglas/nuevo`, enviando `POST /api/v1/risk-rules` con nombre, tipo de regla (`RiskRuleType`), campo objetivo (`RiskRuleTargetField`), peso y un conjunto de parámetros clave/valor editable dinámicamente.

#### Scenario: Alta exitosa
- **WHEN** el usuario completa el formulario con datos válidos y confirma
- **THEN** el sistema envía `POST /api/v1/risk-rules` y, ante una respuesta `200`/`201`, redirige al listado de reglas

#### Scenario: Alta rechazada por el backend
- **WHEN** el backend responde `400 Bad Request` a la creación
- **THEN** el formulario permanece visible mostrando el error devuelto, sin perder los datos ingresados

#### Scenario: Editor de parámetros dinámico
- **WHEN** el usuario agrega o quita filas del editor clave/valor de `parameters`
- **THEN** el conjunto de pares clave/valor enviado en el `POST` refleja exactamente las filas visibles al momento de confirmar

### Requirement: Ver, editar y eliminar una regla de riesgo
El sistema SHALL mostrar el detalle de una regla en `/crm/riesgo/reglas/{id}` (`GET /api/v1/risk-rules/{id}`), permitir editarla inline (`PUT /api/v1/risk-rules/{id}`) y eliminarla (`DELETE /api/v1/risk-rules/{id}`) tras confirmación explícita.

#### Scenario: Detalle no encontrado
- **WHEN** el backend responde `404 Not Found` para el `id` solicitado
- **THEN** la pantalla muestra un estado de error indicando que la regla no existe, sin intentar renderizar un formulario vacío

#### Scenario: Edición exitosa
- **WHEN** el usuario modifica campos del formulario precargado y confirma
- **THEN** el sistema envía `PUT /api/v1/risk-rules/{id}` y, ante éxito, refleja los valores actualizados en la misma pantalla

#### Scenario: Eliminación requiere confirmación
- **WHEN** el usuario presiona "Eliminar"
- **THEN** el sistema muestra `ConfirmDialog` y solo envía `DELETE /api/v1/risk-rules/{id}` si el usuario confirma la acción

#### Scenario: Eliminación exitosa
- **WHEN** el backend responde `200`/`204` a la eliminación confirmada
- **THEN** el sistema redirige al listado de reglas de riesgo
