using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HDDLED
{
    public partial class Form1 : Form
    {
        NotifyIcon hddLedIcon;
        Icon activeIcon;
        Icon idleIcon;
        Thread hddLedWorker;

        public Form1()
        {
            InitializeComponent();
            activeIcon = new Icon("hddiconbusy.ico");
            idleIcon = new Icon("hddicon.ico");

            hddLedIcon = new NotifyIcon();
            hddLedIcon.Icon = idleIcon;
            hddLedIcon.Visible = true;

            MenuItem progNameMenuItem = new MenuItem("HDD");
            MenuItem quitMenuItem = new MenuItem("Quit");
            ContextMenu contextMenu = new ContextMenu();
            contextMenu.MenuItems.Add(progNameMenuItem);
            contextMenu.MenuItems.Add(quitMenuItem);
            hddLedIcon.ContextMenu = contextMenu;

            quitMenuItem.Click += quitMenuItem_Click;

            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
            hddLedWorker = new Thread(new ThreadStart(HddActivityThread));
            hddLedWorker.Start();
        }

        void quitMenuItem_Click(object sender, EventArgs e)
        {
            hddLedIcon.Dispose();
            this.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            

        }
         
        public void HddActivityThread()
        {
            try
            {
                ManagementClass driveDataClass = new ManagementClass("Win32_PerfFormattedData_PerfDisk_PhysicalDisk");
                while (true)
                {
                    ManagementObjectCollection driveDataClassCollection = driveDataClass.GetInstances();
                    foreach (ManagementObject obj in driveDataClassCollection)
                    {
                        if (obj["Name"].ToString() == "_Total")
                        {
                            if (Convert.ToUInt64(obj["DiskBytesPersec"]) > 0)
                            {
                                hddLedIcon.Icon = activeIcon;
                            }
                            else
                            {
                                hddLedIcon.Icon = idleIcon;
                            }
                        }
                    }
                    Thread.Sleep(1);
                }
            }
            catch (ThreadAbortException tbe)
            {

            }
        }
    }
}
