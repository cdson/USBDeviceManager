using System;
using System.Windows.Forms;

namespace DeviceTrackerSupport
{
    /// <summary>
    /// Hidden Form which we use to receive Windows messages about flash drives
    /// </summary>
    internal class DetectorForm : Form
    {
        private Label label1;
        private DriveDetector mDetector = null;

        /// <summary>
        /// Set up the hidden form. 
        /// </summary>
        /// <param name="detector">DriveDetector object which will receive notification about USB drives, see WndProc</param>
        public DetectorForm(DriveDetector detector)
        {
            mDetector = detector;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.ShowInTaskbar = false;
            this.ShowIcon = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Load += new System.EventHandler(this.Load_Form);
            this.Activated += new EventHandler(this.Form_Activated);
        }

        private void Load_Form(object sender, EventArgs e)
        {
            // We don't really need this, just to display the label in designer ...
            InitializeComponent();

            // Create really small form, invisible anyway.
            this.Size = new System.Drawing.Size(5, 5);
        }

        private void Form_Activated(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        /// <summary>
        /// This function receives all the windows messages for this window (form).
        /// We call the DriveDetector from here so that is can pick up the messages about
        /// drives arrived and removed.
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (mDetector != null)
            {
                mDetector.WndProc(ref m);
            }
        }

        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(314, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "This is invisible form. To see DriveDetector code click View Code";
            // 
            // DetectorForm
            // 
            this.ClientSize = new System.Drawing.Size(360, 80);
            this.Controls.Add(this.label1);
            this.Name = "DetectorForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}