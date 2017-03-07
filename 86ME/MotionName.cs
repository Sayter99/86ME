using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace _86ME_ver2
{
    public partial class MotionName : Form
    {
        public string name = "";
        Dictionary<string, string> MotionName_lang_dic;
        ComboBox motions = new ComboBox();

        public MotionName(Dictionary<string, string> lang_dic, string default_name, string title, ComboBox motionCombo)
        {
            InitializeComponent();
            this.Text = title;
            motions = motionCombo;
            name = default_name;
            motionNameText.Text = default_name;
            MotionName_lang_dic = lang_dic;
            motionNameText.SelectAll();
        }

        private void motionNameText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OKButton_Click(sender, e);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            if (!(new System.Text.RegularExpressions.Regex("^[a-zA-Z][a-zA-Z0-9_]{0,20}$")).IsMatch(motionNameText.Text))
            {
                if (motionNameText.Text.Length < 20)
                    warningLabel.Text = MotionName_lang_dic["errorMsg12"];
                else
                    warningLabel.Text = MotionName_lang_dic["errorMsg13"];
                motionNameText.Focus();
                motionNameText.SelectAll();
            }
            else if (motionNameText.Text.IndexOf(" ") == -1) // add new motion successfully
            {
                name = motionNameText.Text;
                for (int i = 0; i < motions.Items.Count; i++)
                {
                    if (motions.Items[i].ToString() == name)
                    {
                        warningLabel.Text = MotionName_lang_dic["errorMsg22"];
                        motionNameText.Focus();
                        motionNameText.SelectAll();
                        return;
                    }
                }
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                warningLabel.Text = MotionName_lang_dic["errorMsg14"];
                motionNameText.Focus();
                motionNameText.SelectAll();
            }
        }
    }
}
