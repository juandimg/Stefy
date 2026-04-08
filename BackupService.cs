using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Angela
{
    /// <summary>
    /// Servicio de backup automático. Se ejecuta en segundo plano y genera un
    /// archivo .sql en la memoria USB conectada todos los días a las 6:00 PM.
    /// </summary>
    public static class BackupService
    {
        private static System.Windows.Forms.Timer _timer;
        private static DateTime   _ultimoBackup = DateTime.MinValue;
        private static NotifyIcon _tray;

        // Ruta de mysqldump incluido con XAMPP
        private const string MysqldumpPath = @"C:\xampp\mysql\bin\mysqldump.exe";

        // Hora del backup automático (18 = 6:00 PM)
        private const int HoraBackup = 18;

        // ── Inicio / Detener ──────────────────────────────────────────────────

        /// <summary>Inicia el servicio. Llamar al abrir la ventana principal.</summary>
        public static void Iniciar(NotifyIcon tray)
        {
            _tray  = tray;
            _timer = new System.Windows.Forms.Timer { Interval = 60_000 }; // cada minuto
            _timer.Tick += (s, e) => VerificarHora();
            _timer.Start();
        }

        /// <summary>Detiene el servicio. Llamar al cerrar la aplicación.</summary>
        public static void Detener()
        {
            _timer?.Stop();
            _timer?.Dispose();
        }

        // ── Verificación periódica ────────────────────────────────────────────

        private static void VerificarHora()
        {
            DateTime now = DateTime.Now;
            // Ejecutar solo a las 18:00 y una vez por día
            if (now.Hour == HoraBackup && now.Minute == 0 && _ultimoBackup.Date < now.Date)
                EjecutarBackup(automatico: true);
        }

        // ── Lógica principal de backup ────────────────────────────────────────

        /// <summary>
        /// Genera el backup. Si <paramref name="automatico"/> es false, muestra
        /// resultado como MessageBox (útil para el botón manual).
        /// </summary>
        public static void EjecutarBackup(bool automatico = false)
        {
            // 1. Buscar USB conectada
            DriveInfo usb = BuscarUSB();
            if (usb == null)
            {
                Notificar("Backup — Sin USB",
                    "No se detectó una memoria USB. Conéctala e inténtalo de nuevo.",
                    error: true, forzarMessageBox: !automatico);
                return;
            }

            // 2. Preparar carpeta de destino en la USB
            string carpeta = Path.Combine(usb.RootDirectory.FullName, "Angela_Backups");
            try { Directory.CreateDirectory(carpeta); }
            catch (Exception ex)
            {
                Notificar("Backup fallido",
                    $"No se pudo crear la carpeta en la USB:\n{ex.Message}",
                    error: true, forzarMessageBox: !automatico);
                return;
            }

            string destino = Path.Combine(carpeta,
                $"angela_backup_{DateTime.Now:yyyy-MM-dd_HH-mm}.sql");

            // 3. Verificar mysqldump
            if (!File.Exists(MysqldumpPath))
            {
                Notificar("Backup fallido",
                    $"No se encontró mysqldump en:\n{MysqldumpPath}\n\nVerifica que XAMPP esté instalado correctamente.",
                    error: true, forzarMessageBox: !automatico);
                return;
            }

            // 4. Ejecutar mysqldump
            var (db, user, pwd) = ConexionBD.ObtenerCredenciales();

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName               = MysqldumpPath,
                    Arguments              = $"-u {user} -p{pwd} --databases {db} --single-transaction --routines --skip-lock-tables",
                    RedirectStandardOutput = true,
                    RedirectStandardError  = true,
                    UseShellExecute        = false,
                    CreateNoWindow         = true
                };

                using (var proc = Process.Start(psi))
                {
                    string sql    = proc.StandardOutput.ReadToEnd();
                    string errMsg = proc.StandardError.ReadToEnd();
                    proc.WaitForExit();

                    // mysqldump escribe advertencias en stderr incluso si tiene éxito;
                    // solo falla si el código de salida es distinto de 0.
                    if (proc.ExitCode != 0)
                    {
                        Notificar("Backup fallido",
                            $"mysqldump terminó con error (código {proc.ExitCode}):\n{errMsg}",
                            error: true, forzarMessageBox: !automatico);
                        return;
                    }

                    File.WriteAllText(destino, sql, System.Text.Encoding.UTF8);
                }

                _ultimoBackup = DateTime.Now;

                string infoArchivo = $"USB: {usb.VolumeLabel} ({usb.RootDirectory.FullName})\n"
                                   + $"Archivo: angela_backup_{DateTime.Now:yyyy-MM-dd_HH-mm}.sql\n"
                                   + $"Carpeta: Angela_Backups\\";

                Notificar("Backup completado",
                    $"Respaldo guardado exitosamente.\n\n{infoArchivo}",
                    error: false, forzarMessageBox: !automatico);
            }
            catch (Exception ex)
            {
                Notificar("Backup fallido", ex.Message,
                    error: true, forzarMessageBox: !automatico);
            }
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private static DriveInfo BuscarUSB()
        {
            return DriveInfo.GetDrives()
                .FirstOrDefault(d => d.DriveType == DriveType.Removable && d.IsReady);
        }

        private static void Notificar(string titulo, string texto, bool error, bool forzarMessageBox = false)
        {
            if (_tray != null && !forzarMessageBox)
            {
                _tray.BalloonTipTitle = titulo;
                _tray.BalloonTipText  = texto;
                _tray.BalloonTipIcon  = error ? ToolTipIcon.Error : ToolTipIcon.Info;
                _tray.ShowBalloonTip(7000);
            }
            else
            {
                MessageBox.Show(texto, titulo, MessageBoxButtons.OK,
                    error ? MessageBoxIcon.Error : MessageBoxIcon.Information);
            }
        }
    }
}
