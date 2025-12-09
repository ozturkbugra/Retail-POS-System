namespace Barkod
{
    partial class CariListeForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panelUst = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.txtAra = new System.Windows.Forms.TextBox();
            this.gridListe = new System.Windows.Forms.DataGridView();
            this.panelAlt = new System.Windows.Forms.Panel();
            this.btnOdeme = new System.Windows.Forms.Button();
            this.btnTahsilat = new System.Windows.Forms.Button();
            this.panelUst.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridListe)).BeginInit();
            this.panelAlt.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelUst
            // 
            this.panelUst.Controls.Add(this.label1);
            this.panelUst.Controls.Add(this.txtAra);
            this.panelUst.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelUst.Location = new System.Drawing.Point(0, 0);
            this.panelUst.Name = "panelUst";
            this.panelUst.Padding = new System.Windows.Forms.Padding(10);
            this.panelUst.Size = new System.Drawing.Size(984, 70);
            this.panelUst.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label1.Location = new System.Drawing.Point(12, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 21);
            this.label1.TabIndex = 1;
            this.label1.Text = "Cari Arama:";
            // 
            // txtAra
            // 
            this.txtAra.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.txtAra.Location = new System.Drawing.Point(113, 21);
            this.txtAra.Name = "txtAra";
            this.txtAra.Size = new System.Drawing.Size(306, 29);
            this.txtAra.TabIndex = 0;
            this.txtAra.TextChanged += new System.EventHandler(this.txtAra_TextChanged);
            // 
            // gridListe
            // 
            this.gridListe.AllowUserToAddRows = false;
            this.gridListe.AllowUserToDeleteRows = false;
            this.gridListe.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridListe.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.gridListe.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridListe.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridListe.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridListe.ColumnHeadersHeight = 40;
            this.gridListe.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridListe.EnableHeadersVisualStyles = false;
            this.gridListe.Location = new System.Drawing.Point(0, 70);
            this.gridListe.MultiSelect = false;
            this.gridListe.Name = "gridListe";
            this.gridListe.ReadOnly = true;
            this.gridListe.RowHeadersVisible = false;
            this.gridListe.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.gridListe.RowTemplate.Height = 35;
            this.gridListe.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridListe.Size = new System.Drawing.Size(984, 411);
            this.gridListe.TabIndex = 1;
            this.gridListe.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridListe_CellDoubleClick);
            // 
            // panelAlt
            // 
            this.panelAlt.BackColor = System.Drawing.Color.White;
            this.panelAlt.Controls.Add(this.btnOdeme);
            this.panelAlt.Controls.Add(this.btnTahsilat);
            this.panelAlt.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelAlt.Location = new System.Drawing.Point(0, 481);
            this.panelAlt.Name = "panelAlt";
            this.panelAlt.Size = new System.Drawing.Size(984, 80);
            this.panelAlt.TabIndex = 2;
            // 
            // btnOdeme
            // 
            this.btnOdeme.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOdeme.BackColor = System.Drawing.Color.Crimson;
            this.btnOdeme.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOdeme.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOdeme.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnOdeme.ForeColor = System.Drawing.Color.White;
            this.btnOdeme.Location = new System.Drawing.Point(766, 15);
            this.btnOdeme.Name = "btnOdeme";
            this.btnOdeme.Size = new System.Drawing.Size(206, 53);
            this.btnOdeme.TabIndex = 1;
            this.btnOdeme.Text = "ÖDEME YAP (ÇIKIŞ)";
            this.btnOdeme.UseVisualStyleBackColor = false;
            this.btnOdeme.Click += new System.EventHandler(this.btnOdeme_Click);
            // 
            // btnTahsilat
            // 
            this.btnTahsilat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTahsilat.BackColor = System.Drawing.Color.SeaGreen;
            this.btnTahsilat.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTahsilat.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTahsilat.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnTahsilat.ForeColor = System.Drawing.Color.White;
            this.btnTahsilat.Location = new System.Drawing.Point(554, 15);
            this.btnTahsilat.Name = "btnTahsilat";
            this.btnTahsilat.Size = new System.Drawing.Size(206, 53);
            this.btnTahsilat.TabIndex = 0;
            this.btnTahsilat.Text = "TAHSİLAT (GİRİŞ)";
            this.btnTahsilat.UseVisualStyleBackColor = false;
            this.btnTahsilat.Click += new System.EventHandler(this.btnTahsilat_Click);
            // 
            // CariListeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 561);
            this.Controls.Add(this.gridListe);
            this.Controls.Add(this.panelAlt);
            this.Controls.Add(this.panelUst);
            this.Name = "CariListeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cari Hesap Takip";
            this.Load += new System.EventHandler(this.CariListeForm_Load);
            this.panelUst.ResumeLayout(false);
            this.panelUst.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridListe)).EndInit();
            this.panelAlt.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelUst;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtAra;
        private System.Windows.Forms.DataGridView gridListe;
        private System.Windows.Forms.Panel panelAlt;
        private System.Windows.Forms.Button btnOdeme;
        private System.Windows.Forms.Button btnTahsilat;
    }
}