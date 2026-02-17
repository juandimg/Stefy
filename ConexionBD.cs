using MySql.Data.MySqlClient;

namespace Angela
{
    public static class ConexionBD
    {
        private static string connectionString = "Server=localhost;Port=3306;Database=Angela;Uid=root;Pwd=Esognare2020.;";

        public static MySqlConnection ObtenerConexion()
        {
            return new MySqlConnection(connectionString);
        }
    }
}
