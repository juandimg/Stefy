using System;
using System.Drawing;
using System.Windows.Forms;

namespace Angela
{
    public class Modulos : Form
    {
        private Label      lblClock;
        private Timer      clockTimer;
        private ToolTip    toolTip;
        private NotifyIcon _trayIcon;

        public Modulos()
        {
            this.Text = "Angela Store";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(240, 241, 246);
            this.Font = new Font("Segoe UI", 10);
            toolTip = new ToolTip() { AutoPopDelay = 5000, InitialDelay = 500 };

            this.Controls.Add(BuildContent());
            this.Controls.Add(BuildSidebar());
            this.Controls.Add(BuildHeader());

            clockTimer = new Timer() { Interval = 1000, Enabled = true };
            clockTimer.Tick += (s, e) => {
                if (lblClock != null && !lblClock.IsDisposed)
                    lblClock.Text = DateTime.Now.ToString("HH:mm:ss") + "\n"
                                  + DateTime.Now.ToString("ddd dd MMM");
            };

            // ── Icono de bandeja del sistema ──────────────────────────────────
            _trayIcon = new NotifyIcon
            {
                Icon    = Icon.ExtractAssociatedIcon(Application.ExecutablePath),
                Text    = "Angela Store",
                Visible = true
            };
            var ctxMenu = new ContextMenuStrip();
            ctxMenu.Items.Add("Hacer backup ahora", null, (s, e) =>
                BackupService.EjecutarBackup(automatico: false));
            ctxMenu.Items.Add(new ToolStripSeparator());
            ctxMenu.Items.Add("Salir", null, (s, e) => Application.Exit());
            _trayIcon.ContextMenuStrip = ctxMenu;
            _trayIcon.BalloonTipTitle  = "Angela Store";
            _trayIcon.BalloonTipText   = "El sistema está activo. Backup automático a las 6:00 PM.";
            _trayIcon.BalloonTipIcon   = ToolTipIcon.Info;
            _trayIcon.ShowBalloonTip(4000);

            // ── Iniciar servicio de backup automático ─────────────────────────
            BackupService.Iniciar(_trayIcon);

            this.FormClosed += (s, e) =>
            {
                BackupService.Detener();
                _trayIcon.Visible = false;
                _trayIcon.Dispose();
            };
        }

        private static string ObtenerSaludo()
        {
            int h = DateTime.Now.Hour;
            if (h >= 6  && h < 12) return "Buenos dias";
            if (h >= 12 && h < 19) return "Buenas tardes";
            return "Buenas noches";
        }

        // ── HEADER ──────────────────────────────────────────────────────────
        private Panel BuildHeader()
        {
            Panel header = new Panel() {
                Dock = DockStyle.Top,
                Height = 72,
                BackColor = Color.FromArgb(18, 18, 32)
            };

            // Derecho: usuario + separador + reloj
            Panel rightSide = new Panel() {
                Dock = DockStyle.Right,
                Width = 280,
                BackColor = Color.Transparent
            };
            lblClock = new Label() {
                Text = DateTime.Now.ToString("HH:mm:ss") + "\n" + DateTime.Now.ToString("ddd dd MMM"),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(148, 148, 185),
                Dock = DockStyle.Right,
                Width = 118,
                TextAlign = ContentAlignment.MiddleCenter
            };
            Panel vSep = new Panel() {
                Dock = DockStyle.Right, Width = 1,
                BackColor = Color.FromArgb(42, 42, 65)
            };
            Label lblUser = new Label() {
                Text = "◎  Administrador",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(205, 205, 228),
                Dock = DockStyle.Right,
                Width = 160,
                TextAlign = ContentAlignment.MiddleCenter
            };
            rightSide.Controls.Add(lblClock);
            rightSide.Controls.Add(vSep);
            rightSide.Controls.Add(lblUser);

            // Izquierdo: logo + subtítulo
            Label lblSysName = new Label() {
                Text = "Sistema de Gestión Comercial",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(115, 115, 160),
                Dock = DockStyle.Left, Width = 268,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(18, 0, 0, 0)
            };
            Panel accentLine = new Panel() {
                Dock = DockStyle.Left, Width = 2,
                BackColor = Color.FromArgb(233, 30, 99)
            };
            Label lblLogo = new Label() {
                Text = "♦  ANGELA STORE",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 100, 160),
                Dock = DockStyle.Left, Width = 238,
                TextAlign = ContentAlignment.MiddleCenter
            };

            header.Controls.Add(rightSide);
            header.Controls.Add(lblSysName);
            header.Controls.Add(accentLine);
            header.Controls.Add(lblLogo);
            return header;
        }

        // ── SIDEBAR ─────────────────────────────────────────────────────────
        private Panel BuildSidebar()
        {
            Panel sidebar = new Panel() {
                Dock = DockStyle.Left, Width = 220,
                BackColor = Color.FromArgb(22, 22, 40)
            };

            // Pie del sidebar: versión y cerrar sesión
            Label lblVersion = new Label() {
                Text = "v1.0  •  Angela Store",
                Font = new Font("Segoe UI", 7.5f),
                ForeColor = Color.FromArgb(58, 58, 88),
                Dock = DockStyle.Bottom, Height = 26,
                TextAlign = ContentAlignment.MiddleCenter
            };
            Button btnSalir = new Button() {
                Text = "  \u21A9  Cerrar Sesión",
                Dock = DockStyle.Bottom, Height = 52,
                BackColor = Color.FromArgb(30, 30, 55),
                ForeColor = Color.FromArgb(210, 100, 148),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(18, 0, 0, 0),
                Cursor = Cursors.Hand
            };
            btnSalir.FlatAppearance.BorderSize = 0;
            btnSalir.MouseEnter += (s, e) => btnSalir.BackColor = Color.FromArgb(52, 22, 40);
            btnSalir.MouseLeave += (s, e) => btnSalir.BackColor = Color.FromArgb(30, 30, 55);
            btnSalir.Click += (s, e) => { this.Hide(); new frmLogin().Show(); };
            toolTip.SetToolTip(btnSalir, "Volver a la pantalla de inicio de sesión");

            Button btnBackup = new Button() {
                Text = "  \u2601  Backup USB ahora",
                Dock = DockStyle.Bottom, Height = 42,
                BackColor = Color.FromArgb(25, 80, 55),
                ForeColor = Color.FromArgb(100, 220, 150),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(18, 0, 0, 0),
                Cursor = Cursors.Hand
            };
            btnBackup.FlatAppearance.BorderSize = 0;
            btnBackup.MouseEnter += (s, e) => btnBackup.BackColor = Color.FromArgb(30, 110, 70);
            btnBackup.MouseLeave += (s, e) => btnBackup.BackColor = Color.FromArgb(25, 80, 55);
            btnBackup.Click += (s, e) => BackupService.EjecutarBackup(automatico: false);
            toolTip.SetToolTip(btnBackup, "Genera un backup .sql en la memoria USB conectada");

            // Contenido con scroll vertical automático
            FlowLayoutPanel flow = new FlowLayoutPanel() {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Color.Transparent
            };

            // ── Avatar / info de la tienda
            Panel avatarArea = new Panel() {
                Width = 220, Height = 135,
                BackColor = Color.FromArgb(25, 25, 48)
            };
            // Círculo de avatar centrado
            Panel circle = new Panel() {
                Size = new Size(54, 54),
                Location = new Point(83, 14),
                BackColor = Color.FromArgb(233, 30, 99)
            };
            Label lblInitial = new Label() {
                Text = "A",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            circle.Controls.Add(lblInitial);

            Label lblStoreName = new Label() {
                Text = "Angela Store",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(238, 238, 255),
                Location = new Point(0, 76), Width = 220,
                TextAlign = ContentAlignment.MiddleCenter
            };
            Label lblSaludoSidebar = new Label() {
                Text = ObtenerSaludo() + ", Admin",
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(148, 110, 165),
                Location = new Point(0, 104), Width = 220,
                TextAlign = ContentAlignment.MiddleCenter
            };
            avatarArea.Controls.Add(circle);
            avatarArea.Controls.Add(lblStoreName);
            avatarArea.Controls.Add(lblSaludoSidebar);

            // Etiqueta de sección
            Label lblSection = new Label() {
                Text = "NAVEGACIÓN",
                Font = new Font("Segoe UI", 7.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(75, 75, 110),
                Width = 220, Height = 40,
                TextAlign = ContentAlignment.BottomLeft,
                Padding = new Padding(22, 0, 0, 6)
            };

            flow.Controls.Add(avatarArea);
            flow.Controls.Add(lblSection);
            flow.Controls.Add(CrearNavItem("Ventas",       Color.FromArgb(233, 30,  99), "$", "VENTAS",       "Registrar ventas y compras"));
            flow.Controls.Add(CrearNavItem("Inventario",   Color.FromArgb(142, 36, 170), "#", "INVENTARIO",   "Gestionar stock de prendas"));
            flow.Controls.Add(CrearNavItem("Clientes",     Color.FromArgb(216, 27,  96), "@", "CLIENTES",     "Base de datos de compradores"));
            flow.Controls.Add(CrearNavItem("Contabilidad", Color.FromArgb( 94, 53, 177), "%", "CONTABILIDAD", "Reportes y balances financieros"));

            sidebar.Controls.Add(lblVersion);
            sidebar.Controls.Add(btnSalir);
            sidebar.Controls.Add(btnBackup);
            sidebar.Controls.Add(flow);
            return sidebar;
        }

        private Panel CrearNavItem(string label, Color color, string simbolo, string modulo, string hint)
        {
            Panel item = new Panel() {
                Width = 220, Height = 52,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };

            Panel badge = new Panel() {
                Size = new Size(34, 34), Location = new Point(20, 9),
                BackColor = color
            };
            Label lbSim = new Label() {
                Text = simbolo,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            badge.Controls.Add(lbSim);

            Label lbLabel = new Label() {
                Text = label,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(182, 182, 215),
                Location = new Point(66, 17), AutoSize = true
            };

            Label lbArrow = new Label() {
                Text = "\u203A",   // ›
                Font = new Font("Segoe UI", 17),
                ForeColor = Color.FromArgb(120, 80, 145),
                Location = new Point(193, 12),
                Size = new Size(18, 28),
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = false
            };

            Action<bool> hover = (on) => {
                item.BackColor  = on ? Color.FromArgb(36, 36, 65) : Color.Transparent;
                lbLabel.ForeColor = on ? Color.White : Color.FromArgb(182, 182, 215);
                lbArrow.Visible = on;
            };

            foreach (Control c in new Control[] { item, badge, lbSim, lbLabel, lbArrow })
            {
                c.MouseEnter += (s, e) => hover(true);
                c.MouseLeave += (s, e) => hover(false);
            }

            toolTip.SetToolTip(item,   hint);
            toolTip.SetToolTip(badge,  hint);
            toolTip.SetToolTip(lbLabel, hint);

            Action navegar = () => {
                switch (modulo)
                {
                    case "VENTAS":       this.Hide(); new frmVentas(this).Show();        break;
                    case "INVENTARIO":   this.Hide(); new frmInventario(this).Show();    break;
                    case "CLIENTES":     this.Hide(); new frmClientes(this).Show();      break;
                    case "CONTABILIDAD": this.Hide(); new frmContabilidad(this).Show();  break;
                    default:
                        MessageBox.Show("Módulo próximamente disponible.", label,
                            MessageBoxButtons.OK, MessageBoxIcon.Information); break;
                }
            };
            item.Click    += (s, e) => navegar();
            badge.Click   += (s, e) => navegar();
            lbSim.Click   += (s, e) => navegar();
            lbLabel.Click += (s, e) => navegar();

            item.Controls.Add(badge);
            item.Controls.Add(lbLabel);
            item.Controls.Add(lbArrow);
            return item;
        }

        // ── CONTENIDO PRINCIPAL ─────────────────────────────────────────────
        private Panel BuildContent()
        {
            Panel content = new Panel() {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(240, 241, 246),
                Padding = new Padding(48, 30, 48, 48)
            };

            // Saludo dinámico + título juntos
            Panel titleArea = new Panel() {
                Dock = DockStyle.Top, Height = 88,
                BackColor = Color.Transparent
            };
            Label lblSaludoMain = new Label() {
                Text = ObtenerSaludo() + ", Administrador",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(200, 60, 120),
                Location = new Point(1, 4), AutoSize = true
            };
            Label lblTitle = new Label() {
                Text = "Panel de Control",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 20, 44),
                Location = new Point(0, 30), AutoSize = true
            };
            titleArea.Controls.Add(lblSaludoMain);
            titleArea.Controls.Add(lblTitle);

            Label lblSub = new Label() {
                Text = "Selecciona un módulo para comenzar a trabajar",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(112, 112, 140),
                Dock = DockStyle.Top, Height = 34,
                TextAlign = ContentAlignment.MiddleLeft
            };
            Panel divider = new Panel() {
                Dock = DockStyle.Top, Height = 2,
                BackColor = Color.FromArgb(210, 210, 230)
            };
            Panel spacer = new Panel() {
                Dock = DockStyle.Top, Height = 6,
                BackColor = Color.Transparent
            };

            TableLayoutPanel grid = new TableLayoutPanel() {
                Dock = DockStyle.Fill,
                ColumnCount = 2, RowCount = 2,
                Padding = new Padding(0, 18, 0, 0),
                BackColor = Color.Transparent
            };
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            grid.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            grid.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            grid.Controls.Add(CrearTarjeta("VENTAS",       "$", "Registrar y gestionar ventas",       Color.FromArgb(233, 30,  99), "Abre el módulo de ventas y facturación"), 0, 0);
            grid.Controls.Add(CrearTarjeta("INVENTARIO",   "#", "Control de stock y prendas",          Color.FromArgb(142, 36, 170), "Administra el inventario de productos"),  1, 0);
            grid.Controls.Add(CrearTarjeta("CLIENTES",     "@", "Base de datos de compradores",        Color.FromArgb(216, 27,  96), "Gestiona la información de clientes"),    0, 1);
            grid.Controls.Add(CrearTarjeta("CONTABILIDAD", "%", "Reportes, balances y estadísticas",   Color.FromArgb( 94, 53, 177), "Consulta reportes financieros"),          1, 1);

            content.Controls.Add(grid);
            content.Controls.Add(spacer);
            content.Controls.Add(divider);
            content.Controls.Add(lblSub);
            content.Controls.Add(titleArea);
            return content;
        }

        private Panel CrearTarjeta(string titulo, string simbolo, string desc, Color color, string hint)
        {
            Panel shadow = new Panel() {
                Anchor = AnchorStyles.None,
                Size = new Size(378, 238),
                BackColor = Color.FromArgb(196, 200, 215)
            };
            Panel card = new Panel() {
                Size = new Size(374, 234),
                Location = new Point(2, 2),
                BackColor = Color.White,
                Cursor = Cursors.Hand
            };
            Panel leftBar = new Panel() {
                Dock = DockStyle.Left, Width = 5,
                BackColor = color
            };
            Panel badge = new Panel() {
                Size = new Size(62, 62), Location = new Point(28, 26),
                BackColor = color, Cursor = Cursors.Hand
            };
            Label lbIco = new Label() {
                Text = simbolo,
                Font = new Font("Segoe UI", 26, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            badge.Controls.Add(lbIco);

            Label lbTitulo = new Label() {
                Text = titulo,
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = Color.FromArgb(24, 24, 46),
                Location = new Point(108, 36), AutoSize = true
            };
            Panel line = new Panel() {
                BackColor = Color.FromArgb(224, 224, 240),
                Location = new Point(28, 106),
                Size = new Size(318, 1)
            };
            Label lbDesc = new Label() {
                Text = desc,
                Font = new Font("Segoe UI", 9.5f),
                ForeColor = Color.FromArgb(115, 115, 145),
                Location = new Point(28, 120),
                Size = new Size(318, 38)
            };
            Button btn = new Button() {
                Text = "Ingresar al módulo  \u2192",
                Location = new Point(28, 172),
                Size = new Size(318, 42),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            // Oscurece el botón ligeramente al hover
            btn.MouseEnter += (s, e) => btn.BackColor = ControlPaint.Dark(color, 0.08f);
            btn.MouseLeave += (s, e) => btn.BackColor = color;

            toolTip.SetToolTip(card, hint);
            toolTip.SetToolTip(btn,  hint);

            Action<bool> setHover = (on) => {
                shadow.BackColor = on ? color : Color.FromArgb(196, 200, 215);
                card.BackColor   = on ? Color.FromArgb(252, 251, 255) : Color.White;
            };
            foreach (Control c in new Control[] { card, badge, lbIco, lbTitulo, lbDesc, line })
            {
                c.MouseEnter += (s, e) => setHover(true);
                c.MouseLeave += (s, e) => setHover(false);
            }

            Action navegar = () => {
                switch (titulo)
                {
                    case "VENTAS":       this.Hide(); new frmVentas(this).Show();        break;
                    case "INVENTARIO":   this.Hide(); new frmInventario(this).Show();    break;
                    case "CLIENTES":     this.Hide(); new frmClientes(this).Show();      break;
                    case "CONTABILIDAD": this.Hide(); new frmContabilidad(this).Show();  break;
                    default:
                        MessageBox.Show("Módulo próximamente disponible.", titulo,
                            MessageBoxButtons.OK, MessageBoxIcon.Information); break;
                }
            };
            card.Click  += (s, e) => navegar();
            btn.Click   += (s, e) => navegar();
            badge.Click += (s, e) => navegar();
            lbIco.Click += (s, e) => navegar();

            card.Controls.Add(leftBar);
            card.Controls.Add(badge);
            card.Controls.Add(lbTitulo);
            card.Controls.Add(line);
            card.Controls.Add(lbDesc);
            card.Controls.Add(btn);
            shadow.Controls.Add(card);
            return shadow;
        }
    }
}
