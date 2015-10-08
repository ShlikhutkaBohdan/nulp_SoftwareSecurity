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
            MyRC5 myRc5 = new MyRC5(8, MyRC5.KeyLength.KEY_LENGTH_16, 16);
            //myRc5.
        }
    }
}
