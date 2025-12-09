using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Barkod
{
    public partial class CariIslemForm : Form
    {
        public int KisiId { get; set; }      // Hangi kişi?
        public string IslemTuru { get; set; } // "Tahsilat" (Para Girişi) veya "Odeme" (Para Çıkışı)

        public CariIslemForm()
        {
            InitializeComponent();
        }

        private void CariIslemForm_Load(object sender, EventArgs e)
        {
            BilgileriGetir();
        }

        void BilgileriGetir()
        {
            using (SqlConnection baglanti = Veritabani.BaglantiGetir())
            {
                baglanti.Open();
                SqlCommand cmd = new SqlCommand("SELECT AdSoyad, Bakiye FROM Kisiler WHERE Id=@id", baglanti);
                cmd.Parameters.AddWithValue("@id", KisiId);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    lblAdSoyad.Text = dr["AdSoyad"].ToString();
                    decimal bakiye = Convert.ToDecimal(dr["Bakiye"]);

                    if (bakiye > 0) lblBakiye.Text = $"Borcu: {bakiye:C2}";
                    else if (bakiye < 0) lblBakiye.Text = $"Alacağı: {Math.Abs(bakiye):C2}";
                    else lblBakiye.Text = "Bakiye: 0.00 ₺";

                    // Renklendirme
                    lblBakiye.ForeColor = bakiye >= 0 ? System.Drawing.Color.Red : System.Drawing.Color.Green;
                }
            }

            this.Text = IslemTuru == "Tahsilat" ? "TAHSİLAT (PARA GİRİŞİ)" : "ÖDEME (PARA ÇIKIŞI)";
            btnKaydet.Text = IslemTuru.ToUpper() + " KAYDET";
            btnKaydet.BackColor = IslemTuru == "Tahsilat" ? System.Drawing.Color.SeaGreen : System.Drawing.Color.Crimson;
            btnKaydet.ForeColor = System.Drawing.Color.White;
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTutar.Text)) return;

            // Tutarı güvenli şekilde çevir
            decimal tutar = 0;
            if (!decimal.TryParse(txtTutar.Text, out tutar))
            {
                MessageBox.Show("Lütfen geçerli bir tutar giriniz.");
                return;
            }

            using (SqlConnection baglanti = Veritabani.BaglantiGetir())
            {
                baglanti.Open();
                SqlTransaction islem = baglanti.BeginTransaction(); // Hata olursa hepsini geri almak için

                try
                {
                    // --- 1. KİŞİNİN BAKİYESİNİ GÜNCELLE ---
                    // Tahsilat (Para Girişi): Müşterinin borcu düşer (Bakiye - Tutar)
                    // Ödeme (Para Çıkışı): Tedarikçinin alacağı düşer/Bizim borcumuz azalır (Bakiye + Tutar)
                    // Not: Bu sistemde (+) Bakiye müşterinin borcu, (-) Bakiye bizim tedarikçiye borcumuzdur.

                    string sqlUpdate = "";
                    if (IslemTuru == "Tahsilat")
                        sqlUpdate = "UPDATE Kisiler SET Bakiye = Bakiye - @tutar WHERE Id=@id";
                    else
                        sqlUpdate = "UPDATE Kisiler SET Bakiye = Bakiye + @tutar WHERE Id=@id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlUpdate, baglanti, islem);
                    cmdUpdate.Parameters.AddWithValue("@tutar", tutar);
                    cmdUpdate.Parameters.AddWithValue("@id", KisiId);
                    cmdUpdate.ExecuteNonQuery();

                    // --- 2. HAREKETİ KAYIT ALTINA AL (TARİHÇE) ---
                    // İşte burası yeni eklediğimiz kısım. Kim, Ne zaman, Ne kadar ödedi?

                    string sqlHareket = @"INSERT INTO KasaHareketler (KisiId, IslemTuru, Tutar, Aciklama, Tarih) 
                                  VALUES (@kisi, @tur, @tutar, @aciklama, @tarih)";

                    SqlCommand cmdHareket = new SqlCommand(sqlHareket, baglanti, islem);
                    cmdHareket.Parameters.AddWithValue("@kisi", KisiId);
                    cmdHareket.Parameters.AddWithValue("@tur", IslemTuru); // "Tahsilat" veya "Odeme"
                    cmdHareket.Parameters.AddWithValue("@tutar", tutar);
                    cmdHareket.Parameters.AddWithValue("@aciklama", txtAciklama.Text); // Açıklama kutusundan gelen
                    cmdHareket.Parameters.AddWithValue("@tarih", DateTime.Now);
                    cmdHareket.ExecuteNonQuery();

                    // İŞLEMİ ONAYLA
                    islem.Commit();
                    MessageBox.Show("İşlem Başarıyla Kaydedildi!");
                    this.Close();
                }
                catch (Exception ex)
                {
                    islem.Rollback(); // Hata varsa hiçbir şeyi kaydetme
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
        }
    }
}