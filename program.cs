using System;
using System.IO;
using System.Windows.Forms;

namespace Angela
{
    // Nombre de este punto de venta — se lee de tienda.txt en la carpeta del exe
    public static class Config
    {
        public static string NombreTienda { get; set; } = "Tienda 1";
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // Leer nombre de tienda desde tienda.txt
                string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tienda.txt");
                if (!File.Exists(configPath))
                    File.WriteAllText(configPath, "Tienda 1");
                Config.NombreTienda = File.ReadAllText(configPath).Trim();

                // Crear tablas si no existen
                ConexionBD.InicializarBD();

                Application.Run(new Angela.frmLogin());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al iniciar el programa:\n\n" + ex.Message + "\n\n" + ex.StackTrace, 
                    "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
