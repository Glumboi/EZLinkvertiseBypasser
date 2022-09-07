﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
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
        LinkOpenerForm warning;
        VersionControl version = new VersionControl();

        public bool ShowWarningAgain
        {
            get => Properties.Settings.Default.UnsafeBypass;
            set => Properties.Settings.Default.UnsafeBypass = value;
        }

        public Form1()
        {
            InitializeComponent();

            this.Activated += Form1_Activated;
            this.Deactivate += Form1_Deactivated;
        }

        async void CheckForUpdates()
        {
            if (await version.CheckGitHubNewerVersion())
            {
                DialogResult dlgr = MessageBox.Show(
                     "There is a new version of the program available!\n\n" +
                     "Do you want to download it?\n\n" +
                     $"Installed version: {Assembly.GetExecutingAssembly().GetName().Version}\n" +
                     "New Version: " + await version.GetGithubVersionAsync(), "Update Available",
                     MessageBoxButtons.YesNo, MessageBoxIcon.Information);


                if (dlgr == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start("https://github.com/Glumboi/EZLinkvertiseBypasser/releases");
                }

            }
        }

        private void Form1_Deactivated(object sender, EventArgs e)
        {
            Hotkeys.Unloadhotkeys(this);
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

        private void Button_Bypass_Click(object sender, EventArgs e)
        {
            StartBypass(ShowWarningAgain);
        }

        Task CreateBypasser(string link)
        {
            Bypasser bypasser = Bypasser.BypassURL(link);
            return Task.CompletedTask;
        }

        async void StartBypass(bool unsafeBypass)
        {
            var link = Link_Textbox.Text;

            if (!Bypasser.Patterns.Any(x => link.StartsWith(x)))
            {
                Glumboi.UI.Toast.ToastHandler.ShowToast("Link is not a valid Linkvertise link!", "Error");
                return;
            }

            await CreateBypasser(link);

            //await Task.Delay(30);

            var dest = $"Destination: {Bypasser.Destination}";
            var time = $"Time: {Bypasser.Time} ms"; //bypasser.Time.ToString();
            var plugin = $"Plugin: {Bypasser.Plugin}";//bypasser.Plugin;
            var query = $"Query: {Bypasser.Query}";

            string[] debugInfo = { query, dest, time, plugin, };

            if (string.IsNullOrEmpty(Bypasser.Destination))
            {
                Glumboi.UI.Toast.ToastHandler.ShowToast(
                    "Could not bypass the link, " +
                    "try again or chose a different link!",
                    "Error");
                return;
            }

            if (List_Debug.Items.Count > 0) List_Debug.Items.Clear();

            foreach (string str in debugInfo)
            {
                List_Debug.Items.Add(str);
            }

            if (!unsafeBypass)
            {
                warning = new LinkOpenerForm();
                warning.Label_Content.Text = $"You are now going to open the bypassed link, are you sure?\n\nLink: {Bypasser.Destination}";
                warning.Button_No.Click += Button_No_Click;
                warning.Button_Yes.Click += Button_Yes_Click;
                warning.Text = "Warning";

                warning.Show();
                return;
            }
            Web.OpenUrl(Bypasser.Destination);
        }

        private void Button_Yes_Click(object sender, EventArgs e)
        {
            warning.Close();
            Web.OpenUrl(Bypasser.Destination);
        }

        private void Button_No_Click(object sender, EventArgs e)
        {
            warning.Close();
        }

        async Task CopyToClipboardFromList(string contentToCopy, RadListDataItem selectedItem)
        {
            Clipboard.SetText(contentToCopy);

            selectedItem.ForeColor = Color.LimeGreen;
            await Task.Delay(1000);
            selectedItem.ForeColor = Color.White;
        }

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
                await CopyToClipboardFromList(Bypasser.Destination, selectedItem);
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
    }
}
