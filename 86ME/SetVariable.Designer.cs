namespace _86ME_ver2
{
    partial class SetVariable
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.VarListView = new System.Windows.Forms.ListView();
            this.NumColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.NameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.nameMaskedTextBox = new System.Windows.Forms.MaskedTextBox();
            this.nameLabel = new System.Windows.Forms.Label();
            this.OKButton = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.warningLabel = new System.Windows.Forms.Label();
            this.removeButton = new System.Windows.Forms.Button();
            this.modifyButton = new System.Windows.Forms.Button();
            this.addButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // VarListView
            // 
            this.VarListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.NumColumnHeader,
            this.NameColumnHeader});
            this.VarListView.FullRowSelect = true;
            this.VarListView.GridLines = true;
            this.VarListView.Location = new System.Drawing.Point(12, 68);
            this.VarListView.MultiSelect = false;
            this.VarListView.Name = "VarListView";
            this.VarListView.Size = new System.Drawing.Size(343, 218);
            this.VarListView.TabIndex = 0;
            this.VarListView.UseCompatibleStateImageBehavior = false;
            this.VarListView.View = System.Windows.Forms.View.Details;
            this.VarListView.SelectedIndexChanged += new System.EventHandler(this.VarListView_SelectedIndexChanged);
            this.VarListView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.VarListView_KeyDown);
            // 
            // NumColumnHeader
            // 
            this.NumColumnHeader.Text = "No";
            this.NumColumnHeader.Width = 82;
            // 
            // NameColumnHeader
            // 
            this.NameColumnHeader.Text = "Name";
            this.NameColumnHeader.Width = 213;
            // 
            // nameMaskedTextBox
            // 
            this.nameMaskedTextBox.Location = new System.Drawing.Point(71, 13);
            this.nameMaskedTextBox.Name = "nameMaskedTextBox";
            this.nameMaskedTextBox.Size = new System.Drawing.Size(176, 20);
            this.nameMaskedTextBox.TabIndex = 1;
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Location = new System.Drawing.Point(12, 16);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(59, 13);
            this.nameLabel.TabIndex = 2;
            this.nameLabel.Text = "Name:   v_";
            // 
            // OKButton
            // 
            this.OKButton.Location = new System.Drawing.Point(195, 295);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 23);
            this.OKButton.TabIndex = 5;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // Cancel
            // 
            this.Cancel.Location = new System.Drawing.Point(280, 295);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 6;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // warningLabel
            // 
            this.warningLabel.AutoSize = true;
            this.warningLabel.ForeColor = System.Drawing.Color.Red;
            this.warningLabel.Location = new System.Drawing.Point(12, 43);
            this.warningLabel.Name = "warningLabel";
            this.warningLabel.Size = new System.Drawing.Size(0, 13);
            this.warningLabel.TabIndex = 10;
            // 
            // removeButton
            // 
            this.removeButton.FlatAppearance.BorderSize = 0;
            this.removeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.removeButton.Image = global::_86ME_ver2.Properties.Resources.remove;
            this.removeButton.Location = new System.Drawing.Point(327, 8);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(28, 28);
            this.removeButton.TabIndex = 9;
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.RemoveButton_Click);
            // 
            // modifyButton
            // 
            this.modifyButton.FlatAppearance.BorderSize = 0;
            this.modifyButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.modifyButton.Image = global::_86ME_ver2.Properties.Resources.modify;
            this.modifyButton.Location = new System.Drawing.Point(293, 8);
            this.modifyButton.Name = "modifyButton";
            this.modifyButton.Size = new System.Drawing.Size(28, 28);
            this.modifyButton.TabIndex = 8;
            this.modifyButton.UseVisualStyleBackColor = true;
            this.modifyButton.Click += new System.EventHandler(this.ModifyButton_Click);
            // 
            // addButton
            // 
            this.addButton.FlatAppearance.BorderSize = 0;
            this.addButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addButton.Image = global::_86ME_ver2.Properties.Resources.add2;
            this.addButton.Location = new System.Drawing.Point(259, 8);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(28, 28);
            this.addButton.TabIndex = 7;
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // SetVariable
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(367, 330);
            this.Controls.Add(this.warningLabel);
            this.Controls.Add(this.removeButton);
            this.Controls.Add(this.modifyButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.nameLabel);
            this.Controls.Add(this.nameMaskedTextBox);
            this.Controls.Add(this.VarListView);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(383, 369);
            this.MinimumSize = new System.Drawing.Size(383, 369);
            this.Name = "SetVariable";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Variables";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView VarListView;
        private System.Windows.Forms.ColumnHeader NumColumnHeader;
        private System.Windows.Forms.ColumnHeader NameColumnHeader;
        private System.Windows.Forms.MaskedTextBox nameMaskedTextBox;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button modifyButton;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.Label warningLabel;
    }
}