using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab4_cryptoApiRsa
{
    public partial class Form1 : Form
    {
        //private MyRsaUnit _myRsaUnit;
        public Form1()
        {
            InitializeComponent();
            //_myRsaUnit = new MyRsaUnit();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)//generate keys pair
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
                        using (var myRsaUnit = new MyRsaUnit())// for deletting all data after creating key pair
                        {
                            if (myRsaUnit.SaveKeyPair(publicKeyFileName, privatKeyFileName))
                            {
                                //_myRsaUnit
                                MessageBox.Show("Пара ключів успішно створені!");
                            }
                        }
                    }
                }
        }


        private void button3_Click(object sender, EventArgs e)//source file
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filename = openFileDialog1.FileName;
                textBox1.Text = filename;
                //var stream = openFileDialog1.OpenFile();
            }
        }

        private void button4_Click(object sender, EventArgs e)//dest file
        {
            SaveFileDialog openFileDialog1 = new SaveFileDialog();
            openFileDialog1.Filter = "crypt files (*.crypt)|*.crypt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filename = openFileDialog1.FileName;
                textBox2.Text = filename;
                //var stream = openFileDialog1.OpenFile();
            }
        }

        private void button6_Click(object sender, EventArgs e)//get public key
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "public key (*.epb)|*.epb";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filename = openFileDialog1.FileName;
                textBox4.Text = filename;
                //var stream = openFileDialog1.OpenFile();
            }
        }

        private void button5_Click(object sender, EventArgs e)//get private key
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "private key (*.epr)|*.epr";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filename = openFileDialog1.FileName;
                textBox5.Text = filename;
                //var stream = openFileDialog1.OpenFile();
            }
        }

        private void button1_Click(object sender, EventArgs e)//encrypt file
        {
            string sourceFileName = textBox1.Text;
            string destFileName = textBox2.Text;
            string publicKeyName = textBox4.Text;
            string privateKeyName = textBox5.Text;
            try
            {
                if (sourceFileName.Equals(""))
                    throw new Exception("Виберіть файл який треба зашифрувати");
                if (destFileName.Equals(""))
                    throw new Exception("Виберіть файл шифрування");
                if (publicKeyName.Equals(""))
                    throw new Exception("Виберіть пубілчний ключ шифрування");
                using (var myRsaUnit = new MyRsaUnit()) // for deletting all data
                {

                    if (myRsaUnit.RsaCrypt(sourceFileName, destFileName, publicKeyName, privateKeyName))
                            MessageBox.Show("Файл успішно зашифровано!");
                        else
                            throw new Exception("Помилка шифрування");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string sourceFileName = textBox1.Text;
            string destFileName = textBox2.Text;
            string publicKeyName = textBox4.Text;
            string privateKeyName = textBox5.Text;
            try
            {
                if (sourceFileName.Equals(""))
                    throw new Exception("Виберіть файл який треба розшифрувати");
                if (destFileName.Equals(""))
                    throw new Exception("Виберіть файл дегифрування");
                if (privateKeyName.Equals(""))
                    throw new Exception("Виберіть приватний ключ дешифрування");
                using (var myRsaUnit = new MyRsaUnit()) // for deletting all data
                {
                    if (myRsaUnit.RsaDecrypt(sourceFileName, destFileName, publicKeyName, privateKeyName))
                            MessageBox.Show("Файл успішно дешифровано!");
                        else
                            throw new Exception("Помилка дешифрування");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
