namespace Barkod
{
    partial class OdemeForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblToplamBaslik = new System.Windows.Forms.Label();
            this.lblGenelToplam = new System.Windows.Forms.Label();
            this.txtNakit = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtKrediKarti = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtVeresiye = new System.Windows.Forms.TextBox();
            this.btnOnayla = new System.Windows.Forms.Button();
            this.btnIptal = new System.Windows.Forms.Button();
            this.lblKalan = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblToplamBaslik
            // 
            this.lblToplamBaslik.AutoSize = true;
            this.lblToplamBaslik.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblToplamBaslik.Location = new System.Drawing.Point(30, 20);
            this.lblToplamBaslik.Name = "lblToplamBaslik";
            this.lblToplamBaslik.Size = new System.Drawing.Size(126, 21);
            this.lblToplamBaslik.TabIndex = 0;
            this.lblToplamBaslik.Text = "Ödenecek Tutar:";
            // 
            // lblGenelToplam
            // 
            this.lblGenelToplam.AutoSize = true;
            this.lblGenelToplam.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblGenelToplam.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblGenelToplam.Location = new System.Drawing.Point(160, 9);
            this.lblGenelToplam.Name = "lblGenelToplam";
            this.lblGenelToplam.Size = new System.Drawing.Size(110, 45);
            this.lblGenelToplam.TabIndex = 1;
            this.lblGenelToplam.Text = "0.00 ₺";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(50, 90);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 21);
            this.label1.TabIndex = 2;
            this.label1.Text = "NAKİT:";
            // 
            // txtNakit
            // 
            this.txtNakit.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.txtNakit.Location = new System.Drawing.Point(120, 85);
            this.txtNakit.Name = "txtNakit";
            this.txtNakit.Size = new System.Drawing.Size(150, 32);
            this.txtNakit.TabIndex = 0;
            this.txtNakit.Text = "0";
            this.txtNakit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtNakit.TextChanged += new System.EventHandler(this.Hesapla);
            this.txtNakit.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SadeceSayi);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(50, 140);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 21);
            this.label2.TabIndex = 4;
            this.label2.Text = "KART:";
            // 
            // txtKrediKarti
            // 
            this.txtKrediKarti.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.txtKrediKarti.Location = new System.Drawing.Point(120, 135);
            this.txtKrediKarti.Name = "txtKrediKarti";
            this.txtKrediKarti.Size = new System.Drawing.Size(150, 32);
            this.txtKrediKarti.TabIndex = 1;
            this.txtKrediKarti.Text = "0";
            this.txtKrediKarti.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtKrediKarti.TextChanged += new System.EventHandler(this.Hesapla);
            this.txtKrediKarti.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SadeceSayi);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.Color.Firebrick;
            this.label3.Location = new System.Drawing.Point(30, 190);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 21);
            this.label3.TabIndex = 6;
            this.label3.Text = "VERESİYE:";
            // 
            // txtVeresiye
            // 
            this.txtVeresiye.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.txtVeresiye.Location = new System.Drawing.Point(120, 185);
            this.txtVeresiye.Name = "txtVeresiye";
            this.txtVeresiye.Size = new System.Drawing.Size(150, 32);
            this.txtVeresiye.TabIndex = 2;
            this.txtVeresiye.Text = "0";
            this.txtVeresiye.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtVeresiye.TextChanged += new System.EventHandler(this.Hesapla);
            this.txtVeresiye.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SadeceSayi);
            // 
            // btnOnayla
            // 
            this.btnOnayla.BackColor = System.Drawing.Color.SeaGreen;
            this.btnOnayla.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnOnayla.ForeColor = System.Drawing.Color.White;
            this.btnOnayla.Location = new System.Drawing.Point(150, 250);
            this.btnOnayla.Name = "btnOnayla";
            this.btnOnayla.Size = new System.Drawing.Size(120, 50);
            this.btnOnayla.TabIndex = 3;
            this.btnOnayla.Text = "ONAYLA";
            this.btnOnayla.UseVisualStyleBackColor = false;
            this.btnOnayla.Click += new System.EventHandler(this.btnOnayla_Click);
            // 
            // btnIptal
            // 
            this.btnIptal.BackColor = System.Drawing.Color.Gray;
            this.btnIptal.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnIptal.ForeColor = System.Drawing.Color.White;
            this.btnIptal.Location = new System.Drawing.Point(34, 250);
            this.btnIptal.Name = "btnIptal";
            this.btnIptal.Size = new System.Drawing.Size(100, 50);
            this.btnIptal.TabIndex = 4;
            this.btnIptal.Text = "İPTAL";
            this.btnIptal.UseVisualStyleBackColor = false;
            this.btnIptal.Click += new System.EventHandler(this.btnIptal_Click);
            // 
            // lblKalan
            // 
            this.lblKalan.AutoSize = true;
            this.lblKalan.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblKalan.ForeColor = System.Drawing.Color.Red;
            this.lblKalan.Location = new System.Drawing.Point(280, 192);
            this.lblKalan.Name = "lblKalan";
            this.lblKalan.Size = new System.Drawing.Size(0, 19);
            this.lblKalan.TabIndex = 10;
            // 
            // OdemeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 330);
            this.Controls.Add(this.lblKalan);
            this.Controls.Add(this.btnIptal);
            this.Controls.Add(this.btnOnayla);
            this.Controls.Add(this.txtVeresiye);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtKrediKarti);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtNakit);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblGenelToplam);
            this.Controls.Add(this.lblToplamBaslik);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "OdemeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ödeme Ekranı";
            this.Load += new System.EventHandler(this.OdemeForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblToplamBaslik;
        private System.Windows.Forms.Label lblGenelToplam;
        private System.Windows.Forms.TextBox txtNakit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtKrediKarti;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtVeresiye;
        private System.Windows.Forms.Button btnOnayla;
        private System.Windows.Forms.Button btnIptal;
        private System.Windows.Forms.Label lblKalan;
    }
}