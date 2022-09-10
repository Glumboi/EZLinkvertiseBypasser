using System;
using System.Windows.Forms;
using Glumboi.UI;

namespace EZLinkvertiseBypasser.Ex_Forms
{
    public partial class LinkOpenerForm : Form
    {
        public LinkOpenerForm()
        {
            InitializeComponent();
        }

        private void LinkOpenerForm_Load(object sender, EventArgs e)
        {
            UIChanger.ChangeTitlebarToDark(Handle);
            Checkbox_Showagain.Checked = Properties.Settings.Default.UnsafeBypass;
        }

        private void Checkbox_Showagain_CheckedChanged(object sender, Bunifu.UI.WinForms.BunifuCheckBox.CheckedChangedEventArgs e)
        {
            if(Checkbox_Showagain.Checked)
            {
                Properties.Settings.Default.UnsafeBypass = true;
                return;
            }
            Properties.Settings.Default.UnsafeBypass = false;
        }

        private void LinkOpenerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Dispose();
        }
    }
}
