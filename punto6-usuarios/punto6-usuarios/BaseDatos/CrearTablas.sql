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

IF OBJECT_ID('Cliente', 'U') IS NULL
BEGIN
    CREATE TABLE Cliente
    (
        DNI VARCHAR(8) NOT NULL PRIMARY KEY,
        Apellido VARCHAR(50) NOT NULL,
        Nombre VARCHAR(50) NOT NULL,
        Direccion VARCHAR(100) NOT NULL
    );
END;

IF OBJECT_ID('Producto', 'U') IS NULL
BEGIN
    CREATE TABLE Producto
    (
        CodProducto VARCHAR(20) NOT NULL PRIMARY KEY,
        Categoria VARCHAR(50) NOT NULL,
        Color VARCHAR(30) NOT NULL,
        Descripcion VARCHAR(100) NOT NULL,
        Precio DECIMAL(10, 2) NOT NULL CHECK (Precio > 0)
    );
END;

IF OBJECT_ID('Vendedor', 'U') IS NULL
BEGIN
    CREATE TABLE Vendedor
    (
        Legajo VARCHAR(20) NOT NULL PRIMARY KEY,
        Apellido VARCHAR(50) NOT NULL,
        Nombre VARCHAR(50) NOT NULL
    );
END;

IF OBJECT_ID('Venta', 'U') IS NULL
BEGIN
    CREATE TABLE Venta
    (
        NroFactura INT NOT NULL PRIMARY KEY,
        FechaFactura DATETIME NOT NULL,
        Legajo VARCHAR(20) NOT NULL,
        DNI VARCHAR(8) NOT NULL,
        CodProducto VARCHAR(20) NOT NULL,
        Precio DECIMAL(10, 2) NOT NULL,
        Cantidad INT NOT NULL,
        Importe DECIMAL(10, 2) NOT NULL,
        CONSTRAINT FK_Venta_Vendedor FOREIGN KEY (Legajo)
            REFERENCES Vendedor(Legajo),
        CONSTRAINT FK_Venta_Cliente FOREIGN KEY (DNI)
            REFERENCES Cliente(DNI),
        CONSTRAINT FK_Venta_Producto FOREIGN KEY (CodProducto)
            REFERENCES Producto(CodProducto)
    );
END;

IF NOT EXISTS (SELECT 1 FROM Producto)
BEGIN
    INSERT INTO Producto VALUES ('P001', 'Mesa', 'Marrón', 'Mesa de comedor', 85000);
    INSERT INTO Producto VALUES ('P002', 'Silla', 'Negro', 'Silla de madera', 25000);
    INSERT INTO Producto VALUES ('P003', 'Placard', 'Blanco', 'Placard de dos puertas', 140000);
END;

IF NOT EXISTS (SELECT 1 FROM Vendedor)
BEGIN
    INSERT INTO Vendedor VALUES ('V001', 'Pérez', 'Ana');
    INSERT INTO Vendedor VALUES ('V002', 'Gómez', 'Luis');
END;


-- ------------------------------------------------------------------
-- TP FINAL - Punto 6: tabla de usuarios para el login
-- (La aplicacion tambien la crea sola al abrir el Login si no existe.)
-- Las contraseñas se guardan como hash SHA-256 en mayusculas.
-- ------------------------------------------------------------------
IF OBJECT_ID('Usuario', 'U') IS NULL
BEGIN
    CREATE TABLE Usuario
    (
        IdUsuario INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        NombreUsuario VARCHAR(30) NOT NULL UNIQUE,
        Password VARCHAR(64) NOT NULL,
        Apellido VARCHAR(50) NOT NULL,
        Nombre VARCHAR(50) NOT NULL,
        Rol VARCHAR(20) NOT NULL CHECK (Rol IN ('Admin', 'Vendedor')),
        Activo BIT NOT NULL DEFAULT 1
    );
END;

IF NOT EXISTS (SELECT 1 FROM Usuario)
BEGIN
    -- admin / admin123
    INSERT INTO Usuario (NombreUsuario, Password, Apellido, Nombre, Rol, Activo)
    VALUES ('admin', '240BE518FABD2724DDB6F04EEB1DA5967448D7E831C08C8FA822809F74C720A9',
            'Administrador', 'Sistema', 'Admin', 1);
    -- vendedor / vend123
    INSERT INTO Usuario (NombreUsuario, Password, Apellido, Nombre, Rol, Activo)
    VALUES ('vendedor', '7E6E1F7BD27296E97D8494FC51CB3D19355B26E062E7C09A163D954213F41AB9',
            'Vendedor', 'Sistema', 'Vendedor', 1);
END;
