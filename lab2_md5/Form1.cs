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

        private void button1_Click(object sender, EventArgs e)
        {
            MyMD5 md5 = new MyMD5();
            label1.Text = md5.GetMd5FromString(textBox1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MyMD5 md5 = new MyMD5();
            label1.Text = md5.GetMd5FromFile(@"e:\lab1_random.exe");
        }
    }
}
