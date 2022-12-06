using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LogApp;
using LMSMonitorService.DataBaseHandler;
using System.Data;
using System.Data.SqlClient;

namespace LMSMonitorService.DataBaseHandler
{
    class CsLMS
    {
        public DateTime NextExecutionTime = DateTime.Now;
        public bool firstTime = false;
        public long ThreadID_LMS = 0;
        public Thread thread_LMS = null;
        public string ApplicationName = "LMS";

        string TrainingOfficer="", CompanyName="";

       public static string EmailSubject = "";
        public static string EmailBody = "";
        public static string Title = "";


        public static bool[] sendReminder= { false,false };
        public static bool sendCompliance=false;

      public static  bool reminder = false;
        public void Start_LMS()
        {
            try {

                Stop_LMS();
                ThreadID_LMS = DateTime.Now.Ticks;
                thread_LMS = new Thread(new ParameterizedThreadStart(LMS_ThreadHandler));
                thread_LMS.Start(ThreadID_LMS);
            }
            catch (Exception ex)
            {
                Log4Net.WriteException(ex);
            }

        }
        public void Stop_LMS()
        {
            try
            {
                ThreadID_LMS = 0;
                if(thread_LMS!=null)
                {
                    thread_LMS.Abort();
                }
            }
            catch (Exception ex)
            {
                Log4Net.WriteException(ex);
            }
            thread_LMS = null;
        }
        public void LMS_ThreadHandler(object obj)
        {
            LogApp.Log4Net.WriteLog("Thread Started", LogType.GENERALLOG);
            long threadid = (long)obj;
            try {

                string LMSDB = ClsController.LMSDB;
                string EmployeeDB = ClsController.EmployeeDB;

                while (ThreadID_LMS==threadid && ClsController.isServerRunning)
                {
                 System.Threading.Thread.Sleep(1000);
                  System.Threading.Thread.Sleep(ClsController.Service_SleepInterval * 1000);
                    try
                    {
                        Log4Net.WriteLog("========================= START THREAD =================================", LogType.GENERALLOG);
                        string SqlConn = clsGetAppConfig.GetValueForTheStringKey("SQLConn");


                      //  if (ClsController.OnPrem == "0")
                      //  {
                            DateTime dateTime = DateTime.Now;
                            bool IsExecuteOverdueCourseMethod = false;
                            //if (firstTime)
                            //{
                            //    IsExecuteOverdueCourseMethod = true;
                            //    firstTime = false;
                            //}
                            //if (NextExecutionTime.Date <= dateTime.Date && dateTime.DayOfWeek == DayOfWeek.Monday)
                            //{
                            //    IsExecuteOverdueCourseMethod = true;
                            //    NextExecutionTime = NextExecutionTime.AddDays(7);
                            //}
                            Collection<int> accountIds = CsSystemMonitorData.Get_AccountIds(ClsDBConnInfo.DBConnectionString_SinglePointCloud);
                      
                        foreach (var DBId in accountIds)
                            {

                             //   ClsController.LMSDB = LMSDB + "_" + DBId;
                                ClsController.EmployeeDB = EmployeeDB + "_" + DBId;

                                Log4Net.WriteLog("------------------------- AccountId : " + ClsController.EmployeeDB + " ---------------------------------", LogType.GENERALLOG);
                                ClsDBConnInfo.DBConnectionString_LMS = SqlConn.Replace("[DBNAME]", ClsController.LMSDB);
                                ClsDBConnInfo.DBConnectionString_Employee = SqlConn.Replace("[DBNAME]", ClsController.EmployeeDB);

                                csSMTPMailEntity objSMTP = csSMTP.GetSMTPSetting(ClsDBConnInfo.DBConnectionString_Employee);
                              //  var isNotificationEnabled = csSMTP.GetLMSNotificationValue(ClsDBConnInfo.DBConnectionString_LMS);

                           // DateTime dateTime = DateTime.Now;
                          //  if (objSMTP!=null && isNotificationEnabled)
                              //  {

                                string time = DateTime.Now.ToLongTimeString();
                            time = "06:17:46 PM";
                            //    if (DateTime.Now.ToLongTimeString()=="11:11:00 PM" && dateTime.DayOfWeek!=DayOfWeek.Saturday && dateTime.DayOfWeek != DayOfWeek.Sunday)


                           



                            ClsController.TrainingCourseCompanyIds = CsSystemMonitorData.Get_TrainingCourseCompanyIds(ClsDBConnInfo.DBConnectionString_SinglePointCloud, DBId);

                         
                          

                           

                            if (objSMTP != null)
                            {
                                Process_TriggerType_OverDueTask_LMS_Alert(objSMTP, ClsController.EmployeeDB);
                                System.Threading.Thread.Sleep(1000);
                             //   Process_TriggerType_OneMonthCompliance_LMS_Alert(objSMTP, ClsController.EmployeeDB);
                              //  System.Threading.Thread.Sleep(1000);
                            }
                            


                            //   }

                            // }
                            // }

                            // else if (ClsController.OnPrem=="1")
                            //  {

                        }

                        }
                    catch (Exception ex) {
                    
                    }

                }


            }
            catch(Exception ex)
            {
                Log4Net.WriteException(ex);
            }

        }



        public void Process_TriggerType_OverDueTask_LMS_Alert(csSMTPMailEntity objSMTP, string EmployeeDB)
        {
            try
            {
                List<Employee> employeeList = CsSystemMonitorData.Get_Employees(ClsDBConnInfo.DBConnectionString_LMS, 0, EmployeeDB).
                    Where(x=>x.EmailReminderType !=0).OrderBy(x=>x.EmployeeID).OrderBy(x=>x.EmailReminderType).ToList();

                Log4Net.WriteLog("Total Courses Count:" + employeeList.Count, LogType.GENERALLOG);

                int UserID = 0;
                int EmailReminderType = 0;
                DataBaseHandler.AssignedCourses objAC;

                Employee _tempEmployee = new Employee();
                sendReminder = CsSystemMonitorData.Get_TrainingCourseReminderSettings(ClsDBConnInfo.DBConnectionString_LMS, ClsController.TrainingCourseCompanyIds);

                foreach (var employee in employeeList)
                {
                    try
                    {
                        if(UserID != employee.EmployeeID || EmailReminderType != employee.EmailReminderType)
                        {
                            Log4Net.WriteLog("Adding User with ID:" + employee.EmployeeID + " | EmailReminderType: " + employee.EmailReminderType, LogType.GENERALLOG);
                            //Send Email to previous values
                            if (UserID !=0 && EmailReminderType != 0)
                            {
                                Email_Process_TriggerType_OverDueTask_LMS_Alert(_tempEmployee, objSMTP);
                            }

                            _tempEmployee = new Employee();
                            _tempEmployee = employee;
                            _tempEmployee.assignedCourses = new List<AssignedCourses>();

                            UserID = employee.EmployeeID;
                            EmailReminderType = employee.EmailReminderType;                            
                        }
                        else
                        {
                            Log4Net.WriteLog("User with ID:" + employee.EmployeeID + " and course: " + employee.CourseName, LogType.GENERALLOG);
                        }

                        objAC = new AssignedCourses();
                        objAC.AsignedCourseID = employee.AssignedCourseId;
                        objAC.CourseName = employee.CourseName;
                        _tempEmployee.assignedCourses.Add(objAC);
                    }
                    catch (Exception ex)
                    {
                        Log4Net.WriteException(ex);
                    }
                }


                //Send last 1 course

             

               if (CsSystemMonitorData.Get_Employees(ClsDBConnInfo.DBConnectionString_LMS, 0, EmployeeDB).Where(x => x.EmailReminderType != 0).Count() > 0)
                {
                    Email_Process_TriggerType_OverDueTask_LMS_Alert(_tempEmployee, objSMTP);
                }
                  
               
            }
            catch (Exception ex)
            {
                Log4Net.WriteException(ex);
            }

        }

        private void Email_Process_TriggerType_OverDueTask_LMS_Alert(Employee _tempEmployee, csSMTPMailEntity objSMTP)
        {
            //Send Email to previous values
            try
            {
                Log4Net.WriteLog("Sending Email for user ID:" + _tempEmployee.EmployeeID + " | EmailReminderType: " + _tempEmployee.EmailReminderType, LogType.GENERALLOG);
                bool emailsent = false;

                if (_tempEmployee.EmailReminderType == 1 && sendReminder[0])
                {
                    Log4Net.WriteLog("Sending Reminder 1 email", LogType.GENERALLOG);
                    EmailBody = GetEmailBody_OverDueCourseTemplate1(_tempEmployee);
                    emailsent = EmailCls.SendEmail(objSMTP.MailServer, objSMTP.Port, objSMTP.EnableSSL, objSMTP.MailUsername, objSMTP.MailPassword, _tempEmployee.Email, ClsController.SinglePoint, EmailSubject.Trim(), EmailBody, "",1);
                }
                else if (_tempEmployee.EmailReminderType == 2 && sendReminder[0])
                {
                    Log4Net.WriteLog("Sending Reminder 2 email", LogType.GENERALLOG);
                    EmailBody = GetEmailBody_OverDueCourseTemplate2(_tempEmployee);
                    emailsent = EmailCls.SendEmail(objSMTP.MailServer, objSMTP.Port, objSMTP.EnableSSL, objSMTP.MailUsername, objSMTP.MailPassword, _tempEmployee.Email, ClsController.SinglePoint, EmailSubject.Trim(), EmailBody, "",2);
                }
                else if (_tempEmployee.EmailReminderType == 3 && sendReminder[0])
                {
                    Log4Net.WriteLog("Sending Reminder 3 email", LogType.GENERALLOG);
                    EmailBody = GetEmailBody_OverDueCourseTemplate3(_tempEmployee);
                    emailsent = EmailCls.SendEmail(objSMTP.MailServer, objSMTP.Port, objSMTP.EnableSSL, objSMTP.MailUsername, objSMTP.MailPassword, _tempEmployee.Email, ClsController.SinglePoint, EmailSubject.Trim(), EmailBody, "",3);
                }
                else if (_tempEmployee.EmailReminderType == 99 && sendReminder[1])
                {
                    Log4Net.WriteLog("Sending compliance courses email: " + _tempEmployee.assignedCourses.Count, LogType.GENERALLOG);

                    foreach (var courses in _tempEmployee.assignedCourses)
                    {
                        EmailBody = GetEmailBody_OneMonthComplianceTemplate(_tempEmployee, courses.CourseName);
                        emailsent = EmailCls.SendEmail(objSMTP.MailServer, objSMTP.Port, objSMTP.EnableSSL, objSMTP.MailUsername, objSMTP.MailPassword, _tempEmployee.Email, ClsController.SinglePoint, EmailSubject.Trim(), EmailBody, "",99);
                    }
                }

                if (emailsent)
                {
                    Log4Net.WriteLog("Updating reminders status", LogType.GENERALLOG);
                    UpdateReminderStatus(ClsDBConnInfo.DBConnectionString_LMS, _tempEmployee);
                }
            }
            catch (Exception ex)
            {
                Log4Net.WriteException(ex);
            }

        }


        #region Omer COde
        /*
        public void Process_TriggerType_OverDueTask_LMS_Alert(csSMTPMailEntity objSMTP, string EmployeeDB)
        {
            try
            {
                Collection<Employee> employeeList = CsSystemMonitorData.Get_Employees(ClsDBConnInfo.DBConnectionString_LMS, 0, EmployeeDB);
                Log4Net.WriteLog("Total Courses Count:" + employeeList.Count, LogType.GENERALLOG);



                int UserID = 0;
                int EmailReminderType = 0;
                string EmailTo = "";
                //string EmailBody = "";
                //string EmailSubject = "";
                DataBaseHandler.AssignedCourses objAC;
                Employee tempEmployee = new Employee();
                Employee _tempEmployee = new Employee();

                foreach (var employee in employeeList)
                {
                    try
                    {
                        if (UserID == 0)
                        {
                            EmailTo = employee.Email;
                            UserID = employee.EmployeeID;
                            EmailReminderType = employee.EmailReminderType;
                            employee.assignedCourses = new List<AssignedCourses>();

                            _tempEmployee.Email = employee.Email;
                            _tempEmployee.EmployeeID = employee.EmployeeID;
                            _tempEmployee.FirstName = employee.FirstName;
                            _tempEmployee.LastName = employee.LastName;
                            _tempEmployee.AssignedCourseId = employee.AssignedCourseId;
                            _tempEmployee.CourseName = employee.CourseName;
                            _tempEmployee.assignedCourses = new List<AssignedCourses>();
                            _tempEmployee.EmailReminderType = employee.EmailReminderType;
                            _tempEmployee.RetakeDate = employee.RetakeDate;
                            _tempEmployee.isComplete = employee.isComplete;

                        }

                        if (UserID == employee.EmployeeID && EmailReminderType == employee.EmailReminderType)
                        {
                            if (((_tempEmployee.EmailReminderType == 1 || _tempEmployee.EmailReminderType == 2 || _tempEmployee.EmailReminderType == 3) && _tempEmployee.isComplete != 1) || (_tempEmployee.EmailReminderType == 99 && _tempEmployee.isComplete == 1))


                            {



                                objAC = new AssignedCourses();


                                objAC.AsignedCourseID = employee.AssignedCourseId;
                                objAC.CourseName = employee.CourseName;


                                //  employee.assignedCourses.Add(objAC);

                                _tempEmployee.assignedCourses.Add(objAC);


                            }






                        }
                        else
                        {

                            //string Courses = string.Join<string>(", ",employee.assignedCourses.Select(x=>x.CourseName).ToList());
                            //email

                            if (EmailReminderType > 0)
                            {
                                if (_tempEmployee.EmailReminderType == 1)
                                {
                                    EmailBody = GetEmailBody_OverDueCourseTemplate1(_tempEmployee);
                                    //  EmailSubject = "Reminder 1";
                                }
                                else if (_tempEmployee.EmailReminderType == 2)
                                {
                                    EmailBody = GetEmailBody_OverDueCourseTemplate2(_tempEmployee);
                                    //  EmailSubject = "Reminder 2";
                                }
                                else if (_tempEmployee.EmailReminderType == 3)
                                {
                                    EmailBody = GetEmailBody_OverDueCourseTemplate3(_tempEmployee);
                                    //  EmailSubject = "Reminder 3";
                                }
                                //else if (_tempEmployee.EmailReminderType == 99)
                                //{
                                //    EmailBody = GetEmailBody_OneMonthComplianceTemplate(_tempEmployee);
                                //    EmailSubject = "Compliance";
                                //}

                                bool emailsent = false;
                                if (_tempEmployee.EmailReminderType == 99 && _tempEmployee.isComplete == 1)
                                {


                                    Log4Net.WriteLog("Compliance Course Name:", LogType.GENERALLOG);


                                    string coursename = "";
                                    foreach (var courses in _tempEmployee.assignedCourses)
                                    {
                                        coursename = courses.CourseName;

                                        EmailBody = GetEmailBody_OneMonthComplianceTemplate(_tempEmployee, coursename);
                                        //  EmailSubject = "Compliance";

                                        sendReminder = CsSystemMonitorData.Get_TrainingCourseReminderSettings(ClsDBConnInfo.DBConnectionString_LMS, ClsController.TrainingCourseCompanyIds);
                                        if (sendReminder[1])
                                        {
                                            emailsent = EmailCls.SendEmail(objSMTP.MailServer, objSMTP.Port, objSMTP.EnableSSL, objSMTP.MailUsername, objSMTP.MailPassword, EmailTo, ClsController.SinglePoint, EmailSubject.Trim(), EmailBody, "");
                                        }


                                    }


                                }



                                if ((_tempEmployee.EmailReminderType == 1 || _tempEmployee.EmailReminderType == 2 || _tempEmployee.EmailReminderType == 3) && _tempEmployee.isComplete != 1)

                                {
                                    sendReminder = CsSystemMonitorData.Get_TrainingCourseReminderSettings(ClsDBConnInfo.DBConnectionString_LMS, ClsController.TrainingCourseCompanyIds);

                                    if (sendReminder[0])
                                    {

                                        emailsent = EmailCls.SendEmail(objSMTP.MailServer, objSMTP.Port, objSMTP.EnableSSL, objSMTP.MailUsername, objSMTP.MailPassword, EmailTo, ClsController.SinglePoint, EmailSubject.Trim(), EmailBody, "");
                                    }
                                }
                                //if(employee.EmailReminderType > 0 && employee.EmailReminderType != 99)
                                //{

                                if (emailsent)
                                {
                                    UpdateReminderStatus(ClsDBConnInfo.DBConnectionString_LMS, _tempEmployee);
                                }


                                //}
                            }

                            objAC = new AssignedCourses();

                            // employee.assignedCourses = new List<AssignedCourses>();
                            // employee.assignedCourses.Add(objAC);

                            UserID = employee.EmployeeID;
                            EmailReminderType = employee.EmailReminderType;
                            EmailTo = employee.Email;

                            _tempEmployee.FirstName = employee.FirstName;
                            _tempEmployee.LastName = employee.LastName;
                            _tempEmployee.assignedCourses = new List<AssignedCourses>();





                            _tempEmployee.EmployeeID = employee.EmployeeID;
                            _tempEmployee.EmailReminderType = employee.EmailReminderType;
                            _tempEmployee.Email = employee.Email;
                            _tempEmployee.RetakeDate = employee.RetakeDate;
                            _tempEmployee.isComplete = employee.isComplete;



                            //  if (employee.EmailReminderType > 0 && employee.EmailReminderType != 99 && employee.isComplete != 1)
                            // {
                            objAC.AsignedCourseID = employee.AssignedCourseId;
                            objAC.CourseName = employee.CourseName;
                            _tempEmployee.assignedCourses.Add(objAC);
                            //  }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log4Net.WriteException(ex);
                    }
                    tempEmployee = employee;
                }


                ///if employee.assignedCourse still have 1 course

                if (employeeList.Count > 0)
                {
                    if (_tempEmployee.assignedCourses.Count > 0)
                    {
                        if (_tempEmployee.EmailReminderType == 1)
                        {
                            EmailBody = GetEmailBody_OverDueCourseTemplate1(_tempEmployee);
                            //  EmailSubject = "Reminder 1";
                        }
                        else if (_tempEmployee.EmailReminderType == 2)
                        {
                            EmailBody = GetEmailBody_OverDueCourseTemplate2(_tempEmployee);
                            //   EmailSubject = "Reminder 2";
                        }
                        else if (_tempEmployee.EmailReminderType == 3)
                        {
                            EmailBody = GetEmailBody_OverDueCourseTemplate3(_tempEmployee);
                            //   EmailSubject = "Reminder 3";
                        }
                        //else if (EmailReminderType == 99)
                        //{
                        //    EmailBody = GetEmailBody_OneMonthComplianceTemplate(_tempEmployee);
                        //    EmailSubject = "Compliance";
                        //}
                        bool emailsent = false;
                        if (_tempEmployee.EmailReminderType == 99 && _tempEmployee.isComplete == 1)
                        {
                            Log4Net.WriteLog("Compliance Course Name:", LogType.GENERALLOG);

                            string coursename = "";
                            foreach (var courses in _tempEmployee.assignedCourses)
                            {
                                coursename = courses.CourseName;
                            }
                            EmailBody = GetEmailBody_OneMonthComplianceTemplate(_tempEmployee, coursename);
                            //    EmailSubject = "Compliance";
                            sendReminder = CsSystemMonitorData.Get_TrainingCourseReminderSettings(ClsDBConnInfo.DBConnectionString_LMS, ClsController.TrainingCourseCompanyIds);

                            if (sendReminder[1])
                            {
                                emailsent = EmailCls.SendEmail(objSMTP.MailServer, objSMTP.Port, objSMTP.EnableSSL, objSMTP.MailUsername, objSMTP.MailPassword, EmailTo, ClsController.SinglePoint, EmailSubject, EmailBody, "");

                            }



                        }

                        if ((_tempEmployee.EmailReminderType == 1 || _tempEmployee.EmailReminderType == 2 || _tempEmployee.EmailReminderType == 3) && _tempEmployee.isComplete != 1)
                        {
                            sendReminder = CsSystemMonitorData.Get_TrainingCourseReminderSettings(ClsDBConnInfo.DBConnectionString_LMS, ClsController.TrainingCourseCompanyIds);

                            if (sendReminder[0])
                            {

                                emailsent = EmailCls.SendEmail(objSMTP.MailServer, objSMTP.Port, objSMTP.EnableSSL, objSMTP.MailUsername, objSMTP.MailPassword, EmailTo, ClsController.SinglePoint, EmailSubject.ToString(), EmailBody, "");
                            }
                        }


                        if (emailsent)
                        {
                            UpdateReminderStatus(ClsDBConnInfo.DBConnectionString_LMS, _tempEmployee);
                        }
                    }
                }







            }
            catch (Exception ex)
            {
                Log4Net.WriteException(ex);
            }

        }
        */
# endregion
        public string GetEmailBody_OverDueCourseTemplate1(Employee employee)
        {
            StringBuilder emailBody = new StringBuilder();

            string Body = "";

            try
            {
         

                Collection<CsCourseAlertData> AsignedCourses = CsCourseAlertData.Get_AsignedCourseAlertData(ClsDBConnInfo.DBConnectionString_LMS, employee.EmployeeID.ToString(), ClsController.EmployeeDB);

          //      LogApp.Log4Net.WriteLog("Reminder 1 Incomplete Courses:" + string.Join<string>(", ", employee.assignedCourses.Select(x => x.CourseName).ToList()), LogType.GENERALLOG);

                getEmailConfiguration(ClsDBConnInfo.DBConnectionString_LMS, "1st Reminder");

                

                Body = EmailBody;
                Body = Body.Replace("[Email To]", employee.FirstName + ", " + employee.LastName);

              

                Body = Body.Replace("[Course List Assigned]", string.Join<string>(", ", AsignedCourses.Select(x => x.Name).ToList()));

              

                Log4Net.WriteLog("Assigned Courses:" + string.Join<string>(", ", AsignedCourses.Select(x => x.Name).ToList()), LogType.GENERALLOG);

           

                Body = Body.Replace("[Course List Incomplete]", string.Join<string>(", ", employee.assignedCourses.Select(x => x.CourseName).ToList()));



                Get_TrainingCourse_Company_Officer(ClsDBConnInfo.DBConnectionString_LMS, ClsController.TrainingCourseCompanyIds);


               


                Body = Body.Replace("[Training Officer Name]", TrainingOfficer);
                Body = Body.Replace("[Company Name]", CompanyName);

            }
            catch (Exception ex)
            {
                Log4Net.WriteException(ex);
            }
            return Body.ToString();
        }

        public string GetEmailBody_OverDueCourseTemplate2(Employee employee)
        {
            StringBuilder emailBody = new StringBuilder();

            string Body = "";

            try
            {
               

                Collection<CsCourseAlertData> AsignedCourses = CsCourseAlertData.Get_AsignedCourseAlertData(ClsDBConnInfo.DBConnectionString_LMS, employee.EmployeeID.ToString(), ClsController.EmployeeDB);

                getEmailConfiguration(ClsDBConnInfo.DBConnectionString_LMS, "2nd Reminder");
                Body = EmailBody;
                Body = Body.Replace("[Email To]", employee.FirstName + ", " + employee.LastName);



                Body = Body.Replace("[Course List Assigned]", string.Join<string>(", ", AsignedCourses.Select(x => x.Name).ToList()));



                Log4Net.WriteLog("Assigned Courses:" + string.Join<string>(", ", AsignedCourses.Select(x => x.Name).ToList()), LogType.GENERALLOG);



                Body = Body.Replace("[Course List Incomplete]", string.Join<string>(", ", employee.assignedCourses.Select(x => x.CourseName).ToList()));



                Get_TrainingCourse_Company_Officer(ClsDBConnInfo.DBConnectionString_LMS, ClsController.TrainingCourseCompanyIds);





                Body = Body.Replace("[Training Officer Name]", TrainingOfficer);
                Body = Body.Replace("[Company Name]", CompanyName);

            }
            catch (Exception ex)
            {
                Log4Net.WriteException(ex);
            }
            return Body.ToString();
        }
        public string GetEmailBody_OverDueCourseTemplate3(Employee employee)
        {
            StringBuilder emailBody = new StringBuilder();

            string Body = "";

            try
            {
              

                Collection<CsCourseAlertData> AsignedCourses = CsCourseAlertData.Get_AsignedCourseAlertData(ClsDBConnInfo.DBConnectionString_LMS, employee.EmployeeID.ToString(), ClsController.EmployeeDB);


                getEmailConfiguration(ClsDBConnInfo.DBConnectionString_LMS, "Final Reminder");
                Body = EmailBody;
                Body = Body.Replace("[Email To]", employee.FirstName + ", " + employee.LastName);



                Body = Body.Replace("[Course List Assigned]", string.Join<string>(", ", AsignedCourses.Select(x => x.Name).ToList()));



                Log4Net.WriteLog("Assigned Courses:" + string.Join<string>(", ", AsignedCourses.Select(x => x.Name).ToList()), LogType.GENERALLOG);



                Body = Body.Replace("[Course List Incomplete]", string.Join<string>(", ", employee.assignedCourses.Select(x => x.CourseName).ToList()));



                Get_TrainingCourse_Company_Officer(ClsDBConnInfo.DBConnectionString_LMS, ClsController.TrainingCourseCompanyIds);





                Body = Body.Replace("[Training Officer Name]", TrainingOfficer);
                Body = Body.Replace("[Company Name]", CompanyName);




            }
            catch (Exception ex)
            {
                Log4Net.WriteException(ex);
            }
            return Body.ToString();
        }

       

        public void UpdateReminderStatus(string ConStr, Employee employee)
        {

            string CommandText = "";
            string AssignedCoursesId = string.Join<int>(", ", employee.assignedCourses.Select(x => x.AsignedCourseID).ToList());


            Log4Net.WriteLog("Employee Reminder Type:" + employee.EmailReminderType, LogType.GENERALLOG);


            if (employee.EmailReminderType == 99)
            {
               // CommandText = "update assignedCourses set reminderStatus=0,assignedcoursedate='" + employee.RetakeDate + "' where id in (" + AssignedCoursesId + ")";
                CommandText = "update assignedCourses set reminderStatus=0,assignedcoursedate=getdate() where id in (" + AssignedCoursesId + ")";
            }
            else
            {
                CommandText = "update assignedCourses set reminderStatus=" + employee.EmailReminderType + " where id in (" + AssignedCoursesId + ")";
            }

           

            SqlCommand cmd = new SqlCommand(CommandText);
         
            DataSet ds = new DataSet();
            
            using (SqlConnection conn = new SqlConnection(ConStr))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                cmd.CommandTimeout = 1;

              

                cmd.Connection = conn;
                //   result = adpt.Fill(ds);


                cmd.ExecuteNonQuery();


                if (conn.State == ConnectionState.Open)
                    conn.Close();


               
            }

        }



        public void Get_TrainingCourse_Company_Officer(string ConnectionString,int tcId)
        {
            int AccountId = 0;
            try
            {

                string Query = "select Name,TraniningOfficerName from Settings s inner join Companies c on s.CompanyID = c.ID where c.ID = "+tcId;
                


                DataSet ds = (DataSet)clsGeneralDBQeueries.GetData(ConnectionString, Query);
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0)
                {

                    try
                    {

                        CompanyName  = DBNull.Value.Equals(ds.Tables[0].Rows[0][0].ToString()) ? "" : ds.Tables[0].Rows[0][0].ToString();
                        TrainingOfficer = DBNull.Value.Equals(ds.Tables[0].Rows[0][1].ToString()) ? "" : ds.Tables[0].Rows[0][1].ToString();

                    }
                    catch (Exception ex)
                    {
                        LogApp.Log4Net.WriteException(ex);
                    }

                }
            }
            catch (Exception ex)
            {
                LogApp.Log4Net.WriteException(ex);
            }
         
        }


        public string GetEmailBody_OneMonthComplianceTemplate(Employee employee,string courseName)
        {

            StringBuilder emailBody = new StringBuilder();

            string Body = "";

            try
            {

                //Log4Net.WriteLog("Compliance Course Name:" + courseName, LogType.GENERALLOG);


               

                getEmailConfiguration(ClsDBConnInfo.DBConnectionString_LMS, "Compliance");
                Body = EmailBody;
                Body = Body.Replace("[Email To]", employee.FirstName + ", " + employee.LastName);

                Body = Body.Replace("[Course Title]", courseName);


                Get_TrainingCourse_Company_Officer(ClsDBConnInfo.DBConnectionString_LMS, ClsController.TrainingCourseCompanyIds);


               



                Body = Body.Replace("[Training Officer Name]", TrainingOfficer);
                Body = Body.Replace("[Company Name]", CompanyName);



            }
            catch (Exception ex)
            {
                Log4Net.WriteException(ex);
            }
            return Body.ToString();

        }




       public void getEmailConfiguration(string ConnectionString,string Reminder)
        {

            try
            {

              //  LogApp.Log4Net.WriteLog("Reminder:"+Reminder, LogType.GENERALLOG);

                int companyId = ClsController.TrainingCourseCompanyIds;

                string Query = "select subject ,body,title from EmailConfigurations where  companyid="+companyId + " and trim(title)=" + "'" + Reminder + "'";
                DataSet ds = (DataSet)clsGeneralDBQeueries.GetData(ConnectionString, Query);
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0)
                {

                    try
                    {

                        EmailSubject = DBNull.Value.Equals(ds.Tables[0].Rows[0][0].ToString()) ? "" : ds.Tables[0].Rows[0][0].ToString();
                        EmailBody = DBNull.Value.Equals(ds.Tables[0].Rows[0][1].ToString()) ? "" : ds.Tables[0].Rows[0][1].ToString();
                        Title = DBNull.Value.Equals(ds.Tables[0].Rows[0][2].ToString()) ? "" : ds.Tables[0].Rows[0][2].ToString();

                    }
                    catch (Exception ex)
                    {
                        LogApp.Log4Net.WriteException(ex);
                    }

                }

            }
            catch(Exception ex) { LogApp.Log4Net.WriteException(ex); }

           

        }






    }
}
