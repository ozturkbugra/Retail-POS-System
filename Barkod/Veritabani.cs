using System;
using System.Data.SqlClient;
using System.IO; // Dosya okuma işlemleri için gerekli
using System.Windows.Forms; // Dosya yolunu bulmak için gerekli

namespace Barkod
{
    class Veritabani
    {
        public static SqlConnection BaglantiGetir()
        {
            // Dosyanın yolu: Programın çalıştığı klasör (exe'nin yanı)
            string dosyaYolu = Application.StartupPath + "\\baglanti.txt";
            string adres = "";

            if (File.Exists(dosyaYolu))
            {
                // Dosya varsa içini oku (Boşlukları temizle)
                adres = File.ReadAllText(dosyaYolu).Trim();
            }
            else
            {
                // Dosya yoksa varsayılanı oluştur (İlk kurulum için kolaylık)
                adres = @"Data Source=.;Initial Catalog=BakkalDB;Integrated Security=True";
                File.WriteAllText(dosyaYolu, adres);
            }

            return new SqlConnection(adres);
        }
    }
}