using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Barkod
{
    public partial class AnaMenu: Form
    {
        public AnaMenu()
        {
            InitializeComponent();
        }

        // SATIŞ YAP Butonu
        private void btnSatisYap_Click(object sender, EventArgs e)
        {
            // Mevcut Satış formunu aç
            Form1 satisEkrani = new Form1();
            satisEkrani.ShowDialog();
        }

        // Henüz yapmadığımız diğer sayfalar için geçici mesaj
        private void btnDigerleri_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            //MessageBox.Show(btn.Text.Replace("\n", " ") + " modülü hazırlanıyor...", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // İleride burayı şöyle dolduracağız:
            // if (btn.Name == "btnMalzemeTanimla") { new UrunForm().ShowDialog(); }
            if(btn.Name == "btnHazirUrun") { btnHazirUrun_Click(sender, e); }

            // if (btn.Name == "btnKisiTanimla") { new KisiForm().ShowDialog(); }
        }

        private void btnHazirUrun_Click(object sender, EventArgs e) // Eğer özel event açtıysan
        {
            HazirUrunForm form = new HazirUrunForm();
            form.ShowDialog();
        }

    }
}
