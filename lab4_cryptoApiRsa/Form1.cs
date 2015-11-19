using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using lab3_rc5.model;

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
                            if (myRsaUnit.GenerateKeyPair(publicKeyFileName, privatKeyFileName))
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

        private async void button1_Click(object sender, EventArgs e)//encrypt file
        {
            if (_isProcessRun)
            {
                MessageBox.Show("Process is running");
                return;
            }
            string sourceFileName = textBox1.Text;
            string destFileName = textBox2.Text;
            string publicKeyName = textBox4.Text;
            try
            {
                if (sourceFileName.Equals(""))
                    throw new Exception("Виберіть файл який треба зашифрувати");
                if (destFileName.Equals(""))
                    throw new Exception("Виберіть файл шифрування");
                if (publicKeyName.Equals(""))
                    throw new Exception("Виберіть пубілчний ключ шифрування");
                await Task.Factory.StartNew(() =>
                {
                    _isProcessRun = true;
                    using (var myRsaUnit = new MyRsaUnit()) // for deletting all data
                    {
                        Stopwatch sw = new Stopwatch();//for encrryption time milis
                        sw.Start();
                        myRsaUnit.InputFilePath = sourceFileName;
                        myRsaUnit.OutputFileFilePath = destFileName;
                        myRsaUnit.OnProgresChange += MyRsaUnitOnOnProgresChange;
                        try
                        {
                            myRsaUnit.Encrypt(File.ReadAllBytes(publicKeyName));
                            label2.Invoke((MethodInvoker)delegate
                            {
                                label2.Text = String.Format("rsa time = {0}ms",
                                    sw.ElapsedMilliseconds);
                            });
                            MessageBox.Show("Файл успішно зашифровано!");
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Помилка шифрування");
                        }
                        sw.Stop();
                    }
                    _isProcessRun = false;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MyRsaUnitOnOnProgresChange(int persents)
        {
            if (persents%5 == 0)
            {
                //progress invoke
                progressBar1.Invoke((MethodInvoker) delegate
                {

                    progressBar1.Value = persents;
                });
            }
        }

        private bool _isProcessRun;

        private async void button2_Click(object sender, EventArgs e)
        {
            if (_isProcessRun)
            {
                MessageBox.Show("Process is running");
                return;
            }
            string sourceFileName = textBox1.Text;
            string destFileName = textBox2.Text;
            string privateKeyName = textBox5.Text;
            try
            {
                if (sourceFileName.Equals(""))
                    throw new Exception("Виберіть файл який треба розшифрувати");
                if (destFileName.Equals(""))
                    throw new Exception("Виберіть файл дешифрування");
                if (privateKeyName.Equals(""))
                    throw new Exception("Виберіть приватний ключ дешифрування");
                await Task.Factory.StartNew(() =>
                {
                    _isProcessRun = true;
                    using (var myRsaUnit = new MyRsaUnit()) // for deletting all data
                    {
                        myRsaUnit.InputFilePath = sourceFileName;
                        myRsaUnit.OutputFileFilePath = destFileName;
                        myRsaUnit.OnProgresChange += MyRsaUnitOnOnProgresChange;
                        Stopwatch sw = new Stopwatch();//for encrryption time milis
                        sw.Start();
                        try
                        {
                            myRsaUnit.Decrypt(File.ReadAllBytes(privateKeyName));
                            label2.Invoke((MethodInvoker)delegate
                            {
                                label2.Text = String.Format("rsa time = {0}ms (decryption)",
                                    sw.ElapsedMilliseconds);
                            });
                            MessageBox.Show("Файл успішно дешифровано!");
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Помилка дешифрування");
                        }
                        sw.Stop();

                    }
                    _isProcessRun = false;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void button11_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filename = openFileDialog1.FileName;
                textBox7.Text = filename;
                //var stream = openFileDialog1.OpenFile();
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            SaveFileDialog openFileDialog1 = new SaveFileDialog();
            openFileDialog1.Filter = "crypt files (*.crypt)|*.crypt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filename = openFileDialog1.FileName;
                textBox6.Text = filename;
                //var stream = openFileDialog1.OpenFile();
            }
        }

        private async void button9_Click(object sender, EventArgs e)
        {
            //crypt
            if (_isProcessRun)
            {
                MessageBox.Show("Process is run!");
                return;
            }
            string plainFilePath = textBox7.Text;//source
            string cryptedFilePath = textBox6.Text;//dest
            string password = textBox3.Text;
            try
            {
                if (plainFilePath.Equals(""))
                    throw new Exception("file path not inputed! Please input path");
                if (cryptedFilePath.Equals(""))
                    throw new Exception("Encrypted file path not inputed! Please output path");
                if (password.Equals(""))
                    throw new Exception("Password not inputed! Please input password");
                await Task.Factory.StartNew(() =>
                {
                    try
                    {
                        Stopwatch sw = new Stopwatch();
                        sw.Start();
                        _isProcessRun = true;
                        MyRc5 crypter = MyRc5.GetRc5(64, password, 16, 16);
                        crypter.OnDecryptedPasswordFailed += CrypterOnOnDecryptedPasswordFailed;
                        crypter.OnProgressChanged += CrypterOnOnProgressChanged;
                        crypter.OnProcessEnded += CrypterOnOnProcessEnded;
                        crypter.Encrypt(plainFilePath, cryptedFilePath);
                        _isProcessRun = false;
                        sw.Stop();
                        label1.Invoke((MethodInvoker)delegate
                        {
                            label1.Text = String.Format("rc5 time = {0}ms",
                                sw.ElapsedMilliseconds);
                        });
                        MessageBox.Show("Encryption complete");
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Some error with encryption");
                    }
                });
            }
            catch (Exception ex)
            {
                _isProcessRun = false;
                MessageBox.Show(ex.Message);
            }
        }

        private void CrypterOnOnDecryptedPasswordFailed()
        {
            //throw new NotImplementedException();
            MessageBox.Show("Some Error!");
        }

        private void CrypterOnOnProgressChanged(int persents)
        {
            if (persents%5 == 0)
            {
                progressBar1.Invoke((MethodInvoker) delegate
                {
                    progressBar1.Value = persents;
                });
            }
        }

        private void CrypterOnOnProcessEnded()
        {
            //MessageBox.Show("Proces ended");
        }

        private async void button8_Click(object sender, EventArgs e)
        {
            //decrypt
            if (_isProcessRun)
            {
                MessageBox.Show("Process is run!");
                return;
            }
            string cryptedFilePath = textBox7.Text;//source
            string plainFilePath = textBox6.Text;//dest
            string password = textBox3.Text;
            try
            {
                if (plainFilePath.Equals(""))
                    throw new Exception("Encrypted file path not inputed! Please input path");
                if (cryptedFilePath.Equals(""))
                    throw new Exception("Decrypted file path not inputed! Please input path");
                if (password.Equals(""))
                    throw new Exception("Password not inputed! Please input password");
                await Task.Factory.StartNew(() =>
                {
                    try
                    {
                        _isProcessRun = true;

                        Stopwatch sw = new Stopwatch();
                        sw.Start();

                        MyRc5 crypter = MyRc5.GetRc5(64, password, 16, 16);
                        crypter.OnDecryptedPasswordFailed += CrypterOnOnDecryptedPasswordFailed;
                        crypter.OnProgressChanged += CrypterOnOnProgressChanged;
                        crypter.OnProcessEnded += CrypterOnOnProcessEnded;
                        crypter.Decrypt(cryptedFilePath, plainFilePath);

                        sw.Stop();
                        label1.Invoke((MethodInvoker)delegate
                        {
                            label1.Text = String.Format("rc5 time = {0}ms (decryption)",
                                sw.ElapsedMilliseconds);
                        });

                        _isProcessRun = false;
                        MessageBox.Show("Decription complete");
                    }
                    catch (Exception)
                    {
                        _isProcessRun = false;
                        MessageBox.Show("Some error with Decription");
                    }
                });

            }
            catch (Exception ex)
            {
                _isProcessRun = false;
                MessageBox.Show(ex.Message);
            }
        }
    }
}
