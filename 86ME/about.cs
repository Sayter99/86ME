using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace _86ME_ver1
{
    public partial class about : Form
    {
        public System.Diagnostics.Process p = new System.Diagnostics.Process();

        public about()
        {
            InitializeComponent();
        }

        private void richTextBox1_LinkClicked(object sender,
        System.Windows.Forms.LinkClickedEventArgs e)
        {
            // Call Process.Start method to open a browser
            // with link text as URL.
            p = System.Diagnostics.Process.Start(e.LinkText);
        }
    }
}
