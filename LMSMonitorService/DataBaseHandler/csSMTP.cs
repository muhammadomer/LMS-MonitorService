using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using LogApp;
namespace LMSMonitorService.DataBaseHandler
{
    class csSMTP
    {

        public static csSMTPMailEntity GetSMTPSetting(string ConnectionString)
        {
            csSMTPMailEntity objSMTP = null;
            try
            {
                string Query = "SELECT [MailServer], [SMTPPort], [EnableSSL], [MailUsername], [MailPassword] FROM [dbo].[DentonsEmployeesSettings];";
                DataSet ds = (DataSet)clsGeneralDBQeueries.GetData(ConnectionString, Query);
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows != null && ds.Tables[0].Rows.Count > 0)
                {
                    try
                    {
                        DataRow row = ds.Tables[0].Rows[0];
                        objSMTP = new csSMTPMailEntity();
                        objSMTP.MailServer = DBNull.Value.Equals(row["MailServer"]) ? string.Empty : row["MailServer"].ToString();
                        objSMTP.Port = DBNull.Value.Equals(row["SMTPPort"]) ? 0 : Convert.ToInt32(row["SMTPPort"]);
                        objSMTP.EnableSSL = DBNull.Value.Equals(row["EnableSSL"]) ? false : Convert.ToBoolean(row["EnableSSL"]);
                        objSMTP.MailUsername = DBNull.Value.Equals(row["MailUsername"]) ? string.Empty : row["MailUsername"].ToString();
                        objSMTP.MailPassword = DBNull.Value.Equals(row["MailPassword"]) ? string.Empty : row["MailPassword"].ToString();

                        if (objSMTP.MailServer.Trim() == string.Empty)
                        {
                            Log4Net.WriteLog("Mail server not found returning NULL", LogType.GENERALLOG);
                            return null;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogApp.Log4Net.WriteException(ex);
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                LogApp.Log4Net.WriteException(ex);
            }
            return objSMTP;
        }
        public static bool GetLMSNotificationValue(string ConnectionString)
        {
            try
            {
                string Query = "SELECT [EnableNotification] FROM [dbo].[Settings];";
                DataSet ds = (DataSet)clsGeneralDBQeueries.GetData(ConnectionString, Query);
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows != null && ds.Tables[0].Rows.Count > 0)
                {
                    try
                    {
                        DataRow row = ds.Tables[0].Rows[0];
                        return DBNull.Value.Equals(row["EnableNotification"]) ? false : Convert.ToBoolean(row["EnableNotification"]);

                    }
                    catch (Exception ex)
                    {
                        LogApp.Log4Net.WriteException(ex);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                LogApp.Log4Net.WriteException(ex);
            }
            return false;
        }

      
    }
    public class csSMTPMailEntity
    {
        public string MailServer { get; set; }
        public int Port { get; set; }
        public bool EnableSSL { get; set; }
        public string MailUsername { get; set; }
        public string MailPassword { get; set; }

    }
}
