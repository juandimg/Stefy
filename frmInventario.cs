using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Angela
{
    public class frmInventario : Form
    {
        // Controles del formulario
        private TextBox txtNombre, txtReferencia, txtTalla, txtPrecio, txtCantidad, txtBuscar;
        private ComboBox cmbCategoria;
        private Button btnAgregar, btnModificar, btnEliminar, btnLimpiar, btnBuscar, btnVolver;
        private DataGridView dgvProductos;
        private Label lblTotalProductos, lblValorTotal, lblStockBajo;
        private Panel panelIzquierdo;
        private int idSeleccionado = -1;

        // Referencia al formulario padre
        private Form formPadre;

        public frmInventario(Form padre)
        {
            formPadre = padre;
            InicializarComponentes();
            CargarProductos();
            ActualizarReportes();
            this.Shown += (s, e) => {
                panelIzquierdo.Width = Math.Max(450, (int)(this.ClientSize.Width * 0.30));
            };
        }

        private void InicializarComponentes()
        {
            // CONFIGURACIÓN DE LA VENTANA
            this.Text = "Inventario - Angela Store";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(255, 240, 245);
            this.Font = new Font("Segoe UI", 10);
            this.FormClosed += (s, e) => { formPadre.Show(); };

            // ===== PANEL SUPERIOR (Título + Volver) =====
            Panel panelSuperior = new Panel()
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.MediumVioletRed
            };

            Label lblTitulo = new Label()
            {
                Text = "INVENTARIO",
                Font = new Font("Arial", 24, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            btnVolver = new Button()
            {
                Text = "← VOLVER",
                Dock = DockStyle.Left,
                Width = 120,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.MediumVioletRed,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnVolver.FlatAppearance.BorderSize = 0;
            btnVolver.Click += (s, e) => { this.Close(); };

            panelSuperior.Controls.Add(lblTitulo);
            panelSuperior.Controls.Add(btnVolver);

            // ===== PANEL PRINCIPAL (Formulario + Grid) =====
            panelIzquierdo = new Panel()
            {
                Dock = DockStyle.Left,
                Width = 450,
                BackColor = Color.FromArgb(255, 240, 245)
            };

            // ----- PANEL IZQUIERDO: Formulario -----
            Panel panelForm = new Panel()
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                AutoScroll = true
            };

            // Redimensiona los controles cuando el panel cambia de tamaño
            panelForm.Resize += (s, e) => {
                int ancho = panelForm.Width - 40;
                if (ancho < 50) return;
                foreach (Control ctrl in panelForm.Controls)
                {
                    if (ctrl is Label) continue;
                    ctrl.Left = 20;
                    ctrl.Width = ancho;
                }
            };

            int yPos = 10;
            int labelHeight = 25;
            int controlHeight = 35;
            int spacing = 10;

           

            // Categoría
            panelForm.Controls.Add(CrearLabel("CATEGORÍA:", yPos));
            yPos += labelHeight;
            cmbCategoria = new ComboBox()
            {
                Location = new Point(20, yPos),
                Size = new Size(360, controlHeight),
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbCategoria.Items.AddRange(new string[] { "Blusas", "Pantalones", "Vestidos", "Faldas", "Otro" });
            cmbCategoria.SelectedIndex = 0;
            panelForm.Controls.Add(cmbCategoria);
            yPos += controlHeight + spacing;


             // Nombre
            panelForm.Controls.Add(CrearLabel("NOMBRE:", yPos));
            yPos += labelHeight;
            txtNombre = CrearTextBox(yPos);
            panelForm.Controls.Add(txtNombre);
            yPos += controlHeight + spacing;

            // Referencia
            panelForm.Controls.Add(CrearLabel("REFERENCIA:", yPos));
            yPos += labelHeight;
            txtReferencia = CrearTextBox(yPos);
            panelForm.Controls.Add(txtReferencia);
            yPos += controlHeight + spacing;

            // Talla
            panelForm.Controls.Add(CrearLabel("TALLA:", yPos));
            yPos += labelHeight;
            txtTalla = CrearTextBox(yPos);
            panelForm.Controls.Add(txtTalla);
            yPos += controlHeight + spacing;

            // Precio
            panelForm.Controls.Add(CrearLabel("PRECIO:", yPos));
            yPos += labelHeight;
            txtPrecio = CrearTextBox(yPos);
            panelForm.Controls.Add(txtPrecio);
            yPos += controlHeight + spacing;

            // Cantidad
            panelForm.Controls.Add(CrearLabel("CANTIDAD:", yPos));
            yPos += labelHeight;
            txtCantidad = CrearTextBox(yPos);
            panelForm.Controls.Add(txtCantidad);
            yPos += controlHeight + spacing + 10;

            // Botones CRUD
            btnAgregar = CrearBoton("AGREGAR", yPos, Color.MediumVioletRed);
            btnAgregar.Click += BtnAgregar_Click;
            panelForm.Controls.Add(btnAgregar);
            yPos += 50;

            btnModificar = CrearBoton("MODIFICAR", yPos, Color.HotPink);
            btnModificar.Click += BtnModificar_Click;
            panelForm.Controls.Add(btnModificar);
            yPos += 50;

            btnEliminar = CrearBoton("ELIMINAR", yPos, Color.IndianRed);
            btnEliminar.Click += BtnEliminar_Click;
            panelForm.Controls.Add(btnEliminar);
            yPos += 50;

            btnLimpiar = CrearBoton("LIMPIAR", yPos, Color.Gray);
            btnLimpiar.Click += (s, e) => LimpiarCampos();
            panelForm.Controls.Add(btnLimpiar);

            panelIzquierdo.Controls.Add(panelForm);

            // ----- PANEL DERECHO: Grid + Búsqueda + Reportes -----
            Panel panelDerecho = new Panel()
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            // Barra de búsqueda
            Panel panelBusqueda = new Panel()
            {
                Dock = DockStyle.Top,
                Height = 50
            };

            txtBuscar = new TextBox()
            {
                Location = new Point(10, 10),
                Size = new Size(300, 35),
                Font = new Font("Segoe UI", 11),
                PlaceholderText = "Buscar por nombre o categoría..."
            };

            btnBuscar = new Button()
            {
                Text = "BUSCAR",
                Location = new Point(320, 8),
                Size = new Size(100, 35),
                BackColor = Color.MediumVioletRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnBuscar.FlatAppearance.BorderSize = 0;
            btnBuscar.Click += BtnBuscar_Click;

            Button btnMostrarTodo = new Button()
            {
                Text = "VER TODO",
                Location = new Point(430, 8),
                Size = new Size(100, 35),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnMostrarTodo.FlatAppearance.BorderSize = 0;
            btnMostrarTodo.Click += (s, e) => { txtBuscar.Clear(); CargarProductos(); };

            panelBusqueda.Controls.Add(txtBuscar);
            panelBusqueda.Controls.Add(btnBuscar);
            panelBusqueda.Controls.Add(btnMostrarTodo);

            // DataGridView
            dgvProductos = new DataGridView()
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 10)
            };
            dgvProductos.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 200, 220);
            dgvProductos.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvProductos.ColumnHeadersDefaultCellStyle.BackColor = Color.MediumVioletRed;
            dgvProductos.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvProductos.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvProductos.EnableHeadersVisualStyles = false;
            dgvProductos.CellClick += DgvProductos_CellClick;

            // Panel de reportes
            Panel panelReportes = new Panel()
            {
                Dock = DockStyle.Bottom,
                Height = 80,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            lblTotalProductos = new Label()
            {
                Text = "Total productos: 0",
                Location = new Point(10, 10),
                AutoSize = true,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.MediumVioletRed
            };

            lblValorTotal = new Label()
            {
                Text = "Valor total: $0.00",
                Location = new Point(10, 40),
                AutoSize = true,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.MediumVioletRed
            };

            lblStockBajo = new Label()
            {
                Text = "Productos con stock bajo (< 5): 0",
                Location = new Point(350, 10),
                AutoSize = true,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.IndianRed
            };

            panelReportes.Controls.Add(lblTotalProductos);
            panelReportes.Controls.Add(lblValorTotal);
            panelReportes.Controls.Add(lblStockBajo);

            // Agregar al panel derecho en orden
            panelDerecho.Controls.Add(dgvProductos);
            panelDerecho.Controls.Add(panelBusqueda);
            panelDerecho.Controls.Add(panelReportes);

            // Orden: Fill primero, luego Left, luego Top
            this.Controls.Add(panelDerecho);
            this.Controls.Add(panelIzquierdo);
            this.Controls.Add(panelSuperior);
        }

        // ===== HELPERS DE UI =====

        private Label CrearLabel(string texto, int y)
        {
            return new Label()
            {
                Text = texto,
                Location = new Point(20, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.MediumVioletRed
            };
        }

        private TextBox CrearTextBox(int y)
        {
            return new TextBox()
            {
                Location = new Point(20, y),
                Size = new Size(360, 35),
                Font = new Font("Segoe UI", 11)
            };
        }

        private Button CrearBoton(string texto, int y, Color color)
        {
            Button btn = new Button()
            {
                Text = texto,
                Location = new Point(20, y),
                Size = new Size(360, 40),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        // ===== OPERACIONES CRUD =====

        private void CargarProductos()
        {
            try
            {
                using (MySqlConnection conn = ConexionBD.ObtenerConexion())
                {
                    conn.Open();
                    string query = "SELECT Id, Nombre, Categoria, Referencia, Talla, Precio, Cantidad FROM productos";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvProductos.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar productos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;

            try
            {
                using (MySqlConnection conn = ConexionBD.ObtenerConexion())
                {
                    conn.Open();
                    string query = "INSERT INTO productos (Nombre, Categoria, Referencia, Talla, Precio, Cantidad) VALUES (@nombre, @categoria, @referencia, @talla, @precio, @cantidad)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@nombre", txtNombre.Text.Trim());
                    cmd.Parameters.AddWithValue("@categoria", cmbCategoria.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@referencia", txtReferencia.Text.Trim());
                    cmd.Parameters.AddWithValue("@talla", txtTalla.Text.Trim());
                    cmd.Parameters.AddWithValue("@precio", decimal.Parse(txtPrecio.Text.Trim()));
                    cmd.Parameters.AddWithValue("@cantidad", int.Parse(txtCantidad.Text.Trim()));
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Producto agregado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                CargarProductos();
                ActualizarReportes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnModificar_Click(object sender, EventArgs e)
        {
            if (idSeleccionado == -1)
            {
                MessageBox.Show("Seleccione un producto de la tabla para modificar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!ValidarCampos()) return;

            try
            {
                using (MySqlConnection conn = ConexionBD.ObtenerConexion())
                {
                    conn.Open();
                    string query = "UPDATE productos SET Nombre=@nombre, Categoria=@categoria, Referencia=@referencia, Talla=@talla, Precio=@precio, Cantidad=@cantidad WHERE Id=@id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@nombre", txtNombre.Text.Trim());
                    cmd.Parameters.AddWithValue("@categoria", cmbCategoria.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@referencia", txtReferencia.Text.Trim());
                    cmd.Parameters.AddWithValue("@talla", txtTalla.Text.Trim());
                    cmd.Parameters.AddWithValue("@precio", decimal.Parse(txtPrecio.Text.Trim()));
                    cmd.Parameters.AddWithValue("@cantidad", int.Parse(txtCantidad.Text.Trim()));
                    cmd.Parameters.AddWithValue("@id", idSeleccionado);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Producto modificado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                CargarProductos();
                ActualizarReportes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al modificar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (idSeleccionado == -1)
            {
                MessageBox.Show("Seleccione un producto de la tabla para eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult resultado = MessageBox.Show("¿Está seguro de eliminar este producto?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resultado != DialogResult.Yes) return;

            try
            {
                using (MySqlConnection conn = ConexionBD.ObtenerConexion())
                {
                    conn.Open();
                    string query = "DELETE FROM productos WHERE Id=@id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", idSeleccionado);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Producto eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                CargarProductos();
                ActualizarReportes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ===== BÚSQUEDA =====

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            string termino = txtBuscar.Text.Trim();
            if (string.IsNullOrEmpty(termino))
            {
                CargarProductos();
                return;
            }

            try
            {
                using (MySqlConnection conn = ConexionBD.ObtenerConexion())
                {
                    conn.Open();
                    string query = "SELECT Id, Nombre, Categoria, Referencia, Talla, Precio, Cantidad FROM productos WHERE Nombre LIKE @termino OR Categoria LIKE @termino";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@termino", "%" + termino + "%");
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvProductos.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ===== REPORTES =====

        private void ActualizarReportes()
        {
            try
            {
                using (MySqlConnection conn = ConexionBD.ObtenerConexion())
                {
                    conn.Open();

                    // Total de productos
                    MySqlCommand cmdTotal = new MySqlCommand("SELECT COUNT(*) FROM productos", conn);
                    int total = Convert.ToInt32(cmdTotal.ExecuteScalar());
                    lblTotalProductos.Text = "Total productos: " + total;

                    // Valor total del inventario (precio * cantidad)
                    MySqlCommand cmdValor = new MySqlCommand("SELECT COALESCE(SUM(Precio * Cantidad), 0) FROM productos", conn);
                    decimal valor = Convert.ToDecimal(cmdValor.ExecuteScalar());
                    lblValorTotal.Text = "Valor total: $" + valor.ToString("N2");

                    // Productos con stock bajo
                    MySqlCommand cmdBajo = new MySqlCommand("SELECT COUNT(*) FROM productos WHERE Cantidad < 5", conn);
                    int bajo = Convert.ToInt32(cmdBajo.ExecuteScalar());
                    lblStockBajo.Text = "Productos con stock bajo (< 5): " + bajo;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar reportes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ===== EVENTOS DEL GRID =====

        private void DgvProductos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow fila = dgvProductos.Rows[e.RowIndex];
            idSeleccionado = Convert.ToInt32(fila.Cells["Id"].Value);
            txtNombre.Text = fila.Cells["Nombre"].Value.ToString();

            string categoria = fila.Cells["Categoria"].Value.ToString();
            int index = cmbCategoria.Items.IndexOf(categoria);
            cmbCategoria.SelectedIndex = index >= 0 ? index : 0;

            txtReferencia.Text = fila.Cells["Referencia"].Value.ToString();
            txtTalla.Text = fila.Cells["Talla"].Value.ToString();
            txtPrecio.Text = fila.Cells["Precio"].Value.ToString();
            txtCantidad.Text = fila.Cells["Cantidad"].Value.ToString();
        }

        // ===== UTILIDADES =====

        private void LimpiarCampos()
        {
            txtNombre.Clear();
            cmbCategoria.SelectedIndex = 0;
            txtReferencia.Clear();
            txtTalla.Clear();
            txtPrecio.Clear();
            txtCantidad.Clear();
            idSeleccionado = -1;
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Ingrese el nombre del producto.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtTalla.Text))
            {
                MessageBox.Show("Ingrese la talla.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTalla.Focus();
                return false;
            }
            if (!decimal.TryParse(txtPrecio.Text.Trim(), out decimal precio) || precio < 0)
            {
                MessageBox.Show("Ingrese un precio válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrecio.Focus();
                return false;
            }
            if (!int.TryParse(txtCantidad.Text.Trim(), out int cantidad) || cantidad < 0)
            {
                MessageBox.Show("Ingrese una cantidad válida.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCantidad.Focus();
                return false;
            }
            return true;
        }
    }
}
