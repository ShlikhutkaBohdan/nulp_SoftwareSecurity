﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using lab3_rc5.model;

namespace lab3_rc5
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            MyRc5W64 crypter = new MyRc5W64("password");
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

        private bool _mIsProcessRun = false;

        private async void button1_Click(object sender, EventArgs e)
        {//crypt
            if (_mIsProcessRun)
            {
                MessageBox.Show("Process is run!");
                return;
            }
            _mIsProcessRun = true;
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
                await Task.Factory.StartNew(() =>
                {
                    try
                    {
                        MyRc5 crypter = MyRc5.GetRc5(_mKeyLength, password);
                        crypter.OnDecryptedPasswordFailed += CrypterOnOnDecryptedPasswordFailed;
                        crypter.OnProgressChanged += CrypterOnOnProgressChanged;
                        crypter.OnProcessEnded += CrypterOnOnProcessEnded;
                        crypter.Encrypt(plainFilePath, cryptedFilePath);
                        _mIsProcessRun = false;
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
                MessageBox.Show(ex.Message);
            }
        }

        private void CrypterOnOnProcessEnded()
        {
            //MessageBox.Show("Process ended");
        }

        private void CrypterOnOnProgressChanged(int persents)
        {
            progressBar1.Invoke((MethodInvoker) delegate
            {
                progressBar1.Value = persents;
            });
            //throw new NotImplementedException();
        }

        private void CrypterOnOnDecryptedPasswordFailed()
        {
            //throw new NotImplementedException();
            MessageBox.Show("Password is wrong. Please input another password and try again!");
        }

        private int _mKeyLength = 16;

        private async void button2_Click(object sender, EventArgs e)
        {//decrypt
            if (_mIsProcessRun)
            {
                MessageBox.Show("Process is run!");
                return;
            }
            _mIsProcessRun = true;
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

                await Task.Factory.StartNew(() =>
                {
                    try
                    {
                        MyRc5 crypter = MyRc5.GetRc5(_mKeyLength, password);
                        crypter.OnDecryptedPasswordFailed += CrypterOnOnDecryptedPasswordFailed;
                        crypter.OnProgressChanged += CrypterOnOnProgressChanged;
                        crypter.OnProcessEnded += CrypterOnOnProcessEnded;
                        crypter.Decrypt(cryptedFilePath, plainFilePath);
                        _mIsProcessRun = false;
                        MessageBox.Show("Decription complete");
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Some error with Decription");
                    }
                });
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
