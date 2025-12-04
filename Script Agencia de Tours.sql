create database AgenciadeTours
use AgenciadeTours

INSERT INTO Paises (Nombre)
VALUES ('República Dominicana'), ('México'), ('España')

INSERT INTO Destinos (Nombre, PaisID, Dias_Duracion, Horas_Duracion)
VALUES ('Punta Cana', 2, 5, 6), ('Cancún Hotel Zone', 2, 4, 8), ('Madrid Centro Histórico', 3, 3, 4)

INSERT INTO Tours (Nombre, PaisID, DestinoID, Fecha, Hora, Precio)
VALUES ('Tour Playa Bávaro Premium', 1, 1, '2025-07-15', '09:00', 150.00), ('Tour Isla Mujeres Deluxe', 2, 2, '2025-08-10', '08:30', 220.00),
('Tour Museo del Prado & Caminata Histórica', 3, 3, '2025-09-05', '10:00', 180.00)

UPDATE Paises
SET Nombre = 'Italia'
WHERE PaisID = '1';

UPDATE Paises
SET PaisID = '1'
WHERE Nombre = 'República Dominicana';

select * from Paises
select * from Destinos
select * from Tours

DELETE FROM Paises
DELETE FROM Paises WHERE Nombre = 'República Dominicana'

DELETE FROM Destinos
DELETE FROM Destinos WHERE Nombre = 'Cancún Hotel Zone'

DELETE FROM Tours
DELETE FROM Tours WHERE Nombre = 'Tour Playa Bávaro Premium'