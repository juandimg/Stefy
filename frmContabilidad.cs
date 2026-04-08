using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySqlConnector;

namespace Angela
{
    public class frmContabilidad : Form
    {
        // ── Paleta de colores
        private static readonly Color ColorSidebar    = Color.FromArgb(28, 28, 45);
        private static readonly Color ColorSidebarAlt = Color.FromArgb(38, 38, 60);
        private static readonly Color ColorAccent     = Color.FromArgb(100, 40, 160);
        private static readonly Color ColorAccentHov  = Color.FromArgb(130, 60, 190);
        private static readonly Color ColorFondo      = Color.FromArgb(242, 242, 248);
        private static readonly Color ColorCard       = Color.White;
        private static readonly Color ColorTextoSide  = Color.FromArgb(190, 190, 210);
        private static readonly Color ColorTextoLight = Color.FromArgb(240, 240, 255);
        private static readonly Color ColorGreen      = Color.FromArgb(27, 150, 90);
        private static readonly Color ColorRed        = Color.FromArgb(180, 40, 40);
        private static readonly Color ColorRedHov     = Color.FromArgb(210, 60, 60);

        // ── Controles ─────────────────────────────────────────────────────────
        private DateTimePicker dtpDesde, dtpHasta;
        private DataGridView   dgvVentas, dgvGastos;
        private Label          lblTotalVentas, lblTotalGastos, lblCostoMercancia, lblUtilidad;
        private TextBox        txtConcepto, txtMontoGasto;
        private ComboBox       cmbCategoriaGasto;
        private Form           formPadre;
        private int            _gastoEditandoId = -1;
        private Button         btnRegistrarGasto;

        public frmContabilidad(Form padre)
        {
            formPadre = padre;
            InicializarComponentes();
            FiltrarPorPeriodo("mes");
        }

        // ══════════════════════════════════════════════════════════════════════
        // UI
        // ══════════════════════════════════════════════════════════════════════

        private void InicializarComponentes()
        {
            this.Text        = "Contabilidad — Stefy Store";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor   = ColorFondo;
            this.Font        = new Font("Segoe UI", 10);
            this.FormClosed += (s, e) => formPadre.Show();

            // ── HEADER ───────────────────────────────────────────────────────
            Panel header = new Panel { Dock = DockStyle.Top, Height = 62, BackColor = ColorSidebar };

            Button btnVolver = new Button {
                Text = "‹  Volver", Dock = DockStyle.Left, Width = 110,
                FlatStyle = FlatStyle.Flat, BackColor = ColorSidebar,
                ForeColor = ColorTextoSide, Font = new Font("Segoe UI", 10), Cursor = Cursors.Hand
            };
            btnVolver.FlatAppearance.BorderSize = 0;
            btnVolver.Click += (s, e) => this.Close();

            Label lblTitulo = new Label {
                Text = "CONTABILIDAD Y REPORTES",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = ColorTextoLight, Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblSub = new Label {
                Text = "STEFY STORE", Font = new Font("Segoe UI", 8),
                ForeColor = ColorAccent, Dock = DockStyle.Right,
                Width = 110, TextAlign = ContentAlignment.MiddleCenter
            };

            header.Controls.Add(lblTitulo);
            header.Controls.Add(btnVolver);
            header.Controls.Add(lblSub);

            // ── SIDEBAR ───────────────────────────────────────────────────────
            Panel sidebar = new Panel { Dock = DockStyle.Left, Width = 310, BackColor = ColorSidebar };
            Panel scrollSide = new Panel { Dock = DockStyle.Fill, AutoScroll = true };

            int x = 20, y = 0;

            // Título filtros
            scrollSide.Controls.Add(new Label {
                Text = "FILTROS", Location = new Point(x, y + 18), AutoSize = true,
                Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = ColorAccent
            });
            y += 52;

            // Botones período rápido (fila 1)
            scrollSide.Controls.Add(CrearLabelSide("Período rápido", y)); y += 22;
            var btnHoy    = CrearBotonFiltro("Hoy",        y, x);
            var btnSemana = CrearBotonFiltro("Esta semana", y, x + 130);
            btnHoy.Click    += (s, e) => FiltrarPorPeriodo("hoy");
            btnSemana.Click += (s, e) => FiltrarPorPeriodo("semana");
            scrollSide.Controls.Add(btnHoy);
            scrollSide.Controls.Add(btnSemana);
            y += 36;

            // Botones período rápido (fila 2)
            var btnMes  = CrearBotonFiltro("Este mes", y, x);
            var btnTodo = CrearBotonFiltro("Todo",      y, x + 130);
            btnMes.Click  += (s, e) => FiltrarPorPeriodo("mes");
            btnTodo.Click += (s, e) => FiltrarPorPeriodo("todo");
            scrollSide.Controls.Add(btnMes);
            scrollSide.Controls.Add(btnTodo);
            y += 46;

            // Rango personalizado
            scrollSide.Controls.Add(CrearLabelSide("Desde", y)); y += 22;
            dtpDesde = new DateTimePicker {
                Location = new Point(x, y), Size = new Size(270, 32),
                Format = DateTimePickerFormat.Short, Font = new Font("Segoe UI", 10)
            };
            dtpDesde.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            scrollSide.Controls.Add(dtpDesde); y += 42;

            scrollSide.Controls.Add(CrearLabelSide("Hasta", y)); y += 22;
            dtpHasta = new DateTimePicker {
                Location = new Point(x, y), Size = new Size(270, 32),
                Format = DateTimePickerFormat.Short, Font = new Font("Segoe UI", 10)
            };
            dtpHasta.Value = DateTime.Now;
            scrollSide.Controls.Add(dtpHasta); y += 52;

            var btnAplicar = CrearBotonSide("  Aplicar Filtro", y, ColorAccent, ColorAccentHov);
            btnAplicar.Click += (s, e) => CargarDatos(dtpDesde.Value, dtpHasta.Value);
            scrollSide.Controls.Add(btnAplicar); y += 54;

            // Separador
            scrollSide.Controls.Add(new Panel {
                Location = new Point(x, y), Size = new Size(270, 1), BackColor = ColorSidebarAlt
            });
            y += 16;

            // ── Sección gastos ───────────────────────────────────────────────
            scrollSide.Controls.Add(new Label {
                Text = "REGISTRAR GASTO", Location = new Point(x, y), AutoSize = true,
                Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = ColorRed
            });
            y += 30;

            scrollSide.Controls.Add(CrearLabelSide("Categoría", y)); y += 22;
            cmbCategoriaGasto = CrearCombo(y, new[] {
                "Arriendo", "Servicios", "Transporte", "Marketing",
                "Compras", "Salarios", "Mantenimiento", "Otro"
            });
            scrollSide.Controls.Add(cmbCategoriaGasto); y += 42;

            scrollSide.Controls.Add(CrearLabelSide("Concepto", y)); y += 22;
            txtConcepto = CrearInput(y); scrollSide.Controls.Add(txtConcepto); y += 42;

            scrollSide.Controls.Add(CrearLabelSide("Monto ($)", y)); y += 22;
            txtMontoGasto = CrearInput(y); scrollSide.Controls.Add(txtMontoGasto); y += 52;

            btnRegistrarGasto = CrearBotonSide("  Registrar Gasto", y, ColorRed, ColorRedHov);
            btnRegistrarGasto.Click += BtnRegistrarGasto_Click;
            scrollSide.Controls.Add(btnRegistrarGasto); y += 46;

            var btnCancelar = CrearBotonSide("  Cancelar edición", y, ColorSidebarAlt, Color.FromArgb(55, 55, 85));
            btnCancelar.Click += (s, e) => CancelarEdicion();
            scrollSide.Controls.Add(btnCancelar);

            sidebar.Controls.Add(scrollSide);

            // ── ÁREA DERECHA ──────────────────────────────────────────────────
            Panel panelDer = new Panel {
                Dock = DockStyle.Fill, BackColor = ColorFondo,
                Padding = new Padding(16, 12, 16, 12)
            };

            // Tarjetas de resumen
            Panel panelStats = new Panel {
                Dock = DockStyle.Top, Height = 100,
                BackColor = ColorFondo
            };

            Panel cardVentas   = CrearCard("INGRESOS (VENTAS)",   "$0.00", ColorGreen);
            Panel cardGastos   = CrearCard("EGRESOS (GASTOS)",    "$0.00", ColorRed);
            Panel cardCosto    = CrearCard("COSTO MERCANCÍA",     "$0.00", Color.FromArgb(48, 63, 159));
            Panel cardUtilidad = CrearCard("UTILIDAD NETA",       "$0.00", Color.FromArgb(183, 28, 90));

            lblTotalVentas    = (Label)cardVentas.Controls["lblValor"];
            lblTotalGastos    = (Label)cardGastos.Controls["lblValor"];
            lblCostoMercancia = (Label)cardCosto.Controls["lblValor"];
            lblUtilidad       = (Label)cardUtilidad.Controls["lblValor"];

            panelStats.Controls.Add(cardUtilidad);
            panelStats.Controls.Add(cardCosto);
            panelStats.Controls.Add(cardGastos);
            panelStats.Controls.Add(cardVentas);
            panelStats.Resize += (s, e) =>
                DistribuirCards(panelStats, cardVentas, cardGastos, cardCosto, cardUtilidad);

            // Tabs: Ventas / Gastos
            TabControl tabs = new TabControl {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10)
            };

            TabPage tabVentas = new TabPage("  Historial de Ventas  ") { BackColor = ColorCard };
            TabPage tabGastos = new TabPage("  Registro de Gastos  ")  { BackColor = ColorCard };

            // Tab ventas
            dgvVentas = CrearGrid();
            dgvVentas.CellDoubleClick += DgvVentas_CellDoubleClick;

            Panel panelBotonesVenta = new Panel { Dock = DockStyle.Bottom, Height = 40, BackColor = ColorFondo };

            Button btnVerDetalle = new Button {
                Text = "🔍  Ver Detalle",
                Dock = DockStyle.Left, Width = 180,
                BackColor = Color.FromArgb(48, 63, 159), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand
            };
            btnVerDetalle.FlatAppearance.BorderSize = 0;
            btnVerDetalle.Click += (s, e) => {
                if (dgvVentas.CurrentRow == null || dgvVentas.CurrentRow.Index < 0) return;
                int id = Convert.ToInt32(dgvVentas.CurrentRow.Cells["Id"].Value);
                string fec = dgvVentas.CurrentRow.Cells["Fecha"].Value.ToString();
                MostrarDetalleVenta(id, fec);
            };

            Button btnEditarVenta = new Button {
                Text = "✎  Editar Venta",
                Dock = DockStyle.Left, Width = 180,
                BackColor = Color.FromArgb(60, 100, 180), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand
            };
            btnEditarVenta.FlatAppearance.BorderSize = 0;
            btnEditarVenta.Click += BtnEditarVenta_Click;

            Button btnEliminarVenta = new Button {
                Text = "✕  Eliminar Venta",
                Dock = DockStyle.Left, Width = 180,
                BackColor = Color.FromArgb(140, 30, 30), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand
            };
            btnEliminarVenta.FlatAppearance.BorderSize = 0;
            btnEliminarVenta.Click += BtnEliminarVenta_Click;

            panelBotonesVenta.Controls.Add(btnEliminarVenta);
            panelBotonesVenta.Controls.Add(btnEditarVenta);
            panelBotonesVenta.Controls.Add(btnVerDetalle);
            tabVentas.Controls.Add(dgvVentas);
            tabVentas.Controls.Add(panelBotonesVenta);

            // Tab gastos
            dgvGastos = CrearGrid();

            Panel panelBotonesGasto = new Panel { Dock = DockStyle.Bottom, Height = 40, BackColor = ColorFondo };

            Button btnEditarGasto = new Button {
                Text = "✎  Editar gasto",
                Dock = DockStyle.Left, Width = 200,
                BackColor = Color.FromArgb(60, 100, 180), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand
            };
            btnEditarGasto.FlatAppearance.BorderSize = 0;
            btnEditarGasto.Click += BtnEditarGasto_Click;

            Button btnEliminarGasto = new Button {
                Text = "✕  Eliminar gasto",
                Dock = DockStyle.Left, Width = 200,
                BackColor = Color.FromArgb(140, 30, 30), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand
            };
            btnEliminarGasto.FlatAppearance.BorderSize = 0;
            btnEliminarGasto.Click += BtnEliminarGasto_Click;

            panelBotonesGasto.Controls.Add(btnEliminarGasto);
            panelBotonesGasto.Controls.Add(btnEditarGasto);
            tabGastos.Controls.Add(dgvGastos);
            tabGastos.Controls.Add(panelBotonesGasto);

            tabs.TabPages.Add(tabVentas);
            tabs.TabPages.Add(tabGastos);

            panelDer.Controls.Add(tabs);
            panelDer.Controls.Add(panelStats);

            // Ensamblaje final
            this.Controls.Add(panelDer);
            this.Controls.Add(sidebar);
            this.Controls.Add(header);

            this.Shown += (s, e) =>
                DistribuirCards(panelStats, cardVentas, cardGastos, cardCosto, cardUtilidad);
        }

        // ══════════════════════════════════════════════════════════════════════
        // LÓGICA DE DATOS
        // ══════════════════════════════════════════════════════════════════════

        private void FiltrarPorPeriodo(string periodo)
        {
            DateTime hasta  = DateTime.Now;
            DateTime desde;
            switch (periodo)
            {
                case "hoy":
                    desde = DateTime.Today;
                    break;
                case "semana":
                    desde = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                    break;
                case "mes":
                    desde = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    break;
                default: // todo
                    desde = new DateTime(2000, 1, 1);
                    break;
            }
            dtpDesde.Value = desde;
            dtpHasta.Value = hasta;
            CargarDatos(desde, hasta);
        }

        private void CargarDatos(DateTime desde, DateTime hasta)
        {
            CargarVentas(desde, hasta);
            CargarGastos(desde, hasta);
            ActualizarResumen(desde, hasta);
        }

        private void CargarVentas(DateTime desde, DateTime hasta)
        {
            try
            {
                using (var conn = ConexionBD.ObtenerConexion())
                {
                    conn.Open();
                    var cmd = new MySqlCommand(@"
                        SELECT v.Fecha, v.Total, v.Tienda, 'Contado' AS Tipo
                        FROM ventas v
                        WHERE v.Fecha >= @d AND v.Fecha <= @h
                          AND NOT EXISTS (SELECT 1 FROM creditos c WHERE c.VentaId = v.Id)
                        UNION ALL
                        SELECT a.Fecha, a.Monto AS Total, a.ClienteNombre AS Tienda, 'Abono' AS Tipo
                        FROM abonos a
                        WHERE a.Fecha >= @d AND a.Fecha <= @h
                        ORDER BY Fecha DESC", conn);
                    cmd.Parameters.AddWithValue("@d", desde.ToString("yyyy-MM-dd") + " 00:00:00");
                    cmd.Parameters.AddWithValue("@h", hasta.ToString("yyyy-MM-dd")  + " 23:59:59");
                    var dt = new DataTable();
                    dt.Load(cmd.ExecuteReader());
                    dgvVentas.DataSource = dt;
                    ConfigurarColumnasVentas();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar ventas: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarGastos(DateTime desde, DateTime hasta)
        {
            try
            {
                using (var conn = ConexionBD.ObtenerConexion())
                {
                    conn.Open();
                    var cmd = new MySqlCommand(@"
                        SELECT Id, Fecha, Categoria, Concepto, Monto
                        FROM gastos
                        WHERE Fecha >= @d AND Fecha <= @h
                        ORDER BY Fecha DESC", conn);
                    cmd.Parameters.AddWithValue("@d", desde.ToString("yyyy-MM-dd") + " 00:00:00");
                    cmd.Parameters.AddWithValue("@h", hasta.ToString("yyyy-MM-dd")  + " 23:59:59");
                    var dt = new DataTable();
                    dt.Load(cmd.ExecuteReader());
                    dgvGastos.DataSource = dt;
                    ConfigurarColumnasGastos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar gastos: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ActualizarResumen(DateTime desde, DateTime hasta)
        {
            try
            {
                using (var conn = ConexionBD.ObtenerConexion())
                {
                    conn.Open();
                    string d = desde.ToString("yyyy-MM-dd") + " 00:00:00";
                    string h = hasta.ToString("yyyy-MM-dd")  + " 23:59:59";

                    var cmdV = new MySqlCommand(
                        @"SELECT COALESCE(SUM(v.Total),0) FROM ventas v
                          WHERE v.Fecha>=@d AND v.Fecha<=@h
                            AND NOT EXISTS (SELECT 1 FROM creditos c WHERE c.VentaId = v.Id)", conn);
                    cmdV.Parameters.AddWithValue("@d", d);
                    cmdV.Parameters.AddWithValue("@h", h);
                    decimal totalVentas = Convert.ToDecimal(cmdV.ExecuteScalar());

                    var cmdAb = new MySqlCommand(
                        "SELECT COALESCE(SUM(Monto),0) FROM abonos WHERE Fecha>=@d AND Fecha<=@h", conn);
                    cmdAb.Parameters.AddWithValue("@d", d);
                    cmdAb.Parameters.AddWithValue("@h", h);
                    totalVentas += Convert.ToDecimal(cmdAb.ExecuteScalar());

                    var cmdC = new MySqlCommand(@"
                        SELECT COALESCE(SUM(dv.Cantidad * p.PrecioCompra), 0)
                        FROM detalle_ventas dv
                        JOIN ventas v    ON dv.VentaId    = v.Id
                        JOIN productos p ON dv.ProductoId = p.Id
                        WHERE v.Fecha >= @d AND v.Fecha <= @h
                          AND NOT EXISTS (SELECT 1 FROM creditos c WHERE c.VentaId = v.Id)", conn);
                    cmdC.Parameters.AddWithValue("@d", d);
                    cmdC.Parameters.AddWithValue("@h", h);
                    decimal costoMercancia = Convert.ToDecimal(cmdC.ExecuteScalar());

                    var cmdG = new MySqlCommand(
                        "SELECT COALESCE(SUM(Monto),0) FROM gastos WHERE Fecha>=@d AND Fecha<=@h", conn);
                    cmdG.Parameters.AddWithValue("@d", d);
                    cmdG.Parameters.AddWithValue("@h", h);
                    decimal totalGastos = Convert.ToDecimal(cmdG.ExecuteScalar());

                    decimal utilidad = totalVentas - costoMercancia - totalGastos;

                    lblTotalVentas.Text    = "$" + totalVentas.ToString("N2");
                    lblTotalGastos.Text    = "$" + totalGastos.ToString("N2");
                    lblCostoMercancia.Text = "$" + costoMercancia.ToString("N2");
                    lblUtilidad.Text       = "$" + utilidad.ToString("N2");
                    lblUtilidad.ForeColor  = utilidad >= 0
                        ? Color.FromArgb(27, 150, 90)
                        : Color.FromArgb(180, 40, 40);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al calcular resumen: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Registrar / Actualizar gasto ─────────────────────────────────────
        private void BtnRegistrarGasto_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtConcepto.Text))
            {
                MessageBox.Show("Ingrese el concepto del gasto.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtConcepto.Focus();
                return;
            }
            if (!decimal.TryParse(txtMontoGasto.Text.Trim(), out decimal monto) || monto <= 0)
            {
                MessageBox.Show("Ingrese un monto válido (mayor a 0).", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMontoGasto.Focus();
                return;
            }

            try
            {
                using (var conn = ConexionBD.ObtenerConexion())
                {
                    conn.Open();
                    MySqlCommand cmd;
                    if (_gastoEditandoId > 0)
                    {
                        cmd = new MySqlCommand(@"
                            UPDATE gastos SET Categoria=@c, Concepto=@co, Monto=@m
                            WHERE Id=@id", conn);
                        cmd.Parameters.AddWithValue("@c",  cmbCategoriaGasto.SelectedItem.ToString());
                        cmd.Parameters.AddWithValue("@co", txtConcepto.Text.Trim());
                        cmd.Parameters.AddWithValue("@m",  monto);
                        cmd.Parameters.AddWithValue("@id", _gastoEditandoId);
                    }
                    else
                    {
                        cmd = new MySqlCommand(@"
                            INSERT INTO gastos (Fecha, Categoria, Concepto, Monto)
                            VALUES (@f, @c, @co, @m)", conn);
                        cmd.Parameters.AddWithValue("@f",  DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@c",  cmbCategoriaGasto.SelectedItem.ToString());
                        cmd.Parameters.AddWithValue("@co", txtConcepto.Text.Trim());
                        cmd.Parameters.AddWithValue("@m",  monto);
                    }
                    cmd.ExecuteNonQuery();
                }

                string msg = _gastoEditandoId > 0 ? "Gasto actualizado correctamente." : "Gasto registrado correctamente.";
                MessageBox.Show(msg, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CancelarEdicion();
                CargarDatos(dtpDesde.Value, dtpHasta.Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar gasto: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Editar gasto ──────────────────────────────────────────────────────
        private void BtnEditarGasto_Click(object sender, EventArgs e)
        {
            if (dgvGastos.CurrentRow == null || dgvGastos.CurrentRow.Index < 0) return;

            DataGridViewRow fila = dgvGastos.CurrentRow;
            _gastoEditandoId = Convert.ToInt32(fila.Cells["Id"].Value);

            string cat = fila.Cells["Categoria"].Value?.ToString() ?? "";
            int idx = cmbCategoriaGasto.Items.IndexOf(cat);
            cmbCategoriaGasto.SelectedIndex = idx >= 0 ? idx : 0;

            txtConcepto.Text   = fila.Cells["Concepto"].Value?.ToString() ?? "";
            txtMontoGasto.Text = fila.Cells["Monto"].Value?.ToString() ?? "";

            btnRegistrarGasto.Text     = "  Actualizar Gasto";
            btnRegistrarGasto.BackColor = Color.FromArgb(60, 100, 180);
        }

        private void CancelarEdicion()
        {
            _gastoEditandoId = -1;
            txtConcepto.Clear();
            txtMontoGasto.Clear();
            cmbCategoriaGasto.SelectedIndex = 0;
            btnRegistrarGasto.Text      = "  Registrar Gasto";
            btnRegistrarGasto.BackColor = ColorRed;
        }

        // ── Eliminar gasto ────────────────────────────────────────────────────
        private void BtnEliminarGasto_Click(object sender, EventArgs e)
        {
            if (dgvGastos.CurrentRow == null || dgvGastos.CurrentRow.Index < 0) return;

            int id = Convert.ToInt32(dgvGastos.CurrentRow.Cells["Id"].Value);
            if (MessageBox.Show("¿Eliminar este gasto?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                using (var conn = ConexionBD.ObtenerConexion())
                {
                    conn.Open();
                    var cmd = new MySqlCommand("DELETE FROM gastos WHERE Id=@id", conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
                CargarDatos(dtpDesde.Value, dtpHasta.Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Ver detalle de venta (doble clic) ─────────────────────────────────
        private void DgvVentas_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            int ventaId = Convert.ToInt32(dgvVentas.Rows[e.RowIndex].Cells["Id"].Value);
            string fecha = dgvVentas.Rows[e.RowIndex].Cells["Fecha"].Value.ToString();
            MostrarDetalleVenta(ventaId, fecha);
        }

        private void BtnEditarVenta_Click(object sender, EventArgs e)
        {
            if (dgvVentas.CurrentRow == null || dgvVentas.CurrentRow.Index < 0) return;
            int ventaId = Convert.ToInt32(dgvVentas.CurrentRow.Cells["Id"].Value);
            string fecha = dgvVentas.CurrentRow.Cells["Fecha"].Value.ToString();
            MostrarEditarVenta(ventaId, fecha);
        }

        private void BtnEliminarVenta_Click(object sender, EventArgs e)
        {
            if (dgvVentas.CurrentRow == null || dgvVentas.CurrentRow.Index < 0) return;
            int id = Convert.ToInt32(dgvVentas.CurrentRow.Cells["Id"].Value);

            if (MessageBox.Show($"¿Eliminar la venta #{id}? Esta acción no se puede deshacer.",
                "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

            try
            {
                using (var conn = ConexionBD.ObtenerConexion())
                {
                    conn.Open();
                    using (var tx = conn.BeginTransaction())
                    {
                        // Devolver stock al inventario antes de borrar
                        var cmdStock = new MySqlCommand(@"
                            UPDATE productos p
                            INNER JOIN detalle_ventas dv ON dv.ProductoId = p.Id
                            SET p.Unidades = p.Unidades + dv.Cantidad
                            WHERE dv.VentaId = @id", conn, tx);
                        cmdStock.Parameters.AddWithValue("@id", id);
                        cmdStock.ExecuteNonQuery();

                        var cmd1 = new MySqlCommand("DELETE FROM detalle_ventas WHERE VentaId=@id", conn, tx);
                        cmd1.Parameters.AddWithValue("@id", id);
                        cmd1.ExecuteNonQuery();

                        var cmd2 = new MySqlCommand("DELETE FROM ventas WHERE Id=@id", conn, tx);
                        cmd2.Parameters.AddWithValue("@id", id);
                        cmd2.ExecuteNonQuery();

                        tx.Commit();
                    }
                }
                CargarDatos(dtpDesde.Value, dtpHasta.Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MostrarEditarVenta(int ventaId, string fecha)
        {
            DataTable dt;
            try
            {
                using (var conn = ConexionBD.ObtenerConexion())
                {
                    conn.Open();
                    var cmd = new MySqlCommand(@"
                        SELECT Id, ProductoId, Descripcion, Talla, Cantidad, PrecioUnitario, Subtotal
                        FROM detalle_ventas WHERE VentaId = @id", conn);
                    cmd.Parameters.AddWithValue("@id", ventaId);
                    dt = new DataTable();
                    dt.Load(cmd.ExecuteReader());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar detalle: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Form frmEditar = new Form {
                Text = $"Editar Venta #{ventaId} — {fecha}",
                Size = new Size(820, 480),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = ColorFondo,
                Font = new Font("Segoe UI", 10)
            };

            var dgvEdit = new DataGridView {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = ColorCard,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 10),
                RowTemplate = { Height = 36 },
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
            };
            dgvEdit.ColumnHeadersDefaultCellStyle.BackColor = ColorSidebar;
            dgvEdit.ColumnHeadersDefaultCellStyle.ForeColor = ColorTextoLight;
            dgvEdit.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvEdit.ColumnHeadersHeight = 40;
            dgvEdit.EnableHeadersVisualStyles = false;
            dgvEdit.DataSource = dt;

            // Configurar columnas
            dgvEdit.DataBindingComplete += (s, ev) => {
                if (dgvEdit.Columns.Contains("Id"))           dgvEdit.Columns["Id"].Visible = false;
                if (dgvEdit.Columns.Contains("ProductoId"))   dgvEdit.Columns["ProductoId"].Visible = false;
                if (dgvEdit.Columns.Contains("Subtotal"))     dgvEdit.Columns["Subtotal"].ReadOnly = true;
                if (dgvEdit.Columns.Contains("Descripcion"))  dgvEdit.Columns["Descripcion"].HeaderText = "Descripción";
                if (dgvEdit.Columns.Contains("Talla"))        dgvEdit.Columns["Talla"].HeaderText = "Talla";
                if (dgvEdit.Columns.Contains("Cantidad"))     dgvEdit.Columns["Cantidad"].HeaderText = "Cantidad";
                if (dgvEdit.Columns.Contains("PrecioUnitario")) dgvEdit.Columns["PrecioUnitario"].HeaderText = "Precio Unit.";
                if (dgvEdit.Columns.Contains("Subtotal"))     dgvEdit.Columns["Subtotal"].HeaderText = "Subtotal";
            };

            // Recalcular subtotal al editar cantidad o precio
            dgvEdit.CellEndEdit += (s, ev) => {
                var row = dgvEdit.Rows[ev.RowIndex];
                if (decimal.TryParse(row.Cells["Cantidad"]?.Value?.ToString(), out decimal cant) &&
                    decimal.TryParse(row.Cells["PrecioUnitario"]?.Value?.ToString(), out decimal precio))
                {
                    row.Cells["Subtotal"].Value = cant * precio;
                }
            };

            Panel panelBtn = new Panel { Dock = DockStyle.Bottom, Height = 46, BackColor = ColorFondo };
            Button btnGuardar = new Button {
                Text = "  Guardar Cambios", Dock = DockStyle.Right, Width = 200,
                BackColor = ColorAccent, ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold), Cursor = Cursors.Hand
            };
            btnGuardar.FlatAppearance.BorderSize = 0;
            btnGuardar.Click += (s, ev) => {
                try
                {
                    using (var conn = ConexionBD.ObtenerConexion())
                    {
                        conn.Open();
                        using (var tx = conn.BeginTransaction())
                        {
                            decimal totalVenta = 0;
                            foreach (DataRow row in dt.Rows)
                            {
                                int detId    = Convert.ToInt32(row["Id"]);
                                int prodId   = Convert.ToInt32(row["ProductoId"]);
                                string desc  = row["Descripcion"]?.ToString() ?? "";
                                string talla = row["Talla"]?.ToString() ?? "";
                                decimal cant  = Convert.ToDecimal(row["Cantidad"]);
                                decimal prec  = Convert.ToDecimal(row["PrecioUnitario"]);
                                decimal sub   = cant * prec;
                                totalVenta   += sub;

                                // Ajustar inventario: devolver la diferencia (cantOriginal - cantNueva)
                                int cantOriginal = Convert.ToInt32(row["Cantidad", DataRowVersion.Original]);
                                int diff = cantOriginal - (int)cant; // positivo → devuelve stock; negativo → descuenta más
                                if (diff != 0)
                                {
                                    var cmdStock = new MySqlCommand(
                                        "UPDATE productos SET Unidades = Unidades + @d WHERE Id = @pid", conn, tx);
                                    cmdStock.Parameters.AddWithValue("@d",   diff);
                                    cmdStock.Parameters.AddWithValue("@pid", prodId);
                                    cmdStock.ExecuteNonQuery();
                                }

                                var cmd = new MySqlCommand(@"
                                    UPDATE detalle_ventas
                                    SET Descripcion=@d, Talla=@t, Cantidad=@c, PrecioUnitario=@p, Subtotal=@s
                                    WHERE Id=@id", conn, tx);
                                cmd.Parameters.AddWithValue("@d",  desc);
                                cmd.Parameters.AddWithValue("@t",  talla);
                                cmd.Parameters.AddWithValue("@c",  cant);
                                cmd.Parameters.AddWithValue("@p",  prec);
                                cmd.Parameters.AddWithValue("@s",  sub);
                                cmd.Parameters.AddWithValue("@id", detId);
                                cmd.ExecuteNonQuery();
                            }

                            var cmdV = new MySqlCommand("UPDATE ventas SET Total=@t WHERE Id=@id", conn, tx);
                            cmdV.Parameters.AddWithValue("@t",  totalVenta);
                            cmdV.Parameters.AddWithValue("@id", ventaId);
                            cmdV.ExecuteNonQuery();

                            tx.Commit();
                        }
                    }
                    MessageBox.Show("Venta actualizada correctamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    frmEditar.DialogResult = DialogResult.OK;
                    frmEditar.Close();
                }
                catch (Exception ex2)
                {
                    MessageBox.Show("Error al guardar: " + ex2.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            panelBtn.Controls.Add(btnGuardar);
            frmEditar.Controls.Add(dgvEdit);
            frmEditar.Controls.Add(panelBtn);

            if (frmEditar.ShowDialog(this) == DialogResult.OK)
                CargarDatos(dtpDesde.Value, dtpHasta.Value);
        }

        private void MostrarDetalleVenta(int ventaId, string fecha)
        {
            try
            {
                using (var conn = ConexionBD.ObtenerConexion())
                {
                    conn.Open();
                    var cmd = new MySqlCommand(@"
                        SELECT dv.Descripcion, dv.Talla, dv.Cantidad,
                               dv.PrecioUnitario AS `Precio Unit.`,
                               dv.Subtotal
                        FROM detalle_ventas dv
                        WHERE dv.VentaId = @id", conn);
                    cmd.Parameters.AddWithValue("@id", ventaId);
                    var dt = new DataTable();
                    dt.Load(cmd.ExecuteReader());

                    Form frmDetalle = new Form {
                        Text = $"Detalle Venta #{ventaId} — {fecha}",
                        Size = new Size(720, 420),
                        StartPosition = FormStartPosition.CenterParent,
                        BackColor = ColorFondo,
                        Font = new Font("Segoe UI", 10)
                    };

                    DataGridView dgv = CrearGrid();
                    dgv.Dock = DockStyle.Fill;
                    dgv.DataSource = dt;
                    frmDetalle.Controls.Add(dgv);
                    frmDetalle.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar detalle: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurarColumnasGastos()
        {
            if (dgvGastos.Columns.Count == 0) return;
            if (dgvGastos.Columns.Contains("Id"))        { dgvGastos.Columns["Id"].Visible = false; }
            if (dgvGastos.Columns.Contains("Fecha"))     { dgvGastos.Columns["Fecha"].HeaderText = "Fecha";     dgvGastos.Columns["Fecha"].FillWeight = 30; }
            if (dgvGastos.Columns.Contains("Categoria")) { dgvGastos.Columns["Categoria"].HeaderText = "Categoría"; dgvGastos.Columns["Categoria"].FillWeight = 25; }
            if (dgvGastos.Columns.Contains("Concepto"))  { dgvGastos.Columns["Concepto"].HeaderText = "Concepto";  dgvGastos.Columns["Concepto"].FillWeight = 30; }
            if (dgvGastos.Columns.Contains("Monto"))     { dgvGastos.Columns["Monto"].HeaderText = "Monto";     dgvGastos.Columns["Monto"].FillWeight = 15; }
        }

        private void ConfigurarColumnasVentas()
        {
            if (dgvVentas.Columns.Count == 0) return;
            if (dgvVentas.Columns.Contains("Id"))     { dgvVentas.Columns["Id"].Visible = false; }
            if (dgvVentas.Columns.Contains("Fecha"))  { dgvVentas.Columns["Fecha"].HeaderText  = "Fecha";    dgvVentas.Columns["Fecha"].FillWeight  = 35; }
            if (dgvVentas.Columns.Contains("Total"))  { dgvVentas.Columns["Total"].HeaderText  = "Monto ($)"; dgvVentas.Columns["Total"].FillWeight  = 25; }
            if (dgvVentas.Columns.Contains("Tienda")) { dgvVentas.Columns["Tienda"].HeaderText = "Cliente / Tienda"; dgvVentas.Columns["Tienda"].FillWeight = 30; }
            if (dgvVentas.Columns.Contains("Tipo"))   { dgvVentas.Columns["Tipo"].HeaderText   = "Tipo";     dgvVentas.Columns["Tipo"].FillWeight   = 10; }
        }

        // ══════════════════════════════════════════════════════════════════════
        // HELPERS DE UI
        // ══════════════════════════════════════════════════════════════════════

        private DataGridView CrearGrid()
        {
            var dgv = new DataGridView {
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
            dgv.ColumnHeadersDefaultCellStyle.BackColor = ColorSidebar;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = ColorTextoLight;
            dgv.ColumnHeadersDefaultCellStyle.Font      = new Font("Segoe UI", 9, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Padding   = new Padding(6, 0, 0, 0);
            dgv.ColumnHeadersHeight          = 40;
            dgv.EnableHeadersVisualStyles    = false;
            dgv.DefaultCellStyle.BackColor              = ColorCard;
            dgv.DefaultCellStyle.ForeColor              = Color.FromArgb(40, 40, 60);
            dgv.DefaultCellStyle.SelectionBackColor     = Color.FromArgb(225, 210, 245);
            dgv.DefaultCellStyle.SelectionForeColor     = Color.FromArgb(30, 30, 50);
            dgv.DefaultCellStyle.Padding                = new Padding(6, 0, 0, 0);
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 245, 252);
            return dgv;
        }

        private Label CrearLabelSide(string texto, int y) => new Label {
            Text = texto.ToUpper(), Location = new Point(20, y), AutoSize = true,
            Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = ColorTextoSide
        };

        private TextBox CrearInput(int y) => new TextBox {
            Location = new Point(20, y), Size = new Size(270, 32),
            Font = new Font("Segoe UI", 11), BackColor = ColorSidebarAlt,
            ForeColor = ColorTextoLight, BorderStyle = BorderStyle.FixedSingle
        };

        private ComboBox CrearCombo(int y, string[] items)
        {
            var cmb = new ComboBox {
                Location = new Point(20, y), Size = new Size(270, 32),
                Font = new Font("Segoe UI", 11), DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = ColorSidebarAlt, ForeColor = ColorTextoLight, FlatStyle = FlatStyle.Flat
            };
            cmb.Items.AddRange(items);
            cmb.SelectedIndex = 0;
            return cmb;
        }

        private Button CrearBotonSide(string texto, int y, Color color, Color hover)
        {
            var btn = new Button {
                Text = texto, Location = new Point(20, y), Size = new Size(270, 38),
                BackColor = color, ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold), Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(12, 0, 0, 0)
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.MouseEnter += (s, e) => btn.BackColor = hover;
            btn.MouseLeave += (s, e) => btn.BackColor = color;
            return btn;
        }

        private Button CrearBotonFiltro(string texto, int y, int x)
        {
            Color c = ColorSidebarAlt;
            Color h = Color.FromArgb(55, 55, 85);
            var btn = new Button {
                Text = texto, Location = new Point(x, y), Size = new Size(120, 28),
                BackColor = c, ForeColor = ColorTextoLight, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.MouseEnter += (s, e) => btn.BackColor = h;
            btn.MouseLeave += (s, e) => btn.BackColor = c;
            return btn;
        }

        private Panel CrearCard(string titulo, string valorInicial, Color colorAcc)
        {
            Panel card  = new Panel { BackColor = ColorCard, Height = 82 };
            Panel barra = new Panel { Dock = DockStyle.Left, Width = 5, BackColor = colorAcc };
            Label lblT  = new Label {
                Text = titulo, Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = Color.FromArgb(130, 130, 150), Location = new Point(16, 12), AutoSize = true
            };
            Label lblV  = new Label {
                Name = "lblValor", Text = valorInicial,
                Font = new Font("Segoe UI", 17, FontStyle.Bold),
                ForeColor = colorAcc, Location = new Point(16, 34), AutoSize = true
            };
            card.Controls.Add(lblT);
            card.Controls.Add(lblV);
            card.Controls.Add(barra);
            return card;
        }

        private void DistribuirCards(Panel contenedor, params Panel[] cards)
        {
            int margen = 8;
            int ancho  = (contenedor.Width - margen * (cards.Length + 1)) / cards.Length;
            for (int i = 0; i < cards.Length; i++)
            {
                cards[i].Location = new Point(margen + i * (ancho + margen), 8);
                cards[i].Size     = new Size(ancho, 80);
            }
        }
    }
}
