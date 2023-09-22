
namespace bifeldy_sd3_updater {

    public sealed partial class CMain {

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CMain));
            this.btnDownloadUpdate = new System.Windows.Forms.Button();
            this.pgLoading = new System.Windows.Forms.ProgressBar();
            this.lblStatus = new System.Windows.Forms.Label();
            this.dgvDaftarAplikasi = new System.Windows.Forms.DataGridView();
            this.txtFilterSearch = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDaftarAplikasi)).BeginInit();
            this.SuspendLayout();
            // 
            // btnDownloadUpdate
            // 
            this.btnDownloadUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDownloadUpdate.Enabled = false;
            this.btnDownloadUpdate.Location = new System.Drawing.Point(272, 107);
            this.btnDownloadUpdate.Name = "btnDownloadUpdate";
            this.btnDownloadUpdate.Size = new System.Drawing.Size(100, 40);
            this.btnDownloadUpdate.TabIndex = 0;
            this.btnDownloadUpdate.Text = "Please Wait ...";
            this.btnDownloadUpdate.UseVisualStyleBackColor = true;
            this.btnDownloadUpdate.Click += new System.EventHandler(this.BtnDownloadUpdate_Click);
            // 
            // pgLoading
            // 
            this.pgLoading.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pgLoading.Location = new System.Drawing.Point(18, 131);
            this.pgLoading.Name = "pgLoading";
            this.pgLoading.Size = new System.Drawing.Size(240, 10);
            this.pgLoading.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pgLoading.TabIndex = 1;
            this.pgLoading.Value = 50;
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(18, 115);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(177, 13);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "Loading All Available Applications ...";
            // 
            // dgvDaftarAplikasi
            // 
            this.dgvDaftarAplikasi.AllowUserToAddRows = false;
            this.dgvDaftarAplikasi.AllowUserToDeleteRows = false;
            this.dgvDaftarAplikasi.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvDaftarAplikasi.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvDaftarAplikasi.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDaftarAplikasi.Enabled = false;
            this.dgvDaftarAplikasi.Location = new System.Drawing.Point(12, 44);
            this.dgvDaftarAplikasi.MultiSelect = false;
            this.dgvDaftarAplikasi.Name = "dgvDaftarAplikasi";
            this.dgvDaftarAplikasi.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvDaftarAplikasi.Size = new System.Drawing.Size(360, 55);
            this.dgvDaftarAplikasi.TabIndex = 4;
            // 
            // txtFilterSearch
            // 
            this.txtFilterSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFilterSearch.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtFilterSearch.Location = new System.Drawing.Point(115, 12);
            this.txtFilterSearch.Name = "txtFilterSearch";
            this.txtFilterSearch.ReadOnly = true;
            this.txtFilterSearch.Size = new System.Drawing.Size(257, 20);
            this.txtFilterSearch.TabIndex = 5;
            this.txtFilterSearch.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtFilterSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtFilterSearch_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label2.Location = new System.Drawing.Point(9, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 15);
            this.label2.TabIndex = 6;
            this.label2.Text = "SEARCH FILTER";
            // 
            // CMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 161);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtFilterSearch);
            this.Controls.Add(this.pgLoading);
            this.Controls.Add(this.btnDownloadUpdate);
            this.Controls.Add(this.dgvDaftarAplikasi);
            this.Controls.Add(this.lblStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CMain";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SD3 Download Update Toolkit";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.CMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDaftarAplikasi)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnDownloadUpdate;
        private System.Windows.Forms.ProgressBar pgLoading;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.DataGridView dgvDaftarAplikasi;
        private System.Windows.Forms.TextBox txtFilterSearch;
        private System.Windows.Forms.Label label2;
    }

}

