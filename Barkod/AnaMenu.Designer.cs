namespace Barkod
{
    partial class AnaMenu
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.tblMenu = new System.Windows.Forms.TableLayoutPanel();
            this.btnSatisYap = new System.Windows.Forms.Button();
            this.btnMalzemeTanimla = new System.Windows.Forms.Button();
            this.btnKisiTanimla = new System.Windows.Forms.Button();
            this.btnKategoriTanimla = new System.Windows.Forms.Button();
            this.btnHazirUrun = new System.Windows.Forms.Button();
            this.btnRaporlar = new System.Windows.Forms.Button();
            this.lblBaslik = new System.Windows.Forms.Label();
            this.panelUst = new System.Windows.Forms.Panel();
            this.tblMenu.SuspendLayout();
            this.panelUst.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelUst (Başlık Alanı)
            // 
            this.panelUst.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.panelUst.Controls.Add(this.lblBaslik);
            this.panelUst.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelUst.Location = new System.Drawing.Point(0, 0);
            this.panelUst.Name = "panelUst";
            this.panelUst.Size = new System.Drawing.Size(1000, 80);
            this.panelUst.TabIndex = 0;
            // 
            // lblBaslik
            // 
            this.lblBaslik.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblBaslik.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblBaslik.ForeColor = System.Drawing.Color.White;
            this.lblBaslik.Location = new System.Drawing.Point(0, 0);
            this.lblBaslik.Name = "lblBaslik";
            this.lblBaslik.Size = new System.Drawing.Size(1000, 80);
            this.lblBaslik.TabIndex = 0;
            this.lblBaslik.Text = "BAKKAL OTOMASYON SİSTEMİ";
            this.lblBaslik.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tblMenu (Butonların Izgarası)
            // 
            this.tblMenu.ColumnCount = 3;
            this.tblMenu.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tblMenu.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tblMenu.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tblMenu.Controls.Add(this.btnSatisYap, 0, 0);
            this.tblMenu.Controls.Add(this.btnMalzemeTanimla, 1, 0);
            this.tblMenu.Controls.Add(this.btnKisiTanimla, 2, 0);
            this.tblMenu.Controls.Add(this.btnKategoriTanimla, 0, 1);
            this.tblMenu.Controls.Add(this.btnHazirUrun, 1, 1);
            this.tblMenu.Controls.Add(this.btnRaporlar, 2, 1);
            this.tblMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMenu.Location = new System.Drawing.Point(0, 80);
            this.tblMenu.Name = "tblMenu";
            this.tblMenu.RowCount = 2;
            this.tblMenu.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblMenu.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblMenu.Size = new System.Drawing.Size(1000, 520);
            this.tblMenu.TabIndex = 1;
            this.tblMenu.Padding = new System.Windows.Forms.Padding(20);
            // 
            // btnSatisYap
            // 
            this.btnSatisYap.BackColor = System.Drawing.Color.SeaGreen;
            this.btnSatisYap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSatisYap.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSatisYap.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.btnSatisYap.ForeColor = System.Drawing.Color.White;
            this.btnSatisYap.Margin = new System.Windows.Forms.Padding(10);
            this.btnSatisYap.Name = "btnSatisYap";
            this.btnSatisYap.Size = new System.Drawing.Size(300, 220);
            this.btnSatisYap.TabIndex = 0;
            this.btnSatisYap.Text = "SATIŞ YAP\r\n(Kasa)";
            this.btnSatisYap.UseVisualStyleBackColor = false;
            this.btnSatisYap.Click += new System.EventHandler(this.btnSatisYap_Click);
            // 
            // btnMalzemeTanimla
            // 
            this.btnMalzemeTanimla.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnMalzemeTanimla.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnMalzemeTanimla.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMalzemeTanimla.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.btnMalzemeTanimla.ForeColor = System.Drawing.Color.White;
            this.btnMalzemeTanimla.Margin = new System.Windows.Forms.Padding(10);
            this.btnMalzemeTanimla.Name = "btnMalzemeTanimla";
            this.btnMalzemeTanimla.Size = new System.Drawing.Size(300, 220);
            this.btnMalzemeTanimla.TabIndex = 1;
            this.btnMalzemeTanimla.Text = "MALZEME TANIMLA\r\n(Ürün Yönetimi)";
            this.btnMalzemeTanimla.UseVisualStyleBackColor = false;
            this.btnMalzemeTanimla.Click += new System.EventHandler(this.btnDigerleri_Click);
            // 
            // btnKisiTanimla
            // 
            this.btnKisiTanimla.BackColor = System.Drawing.Color.DarkOrange;
            this.btnKisiTanimla.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnKisiTanimla.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnKisiTanimla.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.btnKisiTanimla.ForeColor = System.Drawing.Color.White;
            this.btnKisiTanimla.Margin = new System.Windows.Forms.Padding(10);
            this.btnKisiTanimla.Name = "btnKisiTanimla";
            this.btnKisiTanimla.Size = new System.Drawing.Size(300, 220);
            this.btnKisiTanimla.TabIndex = 2;
            this.btnKisiTanimla.Text = "KİŞİ TANIMLA\r\n(Müşteri/Tedarikçi)";
            this.btnKisiTanimla.UseVisualStyleBackColor = false;
            this.btnKisiTanimla.Click += new System.EventHandler(this.btnDigerleri_Click);
            // 
            // btnKategoriTanimla
            // 
            this.btnKategoriTanimla.BackColor = System.Drawing.Color.SlateBlue;
            this.btnKategoriTanimla.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnKategoriTanimla.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnKategoriTanimla.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.btnKategoriTanimla.ForeColor = System.Drawing.Color.White;
            this.btnKategoriTanimla.Margin = new System.Windows.Forms.Padding(10);
            this.btnKategoriTanimla.Name = "btnKategoriTanimla";
            this.btnKategoriTanimla.Size = new System.Drawing.Size(300, 220);
            this.btnKategoriTanimla.TabIndex = 3;
            this.btnKategoriTanimla.Text = "KATEGORİ TANIMLA";
            this.btnKategoriTanimla.UseVisualStyleBackColor = false;
            this.btnKategoriTanimla.Click += new System.EventHandler(this.btnDigerleri_Click);
            // 
            // btnHazirUrun
            // 
            this.btnHazirUrun.BackColor = System.Drawing.Color.Teal;
            this.btnHazirUrun.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnHazirUrun.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHazirUrun.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.btnHazirUrun.ForeColor = System.Drawing.Color.White;
            this.btnHazirUrun.Margin = new System.Windows.Forms.Padding(10);
            this.btnHazirUrun.Name = "btnHazirUrun";
            this.btnHazirUrun.Size = new System.Drawing.Size(300, 220);
            this.btnHazirUrun.TabIndex = 4;
            this.btnHazirUrun.Text = "HAZIR ÜRÜN TANIMLA\r\n(Hızlı Tuşlar)";
            this.btnHazirUrun.UseVisualStyleBackColor = false;
            this.btnHazirUrun.Click += new System.EventHandler(this.btnDigerleri_Click);
            // 
            // btnRaporlar
            // 
            this.btnRaporlar.BackColor = System.Drawing.Color.Crimson;
            this.btnRaporlar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRaporlar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRaporlar.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.btnRaporlar.ForeColor = System.Drawing.Color.White;
            this.btnRaporlar.Margin = new System.Windows.Forms.Padding(10);
            this.btnRaporlar.Name = "btnRaporlar";
            this.btnRaporlar.Size = new System.Drawing.Size(300, 220);
            this.btnRaporlar.TabIndex = 5;
            this.btnRaporlar.Text = "RAPORLAR\r\n(Ciro/Stok)";
            this.btnRaporlar.UseVisualStyleBackColor = false;
            this.btnRaporlar.Click += new System.EventHandler(this.btnDigerleri_Click);
            // 
            // AnaMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 600);
            this.Controls.Add(this.tblMenu);
            this.Controls.Add(this.panelUst);
            this.Name = "AnaMenu";
            this.Text = "Ana Menü";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.tblMenu.ResumeLayout(false);
            this.panelUst.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tblMenu;
        private System.Windows.Forms.Button btnSatisYap;
        private System.Windows.Forms.Button btnMalzemeTanimla;
        private System.Windows.Forms.Button btnKisiTanimla;
        private System.Windows.Forms.Button btnKategoriTanimla;
        private System.Windows.Forms.Button btnHazirUrun;
        private System.Windows.Forms.Button btnRaporlar;
        private System.Windows.Forms.Panel panelUst;
        private System.Windows.Forms.Label lblBaslik;
    }
}