using System;
using System.Windows.Forms;

namespace Barkod
{
    public partial class OdemeForm : Form
    {
        public decimal ToplamTutar { get; set; }

        // Bu değerleri Form1'e geri göndereceğiz
        public decimal OdemeNakit { get; private set; }
        public decimal OdemeKart { get; private set; }
        public decimal OdemeVeresiye { get; private set; }
        public bool IslemOnaylandi { get; private set; } = false;

        public OdemeForm()
        {
            InitializeComponent();
        }

        private void OdemeForm_Load(object sender, EventArgs e)
        {
            lblGenelToplam.Text = ToplamTutar.ToString("C2");
            // Varsayılan olarak hepsini nakit yapalım, kullanıcı değiştirsin
            txtNakit.Text = ToplamTutar.ToString();
        }

        private void Hesapla(object sender, EventArgs e)
        {
            try
            {
                decimal nakit = string.IsNullOrEmpty(txtNakit.Text) ? 0 : Convert.ToDecimal(txtNakit.Text);
                decimal kart = string.IsNullOrEmpty(txtKrediKarti.Text) ? 0 : Convert.ToDecimal(txtKrediKarti.Text);
                decimal veresiye = string.IsNullOrEmpty(txtVeresiye.Text) ? 0 : Convert.ToDecimal(txtVeresiye.Text);

                decimal girilenToplam = nakit + kart + veresiye;
                decimal fark = ToplamTutar - girilenToplam;

                if (fark > 0)
                {
                    lblKalan.Text = "Eksik: " + fark.ToString("C2");
                    lblKalan.ForeColor = System.Drawing.Color.Red;
                    btnOnayla.Enabled = false;
                }
                else if (fark < 0)
                {
                    lblKalan.Text = "Para Üstü: " + (fark * -1).ToString("C2");
                    lblKalan.ForeColor = System.Drawing.Color.Green;
                    btnOnayla.Enabled = true; // Para üstü verilebilir, onaya izin ver
                }
                else
                {
                    lblKalan.Text = "Tamamlandı";
                    lblKalan.ForeColor = System.Drawing.Color.Blue;
                    btnOnayla.Enabled = true;
                }
            }
            catch
            {
                btnOnayla.Enabled = false;
            }
        }

        private void btnOnayla_Click(object sender, EventArgs e)
        {
            OdemeNakit = string.IsNullOrEmpty(txtNakit.Text) ? 0 : Convert.ToDecimal(txtNakit.Text);
            OdemeKart = string.IsNullOrEmpty(txtKrediKarti.Text) ? 0 : Convert.ToDecimal(txtKrediKarti.Text);
            OdemeVeresiye = string.IsNullOrEmpty(txtVeresiye.Text) ? 0 : Convert.ToDecimal(txtVeresiye.Text);

            // Para üstü varsa onu nakitten düşerek gerçek kasaya giren nakiti hesapla
            decimal girilen = OdemeNakit + OdemeKart + OdemeVeresiye;
            if (girilen > ToplamTutar)
            {
                decimal paraUstu = girilen - ToplamTutar;
                OdemeNakit = OdemeNakit - paraUstu; // Para üstünü nakitten verdik sayıyoruz
            }

            IslemOnaylandi = true;
            this.Close();
        }

        // Tasarımcı hatasını çözen metod
        private void btnIptal_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SadeceSayi(object sender, KeyPressEventArgs e)
        {
            // Sadece sayı ve virgül girişine izin ver
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
            {
                e.Handled = true;
            }
        }
    }
}