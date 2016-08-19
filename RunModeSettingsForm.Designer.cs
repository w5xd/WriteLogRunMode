namespace WriteLogRunMode
{
    partial class RunModeSettingsForm
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
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.maxSecondsBetweenCALL = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.controlHpSplit = new System.Windows.Forms.CheckBox();
            this.checkBoxCallVox = new System.Windows.Forms.CheckBox();
            this.checkBoxStopWithFocus = new System.Windows.Forms.CheckBox();
            this.cbRedBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.maxSecondsBetweenCALL)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(242, 110);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(56, 23);
            this.buttonOK.TabIndex = 20;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(242, 139);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(56, 23);
            this.buttonCancel.TabIndex = 21;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // maxSecondsBetweenCALL
            // 
            this.maxSecondsBetweenCALL.Location = new System.Drawing.Point(13, 9);
            this.maxSecondsBetweenCALL.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.maxSecondsBetweenCALL.Name = "maxSecondsBetweenCALL";
            this.maxSecondsBetweenCALL.Size = new System.Drawing.Size(51, 20);
            this.maxSecondsBetweenCALL.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(71, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(143, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Max seconds between CALL";
            // 
            // controlHpSplit
            // 
            this.controlHpSplit.AutoSize = true;
            this.controlHpSplit.Location = new System.Drawing.Point(13, 68);
            this.controlHpSplit.Name = "controlHpSplit";
            this.controlHpSplit.Size = new System.Drawing.Size(145, 17);
            this.controlHpSplit.TabIndex = 3;
            this.controlHpSplit.Text = "&Dynamic headphone split";
            this.controlHpSplit.UseVisualStyleBackColor = true;
            // 
            // checkBoxCallVox
            // 
            this.checkBoxCallVox.AutoSize = true;
            this.checkBoxCallVox.Location = new System.Drawing.Point(13, 40);
            this.checkBoxCallVox.Name = "checkBoxCallVox";
            this.checkBoxCallVox.Size = new System.Drawing.Size(200, 17);
            this.checkBoxCallVox.TabIndex = 2;
            this.checkBoxCallVox.Text = "on first CALL letter, holdTransmit&VOX";
            this.checkBoxCallVox.UseVisualStyleBackColor = true;
            // 
            // checkBoxStopWithFocus
            // 
            this.checkBoxStopWithFocus.AutoSize = true;
            this.checkBoxStopWithFocus.Location = new System.Drawing.Point(13, 96);
            this.checkBoxStopWithFocus.Name = "checkBoxStopWithFocus";
            this.checkBoxStopWithFocus.Size = new System.Drawing.Size(178, 17);
            this.checkBoxStopWithFocus.TabIndex = 4;
            this.checkBoxStopWithFocus.Text = "&Stop aligns xmit&&keyboard focus";
            this.checkBoxStopWithFocus.UseVisualStyleBackColor = true;
            // 
            // cbRedBox
            // 
            this.cbRedBox.AutoSize = true;
            this.cbRedBox.Location = new System.Drawing.Point(13, 123);
            this.cbRedBox.Name = "cbRedBox";
            this.cbRedBox.Size = new System.Drawing.Size(103, 17);
            this.cbRedBox.TabIndex = 5;
            this.cbRedBox.Text = "Enable Red Box";
            this.cbRedBox.UseVisualStyleBackColor = true;
            // 
            // RunModeSettingsForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(308, 168);
            this.Controls.Add(this.cbRedBox);
            this.Controls.Add(this.checkBoxStopWithFocus);
            this.Controls.Add(this.checkBoxCallVox);
            this.Controls.Add(this.controlHpSplit);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.maxSecondsBetweenCALL);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RunModeSettingsForm";
            this.Text = "RunMode Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RunModeSettingsForm_FormClosing);
            this.Load += new System.EventHandler(this.RunModeSettingsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.maxSecondsBetweenCALL)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.NumericUpDown maxSecondsBetweenCALL;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox controlHpSplit;
        private System.Windows.Forms.CheckBox checkBoxCallVox;
        private System.Windows.Forms.CheckBox checkBoxStopWithFocus;
        private System.Windows.Forms.CheckBox cbRedBox;
    }
}