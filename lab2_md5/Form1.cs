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
            MyMd5.md5test();
        }
    }
}
