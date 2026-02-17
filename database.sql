-- Script para crear la base de datos y tabla de productos
-- Ejecutar en phpMyAdmin o MySQL CLI

CREATE DATABASE IF NOT EXISTS Angela;
USE Angela;

CREATE TABLE IF NOT EXISTS productos (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Categoria VARCHAR(50) NOT NULL,
    Referencia VARCHAR(50) NOT NULL,
    Talla VARCHAR(10) NOT NULL,
    Precio DECIMAL(10,2) NOT NULL,
    Cantidad INT NOT NULL DEFAULT 0
);
