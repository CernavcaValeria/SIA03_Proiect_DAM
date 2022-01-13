using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAMS_Proiect
{
    public class UpdateData
    {
        public string ConnString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\DAMS\DataBase\DAMS_DataBase.mdf;Integrated Security=True";
        public SqlConnection Conn;
        public SqlCommand Command;

        public UpdateData()
        {
            Conn = new SqlConnection
            {
                ConnectionString = ConnString
            };
            Command = new SqlCommand
            {
                Connection = Conn
            };
        }

        public bool UpdateCurrentProjectTaskListAfterSaveClick()
        {
            try
            {
                Command.Connection.Open();
                foreach (Task t in Entity.project.tasksList)
                {
                    Command.CommandText = "DELETE FROM  [dbo].[task] WHERE [taskId] = " + t.TaskId + " and [projectId] = " + Entity.project.Id + " and [userId] = " + Entity.user.Id;
                    Command.ExecuteNonQuery();
                }
                Command.Connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Command.Connection.Close();
                return false;
            }

            try
            {
                Command.Connection.Open();
                foreach (Task t in Entity.project.tasksList)
                {
                    Command.CommandText = "INSERT INTO[dbo].[task]([taskId], [taskName], [duration], [start], [finish], " +
                        "[resources], [priority], [complete], [projectId], [userId]) " +
                        "VALUES(" + t.TaskId + ", N' " + t.Name + "', " + t.Duration + ", N'" + t.Start.ToString("yyyy-MM-dd") + "', N'" + t.Finish.ToString("yyyy-MM-dd") + "'," +
                        " NULL, N'" + t.Priority + "', " + t.Complete + "," + Entity.project.Id + ", " + Entity.user.Id + ")";

                    Command.ExecuteNonQuery();
                }
                Command.Connection.Close();
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Command.Connection.Close();
                return false;
            }

            Command.Connection.Close();
            return true;
        }
    }
}
