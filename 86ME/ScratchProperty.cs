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

namespace _86ME_ver2
{
    enum firmata_method { serial, bluetooth };

    public partial class ScratchProperty : Form
    {
        Dictionary<string, string> sp_lang_dic;
        public int method;
        public string bt_baud;
        public string bt_serial;
        public string port;
        public string path;

        public ScratchProperty(Dictionary<string, string> lang_dic)
        {
            InitializeComponent();
            this.btBaudComboBox.SelectedIndex = 0;
            this.btPortComboBox.SelectedIndex = 0;
            this.pathMaskedTextBox.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            this.bt_groupBox.Enabled = false;
            this.sp_lang_dic = lang_dic;
        }

        private void Serial_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            this.bt_groupBox.Enabled = false;
        }

        private void bt_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            this.bt_groupBox.Enabled = true;
        }

        private void numbercheck(object sender, KeyPressEventArgs e)
        {
            if (((int)e.KeyChar < 48 | (int)e.KeyChar > 57) & (int)e.KeyChar != 8)
                e.Handled = true;
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog path = new FolderBrowserDialog();
            path.Description = sp_lang_dic["GenerateScratch_Description"];
            var dialogResult = path.ShowDialog();
            if (!Directory.Exists(path.SelectedPath) || dialogResult != DialogResult.OK)
                return;

            this.pathMaskedTextBox.Text = path.SelectedPath;
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void OK_button_Click(object sender, EventArgs e)
        {
            if (Serial_radioButton.Checked)
                method = (int)firmata_method.serial;
            else if (bt_radioButton.Checked)
                method = (int)firmata_method.bluetooth;

            bt_baud = btBaudComboBox.Text;
            bt_serial = btPortComboBox.Text;
            int tryPort;
            if (int.TryParse(portTextBox.Text, out tryPort) == false)
            {
                MessageBox.Show("Invalid port number, please check again.", "Error");
                return;
            }
            else if (tryPort > 65535 || tryPort < 1)
            {
                MessageBox.Show("Invalid port number, please check again.", "Error");
                return;
            }
            else
            {
                port = tryPort.ToString();
            }

            if (!Directory.Exists(pathMaskedTextBox.Text))
            {
                MessageBox.Show("Invalid path, please check again.", "Error");
                return;
            }
            else
            {
                path = pathMaskedTextBox.Text;
            }

            this.DialogResult = DialogResult.OK;
        }
    }
}
