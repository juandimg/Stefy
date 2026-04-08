using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySqlConnector;

namespace Angela
{
    public class frmInventario : Form
    {
        // Paleta de colores
        private static readonly Color ColorSidebar   = Color.FromArgb(28, 28, 45);
        private static readonly Color ColorSidebarAlt= Color.FromArgb(38, 38, 60);
        private static readonly Color ColorAccent    = Color.FromArgb(183, 28, 90);
        private static readonly Color ColorAccentHov = Color.FromArgb(210, 50, 110);
        private static readonly Color ColorFondo     = Color.FromArgb(242, 242, 248);
        private static readonly Color ColorCard      = Color.White;
        private static readonly Color ColorTextoSide = Color.FromArgb(190, 190, 210);
        private static readonly Color ColorTextoLight= Color.FromArgb(240, 240, 255);

        // Controles
        private TextBox txtSubcategoria, txtMarca, txtReferencia, txtUnidades,
                        txtDescripcion, txtPrecioCompra, txtPrecioVenta, txtBuscar;
        private ComboBox cmbCategoria, cmbTalla;
        private Button btnAgregar, btnModificar, btnEliminar, btnLimpiar, btnBuscar, btnAgregarUnidades;
        private DataGridView dgvProductos;
        private Label lblTotalProductos, lblValorTotal, lblStockBajo;
        private Panel panelIzquierdo;
        private int idSeleccionado = -1;
        private Form formPadre;

        public frmInventario(Form padre)
        {
            formPadre = padre;
            InicializarComponentes();
            CargarProductos();
            ActualizarReportes();
        }

        private void InicializarComponentes()
        {
            this.Text = "Inventario — Stefy Store";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = ColorFondo;
            this.Font = new Font("Segoe UI", 10);
            this.FormClosed += (s, e) => { formPadre.Show(); };

            // ── HEADER ────────────────────────────────────────────────────────
            Panel header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 62,
                BackColor = ColorSidebar
            };

            Button btnVolver = new Button
            {
                Text = "‹  Volver",
                Dock = DockStyle.Left,
                Width = 110,
                FlatStyle = FlatStyle.Flat,
                BackColor = ColorSidebar,
                ForeColor = ColorTextoSide,
                Font = new Font("Segoe UI", 10),
                Cursor = Cursors.Hand
            };
            btnVolver.FlatAppearance.BorderSize = 0;
            btnVolver.Click += (s, e) => this.Close();

            Label lblTitulo = new Label
            {
                Text = "GESTIÓN DE INVENTARIO",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = ColorTextoLight,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblSubtitulo = new Label
            {
                Text = "STEFY STORE",
                Font = new Font("Segoe UI", 8),
                ForeColor = ColorAccent,
                Dock = DockStyle.Right,
                Width = 110,
                TextAlign = ContentAlignment.MiddleCenter
            };

            header.Controls.Add(lblTitulo);
            header.Controls.Add(btnVolver);
            header.Controls.Add(lblSubtitulo);

            // ── SIDEBAR (formulario) ───────────────────────────────────────────
            panelIzquierdo = new Panel
            {
                Dock = DockStyle.Left,
                Width = 390,
                BackColor = ColorSidebar
            };

            Panel scrollable = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(0)
            };

            int x = 24, yPos = 0;

            // Título del formulario
            Label lblFormTitle = new Label
            {
                Text = "NUEVO PRODUCTO",
                Location = new Point(x, yPos + 18),
                AutoSize = true,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = ColorAccent
            };
            scrollable.Controls.Add(lblFormTitle);

            Panel divisor = new Panel
            {
                Location = new Point(x, yPos + 44),
                Size = new Size(340, 1),
                BackColor = ColorSidebarAlt
            };
            scrollable.Controls.Add(divisor);
            yPos = 58;

            // Campos del formulario
            scrollable.Controls.Add(CrearLabelSide("Categoría", yPos)); yPos += 22;
            cmbCategoria = CrearCombo(yPos, new[] { "Blusas", "Pantalones", "Vestidos", "Faldas", "Chaquetas", "Accesorios", "Otro" });
            scrollable.Controls.Add(cmbCategoria); yPos += 42;

            scrollable.Controls.Add(CrearLabelSide("Subcategoría", yPos)); yPos += 22;
            txtSubcategoria = CrearInput(yPos); scrollable.Controls.Add(txtSubcategoria); yPos += 42;

            scrollable.Controls.Add(CrearLabelSide("Marca", yPos)); yPos += 22;
            txtMarca = CrearInput(yPos); scrollable.Controls.Add(txtMarca); yPos += 42;

            scrollable.Controls.Add(CrearLabelSide("Referencia", yPos)); yPos += 22;
            txtReferencia = CrearInput(yPos); scrollable.Controls.Add(txtReferencia); yPos += 42;

            scrollable.Controls.Add(CrearLabelSide("Talla", yPos)); yPos += 22;
            cmbTalla = CrearCombo(yPos, new[] { "XS","S","M","L","XL","XXL","XXXL","6","8","10","12","14","16","18","Único" });
            cmbTalla.SelectedIndex = 2;
            scrollable.Controls.Add(cmbTalla); yPos += 42;

            scrollable.Controls.Add(CrearLabelSide("Unidades en stock", yPos)); yPos += 22;
            txtUnidades = CrearInput(yPos); scrollable.Controls.Add(txtUnidades); yPos += 42;

            scrollable.Controls.Add(CrearLabelSide("Descripción", yPos)); yPos += 22;
            txtDescripcion = CrearInput(yPos); scrollable.Controls.Add(txtDescripcion); yPos += 42;

            // Precios en dos columnas
            scrollable.Controls.Add(CrearLabelSide("Precio Compra", yPos)); yPos += 22;
            txtPrecioCompra = CrearInput(yPos); scrollable.Controls.Add(txtPrecioCompra); yPos += 42;

            scrollable.Controls.Add(CrearLabelSide("Precio Venta", yPos)); yPos += 22;
            txtPrecioVenta = CrearInput(yPos); scrollable.Controls.Add(txtPrecioVenta); yPos += 52;

            // Separador antes de botones
            Panel div2 = new Panel
            {
                Location = new Point(x, yPos),
                Size = new Size(340, 1),
                BackColor = ColorSidebarAlt
            };
            scrollable.Controls.Add(div2);
            yPos += 14;

            // Botones CRUD
            btnAgregar = CrearBotonSide("+ Agregar Producto", yPos, ColorAccent, ColorAccentHov);
            btnAgregar.Click += BtnAgregar_Click;
            scrollable.Controls.Add(btnAgregar); yPos += 46;

            btnModificar = CrearBotonSide("✎  Modificar Seleccionado", yPos, Color.FromArgb(50, 50, 80), Color.FromArgb(70, 70, 105));
            btnModificar.Click += BtnModificar_Click;
            scrollable.Controls.Add(btnModificar); yPos += 46;

            btnAgregarUnidades = CrearBotonSide("➕  Agregar Unidades", yPos, Color.FromArgb(30, 100, 60), Color.FromArgb(45, 130, 80));
            btnAgregarUnidades.Click += BtnAgregarUnidades_Click;
            scrollable.Controls.Add(btnAgregarUnidades); yPos += 46;

            btnEliminar = CrearBotonSide("✕  Eliminar Seleccionado", yPos, Color.FromArgb(140, 30, 30), Color.FromArgb(180, 50, 50));
            btnEliminar.Click += BtnEliminar_Click;
            scrollable.Controls.Add(btnEliminar); yPos += 46;

            btnLimpiar = CrearBotonSide("↺  Limpiar Campos", yPos, Color.FromArgb(60, 60, 60), Color.FromArgb(85, 85, 85));
            btnLimpiar.Click += (s, e) => LimpiarCampos();
            scrollable.Controls.Add(btnLimpiar);

            panelIzquierdo.Controls.Add(scrollable);

            // ── ÁREA DERECHA ───────────────────────────────────────────────────
            Panel panelDerecho = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ColorFondo,
                Padding = new Padding(16, 12, 16, 12)
            };

            // Barra de búsqueda
            Panel panelBusqueda = new Panel
            {
                Dock = DockStyle.Top,
                Height = 56,
                BackColor = ColorCard,
                Padding = new Padding(10, 10, 10, 10)
            };
            EstiloCard(panelBusqueda);

            txtBuscar = new TextBox
            {
                Location = new Point(12, 13),
                Size = new Size(330, 32),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.None,
                PlaceholderText = "  Buscar por marca, categoría, referencia..."
            };

            Button btnBuscarBtn = new Button
            {
                Text = "Buscar",
                Location = new Point(350, 11),
                Size = new Size(90, 34),
                BackColor = ColorAccent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnBuscarBtn.FlatAppearance.BorderSize = 0;
            btnBuscarBtn.Click += BtnBuscar_Click;

            Button btnVerTodo = new Button
            {
                Text = "Ver Todo",
                Location = new Point(448, 11),
                Size = new Size(80, 34),
                BackColor = ColorFondo,
                ForeColor = Color.FromArgb(80, 80, 100),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9),
                Cursor = Cursors.Hand
            };
            btnVerTodo.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 215);
            btnVerTodo.FlatAppearance.BorderSize = 1;
            btnVerTodo.Click += (s, e) => { txtBuscar.Clear(); CargarProductos(); };

            panelBusqueda.Controls.Add(txtBuscar);
            panelBusqueda.Controls.Add(btnBuscarBtn);
            panelBusqueda.Controls.Add(btnVerTodo);

            // Tabla de productos
            Panel panelGrid = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ColorCard,
                Padding = new Padding(0)
            };
            EstiloCard(panelGrid);

            dgvProductos = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = ColorCard,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                GridColor = Color.FromArgb(235, 235, 245),
                Font = new Font("Segoe UI", 10),
                RowTemplate = { Height = 36 },
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
            };
            // Encabezados
            dgvProductos.ColumnHeadersDefaultCellStyle.BackColor = ColorSidebar;
            dgvProductos.ColumnHeadersDefaultCellStyle.ForeColor = ColorTextoLight;
            dgvProductos.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvProductos.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 0, 0, 0);
            dgvProductos.ColumnHeadersHeight = 40;
            dgvProductos.EnableHeadersVisualStyles = false;
            // Filas
            dgvProductos.DefaultCellStyle.BackColor = ColorCard;
            dgvProductos.DefaultCellStyle.ForeColor = Color.FromArgb(40, 40, 60);
            dgvProductos.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 230, 242);
            dgvProductos.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 30, 50);
            dgvProductos.DefaultCellStyle.Padding = new Padding(6, 0, 0, 0);
            dgvProductos.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 245, 252);
            dgvProductos.CellClick += DgvProductos_CellClick;

            panelGrid.Controls.Add(dgvProductos);

            // Tarjetas de estadísticas
            Panel panelStats = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 90,
                BackColor = ColorFondo,
                Padding = new Padding(0, 8, 0, 0)
            };

            Panel cardTotal = CrearCard("TOTAL PRODUCTOS", "0", Color.FromArgb(183, 28, 90));
            Panel cardValor = CrearCard("VALOR INVENTARIO", "$0.00", Color.FromArgb(48, 63, 159));
            Panel cardStock = CrearCard("STOCK BAJO (< 5)", "0", Color.FromArgb(183, 63, 0));

            lblTotalProductos = (Label)cardTotal.Controls["lblValor"];
            lblValorTotal     = (Label)cardValor.Controls["lblValor"];
            lblStockBajo      = (Label)cardStock.Controls["lblValor"];

            panelStats.Controls.Add(cardStock);
            panelStats.Controls.Add(cardValor);
            panelStats.Controls.Add(cardTotal);
            panelStats.Resize += (s, e) => DistribuirCards(panelStats, cardTotal, cardValor, cardStock);

            // Ensamblaje derecho
            panelDerecho.Controls.Add(panelGrid);
            panelDerecho.Controls.Add(panelBusqueda);
            panelDerecho.Controls.Add(panelStats);

            // Orden de controles en el form
            this.Controls.Add(panelDerecho);
            this.Controls.Add(panelIzquierdo);
            this.Controls.Add(header);

            this.Shown += (s, e) => DistribuirCards(panelStats, cardTotal, cardValor, cardStock);
        }

        // ── HELPERS DE UI ────────────────────────────────────────────────────

        private void EstiloCard(Panel p)
        {
            p.Margin = new Padding(0, 0, 0, 10);
        }

        private Label CrearLabelSide(string texto, int y)
        {
            return new Label
            {
                Text = texto.ToUpper(),
                Location = new Point(24, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = ColorTextoSide
            };
        }

        private TextBox CrearInput(int y)
        {
            return new TextBox
            {
                Location = new Point(24, y),
                Size = new Size(342, 32),
                Font = new Font("Segoe UI", 11),
                BackColor = ColorSidebarAlt,
                ForeColor = ColorTextoLight,
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        private ComboBox CrearCombo(int y, string[] items)
        {
            var cmb = new ComboBox
            {
                Location = new Point(24, y),
                Size = new Size(342, 32),
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = ColorSidebarAlt,
                ForeColor = ColorTextoLight,
                FlatStyle = FlatStyle.Flat
            };
            cmb.Items.AddRange(items);
            cmb.SelectedIndex = 0;
            return cmb;
        }

        private Button CrearBotonSide(string texto, int y, Color color, Color hover)
        {
            var btn = new Button
            {
                Text = texto,
                Location = new Point(24, y),
                Size = new Size(342, 38),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(12, 0, 0, 0)
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.MouseEnter += (s, e) => btn.BackColor = hover;
            btn.MouseLeave += (s, e) => btn.BackColor = color;
            return btn;
        }

        private Panel CrearCard(string titulo, string valorInicial, Color colorAccentCard)
        {
            Panel card = new Panel
            {
                BackColor = ColorCard,
                Height = 72
            };

            Panel barra = new Panel
            {
                Dock = DockStyle.Left,
                Width = 5,
                BackColor = colorAccentCard
            };

            Label lblTit = new Label
            {
                Text = titulo,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = Color.FromArgb(130, 130, 150),
                Location = new Point(16, 12),
                AutoSize = true
            };

            Label lblVal = new Label
            {
                Name = "lblValor",
                Text = valorInicial,
                Font = new Font("Segoe UI", 17, FontStyle.Bold),
                ForeColor = colorAccentCard,
                Location = new Point(16, 32),
                AutoSize = true
            };

            card.Controls.Add(lblTit);
            card.Controls.Add(lblVal);
            card.Controls.Add(barra);
            return card;
        }

        private void DistribuirCards(Panel contenedor, params Panel[] cards)
        {
            int margen = 8;
            int ancho = (contenedor.Width - margen * (cards.Length + 1)) / cards.Length;
            for (int i = 0; i < cards.Length; i++)
            {
                cards[i].Location = new Point(margen + i * (ancho + margen), 8);
                cards[i].Size = new Size(ancho, 72);
            }
        }

        // ── CRUD ─────────────────────────────────────────────────────────────

        private void CargarProductos()
        {
            try
            {
                using (MySqlConnection conn = ConexionBD.ObtenerConexion())
                {
                    conn.Open();
                    string query = "SELECT Id, Categoria, Subcategoria, Marca, Referencia, Unidades, Talla, Descripcion, PrecioCompra, PrecioVenta FROM productos";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    DataTable dt = new DataTable();
                    dt.Load(cmd.ExecuteReader());
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
                    string query = "INSERT INTO productos (Categoria, Subcategoria, Marca, Referencia, Unidades, Talla, Descripcion, PrecioCompra, PrecioVenta) VALUES (@categoria, @subcategoria, @marca, @referencia, @unidades, @talla, @descripcion, @precioCompra, @precioVenta)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@categoria", cmbCategoria.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@subcategoria", txtSubcategoria.Text.Trim());
                    cmd.Parameters.AddWithValue("@marca", txtMarca.Text.Trim());
                    cmd.Parameters.AddWithValue("@referencia", txtReferencia.Text.Trim());
                    cmd.Parameters.AddWithValue("@unidades", int.Parse(txtUnidades.Text.Trim()));
                    cmd.Parameters.AddWithValue("@talla", cmbTalla.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@descripcion", txtDescripcion.Text.Trim());
                    cmd.Parameters.AddWithValue("@precioCompra", decimal.Parse(txtPrecioCompra.Text.Trim()));
                    cmd.Parameters.AddWithValue("@precioVenta", decimal.Parse(txtPrecioVenta.Text.Trim()));
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
                    string query = "UPDATE productos SET Categoria=@categoria, Subcategoria=@subcategoria, Marca=@marca, Referencia=@referencia, Unidades=@unidades, Talla=@talla, Descripcion=@descripcion, PrecioCompra=@precioCompra, PrecioVenta=@precioVenta WHERE Id=@id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@categoria", cmbCategoria.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@subcategoria", txtSubcategoria.Text.Trim());
                    cmd.Parameters.AddWithValue("@marca", txtMarca.Text.Trim());
                    cmd.Parameters.AddWithValue("@referencia", txtReferencia.Text.Trim());
                    cmd.Parameters.AddWithValue("@unidades", int.Parse(txtUnidades.Text.Trim()));
                    cmd.Parameters.AddWithValue("@talla", cmbTalla.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@descripcion", txtDescripcion.Text.Trim());
                    cmd.Parameters.AddWithValue("@precioCompra", decimal.Parse(txtPrecioCompra.Text.Trim()));
                    cmd.Parameters.AddWithValue("@precioVenta", decimal.Parse(txtPrecioVenta.Text.Trim()));
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

        private void BtnAgregarUnidades_Click(object sender, EventArgs e)
        {
            if (idSeleccionado == -1)
            {
                MessageBox.Show("Seleccione un producto de la tabla primero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Mini-diálogo para pedir la cantidad
            Form dlg = new Form
            {
                Text = "Agregar Unidades",
                Size = new Size(320, 160),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.FromArgb(242, 242, 248)
            };

            Label lbl = new Label
            {
                Text = "¿Cuántas unidades desea agregar?",
                Location = new Point(16, 18),
                AutoSize = true,
                Font = new Font("Segoe UI", 10)
            };

            TextBox txtCantidad = new TextBox
            {
                Location = new Point(16, 48),
                Size = new Size(270, 28),
                Font = new Font("Segoe UI", 11)
            };

            Button btnOk = new Button
            {
                Text = "Agregar",
                DialogResult = DialogResult.OK,
                Location = new Point(130, 85),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(30, 100, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnOk.FlatAppearance.BorderSize = 0;

            Button btnCancelar = new Button
            {
                Text = "Cancelar",
                DialogResult = DialogResult.Cancel,
                Location = new Point(218, 85),
                Size = new Size(80, 30),
                FlatStyle = FlatStyle.Flat
            };

            dlg.Controls.AddRange(new Control[] { lbl, txtCantidad, btnOk, btnCancelar });
            dlg.AcceptButton = btnOk;
            dlg.CancelButton = btnCancelar;

            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            if (!int.TryParse(txtCantidad.Text.Trim(), out int cantidad) || cantidad <= 0)
            {
                MessageBox.Show("Ingrese una cantidad válida (número entero positivo).", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection conn = ConexionBD.ObtenerConexion())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("UPDATE productos SET Unidades = Unidades + @cantidad WHERE Id = @id", conn);
                    cmd.Parameters.AddWithValue("@cantidad", cantidad);
                    cmd.Parameters.AddWithValue("@id", idSeleccionado);
                    cmd.ExecuteNonQuery();
                }

                int stockActual = int.TryParse(txtUnidades.Text, out int s) ? s : 0;
                txtUnidades.Text = (stockActual + cantidad).ToString();

                MessageBox.Show($"Se agregaron {cantidad} unidades correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarProductos();
                ActualizarReportes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar unidades: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (idSeleccionado == -1)
            {
                MessageBox.Show("Seleccione un producto de la tabla para eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show("¿Está seguro de eliminar este producto?\nEsta acción no se puede deshacer.", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;
            try
            {
                using (MySqlConnection conn = ConexionBD.ObtenerConexion())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("DELETE FROM productos WHERE Id=@id", conn);
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

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            string termino = txtBuscar.Text.Trim();
            if (string.IsNullOrEmpty(termino)) { CargarProductos(); return; }
            try
            {
                using (MySqlConnection conn = ConexionBD.ObtenerConexion())
                {
                    conn.Open();
                    string query = "SELECT Id, Categoria, Subcategoria, Marca, Referencia, Unidades, Talla, Descripcion, PrecioCompra, PrecioVenta FROM productos WHERE Descripcion LIKE @t OR Marca LIKE @t OR Categoria LIKE @t OR Referencia LIKE @t OR Talla LIKE @t";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@t", "%" + termino + "%");
                    DataTable dt = new DataTable();
                    dt.Load(cmd.ExecuteReader());
                    dgvProductos.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ActualizarReportes()
        {
            try
            {
                using (MySqlConnection conn = ConexionBD.ObtenerConexion())
                {
                    conn.Open();

                    MySqlCommand cmdTotal = new MySqlCommand("SELECT COUNT(*) FROM productos", conn);
                    int total = Convert.ToInt32(cmdTotal.ExecuteScalar());
                    lblTotalProductos.Text = total.ToString();

                    MySqlCommand cmdValor = new MySqlCommand("SELECT COALESCE(SUM(PrecioVenta * Unidades), 0) FROM productos", conn);
                    decimal valor = Convert.ToDecimal(cmdValor.ExecuteScalar());
                    lblValorTotal.Text = "$" + valor.ToString("N2");

                    MySqlCommand cmdBajo = new MySqlCommand("SELECT COUNT(*) FROM productos WHERE Unidades < 5", conn);
                    int bajo = Convert.ToInt32(cmdBajo.ExecuteScalar());
                    lblStockBajo.Text = bajo.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar reportes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvProductos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow fila = dgvProductos.Rows[e.RowIndex];
            idSeleccionado = Convert.ToInt32(fila.Cells["Id"].Value);

            string categoria = fila.Cells["Categoria"].Value.ToString();
            int idx = cmbCategoria.Items.IndexOf(categoria);
            cmbCategoria.SelectedIndex = idx >= 0 ? idx : 0;

            txtSubcategoria.Text  = fila.Cells["Subcategoria"].Value.ToString();
            txtMarca.Text         = fila.Cells["Marca"].Value.ToString();
            txtReferencia.Text    = fila.Cells["Referencia"].Value.ToString();
            txtUnidades.Text      = fila.Cells["Unidades"].Value.ToString();

            string talla = fila.Cells["Talla"].Value.ToString();
            int idxTalla = cmbTalla.Items.IndexOf(talla);
            cmbTalla.SelectedIndex = idxTalla >= 0 ? idxTalla : 2;

            txtDescripcion.Text   = fila.Cells["Descripcion"].Value.ToString();
            txtPrecioCompra.Text  = fila.Cells["PrecioCompra"].Value.ToString();
            txtPrecioVenta.Text   = fila.Cells["PrecioVenta"].Value.ToString();
        }

        private void LimpiarCampos()
        {
            cmbCategoria.SelectedIndex = 0;
            txtSubcategoria.Clear();
            txtMarca.Clear();
            txtReferencia.Clear();
            txtUnidades.Clear();
            cmbTalla.SelectedIndex = 2;
            txtDescripcion.Clear();
            txtPrecioCompra.Clear();
            txtPrecioVenta.Clear();
            idSeleccionado = -1;
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtSubcategoria.Text))
            { MessageBox.Show("Ingrese la subcategoría.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtSubcategoria.Focus(); return false; }
            if (string.IsNullOrWhiteSpace(txtMarca.Text))
            { MessageBox.Show("Ingrese la marca.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtMarca.Focus(); return false; }
            if (string.IsNullOrWhiteSpace(txtReferencia.Text))
            { MessageBox.Show("Ingrese la referencia.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtReferencia.Focus(); return false; }
            if (!int.TryParse(txtUnidades.Text.Trim(), out int u) || u < 0)
            { MessageBox.Show("Ingrese una cantidad de unidades válida.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtUnidades.Focus(); return false; }
            if (string.IsNullOrWhiteSpace(txtDescripcion.Text))
            { MessageBox.Show("Ingrese la descripción.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtDescripcion.Focus(); return false; }
            if (!decimal.TryParse(txtPrecioCompra.Text.Trim(), out decimal pc) || pc < 0)
            { MessageBox.Show("Ingrese un precio de compra válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtPrecioCompra.Focus(); return false; }
            if (!decimal.TryParse(txtPrecioVenta.Text.Trim(), out decimal pv) || pv < 0)
            { MessageBox.Show("Ingrese un precio de venta válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtPrecioVenta.Focus(); return false; }
            return true;
        }
    }
}
