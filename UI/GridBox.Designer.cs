namespace MinishMaker.UI
{
    partial class GridBox
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // GridBox
            // 
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.GridBox_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.GridBox_MouseDown);
            this.MouseLeave += new System.EventHandler(this.GridBox_MouseLeave);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GridBox_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.GridBox_MouseUp);
            this.Resize += new System.EventHandler(this.GridBox_Resize);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
    }
}
