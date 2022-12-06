using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMSMonitorService
{
    class ClsConstants
    {
    }

    public class clsGetAppConfig
    {
        public static string GetValueForTheStringKey(string strKey)
        {
            string strValue = "";
            try
            {
                strValue = ConfigurationManager.AppSettings[strKey].Trim();
            }
            catch (Exception E)
            {
                LogApp.Log4Net.WriteException(E);
            }
            return strValue;
        }
    }
}