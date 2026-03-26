using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySqlConnector;

namespace Angela
{
    public class frmVentas : Form
    {
        // ── Paleta POS ────────────────────────────────────────────────────────
        private static readonly Color C_Bg       = Color.FromArgb(15,  15,  25);
        private static readonly Color C_Panel    = Color.FromArgb(22,  22,  38);
        private static readonly Color C_Card     = Color.FromArgb(30,  30,  50);
        private static readonly Color C_Accent   = Color.FromArgb(183, 28,  90);
        private static readonly Color C_AccHov   = Color.FromArgb(210, 50,  110);
        private static readonly Color C_Green    = Color.FromArgb(27,  150, 90);
        private static readonly Color C_GreenHov = Color.FromArgb(40,  180, 110);
        private static readonly Color C_TextMain = Color.FromArgb(240, 240, 255);
        private static readonly Color C_TextDim  = Color.FromArgb(140, 140, 165);
        private static readonly Color C_Border   = Color.FromArgb(50,  50,  75);

        // ── Controles ─────────────────────────────────────────────────────────
        private TextBox    txtBarcode, txtBuscarManual;
        private DataGridView dgvTicket;
        private Label      lblTotal, lblItems, lblHora, lblMensaje;
        private Label      lblCantTicket;
        private DataTable  dtTicket;
        private Form       formPadre;
        private Timer      timerReloj, timerFoco, timerMensaje;

        public frmVentas(Form padre)
        {
            formPadre = padre;
            InicializarTicket();
            InicializarComponentes();
            CargarTimers();
        }

        // ── Ticket en memoria ─────────────────────────────────────────────────
        private void InicializarTicket()
        {
            dtTicket = new DataTable();
            dtTicket.Columns.Add("ProductoId",  typeof(int));
            dtTicket.Columns.Add("Codigo",      typeof(string));
            dtTicket.Columns.Add("Descripcion", typeof(string));
            dtTicket.Columns.Add("Talla",       typeof(string));
            dtTicket.Columns.Add("Precio",      typeof(decimal));
            dtTicket.Columns.Add("Cant",        typeof(int));
            dtTicket.Columns.Add("Subtotal",    typeof(decimal));
        }

        private void InicializarComponentes()
        {
            this.Text            = "Punto de Venta — Stefy Store";
            this.WindowState     = FormWindowState.Maximized;
            this.BackColor       = C_Bg;
            this.Font            = new Font("Segoe UI", 10);
            this.FormClosed     += (s, e) => { formPadre.Show(); };
            this.KeyPreview      = true;
            this.KeyDown        += FrmVentas_KeyDown;

            // ── HEADER ──────────────────────────────────────────────────────
            Panel header = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 58,
                BackColor = C_Panel
            };

            Button btnVolver = new Button
            {
                Text      = "‹  Salir",
                Dock      = DockStyle.Left,
                Width     = 100,
                FlatStyle = FlatStyle.Flat,
                BackColor = C_Panel,
                ForeColor = C_TextDim,
                Font      = new Font("Segoe UI", 10),
                Cursor    = Cursors.Hand
            };
            btnVolver.FlatAppearance.BorderSize = 0;
            btnVolver.Click += (s, e) => this.Close();

            Label lblNombre = new Label
            {
                Text      = "STEFY STORE",
                Font      = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = C_TextMain,
                Dock      = DockStyle.Left,
                Width     = 240,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding   = new Padding(16, 0, 0, 0)
            };

            Label lblPOS = new Label
            {
                Text      = "PUNTO DE VENTA",
                Font      = new Font("Segoe UI", 9),
                ForeColor = C_Accent,
                Dock      = DockStyle.Left,
                Width     = 160,
                TextAlign = ContentAlignment.MiddleLeft
            };

            lblHora = new Label
            {
                Text      = DateTime.Now.ToString("HH:mm:ss  |  dd/MM/yyyy"),
                Font      = new Font("Segoe UI Mono", 11),
                ForeColor = C_TextDim,
                Dock      = DockStyle.Right,
                Width     = 260,
                TextAlign = ContentAlignment.MiddleRight,
                Padding   = new Padding(0, 0, 20, 0)
            };

            header.Controls.Add(lblHora);
            header.Controls.Add(lblPOS);
            header.Controls.Add(lblNombre);
            header.Controls.Add(btnVolver);

            // ── LAYOUT: izquierda (ticket) + derecha (scanner/total) ─────────
            Panel panelIzq = new Panel { Dock = DockStyle.Fill,  BackColor = C_Bg,    Padding = new Padding(16, 12, 8,  12) };
            Panel panelDer = new Panel { Dock = DockStyle.Right, Width = 380, BackColor = C_Panel, Padding = new Padding(0) };

            // ══════════════════════════════════════════════════════════════════
            // PANEL IZQUIERDO — Ticket de venta
            // ══════════════════════════════════════════════════════════════════

            Label lblTicketTitle = new Label
            {
                Text      = "TICKET DE VENTA",
                Dock      = DockStyle.Top,
                Height    = 36,
                Font      = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = C_TextDim,
                TextAlign = ContentAlignment.BottomLeft,
                Padding   = new Padding(4, 0, 0, 4)
            };

            // Grid tipo ticket
            dgvTicket = new DataGridView
            {
                Dock                    = DockStyle.Fill,
                AllowUserToAddRows      = false,
                AllowUserToDeleteRows   = false,
                SelectionMode           = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor         = C_Card,
                BorderStyle             = BorderStyle.None,
                RowHeadersVisible       = false,
                GridColor               = C_Border,
                Font                    = new Font("Segoe UI", 11),
                RowTemplate             = { Height = 42 },
                CellBorderStyle         = DataGridViewCellBorderStyle.SingleHorizontal,
                ReadOnly                = true,
                AutoSizeColumnsMode     = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvTicket.ColumnHeadersDefaultCellStyle.BackColor  = C_Bg;
            dgvTicket.ColumnHeadersDefaultCellStyle.ForeColor  = C_TextDim;
            dgvTicket.ColumnHeadersDefaultCellStyle.Font       = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvTicket.ColumnHeadersDefaultCellStyle.Padding    = new Padding(8, 0, 0, 0);
            dgvTicket.ColumnHeadersHeight                      = 38;
            dgvTicket.EnableHeadersVisualStyles                = false;
            dgvTicket.DefaultCellStyle.BackColor               = C_Card;
            dgvTicket.DefaultCellStyle.ForeColor               = C_TextMain;
            dgvTicket.DefaultCellStyle.SelectionBackColor      = Color.FromArgb(60, 30, 50);
            dgvTicket.DefaultCellStyle.SelectionForeColor      = Color.White;
            dgvTicket.DefaultCellStyle.Padding                 = new Padding(8, 0, 0, 0);
            dgvTicket.AlternatingRowsDefaultCellStyle.BackColor= Color.FromArgb(26, 26, 44);
            dgvTicket.DataSource = dtTicket;
            dgvTicket.DataBindingComplete += ConfigurarColumnas;
            dgvTicket.CellClick          += DgvTicket_CellClick;

            // Barra inferior del ticket
            Panel barraTicket = new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 50,
                BackColor = C_Bg
            };

            Button btnQuitarItem = new Button
            {
                Text      = "✕  Quitar ítem seleccionado",
                Location  = new Point(0, 8),
                Size      = new Size(220, 36),
                BackColor = Color.FromArgb(100, 25, 25),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            btnQuitarItem.FlatAppearance.BorderSize = 0;
            btnQuitarItem.Click += BtnQuitarItem_Click;

            Button btnLimpiarTodo = new Button
            {
                Text      = "↺  Cancelar venta",
                Location  = new Point(228, 8),
                Size      = new Size(160, 36),
                BackColor = Color.FromArgb(55, 55, 75),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 9),
                Cursor    = Cursors.Hand
            };
            btnLimpiarTodo.FlatAppearance.BorderSize = 0;
            btnLimpiarTodo.Click += (s, e) =>
            {
                if (dtTicket.Rows.Count == 0) return;
                if (MessageBox.Show("¿Cancelar la venta actual?", "Confirmar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    dtTicket.Rows.Clear();
                    ActualizarTotales();
                    ActualizarBarraEdicion();
                }
            };

            barraTicket.Controls.Add(btnQuitarItem);
            barraTicket.Controls.Add(btnLimpiarTodo);

            panelIzq.Controls.Add(dgvTicket);
            panelIzq.Controls.Add(barraTicket);
            panelIzq.Controls.Add(lblTicketTitle);

            // ══════════════════════════════════════════════════════════════════
            // PANEL DERECHO — Scanner + Total
            // ══════════════════════════════════════════════════════════════════

            // ── 1. Código de barras ──────────────────────────────────────────
            Panel secScanner = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 130,
                BackColor = C_Panel,
                Padding   = new Padding(16, 10, 16, 10)
            };

            Label lblScanTitle = new Label
            {
                Text      = "ESCANEAR CÓDIGO DE BARRAS",
                Location  = new Point(16, 10),
                AutoSize  = true,
                Font      = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = C_TextDim
            };

            txtBarcode = new TextBox
            {
                Location    = new Point(16, 32),
                Size        = new Size(348, 40),
                Font        = new Font("Segoe UI Mono", 15, FontStyle.Bold),
                BackColor   = C_Card,
                ForeColor   = C_TextMain,
                BorderStyle = BorderStyle.FixedSingle,
                PlaceholderText = "  Escanee aquí..."
            };
            txtBarcode.KeyDown += TxtBarcode_KeyDown;

            lblMensaje = new Label
            {
                Location  = new Point(16, 80),
                Size      = new Size(348, 24),
                Font      = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = C_Green,
                Text      = ""
            };

            Label lblHint = new Label
            {
                Text      = "El escáner agrega el producto automáticamente",
                Location  = new Point(16, 104),
                Size      = new Size(348, 18),
                Font      = new Font("Segoe UI", 8),
                ForeColor = C_TextDim
            };

            secScanner.Controls.Add(lblScanTitle);
            secScanner.Controls.Add(txtBarcode);
            secScanner.Controls.Add(lblMensaje);
            secScanner.Controls.Add(lblHint);

            // ── 2. Búsqueda manual ───────────────────────────────────────────
            Panel secBuscar = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 115,
                BackColor = C_Panel,
                Padding   = new Padding(16, 6, 16, 6)
            };

            Panel divider1 = new Panel { Location = new Point(16, 0), Size = new Size(348, 1), BackColor = C_Border };

            Label lblBuscarTitle = new Label
            {
                Text      = "BÚSQUEDA MANUAL",
                Location  = new Point(16, 10),
                AutoSize  = true,
                Font      = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = C_TextDim
            };

            txtBuscarManual = new TextBox
            {
                Location    = new Point(16, 30),
                Size        = new Size(226, 34),
                Font        = new Font("Segoe UI", 11),
                BackColor   = C_Card,
                ForeColor   = C_TextMain,
                BorderStyle = BorderStyle.FixedSingle,
                PlaceholderText = "  Nombre o referencia..."
            };
            txtBuscarManual.KeyDown += TxtBuscarManual_KeyDown;

            // ── Contador CANT con − / número / + ────────────────────────────
            Label lblCantTitle = new Label
            {
                Text      = "CANT",
                Location  = new Point(252, 14),
                AutoSize  = true,
                Font      = new Font("Segoe UI", 7, FontStyle.Bold),
                ForeColor = C_TextDim
            };

            Button btnMenos = new Button
            {
                Text      = "−",
                Location  = new Point(250, 30),
                Size      = new Size(34, 34),
                BackColor = Color.FromArgb(50, 50, 75),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 14, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            btnMenos.FlatAppearance.BorderSize = 0;
            btnMenos.Click += BtnMenos_Click;

            lblCantTicket = new Label
            {
                Text      = "1",
                Location  = new Point(287, 30),
                Size      = new Size(36, 34),
                Font      = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter
            };

            Button btnMas = new Button
            {
                Text      = "+",
                Location  = new Point(326, 30),
                Size      = new Size(34, 34),
                BackColor = C_Accent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 14, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            btnMas.FlatAppearance.BorderSize = 0;
            btnMas.Click += BtnMas_Click;

            Button btnBuscarManual = new Button
            {
                Text      = "Agregar",
                Location  = new Point(16, 74),
                Size      = new Size(348, 32),
                BackColor = Color.FromArgb(45, 45, 70),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            btnBuscarManual.FlatAppearance.BorderSize = 0;
            btnBuscarManual.Click += BtnBuscarManual_Click;

            secBuscar.Controls.Add(divider1);
            secBuscar.Controls.Add(lblBuscarTitle);
            secBuscar.Controls.Add(txtBuscarManual);
            secBuscar.Controls.Add(lblCantTitle);
            secBuscar.Controls.Add(btnMenos);
            secBuscar.Controls.Add(lblCantTicket);
            secBuscar.Controls.Add(btnMas);
            secBuscar.Controls.Add(btnBuscarManual);

            // ── 3. Resumen ───────────────────────────────────────────────────
            Panel secResumen = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 80,
                BackColor = C_Panel,
                Padding   = new Padding(16, 8, 16, 8)
            };

            Panel divider2 = new Panel { Location = new Point(16, 0), Size = new Size(348, 1), BackColor = C_Border };

            lblItems = new Label
            {
                Text      = "0 artículos en el ticket",
                Location  = new Point(16, 12),
                Size      = new Size(348, 22),
                Font      = new Font("Segoe UI", 10),
                ForeColor = C_TextDim
            };

            Label lblTotalLabel = new Label
            {
                Text      = "TOTAL A COBRAR",
                Location  = new Point(16, 36),
                AutoSize  = true,
                Font      = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = C_TextDim
            };

            lblTotal = new Label
            {
                Text      = "$0.00",
                Location  = new Point(16, 52),
                AutoSize  = true,
                Font      = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White
            };

            secResumen.Controls.Add(divider2);
            secResumen.Controls.Add(lblItems);
            secResumen.Controls.Add(lblTotalLabel);
            secResumen.Controls.Add(lblTotal);

            // ── 4. Botón cobrar ──────────────────────────────────────────────
            Panel secCobrar = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = C_Panel,
                Padding   = new Padding(16, 10, 16, 20)
            };

            // Total grande
            Panel cardTotal = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 100,
                BackColor = C_Bg,
                Padding   = new Padding(12)
            };

            Label lblTotalGrandeLabel = new Label
            {
                Text      = "TOTAL",
                Dock      = DockStyle.Top,
                Height    = 22,
                Font      = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = C_TextDim,
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblTotalGrande = new Label
            {
                Name      = "lblTotalGrande",
                Text      = "$0.00",
                Dock      = DockStyle.Fill,
                Font      = new Font("Segoe UI", 30, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter
            };

            cardTotal.Controls.Add(lblTotalGrande);
            cardTotal.Controls.Add(lblTotalGrandeLabel);

            // Vincular el lblTotal al label grande también
            dtTicket.RowChanged  += (s, e) => { ((Label)cardTotal.Controls["lblTotalGrande"]).Text = lblTotal.Text; };
            dtTicket.RowDeleted  += (s, e) => { ((Label)cardTotal.Controls["lblTotalGrande"]).Text = lblTotal.Text; };

            Button btnCobrar = new Button
            {
                Text      = "  COBRAR VENTA",
                Dock      = DockStyle.Bottom,
                Height    = 70,
                BackColor = C_Green,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 16, FontStyle.Bold),
                Cursor    = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            btnCobrar.FlatAppearance.BorderSize = 0;
            btnCobrar.MouseEnter += (s, e) => btnCobrar.BackColor = C_GreenHov;
            btnCobrar.MouseLeave += (s, e) => btnCobrar.BackColor = C_Green;
            btnCobrar.Click += BtnCobrar_Click;

            secCobrar.Controls.Add(btnCobrar);
            secCobrar.Controls.Add(cardTotal);

            // Ensamblaje panel derecho (de abajo a arriba por Dock.Top)
            panelDer.Controls.Add(secCobrar);
            panelDer.Controls.Add(secResumen);
            panelDer.Controls.Add(secBuscar);
            panelDer.Controls.Add(secScanner);

            // Ensamblaje final
            this.Controls.Add(panelIzq);
            this.Controls.Add(panelDer);
            this.Controls.Add(header);
        }

        private void CargarTimers()
        {
            // Reloj
            timerReloj = new Timer { Interval = 1000 };
            timerReloj.Tick += (s, e) => lblHora.Text = DateTime.Now.ToString("HH:mm:ss  |  dd/MM/yyyy");
            timerReloj.Start();

            // Mantener foco en el campo de código de barras
            timerFoco = new Timer { Interval = 500 };
            timerFoco.Tick += (s, e) =>
            {
                Control foco = this.ActiveControl;
                if (foco != txtBarcode && foco != txtBuscarManual)
                    txtBarcode.Focus();
            };
            timerFoco.Start();

            // Limpiar mensaje después de 2 seg
            timerMensaje = new Timer { Interval = 2000 };
            timerMensaje.Tick += (s, e) => { lblMensaje.Text = ""; timerMensaje.Stop(); };
        }

        // ── Columnas del grid ─────────────────────────────────────────────────
        private void ConfigurarColumnas(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (dgvTicket.Columns.Count == 0) return;
            if (dgvTicket.Columns.Contains("ProductoId")) dgvTicket.Columns["ProductoId"].Visible = false;
            if (dgvTicket.Columns.Contains("Codigo"))
            {
                dgvTicket.Columns["Codigo"].HeaderText = "Código";
                dgvTicket.Columns["Codigo"].FillWeight = 15;
            }
            if (dgvTicket.Columns.Contains("Descripcion"))
            {
                dgvTicket.Columns["Descripcion"].HeaderText = "Descripción";
                dgvTicket.Columns["Descripcion"].FillWeight = 40;
            }
            if (dgvTicket.Columns.Contains("Talla"))
            {
                dgvTicket.Columns["Talla"].HeaderText = "Talla";
                dgvTicket.Columns["Talla"].FillWeight = 10;
            }
            if (dgvTicket.Columns.Contains("Precio"))
            {
                dgvTicket.Columns["Precio"].HeaderText = "Precio";
                dgvTicket.Columns["Precio"].FillWeight = 15;
            }
            if (dgvTicket.Columns.Contains("Cant"))
            {
                dgvTicket.Columns["Cant"].HeaderText = "Cant.";
                dgvTicket.Columns["Cant"].FillWeight = 10;
            }
            if (dgvTicket.Columns.Contains("Subtotal"))
            {
                dgvTicket.Columns["Subtotal"].HeaderText = "Subtotal";
                dgvTicket.Columns["Subtotal"].FillWeight = 15;
            }
        }

        // ── Escáner de código de barras (Enter) ───────────────────────────────
        private void TxtBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;

            string codigo = txtBarcode.Text.Trim();
            txtBarcode.Clear();
            if (string.IsNullOrEmpty(codigo)) return;

            BuscarYAgregarPorCodigo(codigo, 1);
        }

        // ── Búsqueda manual (Enter en el campo) ───────────────────────────────
        private void TxtBuscarManual_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; BtnBuscarManual_Click(sender, e); }
        }

        private void BtnBuscarManual_Click(object sender, EventArgs e)
        {
            string termino = txtBuscarManual.Text.Trim();
            if (string.IsNullOrEmpty(termino)) return;

            if (!int.TryParse(lblCantTicket.Text, out int cantidad) || cantidad <= 0)
            {
                MostrarMensaje("Cantidad inválida.", esError: true);
                return;
            }

            BuscarYAgregarPorTexto(termino, cantidad);
            txtBuscarManual.Clear();
            lblCantTicket.Text = "1";   // resetear contador
        }

        // ── Lógica de búsqueda ────────────────────────────────────────────────
        private void BuscarYAgregarPorCodigo(string referencia, int cantidad)
        {
            try
            {
                using (MySqlConnection conn = ConexionBD.ObtenerConexion())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(
                        "SELECT Id, Referencia, Descripcion, Talla, PrecioVenta, Unidades FROM productos WHERE Referencia=@r LIMIT 1", conn);
                    cmd.Parameters.AddWithValue("@r", referencia);

                    using (MySqlDataReader r = cmd.ExecuteReader())
                    {
                        if (!r.Read())
                        {
                            MostrarMensaje($"Código '{referencia}' no encontrado.", esError: true);
                            return;
                        }
                        AgregarAlTicket(
                            Convert.ToInt32(r["Id"]),
                            r["Referencia"].ToString(),
                            r["Descripcion"].ToString(),
                            r["Talla"].ToString(),
                            Convert.ToDecimal(r["PrecioVenta"]),
                            Convert.ToInt32(r["Unidades"]),
                            cantidad);
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error: " + ex.Message, esError: true);
            }
        }

        private void BuscarYAgregarPorTexto(string termino, int cantidad)
        {
            try
            {
                using (MySqlConnection conn = ConexionBD.ObtenerConexion())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(@"
                        SELECT Id, Referencia, Descripcion, Talla, PrecioVenta, Unidades
                        FROM productos
                        WHERE Unidades > 0
                          AND (Descripcion LIKE @t OR Referencia LIKE @t OR Marca LIKE @t)
                        LIMIT 1", conn);
                    cmd.Parameters.AddWithValue("@t", "%" + termino + "%");

                    using (MySqlDataReader r = cmd.ExecuteReader())
                    {
                        if (!r.Read())
                        {
                            MostrarMensaje($"'{termino}' no encontrado.", esError: true);
                            return;
                        }
                        AgregarAlTicket(
                            Convert.ToInt32(r["Id"]),
                            r["Referencia"].ToString(),
                            r["Descripcion"].ToString(),
                            r["Talla"].ToString(),
                            Convert.ToDecimal(r["PrecioVenta"]),
                            Convert.ToInt32(r["Unidades"]),
                            cantidad);
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error: " + ex.Message, esError: true);
            }
        }

        private void AgregarAlTicket(int id, string codigo, string desc, string talla, decimal precio, int stock, int cantidad)
        {
            // ¿Ya está en el ticket?
            foreach (DataRow row in dtTicket.Rows)
            {
                if (Convert.ToInt32(row["ProductoId"]) == id)
                {
                    int nueva = Convert.ToInt32(row["Cant"]) + cantidad;
                    if (nueva > stock)
                    {
                        MostrarMensaje($"Stock máximo: {stock} unidades.", esError: true);
                        return;
                    }
                    row["Cant"]     = nueva;
                    row["Subtotal"] = precio * nueva;
                    ActualizarTotales();
                    MostrarMensaje($"✔  {desc} — cantidad actualizada a {nueva}");
                    return;
                }
            }

            if (cantidad > stock)
            {
                MostrarMensaje($"Stock insuficiente ({stock} disponibles).", esError: true);
                return;
            }

            dtTicket.Rows.Add(id, codigo, desc, talla, precio, cantidad, precio * cantidad);
            ActualizarTotales();
            MostrarMensaje($"✔  {desc} agregado");
        }

        // ── Acciones sobre el ticket ─────────────────────────────────────────
        private void BtnQuitarItem_Click(object sender, EventArgs e)
        {
            if (dgvTicket.CurrentRow == null || dgvTicket.CurrentRow.Index < 0) return;
            dtTicket.Rows.RemoveAt(dgvTicket.CurrentRow.Index);
            ActualizarTotales();
            ActualizarBarraEdicion();
        }

        private void ActualizarTotales()
        {
            decimal total = 0;
            int items = 0;
            foreach (DataRow row in dtTicket.Rows)
            {
                total += Convert.ToDecimal(row["Subtotal"]);
                items += Convert.ToInt32(row["Cant"]);
            }
            lblTotal.Text = "$" + total.ToString("N2");
            lblItems.Text = $"{items} artículo{(items != 1 ? "s" : "")} en el ticket";

            // Actualizar el total grande
            var lblGrande = this.Controls.Find("lblTotalGrande", true);
            if (lblGrande.Length > 0) lblGrande[0].Text = lblTotal.Text;
        }

        // ── F12 = cobrar desde teclado ────────────────────────────────────────
        private void FrmVentas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F12) BtnCobrar_Click(sender, e);
            if (e.KeyCode == Keys.Escape && dtTicket.Rows.Count > 0)
            {
                if (MessageBox.Show("¿Cancelar la venta actual?", "Confirmar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    dtTicket.Rows.Clear();
                    ActualizarTotales();
                }
            }
        }

        // ── Procesar venta ────────────────────────────────────────────────────
        private void BtnCobrar_Click(object sender, EventArgs e)
        {
            if (dtTicket.Rows.Count == 0)
            {
                MostrarMensaje("El ticket está vacío.", esError: true);
                return;
            }

            decimal total = 0;
            foreach (DataRow row in dtTicket.Rows)
                total += Convert.ToDecimal(row["Subtotal"]);

            if (MessageBox.Show(
                $"Total a cobrar: {total:C2}\n\n¿Confirmar la venta?",
                "Confirmar venta",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                using (MySqlConnection conn = ConexionBD.ObtenerConexion())
                {
                    conn.Open();
                    using (MySqlTransaction tx = conn.BeginTransaction())
                    {
                        MySqlCommand cmdV = new MySqlCommand(
                            "INSERT INTO ventas (Fecha, Total, Tienda) VALUES (@f, @t, @tienda)", conn, tx);
                        cmdV.Parameters.AddWithValue("@f", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmdV.Parameters.AddWithValue("@t", total);
                        cmdV.Parameters.AddWithValue("@tienda", Config.NombreTienda);
                        cmdV.ExecuteNonQuery();
                        long ventaId = cmdV.LastInsertedId;

                        foreach (DataRow row in dtTicket.Rows)
                        {
                            int     pid      = Convert.ToInt32(row["ProductoId"]);
                            int     cantidad = Convert.ToInt32(row["Cant"]);
                            decimal precio   = Convert.ToDecimal(row["Precio"]);
                            decimal subtotal = Convert.ToDecimal(row["Subtotal"]);

                            MySqlCommand cmdChk = new MySqlCommand("SELECT Unidades FROM productos WHERE Id=@id", conn, tx);
                            cmdChk.Parameters.AddWithValue("@id", pid);
                            int stockActual = Convert.ToInt32(cmdChk.ExecuteScalar());

                            if (cantidad > stockActual)
                            {
                                tx.Rollback();
                                MessageBox.Show($"Stock insuficiente para '{row["Descripcion"]}'. Disponible: {stockActual}.",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            MySqlCommand cmdD = new MySqlCommand(@"
                                INSERT INTO detalle_ventas (VentaId,ProductoId,Descripcion,Talla,Cantidad,PrecioUnitario,Subtotal)
                                VALUES (@v,@p,@d,@ta,@c,@pr,@s)", conn, tx);
                            cmdD.Parameters.AddWithValue("@v",  ventaId);
                            cmdD.Parameters.AddWithValue("@p",  pid);
                            cmdD.Parameters.AddWithValue("@d",  row["Descripcion"].ToString());
                            cmdD.Parameters.AddWithValue("@ta", row["Talla"].ToString());
                            cmdD.Parameters.AddWithValue("@c",  cantidad);
                            cmdD.Parameters.AddWithValue("@pr", precio);
                            cmdD.Parameters.AddWithValue("@s",  subtotal);
                            cmdD.ExecuteNonQuery();

                            MySqlCommand cmdU = new MySqlCommand(
                                "UPDATE productos SET Unidades=Unidades-@c WHERE Id=@id", conn, tx);
                            cmdU.Parameters.AddWithValue("@c",  cantidad);
                            cmdU.Parameters.AddWithValue("@id", pid);
                            cmdU.ExecuteNonQuery();
                        }

                        tx.Commit();
                    }
                }

                MessageBox.Show($"Venta registrada.\nTotal cobrado: {total:C2}",
                    "Venta exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);

                dtTicket.Rows.Clear();
                ActualizarTotales();
                ActualizarBarraEdicion();
                txtBarcode.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al procesar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Editor de cantidad en el carrito ──────────────────────────────────

        // Cuando se hace clic en el carrito, el contador refleja la cantidad del ítem
        private void DgvTicket_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            ActualizarBarraEdicion();
        }

        private void ActualizarBarraEdicion()
        {
            if (dgvTicket.CurrentRow == null || dgvTicket.CurrentRow.Index < 0
                || dtTicket.Rows.Count == 0)
            {
                lblCantTicket.Text = "1";   // sin selección: contador en 1 para agregar
                return;
            }
            int cant = Convert.ToInt32(dtTicket.Rows[dgvTicket.CurrentRow.Index]["Cant"]);
            lblCantTicket.Text = cant.ToString();
        }

        private void BtnMas_Click(object sender, EventArgs e)
        {
            // ¿Hay un ítem seleccionado en el carrito?
            if (dgvTicket.CurrentRow != null && dgvTicket.CurrentRow.Index >= 0
                && dtTicket.Rows.Count > 0)
            {
                int idx        = dgvTicket.CurrentRow.Index;
                int pid        = Convert.ToInt32(dtTicket.Rows[idx]["ProductoId"]);
                int cantActual = Convert.ToInt32(dtTicket.Rows[idx]["Cant"]);
                decimal precio = Convert.ToDecimal(dtTicket.Rows[idx]["Precio"]);

                int stock = ObtenerStock(pid);
                if (cantActual >= stock)
                {
                    MostrarMensaje($"Stock máximo: {stock} unidades.", esError: true);
                    return;
                }
                int nueva = cantActual + 1;
                dtTicket.Rows[idx]["Cant"]     = nueva;
                dtTicket.Rows[idx]["Subtotal"] = precio * nueva;
                ActualizarTotales();
                lblCantTicket.Text = nueva.ToString();
            }
            else
            {
                // Sin selección: ajustar contador de cantidad para agregar
                if (int.TryParse(lblCantTicket.Text, out int n))
                    lblCantTicket.Text = (n + 1).ToString();
            }
        }

        private void BtnMenos_Click(object sender, EventArgs e)
        {
            // ¿Hay un ítem seleccionado en el carrito?
            if (dgvTicket.CurrentRow != null && dgvTicket.CurrentRow.Index >= 0
                && dtTicket.Rows.Count > 0)
            {
                int idx        = dgvTicket.CurrentRow.Index;
                int cantActual = Convert.ToInt32(dtTicket.Rows[idx]["Cant"]);

                if (cantActual <= 1)
                {
                    if (MessageBox.Show("¿Quitar este producto del ticket?", "Confirmar",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        dtTicket.Rows.RemoveAt(idx);
                        ActualizarTotales();
                        ActualizarBarraEdicion();
                    }
                    return;
                }
                decimal precio = Convert.ToDecimal(dtTicket.Rows[idx]["Precio"]);
                int nueva = cantActual - 1;
                dtTicket.Rows[idx]["Cant"]     = nueva;
                dtTicket.Rows[idx]["Subtotal"] = precio * nueva;
                ActualizarTotales();
                lblCantTicket.Text = nueva.ToString();
            }
            else
            {
                // Sin selección: ajustar contador de cantidad para agregar (mínimo 1)
                if (int.TryParse(lblCantTicket.Text, out int n) && n > 1)
                    lblCantTicket.Text = (n - 1).ToString();
            }
        }

        private int ObtenerStock(int productoId)
        {
            try
            {
                using (MySqlConnection conn = ConexionBD.ObtenerConexion())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(
                        "SELECT Unidades FROM productos WHERE Id=@id", conn);
                    cmd.Parameters.AddWithValue("@id", productoId);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch { return 0; }
        }

        // ── Mensajes de feedback ──────────────────────────────────────────────
        private void MostrarMensaje(string texto, bool esError = false)
        {
            lblMensaje.ForeColor = esError ? Color.FromArgb(220, 80, 80) : C_Green;
            lblMensaje.Text = texto;
            timerMensaje.Stop();
            timerMensaje.Start();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            timerReloj?.Stop();
            timerFoco?.Stop();
            timerMensaje?.Stop();
            base.OnFormClosed(e);
        }
    }
}
