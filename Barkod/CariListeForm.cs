using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Barkod
{
    public partial class CariListeForm : Form
    {
        public CariListeForm()
        {
            InitializeComponent();
        }

        private void CariListeForm_Load(object sender, EventArgs e)
        {
            GridTasarim();
            ListeyiGetir();
        }

        void GridTasarim()
        {
            gridListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridListe.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridListe.RowHeadersVisible = false;
            gridListe.ReadOnly = true;
            gridListe.AllowUserToAddRows = false;
            gridListe.RowTemplate.Height = 35;
            gridListe.DefaultCellStyle.Font = new Font("Segoe UI", 11F);
        }

        void ListeyiGetir()
        {
            using (SqlConnection baglanti = Veritabani.BaglantiGetir())
            {
                // Arama kutusu doluysa ona göre filtrele
                string sql = "SELECT Id, AdSoyad, Telefon, Bakiye FROM Kisiler WHERE AdSoyad LIKE @ara and Bakiye != 0 ORDER BY AdSoyad";
                SqlDataAdapter da = new SqlDataAdapter(sql, baglanti);
                da.SelectCommand.Parameters.AddWithValue("@ara", "%" + txtAra.Text + "%");

                DataTable dt = new DataTable();
                da.Fill(dt);
                gridListe.DataSource = dt;

                // Kolon Başlıkları ve Format
                gridListe.Columns["Id"].Visible = false;
                gridListe.Columns["AdSoyad"].HeaderText = "Kişi Adı";
                gridListe.Columns["Bakiye"].HeaderText = "Bakiye Durumu";
                gridListe.Columns["Bakiye"].DefaultCellStyle.Format = "C2"; // Para formatı

                Renklendir();
                OzetHesapla(dt);
            }
        }

        void Renklendir()
        {
            foreach (DataGridViewRow row in gridListe.Rows)
            {
                decimal bakiye = Convert.ToDecimal(row.Cells["Bakiye"].Value);
                if (bakiye > 0)
                {
                    // Borçlu (Bize verecek) - Kırmızı
                    row.Cells["Bakiye"].Style.ForeColor = Color.Red;
                    row.Cells["Bakiye"].Style.SelectionForeColor = Color.Red;
                }
                else if (bakiye < 0)
                {
                    // Alacaklı (Biz ona vereceğiz) - Yeşil
                    row.Cells["Bakiye"].Style.ForeColor = Color.Green;
                    row.Cells["Bakiye"].Style.SelectionForeColor = Color.LightGreen;
                }
            }
        }

        void OzetHesapla(DataTable dt)
        {
            decimal toplamAlacak = 0; // Bizim alacağımız (Müşterilerin borcu)
            decimal toplamBorc = 0;   // Bizim borcumuz (Tedarikçiye)

            foreach (DataRow row in dt.Rows)
            {
                decimal bak = Convert.ToDecimal(row["Bakiye"]);
                if (bak > 0) toplamAlacak += bak;
                else toplamBorc += Math.Abs(bak);
            }

            // Formun başlığına veya bir labele yazdırabilirsin
            this.Text = $"TOPLAM ALACAĞIMIZ: {toplamAlacak:C2}  |  TOPLAM BORCUMUZ: {toplamBorc:C2}";
        }

        // Arama yapınca listeyi yenile
        private void txtAra_TextChanged(object sender, EventArgs e)
        {
            ListeyiGetir();
        }

        // --- İŞLEM BUTONLARI ---

        private void btnTahsilat_Click(object sender, EventArgs e)
        {
            IslemAc("Tahsilat");
        }

        private void btnOdeme_Click(object sender, EventArgs e)
        {
            IslemAc("Odeme");
        }

        void IslemAc(string tur)
        {
            if (gridListe.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen listeden bir kişi seçiniz.");
                return;
            }

            int id = Convert.ToInt32(gridListe.SelectedRows[0].Cells["Id"].Value);

            CariIslemForm frm = new CariIslemForm();
            frm.KisiId = id;
            frm.IslemTuru = tur; // "Tahsilat" veya "Odeme"
            frm.ShowDialog();

            // İşlem bitip form kapanınca listeyi yenile ki güncel bakiyeyi görelim
            ListeyiGetir();
        }

        // Çift tıklayınca varsayılan olarak Tahsilat açsın (İsteğe bağlı)
        private void gridListe_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            btnTahsilat.PerformClick();
        }
    }
}