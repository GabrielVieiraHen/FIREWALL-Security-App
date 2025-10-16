using System;
using System.IO;
using System.ServiceProcess;
using System.Threading;

namespace FirewallService
{
    public partial class FirewallService : ServiceBase
    {
        private FileSystemWatcher watcher;
        private readonly string logPath = @"C:\temp\firewall_log.txt";

        public FirewallService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(logPath));
                Log("Serviço de proteção em tempo real iniciado.");

                string pathToWatch = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

                if (!Directory.Exists(pathToWatch))
                {
                    Log($"Erro: O diretório de Downloads não foi encontrado em '{pathToWatch}'.");
                    return;
                }

                watcher = new FileSystemWatcher(pathToWatch)
                {
                    NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
                    IncludeSubdirectories = true,
                    EnableRaisingEvents = true
                };

                watcher.Created += OnFileCreated;
                Log($"A monitorizar: {pathToWatch}");
            }
            catch (Exception ex)
            {
                Log($"Erro crítico ao iniciar o serviço: {ex.Message}");
            }
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            Thread.Sleep(500);
            Log($"[DETETADO] Novo ficheiro: {e.FullPath}");

            if (IsFileMalicious(e.FullPath))
            {
                Log($"[ALERTA] Ameaça encontrada e bloqueada: {e.Name}");
                // Aqui entraria a lógica para mover o ficheiro para quarentena
            }
            else
            {
                Log($"[SEGURO] O ficheiro parece seguro: {e.Name}");
            }
        }

        private bool IsFileMalicious(string filePath)
        {
            try
            {
                string fileName = Path.GetFileName(filePath).ToLower();
                if (fileName.Contains("hack") || fileName.Contains("trojan") || fileName.Contains("virus"))
                {
                    return true;
                }

                string content = File.ReadAllText(filePath);
                if (content.Contains("X5O!P%@AP[4\\PZX54(P^)7CC)7}$EICAR-STANDARD-ANTIVIRUS-TEST-FILE!$H+H*"))
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        protected override void OnStop()
        {
            Log("Serviço de proteção parado.");
            if (watcher != null)
            {
                watcher.Dispose();
            }
        }

        private void Log(string message)
        {
            try
            {
                File.AppendAllText(logPath, $"[{DateTime.Now:HH:mm:ss}] - {message}{Environment.NewLine}");
            }
            catch
            {
                // Ignorar erros de escrita no log para não parar o serviço
            }
        }
    }
}