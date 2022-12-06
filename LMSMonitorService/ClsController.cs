using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LMSMonitorService.DataBaseHandler;
using LogApp;

namespace LMSMonitorService
{
    class ClsController
    {

        public static bool isServerRunning = false;
        public static int Service_SleepInterval = 0;
        public static string LMSDB = string.Empty;
        public static string EmployeeDB = string.Empty;
        public static string OnPrem = string.Empty;
        public static string SinglePointCloudDB = string.Empty;
        public static string SinglePoint = string.Empty;
        CsLMS objLMS;
        public static int TrainingCourseCompanyIds = 0;

        public void StartServer()
        {

            try
            {
              //  System.Diagnostics.Debugger.Launch();
                Log4Net.WriteLog("starting server", LogType.GENERALLOG);
                LMSDB = clsGetAppConfig.GetValueForTheStringKey("LMsDB");
                EmployeeDB = clsGetAppConfig.GetValueForTheStringKey("EmployeeDB");

                SinglePointCloudDB = clsGetAppConfig.GetValueForTheStringKey("CloudDB");
                OnPrem = clsGetAppConfig.GetValueForTheStringKey("onPrem");

                string SqlConn = clsGetAppConfig.GetValueForTheStringKey("SQLConn");
                ClsDBConnInfo.DBConnectionString_Employee = SqlConn.Replace("[DBNAME]", EmployeeDB);
                ClsDBConnInfo.DBConnectionString_LMS = SqlConn.Replace("[DBNAME]", LMSDB);
                ClsDBConnInfo.DBConnectionString_SinglePointCloud = SqlConn.Replace("[DBNAME]", SinglePointCloudDB);
                Service_SleepInterval = Convert.ToInt32(clsGetAppConfig.GetValueForTheStringKey("Service_SleepInterval"));
                SinglePoint = clsGetAppConfig.GetValueForTheStringKey("SinglePoint");
                string sqlCon = clsGetAppConfig.GetValueForTheStringKey("SQLConn");
                bool RunLMS = clsGetAppConfig.GetValueForTheStringKey("RunLMS") == "1" ? true : false;

                isServerRunning = true;
                if (Service_SleepInterval > 0)
                {
                    if (RunLMS)
                    {
                        objLMS = new CsLMS();
                        objLMS.Start_LMS();

                    }
                }
            }
            catch(Exception ex)
            {
                Log4Net.WriteException(ex);
            }

          


          



        }

        public void StopServer()
        {
            try
            {
                bool RunLMS = clsGetAppConfig.GetValueForTheStringKey("RunLMS") == "1" ? true : false;
                Log4Net.WriteLog("Stopping Server", LogType.GENERALLOG);
                isServerRunning = false;
                if (RunLMS)
                {

                    objLMS.Stop_LMS();
                }

                Application.Exit();
                Environment.Exit(0);
            }

            catch (Exception ex)
            {

            }
        }



    }
}
