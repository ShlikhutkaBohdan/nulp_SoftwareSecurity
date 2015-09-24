using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab2_md5
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
        }

        private bool isStartedTask = false;

        private int _mPersentage = 0;

        private void Md5OnOnProgressChanged(ushort persentage)
        {
            if (_mPersentage != persentage)
            {
                progressBar1.Invoke((MethodInvoker)delegate
                {
                    _mPersentage = persentage;
                    progressBar1.Value = _mPersentage;
                });
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            string filename;
            if (!isStartedTask)
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                //openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    filename = openFileDialog1.FileName;
                    label7.Text = openFileDialog1.SafeFileName;
                    var stream = openFileDialog1.OpenFile();
                    label6.Text = stream.Length.ToString() + " KB";
                    stream.Close();
                    textBox3.Text = "";
                }
                else return;
                isStartedTask = true;
            }
            else
            {
                MessageBox.Show("Task is started");
                return;
            }
            await Task.Factory.StartNew(() =>
            {
                MyMD5 md5 = new MyMD5();
                md5.OnProgressChanged += Md5OnOnProgressChanged;
                string md5Value = md5.GetMd5FromFile(filename);
                textBox3.Invoke((MethodInvoker)delegate
                {
                    textBox3.Text = md5Value;
                    if (!textBox2.Text.Equals(""))
                    {
                        if (textBox2.Text.Equals(md5Value))
                            MessageBox.Show("Correct checksum!");
                        else MessageBox.Show("Incorrect checksum");
                    }
                });
                progressBar1.Invoke((MethodInvoker)delegate
                {
                    _mPersentage = 100;
                    progressBar1.Value = _mPersentage;
                });
                isStartedTask = false;
            });
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            MyMD5 md5 = new MyMD5();
            string md5Value = md5.GetMd5FromString(textBox1.Text);
            textBox4.Text = md5Value;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            MyMD5 md5 = new MyMD5();
            string md5Value = md5.GetMd5FromString(textBox1.Text);
            textBox4.Text = md5Value;
        }
    }
}
