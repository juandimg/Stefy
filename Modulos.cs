using System;
using System.Drawing;
using System.Windows.Forms;

namespace Angela
{
    public class Modulos : Form
    {
        public Modulos()
        {
            // 1. CONFIGURACIÓN DE LA VENTANA
            this.Text = "Panel de Módulos - Angela Store";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(255, 240, 245); // Rosa muy claro de fondo
            this.Font = new Font("Segoe UI", 12);

            // 2. TÍTULO SUPERIOR
            Label lblBienvenida = new Label() {
                Text = "Selecciona un Módulo para Ingresar",
                Font = new Font("Arial", 28, FontStyle.Bold),
                ForeColor = Color.DeepPink,
                Dock = DockStyle.Top,
                Height = 150,
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblBienvenida);

            // 3. CONTENEDOR DE TARJETAS (Grid)
            TableLayoutPanel panelGrid = new TableLayoutPanel();
            panelGrid.Dock = DockStyle.Fill;
            panelGrid.ColumnCount = 2; // 2 Columnas
            panelGrid.RowCount = 2;    // 2 Filas
            
            // Hacer que las celdas sean iguales
            panelGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            panelGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            panelGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            panelGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            // 4. CREAR LOS 4 MÓDULOS
            panelGrid.Controls.Add(CrearTarjetaModulo("VENTAS", Color.HotPink, "Registrar nuevas compras"), 0, 0);
            panelGrid.Controls.Add(CrearTarjetaModulo("INVENTARIO", Color.MediumVioletRed, "Gestionar stock de prendas"), 1, 0);
            panelGrid.Controls.Add(CrearTarjetaModulo("CLIENTES", Color.DeepPink, "Base de datos de compradores"), 0, 1);
            panelGrid.Controls.Add(CrearTarjetaModulo("CONTABILIDAD", Color.Plum, "Reportes y balances"), 1, 1);

            this.Controls.Add(panelGrid);
        }

        private Panel CrearTarjetaModulo(string titulo, Color colorBase, string descripcion)
        {
            // Contenedor de la tarjeta
            Panel card = new Panel() {
                Size = new Size(350, 200),
                BackColor = Color.White,
                Margin = new Padding(50),
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.None // Mantiene la tarjeta centrada en su celda
            };

            // Etiqueta de Título
            Label lblTitle = new Label() {
                Text = titulo,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = colorBase,
                Location = new Point(20, 30),
                AutoSize = true
            };

            // Etiqueta de Descripción
            Label lblDesc = new Label() {
                Text = descripcion,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Gray,
                Location = new Point(23, 80),
                Size = new Size(300, 40)
            };

            // Botón de Ingreso dentro de la tarjeta
            Button btnIr = new Button() {
                Text = "INGRESAR",
                Location = new Point(20, 130),
                Size = new Size(120, 40),
                BackColor = colorBase,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnIr.FlatAppearance.BorderSize = 0;

            // Eventos de interactividad
            card.MouseEnter += (s, e) => { card.BackColor = Color.FromArgb(250, 250, 250); card.BorderStyle = BorderStyle.FixedSingle; };
            card.MouseLeave += (s, e) => { card.BackColor = Color.White; card.BorderStyle = BorderStyle.None; };
            
            Action irAModulo = () => {
                if (titulo == "INVENTARIO")
                {
                    this.Hide();
                    frmInventario ventanaInventario = new frmInventario(this);
                    ventanaInventario.Show();
                }
                else
                {
                    MessageBox.Show("Módulo " + titulo + " próximamente.");
                }
            };

            card.Click += (s, e) => irAModulo();
            btnIr.Click += (s, e) => irAModulo();

            card.Controls.Add(lblTitle);
            card.Controls.Add(lblDesc);
            card.Controls.Add(btnIr);

            return card;
        }
    }
}