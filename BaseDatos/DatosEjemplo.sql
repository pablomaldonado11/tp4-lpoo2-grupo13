-- Datos de prueba para probar Ventas, listados y resúmenes.
-- Se ejecuta DESPUÉS de CrearTablas.sql (que ya carga productos y vendedores).
-- Es seguro correrlo varias veces: solo inserta lo que todavía no existe.

IF OBJECT_ID('Proveedor', 'U') IS NULL
BEGIN
    CREATE TABLE Proveedor
    (
        CUIT VARCHAR(11) NOT NULL PRIMARY KEY,
        RazonSocial VARCHAR(80) NOT NULL,
        Domicilio VARCHAR(100) NOT NULL,
        Telefono VARCHAR(15) NOT NULL
    );
END;

IF NOT EXISTS (SELECT 1 FROM Cliente WHERE DNI = '30111222')
    INSERT INTO Cliente VALUES ('30111222', 'Rodríguez', 'María', 'Belgrano 120');
IF NOT EXISTS (SELECT 1 FROM Cliente WHERE DNI = '28999888')
    INSERT INTO Cliente VALUES ('28999888', 'Fernández', 'Carlos', 'San Martín 450');
IF NOT EXISTS (SELECT 1 FROM Cliente WHERE DNI = '35444555')
    INSERT INTO Cliente VALUES ('35444555', 'López', 'Lucía', 'Alvear 78');

IF NOT EXISTS (SELECT 1 FROM Proveedor WHERE CUIT = '30711111119')
    INSERT INTO Proveedor VALUES ('30711111119', 'Maderas del Norte SA', 'Ruta 9 km 12', '3884123456');
IF NOT EXISTS (SELECT 1 FROM Proveedor WHERE CUIT = '30722222220')
    INSERT INTO Proveedor VALUES ('30722222220', 'Herrajes Jujuy SRL', 'Av. Bolivia 900', '3884987654');

IF NOT EXISTS (SELECT 1 FROM Venta)
BEGIN
    INSERT INTO Venta VALUES (1, GETDATE() - 10, 'V001', '30111222', 'P001', 85000, 1, 85000);
    INSERT INTO Venta VALUES (2, GETDATE() - 5,  'V002', '28999888', 'P002', 25000, 4, 100000);
    INSERT INTO Venta VALUES (3, GETDATE() - 2,  'V001', '35444555', 'P003', 140000, 1, 140000);
    INSERT INTO Venta VALUES (4, GETDATE(),      'V002', '30111222', 'P002', 25000, 2, 50000);
END;
