using System;
using System.Windows.Forms;

namespace Barkod
{
    public partial class AnaMenu : Form
    {
        public AnaMenu()
        {
            InitializeComponent();
        }

        private void btnSatisYap_Click(object sender, EventArgs e)
        {
            Form1 satisEkrani = new Form1();
            satisEkrani.ShowDialog();
        }

        private void btnDigerleri_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            if (btn.Name == "btnMalzemeTanimla")
            {
                UrunForm frm = new UrunForm();
                frm.ShowDialog();
            }
            else if (btn.Name == "btnKisiTanimla")
            {
                KisiForm frm = new KisiForm();
                frm.ShowDialog();
            }
            else if (btn.Name == "btnKategoriTanimla")
            {
                KategoriForm frm = new KategoriForm();
                frm.ShowDialog();
            }
            else if (btn.Name == "btnHazirUrun")
            {
                HazirUrunForm frm = new HazirUrunForm();
                frm.ShowDialog();
            }
            else if (btn.Name == "btnStokGiris")
            {
                StokGirisForm frm = new StokGirisForm();
                frm.ShowDialog();
            }
            else if (btn.Name == "btnBorcTakip")
            {
                CariListeForm frm = new CariListeForm();
                frm.ShowDialog();
                // BorcTakipForm frm = new BorcTakipForm();
                // frm.ShowDialog();
            }
            else if (btn.Name == "btnRaporlar")
            {
                MessageBox.Show("Raporlar Modülü Hazırlanıyor...", "Bilgi");
                // RaporForm frm = new RaporForm();
                // frm.ShowDialog();
            }
        }

        private void AnaMenu_Load(object sender, EventArgs e)
        {

        }
    }
}