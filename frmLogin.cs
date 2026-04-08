using System;
using System.Drawing;
using System.Windows.Forms;

namespace Angela
{
    public class frmLogin : Form
    {
        private TextBox txtUsuario;
        private TextBox txtPassword;
        private Button  btnIngresar;

        public frmLogin()
        {
            this.Text            = "Angela Store — Iniciar Sesión";
            this.WindowState     = FormWindowState.Maximized;
            this.BackColor       = Color.FromArgb(240, 241, 246);
            this.Font            = new Font("Segoe UI", 10);

            TableLayoutPanel split = new TableLayoutPanel() {
                Dock = DockStyle.Fill,
                ColumnCount = 2, RowCount = 1
            };
            split.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
            split.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));
            split.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            split.Controls.Add(BuildLeftPanel(),  0, 0);
            split.Controls.Add(BuildRightPanel(), 1, 0);

            this.Controls.Add(split);
        }

        // ── PANEL IZQUIERDO: branding ────────────────────────────────────────
        private Panel BuildLeftPanel()
        {
            Panel left = new Panel() {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(18, 18, 35)
            };

            // Crédito al fondo
            Panel creditBar = new Panel() {
                Dock = DockStyle.Bottom,
                Height = 52,
                BackColor = Color.FromArgb(14, 14, 28)
            };
            Label lblCredit = new Label() {
                Text = "Creada por  Juan Diego Montes",
                Font = new Font("Segoe UI", 9.5f),
                ForeColor = Color.FromArgb(110, 80, 140),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            // Ícono decorativo antes del crédito
            Label lblCreditIcon = new Label() {
                Text = "♦",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(233, 30, 99),
                Dock = DockStyle.Left,
                Width = 38,
                TextAlign = ContentAlignment.MiddleRight
            };
            creditBar.Controls.Add(lblCredit);
            creditBar.Controls.Add(lblCreditIcon);

            // Contenido de branding centrado
            TableLayoutPanel centerLayout = new TableLayoutPanel() {
                Dock = DockStyle.Fill,
                ColumnCount = 1, RowCount = 1,
                BackColor = Color.Transparent
            };
            centerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            centerLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            Panel branding = new Panel() {
                Anchor = AnchorStyles.None,
                Size   = new Size(340, 285),
                BackColor = Color.Transparent
            };

            Label lblDiamond = new Label() {
                Text = "♦",
                Font = new Font("Segoe UI", 44),
                ForeColor = Color.FromArgb(233, 30, 99),
                Location = new Point(0, 0),
                AutoSize = true
            };
            Label lblStoreName = new Label() {
                Text = "ANGELA STORE",
                Font = new Font("Segoe UI", 27, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(0, 64),
                AutoSize = true
            };
            Label lblTagline = new Label() {
                Text = "Tu tienda, tu control.",
                Font = new Font("Segoe UI", 13, FontStyle.Italic),
                ForeColor = Color.FromArgb(150, 108, 168),
                Location = new Point(2, 112),
                AutoSize = true
            };
            Panel accentBar = new Panel() {
                BackColor = Color.FromArgb(233, 30, 99),
                Location  = new Point(0, 152),
                Size      = new Size(58, 3)
            };
            Label lblDesc = new Label() {
                Text = "Sistema de gestión comercial\npara tiendas de ropa y accesorios.",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(122, 122, 168),
                Location = new Point(0, 168),
                Size     = new Size(330, 55)
            };

            branding.Controls.Add(lblDiamond);
            branding.Controls.Add(lblStoreName);
            branding.Controls.Add(lblTagline);
            branding.Controls.Add(accentBar);
            branding.Controls.Add(lblDesc);

            centerLayout.Controls.Add(branding, 0, 0);

            left.Controls.Add(creditBar);
            left.Controls.Add(centerLayout);
            return left;
        }

        // ── PANEL DERECHO: formulario ────────────────────────────────────────
        private Panel BuildRightPanel()
        {
            Panel right = new Panel() {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 248, 252)
            };

            // Centrar el formulario vertical y horizontalmente
            TableLayoutPanel center = new TableLayoutPanel() {
                Dock = DockStyle.Fill,
                ColumnCount = 3, RowCount = 3,
                BackColor = Color.Transparent
            };
            center.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12F));
            center.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 76F));
            center.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12F));
            center.RowStyles.Add(new RowStyle(SizeType.Percent, 18F));
            center.RowStyles.Add(new RowStyle(SizeType.Percent, 64F));
            center.RowStyles.Add(new RowStyle(SizeType.Percent, 18F));

            Panel form = new Panel() {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(8, 0, 8, 0)
            };

            // Encabezado del formulario
            Label lblWelcome = new Label() {
                Text = "Bienvenida de nuevo",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 20, 44),
                Dock = DockStyle.Top, Height = 50,
                TextAlign = ContentAlignment.BottomLeft
            };
            Label lblSubtitle = new Label() {
                Text = "Ingresa tus credenciales para continuar",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(128, 128, 155),
                Dock = DockStyle.Top, Height = 32,
                TextAlign = ContentAlignment.TopLeft
            };
            Panel gap0 = Spacer(28);

            // Campo: Usuario
            Label lblUser = FieldLabel("Usuario");
            Panel wrapUser = CrearCampo(out txtUsuario, false);
            Panel gap1 = Spacer(16);

            // Campo: Contraseña
            Label lblPass = FieldLabel("Contraseña");
            Panel wrapPass = CrearCampo(out txtPassword, true);
            Panel gap2 = Spacer(32);

            // Botón
            btnIngresar = new Button() {
                Text      = "Iniciar Sesión  \u2192",
                Dock      = DockStyle.Top, Height = 52,
                BackColor = Color.FromArgb(233, 30, 99),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            btnIngresar.FlatAppearance.BorderSize = 0;
            btnIngresar.MouseEnter += (s, e) => btnIngresar.BackColor = Color.FromArgb(198, 20, 78);
            btnIngresar.MouseLeave += (s, e) => btnIngresar.BackColor = Color.FromArgb(233, 30, 99);
            btnIngresar.Click += OnLogin;

            // Enter en el campo de contraseña dispara el login
            txtPassword.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) OnLogin(s, e); };
            txtUsuario.KeyDown  += (s, e) => { if (e.KeyCode == Keys.Enter) txtPassword.Focus(); };

            // Agregar en orden inverso (DockStyle.Top = último visible arriba)
            form.Controls.Add(btnIngresar);
            form.Controls.Add(gap2);
            form.Controls.Add(wrapPass);
            form.Controls.Add(lblPass);
            form.Controls.Add(gap1);
            form.Controls.Add(wrapUser);
            form.Controls.Add(lblUser);
            form.Controls.Add(gap0);
            form.Controls.Add(lblSubtitle);
            form.Controls.Add(lblWelcome);

            center.Controls.Add(form, 1, 1);
            right.Controls.Add(center);
            return right;
        }

        // ── HELPERS ──────────────────────────────────────────────────────────
        private static Label FieldLabel(string texto) => new Label() {
            Text = texto,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            ForeColor = Color.FromArgb(72, 72, 105),
            Dock = DockStyle.Top, Height = 28,
            TextAlign = ContentAlignment.BottomLeft
        };

        private static Panel Spacer(int h) => new Panel() {
            Dock = DockStyle.Top, Height = h,
            BackColor = Color.Transparent
        };

        private Panel CrearCampo(out TextBox txt, bool esPassword)
        {
            bool focused = false;

            Panel wrap = new Panel() {
                Dock = DockStyle.Top, Height = 48,
                BackColor = Color.White,
                Padding = new Padding(12, 11, 12, 11)
            };

            txt = new TextBox() {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 12),
                BorderStyle = BorderStyle.None,
                BackColor = Color.White,
                UseSystemPasswordChar = esPassword
            };

            // Borde dibujado: gris por defecto, rosa cuando tiene foco
            wrap.Paint += (s, e) => {
                Color bc = focused ? Color.FromArgb(233, 30, 99) : Color.FromArgb(205, 205, 228);
                ControlPaint.DrawBorder(e.Graphics, wrap.ClientRectangle,
                    bc, 1, ButtonBorderStyle.Solid,
                    bc, 1, ButtonBorderStyle.Solid,
                    bc, 1, ButtonBorderStyle.Solid,
                    bc, 1, ButtonBorderStyle.Solid);
            };

            txt.GotFocus  += (s, e) => { focused = true;  wrap.BackColor = Color.FromArgb(255, 252, 255); wrap.Invalidate(); };
            txt.LostFocus += (s, e) => { focused = false; wrap.BackColor = Color.White;                   wrap.Invalidate(); };

            wrap.Controls.Add(txt);
            return wrap;
        }

        private void OnLogin(object sender, EventArgs e)
        {
            if (txtUsuario.Text.Trim() == "admin" && txtPassword.Text == "1234")
            {
                this.Hide();
                new Modulos().Show();
            }
            else
            {
                MessageBox.Show(
                    "Usuario o contraseña incorrectos.\nPor favor, intenta de nuevo.",
                    "Acceso denegado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtPassword.Clear();
                txtPassword.Focus();
            }
        }
    }
}
