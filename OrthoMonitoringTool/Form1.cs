using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.DirectoryServices;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Windows.Forms;
using Microsoft.Web.Administration;
using Microsoft.Win32;
using OrthoMonitoringTool.Properties;

namespace OrthoMonitoringTool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            bool isStopped = StartStopOrtho();
            InitializeComponent(isStopped);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            bool isConFailed = GetOrthoMiddlewareConnection();
            bool isStopped = StartStopOrtho(isConFailed);
            SetMessage(isStopped);
            Notify.BalloonTipTitle = "Ortho";


            /*ServerManager mgr = new ServerManager();
            string SiteName = HostingEnvironment.SiteName;
            Site currentSite = mgr.Sites[SiteName];

            //The following obtains the application name and application object
            //The application alias is just the application name with the "/" in front

            string ApplicationAlias = HostingEnvironment.ApplicationVirtualPath;
            string ApplicationName = ApplicationAlias.Substring(1);
            Microsoft.Web.Administration.Application app = currentSite.Applications[ApplicationAlias];
            */
        }

        private bool GetOrthoMiddlewareConnection()
        {
            bool isFailed = false;
            IEnumerable<string> conection = File.ReadAllLines(@"D:\Ortho\MiddlewareCon.txt").Where(x => x.Contains("failed"));

            foreach (var item in conection)
            {
                isFailed = true;
            }
            // Open the text file using a stream reader.
            //using (var sr = new StreamReader("TestFile.txt"))
            //{
            //    // Read the stream as a string, and write the string to the console.
            //    Console.WriteLine(sr.ReadToEnd());
            //}
            return isFailed;
        }
        private void SetMessage(bool isStopped)
        {
            if (!isStopped)
            {
                Notify.BalloonTipText = "Ortho application started successfully";
                this.Notify.Icon = Resources.Blue;
                Notify.Text = "Ortho started";
                Notify.ShowBalloonTip(10000);
            }
            else
            {
                Notify.BalloonTipText = "Ortho application stopped successfully";
                this.Notify.Icon = Resources.Red;
                Notify.Text = "Ortho stopped";

                Notify.ShowBalloonTip(10000);
            }
        }

        private bool StartStopOrtho(bool startOrtho = false)
        {
            bool isStopped = false;
            ServerManager serverManager = new ServerManager();
            var sites1 = serverManager.Sites[0].Applications[0].pa;
            var sites = serverManager.Sites;
            var pool = serverManager.ApplicationDefaults.ApplicationPoolName;
            foreach (var site in sites)
            {
                if (site.State == ObjectState.Stopped && startOrtho)
                {
                    site.Start();
                    isStopped = false;
                }
                else if (site.State == ObjectState.Started && !startOrtho)
                {
                    site.Stop();
                    isStopped = true;
                }
                else if (site.State == ObjectState.Stopped)
                {
                    isStopped = true;
                }
            }
            return isStopped;
        }
        private void notifyIcon_Click(object sender, EventArgs e)
        {
            var eventArgs = e as MouseEventArgs;
            switch (eventArgs.Button)
            {
                // Left click to reactivate
                case MouseButtons.Left:
                    // Do your stuff
                    break;
            }
        }

        private void Notify_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            Notify.Visible = true;
            WindowState = FormWindowState.Normal;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                Notify.Visible = true;
                Notify.ShowBalloonTip(10000);
            }
            else if (FormWindowState.Normal == this.WindowState)
                Notify.Visible = false;
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool isStopped = StartStopOrtho(true);
            SetMessage(isStopped);
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool isStopped = StartStopOrtho(false);
            SetMessage(isStopped);
        }

        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool isStarted = StartStopOrtho(true);
            bool isStarted1 = StartStopOrtho(false);
            if (isStarted1)
            {
                Notify.BalloonTipTitle = "Ortho app restarted successfully";
                this.Notify.Icon = Resources.Blue;
            }
        }
    }

}
