namespace Aok_Patch.patcher_
{
    partial class BinaViewer
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
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.btn_BinaSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.Location = new System.Drawing.Point(30, 29);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(563, 402);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // btn_BinaSave
            // 
            this.btn_BinaSave.Image = global::Aok_Patch.patcher_.MainForm.save;
            this.btn_BinaSave.Location = new System.Drawing.Point(441, 466);
            this.btn_BinaSave.Name = "btn_BinaSave";
            this.btn_BinaSave.Size = new System.Drawing.Size(126, 67);
            this.btn_BinaSave.TabIndex = 1;
            this.btn_BinaSave.UseVisualStyleBackColor = true;
            this.btn_BinaSave.Click += new System.EventHandler(this.btn_BinaSave_Click);
            // 
            // BinaViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(621, 545);
            this.Controls.Add(this.btn_BinaSave);
            this.Controls.Add(this.richTextBox1);
            this.MinimumSize = new System.Drawing.Size(500, 450);
            this.Name = "BinaViewer";
            this.Text = "BinaViewer";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button btn_BinaSave;
    }
}