-- Script para crear la base de datos y tabla de productos
-- Ejecutar en phpMyAdmin o MySQL CLI

CREATE DATABASE IF NOT EXISTS Angela;
USE Angela;

CREATE TABLE IF NOT EXISTS productos (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Categoria VARCHAR(50) NOT NULL,
    Subcategoria VARCHAR(50) NOT NULL,
    Marca VARCHAR(50) NOT NULL,
    Referencia VARCHAR(50) NOT NULL,
    Unidades INT NOT NULL DEFAULT 0,
    Talla VARCHAR(10) NOT NULL DEFAULT 'M',
    Descripcion VARCHAR(200) NOT NULL,
    PrecioCompra DECIMAL(10,2) NOT NULL,
    PrecioVenta DECIMAL(10,2) NOT NULL
);

-- Si la tabla ya existe, agregar la columna con:
-- ALTER TABLE productos ADD COLUMN Talla VARCHAR(10) NOT NULL DEFAULT 'M' AFTER Unidades;
