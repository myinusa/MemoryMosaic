using MemoryMosaic.Extensions;
using MemoryMosaic.Scanner;
using Processory;
using Serilog;

namespace MemoryMosaic {
    public class App {
        private static readonly ILogger Log = Serilog.Log.ForContext<App>();
        private readonly ProcessoryClient processoryClient;
        private readonly IModuleScanner moduleScanner;

        public App(IModuleScanner moduleScanner, ProcessoryClient processoryClient) {
            this.moduleScanner = moduleScanner;
            this.processoryClient = processoryClient;
        }

        public void StartModuleScan(bool start) {
            if (!start) return;

            moduleScanner.ScanModulesForRTTIClassNames();
        }

        private void ScanForProcessesAndModules(bool start, string year) {
            if (!start) return;

            if (!processoryClient.ProcessService.ProcessHandle.MainWindowTitle.Contains(year)) {
                Log.Here().Error("MainWindowTitle does not contain {Year}", year);
                Log.Here().Debug("MainWindowTitle: {Title}", processoryClient.ProcessService.ProcessHandle.MainWindowTitle);
                return;
            }
            StartModuleScan(true);
        }

        public void InitializeApplication() {
            ScanForProcessesAndModules(true, "2022");
        }

        public void Run() {
            try {
                Log.Here().Information("Application is starting.");
                InitializeApplication();
            }
            catch (Exception ex) {
                Log.Here().Error(ex, ex.Message);
            }
        }





    }
}