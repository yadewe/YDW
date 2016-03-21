using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using log4net;

namespace YDW.WinService
{
    public partial class RecordService : ServiceBase
    {
        RecordFile file = new RecordFile();
        UserOpration userOpration = new UserOpration();
        //public ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public RecordService()
        {
            InitializeComponent();
        }
        public void Startss()
        {
            OnStart(null);
        }
        protected override void OnStart(string[] args)
        {
            file.Init();

            file.SetStart();
            userOpration.ShowNoOperationTime += userOpration_ShowNoOperationTime;
            userOpration.LogShowNoOperationTime += userOpration_LogNoOperationTime;
        }

        void userOpration_ShowNoOperationTime(long obj)
        {
            file.SetNoOperation(obj);
        }
        void userOpration_LogNoOperationTime(string obj)
        {
            file.SetNoOperation(obj);
        }

        protected override void OnPause()
        {
            //AddLine(" -- at -------------- |Pause|" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            file.SetPause();
            base.OnPause();
        }
        protected override void OnStop()
        {
            //AddLine(" -- at -------------- |stop|" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            file.SetStop();
        }

        protected override void OnShutdown()
        {
            //AddLine(" --  at --------- |shutdown|" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            file.SetShutdown();
            base.OnShutdown();
        }

       

    }
}
