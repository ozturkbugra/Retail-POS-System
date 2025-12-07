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
        // BAĞLANTI ADRESİ
        SqlConnection baglanti = new SqlConnection(@"Data Source=.;Initial Catalog=BakkalDB;Integrated Security=True");

        DataTable dtSepet = new DataTable();
        decimal genelToplam = 0;
        int sonKesilenFaturaId = 0;

        // Hesap Makinesi Değişkenleri
        double sayi1 = 0;
        string islem = "";
        bool islemTiklandi = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            GridTasarim();
            MusterileriYukle();
            HizliButonlariDoldur();
            txtBarkod.Focus();
            gridSepet.CellContentClick += gridSepet_CellContentClick;
        }

        private void gridSepet_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Tıklanan yer bir satırsa (başlık değilse) VE Tıklanan kolon "btnSil" ise
            if (e.RowIndex >= 0 && gridSepet.Columns[e.ColumnIndex].Name == "btnSil")
            {
                dtSepet.Rows[e.RowIndex].Delete();
                ToplamHesapla();
                txtBarkod.Focus();
            }
        }

        // --- TASARIM VE AYARLAR ---
        void GridTasarim()
        {
            dtSepet.Columns.Add("Barkod");
            dtSepet.Columns.Add("UrunAdi");
            dtSepet.Columns.Add("Adet", typeof(int));
            dtSepet.Columns.Add("SatisFiyati", typeof(decimal));
            dtSepet.Columns.Add("Tutar", typeof(decimal));

            // Gizli Kolonlar
            dtSepet.Columns.Add("UrunId", typeof(int));
            dtSepet.Columns.Add("AlisFiyati", typeof(decimal));

            gridSepet.DataSource = dtSepet;

            gridSepet.Columns["UrunId"].Visible = false;
            gridSepet.Columns["AlisFiyati"].Visible = false;

            gridSepet.Columns["UrunAdi"].HeaderText = "Ürün Adı";
            gridSepet.Columns["SatisFiyati"].HeaderText = "Fiyat";

            gridSepet.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridSepet.RowHeadersVisible = false;
            gridSepet.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridSepet.ReadOnly = true;

            // --- SİL BUTONU ---
            if (!gridSepet.Columns.Contains("btnSil"))
            {
                DataGridViewButtonColumn btnSil = new DataGridViewButtonColumn();
                btnSil.HeaderText = "";
                btnSil.Text = "SİL";
                btnSil.Name = "btnSil";
                btnSil.UseColumnTextForButtonValue = true;
                btnSil.FlatStyle = FlatStyle.Flat;
                btnSil.DefaultCellStyle.BackColor = Color.Crimson;
                btnSil.DefaultCellStyle.ForeColor = Color.White;
                btnSil.DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                gridSepet.Columns.Add(btnSil);
            }
        }

        void MusterileriYukle()
        {
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT Id, AdSoyad, Bakiye FROM Kisiler ORDER BY AdSoyad", baglanti);
                DataTable dtMusteri = new DataTable();
                da.Fill(dtMusteri);

                cmbKisiler.DataSource = dtMusteri;
                cmbKisiler.DisplayMember = "AdSoyad";
                cmbKisiler.ValueMember = "Id";
            }
            catch (Exception ex) { MessageBox.Show("Müşteri hatası: " + ex.Message); }
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
            catch (Exception ex) { MessageBox.Show("Buton hatası: " + ex.Message); }
            finally { baglanti.Close(); }
        }

        // --- BUTON OLAYLARI ---
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

        // --- ÖDEME BUTONLARI (Hepsi tek bir merkezi metoda gidiyor) ---
        private void btnNakit_Click(object sender, EventArgs e)
        {
            SatisKaydet(genelToplam, 0, 0, "Nakit");
        }

        private void btnKrediKarti_Click(object sender, EventArgs e)
        {
            SatisKaydet(0, genelToplam, 0, "Kredi Kartı");
        }

        private void btnVeresiye_Click(object sender, EventArgs e)
        {
            SatisKaydet(0, 0, genelToplam, "Veresiye");
        }

        private void btnParcali_Click(object sender, EventArgs e)
        {
            if (dtSepet.Rows.Count == 0) { MessageBox.Show("Sepet boş!"); return; }

            OdemeForm frm = new OdemeForm();
            frm.ToplamTutar = genelToplam;
            frm.ShowDialog();

            if (frm.IslemOnaylandi)
            {
                SatisKaydet(frm.OdemeNakit, frm.OdemeKart, frm.OdemeVeresiye, "Parçalı");
            }
        }

        // --- İŞ MANTIĞI ---
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
                    decimal satisFiyati = Convert.ToDecimal(dr["SatisFiyati"]);
                    decimal alisFiyati = Convert.ToDecimal(dr["AlisFiyati"]);
                    dtSepet.Rows.Add(barkod, dr["UrunAdi"], 1, satisFiyati, satisFiyati, dr["Id"], alisFiyati);
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
            foreach (DataRow row in dtSepet.Rows) genelToplam += Convert.ToDecimal(row["Tutar"]);
            lblGenelToplam.Text = genelToplam.ToString("C2");
        }

        // --- MERKEZİ SATIŞ KAYDETME METODU ---
        void SatisKaydet(decimal nakit, decimal kart, decimal veresiye, string odemeTipiMetin)
        {
            if (dtSepet.Rows.Count == 0) { MessageBox.Show("Sepet boş!"); return; }

            int musteriId = Convert.ToInt32(cmbKisiler.SelectedValue);

            if (baglanti.State == ConnectionState.Closed) baglanti.Open();
            SqlTransaction islem = baglanti.BeginTransaction();

            try
            {
                // 1. FATURA KAYDI
                string sqlFatura = @"INSERT INTO Faturalar 
                                    (KisiId, Tarih, OdemeTipi, GenelToplam, TutarNakit, TutarKrediKarti, TutarVeresiye) 
                                    VALUES (@kisi, @tarih, @tip, @toplam, @nakit, @kart, @veresiye); 
                                    SELECT SCOPE_IDENTITY();";

                SqlCommand cmdFatura = new SqlCommand(sqlFatura, baglanti, islem);
                cmdFatura.Parameters.AddWithValue("@kisi", musteriId);
                cmdFatura.Parameters.AddWithValue("@tarih", DateTime.Now);
                cmdFatura.Parameters.AddWithValue("@tip", odemeTipiMetin);
                cmdFatura.Parameters.AddWithValue("@toplam", genelToplam);
                cmdFatura.Parameters.AddWithValue("@nakit", nakit);
                cmdFatura.Parameters.AddWithValue("@kart", kart);
                cmdFatura.Parameters.AddWithValue("@veresiye", veresiye);

                sonKesilenFaturaId = Convert.ToInt32(cmdFatura.ExecuteScalar());

                // 2. SATIRLARI KAYDET
                foreach (DataRow row in dtSepet.Rows)
                {
                    string sqlSatir = @"INSERT INTO FaturaSatirlar (FaturaId, UrunId, Miktar, AlisFiyati, SatisFiyati, Tutar) 
                                        VALUES (@fid, @uid, @mik, @alis, @satis, @tut)";

                    SqlCommand cmdSatir = new SqlCommand(sqlSatir, baglanti, islem);
                    cmdSatir.Parameters.AddWithValue("@fid", sonKesilenFaturaId);
                    cmdSatir.Parameters.AddWithValue("@uid", row["UrunId"]);
                    cmdSatir.Parameters.AddWithValue("@mik", row["Adet"]);
                    cmdSatir.Parameters.AddWithValue("@alis", row["AlisFiyati"]);
                    cmdSatir.Parameters.AddWithValue("@satis", row["SatisFiyati"]);
                    cmdSatir.Parameters.AddWithValue("@tut", row["Tutar"]);
                    cmdSatir.ExecuteNonQuery();

                    // Stok Düş
                    SqlCommand cmdStok = new SqlCommand("UPDATE Urunler SET Stok = Stok - @adet WHERE Id=@uid", baglanti, islem);
                    cmdStok.Parameters.AddWithValue("@adet", row["Adet"]);
                    cmdStok.Parameters.AddWithValue("@uid", row["UrunId"]);
                    cmdStok.ExecuteNonQuery();
                }

                // 3. EĞER VERESİYE VARSA MÜŞTERİ BAKİYESİNİ GÜNCELLE
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

        // --- YAZDIRMA ---
        void FisYazdir()
        {
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(FisTasarimi);
            PrintPreviewDialog onizleme = new PrintPreviewDialog();
            onizleme.Document = pd;
            onizleme.Width = 800;
            onizleme.Height = 600;
            onizleme.PrintPreviewControl.Zoom = 1.5;
            onizleme.ShowDialog();
        }

        void FisTasarimi(object sender, PrintPageEventArgs e)
        {
            Font baslik = new Font("Arial", 12, FontStyle.Bold);
            Font normal = new Font("Arial", 8);
            Brush firca = Brushes.Black;
            float y = 10, x = 10, w = 270;
            StringFormat saga = new StringFormat { Alignment = StringAlignment.Far };
            StringFormat orta = new StringFormat { Alignment = StringAlignment.Center };

            e.Graphics.DrawString("AGANIN BAKKALI", baslik, firca, new RectangleF(0, y, w, 20), orta); y += 25;
            e.Graphics.DrawString("Fiş No: " + sonKesilenFaturaId, normal, firca, x, y); y += 15;
            e.Graphics.DrawString("Tarih: " + DateTime.Now, normal, firca, x, y); y += 15;
            e.Graphics.DrawString("Müşteri: " + cmbKisiler.Text, normal, firca, x, y); y += 20;
            e.Graphics.DrawString("------------------------------------------------", normal, firca, x, y); y += 15;

            foreach (DataRow row in dtSepet.Rows)
            {
                string ad = row["UrunAdi"].ToString();
                if (ad.Length > 15) ad = ad.Substring(0, 15) + "..";
                string tutar = Convert.ToDecimal(row["Tutar"]).ToString("C2");

                e.Graphics.DrawString(ad, normal, firca, x, y);
                e.Graphics.DrawString(tutar, normal, firca, new RectangleF(x, y, w - 20, 15), saga); y += 15;
                e.Graphics.DrawString($"{row["Adet"]} x {Convert.ToDecimal(row["SatisFiyati"]):C2}", new Font("Arial", 7, FontStyle.Italic), Brushes.Gray, x + 5, y); y += 15;
            }

            e.Graphics.DrawString("------------------------------------------------", normal, firca, x, y); y += 15;
            e.Graphics.DrawString("TOPLAM: " + genelToplam.ToString("C2"), baslik, firca, new RectangleF(x, y, w - 20, 20), saga);
        }

        // --- HESAP MAKİNESİ ---
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
            try { sayi1 = Convert.ToDouble(txtBarkod.Text); islem = btn.Text; islemTiklandi = true; } catch { }
        }

        private void btnEsittir_Click(object sender, EventArgs e)
        {
            try
            {
                double sayi2 = Convert.ToDouble(txtBarkod.Text);
                double sonuc = 0;
                if (islem == "+") sonuc = sayi1 + sayi2;
                if (islem == "-") sonuc = sayi1 - sayi2;
                if (islem == "*") sonuc = sayi1 * sayi2;
                if (islem == "/") sonuc = sayi1 / sayi2;
                txtBarkod.Text = sonuc.ToString();
            }
            catch { }
        }

        private void btnTemizle_Click(object sender, EventArgs e)
        {
            txtBarkod.Clear(); sayi1 = 0; islem = ""; txtBarkod.Focus();
        }
    }
}