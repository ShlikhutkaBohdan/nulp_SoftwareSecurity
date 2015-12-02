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

namespace lab5_cryptoApiDss
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string signature = "";
            string pub = textBox1.Text;
            string pri = textBox2.Text;
            if (pub.Equals("") || pri.Equals(""))
            {
                MessageBox.Show("Виберіть шляхи до ключів");
                return;
            }
            
            if (radioButton1.Checked)
            {
                string text = textBox3.Text;
                using (var stream = GenerateStreamFromString(text))
                {
                    using (var myDssUnit = new DssUnit()) // for deletting all data after creating key pair
                    {
                        if (myDssUnit.OpenKeyPair(pub, pri))
                        {
                            //MessageBox.Show("Пара ключів успішно завантажені!");
                            string sign = myDssUnit.CreateSignatureForStream(stream);
                            if (sign != null)
                            {
                                signature = sign;
                            }
                        }
                    }
                    stream.Close();   
                }
            }
            else
            {
                string filePath = textBox4.Text;
                if (filePath.Equals(""))
                {
                    MessageBox.Show("Введіть назву файлу");
                    return;
                }
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (var myDssUnit = new DssUnit()) // for deletting all data after creating key pair
                    {
                        if (myDssUnit.OpenKeyPair(pub, pri))
                        {
                            string sign = myDssUnit.CreateSignatureForStream(fs);
                            if (sign != null)
                            {
                                signature = sign;
                            }
                            //MessageBox.Show("Пара ключів успішно завантажені!");
                        }
                    }
                    fs.Close();
                }
            }
            textBox5.Text = signature;

            

        }



        private void button1_Click(object sender, EventArgs e)
        {
            var saveDialog1 = new SaveFileDialog();
            saveDialog1.Filter = "private key (*.epr)|*.epr";
            saveDialog1.FilterIndex = 2;
            saveDialog1.RestoreDirectory = true;
            saveDialog1.Title = "Виберіть файл для збереження приватного ключа";
            string privatKeyFileName = "";
            if (saveDialog1.ShowDialog() == DialogResult.OK)
            {
                privatKeyFileName = saveDialog1.FileName;
                var saveDialog2 = new SaveFileDialog();
                saveDialog2.Filter = "public key (*.epb)|*.epb";
                saveDialog2.FilterIndex = 2;
                saveDialog2.RestoreDirectory = true;
                saveDialog2.Title = "Виберіть файл для збереження публічного ключа";
                string publicKeyFileName = "";
                if (saveDialog2.ShowDialog() == DialogResult.OK)
                {
                    publicKeyFileName = saveDialog2.FileName;
                    using (var myDssUnit = new DssUnit()) // for deletting all data after creating key pair
                    {
                        if (myDssUnit.GenerateKeyPair(publicKeyFileName, privatKeyFileName))
                        {
                            textBox1.Text = publicKeyFileName;
                            textBox2.Text = privatKeyFileName;
                            //_myRsaUnit
                            MessageBox.Show("Пара ключів успішно створені!");
                        }
                    }
                }
            }
        }

        public string CustomOpenDialog(string filter, string title)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = filter;
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.Title = title;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filename = openFileDialog1.FileName;
                return filename;
                //var stream = openFileDialog1.OpenFile();
            }
            return null;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string pub = CustomOpenDialog("public key (*.epb)|*.epb", "Публічний ключ");
            if (pub == null) return;
            string pri = CustomOpenDialog("public key (*.epr)|*.epr", "Приватний ключ");
            if (pri == null) return;
            textBox1.Text = pub;
            textBox2.Text = pri;
            
        }

        private string CustomSaveDialog(string filter, string title)
        {
            var saveDialog2 = new SaveFileDialog();
            saveDialog2.Filter = filter;
            saveDialog2.FilterIndex = 2;
            saveDialog2.Title = title;
            saveDialog2.RestoreDirectory = true;
            string publicKeyFileName = "";
            if (saveDialog2.ShowDialog() == DialogResult.OK)
            {
                return saveDialog2.FileName;
                
            }
            return null;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string signature = textBox5.Text;
            File.WriteAllText(CustomSaveDialog("txt (*.txt)|*.txt", "Збереження підпису"), signature);
            MessageBox.Show("Файл підпису збережений");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            bool isEqual = false;

            string signature = textBox5.Text;
            string pub = textBox1.Text;
            string pri = textBox2.Text;
            if (pub.Equals("") || pri.Equals(""))
            {
                MessageBox.Show("Виберіть шляхи до ключів");
                return;
            }

            if (radioButton1.Checked)
            {
                string text = textBox3.Text;
                using (var stream = GenerateStreamFromString(text))
                {
                    using (var myDssUnit = new DssUnit()) // for deletting all data after creating key pair
                    {
                        if (myDssUnit.OpenKeyPair(pub, pri))
                        {
                            if (myDssUnit.OpenKeyPair(pub, pri))
                            {
                                isEqual = myDssUnit.VerifySignature(stream, signature);
                            }
                        }
                    }
                    stream.Close();
                }
            }
            else
            {
                string filePath = textBox4.Text;
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (var myDssUnit = new DssUnit()) // for deletting all data after creating key pair
                    {
                        if (myDssUnit.OpenKeyPair(pub, pri))
                        {
                            isEqual = myDssUnit.VerifySignature(fs, signature);
                        }
                    }
                    fs.Close();
                }
            }
            if (isEqual)
                MessageBox.Show("Підписи співпадають");
            else
                MessageBox.Show("Підписи не співпадають");
                //textBox5.Text = signature;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string file = CustomOpenDialog("any files (*.*)|*.*", "Виберіть файл для підписування");
            if (file == null) return;
            textBox4.Text = file;
        }
    }
}