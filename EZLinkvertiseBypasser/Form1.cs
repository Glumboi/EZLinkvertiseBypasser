using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using EZLinkvertiseBypasser.Core;
using EZLinkvertiseBypasser.Ex_Forms;
using Glumboi.UI;
using Telerik.WinControls.UI;

namespace EZLinkvertiseBypasser
{
    public partial class Form1 : Form
    {
        private LinkOpenerForm warning;
        private readonly VersionControl version = new VersionControl();
        private bool warningIsActive;
        private string lastDestination;
        private string destination = String.Empty;

        private bool ShowWarningAgain
        {
            get => Properties.Settings.Default.UnsafeBypass;
            set => Properties.Settings.Default.UnsafeBypass = value;
        }

        private string LinkText { get; set; }

        public Form1()
        {
            InitializeComponent();

            this.Activated += Form1_Activated;
            this.Deactivate += Form1_Deactivated;
        }

        private async void CheckForUpdates()
        {
            if (!await version.CheckGitHubNewerVersion()) return;
            DialogResult dlgr = MessageBox.Show(
                "There is a new version of the program available!\n\n" +
                "Do you want to download it?\n\n" +
                $"Installed version: {Assembly.GetExecutingAssembly().GetName().Version}\n" +
                "New Version: " + await version.GetGithubVersionAsync(), "Update Available",
                MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (dlgr == DialogResult.Yes)
                System.Diagnostics.Process.Start("https://github.com/Glumboi/EZLinkvertiseBypasser/releases");
        }

        private void Form1_Deactivated(object sender, EventArgs e)
        {
            Hotkeys.UnloadHotkeys(this);
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            Hotkeys.LoadHotkeys(this);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UIChanger.ChangeTitlebarToDark(Handle);
            CheckForUpdates();
        }

        //Disabled to test certain things
        //(trying to fix a bug where clicking the button does nothing)
        /*private async void Button_Bypass_Click(object sender, EventArgs e)
        {
            //await StartBypass(ShowWarningAgain);
        }*/

        private void CreateBypasser(string link)
        {
            Bypasser.BypassURL(link);
            //return Task.CompletedTask;
        }

        void ScanForPatterns()
        {
            if (!Bypasser.Patterns.Any(x => LinkText.StartsWith(x)))
            {
                Glumboi.UI.Toast.ToastHandler.ShowToast("Link is not a valid Linkvertise link!", "Error");
                return;
            }
        }

        private async Task WaitForBypasserToFinish()
        {
            //This will let the app wait until the destination updated to ensure that every bypass doesnt end up empty
            //or the same as the one before
            while (lastDestination == Bypasser.Destination || string.IsNullOrWhiteSpace(Bypasser.Destination))
            {
                await Task.Delay(100);
                lastDestination = string.Empty;
            }

            lastDestination = Bypasser.Destination;
        }

        List<string> GetDebugInfo()
        {
            var dest = $"Destination: {Bypasser.Destination}";
            var time = $"Time: {Bypasser.Time} ms";
            var plugin = $"Plugin: {Bypasser.Plugin}";
            var query = $"Query: {Bypasser.Query}";
            destination = Bypasser.Destination;
            
            return new List<string>() { query, dest, time, plugin, };
        }

        void UpdateDebugList()
        {
            if (List_Debug.Items.Count > 0) List_Debug.Items.Clear();

            var info = GetDebugInfo();
            for (var index = 0; index < info.Count; index++)
            {
                var str = info[index];
                List_Debug.Items.Add(str);
            }
        }

        void OpenDestinationLink(bool unsafeBypass)
        {
            //If statement is inverted because of me being a little too dumb xD it should be unsafeBypass.
            //Instead of !unsafeBypass 
            if (!unsafeBypass)
            {
                if (!warningIsActive) //Blocks the user from opening another warning form which can lead into issues
                {
                    warning = new LinkOpenerForm();
                    warning.Label_Content.Text =
                        $"You are now going to open the bypassed link, are you sure?\n\nLink: {Bypasser.Destination}";
                    warning.Button_No.Click += Button_No_Click;
                    warning.Button_Yes.Click += Button_Yes_Click;
                    warning.FormClosing += Warning_FormClosing;
                    warning.Text = "Warning";

                    warning.Show();
                }

                warningIsActive = true;

                return;
            }

            Web.OpenUrl(Bypasser.Destination);
        }

        async Task StartBypass(bool unsafeBypass)
        {
            ScanForPatterns();
            CreateBypasser(LinkText);
            await WaitForBypasserToFinish();
            UpdateDebugList();
            OpenDestinationLink(unsafeBypass);
            
            Bypasser.Destination = string.Empty;
        }

        private void Warning_FormClosing(object sender, FormClosingEventArgs e)
        {
            warningIsActive = false;
            warning.Dispose();
        }

        private void Button_Yes_Click(object sender, EventArgs e)
        {
            warning.Close();
            Web.OpenUrl(destination);
        }

        private void Button_No_Click(object sender, EventArgs e)
        {
            warning.Close();
        }

        async Task CopyToClipboardFromList(string contentToCopy, RadListDataItem selectedItem)
        {
            Clipboard.SetText(contentToCopy);

            selectedItem.ForeColor = Color.LimeGreen;
            await Task.Delay(1300);
            selectedItem.ForeColor = Color.White;
        }

        //Old version of the function above, this may be used again in the future
        void CopyToClipboard(string contentToCopy, string toastContent)
        {
            Clipboard.SetText(contentToCopy);

            Glumboi.UI.Toast.ToastHandler.ShowToast(
                toastContent,
                "Info");
        }

        private async void List_Debug_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            RadListDataItem selectedItem = List_Debug.SelectedItem;
            string selectedItemStr = selectedItem.ToString();

            if (selectedItemStr.Contains("Destination"))
            {
                await CopyToClipboardFromList(destination, selectedItem);
            }

            if (selectedItemStr.Contains("Time"))
            {
                await CopyToClipboardFromList("Time: " + Bypasser.Time.ToString() + " ms", selectedItem);
            }

            if (selectedItemStr.Contains("Plugin"))
            {
                await CopyToClipboardFromList("Plugin: " + Bypasser.Plugin, selectedItem);
            }

            if (selectedItemStr.Contains("Query"))
            {
                await CopyToClipboardFromList(Bypasser.Query, selectedItem);
            }
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x0312 when m.WParam.ToInt32() == (int)Hotkeys.HotkeyIDs.HotkeyID:
                    Properties.Settings.Default.UnsafeBypass = false;
                    break;
            }

            base.WndProc(ref m);
        }

        private void Link_Textbox_TextChanged(object sender, EventArgs e)
        {
            LinkText = Link_Textbox.Text;
        }

        private async void Button_Bypass_MouseClick(object sender, MouseEventArgs e)
        {
            await StartBypass(ShowWarningAgain);
        }
    }
}