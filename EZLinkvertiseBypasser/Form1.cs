using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EZLinkvertiseBypasser.Core;
using Glumboi.UI;

namespace EZLinkvertiseBypasser
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UIChanger.ChangeTitlebarToDark(Handle);
        }

        private void Button_Bypass_Click(object sender, EventArgs e)
        {
            StartBypass();
        }

        Task CreateBypasser(string link)
        {
            Bypasser bypasser = Bypasser.BypassURL(link);
            return Task.CompletedTask;
        }

        async void StartBypass()
        {
            var link = Link_Textbox.Text;

            if (!Bypasser.Patterns.Any(x => link.StartsWith(x)))
            {
                Glumboi.UI.Toast.ToastHandler.ShowToast("Link is not a valid Linkvertise link!", "Error");
                return;
            }

            await CreateBypasser(link);

            var dest = $"Destination: {Bypasser.Destination}";
            var time = $"Time: {Bypasser.Time} ms"; //bypasser.Time.ToString();
            var plugin = $"Plugin: {Bypasser.Plugin}";//bypasser.Plugin;
            var query = $"Query: {Bypasser.Query}";

            string[] debugInfo = { query, dest, time, plugin,};

            if (string.IsNullOrEmpty(Bypasser.Destination))
            {
                /*MessageBox.Show("Could not bypass the link, try again or chose a different link!",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);*/

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

            DialogResult dialogResult = MessageBox.Show($"Link bypassed with success, " +
                $"do you want to open it in your browser?\n\nLink: {Bypasser.Destination}",
                "Info",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information);

            if (dialogResult == DialogResult.Yes)
            {
                Web.OpenUrl(Bypasser.Destination);
            }
            else if (dialogResult == DialogResult.No)
            {
                CopyToClipboard(Bypasser.Destination, "Destination copied to clipboard!");
                return;
            }
        }

        void CopyToClipboard(string contentToCopy, string toastContent)
        {
            Clipboard.SetText(contentToCopy);

            Glumboi.UI.Toast.ToastHandler.ShowToast(
                toastContent,
                "Info");
        }

        private void List_Debug_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string selectedItem = List_Debug.SelectedItem.ToString();

            if(selectedItem.Contains("Destination"))
            {
                CopyToClipboard(Bypasser.Destination, "Destination copied to clipboard!");
            }

            if (selectedItem.Contains("Time"))
            {
                CopyToClipboard("Time: " + Bypasser.Time.ToString() + " ms", "Time copied to clipboard!");
            }

            if (selectedItem.Contains("Plugin"))
            {
                CopyToClipboard("Plugin: " + Bypasser.Plugin, "Plugin copied to clipboard!");
            }

            if (selectedItem.Contains("Query"))
            {
                CopyToClipboard(Bypasser.Query, "Query copied to clipboard!");
            }
        }
    }
}
