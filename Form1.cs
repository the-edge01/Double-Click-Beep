using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DoubleClickBeep
{
    public partial class Form1 : Form
    {
        DateTime lastClick = DateTime.MinValue;
        int hHook;

        const int WM_LBUTTONDOWN = 0x201;
        const int WH_MOUSE_LL = 14;
        Win32Api.HookProc hProc;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            hProc = new Win32Api.HookProc(MouseHookProc);
            hHook = Win32Api.SetWindowsHookEx(WH_MOUSE_LL, hProc, IntPtr.Zero, 0);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Win32Api.UnhookWindowsHookEx(hHook);
        }

        private void Form1_FormClosed_1(object sender, FormClosedEventArgs e)
        {
            Win32Api.UnhookWindowsHookEx(hHook);
        }

        private int MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                if ((Int32)wParam == WM_LBUTTONDOWN)
                {
                    DateTime newClick = DateTime.Now;
                    long diff = newClick.Ticks - lastClick.Ticks;
                    diff = diff / 10_000; //convert ticks to ms
                    if (diff <= SystemInformation.DoubleClickTime)
                    {
                        Console.Beep();
                    }
                    lastClick = newClick;
                }
            }
            return Win32Api.CallNextHookEx(hHook, nCode, wParam, lParam);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

    }

    public class Win32Api
    {
        public delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, IntPtr wParam, IntPtr lParam);
    }

}