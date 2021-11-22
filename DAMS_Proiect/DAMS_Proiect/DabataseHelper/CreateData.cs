using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAMS_Proiect
{
    public class CreateData
    {
        public string ConnString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\DAMS\DataBase\DAMS_DataBase.mdf;Integrated Security=True";
        public SqlConnection Conn;
        public SqlCommand Command;

        public CreateData()
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

        public bool RegisterUser(string name, string username, string password, string email,
                                string address, string phone, string departament)
        {
            bool succesInsertInUserTable;
            int availableUserId = SelectMaxIdValue("user") + 1;
            try
            {
                Command.Connection.Open();
                Command.CommandText = "INSERT INTO[dbo].[userTable] ([userId], [userName], [adress], [phoneNumber], [email], [departament]) " +
                    "VALUES( " + availableUserId + ", N'" + name + "', N'" + address + "', N'" + phone + "',N'" + email + "', N'" + departament + "')";
                Command.ExecuteNonQuery();
                succesInsertInUserTable = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Command.Connection.Close();
                return false;
            }
            Command.Connection.Close();

            bool succesInsertInAccountTable;
            try
            {
                int availableAccountId = SelectMaxIdValue("account") + 1;
                Command.Connection.Open();
                Command.CommandText = "INSERT INTO [dbo].[account] ([accountId], [accountName], [password], [userId])" +
                    " VALUES( " + availableAccountId + ", N'" + username + "', N'" + Encryptor.MD5Hash(password) + "', " + availableUserId + ")";
                Command.ExecuteNonQuery();
                succesInsertInAccountTable = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Command.Connection.Close();
                return false;
            }
            Command.Connection.Close();
            return succesInsertInUserTable && succesInsertInAccountTable;
        }

        private int SelectMaxIdValue(string table)
        {
            Command.Connection.Open();
            if (table.Equals("user"))
                Command.CommandText = "SELECT max(userId) FROM userTable";
            else if (table.Equals("account"))
                Command.CommandText = "SELECT max(accountId) FROM account";
            else if (table.Equals("project"))
                Command.CommandText = "SELECT max(projectId) FROM project";

            Int32 maxId = Convert.ToInt32(Command.ExecuteScalar());
            Command.Connection.Close();
            return maxId;
        }

        public bool CreateAndSaveNewProject(string projectName)
        {
            int availablepProjectId = SelectMaxIdValue("project") + 1;
            //insert project task in task table
            bool succesInsertProjectTasksInDB;
            try
            {
                Command.Connection.Open();
                foreach (Task t in Entity.project.tasksList)
                {
                    Command.CommandText = "INSERT INTO[dbo].[task]([taskId], [taskName], [duration], [start], [finish], " +
                        "[resources], [priority], [complete], [projectId], [userId]) " +
                        "VALUES(" + t.TaskId + ", N' " + t.Name + "', " + t.Duration + ", N'" + t.Start.ToString("yyyy-MM-dd") + "', N'" + t.Finish.ToString("yyyy-MM-dd") + "'," +
                        " NULL, N'" + t.Priority + "', " + t.Complete + "," + availablepProjectId + ", " + Entity.user.Id + ")";

                    Command.ExecuteNonQuery();
                }
                Entity.project.Id = availablepProjectId;
                succesInsertProjectTasksInDB = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Command.Connection.Close();
                return false;
            }
            Command.Connection.Close();


            //insert in project table
            bool succesInsertProjectInDB;
            try
            {
                Command.Connection.Open();
                Command.CommandText = "INSERT INTO [dbo].[project] ([projectId], [projectName], [userId]) " +
                    "VALUES (" + availablepProjectId + ", N'" + projectName + "'," + Entity.user.Id + ")";
                Command.ExecuteNonQuery();
                Entity.project.Id = availablepProjectId;
                succesInsertProjectInDB = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Command.Connection.Close();
                return false;
            }
            Command.Connection.Close();

            //insert in user table
            bool succesInsertUserProjectInDB;
            try
            {
                Command.Connection.Open();
                Command.CommandText = "INSERT INTO[dbo].[userTable]([userId], [userName], [adress], [phoneNumber], " +
                    "[resources], [email], [departament], [teamId], [projectId], [roleId]) " +
                    "VALUES(" + Entity.user.Id + ", N'" + Entity.user.Name + "', N'" + Entity.user.Addres + "', N'" + Entity.user.PhoneNumber + "', " +
                    "NULL, N'" + Entity.user.Email + "', N'" + Entity.user.UserDepartamanet.ToString() + "', "
                    + Entity.user.TeamId + ", " + availablepProjectId + "," + Entity.user.RoleId + ")";

                Command.ExecuteNonQuery();
                succesInsertUserProjectInDB = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Command.Connection.Close();
                return false;
            }

            Command.Connection.Close();
            return succesInsertProjectInDB && succesInsertUserProjectInDB && succesInsertProjectTasksInDB;
        }
    }
}
