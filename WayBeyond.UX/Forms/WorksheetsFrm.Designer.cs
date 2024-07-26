namespace WayBeyond.UX.Forms
{
    partial class WorksheetsFrm
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
            label1 = new System.Windows.Forms.Label();
            WrkShtLstBx = new System.Windows.Forms.ListBox();
            OkBtn = new System.Windows.Forms.Button();
            CancelBtn = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 9);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(139, 15);
            label1.TabIndex = 0;
            label1.Text = "Please select a worksheet";
            // 
            // WrkShtLstBx
            // 
            WrkShtLstBx.FormattingEnabled = true;
            WrkShtLstBx.ItemHeight = 15;
            WrkShtLstBx.Location = new System.Drawing.Point(13, 35);
            WrkShtLstBx.Name = "WrkShtLstBx";
            WrkShtLstBx.Size = new System.Drawing.Size(268, 109);
            WrkShtLstBx.TabIndex = 1;
            // 
            // OkBtn
            // 
            OkBtn.Location = new System.Drawing.Point(125, 146);
            OkBtn.Name = "OkBtn";
            OkBtn.Size = new System.Drawing.Size(75, 23);
            OkBtn.TabIndex = 2;
            OkBtn.Text = "OK";
            OkBtn.UseVisualStyleBackColor = true;
            OkBtn.Click += OkBtn_Click;
            // 
            // CancelBtn
            // 
            CancelBtn.Location = new System.Drawing.Point(206, 146);
            CancelBtn.Name = "CancelBtn";
            CancelBtn.Size = new System.Drawing.Size(75, 23);
            CancelBtn.TabIndex = 3;
            CancelBtn.Text = "Cancel";
            CancelBtn.UseVisualStyleBackColor = true;
            CancelBtn.Click += CancelBtn_Click;
            // 
            // WorksheetsFrm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(293, 176);
            Controls.Add(CancelBtn);
            Controls.Add(OkBtn);
            Controls.Add(WrkShtLstBx);
            Controls.Add(label1);
            Name = "WorksheetsFrm";
            Text = "Worksheet Selection";
            Load += WorksheetsFrm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox WrkShtLstBx;
        private System.Windows.Forms.Button OkBtn;
        private System.Windows.Forms.Button CancelBtn;
    }
}