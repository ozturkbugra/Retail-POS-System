using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Barkod
{
    public partial class UrunForm : Form
    {
        SqlConnection baglanti = Veritabani.BaglantiGetir();
        int secilenUrunId = 0;

        public UrunForm()
        {
            InitializeComponent();
        }

        private void UrunForm_Load(object sender, EventArgs e)
        {
            KategorileriYukle();
            KdvleriYukle();
            UrunleriListele();
        }

        void KdvleriYukle()
        {
            cmbKdv.Items.Clear();
            cmbKdv.Items.Add("0");
            cmbKdv.Items.Add("1");
            cmbKdv.Items.Add("8");
            cmbKdv.Items.Add("10");
            cmbKdv.Items.Add("18");
            cmbKdv.Items.Add("20");
            cmbKdv.SelectedIndex = 3; // Varsayılan %10
        }

        void KategorileriYukle()
        {
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Kategoriler", baglanti);
                DataTable dt = new DataTable();
                da.Fill(dt);
                cmbKategori.DataSource = dt;
                cmbKategori.DisplayMember = "Ad";
                cmbKategori.ValueMember = "Id";
                cmbKategori.SelectedIndex = -1;
            }
            catch { }
        }

        void UrunleriListele(string arama = "")
        {
            try
            {
                // KritikStok çekilmiyor, sadece lazım olanlar
                string sorgu = @"SELECT u.Id, u.Barkod, u.UrunAdi, k.Ad AS Kategori, u.AlisFiyati, u.SatisFiyati, u.Stok, u.KdvOrani 
                                FROM Urunler u 
                                LEFT JOIN Kategoriler k ON u.KategoriId = k.Id 
                                WHERE u.UrunAdi LIKE @ara OR u.Barkod LIKE @ara
                                ORDER BY u.Id DESC";

                SqlDataAdapter da = new SqlDataAdapter(sorgu, baglanti);
                da.SelectCommand.Parameters.AddWithValue("@ara", "%" + arama + "%");
                DataTable dt = new DataTable();
                da.Fill(dt);
                gridUrunler.DataSource = dt;
                gridUrunler.Columns["Id"].Visible = false;
            }
            catch (Exception ex) { MessageBox.Show("Listeleme Hatası: " + ex.Message); }
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtBarkod.Text) || string.IsNullOrEmpty(txtUrunAdi.Text))
            {
                MessageBox.Show("Barkod ve Ad zorunludur!"); return;
            }

            try
            {
                if (baglanti.State == ConnectionState.Closed) baglanti.Open();

                SqlCommand kontrol = new SqlCommand("SELECT COUNT(*) FROM Urunler WHERE Barkod=@bar", baglanti);
                kontrol.Parameters.AddWithValue("@bar", txtBarkod.Text);
                if (Convert.ToInt32(kontrol.ExecuteScalar()) > 0)
                {
                    MessageBox.Show("Barkod zaten var!"); return;
                }

                // Kritik Stok EKLEMİYORUZ (Veritabanında Default 0 veya 10 ise o değer atanır)
                string sql = @"INSERT INTO Urunler (Barkod, UrunAdi, KategoriId, AlisFiyati, SatisFiyati, Stok, KdvOrani) 
                               VALUES (@bar, @ad, @kat, @alis, @satis, @stok, @kdv)";

                SqlCommand cmd = new SqlCommand(sql, baglanti);
                cmd.Parameters.AddWithValue("@bar", txtBarkod.Text);
                cmd.Parameters.AddWithValue("@ad", txtUrunAdi.Text);
                cmd.Parameters.AddWithValue("@kat", cmbKategori.SelectedValue ?? 1);
                cmd.Parameters.AddWithValue("@alis", decimal.Parse(string.IsNullOrEmpty(txtAlisFiyat.Text) ? "0" : txtAlisFiyat.Text));
                cmd.Parameters.AddWithValue("@satis", decimal.Parse(string.IsNullOrEmpty(txtSatisFiyat.Text) ? "0" : txtSatisFiyat.Text));
                cmd.Parameters.AddWithValue("@stok", int.Parse(string.IsNullOrEmpty(txtStok.Text) ? "0" : txtStok.Text));

                int kdv = cmbKdv.SelectedItem != null ? Convert.ToInt32(cmbKdv.SelectedItem) : 18;
                cmd.Parameters.AddWithValue("@kdv", kdv);

                cmd.ExecuteNonQuery();
                MessageBox.Show("Eklendi!");
                Temizle();
                UrunleriListele();
            }
            catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
            finally { baglanti.Close(); }
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            if (secilenUrunId == 0) { MessageBox.Show("Seçim yapın!"); return; }

            try
            {
                if (baglanti.State == ConnectionState.Closed) baglanti.Open();

                // Kritik Stok güncellemiyoruz
                string sql = @"UPDATE Urunler SET 
                               Barkod=@bar, UrunAdi=@ad, KategoriId=@kat, AlisFiyati=@alis, SatisFiyati=@satis, Stok=@stok, KdvOrani=@kdv
                               WHERE Id=@id";

                SqlCommand cmd = new SqlCommand(sql, baglanti);
                cmd.Parameters.AddWithValue("@id", secilenUrunId);
                cmd.Parameters.AddWithValue("@bar", txtBarkod.Text);
                cmd.Parameters.AddWithValue("@ad", txtUrunAdi.Text);
                cmd.Parameters.AddWithValue("@kat", cmbKategori.SelectedValue ?? 1);
                cmd.Parameters.AddWithValue("@alis", decimal.Parse(txtAlisFiyat.Text));
                cmd.Parameters.AddWithValue("@satis", decimal.Parse(txtSatisFiyat.Text));
                cmd.Parameters.AddWithValue("@stok", int.Parse(txtStok.Text));

                int kdv = cmbKdv.SelectedItem != null ? Convert.ToInt32(cmbKdv.SelectedItem) : 18;
                cmd.Parameters.AddWithValue("@kdv", kdv);

                cmd.ExecuteNonQuery();
                MessageBox.Show("Güncellendi!");
                Temizle();
                UrunleriListele();
            }
            catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
            finally { baglanti.Close(); }
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            if (secilenUrunId == 0) { MessageBox.Show("Seçim yapın!"); return; }
            if (MessageBox.Show("Silinsin mi?", "Onay", MessageBoxButtons.YesNo) == DialogResult.No) return;

            try
            {
                if (baglanti.State == ConnectionState.Closed) baglanti.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Urunler WHERE Id=@id", baglanti);
                cmd.Parameters.AddWithValue("@id", secilenUrunId);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Silindi.");
                Temizle();
                UrunleriListele();
            }
            catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
            finally { baglanti.Close(); }
        }

        private void gridUrunler_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = gridUrunler.Rows[e.RowIndex];
                secilenUrunId = Convert.ToInt32(row.Cells["Id"].Value);

                txtBarkod.Text = row.Cells["Barkod"].Value.ToString();
                txtUrunAdi.Text = row.Cells["UrunAdi"].Value.ToString();
                txtAlisFiyat.Text = row.Cells["AlisFiyati"].Value.ToString();
                txtSatisFiyat.Text = row.Cells["SatisFiyati"].Value.ToString();
                txtStok.Text = row.Cells["Stok"].Value.ToString();
                cmbKategori.Text = row.Cells["Kategori"].Value.ToString();

                string kdv = row.Cells["KdvOrani"].Value != DBNull.Value ? row.Cells["KdvOrani"].Value.ToString() : "18";
                cmbKdv.SelectedItem = kdv;
            }
        }

        private void btnTemizle_Click(object sender, EventArgs e) { Temizle(); }
        private void txtAra_TextChanged(object sender, EventArgs e) { UrunleriListele(txtAra.Text); }

        void Temizle()
        {
            secilenUrunId = 0;
            txtBarkod.Clear(); txtUrunAdi.Clear(); txtAlisFiyat.Clear(); txtSatisFiyat.Clear(); txtStok.Clear();
            cmbKategori.SelectedIndex = -1; cmbKdv.SelectedIndex = 3;
            txtBarkod.Focus();
        }
    }
}