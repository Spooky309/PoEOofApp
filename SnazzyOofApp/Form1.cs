using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
namespace SnazzyOofApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Thread oofThread;
        private void Form1_Load(object sender, EventArgs e)
        {
            if (!System.Reflection.Assembly.GetExecutingAssembly().Location.Contains("Startup")) {
                DialogResult yes = MessageBox.Show("I see I am not running from the startup folder. Do you want me to go there and start on boot?", "Startup", MessageBoxButtons.YesNo);
                if (yes == DialogResult.Yes)
                {
                    string myPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    System.IO.File.Copy(myPath, Environment.ExpandEnvironmentVariables("%appdata%\\Microsoft\\Windows\\Start Menu\\Programs\\Startup\\PoEOofApp.exe"), true);
                    MessageBox.Show(@"If you need to get rid of it, you can delete PoEOofApp.exe in %appdata%\Microsoft\Windows\Start Menu\Programs\Startup");
                }
            }
            notifyIcon1.Visible = true;
            oofThread = new Thread(new ThreadStart(OofRoutine));
            oofThread.Start();
        }
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.Visible = false;
            this.Hide();
        }
        private static void OofRoutine() {
            Process process = null;
            bool PoeExists = false;
            while (PoeExists == false)
            {
                foreach (Process p in Process.GetProcesses())
                {
                    if (p.ProcessName.Contains("PathOfExile"))
                    {
                        process = p;
                        PoeExists = true;
                    }
                }
                if (!PoeExists)
                {
                    System.Threading.Thread.Sleep(1000);
                }
            }
            // Poe exists and is loaded into process variable
            string logPath = process.MainModule.FileName.Substring(0, process.MainModule.FileName.LastIndexOf("\\")) + "\\logs\\Client.txt";
            // Now we have the log path, we can open it, but first we'll open the sound resource
            System.Reflection.Assembly ass = Assembly.GetExecutingAssembly();
            Stream st = ass.GetManifestResourceStream("SnazzyOofApp.oof.wav");
            System.Media.SoundPlayer oof = new System.Media.SoundPlayer(st);

            using (FileStream s = File.Open(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                string str = string.Empty; // For storing what was read last time
                while (true)
                {
                    s.Seek(-60, SeekOrigin.End);
                    byte[] bytes = new byte[60];
                    s.Read(bytes, 0, 60);
                    string str2 = Encoding.Default.GetString(bytes);
                    if (str2 != str) // If it's not the same as it was last time
                    {
                        str = str2; // Set the string and do whatever we need to do this time
                        if (str.Contains("has been slain"))
                        {
                            oof.PlaySync();
                        }
                    }
                    System.Threading.Thread.Sleep(100);
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            oofThread.Abort();
            Application.Exit();
        }
    }
}
