using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace YDW.WinService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            //var a= new RecordFile();
            //a.Init();
            //a.Save();
            //return;
            //new Service1().Startss();
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new RecordService() 
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
