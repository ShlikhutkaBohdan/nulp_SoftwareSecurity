using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab5_cryptoApiDss
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string SaveFilePath(string title, string filter)
        {
            var saveDialog2 = new SaveFileDialog();
            saveDialog2.Filter = filter;//"public key (*.epb)|*.epb";
            saveDialog2.FilterIndex = 2;
            saveDialog2.RestoreDirectory = true;
            saveDialog2.Title = title;//"Виберіть файл для збереження публічного ключа";
            if (saveDialog2.ShowDialog() == DialogResult.OK)
            {
                return saveDialog2.FileName;
            }
            return null;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string rsaPub = SaveFilePath("Публічний ключ RSA", "rsapub files (*.rsapub)|*.rsapub");//tbRsapub.Text;
            if (rsaPub == null) 
                return;
            string rsaPriv = SaveFilePath("Приватний ключ RSA", "rsapriv files (*.rsapriv)|*.rsapriv");//tbRsapriv.Text;
            if (rsaPriv == null) 
                return;
            string dsaPub = SaveFilePath("Публічний ключ DSA", "dsapub files (*.dsapub)|*.dsapub");//tbDsapub.Text;
            if (dsaPub == null) 
                return;
            string dsaPriv = SaveFilePath("Приватний ключ DSA", "dsapriv files (*.dsapriv)|*.dsapriv");//tbDsapriv.Text;
            if (dsaPriv == null) 
                return;

            tbRsapub.Text = rsaPub;
            tbRsapriv.Text = rsaPriv;
            tbDsapub.Text = dsaPub;
            tbDsapriv.Text = dsaPriv;

            DsaApi.KeyFactory keyFactory = new DsaApi.KeyFactory();
            keyFactory.RsaPublicKeyPath = rsaPub;
            keyFactory.RsaPrivateKeyPath = rsaPriv;
            keyFactory.DsaPublicKeyPath = dsaPub;
            keyFactory.DsaPrivateKeyPath = dsaPriv;
            if (keyFactory.GenerateKeysPairs())
            {
                MessageBox.Show("Success!");
            }
            else
            {
                MessageBox.Show("Error generating keays");
                return;
            }
        }

        string OpenFilePath(string title,string filter)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = filter;
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.Title = title;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filename = openFileDialog1.FileName;
                textBox1.Text = filename;
                return filename;
            }
            return null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string s = OpenFilePath("Приватний ключ RSA", "rsapriv files (*.rsapriv)|*.rsapriv");
            if (s != null)
                tbRsapriv.Text = s;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string s = OpenFilePath("Публічний ключ RSA", "rsapub files (*.rsapub)|*.rsapub");
            if (s != null)
                tbRsapub.Text = s;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string s = OpenFilePath("Приватний ключ DSA","dsapriv files (*.dsapriv)|*.dsapriv");
            if (s != null)
                tbDsapriv.Text = s;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string s = OpenFilePath("Публічний ключ DSA", "dsapub files (*.dsapub)|*.dsapub");
            if (s != null)
                tbDsapub.Text = s;
        }
    }
}
