using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace Barkod
{
    public partial class Form1 : Form
    {
        // BAĞLANTI ADRESİ (Kendi sunucuna göre ayarla)
        SqlConnection baglanti = new SqlConnection(@"Data Source=.;Initial Catalog=BakkalDB;Integrated Security=True");

        DataTable dtSepet = new DataTable();
        decimal genelToplam = 0;
        int sonKesilenFaturaId = 0;
        bool hesaplaniyor = false;

        // Hesap Makinesi Değişkenleri
        bool islemTiklandi = false;

        public Form1()
        {
            InitializeComponent();
        }

        string isletmeAdi = "", isletmeAdres = "", isletmeIlce = "", isletmeIl = "", isletmeTel = "";
        private void Form1_Load(object sender, EventArgs e)
        {
            AyarlariGetir(); // Yeni metod
            GridTasarim();
            MusterileriYukle();
            HizliButonlariDoldur();
            txtBarkod.Focus();

            // Grid Olayları (Sadece silme işlemi için)
            gridSepet.CellContentClick += gridSepet_CellContentClick;
        }

        void AyarlariGetir()
        {
            try
            {
                if (baglanti.State == ConnectionState.Closed) baglanti.Open();
                SqlCommand cmd = new SqlCommand("SELECT TOP 1 * FROM Ayarlar", baglanti);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    isletmeAdi = dr["IsletmeAdi"].ToString();
                    isletmeAdres = dr["Adres"].ToString();
                    isletmeIlce = dr["Ilce"].ToString();
                    isletmeIl = dr["Il"].ToString();
                    isletmeTel = dr["Telefon"].ToString();
                }
                dr.Close();
            }
            catch { }
            finally { baglanti.Close(); }
        }





        // --- TASARIM VE AYARLAR ---
        void GridTasarim()
        {
            if (dtSepet.Columns.Count == 0)
            {
                dtSepet.Columns.Add("Barkod");
                dtSepet.Columns.Add("UrunAdi");
                dtSepet.Columns.Add("Adet", typeof(int));
                dtSepet.Columns.Add("SatisFiyati", typeof(decimal));
                dtSepet.Columns.Add("Tutar", typeof(decimal));
                // Gizli Kolonlar
                dtSepet.Columns.Add("UrunId", typeof(int));
                dtSepet.Columns.Add("AlisFiyati", typeof(decimal));
                dtSepet.Columns.Add("KdvOrani", typeof(int)); // YENİ EKLENDİ
            }

            gridSepet.DataSource = dtSepet;

            // Gizlemeler
            gridSepet.Columns["UrunId"].Visible = false;
            gridSepet.Columns["AlisFiyati"].Visible = false;
            gridSepet.Columns["KdvOrani"].Visible = false; // Müşteri görmesin

            // Başlıklar
            gridSepet.Columns["UrunAdi"].HeaderText = "Ürün Adı";
            gridSepet.Columns["SatisFiyati"].HeaderText = "Fiyat";
            gridSepet.Columns["Adet"].HeaderText = "Miktar";
            gridSepet.Columns["Tutar"].HeaderText = "Toplam";

            // Stil Ayarları (Aynen kalsın)
            gridSepet.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridSepet.RowHeadersVisible = false;
            gridSepet.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridSepet.RowTemplate.Height = 40;
            gridSepet.DefaultCellStyle.Font = new Font("Segoe UI", 12F);
            gridSepet.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            gridSepet.ColumnHeadersHeight = 50;
            gridSepet.ReadOnly = true;

            if (!gridSepet.Columns.Contains("btnSil"))
            {
                DataGridViewButtonColumn btnSil = new DataGridViewButtonColumn();
                btnSil.Text = "SİL";
                btnSil.Name = "btnSil";
                btnSil.UseColumnTextForButtonValue = true;
                btnSil.FlatStyle = FlatStyle.Flat;
                btnSil.DefaultCellStyle.BackColor = Color.Crimson;
                btnSil.DefaultCellStyle.ForeColor = Color.White;
                gridSepet.Columns.Add(btnSil);
            }
        }

        // --- MÜŞTERİ YÜKLEME ---
        void MusterileriYukle()
        {
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT Id, AdSoyad, Bakiye FROM Kisiler ORDER BY AdSoyad", baglanti);
                DataTable dtMusteri = new DataTable();
                da.Fill(dtMusteri);

                // ComboBox Ayarları
                cmbKisiler.DataSource = dtMusteri;
                cmbKisiler.DisplayMember = "AdSoyad";
                cmbKisiler.ValueMember = "Id";

                // Arama Özelliği Açık
                cmbKisiler.DropDownStyle = ComboBoxStyle.DropDown;
                cmbKisiler.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                cmbKisiler.AutoCompleteSource = AutoCompleteSource.ListItems;
            }
            catch (Exception ex) { MessageBox.Show("Müşteri hatası: " + ex.Message); }
        }

        // --- SİLME İŞLEMİ ---
        private void gridSepet_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && gridSepet.Columns[e.ColumnIndex].Name == "btnSil")
            {
                hesaplaniyor = true;
                dtSepet.Rows[e.RowIndex].Delete();
                hesaplaniyor = false;
                ToplamHesapla();
                txtBarkod.Focus();
            }
        }

        // --- MERKEZİ SATIŞ KAYDETME METODU ---
        void SatisKaydet(decimal nakit, decimal kart, decimal veresiye, string odemeTipiMetin)
        {
            if (dtSepet.Rows.Count == 0) { MessageBox.Show("Sepet boş!"); return; }

            if (cmbKisiler.SelectedValue == null) { MessageBox.Show("Lütfen müşteri seçiniz!"); return; }
            int musteriId = Convert.ToInt32(cmbKisiler.SelectedValue);

            if (baglanti.State == ConnectionState.Closed) baglanti.Open();
            SqlTransaction islem = baglanti.BeginTransaction();

            try
            {
                // GÜNLÜK FATURA NO HESAPLAMA (GARANTİ YÖNTEM)
                // Transaction içinde olduğumuz için çakışma olmaz.
                // Bugünün tarihine ait en yüksek numarayı bul, yoksa 0 al, sonra 1 ekle.
                string sqlNo = @"SELECT ISNULL(MAX(FaturaNo), 0) + 1 FROM Faturalar 
                                 WHERE CONVERT(DATE, Tarih) = CONVERT(DATE, @bugun)";

                SqlCommand cmdNo = new SqlCommand(sqlNo, baglanti, islem);
                cmdNo.Parameters.AddWithValue("@bugun", DateTime.Now);

                int yeniFaturaNo = Convert.ToInt32(cmdNo.ExecuteScalar());

                // 1. FATURA KAYDI (FaturaNo parametresini ekledik)
                string sqlFatura = @"INSERT INTO Faturalar 
                                    (KisiId, Tarih, OdemeTipi, GenelToplam, TutarNakit, TutarKrediKarti, TutarVeresiye, FaturaNo) 
                                    VALUES (@kisi, @tarih, @tip, @toplam, @nakit, @kart, @veresiye, @fno); 
                                    SELECT SCOPE_IDENTITY();";

                SqlCommand cmdFatura = new SqlCommand(sqlFatura, baglanti, islem);
                cmdFatura.Parameters.AddWithValue("@kisi", musteriId);
                cmdFatura.Parameters.AddWithValue("@tarih", DateTime.Now);
                cmdFatura.Parameters.AddWithValue("@tip", odemeTipiMetin);
                cmdFatura.Parameters.AddWithValue("@toplam", genelToplam);
                cmdFatura.Parameters.AddWithValue("@nakit", nakit);
                cmdFatura.Parameters.AddWithValue("@kart", kart);
                cmdFatura.Parameters.AddWithValue("@veresiye", veresiye);
                cmdFatura.Parameters.AddWithValue("@fno", yeniFaturaNo); // İşte burası

                sonKesilenFaturaId = Convert.ToInt32(cmdFatura.ExecuteScalar());

                // 2. SATIRLARI KAYDET VE HAREKET EKLE (Aynen Kalıyor)
                foreach (DataRow row in dtSepet.Rows)
                {
                    if (row.RowState == DataRowState.Deleted) continue;

                    int urunId = Convert.ToInt32(row["UrunId"]);
                    decimal miktar = Convert.ToDecimal(row["Adet"]);
                    decimal fiyat = Convert.ToDecimal(row["SatisFiyati"]);
                    decimal tutar = Convert.ToDecimal(row["Tutar"]);

                    // Fatura Satırları
                    string sqlSatir = @"INSERT INTO FaturaSatirlar (FaturaId, UrunId, Miktar, AlisFiyati, SatisFiyati, Tutar) 
                                        VALUES (@fid, @uid, @mik, @alis, @satis, @tut)";
                    SqlCommand cmdSatir = new SqlCommand(sqlSatir, baglanti, islem);
                    cmdSatir.Parameters.AddWithValue("@fid", sonKesilenFaturaId);
                    cmdSatir.Parameters.AddWithValue("@uid", urunId);
                    cmdSatir.Parameters.AddWithValue("@mik", miktar);
                    cmdSatir.Parameters.AddWithValue("@alis", row["AlisFiyati"]);
                    cmdSatir.Parameters.AddWithValue("@satis", fiyat);
                    cmdSatir.Parameters.AddWithValue("@tut", tutar);
                    cmdSatir.ExecuteNonQuery();

                    // Hareket Ekle (Kalan Miktar Hesabı ile)
                    string sqlHareket = @"INSERT INTO Hareketler 
                                          (urun_id, kisi_id, girismiktari, cikismiktari, birimfiyat, toplamtutar, kalanmiktar, tarih)
                                          VALUES 
                                          (@uid, @kid, 0, @mik, @fiyat, @tut, 
                                          (SELECT ISNULL(SUM(girismiktari - cikismiktari), 0) FROM Hareketler WHERE urun_id = @uid) - @mik,
                                          @tarih)";

                    SqlCommand cmdHareket = new SqlCommand(sqlHareket, baglanti, islem);
                    cmdHareket.Parameters.AddWithValue("@uid", urunId);
                    cmdHareket.Parameters.AddWithValue("@kid", musteriId);
                    cmdHareket.Parameters.AddWithValue("@mik", miktar);
                    cmdHareket.Parameters.AddWithValue("@fiyat", fiyat);
                    cmdHareket.Parameters.AddWithValue("@tut", tutar);
                    cmdHareket.Parameters.AddWithValue("@tarih", DateTime.Now);
                    cmdHareket.ExecuteNonQuery();
                }

                // 3. BAKİYE GÜNCELLEME
                if (veresiye > 0)
                {
                    SqlCommand cmdBakiye = new SqlCommand("UPDATE Kisiler SET Bakiye = Bakiye + @tutar WHERE Id=@kisi", baglanti, islem);
                    cmdBakiye.Parameters.AddWithValue("@tutar", veresiye);
                    cmdBakiye.Parameters.AddWithValue("@kisi", musteriId);
                    cmdBakiye.ExecuteNonQuery();
                }

                islem.Commit();

                DialogResult cevap = MessageBox.Show("Satış Başarılı!\nFiş yazdırmak ister misiniz?", "Yazdır", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (cevap == DialogResult.Yes) FisYazdir();

                dtSepet.Rows.Clear();
                genelToplam = 0;
                lblGenelToplam.Text = "0.00 ₺";
            }
            catch (Exception ex)
            {
                islem.Rollback();
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally { baglanti.Close(); }
        }

        // --- İŞ MANTIĞI VE DİĞERLERİ ---

        void HizliButonlariDoldur()
        {
            // Önce temizle ve eventleri bağla
            for (int i = 1; i <= 12; i++)
            {
                Control[] btn = this.Controls.Find("btnUrun" + i, true);
                if (btn.Length > 0)
                {
                    btn[0].Text = "BOŞ";
                    btn[0].Tag = null;
                    btn[0].BackColor = Color.Orange;
                    btn[0].Click -= HizliUrun_Click;
                    btn[0].Click += HizliUrun_Click;
                }
            }
            // Veritabanından doldur
            try
            {
                if (baglanti.State == ConnectionState.Closed) baglanti.Open();
                SqlCommand cmd = new SqlCommand("SELECT HizliTus, Barkod, UrunAdi FROM Urunler WHERE HizliTus IS NOT NULL AND HizliTus > 0", baglanti);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string tusNo = dr["HizliTus"].ToString();
                    Control[] btn = this.Controls.Find("btnUrun" + tusNo, true);
                    if (btn.Length > 0)
                    {
                        btn[0].Text = dr["UrunAdi"].ToString();
                        btn[0].Tag = dr["Barkod"].ToString();
                        btn[0].BackColor = Color.SeaGreen;
                    }
                }
                dr.Close();
            }
            catch { }
            finally { baglanti.Close(); }
        }

        private void HizliUrun_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.Tag != null) UrunEkle(btn.Tag.ToString());
        }

        private void txtBarkod_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!string.IsNullOrEmpty(txtBarkod.Text))
                {
                    UrunEkle(txtBarkod.Text.Trim());
                    txtBarkod.Clear();
                }
                e.SuppressKeyPress = true;
            }
        }

        void UrunEkle(string barkod)
        {
            // Sepette var mı kontrolü (Aynı)
            foreach (DataRow row in dtSepet.Rows)
            {
                if (row["Barkod"].ToString() == barkod)
                {
                    row["Adet"] = Convert.ToInt32(row["Adet"]) + 1;
                    row["Tutar"] = Convert.ToInt32(row["Adet"]) * Convert.ToDecimal(row["SatisFiyati"]);
                    ToplamHesapla();
                    return;
                }
            }

            try
            {
                if (baglanti.State == ConnectionState.Closed) baglanti.Open();
                // KDV Oranını da çekiyoruz
                SqlCommand cmd = new SqlCommand("SELECT * FROM Urunler WHERE Barkod=@b", baglanti);
                cmd.Parameters.AddWithValue("@b", barkod);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    decimal satisFiyati = Convert.ToDecimal(dr["SatisFiyati"]);
                    decimal alisFiyati = Convert.ToDecimal(dr["AlisFiyati"]);
                    int kdv = Convert.ToInt32(dr["KdvOrani"]); // KDV geldi

                    dtSepet.Rows.Add(barkod, dr["UrunAdi"], 1, satisFiyati, satisFiyati, dr["Id"], alisFiyati, kdv);
                }
                else
                {
                    MessageBox.Show("Ürün Bulunamadı!");
                }
                dr.Close();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { baglanti.Close(); }

            ToplamHesapla();
        }

        void ToplamHesapla()
        {
            genelToplam = 0;
            foreach (DataRow row in dtSepet.Rows)
            {
                if (row.RowState != DataRowState.Deleted)
                    genelToplam += Convert.ToDecimal(row["Tutar"]);
            }
            lblGenelToplam.Text = genelToplam.ToString("C2");
        }

        // --- BUTON YÖNLENDİRMELERİ ---
        private void btnNakit_Click(object sender, EventArgs e) => SatisKaydet(genelToplam, 0, 0, "Nakit");
        private void btnKrediKarti_Click(object sender, EventArgs e) => SatisKaydet(0, genelToplam, 0, "Kredi Kartı");
        private void btnVeresiye_Click(object sender, EventArgs e) => SatisKaydet(0, 0, genelToplam, "Veresiye");

        private void btnParcali_Click(object sender, EventArgs e)
        {
            if (dtSepet.Rows.Count == 0) { MessageBox.Show("Sepet boş!"); return; }
            OdemeForm frm = new OdemeForm();
            frm.ToplamTutar = genelToplam;
            frm.ShowDialog();
            if (frm.IslemOnaylandi) SatisKaydet(frm.OdemeNakit, frm.OdemeKart, frm.OdemeVeresiye, "Parçalı");
        }

        // --- HESAP MAKİNESİ BUTONLARI ---
        private void Numpad_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (islemTiklandi) { txtBarkod.Clear(); islemTiklandi = false; }
            txtBarkod.Text += btn.Text;
            txtBarkod.Focus(); txtBarkod.SelectionStart = txtBarkod.Text.Length;
        }

        private void Islem_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string islemTuru = btn.Text;

            if (gridSepet.SelectedRows.Count == 0) return;

            DataGridViewRow row = gridSepet.SelectedRows[0];
            int mevcutAdet = Convert.ToInt32(row.Cells["Adet"].Value);
            decimal fiyat = Convert.ToDecimal(row.Cells["SatisFiyati"].Value);

            if (islemTuru == "+") row.Cells["Adet"].Value = mevcutAdet + 1;
            else if (islemTuru == "-" && mevcutAdet > 1) row.Cells["Adet"].Value = mevcutAdet - 1;
            else if (islemTuru == "*") // Adet Güncelleme
            {
                if (int.TryParse(txtBarkod.Text, out int yeniAdet) && yeniAdet > 0)
                {
                    row.Cells["Adet"].Value = yeniAdet;
                    txtBarkod.Clear();
                }
            }

            // Tutar Güncelle
            int sonAdet = Convert.ToInt32(row.Cells["Adet"].Value);
            row.Cells["Tutar"].Value = sonAdet * fiyat;

            ToplamHesapla();
            txtBarkod.Focus();
        }

        private void btnEsittir_Click(object sender, EventArgs e) { }
        private void btnTemizle_Click(object sender, EventArgs e) { txtBarkod.Clear(); }

        // --- YAZDIRMA ---
        void FisYazdir()
        {
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(FisTasarimi);
            PrintPreviewDialog onizleme = new PrintPreviewDialog();
            onizleme.Document = pd;
            onizleme.Width = 800; onizleme.Height = 600; onizleme.PrintPreviewControl.Zoom = 1.5;
            onizleme.ShowDialog();
        }

        void FisTasarimi(object sender, PrintPageEventArgs e)
        {
            // --- AYARLAR ---
            Font fontNormal = new Font("Courier New", 8, FontStyle.Regular);
            Font fontKalin = new Font("Courier New", 8, FontStyle.Bold);
            Font fontBaslik = new Font("Courier New", 12, FontStyle.Bold); // Başlık biraz küçüldü

            Brush firca = Brushes.Black;
            float genislik = 280;
            float x = 5;
            float y = 5;

            StringFormat orta = new StringFormat() { Alignment = StringAlignment.Center };
            StringFormat sag = new StringFormat() { Alignment = StringAlignment.Far };
            StringFormat sol = new StringFormat() { Alignment = StringAlignment.Near };

            // --- BAŞLIK (DB'den Gelen) ---
            e.Graphics.DrawString(isletmeAdi, fontBaslik, firca, new RectangleF(0, y, genislik, 20), orta);
            y += 20;
            e.Graphics.DrawString(isletmeAdres, fontNormal, firca, new RectangleF(0, y, genislik, 15), orta);
            y += 15;
            e.Graphics.DrawString(isletmeIlce + " / " + isletmeIl, fontNormal, firca, new RectangleF(0, y, genislik, 15), orta);
            y += 15;
            e.Graphics.DrawString("Tel: " + isletmeTel, fontNormal, firca, new RectangleF(0, y, genislik, 15), orta);
            y += 20;

            // --- FİŞ BİLGİLERİ ---
            // Günlük Fatura No'yu veritabanından çekmemiz lazım çünkü satış bitince değişken sıfırlandı.
            // Ancak SatisKaydet metodunda "sonKesilenFaturaId" değişkeni GLOBAL tutulduğu için
            // O ID'ye ait FaturaNo'yu çekip yazdırabiliriz.

            // (Basitlik adına: SatisKaydet'te faturaNo'yu global bir değişkene atayabilirsin veya
            // sonKesilenFaturaId'den SQL ile çekebilirsin. Ben SQL ile çekiyorum garanti olsun.)

            string gunlukNo = "0";
            try
            {
                if (baglanti.State == ConnectionState.Closed) baglanti.Open();
                SqlCommand cmdNo = new SqlCommand("SELECT FaturaNo FROM Faturalar WHERE Id=" + sonKesilenFaturaId, baglanti);
                gunlukNo = cmdNo.ExecuteScalar().ToString();
            }
            catch { }
            finally { if (baglanti.State == ConnectionState.Open) baglanti.Close(); }


            e.Graphics.DrawString("Tarih: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm"), fontNormal, firca, x, y);
            y += 12;
            e.Graphics.DrawString("Fiş No: " + gunlukNo, fontKalin, firca, x, y);
            y += 15;

            e.Graphics.DrawString("--------------------------------", fontNormal, firca, x, y);
            y += 12;

            // --- ÜRÜN LİSTESİ ---
            e.Graphics.DrawString("ÜRÜN ADI", fontKalin, firca, x, y);
            e.Graphics.DrawString("TUTAR", fontKalin, firca, new RectangleF(x, y, genislik - 15, 15), sag);
            y += 12;
            e.Graphics.DrawString("--------------------------------", fontNormal, firca, x, y);
            y += 12;

            // KDV Hesapları İçin Değişkenler
            decimal toplamKDV = 0;
            decimal matrahToplam = 0; // KDV hariç toplam
                                      // KDV oranlarına göre gruplama yapmak için basit sözlük (Dictionary)
            System.Collections.Generic.Dictionary<int, decimal> kdvGruplari = new System.Collections.Generic.Dictionary<int, decimal>();

            foreach (DataRow row in dtSepet.Rows)
            {
                if (row.RowState == DataRowState.Deleted) continue;

                string urunAdi = row["UrunAdi"].ToString();
                if (urunAdi.Length > 20) urunAdi = urunAdi.Substring(0, 18) + "..";

                decimal miktar = Convert.ToDecimal(row["Adet"]);
                decimal satisFiyati = Convert.ToDecimal(row["SatisFiyati"]); // KDV DAHİL FİYAT
                decimal satirTutar = Convert.ToDecimal(row["Tutar"]);
                int kdvOrani = Convert.ToInt32(row["KdvOrani"]);

                // KDV HESABI (İç yüzde yöntemi: Fiyat / (1 + Oran/100))
                decimal kdvHaricTutar = satirTutar / (1 + (decimal)kdvOrani / 100);
                decimal kdvTutari = satirTutar - kdvHaricTutar;

                toplamKDV += kdvTutari;
                matrahToplam += kdvHaricTutar;

                // KDV Gruplarına Ekle
                if (kdvGruplari.ContainsKey(kdvOrani))
                    kdvGruplari[kdvOrani] += kdvTutari;
                else
                    kdvGruplari.Add(kdvOrani, kdvTutari);

                // Satırı Yazdır
                e.Graphics.DrawString(urunAdi, fontNormal, firca, x, y);
                e.Graphics.DrawString(satirTutar.ToString("N2"), fontNormal, firca, new RectangleF(x, y, genislik - 15, 15), sag);
                y += 12;
                e.Graphics.DrawString($"{miktar} Adet x {satisFiyati:N2} (%{kdvOrani})", new Font("Courier New", 7), Brushes.Gray, x + 5, y);
                y += 15;
            }

            e.Graphics.DrawString("--------------------------------", fontNormal, firca, x, y);
            y += 12;

            // --- ALT TOPLAMLAR ---

            // Toplam KDV
            e.Graphics.DrawString("TOPLAM KDV:", fontNormal, firca, x, y);
            e.Graphics.DrawString(toplamKDV.ToString("N2"), fontNormal, firca, new RectangleF(x, y, genislik - 15, 15), sag);
            y += 12;

            // KDV Hariç Toplam
            e.Graphics.DrawString("KDV HARİÇ TOPLAM:", fontNormal, firca, x, y);
            e.Graphics.DrawString(matrahToplam.ToString("N2"), fontNormal, firca, new RectangleF(x, y, genislik - 15, 15), sag);
            y += 12;

            // KDV Dökümü (%1, %8, %18 vs.)
            foreach (var kdv in kdvGruplari)
            {
                e.Graphics.DrawString($"KDV %{kdv.Key} Toplam:", new Font("Courier New", 7), firca, x, y);
                e.Graphics.DrawString(kdv.Value.ToString("N2"), new Font("Courier New", 7), firca, new RectangleF(x, y, genislik - 15, 15), sag);
                y += 10;
            }

            e.Graphics.DrawString("--------------------------------", fontNormal, firca, x, y);
            y += 12;

            // GENEL TOPLAM
            e.Graphics.DrawString("GENEL TOPLAM", fontBaslik, firca, x, y);
            e.Graphics.DrawString(genelToplam.ToString("C2"), fontBaslik, firca, new RectangleF(x, y, genislik - 15, 25), sag);
            y += 40;

            // --- ALT BİLGİ ---
            e.Graphics.DrawString("MALİ DEĞERİ YOKTUR", fontKalin, firca, new RectangleF(0, y, genislik, 15), orta);
            y += 15;
            e.Graphics.DrawString("İYİ GÜNLER DİLERİZ", fontNormal, firca, new RectangleF(0, y, genislik, 15), orta);
        }
    }
}