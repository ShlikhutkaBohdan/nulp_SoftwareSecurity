using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab3_rc5
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //myRc5.
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

       

        private void button3_Click(object sender, EventArgs e)
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

        private void button4_Click(object sender, EventArgs e)
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

        private void button1_Click(object sender, EventArgs e)
        {//crypt
            string plainFilePath = textBox1.Text;//source
            string cryptedFilePath = textBox2.Text;//dest
            string password = textBox3.Text;
            try
            {
                if (plainFilePath.Equals(""))
                    throw new Exception("file path not inputed! Please input path");
                if (cryptedFilePath.Equals(""))
                    throw new Exception("Encrypted file path not inputed! Please output path");
                if (password.Equals(""))
                    throw new Exception("Password not inputed! Please input password");
                FileCrypter crypter = new FileCrypter(password);
                crypter.Crypt(plainFilePath, cryptedFilePath);
                MessageBox.Show("File encrypted");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {//decrypt
            string cryptedFilePath = textBox1.Text;//source
            string plainFilePath = textBox2.Text;//dest
            string password = textBox3.Text;
            try
            {
                if (plainFilePath.Equals(""))
                    throw new Exception("Encrypted file path not inputed! Please input path");
                if (cryptedFilePath.Equals(""))
                    throw new Exception("Decrypted file path not inputed! Please input path");
                if (password.Equals(""))
                    throw new Exception("Password not inputed! Please input password");
                FileCrypter crypter = new FileCrypter(password);
                crypter.Decrypt(cryptedFilePath, plainFilePath);
                MessageBox.Show("File decrypted");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
