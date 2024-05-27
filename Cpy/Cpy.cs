using System;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;

namespace Cpy
{
    public partial class Cpy : Form
    {
        string[] format = File.ReadAllLines("ex.txt");
        public Cpy()
        {
            InitializeComponent();
            progressBar.Value = 0;
            bgWorker.RunWorkerAsync();
            if (format[1] == "show")
            {
                this.Opacity = 100;
            }
            else
            {
                this.Opacity = 0;
            }
        }

        private bool hasWriteAccessToFolder(string folderPath)
        {
            try
            {
                System.Security.AccessControl.DirectorySecurity ds = Directory.GetAccessControl(folderPath);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        private void backgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                List<string> namesOfDirectories = new List<string>();
                foreach (DriveInfo driveInfo in DriveInfo.GetDrives())
                {
                    if (driveInfo.Name != format[2] && driveInfo.DriveType.ToString()!="Removable")
                    {
                        if (hasWriteAccessToFolder(driveInfo.Name))
                        {
                            DirectoryInfo info = new DirectoryInfo(driveInfo.Name);
                            FileInfo[] Files = info.GetFiles(format[0]);
                            foreach (FileInfo file in Files)
                            {
                                File.Copy(file.FullName + "", @"Exp\" + file.Name, true);
                            }
                        }
                        namesOfDirectories = subDirectories(driveInfo.Name);
                        {
                            int index = 1;
                            foreach (string s in namesOfDirectories)
                            {
                                if (s != null)
                                {
                                    if (hasWriteAccessToFolder(s))
                                    {
                                        DirectoryInfo info = new DirectoryInfo(s);
                                        FileInfo[] Files = info.GetFiles(format[0]);
                                        foreach (FileInfo file in Files)
                                        {
                                            File.Copy(file.FullName + "", @"Exp\" + file.Name, true);
                                        }
                                    }
                                }
                                if (!bgWorker.CancellationPending)
                                {
                                    bgWorker.ReportProgress(index++ * 100 / (namesOfDirectories.Count-1));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        List<string> nameOfDirectories = new List<string>();
        private List<string> subDirectories(string path)
        {
            var directory = new DirectoryInfo(path);
            if (hasWriteAccessToFolder(directory.FullName))
            {
                foreach (var d in directory.GetDirectories())
                {
                    if (hasWriteAccessToFolder(d.FullName))
                    {
                        if(d.FullName != @"C:\Documents and Settings")
                        {
                            if (d.GetDirectories().Length >= 0)
                            {
                                nameOfDirectories.Add(d.FullName);
                                subDirectories(d.FullName);
                            }
                        }
                    }
                }
            }
            return nameOfDirectories;
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            this.Close();
        }

        private void backgroundWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }
    }
}
