using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Management;

namespace _86ME_ver1
{
    public partial class SetForm : Form
    {
        string[] serialPorts;

        public SetForm()
        {
            InitializeComponent();
            serialPorts = SerialPort.GetPortNames();
            ComboBox0.SelectedIndex = 0;
            foreach (string serialPort in serialPorts)
                this.ComboBox0.Items.Add(serialPort);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
            if (string.Compare(ComboBox0.Text, "--auto--") == 0)
            {
                if (!have_86())
                    this.com_port = "AUTO";
            }
            else if (string.Compare(ComboBox0.Text, "--offline--") == 0)
                this.com_port = "OFF";
            else
                this.com_port = ComboBox0.Text;
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
        }

        public bool have_86()
        {
            ManagementScope scope = new ManagementScope("\\\\.\\root\\cimv2");
            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_PnPEntity");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
            ManagementObjectCollection queryCollection = searcher.Get();
            foreach (ManagementObject Obj in queryCollection)
            {
                if (Obj["Name"] != null && Obj["Name"].ToString().Contains("86Duino"))
                {
                    foreach (string serialPort in serialPorts)
                        if (Obj["Name"].ToString().Contains(serialPort))
                            com_port = serialPort;
                    return true;
                }
            }
            this.com_port = "OFF";
            MessageBox.Show("Cannot find 86Duino, entering offline mode", "",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
    }
}
