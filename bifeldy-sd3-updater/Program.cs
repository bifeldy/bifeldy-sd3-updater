/**
* 
* Author       :: Basilius Bias Astho Christyono
* Phone        :: (+62) 889 236 6466
* 
* Department   :: IT SD 03
* Mail         :: bias@indomaret.co.id
* 
* Catatan      :: Entry Point
* 
*/

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace bifeldy_sd3_updater {

    static class Program {

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [STAThread] // bifeldy-sd3-updater.exe app_name.exe pid
        static void Main(string[] args) {
            Process currentProcess = Process.GetCurrentProcess();
            Process[] allProcess = Process.GetProcessesByName(currentProcess.ProcessName);
            using (Mutex mutex = new Mutex(true, currentProcess.MainModule.ModuleName, out bool createdNew)) {
                if (createdNew) {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new CMain(args));
                }
                else {
                    foreach (Process process in allProcess) {
                        if (process.Id != currentProcess.Id) {
                            SetForegroundWindow(process.MainWindowHandle);
                            break;
                        }
                    }
                }
            }
        }

    }

}
