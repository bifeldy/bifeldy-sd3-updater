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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bifeldy_sd3_updater {

    public sealed partial class CMain : Form {

        private readonly string[] _args;

        public CMain() {
            InitializeComponent();
        }

        public CMain(string[] args) : this() {
            _args = args;
        }

        private async void CMain_Load(object sender, EventArgs e) {
            await LoadAllAplicationList();
            if (_args.Length == 0) {
                txtFilterSearch.ReadOnly = false;
                dgvDaftarAplikasi.Enabled = true;
                btnDownloadUpdate.Enabled = true;
            }
            else if (_args.Length > 2) {
                MessageBox.Show("Invalid Arguments", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            else {
                btnDownloadUpdate.Text = "Please Wait";
                txtFilterSearch.Text = _args[0];
                // TODO ::
                BtnDownloadUpdate_Click(this, EventArgs.Empty);
            }
        }

        private async void BtnDownloadUpdate_Click(object sender, EventArgs e) {
            btnDownloadUpdate.Text = "Please Wait";
            // TODO ::
        }

        private async Task LoadAllAplicationList() {
            // TODO ::
            await Task.Run(() => {
                Thread.Sleep(5000);
            });
        }

        private void TxtFilterSearch_TextChanged(object sender, EventArgs e) {
            // TODO ::
        }

    }

}
