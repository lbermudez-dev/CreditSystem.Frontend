## ADDED Requirements

### Requirement: Disparar una evaluación de riesgo de prueba
El sistema SHALL permitir, desde `/crm/riesgo/evaluaciones`, ingresar un `creditApplicationId` (GUID) y disparar `POST /api/v1/risk-evaluations`, mostrando el resultado devuelto por el backend como herramienta de validación de la matriz de riesgo actualmente activa.

#### Scenario: Evaluación exitosa
- **WHEN** el usuario ingresa un `creditApplicationId` válido y confirma
- **THEN** el sistema envía `POST /api/v1/risk-evaluations` y, ante `200`/`201`, muestra el resultado (`outcome`, `totalScore`, tasa de interés sugerida, monto máximo sugerido, `riskMatrixId` y `riskMatrixVersion` usados) en la misma pantalla

#### Scenario: Detalle de ScoreCard
- **WHEN** el resultado de la evaluación incluye entradas de ScoreCard (`entries`)
- **THEN** la pantalla muestra una tabla con, por cada entrada, el nombre de la regla, el campo objetivo, el valor observado, si pasó o no (`passed`) y su contribución ponderada al puntaje total

#### Scenario: Evaluación rechazada por el backend
- **WHEN** el backend responde `400`/`422` (por ejemplo, `creditApplicationId` inexistente o sin matriz activa)
- **THEN** la pantalla muestra el error devuelto sin dejar la interfaz en un estado de carga indefinido

#### Scenario: Trazabilidad de la matriz usada
- **WHEN** se muestra el resultado de una evaluación exitosa
- **THEN** la pantalla deja visible qué matriz y versión (`riskMatrixId`/`riskMatrixVersion`) se usó, dado que el endpoint no permite elegir la matriz explícitamente y siempre usa la que esté `Active`
