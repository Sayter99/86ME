using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace _86ME_ver1
{
    public partial class SetForm : Form
    {
        public SetForm()
        {
            InitializeComponent();
            string[] serialPorts = SerialPort.GetPortNames();
            ComboBox0.SelectedIndex = 0;
            foreach (string serialPort in serialPorts)
                this.ComboBox0.Items.Add(serialPort);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
            if (string.Compare(ComboBox0.Text, "--auto--") == 0)
                this.com_port = "AUTO";
            else if (string.Compare(ComboBox0.Text, "--write yourself--") == 0)
                this.com_port = "AUTO";
            else if (string.Compare(ComboBox0.Text, "--offline--") == 0)
                this.com_port = "OFF";
            else
                this.com_port = ComboBox0.Text;
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
        }
    }
}
