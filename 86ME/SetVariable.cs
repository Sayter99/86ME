using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace _86ME_ver2
{
    public partial class SetVariable : Form
    {
        Dictionary<string, double> global_var;
        Dictionary<string, string> var_lang_dic;
        List<string> used_var;
        ListViewItem last;
        ArrayList ME_Motionlist;
        ArrayList used_element = new ArrayList();
        ArrayList tmp_element = new ArrayList();
        int var_num;

        public SetVariable(Dictionary<string, double> variables, Dictionary<string, string> lang_dic, ArrayList ME_List, List<string> used_var)
        {
            InitializeComponent();
            this.var_lang_dic = lang_dic;
            this.global_var = variables;
            this.var_num = 0;
            this.ME_Motionlist = ME_List;
            this.used_var = used_var;
            foreach (KeyValuePair<string, double> entry in variables)
            {
                var_num = var_num + 1;
                string[] row = { var_num.ToString(), entry.Key };
                var varItem = new ListViewItem(row);
                VarListView.Items.Add(varItem);
            }
            for (int i = 0; i < ME_Motionlist.Count; i++)
            {
                ME_Motion m = (ME_Motion)ME_Motionlist[i];
                for (int j = 0; j < m.Events.Count; j++)
                {
                    if (m.Events[j] is ME_If)
                    {
                        ME_If mif = (ME_If)m.Events[j];
                        if (mif.left_var < variables.Count && !used_element.Contains(mif))
                        {
                            used_element.Add(mif);
                            tmp_element.Add(mif.Copy());
                        }
                        if (mif.right_var < variables.Count && !used_element.Contains(mif))
                        {
                            used_element.Add(mif);
                            tmp_element.Add(mif.Copy());
                        }
                    }
                    else if (m.Events[j] is ME_Compute)
                    {
                        ME_Compute op = (ME_Compute)m.Events[j];
                        if (op.left_var < variables.Count && !used_element.Contains(op))
                        {
                            used_element.Add(op);
                            tmp_element.Add(op.Copy());
                        }
                        if (op.f1_var1 < variables.Count && !used_element.Contains(op))
                        {
                            used_element.Add(op);
                            tmp_element.Add(op.Copy());
                        }
                        if (op.f1_var2 < variables.Count && !used_element.Contains(op))
                        {
                            used_element.Add(op);
                            tmp_element.Add(op.Copy());
                        }
                        if (op.f2_var < variables.Count && !used_element.Contains(op))
                        {
                            used_element.Add(op);
                            tmp_element.Add(op.Copy());
                        }
                        if (op.f3_var < variables.Count && !used_element.Contains(op))
                        {
                            used_element.Add(op);
                            tmp_element.Add(op.Copy());
                        }
                    }
                }
            }
        }

        private void OKButton_Click(object sender, System.EventArgs e)
        {
            global_var.Clear();
            int i = 0;
            if (var_num > 0)
            {
                foreach (ListViewItem item in VarListView.Items)
                    global_var.Add(item.SubItems[1].Text, i++);
            }
            for (int j = 0; j < used_element.Count; j++)
            {
                if (used_element[j] is ME_If)
                {
                    ((ME_If)used_element[j]).left_var = ((ME_If)(tmp_element[j])).left_var;
                    ((ME_If)used_element[j]).right_var = ((ME_If)(tmp_element[j])).right_var;
                }
                else if (used_element[j] is ME_Compute)
                {
                    ((ME_Compute)used_element[j]).left_var = ((ME_Compute)(tmp_element[j])).left_var;
                    ((ME_Compute)used_element[j]).f1_var1 = ((ME_Compute)(tmp_element[j])).f1_var1;
                    ((ME_Compute)used_element[j]).f1_var2 = ((ME_Compute)(tmp_element[j])).f1_var2;
                    ((ME_Compute)used_element[j]).f2_var = ((ME_Compute)(tmp_element[j])).f2_var;
                    ((ME_Compute)used_element[j]).f3_var = ((ME_Compute)(tmp_element[j])).f3_var;
                }
            }
            this.DialogResult = DialogResult.OK;
        }

        private void CancelButton_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void AddButton_Click(object sender, System.EventArgs e)
        {
            if (checkName() == true)
            {
                var_num = var_num + 1;
                string[] row = { var_num.ToString(), "v_" + nameMaskedTextBox.Text };
                var varItem = new ListViewItem(row);
                VarListView.Items.Add(varItem);
                warningLabel.Text = "";
                varItem.Selected = true;
            }
        }

        private void ModifyButton_Click(object sender, System.EventArgs e)
        {
            if (VarListView.SelectedItems.Count == 0)
                return;
            ListViewItem cur = VarListView.SelectedItems[0];
            if (string.Compare(nameMaskedTextBox.Text, cur.SubItems[1].Text) == 0)
            {
                warningLabel.Text = "";
                return;
            }
            else if (checkName() == true)
            {
                warningLabel.Text = "";
                for (int i = 0; i < used_var.Count; i++)
                    if (string.Compare(cur.SubItems[1].Text, used_var[i]) == 0)
                        used_var[i] = "v_" + nameMaskedTextBox.Text;
                cur.SubItems[1].Text = "v_" + nameMaskedTextBox.Text;
            }
        }

        private void RemoveButton_Click(object sender, System.EventArgs e)
        {
            removeSelectedItem();
        }

        private void VarListView_KeyDown(object sender, KeyEventArgs e)
        {
            removeSelectedItem();
        }

        private void removeSelectedItem()
        {
            if (VarListView.SelectedItems.Count == 0)
                return;
            ListViewItem cur = VarListView.SelectedItems[0];
            for (int i = 0; i < used_var.Count; i++)
            {
                if (string.Compare(used_var[i], VarListView.SelectedItems[0].SubItems[1].Text) == 0)
                {
                    warningLabel.Text = var_lang_dic["errorMsg27"];
                    return;
                }
            }
            int index = VarListView.Items.IndexOf(cur);
            if (index != VarListView.Items.Count - 1)
            {
                for (int i = index + 1; i < var_num; i++)
                {
                    int origin_num = int.Parse(VarListView.Items[i].SubItems[0].Text);
                    VarListView.Items[i].SubItems[0].Text = (origin_num - 1).ToString();
                    for (int j = 0; j < tmp_element.Count; j++)
                    {
                        int prev_num = global_var.Count;
                        if (tmp_element[j] is ME_If)
                        {
                            ME_If mif = (ME_If)tmp_element[j];
                            if (mif.left_var == i && mif.left_var < prev_num)
                                mif.left_var = mif.left_var - 1;
                            if (mif.right_var == i && mif.right_var < prev_num)
                                mif.right_var = mif.right_var - 1;
                        }
                        else if (tmp_element[j] is ME_Compute)
                        {
                            ME_Compute op = (ME_Compute)tmp_element[j];
                            if (op.left_var == i && op.left_var < prev_num)
                                op.left_var = op.left_var - 1;
                            if (op.f1_var1 == i && op.f1_var1 < prev_num)
                                op.f1_var1 = op.f1_var1 - 1;
                            if (op.f1_var2 == i && op.f1_var2 < prev_num)
                                op.f1_var2 = op.f1_var2 - 1;
                            if (op.f2_var == i && op.f2_var < prev_num)
                                op.f2_var = op.f2_var - 1;
                            if (op.f3_var == i && op.f3_var < prev_num)
                                op.f3_var = op.f3_var - 1;
                        }
                    }
                }
            }
            VarListView.Items.Remove(cur);
            var_num = var_num - 1;
            nameMaskedTextBox.Text = "";
            warningLabel.Text = "";
        }

        private void VarListView_SelectedIndexChanged(object senderm, System.EventArgs e)
        {
            warningLabel.Text = "";
            if (VarListView.SelectedItems.Count != 0)
            {
                VarListView.SelectedItems[0].BackColor = Color.SkyBlue;
                nameMaskedTextBox.Text = VarListView.SelectedItems[0].SubItems[1].Text.Substring(2);
            }
            if (last == null)
                last = VarListView.SelectedItems[0];
            else if (VarListView.SelectedItems.Count != 0)
            {
                last.BackColor = Color.White;
                last = VarListView.SelectedItems[0];
            }
        }

        private bool checkName()
        {
            if (!(new System.Text.RegularExpressions.Regex("^[a-zA-Z0-9_]{1,20}$")).IsMatch(nameMaskedTextBox.Text))
            {
                if (nameMaskedTextBox.Text.Length < 20)
                    warningLabel.Text = var_lang_dic["errorMsg12"];
                else
                    warningLabel.Text = var_lang_dic["errorMsg13"];
                nameMaskedTextBox.Focus();
                nameMaskedTextBox.SelectAll();
            }
            else if (nameMaskedTextBox.Text.IndexOf(" ") == -1)
            {
                foreach (ListViewItem item in VarListView.Items)
                {
                    if (string.Compare(item.SubItems[1].Text, "v_" + nameMaskedTextBox.Text) == 0)
                    {
                        warningLabel.Text = var_lang_dic["errorMsg26"];
                        nameMaskedTextBox.Focus();
                        nameMaskedTextBox.SelectAll();
                        return false;
                    }
                }
                return true;
            }
            else
            {
                warningLabel.Text = var_lang_dic["errorMsg14"];
                nameMaskedTextBox.Focus();
                nameMaskedTextBox.SelectAll();
            }
            return false;
        }
    }
}
