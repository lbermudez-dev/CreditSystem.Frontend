## ADDED Requirements

### Requirement: Listar matrices de riesgo
El sistema SHALL mostrar en `/crm/riesgo/matrices` el listado de matrices de riesgo obtenido de `GET /api/v1/risk-matrices`, con nombre, versión, estado (`RiskMatrixStatus` vía `StatusBadge`), umbrales de auto-aprobación/auto-rechazo y fecha de creación.

#### Scenario: Listado carga correctamente
- **WHEN** el usuario navega a `/crm/riesgo/matrices` y el backend responde `200 OK`
- **THEN** la tabla muestra una fila por matriz con nombre, versión, badge de estado y ambos umbrales

#### Scenario: Error al listar
- **WHEN** la llamada a `GET /api/v1/risk-matrices` falla
- **THEN** la pantalla muestra `ErrorState` con opción de reintentar

### Requirement: Crear matriz de riesgo
El sistema SHALL permitir crear una matriz de riesgo desde `/crm/riesgo/matrices/nuevo`, cargando primero el catálogo de reglas existentes (`GET /api/v1/risk-rules`) para que el usuario seleccione cuáles asociar por `id` (`ruleIds`), y enviando `POST /api/v1/risk-matrices` con nombre, umbrales y bandas de precio (`pricingBands`) editables dinámicamente.

#### Scenario: Selección de reglas existentes
- **WHEN** el formulario de alta se carga
- **THEN** el sistema primero obtiene el listado de reglas de riesgo y las presenta como opciones seleccionables (multi-selección), sin permitir crear una regla nueva desde este formulario

#### Scenario: Alta exitosa
- **WHEN** el usuario completa nombre, umbrales, selecciona una o más reglas y confirma
- **THEN** el sistema envía `POST /api/v1/risk-matrices` con `ruleIds` correspondiente a las reglas seleccionadas y, ante éxito, redirige al listado de matrices

#### Scenario: Bandas de precio dinámicas
- **WHEN** el usuario agrega o quita filas de la tabla de `pricingBands` (rango de score, tasa de interés, monto máximo)
- **THEN** el arreglo `pricingBands` enviado en el `POST` refleja exactamente las filas visibles al confirmar

#### Scenario: Alta rechazada por el backend
- **WHEN** el backend responde `400`/`422` a la creación
- **THEN** el formulario permanece visible mostrando el error devuelto, sin perder los datos ingresados

### Requirement: Ver y editar el detalle de una matriz de riesgo
El sistema SHALL mostrar el detalle de una matriz en `/crm/riesgo/matrices/{id}` (`GET /api/v1/risk-matrices/{id}`), incluyendo las reglas asociadas (nombre, tipo, campo objetivo, peso, orden) y las bandas de precio, y permitir editarla inline (`PUT /api/v1/risk-matrices/{id}`).

#### Scenario: Detalle no encontrado
- **WHEN** el backend responde `404 Not Found`
- **THEN** la pantalla muestra un estado de error indicando que la matriz no existe

#### Scenario: Detalle muestra reglas asociadas
- **WHEN** el backend responde `200 OK` con una matriz que tiene reglas asociadas
- **THEN** la pantalla lista cada regla asociada con su nombre, tipo, campo objetivo, peso y orden, sin requerir una llamada adicional a `risk-rules`

#### Scenario: Edición exitosa
- **WHEN** el usuario modifica campos editables y confirma
- **THEN** el sistema envía `PUT /api/v1/risk-matrices/{id}` y, ante éxito, refleja los valores actualizados en la misma pantalla

### Requirement: Activar una matriz de riesgo
El sistema SHALL permitir activar una matriz desde su pantalla de detalle (`POST /api/v1/risk-matrices/{id}/activate`), solicitando confirmación explícita antes de enviar la solicitud por tratarse de una acción con efecto sobre qué matriz queda vigente.

#### Scenario: Activación requiere confirmación
- **WHEN** el usuario presiona "Activar" en el detalle de una matriz
- **THEN** el sistema muestra `ConfirmDialog` y solo envía `POST /api/v1/risk-matrices/{id}/activate` si el usuario confirma

#### Scenario: Activación exitosa
- **WHEN** el backend responde `200`/`204` a la activación confirmada
- **THEN** la pantalla de detalle refleja el nuevo estado (`Active`) de la matriz

#### Scenario: Activación rechazada
- **WHEN** el backend responde `400`/`422` a la activación
- **THEN** la pantalla muestra el error devuelto sin modificar el estado mostrado de la matriz

### Requirement: Eliminar una matriz de riesgo
El sistema SHALL permitir eliminar una matriz de riesgo desde su pantalla de detalle (`DELETE /api/v1/risk-matrices/{id}`) tras confirmación explícita.

#### Scenario: Eliminación requiere confirmación
- **WHEN** el usuario presiona "Eliminar" en el detalle de una matriz
- **THEN** el sistema muestra `ConfirmDialog` y solo envía `DELETE /api/v1/risk-matrices/{id}` si el usuario confirma

#### Scenario: Eliminación exitosa
- **WHEN** el backend responde `200`/`204` a la eliminación confirmada
- **THEN** el sistema redirige al listado de matrices de riesgo
