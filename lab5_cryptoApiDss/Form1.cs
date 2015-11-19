using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using lab5_cryptoApiDss.DsaApi;

namespace lab5_cryptoApiDss
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private string SaveFilePath(string title, string filter)
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

            KeyFactory keyFactory = new KeyFactory();
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

        private string OpenFilePath(string title,string filter)
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


        private KeyFactory LoadKeys()
        {
            string rsaPub = tbRsapub.Text;
            if (rsaPub.Equals(""))
                return null;
            string rsaPriv = tbRsapriv.Text;
            if (rsaPriv.Equals(""))
                return null;
            string dsaPub = tbDsapub.Text;
            if (dsaPub.Equals(""))
                return null;
            string dsaPriv = tbDsapriv.Text;
            if (dsaPriv.Equals(""))
                return null;

            KeyFactory keyFactory = new KeyFactory();
            keyFactory.RsaPublicKeyPath = rsaPub;
            keyFactory.RsaPrivateKeyPath = rsaPriv;
            keyFactory.DsaPublicKeyPath = dsaPub;
            keyFactory.DsaPrivateKeyPath = dsaPriv;
            if (!keyFactory.LoadKeysPairs())
                return null;

            return keyFactory;
        }

        private bool CreateDigitalSignatureForText(string text)
        {
            KeyFactory keyFactory = LoadKeys();
            if (keyFactory == null)
                return false;
            using (var ins = (new MemoryStream(Encoding.UTF8.GetBytes(text))))
            {
                string filePath = SaveFilePath("Збереження в файл", "");
                if (filePath == null)
                    return false;
                string signFilePath = SaveFilePath("Збереження підпису", "signature files (*.sign)|*.sign");
                if (signFilePath == null)
                    return false;

                DsaSignatureCreatorApi dsaSignatureCreatorApi = new DsaSignatureCreatorApi();
                dsaSignatureCreatorApi.Keys = keyFactory;
                dsaSignatureCreatorApi.SaveFilePath = filePath;
                dsaSignatureCreatorApi.SignatureFilePath = signFilePath;
                if (!dsaSignatureCreatorApi.CreateSignature(ins))
                    return false;
                
            }

            return true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string text = tbPlaintext.Text;
            if (CreateDigitalSignatureForText(text))
            {
                MessageBox.Show("Success");
            }
            else
            {
                MessageBox.Show("error");
            }
        }

        private bool CheckTextSignature()
        {
            string encryptedFilePath = OpenFilePath("Зашифрований файл", "");
            if (encryptedFilePath == null)
                return false;
            string signatureFilePath = OpenFilePath("Файл підпису", "signature files (*.sign)|*.sign");
            if (signatureFilePath == null)
                return false;
            string saveFilePath = SaveFilePath("Зберегти", "");
            if (saveFilePath == null)
                return false;

            KeyFactory keyFactory = LoadKeys();
            if (keyFactory == null)
                return false;

            using (var dsaSignatureChecker = new DsaSignatureCheckerApi())
            {
                dsaSignatureChecker.EncryptedFilePath = encryptedFilePath;
                dsaSignatureChecker.SignatureFilePath = signatureFilePath;
                dsaSignatureChecker.SaveFilePath = saveFilePath;
                dsaSignatureChecker.Keys = keyFactory;

                if (!dsaSignatureChecker.Process())
                    return false;

                textBox1.Text = dsaSignatureChecker.ResultOutput;
            }
            return true;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (!CheckTextSignature())
                MessageBox.Show("error");
        }
    }
}
