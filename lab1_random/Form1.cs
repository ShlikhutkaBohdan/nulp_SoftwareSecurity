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
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int count = (int) numericUpDown1.Value;
            var list = GetSequence(count);
            long period = GetPeriod();

            textBox1.Text = GetSequenceText(list);
            MessageBox.Show("Period = " + period);

        }

        private List<long> GetSequence(int count)
        {
            List<long> list = new List<long>();
            MyRandom.srand();
            while (count-- != 0)
            {
                list.Add(MyRandom.random());
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
            MyRandom.srand();
            long first = MyRandom.random();
            long i = 0;
            do i++; while (MyRandom.random() != first);
            return first;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int count = (int)numericUpDown1.Value;
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
