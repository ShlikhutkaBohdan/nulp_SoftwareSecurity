using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace lab1_random
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //label2.Text = GetPeriod().ToString();
            _mTimer.Interval = 1;
            _mTimer.Elapsed += delegate(object o, ElapsedEventArgs args)
            {
                progressBar1.Invoke((MethodInvoker)delegate
                {
                    progressBar1.Value = _procent;
                });


            };
        }

        private MyRandom _myRandom;

        private void InputData()
        {
            /*numericUpDown1.Value =20;
            textBox5.Text = "2";
            textBox2.Text = "1023";
            textBox3.Text = "32";
            textBox4.Text = "0";*/
            ulong x0 = Convert.ToUInt64(textBox5.Text);
            ulong m = Convert.ToUInt64(textBox2.Text);
            ulong a = Convert.ToUInt64(textBox3.Text);
            ulong c = Convert.ToUInt64(textBox4.Text);
            _myRandom = new MyRandom(x0, m, a, c);
        }

        private int _procent;

        private void button1_Click(object sender, EventArgs e)
        {
            ulong count;
            try
            {
                InputData();
                count = (ulong)numericUpDown1.Value;
                
            }
            catch
            {
                MessageBox.Show("Error input data!");
                return;
            }


            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Gen(count, saveFileDialog1.FileName);
            }

        }

        private Timer _mTimer = new Timer();
        private bool _isStarted = false;

        private async void Gen(ulong count, string filename)
        {
            if (_isStarted)
            {
                MessageBox.Show("Генератор вже запущений");
                return;
            }
            listBox1.Items.Clear();
            label2.Text = "Період не визначений";
            StringBuilder sb = new StringBuilder();
            ulong oldCount = count;

            _mTimer.Start();
            _isStarted = true;
            label8.Text = "Статус : Заповнення файлу псевдовипадковими числами";
            await Task.Factory.StartNew(() =>
            {
                var fs = File.Create(filename);
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    _myRandom.srand();
                    ulong first = _myRandom.random();
                    int i = 0;
                    _myRandom.srand();
                    do
                    {
                        
                        ulong value = _myRandom.random();
                        sw.WriteLine(value);
                        if (sb.Length < 2000)
                        {
                            sb.Append(value);
                            sb.AppendLine();
                        }

                        _procent = (int) (((oldCount - count)/(double) oldCount)*100);

                        if (first == value && i != 0)
                        {
                            label2.Invoke((MethodInvoker)delegate
                            {
                                label2.Text = i.ToString();
                                MessageBox.Show("Період : "+i);
                            });
                            first = ulong.MaxValue;
                        };
                        i++;
                    } while (--count != 0);
                    sw.Close();
                }
                fs.Close();
                progressBar1.Invoke((MethodInvoker) delegate
                        {
                            progressBar1.Value = (int) (((oldCount-count)/(double) oldCount)*100);
                        });
                /*textBox1.Invoke((MethodInvoker)delegate
                {
                    textBox1.Text = sb.ToString();
                });*/
                label8.Invoke((MethodInvoker) delegate
                {
                    label8.Text = "Статус : Вивід псевдовипадкових чисел";
                });
                fs = File.Open(filename, FileMode.Open);
                using (StreamReader sr = new StreamReader(fs))
                {
                    string l;
                    while (!sr.EndOfStream)
                    {
                        l = sr.ReadLine();
                        listBox1.Invoke((MethodInvoker) delegate
                        {
                            listBox1.Items.Add(l);
                        });

                    }
                    sr.Close();
                }
                fs.Close();
                
                MessageBox.Show("Saved");
                label8.Invoke((MethodInvoker)delegate
                {
                    label8.Text = "Статус : Фініш";
                });
                _mTimer.Stop();
                _isStarted = false;
            });
        }


        private ulong GetPeriod()
        {
            _myRandom.srand();
            ulong first = _myRandom.random();
            ulong i = 0;
            do ++i; while (_myRandom.random() != first);
            return i;
        }

  
    }
}
