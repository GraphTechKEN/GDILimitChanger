using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;

namespace GDI_UpperChanger
{
    public partial class Form1 : Form
    {
        private int GDIValue = 10000;
        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (IsAdministrator())
            {
                GDIValue = (int)Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Windows", "GDIProcessHandleQuota", "default");
                textBox1.Text = GDIValue.ToString();
                textBox2.Text = GDIValue.ToString("x4");
            }
            else
            {
                MessageBox.Show("管理者権限で実行してください、終了します");
                this.Close();
            }

        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            int.TryParse(textBox1.Text, out GDIValue);
            if (GDIValue >= 256 && GDIValue <= 65535)
            {
                textBox2.Text = GDIValue.ToString("x4");
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Windows", "GDIProcessHandleQuota", GDIValue);
                MessageBox.Show("適用するにはPCを再起動してください");
            }
            else
            {
                MessageBox.Show("値は 256 から 65535 の範囲で指定してください");
            }
        }

        private void btnDefault_Click(object sender, EventArgs e)
        {
            textBox1.Text = "10000";
            btnChange_Click(sender, e);
        }

        private void btnMax_Click(object sender, EventArgs e)
        {
            textBox1.Text = "65535";
            btnChange_Click(sender, e);
        }

        private void btnDouble_Click(object sender, EventArgs e)
        {
            textBox1.Text = "20000";
            btnChange_Click(sender, e);
        }

        public static bool IsAdministrator()
        {
            System.Security.Principal.WindowsIdentity wi =
                System.Security.Principal.WindowsIdentity.GetCurrent();
            System.Security.Principal.WindowsPrincipal wp =
                new System.Security.Principal.WindowsPrincipal(wi);
            return wp.IsInRole(
                System.Security.Principal.WindowsBuiltInRole.Administrator);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Process[] ps = Process.GetProcesses();
            foreach (Process p in ps)
            {
                try
                {
                    if (p.ProcessName == "BveTs")
                    {
                        var exeName = "BveTs.exe";
                        var pinfo = Process.GetProcessesByName(exeName);
                        var procHandle = NativeMethods.OpenProcess((uint)NativeMethods.ProcessAccessFlags.All, false, (int)p.Id);
                        var gdiObjCount = NativeMethods.GetGuiResources(procHandle, (uint)NativeMethods.GdiKind.GR_GDIOBJECTS);
                        tbGDI.Text=gdiObjCount.ToString();
                    }
                }
                catch { }
            }
        }
    }

    public class NativeMethods
    {
        [DllImport("user32.dll")]
        public static extern uint GetGuiResources(IntPtr hProcess, uint uiFlags);

        public enum GdiKind
        {
            GR_GDIOBJECTS = 0,
            GR_USEROBJECTS = 1,
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, int processId);

        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VirtualMemoryOperation = 0x00000008,
            VirtualMemoryRead = 0x00000010,
            VirtualMemoryWrite = 0x00000020,
            DuplicateHandle = 0x00000040,
            CreateProcess = 0x000000080,
            SetQuota = 0x00000100,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            QueryLimitedInformation = 0x00001000,
            Synchronize = 0x00100000
        }

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hObject);
    }
}
