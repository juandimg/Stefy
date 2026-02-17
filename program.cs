using System;
using System.Windows.Forms;

namespace Angela
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // Estas dos líneas reemplazan a la que te daba error
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Iniciamos el Login
            Application.Run(new Angela.frmLogin());
        }
    }
}