using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySqlConnector;

namespace Angela
{
    public class frmClientes : Form
    {
        // ── Paleta ───────────────────────────────────────────────────────────
        private static readonly Color C_Bg       = Color.FromArgb(15,  15,  25);
        private static readonly Color C_Panel    = Color.FromArgb(22,  22,  38);
        private static readonly Color C_Card     = Color.FromArgb(30,  30,  50);
        private static readonly Color C_Accent   = Color.FromArgb(216, 27,  96);
        private static readonly Color C_AccHov   = Color.FromArgb(240, 60,  120);
        private static readonly Color C_TextMain = Color.FromArgb(240, 240, 255);
        private static readonly Color C_TextDim  = Color.FromArgb(140, 140, 165);
        private static readonly Color C_Border   = Color.FromArgb(50,  50,  75);

        // ── Controles ────────────────────────────────────────────────────────
        private TextBox      txtCedula, txtNombre, txtTelefono, txtAbono, txtBuscar;
        private DataGridView dgvClientes;
        private Button       btnGuardar, btnNuevo, btnEliminar, btnAbonar, btnVerDeuda;
        private Label        lblMensaje;
        private int          _idSeleccionado = -1;
        private Form         formPadre;

        public frmClientes(Form padre)
        {
            formPadre = padre;
            InicializarComponentes();
            CargarClientes();
        }

        // ── UI ────────────────────────────────────────────────────────────────
        private void InicializarComponentes()
        {
            this.Text        = "Clientes — Angela Store";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor   = C_Bg;
            this.Font        = new Font("Segoe UI", 10);
            this.FormClosed += (s, e) => formPadre.Show();

            // ── Header ───────────────────────────────────────────────────────
            Panel header = new Panel() {
                Dock = DockStyle.Top, Height = 64,
                BackColor = Color.FromArgb(18, 18, 32)
            };

            Button btnVolver = new Button() {
                Text = "←  Volver",
                Location = new Point(20, 16), Size = new Size(110, 34),
                BackColor = C_Panel, ForeColor = C_TextDim,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9)
            };
            btnVolver.FlatAppearance.BorderColor = C_Border;
            btnVolver.Click += (s, e) => { this.Close(); };

            Label lblTitulo = new Label() {
                Text = "@  CLIENTES",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = C_Accent,
                Location = new Point(150, 12), AutoSize = true
            };

            lblMensaje = new Label() {
                Text = "",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(60, 200, 120),
                Dock = DockStyle.Right, Width = 380,
                TextAlign = ContentAlignment.MiddleRight,
                Padding = new Padding(0, 0, 24, 0)
            };

            header.Controls.Add(btnVolver);
            header.Controls.Add(lblTitulo);
            header.Controls.Add(lblMensaje);

            // ── Panel izquierdo: formulario ───────────────────────────────────
            Panel panelForm = new Panel() {
                Dock = DockStyle.Left, Width = 360,
                BackColor = C_Panel,
                Padding = new Padding(24, 20, 24, 20)
            };

            Label lblFormTitulo = new Label() {
                Text = "Datos del Cliente",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = C_TextMain,
                Location = new Point(24, 20), AutoSize = true
            };

            // Campos
            int yBase = 64;
            int gap   = 72;

            txtCedula   = CrearCampo(panelForm, "Cédula",   yBase + gap * 0);
            txtNombre   = CrearCampo(panelForm, "Nombre",   yBase + gap * 1);
            txtTelefono = CrearCampo(panelForm, "Teléfono", yBase + gap * 2);

            // Botón Guardar
            btnGuardar = new Button() {
                Text = "Guardar Cliente",
                Location = new Point(24, yBase + gap * 3 + 8),
                Size = new Size(312, 44),
                BackColor = C_Accent, ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnGuardar.FlatAppearance.BorderSize = 0;
            btnGuardar.MouseEnter += (s, e) => btnGuardar.BackColor = C_AccHov;
            btnGuardar.MouseLeave += (s, e) => btnGuardar.BackColor = C_Accent;
            btnGuardar.Click += BtnGuardar_Click;

            // Botón Nuevo
            btnNuevo = new Button() {
                Text = "Limpiar / Nuevo",
                Location = new Point(24, yBase + gap * 3 + 62),
                Size = new Size(150, 38),
                BackColor = C_Card, ForeColor = C_TextDim,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9),
                Cursor = Cursors.Hand
            };
            btnNuevo.FlatAppearance.BorderColor = C_Border;
            btnNuevo.Click += (s, e) => LimpiarFormulario();

            // Botón Eliminar
            btnEliminar = new Button() {
                Text = "Eliminar",
                Location = new Point(186, yBase + gap * 3 + 62),
                Size = new Size(150, 38),
                BackColor = Color.FromArgb(120, 20, 40), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnEliminar.FlatAppearance.BorderSize = 0;
            btnEliminar.Click += BtnEliminar_Click;

            // Botón Ver Deuda
            btnVerDeuda = new Button() {
                Text = "Ver Deuda",
                Location = new Point(24, yBase + gap * 3 + 110),
                Size = new Size(312, 38),
                BackColor = Color.FromArgb(30, 60, 100), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnVerDeuda.FlatAppearance.BorderSize = 0;
            btnVerDeuda.Click += (s, e) => MostrarDeuda();

            // Campo Abono
            int yAbono = yBase + gap * 3 + 162;
            Label lblAbono = new Label() {
                Text = "Abonar ($)",
                ForeColor = C_TextDim,
                Location = new Point(24, yAbono),
                AutoSize = true
            };
            txtAbono = new TextBox() {
                Location = new Point(24, yAbono + 22),
                Size = new Size(312, 28),
                BackColor = C_Card, ForeColor = C_TextMain,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10),
                Enabled = false
            };

            btnAbonar = new Button() {
                Text = "Registrar Abono",
                Location = new Point(24, yAbono + 60),
                Size = new Size(312, 44),
                BackColor = Color.FromArgb(20, 120, 60), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnAbonar.FlatAppearance.BorderSize = 0;
            btnAbonar.Click += BtnAbonar_Click;

            panelForm.Controls.Add(lblFormTitulo);
            panelForm.Controls.Add(btnGuardar);
            panelForm.Controls.Add(btnNuevo);
            panelForm.Controls.Add(btnEliminar);
            panelForm.Controls.Add(btnVerDeuda);
            panelForm.Controls.Add(lblAbono);
            panelForm.Controls.Add(txtAbono);
            panelForm.Controls.Add(btnAbonar);

            // ── Panel derecho: listado ────────────────────────────────────────
            Panel panelLista = new Panel() {
                Dock = DockStyle.Fill,
                BackColor = C_Bg,
                Padding = new Padding(20, 16, 20, 20)
            };

            // Barra de búsqueda
            Panel barraTop = new Panel() {
                Dock = DockStyle.Top, Height = 52,
                BackColor = Color.Transparent
            };

            Label lblBuscarLbl = new Label() {
                Text = "Buscar:",
                ForeColor = C_TextDim,
                Location = new Point(0, 16), AutoSize = true
            };
            txtBuscar = new TextBox() {
                Location = new Point(60, 12), Size = new Size(260, 28),
                BackColor = C_Card, ForeColor = C_TextMain,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10)
            };
            txtBuscar.TextChanged += (s, e) => FiltrarClientes();

            barraTop.Controls.Add(lblBuscarLbl);
            barraTop.Controls.Add(txtBuscar);

            // DataGridView
            dgvClientes = new DataGridView() {
                Dock = DockStyle.Fill,
                BackgroundColor = C_Panel,
                GridColor = C_Border,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeight = 38,
                RowTemplate = { Height = 36 },
                Font = new Font("Segoe UI", 9.5f),
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
            };

            // Colores de cabecera y celdas
            dgvClientes.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle() {
                BackColor = Color.FromArgb(28, 28, 48),
                ForeColor = C_Accent,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                SelectionBackColor = Color.FromArgb(28, 28, 48),
                Padding = new Padding(8, 0, 0, 0)
            };
            dgvClientes.DefaultCellStyle = new DataGridViewCellStyle() {
                BackColor = C_Panel,
                ForeColor = C_TextMain,
                SelectionBackColor = Color.FromArgb(216, 27, 96, 80),
                SelectionForeColor = Color.White,
                Padding = new Padding(8, 0, 0, 0)
            };
            dgvClientes.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle() {
                BackColor = C_Card,
                ForeColor = C_TextMain,
                SelectionBackColor = Color.FromArgb(216, 27, 96, 80),
                SelectionForeColor = Color.White,
                Padding = new Padding(8, 0, 0, 0)
            };
            dgvClientes.EnableHeadersVisualStyles = false;

            dgvClientes.SelectionChanged += DgvClientes_SelectionChanged;

            panelLista.Controls.Add(dgvClientes);
            panelLista.Controls.Add(barraTop);

            this.Controls.Add(panelLista);
            this.Controls.Add(panelForm);
            this.Controls.Add(header);
        }

        // Crea un par Label + TextBox dentro del panel del formulario
        private TextBox CrearCampo(Panel parent, string etiqueta, int y)
        {
            Label lbl = new Label() {
                Text = etiqueta,
                ForeColor = C_TextDim,
                Location = new Point(24, y),
                AutoSize = true
            };
            TextBox txt = new TextBox() {
                Location = new Point(24, y + 22),
                Size = new Size(312, 28),
                BackColor = C_Card,
                ForeColor = C_TextMain,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10)
            };
            parent.Controls.Add(lbl);
            parent.Controls.Add(txt);
            return txt;
        }

        // ── Carga y filtro ────────────────────────────────────────────────────
        private void CargarClientes(string filtro = "")
        {
            try
            {
                using var conn = ConexionBD.ObtenerConexion();
                conn.Open();

                string sql = @"SELECT Id, Cedula, Nombre, Telefono, Valor
                               FROM clientes";
                if (!string.IsNullOrWhiteSpace(filtro))
                    sql += @" WHERE Cedula LIKE @f OR Nombre LIKE @f OR Telefono LIKE @f";
                sql += " ORDER BY Nombre";

                using var cmd = new MySqlCommand(sql, conn);
                if (!string.IsNullOrWhiteSpace(filtro))
                    cmd.Parameters.AddWithValue("@f", $"%{filtro}%");

                var dt = new DataTable();
                new MySqlDataAdapter(cmd).Fill(dt);

                dgvClientes.DataSource = dt;

                if (dgvClientes.Columns.Count > 0)
                {
                    dgvClientes.Columns["Id"].Visible          = false;
                    dgvClientes.Columns["Cedula"].HeaderText   = "Cédula";
                    dgvClientes.Columns["Nombre"].HeaderText   = "Nombre";
                    dgvClientes.Columns["Telefono"].HeaderText = "Teléfono";
                    dgvClientes.Columns["Valor"].HeaderText    = "Deuda ($)";
                    dgvClientes.Columns["Valor"].DefaultCellStyle.Format = "C2";
                    dgvClientes.Columns["Valor"].DefaultCellStyle.ForeColor = Color.FromArgb(240, 80, 100);
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al cargar: " + ex.Message, error: true);
            }
        }

        private void FiltrarClientes() => CargarClientes(txtBuscar.Text.Trim());

        // ── Selección en tabla ────────────────────────────────────────────────
        private void DgvClientes_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvClientes.SelectedRows.Count == 0) return;

            var row = dgvClientes.SelectedRows[0];
            _idSeleccionado       = Convert.ToInt32(row.Cells["Id"].Value);
            txtCedula.Text        = row.Cells["Cedula"].Value?.ToString()   ?? "";
            txtNombre.Text        = row.Cells["Nombre"].Value?.ToString()   ?? "";
            txtTelefono.Text      = row.Cells["Telefono"].Value?.ToString() ?? "";
            btnEliminar.Enabled   = true;
            btnVerDeuda.Enabled   = true;
            btnAbonar.Enabled     = true;
            txtAbono.Enabled      = true;
            txtAbono.Text         = "";
            btnGuardar.Text       = "Actualizar Cliente";
        }

        // ── Guardar / Actualizar ───────────────────────────────────────────────
        private void BtnGuardar_Click(object? sender, EventArgs e)
        {
            if (!Validar()) return;

            try
            {
                using var conn = ConexionBD.ObtenerConexion();
                conn.Open();

                string sql;
                if (_idSeleccionado == -1)
                {
                    sql = @"INSERT INTO clientes (Cedula, Nombre, Telefono)
                            VALUES (@ced, @nom, @tel)";
                }
                else
                {
                    sql = @"UPDATE clientes
                            SET Cedula=@ced, Nombre=@nom, Telefono=@tel
                            WHERE Id=@id";
                }

                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ced", txtCedula.Text.Trim());
                cmd.Parameters.AddWithValue("@nom", txtNombre.Text.Trim());
                cmd.Parameters.AddWithValue("@tel", txtTelefono.Text.Trim());
                if (_idSeleccionado != -1)
                    cmd.Parameters.AddWithValue("@id", _idSeleccionado);

                cmd.ExecuteNonQuery();

                MostrarMensaje(_idSeleccionado == -1
                    ? "Cliente guardado correctamente."
                    : "Cliente actualizado correctamente.");

                LimpiarFormulario();
                CargarClientes(txtBuscar.Text.Trim());
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error: " + ex.Message, error: true);
            }
        }

        // ── Eliminar ──────────────────────────────────────────────────────────
        private void BtnEliminar_Click(object? sender, EventArgs e)
        {
            if (_idSeleccionado == -1) return;

            var confirm = MessageBox.Show(
                $"¿Eliminar al cliente \"{txtNombre.Text}\"?",
                "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            try
            {
                using var conn = ConexionBD.ObtenerConexion();
                conn.Open();
                using var cmd = new MySqlCommand(
                    "DELETE FROM clientes WHERE Id=@id", conn);
                cmd.Parameters.AddWithValue("@id", _idSeleccionado);
                cmd.ExecuteNonQuery();

                MostrarMensaje("Cliente eliminado.");
                LimpiarFormulario();
                CargarClientes(txtBuscar.Text.Trim());
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error: " + ex.Message, error: true);
            }
        }

        // ── Validaciones ──────────────────────────────────────────────────────
        private bool Validar()
        {
            if (string.IsNullOrWhiteSpace(txtCedula.Text))
            { MostrarMensaje("La cédula es obligatoria.", error: true); return false; }

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            { MostrarMensaje("El nombre es obligatorio.", error: true); return false; }

            return true;
        }

        // ── Abonar ────────────────────────────────────────────────────────────
        private void BtnAbonar_Click(object? sender, EventArgs e)
        {
            if (_idSeleccionado == -1) return;

            if (!decimal.TryParse(txtAbono.Text.Trim().Replace(",", "."),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out decimal abono) || abono <= 0)
            {
                MostrarMensaje("Ingresa un valor válido para el abono.", error: true);
                return;
            }

            try
            {
                using var conn = ConexionBD.ObtenerConexion();
                conn.Open();

                // Verificar deuda actual
                using var cmdDeuda = new MySqlCommand(
                    "SELECT Valor FROM clientes WHERE Id=@id", conn);
                cmdDeuda.Parameters.AddWithValue("@id", _idSeleccionado);
                decimal deudaActual = Convert.ToDecimal(cmdDeuda.ExecuteScalar());

                if (abono > deudaActual)
                {
                    MostrarMensaje($"El abono (${abono:N2}) supera la deuda (${deudaActual:N2}).", error: true);
                    return;
                }

                // Restar de clientes.Valor
                using var cmdCliente = new MySqlCommand(
                    "UPDATE clientes SET Valor = Valor - @abono WHERE Id = @id", conn);
                cmdCliente.Parameters.AddWithValue("@abono", abono);
                cmdCliente.Parameters.AddWithValue("@id", _idSeleccionado);
                cmdCliente.ExecuteNonQuery();

                // Aplicar el abono a los créditos pendientes (más antiguos primero)
                decimal resto = abono;
                using var cmdCreditos = new MySqlCommand(
                    @"SELECT Id, Total, Abonado FROM creditos
                      WHERE ClienteId=@id AND Estado='Pendiente'
                      ORDER BY Fecha ASC", conn);
                cmdCreditos.Parameters.AddWithValue("@id", _idSeleccionado);

                var dtC = new DataTable();
                new MySqlDataAdapter(cmdCreditos).Fill(dtC);

                foreach (DataRow r in dtC.Rows)
                {
                    if (resto <= 0) break;

                    int    cid      = Convert.ToInt32(r["Id"]);
                    decimal total   = Convert.ToDecimal(r["Total"]);
                    decimal abonado = Convert.ToDecimal(r["Abonado"]);
                    decimal pendiente = total - abonado;

                    decimal aplicar = Math.Min(resto, pendiente);
                    decimal nuevoAbonado = abonado + aplicar;
                    bool saldado = nuevoAbonado >= total;

                    if (saldado)
                    {
                        // Crédito pagado en su totalidad → eliminar registro
                        using var cmdDel = new MySqlCommand(
                            "DELETE FROM creditos WHERE Id=@cid", conn);
                        cmdDel.Parameters.AddWithValue("@cid", cid);
                        cmdDel.ExecuteNonQuery();
                    }
                    else
                    {
                        using var cmdUpd = new MySqlCommand(
                            "UPDATE creditos SET Abonado=@ab WHERE Id=@cid", conn);
                        cmdUpd.Parameters.AddWithValue("@ab",  nuevoAbonado);
                        cmdUpd.Parameters.AddWithValue("@cid", cid);
                        cmdUpd.ExecuteNonQuery();
                    }

                    resto -= aplicar;
                }

                // Si el cliente quedó sin deuda, asegurar que Valor sea exactamente 0
                using var cmdVerif = new MySqlCommand(
                    @"UPDATE clientes SET Valor = 0
                      WHERE Id = @id
                        AND NOT EXISTS (SELECT 1 FROM creditos WHERE ClienteId = @id)", conn);
                cmdVerif.Parameters.AddWithValue("@id", _idSeleccionado);
                cmdVerif.ExecuteNonQuery();

                // Registrar el abono en contabilidad
                using var cmdAbono = new MySqlCommand(
                    @"INSERT INTO abonos (ClienteId, ClienteNombre, Fecha, Monto)
                      VALUES (@cli, @nom, @f, @m)", conn);
                cmdAbono.Parameters.AddWithValue("@cli", _idSeleccionado);
                cmdAbono.Parameters.AddWithValue("@nom", txtNombre.Text.Trim());
                cmdAbono.Parameters.AddWithValue("@f",   DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                cmdAbono.Parameters.AddWithValue("@m",   abono);
                cmdAbono.ExecuteNonQuery();

                MostrarMensaje($"Abono de ${abono:N2} registrado correctamente.");
                txtAbono.Text = "";
                CargarClientes(txtBuscar.Text.Trim());
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al registrar abono: " + ex.Message, error: true);
            }
        }

        // ── Ver Deuda ─────────────────────────────────────────────────────────
        private void MostrarDeuda()
        {
            if (_idSeleccionado == -1) return;

            try
            {
                using var conn = ConexionBD.ObtenerConexion();
                conn.Open();

                // Traer créditos del cliente con su detalle de productos
                string sql = @"
                    SELECT
                        c.VentaId,
                        c.Fecha,
                        c.Total,
                        c.Abonado,
                        (c.Total - c.Abonado) AS Pendiente,
                        c.Estado,
                        GROUP_CONCAT(CONCAT(dv.Descripcion, IF(dv.Talla IS NOT NULL AND dv.Talla != '', CONCAT(' T', dv.Talla), ''), ' x', dv.Cantidad) SEPARATOR ' | ') AS Productos
                    FROM creditos c
                    JOIN detalle_ventas dv ON dv.VentaId = c.VentaId
                    WHERE c.ClienteId = @id
                    GROUP BY c.Id
                    ORDER BY c.Fecha DESC";

                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", _idSeleccionado);

                var dt = new DataTable();
                new MySqlDataAdapter(cmd).Fill(dt);

                // Ventana de detalle
                Form dlg = new Form() {
                    Text        = $"Deuda de {txtNombre.Text}",
                    Size        = new Size(900, 500),
                    StartPosition = FormStartPosition.CenterParent,
                    BackColor   = C_Bg,
                    Font        = new Font("Segoe UI", 9.5f),
                    MinimumSize = new Size(700, 400)
                };

                Label lblTit = new Label() {
                    Text      = $"Deuda de: {txtNombre.Text}",
                    Font      = new Font("Segoe UI", 13, FontStyle.Bold),
                    ForeColor = C_Accent,
                    Dock      = DockStyle.Top,
                    Height    = 40,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding   = new Padding(12, 0, 0, 0)
                };

                DataGridView dgv = new DataGridView() {
                    Dock                    = DockStyle.Fill,
                    BackgroundColor         = C_Panel,
                    GridColor               = C_Border,
                    BorderStyle             = BorderStyle.None,
                    RowHeadersVisible       = false,
                    AllowUserToAddRows      = false,
                    AllowUserToDeleteRows   = false,
                    ReadOnly                = true,
                    SelectionMode           = DataGridViewSelectionMode.FullRowSelect,
                    AutoSizeColumnsMode     = DataGridViewAutoSizeColumnsMode.Fill,
                    ColumnHeadersHeight     = 36,
                    RowTemplate             = { Height = 34 },
                    EnableHeadersVisualStyles = false
                };
                dgv.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle() {
                    BackColor           = Color.FromArgb(28, 28, 48),
                    ForeColor           = C_Accent,
                    Font                = new Font("Segoe UI", 9, FontStyle.Bold),
                    SelectionBackColor  = Color.FromArgb(28, 28, 48)
                };
                dgv.DefaultCellStyle = new DataGridViewCellStyle() {
                    BackColor          = C_Panel,
                    ForeColor          = C_TextMain,
                    SelectionBackColor = Color.FromArgb(216, 27, 96, 80),
                    SelectionForeColor = Color.White
                };

                dgv.DataBindingComplete += (s, e) => {
                    if (dgv.Columns.Count == 0) return;
                    if (dgv.Columns.Contains("VentaId"))   dgv.Columns["VentaId"].Visible      = false;
                    if (dgv.Columns.Contains("Fecha"))     dgv.Columns["Fecha"].HeaderText     = "Fecha";
                    if (dgv.Columns.Contains("Total"))   { dgv.Columns["Total"].HeaderText     = "Total ($)";    dgv.Columns["Total"].DefaultCellStyle.Format   = "C2"; }
                    if (dgv.Columns.Contains("Abonado")) { dgv.Columns["Abonado"].HeaderText   = "Abonado ($)";  dgv.Columns["Abonado"].DefaultCellStyle.Format = "C2"; }
                    if (dgv.Columns.Contains("Pendiente")){ dgv.Columns["Pendiente"].HeaderText= "Pendiente ($)"; dgv.Columns["Pendiente"].DefaultCellStyle.Format = "C2"; dgv.Columns["Pendiente"].DefaultCellStyle.ForeColor = Color.FromArgb(240,80,100); }
                    if (dgv.Columns.Contains("Estado"))    dgv.Columns["Estado"].HeaderText    = "Estado";
                    if (dgv.Columns.Contains("Productos")) dgv.Columns["Productos"].HeaderText = "Productos (doble clic para detalle)";
                };

                dgv.CellDoubleClick += (s, e) => {
                    if (e.RowIndex < 0) return;
                    int ventaId = Convert.ToInt32(dgv.Rows[e.RowIndex].Cells["VentaId"].Value);
                    MostrarDetalleProductos(ventaId, dlg);
                };

                dgv.DataSource = dt;

                if (dt.Rows.Count == 0)
                {
                    Label lblSin = new Label() {
                        Text      = "Este cliente no tiene deudas registradas.",
                        ForeColor = C_TextDim,
                        Dock      = DockStyle.Fill,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Font      = new Font("Segoe UI", 11)
                    };
                    dlg.Controls.Add(lblSin);
                }
                else
                {
                    dlg.Controls.Add(dgv);
                }

                dlg.Controls.Add(lblTit);
                dlg.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al cargar deuda: " + ex.Message, error: true);
            }
        }

        // ── Detalle productos de una venta ────────────────────────────────────
        private void MostrarDetalleProductos(int ventaId, Form owner)
        {
            try
            {
                using var conn = ConexionBD.ObtenerConexion();
                conn.Open();

                using var cmd = new MySqlCommand(@"
                    SELECT
                        dv.Descripcion  AS Producto,
                        dv.Talla,
                        dv.Cantidad,
                        dv.PrecioUnitario AS `Precio Unitario ($)`,
                        dv.Subtotal       AS `Subtotal ($)`
                    FROM detalle_ventas dv
                    WHERE dv.VentaId = @vid", conn);
                cmd.Parameters.AddWithValue("@vid", ventaId);

                var dt = new DataTable();
                new MySqlDataAdapter(cmd).Fill(dt);

                Form dlg2 = new Form() {
                    Text          = "Detalle de productos",
                    Size          = new Size(750, 380),
                    StartPosition = FormStartPosition.CenterParent,
                    BackColor     = C_Bg,
                    Font          = new Font("Segoe UI", 9.5f)
                };

                Label lblTit = new Label() {
                    Text      = "Detalle de la venta",
                    Font      = new Font("Segoe UI", 12, FontStyle.Bold),
                    ForeColor = C_Accent,
                    Dock      = DockStyle.Top,
                    Height    = 38,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding   = new Padding(12, 0, 0, 0)
                };

                DataGridView dgv2 = new DataGridView() {
                    Dock                      = DockStyle.Fill,
                    DataSource                = dt,
                    BackgroundColor           = C_Panel,
                    GridColor                 = C_Border,
                    BorderStyle               = BorderStyle.None,
                    RowHeadersVisible         = false,
                    AllowUserToAddRows        = false,
                    AllowUserToDeleteRows     = false,
                    ReadOnly                  = true,
                    SelectionMode             = DataGridViewSelectionMode.FullRowSelect,
                    AutoSizeColumnsMode       = DataGridViewAutoSizeColumnsMode.Fill,
                    ColumnHeadersHeight       = 36,
                    RowTemplate               = { Height = 34 },
                    EnableHeadersVisualStyles = false
                };
                dgv2.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle() {
                    BackColor          = Color.FromArgb(28, 28, 48),
                    ForeColor          = C_Accent,
                    Font               = new Font("Segoe UI", 9, FontStyle.Bold),
                    SelectionBackColor = Color.FromArgb(28, 28, 48)
                };
                dgv2.DefaultCellStyle = new DataGridViewCellStyle() {
                    BackColor          = C_Panel,
                    ForeColor          = C_TextMain,
                    SelectionBackColor = Color.FromArgb(216, 27, 96, 80),
                    SelectionForeColor = Color.White,
                    Padding            = new Padding(6, 0, 0, 0)
                };
                dgv2.DataBindingComplete += (s, e) => {
                    if (dgv2.Columns.Contains("Precio Unitario ($)")) dgv2.Columns["Precio Unitario ($)"].DefaultCellStyle.Format = "C2";
                    if (dgv2.Columns.Contains("Subtotal ($)"))        dgv2.Columns["Subtotal ($)"].DefaultCellStyle.Format        = "C2";
                };

                dlg2.Controls.Add(dgv2);
                dlg2.Controls.Add(lblTit);
                dlg2.ShowDialog(owner);
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al cargar detalle: " + ex.Message, error: true);
            }
        }

        // ── Helpers ───────────────────────────────────────────────────────────
        private void LimpiarFormulario()
        {
            _idSeleccionado       = -1;
            txtCedula.Text        = "";
            txtNombre.Text        = "";
            txtTelefono.Text      = "";
            txtAbono.Text         = "";
            btnEliminar.Enabled   = false;
            btnVerDeuda.Enabled   = false;
            btnAbonar.Enabled     = false;
            txtAbono.Enabled      = false;
            btnGuardar.Text       = "Guardar Cliente";
            dgvClientes.ClearSelection();
        }

        private void MostrarMensaje(string texto, bool error = false)
        {
            lblMensaje.ForeColor = error
                ? Color.FromArgb(240, 80, 100)
                : Color.FromArgb(60,  200, 120);
            lblMensaje.Text = texto;
        }
    }
}
