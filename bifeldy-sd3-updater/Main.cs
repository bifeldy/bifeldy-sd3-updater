/**
* 
* Author       :: Basilius Bias Astho Christyono
* Phone        :: (+62) 889 236 6466
* 
* Department   :: IT SD 03
* Mail         :: bias@indomaret.co.id
* 
* Catatan      :: Window Utama
* 
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bifeldy_sd3_updater {

    public sealed partial class CMain : Form {

        private readonly string[] _args;

        private readonly string UpdaterFtpIpDomain = "172.24.16.171";
        private readonly string UpdaterFtpPort = "21";
        private readonly string UpdaterFtpUsername = "sd3";
        private readonly string UpdaterFtpPassword = "itsd3";

        private readonly string UpdaterWorkDir = "_updater";

        private string ConnectionString = string.Empty;
        List<string> Directories = new List<string>();

        IProgress<string> statInfo = null;

        string appName = string.Empty;
        string appVersion = string.Empty;
        int appPid = 0;

        public CMain() {
            InitializeComponent();
        }

        public CMain(string[] args) : this() {
            _args = args.Select(a => a.ToUpper()).ToArray();
            // --
            ConnectionString = $"ftp://{UpdaterFtpUsername}:{UpdaterFtpPassword}@{UpdaterFtpIpDomain}:{UpdaterFtpPort}/{UpdaterWorkDir}";

            statInfo = new Progress<string>(txt => {
                lblStatus.Text = txt;
            });
        }

        private async void CMain_Load(object sender, EventArgs e) {
            pgLoading.Value = 0;
            pgLoading.Maximum = 1;
            await LoadAllAplicationList();
            if (_args.Length == 0) {
                txtFilterSearch.ReadOnly = false;
                dgvDaftarAplikasi.Enabled = true;
                btnDownloadUpdate.Enabled = true;
                btnDownloadUpdate.Text = "Download Or Update";
                FilterSearchAllApps();
            }
            else if (_args.Length > 6) {
                MessageBox.Show("Invalid Arguments", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            else {
                for (int i = 0; i < _args.Length; i++) {
                    string argKey = _args[i].ToUpper();
                    string argVal = _args[i + 1].ToUpper();
                    if (argVal.StartsWith("-")) {
                        MessageBox.Show($"Invalid Value {argVal} For Argument {argKey}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Application.Exit();
                        break;
                    }
                    if (argKey == "--NAME") {
                        appName = argVal.Replace(".EXE", "");
                        i++;
                    }
                    else if (argKey == "--VERSION") {
                        appVersion = int.Parse(argVal).ToString();
                        i++;
                    }
                    else if (argKey == "--PID") {
                        appPid = int.Parse(argVal);
                        i++;
                    }
                }
                if (!string.IsNullOrEmpty(appName) && !string.IsNullOrEmpty(appVersion) && appPid > 0) {
                    txtFilterSearch.Text = appName;
                    txtFilterSearch.Refresh();
                    FilterSearchAllApps(false);
                    AutoDownloadAndInstall();
                }
            }
        }

        private async void BtnDownloadUpdate_Click(object sender, EventArgs e) {
            await DownloadAndInstall();
        }

        private void TxtFilterSearch_KeyDown(object sender, KeyEventArgs e) {
            switch (e.KeyCode) {
                case Keys.Enter:
                    FilterSearchAllApps();
                    break;
                default:
                    break;
            }
        }

        private async Task DownloadAndInstall() {
            try {
                string selectedFilePath = dgvDaftarAplikasi.SelectedCells[0].Value.ToString();
                string downloadedFilePath = Path.Combine(Application.StartupPath, selectedFilePath);
                pgLoading.Maximum += 3;
                btnDownloadUpdate.Text = "Please Wait ...";
                btnDownloadUpdate.Enabled = false;
                using (WebClient request = new WebClient()) {
                    request.DownloadProgressChanged += (s, e) => {
                        lblStatus.Text = $"Downloading ... {e.BytesReceived} Bytes";
                    };
                    await request.DownloadFileTaskAsync(new Uri($"{ConnectionString}/{selectedFilePath}"), downloadedFilePath);
                }
                pgLoading.Value++;
                if (appPid > 0) {
                    pgLoading.Maximum++;
                    lblStatus.Text = $"Killing Process '{appName}' With PID {appPid} ...";
                    await Task.Run(() => {
                        try {
                            do {
                                Process proc = Process.GetProcessById(appPid);
                                if (proc.ProcessName.ToUpper().Contains(appName)) {
                                    proc.Kill();
                                }
                                else {
                                    MessageBox.Show(
                                        $"Please Manually Close Your Application {appName} Before Continue",
                                        "Can't Kill Application",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information
                                    );
                                    break;
                                }
                                Thread.Sleep(3000);
                            }
                            while (true);
                        }
                        catch {
                            // Process Already Exited
                        }
                    });
                    pgLoading.Value++;
                }
                await Task.Run(() => {
                    using (FileStream fs = File.OpenRead(downloadedFilePath)) {
                        using (ZipArchive za = new ZipArchive(fs)) {
                            string parentFolder = string.Empty;
                            for (int i = 0; i < za.Entries.Count; i++) {
                                if (za.Entries[i].FullName.EndsWith("/")) {
                                    if (string.IsNullOrEmpty(parentFolder) && i == 0) {
                                        parentFolder = za.Entries[i].FullName.Split('/').First() + "/";
                                    }
                                    else if (za.Entries[i].FullName.Length < parentFolder.Length) {
                                        parentFolder = za.Entries[i].FullName;
                                    }
                                }
                            }
                            for (int i = 0; i < za.Entries.Count; i++) {
                                string name = za.Entries[i].FullName;
                                if (!string.IsNullOrEmpty(name) && !name.StartsWith(parentFolder)) {
                                    parentFolder = string.Empty;
                                    break;
                                }
                                else if (name.StartsWith(parentFolder + "Release/") || name.StartsWith(parentFolder + "Debug/")) {
                                    parentFolder = name;
                                    break;
                                }
                            }
                            string extractPrefix = parentFolder;
                            foreach (ZipArchiveEntry zae in za.Entries) {
                                if (zae.FullName.StartsWith(extractPrefix)) {
                                    string name = zae.FullName;
                                    if (!string.IsNullOrEmpty(extractPrefix)) {
                                        name = zae.FullName.Replace(extractPrefix, "");
                                    }
                                    string path = Path.Combine(Application.StartupPath, name);
                                    if (string.IsNullOrEmpty(zae.Name)) {
                                        if (!Directory.Exists(path)) {
                                            Directory.CreateDirectory(path);
                                        }
                                    }
                                    else {
                                        statInfo.Report($"Extracting '{zae.Name}' ...");
                                        zae.ExtractToFile(path, true);
                                    }
                                }
                            }
                        }
                    }
                    File.Delete(downloadedFilePath);
                });
                pgLoading.Value++;
                try {
                    Process proc = null;
                    if (!string.IsNullOrEmpty(appName)) {
                        string exeName = appName.EndsWith(".EXE") ? appName : $"{appName}.EXE";
                        statInfo.Report($"Re-Launching '{exeName}' ...");
                        proc = Process.Start(Path.Combine(Application.StartupPath, exeName), "--skip-update");
                    }
                    else {
                        string[] directories = Directory.GetFiles(Application.StartupPath);
                        foreach (string d in directories) {
                            if (d.ToUpper().EndsWith(".EXE") && !d.ToUpper().EndsWith($"{Application.ProductName}.EXE".ToUpper())) {
                                string path = Path.Combine(Application.StartupPath, d);
                                proc = Process.Start(path, "--skip-update");
                            }
                        }
                    }
                    if (proc == null) {
                        MessageBox.Show(
                            "Update Completed",
                            "Can't Auto-Run Application",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                        throw new Exception("Executable File Not Found");
                    }
                }
                catch {
                    Process.Start("explorer.exe", Application.StartupPath);
                }
                pgLoading.Value++;
                lblStatus.Text = "Download Update Completed ...";
                btnDownloadUpdate.Enabled = true;
                btnDownloadUpdate.Text = "Download Or Update";
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadAllAplicationList() {
            lblStatus.Text = "Getting List ...";
            Directories.Clear();
            try {
                await Task.Run(() => {
                    FtpWebRequest ftpRequest = (FtpWebRequest) WebRequest.Create(new Uri(ConnectionString));
                    ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                    FtpWebResponse response = (FtpWebResponse) ftpRequest.GetResponse();
                    using (StreamReader streamReader = new StreamReader(response.GetResponseStream())) {
                        string line = streamReader.ReadLine();
                        while (!string.IsNullOrEmpty(line)) {
                            Directories.Add(line);
                            line = streamReader.ReadLine();
                        }
                    }
                });
            }
            catch (Exception ex) {
                if (_args.Length == 0) {
                    MessageBox.Show(ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else {
                    Application.Exit();
                }
            }
            pgLoading.Value++;
            lblStatus.Text = "Loading Completed ...";
        }

        private void FilterSearchAllApps(bool enableAfterLoaded = true) {
            dgvDaftarAplikasi.Enabled = false;
            dgvDaftarAplikasi.Rows.Clear();
            dgvDaftarAplikasi.Columns.Clear();
            dgvDaftarAplikasi.Columns.Add(new DataGridViewTextBoxColumn {
                Name = "filepath",
                HeaderText = "File Name",
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            foreach (string dir in Directories.OrderByDescending(d => d.ToUpper())) {
                string d = dir.Replace($"{UpdaterWorkDir.Split('/').Last()}/", "").ToUpper();
                if (d.Contains(txtFilterSearch.Text.ToUpper()) && d.Contains(appVersion.ToUpper()) && d.EndsWith(".ZIP")) {
                    dgvDaftarAplikasi.Rows.Add(d);
                }
            }
            dgvDaftarAplikasi.Refresh();
            if (enableAfterLoaded) {
                dgvDaftarAplikasi.Enabled = true;
            }
        }

        private async void AutoDownloadAndInstall() {
            if (dgvDaftarAplikasi.Rows.Count > 0) {
                await DownloadAndInstall();
            }
            Application.Exit();
        }

    }

}
