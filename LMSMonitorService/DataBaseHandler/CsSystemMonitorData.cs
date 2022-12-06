using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
namespace LMSMonitorService.DataBaseHandler
{
    class CsSystemMonitorData
    {


        public static Collection<int> Get_AccountIds(string ConnectionString)
        {
            Collection<int> list = new Collection<int>();
            try
            {

                string Query = "SeLECT AccountId FROM Accounts ";
                Query = Query + "WHERE  IsDeleted = 0 AND Active = 1 ";


                DataSet ds = (DataSet)clsGeneralDBQeueries.GetData(ConnectionString, Query);
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        try
                        {
                            int accountId = 0;
                            accountId = DBNull.Value.Equals(row["AccountId"]) ? 0 : Convert.ToInt32(row["AccountId"]);
                            list.Add(accountId);
                        }
                        catch (Exception ex)
                        {
                            LogApp.Log4Net.WriteException(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogApp.Log4Net.WriteException(ex);
            }
            return list;
        }

        public static int Get_TrainingCourseCompanyIds(string ConnectionString,int accountId)
        {
            int AccountId = 0;
            try
            {

                string Query = "SeLECT TrainingCoursesCompanyId FROM Accounts ";
                Query = Query + "WHERE  IsDeleted = 0 AND Active = 1 and accountid="+accountId;


                DataSet ds = (DataSet)clsGeneralDBQeueries.GetData(ConnectionString, Query);
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0)
                {
                  
                    try
                        {
                           
                            AccountId = DBNull.Value.Equals(ds.Tables[0].Rows[0][0].ToString()) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString());
                           
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
            return AccountId;
        }




        public static bool[] Get_TrainingCourseReminderSettings(string ConnectionString, int CompanyId)
        {
            bool []Reminder = { false ,false};
            bool Compliance = false;
            try
            {

                string Query = "select EmailReminder,EmailCompliance from Settings where CompanyID= " + CompanyId;
              //  Query = Query + "WHERE  IsDeleted = 0 AND Active = 1 and accountid=" + accountId;


                DataSet ds = (DataSet)clsGeneralDBQeueries.GetData(ConnectionString, Query);
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0)
                {

                    try
                    {

                        Reminder[0] = DBNull.Value.Equals(ds.Tables[0].Rows[0][0].ToString()) ? false : Convert.ToBoolean(ds.Tables[0].Rows[0][0].ToString());
                        Reminder[1] = DBNull.Value.Equals(ds.Tables[0].Rows[0][1].ToString()) ? false : Convert.ToBoolean(ds.Tables[0].Rows[0][1].ToString());

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
            return Reminder;
        }




        public static Collection<Employee> Get_Employees(string ConnectionString, int SystemMonitorId, string EmployeeDB )
        {
            Collection<Employee> list = new Collection<Employee>();
            try
            {


                string Query;
                Query = "select ID,[First Name] ,[Last Name] ,Email, CourseName, EmailReminderType, RetakeDate, AssignedCourseId,iscomplete";
                Query = Query + " from(";
                Query = Query + " select";
                Query = Query + " TCUserId as ID,[First Name],[Last Name], Email, c.Name as CourseName, ReminderStatus, uc.RetakeDate, ac.Id AssignedCourseId,iscomplete,";
                Query = Query + "      case when datediff(week, DATEADD(week, s.TrainingCoursesWeeks , ac.AssignedCourseDate), GETDATE()) > DaysReminder3+DaysReminder1+DaysReminder2 AND ReminderStatus = 2 THEN 3";
                Query = Query + " when datediff(week, DATEADD(week, s.TrainingCoursesWeeks , ac.AssignedCourseDate), GETDATE()) > DaysReminder2+DaysReminder1  AND ReminderStatus = 1 THEN 2 ";
                Query = Query + " when datediff(week, DATEADD(week, s.TrainingCoursesWeeks , ac.AssignedCourseDate), GETDATE()) > DaysReminder1 AND ReminderStatus = 0 THEN 1 ";
              
                Query = Query + " ELSE 0 END as EmailReminderType,";
                Query = Query + " s.DaysReminder1, s.DaysReminder2, s.DaysReminder3";
                Query = Query + " from AssignedCourses ac ";
                Query = Query + " inner";
                Query = Query + " join "+EmployeeDB+".dbo.Users u on u.TCUserId = ac.UserID";
                Query = Query + " inner";
                Query = Query + " join Settings s on s.CompanyID = ac.CompanyID";
                Query = Query + " inner";
                Query = Query + " join Courses c on c.ID = ac.CourseID";
                Query = Query + " left outer join UserCourses uc on ac.UserID = uc.UserID and ac.CourseID = uc.CourseID";
                Query = Query + " where  u.IsDeleted = 0 and u.IsActive = 1 and (IsComplete<>1 or IsComplete is null) ";
                Query = Query + " and datediff(week, DATEADD(week, s.TrainingCoursesWeeks , ac.AssignedCourseDate), GETDATE()) >= s.DaysReminder1";
                Query = Query + " union";
                Query = Query + " select";
                Query = Query + " TCUserId as ID,[First Name],[Last Name], Email, c.Name as CourseName, ReminderStatus, uc.RetakeDate, ac.Id AssignedCourseId, iscomplete,               " ;
                Query = Query + " ReminderStatus as EmailReminderType,  s.DaysReminder1, s.DaysReminder2, s.DaysReminder3  ";
                Query = Query + "  from AssignedCourses ac  inner join "+ EmployeeDB + ".dbo.Users u on u.TCUserId = ac.UserID inner  join Settings s on s.CompanyID = ac.CompanyID ";
                Query = Query + "  inner  join Courses c on c.ID = ac.CourseID left outer join UserCourses uc on ac.UserID = uc.UserID and ac.CourseID = uc.CourseID";
                Query = Query + "  where u.IsDeleted = 0 and u.IsActive = 1 and  cast( uc.RetakeDate as date)= cast(dateadd(month,1, getdate() )as date)  and ReminderStatus = 99";
                Query = Query + "   and RetakeDuration>30 and IsComplete=1   ) A";
                Query = Query + " order by ID, EmailReminderType, CourseName";




















                DataSet ds = (DataSet)clsGeneralDBQeueries.GetData(ConnectionString, Query);
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        try
                        {
                            Employee employee = new Employee();
                            employee.EmployeeID = DBNull.Value.Equals(row["ID"]) ? 0 : Convert.ToInt32(row["ID"]);
                            employee.FirstName = DBNull.Value.Equals(row["First Name"]) ? string.Empty : row["First Name"].ToString();
                            employee.LastName = DBNull.Value.Equals(row["Last Name"]) ? string.Empty : row["Last Name"].ToString();
                            employee.Email = DBNull.Value.Equals(row["Email"]) ? string.Empty : row["Email"].ToString();
                            employee.CourseName = DBNull.Value.Equals(row["CourseName"]) ? string.Empty : row["CourseName"].ToString();
                            employee.EmailReminderType = DBNull.Value.Equals(row["EmailReminderType"]) ? 0 :Convert.ToInt32( row["EmailReminderType"]);
                            //employee.RetakeDate = DBNull.Value.Equals(row["RetakeDate"]) ? System.DateTime.Now : Convert.ToDateTime(row["RetakeDate"]);
                            employee.AssignedCourseId = DBNull.Value.Equals(row["AssignedCourseId"]) ? 0 : Convert.ToInt32(row["AssignedCourseId"]);
                            employee.isComplete = DBNull.Value.Equals(row["isComplete"]) ? 0 : Convert.ToInt32(row["isComplete"]);
                            //   employee.ProfileImage = DBNull.Value.Equals(row["ProfileImage"]) ? string.Empty : row["ProfileImage"].ToString();

                            list.Add(employee);
                        }
                        catch (Exception ex)
                        {
                            LogApp.Log4Net.WriteException(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogApp.Log4Net.WriteException(ex);
            }
            return list;
        }

        //public static Collection<AsignedCourses> Get_UserCourses(string ConnectionString, int EMPID, string EmployeeDB)
        //{
        //    Collection<AsignedCourse> list = new Collection<AsignedCourse>();
        //    try
        //    {


        //        string Query;
        //        Query = "select  ac.ID as AsignedCourseID,ReminderStatus,retakedate";
        //        Query = Query + " from AssignedCourses ac";

        //        Query = Query + " inner join UserCourses uc ";
        //        Query = Query + " on ac.UserID = uc.UserID and ac.CourseID = uc.CourseID";
        //        Query = Query + " inner join " + EmployeeDB + ".dbo.Users u";
        //        Query = Query + " on u.TCUserId = ac.UserID";
        //        Query = Query + " inner join Settings s";
        //        Query = Query + " on s.CompanyID = ac.CompanyID";
        //        Query = Query + " and IsComplete = 0";
        //        Query = Query + " and u.IsDeleted = 0";
        //        Query = Query + " and u.IsActive = 1";
        //      //  Query = Query + " and ReminderStatus = 0";
        //      //  Query = Query + " and datediff(day, DATEADD(day,28, AssignedCourseDate ),getdate())= s.DaysReminder1";

        //        Query = Query + " and u.TCUserId ="+EMPID;







        //        DataSet ds = (DataSet)clsGeneralDBQeueries.GetData(ConnectionString, Query);
        //        if (ds != null && ds.Tables != null && ds.Tables.Count > 0)
        //        {
        //            foreach (DataRow row in ds.Tables[0].Rows)
        //            {
        //                try
        //                {
        //                    AsignedCourse asignedCourse= new AsignedCourse();
        //                    asignedCourse.AsignedCourseID = DBNull.Value.Equals(row["AsignedCourseID"]) ? 0 : Convert.ToInt32(row["AsignedCourseID"]);
        //                    asignedCourse.ReminderStatus = DBNull.Value.Equals(row["ReminderStatus"]) ? 0 : Convert.ToInt32(row["ReminderStatus"]);
        //                    asignedCourse.AssignedCourseDate = DBNull.Value.Equals(row["retakedate"]) ? System.DateTime.Now : Convert.ToDateTime(row["retakedate"]);



        //                    //   employee.ProfileImage = DBNull.Value.Equals(row["ProfileImage"]) ? string.Empty : row["ProfileImage"].ToString();

        //                    list.Add(asignedCourse);
        //                }
        //                catch (Exception ex)
        //                {
        //                    LogApp.Log4Net.WriteException(ex);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogApp.Log4Net.WriteException(ex);
        //    }
        //    return list;
        //}


        public static int GetReminderStatus(string connectionString, string commandText)
        {
            SqlCommand cmd = new SqlCommand(commandText);
            SqlDataAdapter adpt = new SqlDataAdapter();
            DataSet ds = new DataSet();
            int reminderStatus = 0;
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





        public static Collection<Employee> Get_OneMonthComplianceEmployees(string ConnectionString, int SystemMonitorId, string EmployeeDB)
        {
            Collection<Employee> list = new Collection<Employee>();
            try
            {


                string Query;
                //Query = "select distinct TCUserId as ID,[First Name] ,[Last Name] ,Email";
                //Query = Query + " from AssignedCourses ac inner join " + EmployeeDB + ".dbo.Users u on u.TCUserId = ac.UserID";

                //Query = Query + " inner join Settings s on s.CompanyID = ac.CompanyID";
                //Query = Query + " left outer join UserCourses uc on ac.UserID = uc.UserID and ac.CourseID = uc.CourseID";
                //Query = Query + " where IsComplete = 0 and u.IsDeleted = 0 and u.IsActive = 1 and ReminderStatus = 0";
                //Query = Query + " and datediff(day, DATEADD(day,TrainingCoursesWeeks*7, AssignedCourseDate ),cast (getdate() as date))>= s.DaysReminder1";
                //Query = Query + " union";

                Query = "select TCUserId as ID,[First Name],[Last Name], Email, Name";
                Query = Query + " from AssignedCourses ac inner";
                Query = Query + " join UserCourses uc";
                Query = Query + " on ac.CourseID = uc.CourseID and ac.UserID = uc.UserID";
                Query = Query + " inner join courses c on c.ID = uc.CourseID" ;
                Query = Query + " inner join " + EmployeeDB + ".dbo.Users u";
                Query = Query + " on u.tcuserid = uc.UserID";
                     Query = Query + " and cast(DATEADD(month,-1 ,RetakeDate ) as date)= cast(GETDATE() as date) ";








                DataSet ds = (DataSet)clsGeneralDBQeueries.GetData(ConnectionString, Query);
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        try
                        {
                            Employee employee = new Employee();
                            employee.EmployeeID = DBNull.Value.Equals(row["ID"]) ? 0 : Convert.ToInt32(row["ID"]);
                            employee.FirstName = DBNull.Value.Equals(row["First Name"]) ? string.Empty : row["First Name"].ToString();
                            employee.LastName = DBNull.Value.Equals(row["Last Name"]) ? string.Empty : row["Last Name"].ToString();
                            employee.Email = DBNull.Value.Equals(row["Email"]) ? string.Empty : row["Email"].ToString();
                            employee.CourseName = DBNull.Value.Equals(row["Name"]) ? string.Empty : row["Name"].ToString();
                            //   employee.ProfileImage = DBNull.Value.Equals(row["ProfileImage"]) ? string.Empty : row["ProfileImage"].ToString();

                            list.Add(employee);
                        }
                        catch (Exception ex)
                        {
                            LogApp.Log4Net.WriteException(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogApp.Log4Net.WriteException(ex);
            }
            return list;
        }

    }



    public class AssignedCourses
    {
        public string CourseName = "";
        public int AsignedCourseID = 0;
        //public int ReminderStatus = 0;
        //public DateTime AssignedCourseDate = DateTime.Now;
    }



    public class Employee
    {
        public int EmployeeID = 0;
        public string FirstName = string.Empty;
        public string LastName = string.Empty;
        public string Email = string.Empty;
        public string CourseName = string.Empty;
        public int EmailReminderType = 0;
        public DateTime RetakeDate = DateTime.Now;
         public int AssignedCourseId =0;
        public  List<AssignedCourses> assignedCourses;
        public int isComplete = 0;
        //  public int OverDueCourseCount = 0;
        //  public float OverDueCoursePercentage = 0;
        //  public string ProfileImage = string.Empty;

        //public Employee()
        //{
        //}
    }
}
