namespace FourZug.Frontend
{
    partial class UIForm
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
            gbxGameBoard = new GroupBox();
            txtGameResult = new TextBox();
            SuspendLayout();
            // 
            // gbxGameBoard
            // 
            gbxGameBoard.BackColor = SystemColors.GradientInactiveCaption;
            gbxGameBoard.Location = new Point(12, 21);
            gbxGameBoard.Name = "gbxGameBoard";
            gbxGameBoard.Size = new Size(400, 336);
            gbxGameBoard.TabIndex = 0;
            gbxGameBoard.TabStop = false;
            // 
            // txtGameResult
            // 
            txtGameResult.Location = new Point(140, 373);
            txtGameResult.Name = "txtGameResult";
            txtGameResult.ReadOnly = true;
            txtGameResult.Size = new Size(100, 23);
            txtGameResult.TabIndex = 1;
            txtGameResult.TabStop = false;
            // 
            // UIForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlDark;
            ClientSize = new Size(430, 423);
            Controls.Add(txtGameResult);
            Controls.Add(gbxGameBoard);
            Name = "UIForm";
            Text = "UIForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private GroupBox gbxGameBoard;
        private TextBox txtGameResult;
    }
}