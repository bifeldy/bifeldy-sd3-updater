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

        private readonly string UpdaterWorkDir = "Project_Postgresql/Hasil_PG";

        private string ConnectionString = string.Empty;
        List<string> Directories = new List<string>();

        IProgress<string> statInfo = null;

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
            else if (_args.Length > 2) {
                MessageBox.Show("Invalid Arguments", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            else {
                txtFilterSearch.Text = _args[0].ToUpper().Replace(".EXE", "");
                txtFilterSearch.Refresh();
                FilterSearchAllApps(false);
                AutoDownloadAndInstall();
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
                if (_args.Length == 2) {
                    pgLoading.Maximum++;
                    lblStatus.Text = $"Killing Process '{_args[0]}' ...";
                    await Task.Run(() => {
                        try {
                            Process proc = Process.GetProcessById(int.Parse(_args[1]));
                            if (_args[0].Contains(proc.ProcessName.ToUpper())) {
                                proc.Kill();
                            }
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
                    if (_args.Length == 2) {
                        statInfo.Report($"Re-Launching '{_args[0]}' ...");
                        proc = Process.Start(Path.Combine(Application.StartupPath, $"{_args[0]}"));
                    }
                    else {
                        string[] directories = Directory.GetFiles(Application.StartupPath);
                        foreach (string d in directories) {
                            if (d.ToUpper().EndsWith(".EXE") && !d.ToUpper().EndsWith($"{Application.ProductName}.EXE".ToUpper())) {
                                string path = Path.Combine(Application.StartupPath, d);
                                proc = Process.Start(path);
                            }
                        }
                    }
                    if (proc == null) {
                        throw new Exception("Executable File Not Found");
                    }
                }
                catch (Exception e) {
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
                MessageBox.Show(ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                string d = dir.Replace($"{UpdaterWorkDir.Split('/').Last()}/", "");
                if (d.ToUpper().Contains(txtFilterSearch.Text.ToUpper()) && d.ToUpper().EndsWith(".ZIP")) {
                    dgvDaftarAplikasi.Rows.Add(d);
                }
            }
            dgvDaftarAplikasi.Refresh();
            if (enableAfterLoaded) {
                dgvDaftarAplikasi.Enabled = true;
            }
        }

        private async void AutoDownloadAndInstall() {
            if (dgvDaftarAplikasi.Rows.Count == 0) {
                MessageBox.Show("No Update Available", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else {
                await DownloadAndInstall();
            }
            Application.Exit();
        }

    }

}
