using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Application1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int result = new BusinessObject1.Class1().DoSomethingBusinessPeopleCareAbout();

            MessageBox.Show("Please contact support for help with your problems. Mention promotion code " + result + " for a free gift.");
        }
    }
}