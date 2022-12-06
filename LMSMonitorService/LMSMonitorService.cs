using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogApp;

namespace LMSMonitorService
{
    public partial class LMSMonitorService : ServiceBase
    {
        ClsController objController=null;
        public LMSMonitorService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                // System.Diagnostics.Debugger.Launch();
                LogApp.Log4Net.WriteLog("on start ", LogType.GENERALLOG);
                initLogs();
                objController = new ClsController();
                new System.Threading.Thread(new System.Threading.ThreadStart(objController.StartServer)).Start();
            }
            catch (Exception ex)
            {
                LogApp.Log4Net.WriteLog(ex.Message, LogType.GENERALLOG);
            }
        }


        public void OnStartManual()
        {
            try
            {
                initLogs();
                objController = new ClsController();
                objController.StartServer();
            }
            catch(Exception ex)
            {

            }
           
        }

        protected override void OnStop()
        {
            objController.StopServer();
        }

       public void OnStopManual()
        {
            objController.StopServer();
        }
        public static bool initLogs()
        {
            try
            {
                #region LogApp.Log4Net Settings

                try
                {

                    LogApp.Log4Net.EnableCOMMSLOG = LogApp.Log4Net.EnableDBLOG = LogApp.Log4Net.EnableERRORLOG = LogApp.Log4Net.EnableGENERALLOG = LogApp.Log4Net.EnableTELEPHONYLOG = true;
                    LogApp.Log4Net.FilePath = Path.GetDirectoryName(Application.ExecutablePath) + "\\Logs";

                    if (!Directory.Exists(LogApp.Log4Net.FilePath))
                        Directory.CreateDirectory(LogApp.Log4Net.FilePath);

                    LogApp.Log4Net.FileName = "LMSSystemMonitorServiceLog.txt";
                    LogApp.Log4Net.FileSize = 15;
                    LogApp.Log4Net.TotalFiles = 50;
                    //LogApp.Log4Net.Seperator = clsGetAppConfig.GetValueForTheStringKey(Constants.AppConfigKeys.LogSeperator);
                    LogApp.Log4Net.Activate(false);

                }
                catch (Exception ex)
                {
                    LogApp.Log4Net.WriteException(ex);
                }
                #endregion LogApp.Log4Net Settings
            }
            catch (Exception ex)
            {
                LogApp.Log4Net.WriteException(ex);
            }
            return true;
        }
    }
}
