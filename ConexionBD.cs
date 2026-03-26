using MySqlConnector;

namespace Angela
{
    public static class ConexionBD
    {
        private static string Server   = "localhost";
        private static string Database = "angela_store";
        private static string User     = "root";
        private static string Password = "Esognare2020.";

        private static string ConnectionString =>
            $"Server={Server};Database={Database};Uid={User};Pwd={Password};";

        public static MySqlConnection ObtenerConexion()
        {
            return new MySqlConnection(ConnectionString);
        }

        public static void InicializarBD()
        {
            // Crear la base de datos si no existe
            using (var conn = new MySqlConnection($"Server={Server};Uid={User};Pwd={Password};"))
            {
                conn.Open();
                new MySqlCommand($"CREATE DATABASE IF NOT EXISTS `{Database}`", conn).ExecuteNonQuery();
            }

            using (var conn = ObtenerConexion())
            {
                conn.Open();

                new MySqlCommand(@"
                    CREATE TABLE IF NOT EXISTS productos (
                        Id          INT PRIMARY KEY AUTO_INCREMENT,
                        Categoria   VARCHAR(100) NOT NULL,
                        Subcategoria VARCHAR(100) NOT NULL,
                        Marca       VARCHAR(100) NOT NULL,
                        Referencia  VARCHAR(100) NOT NULL,
                        Unidades    INT NOT NULL DEFAULT 0,
                        Talla       VARCHAR(20)  NOT NULL DEFAULT 'M',
                        Descripcion VARCHAR(500) NOT NULL,
                        PrecioCompra DECIMAL(10,2) NOT NULL,
                        PrecioVenta  DECIMAL(10,2) NOT NULL
                    )", conn).ExecuteNonQuery();

                new MySqlCommand(@"
                    CREATE TABLE IF NOT EXISTS ventas (
                        Id     INT PRIMARY KEY AUTO_INCREMENT,
                        Fecha  VARCHAR(30)   NOT NULL,
                        Total  DECIMAL(10,2) NOT NULL,
                        Tienda VARCHAR(100)  NOT NULL DEFAULT 'Principal'
                    )", conn).ExecuteNonQuery();

                new MySqlCommand(@"
                    CREATE TABLE IF NOT EXISTS detalle_ventas (
                        Id             INT PRIMARY KEY AUTO_INCREMENT,
                        VentaId        INT NOT NULL,
                        ProductoId     INT NOT NULL,
                        Descripcion    VARCHAR(500)  NOT NULL,
                        Talla          VARCHAR(20)   NOT NULL,
                        Cantidad       INT NOT NULL,
                        PrecioUnitario DECIMAL(10,2) NOT NULL,
                        Subtotal       DECIMAL(10,2) NOT NULL,
                        FOREIGN KEY (VentaId)    REFERENCES ventas(Id),
                        FOREIGN KEY (ProductoId) REFERENCES productos(Id)
                    )", conn).ExecuteNonQuery();

                new MySqlCommand(@"
                    CREATE TABLE IF NOT EXISTS gastos (
                        Id        INT PRIMARY KEY AUTO_INCREMENT,
                        Fecha     VARCHAR(30)   NOT NULL,
                        Categoria VARCHAR(100)  NOT NULL,
                        Concepto  VARCHAR(500)  NOT NULL,
                        Monto     DECIMAL(10,2) NOT NULL
                    )", conn).ExecuteNonQuery();
            }
        }
    }
}
