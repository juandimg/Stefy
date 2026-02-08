using System;
using System.Drawing;
using System.Windows.Forms;

namespace Angela
{
    public class frmLogin : Form
    {
        private TextBox txtUsuario;
        private TextBox txtPassword;
        private Button btnIngresar;
        private Panel panelCentral;

        public frmLogin()
        {
            // 1. CONFIGURACIÓN DE LA VENTANA
            this.Text = "Sistema Angela - Acceso";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(255, 182, 193); // Rosa un poco más suave

            // 2. CONTENEDOR PARA CENTRAR (Layout invisible)
            TableLayoutPanel tlp = new TableLayoutPanel();
            tlp.Dock = DockStyle.Fill; // Ocupa toda la pantalla
            tlp.ColumnCount = 3;
            tlp.RowCount = 3;
            // Crea 3 columnas y 3 filas, la del medio (donde va el login) es fija
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 400F)); 
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 450F));
            tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            // 3. PANEL CENTRAL BLANCO
            panelCentral = new Panel();
            panelCentral.BackColor = Color.White;
            panelCentral.Dock = DockStyle.Fill;
            panelCentral.Padding = new Padding(30);

            // 4. ELEMENTOS INTERACTIVOS
            Label lblTitulo = new Label() { 
                Text = "BIENVENIDA", 
                Font = new Font("Arial", 22, FontStyle.Bold), 
                ForeColor = Color.DeepPink,
                Dock = DockStyle.Top, Height = 60, TextAlign = ContentAlignment.MiddleCenter 
            };

            Label lblUser = new Label() { Text = "USUARIO", Font = new Font("Segoe UI", 10, FontStyle.Bold), Dock = DockStyle.Top, Height = 30 };
            txtUsuario = new TextBox() { Font = new Font("Segoe UI", 14), Dock = DockStyle.Top };
            
            // Espaciador
            Label spacer1 = new Label() { Dock = DockStyle.Top, Height = 20 };

            Label lblPass = new Label() { Text = "CONTRASEÑA", Font = new Font("Segoe UI", 10, FontStyle.Bold), Dock = DockStyle.Top, Height = 30 };
            txtPassword = new TextBox() { Font = new Font("Segoe UI", 14), Dock = DockStyle.Top, UseSystemPasswordChar = true };

            // Espaciador
            Label spacer2 = new Label() { Dock = DockStyle.Top, Height = 40 };

            btnIngresar = new Button() { 
                Text = "INICIAR SESIÓN", 
                Dock = DockStyle.Top, Height = 55,
                BackColor = Color.DeepPink, ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            // EFECTO INTERACTIVO
            btnIngresar.MouseEnter += (s, e) => { btnIngresar.BackColor = Color.HotPink; btnIngresar.Text = "¡LISTA PARA ENTRAR!"; };
            btnIngresar.MouseLeave += (s, e) => { btnIngresar.BackColor = Color.DeepPink; btnIngresar.Text = "INICIAR SESIÓN"; };

            btnIngresar.Click += (s, e) => {
                if (txtUsuario.Text == "admin" && txtPassword.Text == "1234") {
                    MessageBox.Show("¡Acceso Correcto!");
                } else {
                    MessageBox.Show("Credenciales incorrectas");
                }
            };

            // Agregar controles al panel (en orden inverso por el Dock.Top)
            panelCentral.Controls.Add(btnIngresar);
            panelCentral.Controls.Add(spacer2);
            panelCentral.Controls.Add(txtPassword);
            panelCentral.Controls.Add(lblPass);
            panelCentral.Controls.Add(spacer1);
            panelCentral.Controls.Add(txtUsuario);
            panelCentral.Controls.Add(lblUser);
            panelCentral.Controls.Add(lblTitulo);

            // Colocar el panel en la celda central de la rejilla
            tlp.Controls.Add(panelCentral, 1, 1);
            this.Controls.Add(tlp);
        }
    }
}