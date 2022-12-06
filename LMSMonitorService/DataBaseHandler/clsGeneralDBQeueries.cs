using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogApp;
using System.Data;
using System.Data.SqlClient;
namespace LMSMonitorService.DataBaseHandler
{
    class clsGeneralDBQeueries
    {

        public static object GetData(string connectionString, string commandText)
        {
            try
            {
              //  LogApp.Log4Net.WriteLog("Query to Execute : [ " + commandText + " ]", LogType.GENERALLOG);

                SqlCommand cmd = new SqlCommand(commandText);
                SqlDataAdapter adpt = new SqlDataAdapter();
                DataSet ds = new DataSet();
                int result = -5;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    cmd.CommandTimeout = 1;

                    adpt.SelectCommand = cmd;

                    cmd.Connection = conn;
                    result = adpt.Fill(ds);


                    if (conn.State == ConnectionState.Open)
                        conn.Close();


                    return ds;
                }
            }
            catch (Exception E)
            {
                LogApp.Log4Net.WriteException(E);
            }
            return null;
        }


        public int GetReminderStatus(string connectionString, string commandText)
        {
            SqlCommand cmd = new SqlCommand(commandText);
            SqlDataAdapter adpt = new SqlDataAdapter();
            DataSet ds = new DataSet();
            int reminderStatus=0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                cmd.CommandTimeout = 1;

                adpt.SelectCommand = cmd;

                cmd.Connection = conn;
             //   result = adpt.Fill(ds);

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    try
                    {
                       
                        reminderStatus = DBNull.Value.Equals(row["ReminderStatus"]) ? 0 : Convert.ToInt32(row["ReminderStatus"]);
                       

                       
                       
                    }
                    catch (Exception ex)
                    {
                        LogApp.Log4Net.WriteException(ex);
                    }
                }



                if (conn.State == ConnectionState.Open)
                    conn.Close();


                return reminderStatus;
            }

        }

    }
}
