using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Barkod
{
    public partial class KategoriForm : Form
    {
        SqlConnection baglanti = Veritabani.BaglantiGetir();

        int secilenKategoriId = 0;

        public KategoriForm()
        {
            InitializeComponent();
        }

        private void KategoriForm_Load(object sender, EventArgs e)
        {
            Listele();
        }

        void Listele()
        {
            try
            {
                if (baglanti.State == ConnectionState.Closed) baglanti.Open();

                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Kategoriler ORDER BY Ad", baglanti);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gridKategoriler.DataSource = dt;

                // Id'yi gizle, kafamız karışmasın
                if (gridKategoriler.Columns.Contains("Id"))
                    gridKategoriler.Columns["Id"].Visible = false;
            }
            catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
            finally { baglanti.Close(); }
        }

        // EKLE
        private void btnEkle_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtAd.Text)) return;

            try
            {
                if (baglanti.State == ConnectionState.Closed) baglanti.Open();

                SqlCommand cmd = new SqlCommand("INSERT INTO Kategoriler (Ad) VALUES (@ad)", baglanti);
                cmd.Parameters.AddWithValue("@ad", txtAd.Text);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Kategori Eklendi");
                Temizle();
                Listele();
            }
            catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
            finally { baglanti.Close(); }
        }

        // GÜNCELLE
        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            if (secilenKategoriId == 0) { MessageBox.Show("Seçim yapınız."); return; }

            try
            {
                if (baglanti.State == ConnectionState.Closed) baglanti.Open();

                SqlCommand cmd = new SqlCommand("UPDATE Kategoriler SET Ad=@ad WHERE Id=@id", baglanti);
                cmd.Parameters.AddWithValue("@ad", txtAd.Text);
                cmd.Parameters.AddWithValue("@id", secilenKategoriId);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Kategori Güncellendi");
                Temizle();
                Listele();
            }
            catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
            finally { baglanti.Close(); }
        }

        // SİL
        private void btnSil_Click(object sender, EventArgs e)
        {
            if (secilenKategoriId == 0) { MessageBox.Show("Seçim yapınız."); return; }

            // KONTROL: Bu kategoride ürün var mı?
            try
            {
                if (baglanti.State == ConnectionState.Closed) baglanti.Open();

                SqlCommand kontrol = new SqlCommand("SELECT COUNT(*) FROM Urunler WHERE KategoriId=@id", baglanti);
                kontrol.Parameters.AddWithValue("@id", secilenKategoriId);
                int urunSayisi = Convert.ToInt32(kontrol.ExecuteScalar());

                if (urunSayisi > 0)
                {
                    MessageBox.Show($"Bu kategoriye ait {urunSayisi} adet ürün var. Önce ürünleri silin veya kategorisini değiştirin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Sorun yoksa sil
                DialogResult onay = MessageBox.Show("Kategoriyi silmek istiyor musunuz?", "Sil", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (onay == DialogResult.Yes)
                {
                    SqlCommand cmd = new SqlCommand("DELETE FROM Kategoriler WHERE Id=@id", baglanti);
                    cmd.Parameters.AddWithValue("@id", secilenKategoriId);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Kategori Silindi");
                    Temizle();
                    Listele();
                }
            }
            catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
            finally { baglanti.Close(); }
        }

        // Seçim ve Temizleme
        private void gridKategoriler_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                secilenKategoriId = Convert.ToInt32(gridKategoriler.Rows[e.RowIndex].Cells["Id"].Value);
                txtAd.Text = gridKategoriler.Rows[e.RowIndex].Cells["Ad"].Value.ToString();
            }
        }

        private void btnTemizle_Click(object sender, EventArgs e)
        {
            Temizle();
        }

        void Temizle()
        {
            secilenKategoriId = 0;
            txtAd.Clear();
            txtAd.Focus();
        }
    }
}