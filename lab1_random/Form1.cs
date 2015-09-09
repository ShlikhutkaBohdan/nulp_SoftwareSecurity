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

namespace lab1_random
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //label2.Text = GetPeriod().ToString();
        }

        private MyRandom _myRandom;

        private void InputData()
        {
            long x0 = Convert.ToInt64(textBox5.Text);
            long m = Convert.ToInt64(textBox2.Text);
            long a = Convert.ToInt64(textBox3.Text);
            long c = Convert.ToInt64(textBox4.Text);
            _myRandom = new MyRandom(x0, m, a, c);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int count = 0;
            try
            {
                count = (int)numericUpDown1.Value;
                InputData();
                label2.Text = GetPeriod().ToString();
            }
            catch
            {
                MessageBox.Show("Error input data!");
            }
            
            var list = GetSequence(count);
            textBox1.Text = GetSequenceText(list);

        }

        private List<long> GetSequence(int count)
        {
            List<long> list = new List<long>();
            _myRandom.srand();
            while (count-- != 0)
            {
                list.Add(_myRandom.random());
            }
            return list;
        }

        private string GetSequenceText(List<long> list)
        {
            StringBuilder mytext = new StringBuilder();
            foreach (var item in list)
            {
                mytext.AppendLine(item + "");
            }
            return mytext.ToString();
        }

        private long GetPeriod()
        {
            _myRandom.srand();
            long first = _myRandom.random();
            long i = 0;
            do i++; while (_myRandom.random() != first);
            return i;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int count;
            try
            {
                count = (int)numericUpDown1.Value;
                InputData();
                label2.Text = GetPeriod().ToString();
            }
            catch
            {
                MessageBox.Show("Error input data!");
                return;
            }
             

            var list = GetSequence(count);
 
            Stream myStream;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string message = GetSequenceText(list);
                UnicodeEncoding uniEncoding = new UnicodeEncoding();
                if ((myStream = saveFileDialog1.OpenFile()) != null)
                {
                    myStream.Write(uniEncoding.GetBytes(message), 0, message.Length);
                    myStream.Close();
                }
                textBox1.Text = GetSequenceText(list);
                MessageBox.Show("Saved");
            }
        } 
    }
}
