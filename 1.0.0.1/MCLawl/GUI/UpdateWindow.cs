using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Net;
using System.Diagnostics;

namespace MCSong.Gui
{
    public partial class UpdateWindow : Form
    {
        public static bool updating = false;

        public UpdateWindow()
        {
            InitializeComponent();
        }
        private void UpdateWindow_Load(object sender, EventArgs e)
        {
            txtStatus.Text = "Retrieving Updates";
            prgStatus.Value = 0;
            prgStatus.Update();
            txtStatus.Update();
            try
            {
                WebClient client = new WebClient();
                client.DownloadFile("http://mcsong.comule.com/updates/text/revs.txt", "text/revs.txt");
                listRevisions.Items.Clear();
                FileInfo file = new FileInfo("text/revs.txt");
                StreamReader stRead = file.OpenText();
                if (File.Exists("text/revs.txt"))
                {
                    while (!stRead.EndOfStream)
                    {
                        listRevisions.Items.Add(stRead.ReadLine());
                    }
                }
                txtStatus.Text = "Retrieved Updates";
                stRead.Close();
                stRead.Dispose();
                file.Delete();
                client.Dispose();
                updating = false;
            }
            catch (Exception ex)
            {
                Server.ErrorLog(ex);
                listRevisions.Items.Clear();
                txtStatus.Text = "ERROR";
            }
            prgStatus.Value = 0;
            prgStatus.Update();
            txtStatus.Update();
            updating = false;
        }

        private void cmdUpdate_Click(object sender, EventArgs e)
        {
            updating = true;
            if (Server.selectedrevision != "")
            {
                PerformUpdate(true);
            }
            else { PerformUpdate(false); }
            updating = false;
      /*      if (!Program.CurrentUpdate)
                Program.UpdateCheck();
            else
            {
                Thread messageThread = new Thread(new ThreadStart(delegate
                {
                    MessageBox.Show("Already checking for updates.");
                })); messageThread.Start();
            } */
        }


        private void listRevisions_SelectedValueChanged(object sender, EventArgs e)
        {
            Server.selectedrevision = listRevisions.SelectedItem.ToString();
        }

        private void PerformUpdate(bool oldrevision)
        {
            try
            {
                prgStatus.Value = 20;
                txtStatus.Text = "Creating Update.bat";
                prgStatus.Update();
                txtStatus.Update();
                StreamWriter SW;
                if (!Server.mono)
                {
                    if (!File.Exists("Update.bat"))
                        SW = new StreamWriter(File.Create("Update.bat"));
                    else
                    {
                        if (File.ReadAllLines("Update.bat")[0] != "::Version 3")
                        {
                            SW = new StreamWriter(File.Create("Update.bat"));
                        }
                        else
                        {
                            SW = new StreamWriter(File.Create("Update_generated.bat"));
                        }
                    }
                    SW.WriteLine("::Version 3");
                    SW.WriteLine("TASKKILL /pid %2 /F");
                    SW.WriteLine("if exist MCSong_.dll.backup (erase MCSong_.dll.backup)");
                    SW.WriteLine("if exist MCSong_.dll (rename MCSong_.dll MCSong_.dll.backup)");
                    SW.WriteLine("if exist MCSong.new (rename MCSong.new MCSong_.dll)");
                    SW.WriteLine("start MCSong.exe");
                }
                else
                {
                    prgStatus.Value = 20;
                    txtStatus.Text = "Creating Update.sh";
                    prgStatus.Update();
                    txtStatus.Update();
                    if (!File.Exists("Update.sh"))
                        SW = new StreamWriter(File.Create("Update.sh"));
                    else
                    {
                        if (File.ReadAllLines("Update.sh")[0] != "#Version 2")
                        {
                            SW = new StreamWriter(File.Create("Update.sh"));
                        }
                        else
                        {
                            SW = new StreamWriter(File.Create("Update_generated.sh"));
                        }
                    }
                    SW.WriteLine("#Version 2");
                    SW.WriteLine("#!/bin/bash");
                    SW.WriteLine("kill $2");
                    SW.WriteLine("rm MCSong_.dll.backup");
                    SW.WriteLine("mv MCSong_.dll MCSong.dll_.backup");
                    SW.WriteLine("wget http://mcsong.comule.com/updates/MCSong_.dll");
                    SW.WriteLine("mono MCSong.exe");
                }

                SW.Flush(); SW.Close(); SW.Dispose();
                prgStatus.Value = 40;
                txtStatus.Text = "File Created";
                prgStatus.Update();
                txtStatus.Update();

                string filelocation = "";
                string verscheck = "";
                Process proc = Process.GetCurrentProcess();
                string assemblyname = proc.ProcessName + ".exe";
                prgStatus.Value = 60;
                txtStatus.Text = "Checking Version";
                prgStatus.Update();
                txtStatus.Update();
                if (!oldrevision)
                {
                    try
                    {
                        WebClient client = new WebClient();
                        Server.selectedrevision = client.DownloadString("http://mcsong.comule.com/updates/text/curversion.txt");
                        client.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Server.ErrorLog(ex);
                        txtStatus.Text = "ERROR";
                        listRevisions.Items.Clear();
                        prgStatus.Value = 0;
                        updating = false;
                        return;
                    }
                }
                verscheck = Server.selectedrevision.TrimStart('r');
                int vers = int.Parse(verscheck.Split('.')[0]);
                prgStatus.Value = 80;
                txtStatus.Text = "Downloading Files";
                prgStatus.Update();
                txtStatus.Update();
                try
                {
                    if (oldrevision) { filelocation = ("http://mcsong.comule.com/updates/archives/" + Server.selectedrevision + "/MCSong.exe"); }
                    if (!oldrevision) { filelocation = ("http://mcsong.comule.com/updates/MCSong_.dll"); }
                    WebClient Client = new WebClient();
                    Client.DownloadFile(filelocation, "MCSong.new");
                    Client.DownloadFile("http://mcsong.comule.com/updates/text/changelog.txt", "extra/Changelog.txt");
                }
                catch (Exception ex)
                {
                    Server.ErrorLog(ex);
                    txtStatus.Text = "ERROR";
                    listRevisions.Items.Clear();
                    prgStatus.Value = 0;
                    updating = false;
                    return;
                }
                foreach (Level l in Server.levels) l.Save();
                foreach (Player pl in Player.players) pl.save();

                string fileName;
                if (!Server.mono) fileName = "Update.bat";
                else fileName = "Update.sh";

                try
                {
                    if (MCSong.Gui.Window.thisWindow.notifyIcon1 != null)
                    {
                        MCSong.Gui.Window.thisWindow.notifyIcon1.Icon = null;
                        MCSong.Gui.Window.thisWindow.notifyIcon1.Visible = false;
                    }
                }
                catch { }

                prgStatus.Value = 100;
                txtStatus.Text = "Starting Update";

                Process p = Process.Start(fileName, "main " + System.Diagnostics.Process.GetCurrentProcess().Id.ToString());
                p.WaitForExit();

                prgStatus.Value = 0;
                txtStatus.Text = "Update Complete";
            }
            catch (Exception e) { Server.ErrorLog(e); }
        }

        private void UpdateWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (updating)
            {
                e.Cancel = true;
                MessageBox.Show("Update in progress! Please do not close the updater!", "Updater", MessageBoxButtons.OK);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txtStatus.Text = "Retrieving Updates";
            prgStatus.Value = 0;
            prgStatus.Update();
            txtStatus.Update();
            try
            {
                WebClient client = new WebClient();
                client.DownloadFile("http://mcsong.comule.com/updates/text/revs.txt", "text/revs.txt");
                listRevisions.Items.Clear();
                FileInfo file = new FileInfo("text/revs.txt");
                StreamReader stRead = file.OpenText();
                if (File.Exists("text/revs.txt"))
                {
                    while (!stRead.EndOfStream)
                    {
                        listRevisions.Items.Add(stRead.ReadLine());
                    }
                }
                txtStatus.Text = "Retrieved Updates";
                stRead.Close();
                stRead.Dispose();
                file.Delete();
                client.Dispose();
                updating = false;
            }
            catch (Exception ex)
            {
                Server.ErrorLog(ex);
                listRevisions.Items.Clear();
                txtStatus.Text = "ERROR";
                updating = false;
            }
            prgStatus.Value = 0;
            prgStatus.Update();
            txtStatus.Update();
        }
    }
}
