using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;

namespace YDW.WinService
{
    public class UserOpration2
    { 
        // 创建结构体用于返回捕获时间
        [StructLayout(LayoutKind.Sequential)]
        struct LASTINPUTINFO
        {
            // 设置结构体块容量
            [MarshalAs(UnmanagedType.U4)]
            public int cbSize;
            // 捕获的时间
            [MarshalAs(UnmanagedType.U4)]
            public uint dwTime;
        }
        [DllImport("user32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);
        // 获取键盘和鼠标没有操作的时间
        public static long GetLastInputTime()
        {
            LASTINPUTINFO vLastInputInfo = new LASTINPUTINFO();
            vLastInputInfo.cbSize = Marshal.SizeOf(vLastInputInfo);
            // 捕获时间
            if (!GetLastInputInfo(ref vLastInputInfo))
                return 0;
            else
                return Environment.TickCount - (long)vLastInputInfo.dwTime;
        }
        public static string GetLogGetLastInputTime()
        {
            LASTINPUTINFO vLastInputInfo = new LASTINPUTINFO();
            vLastInputInfo.cbSize = Marshal.SizeOf(vLastInputInfo);
            // 捕获时间
            if (!GetLastInputInfo(ref vLastInputInfo))
                return "none";
            else
                return string.Format("Environment.TickCount {0}, vLastInputInfo.dwTime {1}, vLastInputInfo.cbSize {2}", Environment.TickCount, vLastInputInfo.dwTime, vLastInputInfo.cbSize);
        }
    }

    public class UserOpration
    {

        public event Action<long> ShowNoOperationTime;
        public event Action<string> LogShowNoOperationTime;
        Timer timer1;

        [StructLayout(LayoutKind.Sequential)]
        public struct LASTINPUTINFO
        {
            [MarshalAs(UnmanagedType.U4)]
            public int cbSize;
            [MarshalAs(UnmanagedType.U4)]
            public uint dwTime;
        }
        [DllImport("user32.dll")]
        public static extern bool GetLastInputInfo(ref    LASTINPUTINFO plii);

        public UserOpration()
        {
            timer1 = new Timer();
            this.timer1.Interval = 2000;
            timer1.Elapsed += timer1_Tick;
        }

        public void Start()
        {
            this.timer1.Enabled = true;
        }

        public long getIdleTick()
        {
            LASTINPUTINFO vLastInputInfo = new LASTINPUTINFO();
            vLastInputInfo.cbSize = Marshal.SizeOf(vLastInputInfo);
            if (!GetLastInputInfo(ref    vLastInputInfo)) return 0;
            return Environment.TickCount - (long)vLastInputInfo.dwTime;
        }



        private void timer1_Tick(object sender, EventArgs e)
        {
            long i = UserOpration2.GetLastInputTime();
            string msg = UserOpration2.GetLogGetLastInputTime();

            if (ShowNoOperationTime != null)
                ShowNoOperationTime(i);
            if (LogShowNoOperationTime != null)
                LogShowNoOperationTime(msg);
        }
    }
}