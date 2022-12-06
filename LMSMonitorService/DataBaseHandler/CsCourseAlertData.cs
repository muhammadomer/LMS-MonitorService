using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Data;


namespace LMSMonitorService.DataBaseHandler
{
    class CsCourseAlertData
    {

        public string Name = string.Empty;
        public DateTime StartedOn = DateTime.Now;
        public DateTime CompleteOn = DateTime.Now;
        public static Collection<CsCourseAlertData> Get_OverDueCourseAlertData(string ConnectionString, string Filter, string EmployeeDB)
        {
            Collection<CsCourseAlertData> list = new Collection<CsCourseAlertData>();
            try
            {


                string Query;
                Query = "select ac.ID as id1, TCUserId as ID,[First Name] ,[Last Name] ,Email,ac.ReminderStatus,Name";
                Query = Query + " from AssignedCourses ac";

                Query = Query + " inner join UserCourses uc";
                Query = Query + " on ac.UserID = uc.UserID and ac.CourseID = uc.CourseID";
                Query = Query + " inner join "+ EmployeeDB + ".dbo.Users u";
                Query = Query + " on u.TCUserId = ac.UserID";
                Query = Query + " inner join Settings s";
                Query = Query + " on s.CompanyID = ac.CompanyID";
                Query = Query + " inner join Courses c on c.ID = ac.CourseID";
                Query = Query + " and IsComplete = 0";
                Query = Query + " and u.IsDeleted = 0";
                Query = Query + " and u.IsActive = 1";
             //   Query = Query + "and ReminderStatus = 0";
             //   Query = Query + " and datediff(day, DATEADD(day,28, AssignedCourseDate ),getdate())= s.DaysReminder1";
                Query = Query + " and u.TCUserId=" + Filter;

                DataSet ds = (DataSet)clsGeneralDBQeueries.GetData(ConnectionString, Query);


                if (ds != null && ds.Tables != null && ds.Tables.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {

                        CsCourseAlertData CourseAlert = new CsCourseAlertData();
                       CourseAlert .Name = DBNull.Value.Equals(row["Name"]) ? string.Empty : row["Name"].ToString();
                      //  CourseAlert.StartedOn= DBNull.Value.Equals(row["StartedOn"]) ? DateTime.Now : Convert.ToDateTime(row["StartedOn"]);
                      //  CourseAlert.CompleteOn = DBNull.Value.Equals(row["CompleteOn"]) ? DateTime.Now : Convert.ToDateTime(row["CompleteOn"]);
                        list.Add(CourseAlert);
                    }
                }

                    }
            catch(Exception ex)
            {

            }

            return list;
            }


        public static Collection<CsCourseAlertData> Get_AsignedCourseAlertData(string ConnectionString, string Filter, string EmployeeDB)
        {
            Collection<CsCourseAlertData> list = new Collection<CsCourseAlertData>();
            try
            {


                string Query;
                Query = "select ac.ID as id1, TCUserId as ID,[First Name] ,[Last Name] ,Email,ac.ReminderStatus,Name";
                Query = Query + " from AssignedCourses ac";

                Query = Query + " left outer join UserCourses uc";
                Query = Query + " on ac.UserID = uc.UserID and ac.CourseID = uc.CourseID";
                Query = Query + " inner join "+ EmployeeDB + ".dbo.Users u";
                Query = Query + " on u.TCUserId = ac.UserID";
                Query = Query + " inner join Settings s";
                Query = Query + " on s.CompanyID = ac.CompanyID";
                Query = Query + " inner join Courses c on c.ID = ac.CourseID";
             //   Query = Query + " and IsComplete = 0";
                Query = Query + " and u.IsDeleted = 0";
                Query = Query + " and u.IsActive = 1";
                //Query = Query + "and ReminderStatus = 0";
                //Query = Query + " and datediff(day, DATEADD(day,28, AssignedCourseDate ),getdate())= s.DaysReminder1";
                Query = Query + " and u.TCUserId=" + Filter;

                DataSet ds = (DataSet)clsGeneralDBQeueries.GetData(ConnectionString, Query);


                if (ds != null && ds.Tables != null && ds.Tables.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {

                        CsCourseAlertData CourseAlert = new CsCourseAlertData();
                        CourseAlert.Name = DBNull.Value.Equals(row["Name"]) ? string.Empty : row["Name"].ToString();
                        //  CourseAlert.StartedOn= DBNull.Value.Equals(row["StartedOn"]) ? DateTime.Now : Convert.ToDateTime(row["StartedOn"]);
                        //  CourseAlert.CompleteOn = DBNull.Value.Equals(row["CompleteOn"]) ? DateTime.Now : Convert.ToDateTime(row["CompleteOn"]);
                        list.Add(CourseAlert);
                    }
                }

            }
            catch (Exception ex)
            {

            }

            return list;
        }


    }
}
