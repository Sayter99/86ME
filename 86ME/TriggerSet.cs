using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace _86ME_ver1._0
{
    public partial class TriggerSet : Form
    {
        public TriggerSet(String inkey)
        {
            InitializeComponent();
            Keyvalue.Text = inkey;
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (String.Compare(textBox1.Text, "") != 0 && String.Compare(textBox2.Text, "") != 0)
                Keyvalue.Text = textBox1.Text + "+" + textBox2.Text;
            else if (String.Compare(textBox1.Text, "") != 0)
                Keyvalue.Text = textBox1.Text;
            else if (String.Compare(textBox1.Text, "") == 0)
                Keyvalue.Text = textBox2.Text;
            else
                Keyvalue.Text = "";

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (String.Compare(textBox1.Text, "") != 0 && String.Compare(textBox2.Text, "") != 0)
                Keyvalue.Text = textBox1.Text + "+" + textBox2.Text;
            else if (String.Compare(textBox2.Text, "") != 0)
                Keyvalue.Text = textBox2.Text;
            else if (String.Compare(textBox2.Text, "") == 0)
                Keyvalue.Text = textBox1.Text;
            else
                Keyvalue.Text = "";

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }


        private void button1_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Modifiers == Keys.Control)
                textBox1.Text = "ctrl";
            else if (e.Modifiers == Keys.Alt)
                textBox1.Text = "alt";
            else if (e.Modifiers == Keys.Shift)
                textBox1.Text = "shift";
            else
                textBox1.Text = "";
            if (e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z)
                textBox2.Text = ((char)e.KeyCode).ToString();
            else if (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
                textBox2.Text = ((char)e.KeyCode).ToString();
            else if (e.KeyCode >= Keys.F1 && e.KeyCode <= Keys.F12)
                textBox2.Text = "F" + ((int)(e.KeyCode - Keys.F1 + 1)).ToString();
            else if (e.KeyCode == Keys.Up)
                textBox2.Text = "Up";
            else if (e.KeyCode == Keys.Down)
                textBox2.Text = "Down";
            else if (e.KeyCode == Keys.Left)
                textBox2.Text = "Left";
            else if (e.KeyCode == Keys.Right)
                textBox2.Text = "Right";
            else
                textBox2.Text = "";

        }

        private void button2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control)
                textBox1.Text = "ctrl";
            else if (e.Modifiers == Keys.Alt)
                textBox1.Text = "alt";
            else if (e.Modifiers == Keys.Shift)
                textBox1.Text = "shift";
            else
                textBox1.Text = "";
            if (e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z)
                textBox2.Text = ((char)e.KeyCode).ToString();
            else if (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
                textBox2.Text = ((char)e.KeyCode).ToString();
            else if (e.KeyCode >= Keys.F1 && e.KeyCode <= Keys.F12)
                textBox2.Text = "F" + ((int)(e.KeyCode - Keys.F1 + 1)).ToString();
            else if (e.KeyCode == Keys.Up)
                textBox2.Text = "Up";
            else if (e.KeyCode == Keys.Down)
                textBox2.Text = "Down";
            else if (e.KeyCode == Keys.Left)
                textBox2.Text = "Left";
            else if (e.KeyCode == Keys.Right)
                textBox2.Text = "Right";
            else
                textBox2.Text = "";
        }


        private void button1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            e.IsInputKey = true;
        }

        private void button2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            e.IsInputKey = true;
        }

    }
}
