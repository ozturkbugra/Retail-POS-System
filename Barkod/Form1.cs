using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Barkod
{
    public partial class Form1 : Form
    {
        // --- VERİTABANI BAĞLANTISI ---
        SqlConnection baglanti = Veritabani.BaglantiGetir();


        DataTable dtSepet = new DataTable();
        decimal genelToplam = 0;
        int sonKesilenFaturaId = 0;

        // Hesaplama kilidi (Sonsuz döngüyü engeller)
        bool hesaplaniyor = false;

        public Form1()
        {
            InitializeComponent();
        }

        string isletmeAdi = "", isletmeAdres = "", isletmeIlce = "", isletmeIl = "", isletmeTel = "";

        private void Form1_Load(object sender, EventArgs e)
        {
            AyarlariGetir();
            GridTasarim();
            MusterileriYukle();
            HizliButonlariDoldur();
            txtBarkod.Focus();

            // Olayları Bağla
            gridSepet.CellContentClick += gridSepet_CellContentClick;
            gridSepet.CellValueChanged += gridSepet_CellValueChanged;
            gridSepet.DataError += gridSepet_DataError;
        }

        // --- 1. İŞLETME BİLGİLERİ ---
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

        // --- 2. GRID YAPISI ---
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
                dtSepet.Columns.Add("KdvOrani", typeof(int));
            }

            gridSepet.DataSource = dtSepet;

            gridSepet.Columns["UrunId"].Visible = false;
            gridSepet.Columns["AlisFiyati"].Visible = false;
            gridSepet.Columns["KdvOrani"].Visible = false;

            gridSepet.Columns["UrunAdi"].HeaderText = "Ürün Adı";
            gridSepet.Columns["SatisFiyati"].HeaderText = "Fiyat";
            gridSepet.Columns["Adet"].HeaderText = "Adet";
            gridSepet.Columns["Tutar"].HeaderText = "Top.";

            gridSepet.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridSepet.RowHeadersVisible = false;
            gridSepet.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridSepet.RowTemplate.Height = 35;
            gridSepet.DefaultCellStyle.Font = new Font("Segoe UI", 11F);
            gridSepet.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);

            gridSepet.ReadOnly = false;
            gridSepet.Columns["Barkod"].ReadOnly = true;
            gridSepet.Columns["UrunAdi"].ReadOnly = true;
            gridSepet.Columns["Tutar"].ReadOnly = true;

            // Sil Butonu
            if (!gridSepet.Columns.Contains("btnSil"))
            {
                DataGridViewButtonColumn btnSil = new DataGridViewButtonColumn();
                btnSil.Text = "X";
                btnSil.Name = "btnSil";
                btnSil.UseColumnTextForButtonValue = true;
                btnSil.FlatStyle = FlatStyle.Flat;
                btnSil.DefaultCellStyle.BackColor = Color.Crimson;
                btnSil.DefaultCellStyle.ForeColor = Color.White;
                btnSil.Width = 40;
                gridSepet.Columns.Add(btnSil);
            }
        }

        // --- 3. HESAPLAMA VE GRID OLAYLARI ---
        private void gridSepet_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (hesaplaniyor || e.RowIndex < 0) return;

            try
            {
                hesaplaniyor = true;
                DataGridViewRow row = gridSepet.Rows[e.RowIndex];
                string kolunAdi = gridSepet.Columns[e.ColumnIndex].Name;

                int adet = row.Cells["Adet"].Value != DBNull.Value ? Convert.ToInt32(row.Cells["Adet"].Value) : 0;
                decimal fiyat = row.Cells["SatisFiyati"].Value != DBNull.Value ? Convert.ToDecimal(row.Cells["SatisFiyati"].Value) : 0;

                if (kolunAdi == "Adet" || kolunAdi == "SatisFiyati")
                {
                    row.Cells["Tutar"].Value = adet * fiyat;
                }

                hesaplaniyor = false;
                ToplamHesapla();
            }
            catch { hesaplaniyor = false; }
        }

        private void gridSepet_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
            MessageBox.Show("Lütfen geçerli bir sayı giriniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

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

        // --- 4. VERİ YÜKLEME ---
        void MusterileriYukle()
        {
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT Id, AdSoyad FROM Kisiler ORDER BY AdSoyad", baglanti);
                DataTable dtMusteri = new DataTable();
                da.Fill(dtMusteri);
                cmbKisiler.DataSource = dtMusteri;
                cmbKisiler.DisplayMember = "AdSoyad";
                cmbKisiler.ValueMember = "Id";
            }
            catch { }
        }

        void HizliButonlariDoldur()
        {
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

            try
            {
                if (baglanti.State == ConnectionState.Closed) baglanti.Open();
                SqlCommand cmd = new SqlCommand("SELECT HizliTus, Barkod, UrunAdi FROM Urunler WHERE HizliTus > 0", baglanti);
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

        // --- 5. ÜRÜN EKLEME ---
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
                SqlCommand cmd = new SqlCommand("SELECT * FROM Urunler WHERE Barkod=@b", baglanti);
                cmd.Parameters.AddWithValue("@b", barkod);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    decimal satis = Convert.ToDecimal(dr["SatisFiyati"]);
                    decimal alis = Convert.ToDecimal(dr["AlisFiyati"]);
                    int kdv = Convert.ToInt32(dr["KdvOrani"]);
                    dtSepet.Rows.Add(barkod, dr["UrunAdi"], 1, satis, satis, dr["Id"], alis, kdv);
                }
                else
                {
                    MessageBox.Show("Ürün Bulunamadı!");
                }
                dr.Close();
            }
            catch { }
            finally { baglanti.Close(); }

            ToplamHesapla();
        }

        // --- 6. SATIŞ KAYDETME (DÜZENLENDİ) ---
        void SatisKaydet(decimal nakit, decimal kart, decimal veresiye, string odemeTipiMetin)
        {
            if (dtSepet.Rows.Count == 0) { MessageBox.Show("Sepet boş!"); return; }

            // -- ONAY SORUSU --
            DialogResult onay = MessageBox.Show(
                $"Toplam: {genelToplam:C2}\nÖdeme: {odemeTipiMetin}\n\nSatışı onaylıyor musunuz?",
                "Satış Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (onay != DialogResult.Yes) return;

            if (cmbKisiler.SelectedValue == null) { MessageBox.Show("Müşteri seçiniz!"); return; }
            int musteriId = Convert.ToInt32(cmbKisiler.SelectedValue);

            if (baglanti.State == ConnectionState.Closed) baglanti.Open();
            SqlTransaction islem = baglanti.BeginTransaction();

            try
            {
                // Fatura No
                SqlCommand cmdNo = new SqlCommand("SELECT ISNULL(MAX(FaturaNo), 0) + 1 FROM Faturalar WHERE CONVERT(DATE, Tarih) = CONVERT(DATE, @bugun)", baglanti, islem);
                cmdNo.Parameters.AddWithValue("@bugun", DateTime.Now);
                int yeniFaturaNo = Convert.ToInt32(cmdNo.ExecuteScalar());

                // Fatura Ekle
                string sqlFatura = @"INSERT INTO Faturalar (KisiId, Tarih, OdemeTipi, GenelToplam, TutarNakit, TutarKrediKarti, TutarVeresiye, FaturaNo) 
                                     VALUES (@kisi, @tarih, @tip, @toplam, @nakit, @kart, @veresiye, @fno); SELECT SCOPE_IDENTITY();";
                SqlCommand cmdFat = new SqlCommand(sqlFatura, baglanti, islem);
                cmdFat.Parameters.AddWithValue("@kisi", musteriId);
                cmdFat.Parameters.AddWithValue("@tarih", DateTime.Now);
                cmdFat.Parameters.AddWithValue("@tip", odemeTipiMetin);
                cmdFat.Parameters.AddWithValue("@toplam", genelToplam);
                cmdFat.Parameters.AddWithValue("@nakit", nakit);
                cmdFat.Parameters.AddWithValue("@kart", kart);
                cmdFat.Parameters.AddWithValue("@veresiye", veresiye);
                cmdFat.Parameters.AddWithValue("@fno", yeniFaturaNo);

                sonKesilenFaturaId = Convert.ToInt32(cmdFat.ExecuteScalar());

                // Detaylar
                foreach (DataRow row in dtSepet.Rows)
                {
                    if (row.RowState == DataRowState.Deleted) continue;

                    int uid = Convert.ToInt32(row["UrunId"]);
                    decimal miktar = Convert.ToDecimal(row["Adet"]);
                    decimal fiyat = Convert.ToDecimal(row["SatisFiyati"]);
                    decimal tutar = Convert.ToDecimal(row["Tutar"]);

                    // Satır Ekle
                    SqlCommand cmdSatir = new SqlCommand("INSERT INTO FaturaSatirlar (FaturaId, UrunId, Miktar, AlisFiyati, SatisFiyati, Tutar) VALUES (@fid, @uid, @mik, @alis, @satis, @tut)", baglanti, islem);
                    cmdSatir.Parameters.AddWithValue("@fid", sonKesilenFaturaId);
                    cmdSatir.Parameters.AddWithValue("@uid", uid);
                    cmdSatir.Parameters.AddWithValue("@mik", miktar);
                    cmdSatir.Parameters.AddWithValue("@alis", row["AlisFiyati"]);
                    cmdSatir.Parameters.AddWithValue("@satis", fiyat);
                    cmdSatir.Parameters.AddWithValue("@tut", tutar);
                    cmdSatir.ExecuteNonQuery();

                    // Stok Hareket
                    string sqlHar = @"INSERT INTO Hareketler (urun_id, kisi_id, girismiktari, cikismiktari, birimfiyat, toplamtutar, kalanmiktar, tarih) 
                                      VALUES (@uid, @kid, 0, @mik, @fiyat, @tut, 
                                      (SELECT ISNULL(SUM(girismiktari - cikismiktari), 0) FROM Hareketler WHERE urun_id = @uid) - @mik, @tarih)";
                    SqlCommand cmdHar = new SqlCommand(sqlHar, baglanti, islem);
                    cmdHar.Parameters.AddWithValue("@uid", uid);
                    cmdHar.Parameters.AddWithValue("@kid", musteriId);
                    cmdHar.Parameters.AddWithValue("@mik", miktar);
                    cmdHar.Parameters.AddWithValue("@fiyat", fiyat);
                    cmdHar.Parameters.AddWithValue("@tut", tutar);
                    cmdHar.Parameters.AddWithValue("@tarih", DateTime.Now);
                    cmdHar.ExecuteNonQuery();
                }

                if (veresiye > 0)
                {
                    SqlCommand cmdBak = new SqlCommand("UPDATE Kisiler SET Bakiye = Bakiye + @tutar WHERE Id=@kisi", baglanti, islem);
                    cmdBak.Parameters.AddWithValue("@tutar", veresiye);
                    cmdBak.Parameters.AddWithValue("@kisi", musteriId);
                    cmdBak.ExecuteNonQuery();
                }

                islem.Commit();

                // -- OTOMATİK FİŞ YAZDIRMA --
                FisYazdir();

                // Temizlik
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

        // --- 7. FİŞ VE YAZDIRMA (DÜZENLENDİ - TERMAL) ---
        void FisYazdir()
        {
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(FisTasarimi);

            // Termal Boyut Hesabı (80mm Genişlik, Dinamik Yükseklik)
            // Header ~280px + (Satır Sayısı * 30px) + Footer
            int satirSayisi = dtSepet.Rows.Count;
            int hesaplananYukseklik = 350 + (satirSayisi * 30);

            // 315 birim = yaklaşık 80mm
            pd.DefaultPageSettings.PaperSize = new PaperSize("TermalFis", 315, hesaplananYukseklik);
            pd.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);

            PrintPreviewDialog onizleme = new PrintPreviewDialog();
            onizleme.Document = pd;
            onizleme.ShowIcon = false;
            onizleme.Text = "Fiş Önizleme";
            onizleme.Width = 420; // Telefon ekranı gibi dar
            onizleme.Height = 600;
            onizleme.PrintPreviewControl.Zoom = 1.0;

            // Kullanıcı bu pencereyi kapatırsa yazdırmaz, yazıcı ikonuna basarsa yazdırır.
            onizleme.ShowDialog();
        }

        void FisTasarimi(object sender, PrintPageEventArgs e)
        {
            Font fontNormal = new Font("Courier New", 8, FontStyle.Regular);
            Font fontKalin = new Font("Courier New", 8, FontStyle.Bold);
            Font fontBaslik = new Font("Courier New", 12, FontStyle.Bold);
            Font fontKucuk = new Font("Courier New", 7, FontStyle.Regular);

            Brush firca = Brushes.Black;
            float genislik = 280; // Yazılabilir alan
            float x = 5;
            float y = 5;

            StringFormat orta = new StringFormat() { Alignment = StringAlignment.Center };
            StringFormat sag = new StringFormat() { Alignment = StringAlignment.Far };

            // Başlık
            e.Graphics.DrawString(isletmeAdi, fontBaslik, firca, new RectangleF(0, y, 315, 20), orta); y += 20;
            e.Graphics.DrawString(isletmeAdres, fontNormal, firca, new RectangleF(0, y, 315, 15), orta); y += 15;
            e.Graphics.DrawString(isletmeIlce + " / " + isletmeIl, fontNormal, firca, new RectangleF(0, y, 315, 15), orta); y += 15;
            e.Graphics.DrawString("Tel: " + isletmeTel, fontNormal, firca, new RectangleF(0, y, 315, 15), orta); y += 25;

            // Fiş No
            string gunlukNo = "0";
            try
            {
                if (baglanti.State == ConnectionState.Closed) baglanti.Open();
                SqlCommand cmdNo = new SqlCommand("SELECT FaturaNo FROM Faturalar WHERE Id=" + sonKesilenFaturaId, baglanti);
                var snc = cmdNo.ExecuteScalar();
                if (snc != null) gunlukNo = snc.ToString();
            }
            catch { }
            finally { if (baglanti.State == ConnectionState.Open) baglanti.Close(); }

            e.Graphics.DrawString("Tarih: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm"), fontNormal, firca, x, y); y += 12;
            e.Graphics.DrawString("Fiş No: " + gunlukNo, fontKalin, firca, x, y); y += 15;
            e.Graphics.DrawString("----------------------------------------", fontNormal, firca, x, y); y += 12;

            e.Graphics.DrawString("ÜRÜN", fontKalin, firca, x, y);
            e.Graphics.DrawString("TUTAR", fontKalin, firca, new RectangleF(x, y, genislik, 15), sag); y += 12;
            e.Graphics.DrawString("----------------------------------------", fontNormal, firca, x, y); y += 12;

            decimal topKdv = 0, topMatrah = 0;
            Dictionary<int, decimal> kdvList = new Dictionary<int, decimal>();

            // Ürün Listesi
            foreach (DataRow row in dtSepet.Rows)
            {
                if (row.RowState == DataRowState.Deleted) continue;

                string urun = row["UrunAdi"].ToString();
                if (urun.Length > 18) urun = urun.Substring(0, 16) + "..";

                decimal miktar = Convert.ToDecimal(row["Adet"]);
                decimal fiyat = Convert.ToDecimal(row["SatisFiyati"]);
                decimal tutar = Convert.ToDecimal(row["Tutar"]);
                int kdv = Convert.ToInt32(row["KdvOrani"]);

                decimal kdvHaric = tutar / (1 + (decimal)kdv / 100);
                topKdv += (tutar - kdvHaric);
                topMatrah += kdvHaric;

                if (kdvList.ContainsKey(kdv)) kdvList[kdv] += (tutar - kdvHaric);
                else kdvList.Add(kdv, (tutar - kdvHaric));

                e.Graphics.DrawString(urun, fontNormal, firca, x, y);
                e.Graphics.DrawString(tutar.ToString("N2"), fontNormal, firca, new RectangleF(x, y, genislik, 15), sag); y += 12;
                e.Graphics.DrawString($"{miktar} x {fiyat:N2}", fontKucuk, Brushes.Gray, x + 5, y); y += 15;
            }

            e.Graphics.DrawString("----------------------------------------", fontNormal, firca, x, y); y += 12;

            // Alt Toplamlar
            e.Graphics.DrawString("TOPLAM KDV:", fontNormal, firca, x, y);
            e.Graphics.DrawString(topKdv.ToString("N2"), fontNormal, firca, new RectangleF(x, y, genislik, 15), sag); y += 12;

            e.Graphics.DrawString("KDV HARİÇ:", fontNormal, firca, x, y);
            e.Graphics.DrawString(topMatrah.ToString("N2"), fontNormal, firca, new RectangleF(x, y, genislik, 15), sag); y += 12;

            foreach (var k in kdvList)
            {
                e.Graphics.DrawString($"KDV %{k.Key}:", fontKucuk, firca, x, y);
                e.Graphics.DrawString(k.Value.ToString("N2"), fontKucuk, firca, new RectangleF(x, y, genislik, 15), sag); y += 10;
            }

            e.Graphics.DrawString("----------------------------------------", fontNormal, firca, x, y); y += 12;

            // Genel Toplam
            e.Graphics.DrawString("GENEL TOPLAM", fontBaslik, firca, x, y);
            e.Graphics.DrawString(genelToplam.ToString("C2"), fontBaslik, firca, new RectangleF(x, y, genislik, 25), sag); y += 40;

            e.Graphics.DrawString("MALİ DEĞERİ YOKTUR", fontKalin, firca, new RectangleF(0, y, 315, 15), orta); y += 15;
            e.Graphics.DrawString("İYİ GÜNLER DİLERİZ", fontNormal, firca, new RectangleF(0, y, 315, 15), orta);
        }

        // --- 8. BUTONLAR ---
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

        private void Numpad_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            txtBarkod.Text += btn.Text;
            txtBarkod.Focus();
            txtBarkod.SelectionStart = txtBarkod.Text.Length;
        }

        private void Islem_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string islemTuru = btn.Text;
            if (gridSepet.SelectedRows.Count == 0) return;
            DataGridViewRow row = gridSepet.SelectedRows[0];
            int mevcutAdet = Convert.ToInt32(row.Cells["Adet"].Value);

            if (islemTuru == "+") row.Cells["Adet"].Value = mevcutAdet + 1;
            else if (islemTuru == "-" && mevcutAdet > 1) row.Cells["Adet"].Value = mevcutAdet - 1;

            ToplamHesapla();
            txtBarkod.Focus();
        }

        private void btnTemizle_Click(object sender, EventArgs e)
        {
            txtBarkod.Clear();
            txtBarkod.Focus();
        }

        // Eşittir butonu boş kalabilir veya ödeme ekranını açabilir
        private void btnEsittir_Click(object sender, EventArgs e) { }
    }
}