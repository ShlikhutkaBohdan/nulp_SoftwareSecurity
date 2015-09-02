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
            var c = MyMd5.md5test();
            //String.Format("{0:X}", Dec)
            string[] s =
            {
                String.Format("{0:X}", c[0]),
                String.Format("{0:X}", c[1]),
                String.Format("{0:X}", c[2]),
                String.Format("{0:X}", c[3]),

            };
            label1.Text = s[0] + " " + s[1] + " " + s[2] + " " + s[3];

        }
    }
}
