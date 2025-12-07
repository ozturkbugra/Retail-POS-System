using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Barkod
{
    public partial class HazirUrunForm : Form
    {
        SqlConnection baglanti = new SqlConnection(@"Data Source=.;Initial Catalog=BakkalDB;Integrated Security=True");

        int seciliTusNo = 0; // Hangi butonun ayarlandığını tutar

        public HazirUrunForm()
        {
            InitializeComponent();
        }

        private void HazirUrunForm_Load(object sender, EventArgs e)
        {
            UrunleriGetir("");
            ButonlariGuncelle();
        }

        // Sağdaki Listeye Ürünleri Çek
        void UrunleriGetir(string arama)
        {
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT Id, Barkod, UrunAdi, SatisFiyati FROM Urunler WHERE UrunAdi LIKE @ad", baglanti);
                da.SelectCommand.Parameters.AddWithValue("@ad", "%" + arama + "%");
                DataTable dt = new DataTable();
                da.Fill(dt);
                gridUrunler.DataSource = dt;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        // Sol Taraftaki Butonların Üzerine Ürün İsimlerini Yaz
        void ButonlariGuncelle()
        {
            // Önce hepsini sıfırla
            for (int i = 1; i <= 12; i++)
            {
                Control[] btn = this.Controls.Find("btnHizli" + i, true);
                if (btn.Length > 0)
                {
                    btn[0].Text = i + ". TUŞ\n(Boş)";
                    btn[0].BackColor = Color.LightGray;
                }
            }

            try
            {
                // Dolu olanları çek
                if (baglanti.State == ConnectionState.Closed) baglanti.Open();
                SqlCommand cmd = new SqlCommand("SELECT HizliTus, UrunAdi FROM Urunler WHERE HizliTus IS NOT NULL AND HizliTus > 0", baglanti);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string tusNo = dr["HizliTus"].ToString();
                    string ad = dr["UrunAdi"].ToString();

                    Control[] btn = this.Controls.Find("btnHizli" + tusNo, true);
                    if (btn.Length > 0)
                    {
                        btn[0].Text = tusNo + ". TUŞ\n" + ad;
                        btn[0].BackColor = Color.Orange; // Dolu olduğu belli olsun
                    }
                }
                dr.Close();
            }
            catch { }
            finally { baglanti.Close(); }
        }

        // 1. ADIM: Sol taraftan butona tıklama
        private void BtnHizli_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            seciliTusNo = Convert.ToInt32(btn.Tag); // Butonun Tag'inde numarası yazıyor

            lblBilgi.Text = $"SEÇİLİ TUŞ: {seciliTusNo}. Şimdi sağdaki listeden bir ürüne ÇİFT TIKLA.";
            lblBilgi.BackColor = Color.LightGreen;
        }

        // 2. ADIM: Sağ taraftan ürüne çift tıklama
        private void gridUrunler_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (seciliTusNo == 0)
            {
                MessageBox.Show("Lütfen önce soldan atama yapacağınız tuşu seçin!");
                return;
            }

            if (e.RowIndex >= 0) // Başlığa tıklanmadıysa
            {
                string urunId = gridUrunler.Rows[e.RowIndex].Cells["Id"].Value.ToString();
                string urunAdi = gridUrunler.Rows[e.RowIndex].Cells["UrunAdi"].Value.ToString();

                try
                {
                    if (baglanti.State == ConnectionState.Closed) baglanti.Open();

                    // 1. Önce bu tuşa daha önce atanmış bir ürün varsa onun atamasını kaldır (NULL yap)
                    SqlCommand cmdTemizle = new SqlCommand("UPDATE Urunler SET HizliTus = NULL WHERE HizliTus = @tus", baglanti);
                    cmdTemizle.Parameters.AddWithValue("@tus", seciliTusNo);
                    cmdTemizle.ExecuteNonQuery();

                    // 2. Yeni seçilen ürüne bu tuşu ata
                    SqlCommand cmdAta = new SqlCommand("UPDATE Urunler SET HizliTus = @tus WHERE Id = @id", baglanti);
                    cmdAta.Parameters.AddWithValue("@tus", seciliTusNo);
                    cmdAta.Parameters.AddWithValue("@id", urunId);
                    cmdAta.ExecuteNonQuery();

                    MessageBox.Show($"{seciliTusNo}. Tuşa '{urunAdi}' atandı!", "Başarılı");

                    ButonlariGuncelle(); // Ekranı yenile
                    seciliTusNo = 0; // Seçimi sıfırla
                    lblBilgi.Text = "Atama yapıldı. Yeni tuş seçebilirsiniz.";
                    lblBilgi.BackColor = Color.LightYellow;
                }
                catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
                finally { baglanti.Close(); }
            }
        }

        private void txtAra_TextChanged(object sender, EventArgs e)
        {
            UrunleriGetir(txtAra.Text);
        }
    }
}