using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DAMS_Proiect.Entity;
using static DAMS_Proiect.Convertor;

namespace DAMS_Proiect
{
    public class GetData
    {
        public string ConnString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\DAMS\DataBase\DAMS_DataBase.mdf;Integrated Security=True";
        public SqlConnection Conn;
        public SqlCommand Command;

        public GetData()
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

        public bool CheckUserNameExistence(string username)
        {
            Command.Connection.Open();
            Command.CommandText = "SELECT  userId from account " +
                                " where accountName like N'"+ username + "'";
            int userId = 0;
            try
            {
                using (SqlDataReader reader = Command.ExecuteReader())
                    while (reader.Read())
                        userId = (int)reader.GetDecimal(0);
            }
            catch (Exception)
            {
                Command.Connection.Close();
                return false;
            }
            Command.Connection.Close();

            if(userId>0)
            {
                Entity.account = new Account();
                account.UserName = username;
                account.UserId = userId;

                Command.Connection.Open();
                Command.CommandText = "select [userName], [adress], [phoneNumber]," +
                    "[email], [departament], [teamId], [roleId]  from [dbo].[userTable]" +
                    " where userId= " + userId;
                string name = "";
                string address = "";
                string phone = "";
                string mail = "";
                string departament = "";
                int teamid = 1;
                int roleid = 0;
                int counter = 0;
                try
                {
                    using (SqlDataReader reader = Command.ExecuteReader())
                        while (reader.Read())
                        {
                            if (counter > 0) break;
                            name = reader.GetString(0);
                            address = reader.GetString(1);
                            phone = reader.GetString(2);
                            mail = reader.GetString(3);
                            departament = reader.GetString(4);
                            try
                            {
                                teamid = (int)reader.GetDecimal(5);
                            }
                            catch(Exception)
                            {
                                teamid = 0;
                            }
                            try
                            {
                                roleid = (int)reader.GetDecimal(6);
                            }
                            catch (Exception)
                            {
                                //ex
                            }
                            counter++;
                        }
                    if (counter > 0)
                    {
                        User tempUser = new User();
                        tempUser.Id = userId;
                        tempUser.Name = name;
                        tempUser.Addres = address;
                        tempUser.PhoneNumber = phone;
                        tempUser.Email = mail;
                        tempUser.UserDepartamanet = User.GetDepartamanetByName(departament);
                        tempUser.RoleId = roleid;
                        tempUser.TeamId = teamid;
                        Entity.user = tempUser;
                        Command.Connection.Close();
                        return true;
                    }
                }
                catch (Exception)
                {
                    Command.Connection.Close();
                    return false;
                }
            }
            Command.Connection.Close();
            return false;
        }

        public bool ValidatePassword(string password,string username)
        {
            var encrPassword = Encryptor.MD5Hash(password);
            Command.Connection.Open();
            Command.CommandText = "SELECT password from account " +
                                     " where accountName like N'" + username + "' and userId="+ Entity.user.Id;
            string passwordFromDb = "";
            try
            {
                using (SqlDataReader reader = Command.ExecuteReader())
                    while (reader.Read())
                        passwordFromDb = reader.GetString(0);

            }
            catch(Exception)
            {
                Command.Connection.Close();
                return false;
            }
            Command.Connection.Close();

            if (!passwordFromDb.Equals(encrPassword))
            {
                Entity.user = null;
                Entity.account = null;
                return false;
            }
                return true;

        }

        public Dictionary<int, string> GetUserProjectList()
        {
            if (Entity.Acces.IsLeaderCurrentLoggedUser)
            {
                var ids = GetTeamMembersIdOfCurrentLeader();
                if (ids.Length > 0)
                {
                    Dictionary<int, string> projects = new Dictionary<int, string>();
                    foreach (var userId in ids)
                    {

                        Command.Connection.Open();
                        Command.CommandText = "SELECT [projectId], [projectName] from project " +
                                                 " where userId=" + userId;
                        try
                        {
                            using (SqlDataReader reader = Command.ExecuteReader())
                                while (reader.Read())
                                    if (!projects.ContainsKey((int)reader.GetDecimal(0)))
                                        projects.Add((int)reader.GetDecimal(0), reader.GetString(1).ToString());

                        }
                        catch (Exception)
                        {
                            Command.Connection.Close();
                            return null;
                        }
                        Command.Connection.Close();

                    }
                    return projects;
                } else
                    return null;
            }
            else
            {
                Dictionary<int, string> projects = new Dictionary<int, string>();
                Command.Connection.Open();
                Command.CommandText = "SELECT [projectId], [projectName] from project " +
                                         " where userId=" + Entity.user.Id;
                try
                {
                    using (SqlDataReader reader = Command.ExecuteReader())
                        while (reader.Read())
                            if (!projects.ContainsKey((int)reader.GetDecimal(0)))
                                projects.Add((int)reader.GetDecimal(0), reader.GetString(1).ToString());

                }
                catch (Exception)
                {
                    Command.Connection.Close();
                    return null;
                }
                Command.Connection.Close();
                return projects;
            }
        }


        public DataGridView OpenProjectFromDB( DataGridView dg)
        {
            try
            {
                Entity.project.tasksList.Clear();

                Entity.Acces.IsFillingFromDataBase = true;
                dg.Columns[0].ReadOnly = false;

                Command.Connection.Open();
                Command.CommandText = "SELECT taskId , taskName," +
                                    " duration , complete, start, finish,priority, resources from task " +
                                    " where projectId=" + Entity.project.Id;

                DataTable data = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(Command);
                adapter.Fill(data);

                for (int i = 0; i < data.Rows.Count; i++)
                {
                    Task task;
                    for (int j = 0; j < dg.Columns.Count; j++)
                    {
                        dg.Rows[i].Cells["dataGridViewTextBoxColumn" + (j + 1).ToString()].Value = data.Rows[i][j].ToString() + ((j == 3) ? "%" : "");
                    }
                    task = Convertor.RowToTaskConvert(dg.Rows[i]);
                    Entity.project.tasksList.Add(task);
                }
                Command.Connection.Close();

                Entity.Acces.IsFillingFromDataBase = false;
                dg.Columns[0].ReadOnly = true;
                return dg;
            }
            catch
            {
                Entity.Acces.IsFillingFromDataBase = false;
                dg.Columns[0].ReadOnly = true;

                XML_Deserializer xmlDeserializer = new XML_Deserializer();
                var xmlfilePath = "D:\\DAMS\\DAMS_Proiect\\xml_exemple.xml";
                Project projectFromXml = xmlDeserializer.DeserializeProjectInstance(xmlfilePath);

                foreach (Task task in projectFromXml.tasksList)
                {
                    dg.Rows[task.TaskId - 1].Cells[0].Value = task.TaskId;
                    dg.Rows[task.TaskId - 1].Cells[1].Value = task.Name;
                    dg.Rows[task.TaskId - 1].Cells[2].Value = task.Duration;
                    dg.Rows[task.TaskId - 1].Cells[3].Value = task.Complete;
                    dg.Rows[task.TaskId - 1].Cells[4].Value = task.Start;
                    dg.Rows[task.TaskId - 1].Cells[5].Value = task.Finish;
                    dg.Rows[task.TaskId - 1].Cells[6].Value = task.Priority;
                    dg.Rows[task.TaskId - 1].Cells[7].Value = task.Resource;
                }
                return dg;
            }
        }

        public string[] GedTeamsFromDataBase()
        {
            Command.Connection.Open();
            Command.CommandText = "SELECT Id, TeamName from team ";
            int counter = 0;
            try
            {
                using (SqlDataReader reader = Command.ExecuteReader())
                    while (reader.Read())
                        counter++;
            }
            catch (Exception)
            {
                Command.Connection.Close();
                return new string[1] { "default" };
            }
            Command.Connection.Close();
            int teamId;
            string teamName;
            Command.Connection.Open();
            Command.CommandText = "SELECT Id, TeamName from team ";
            string[] teams = new string[counter];
            int index = 0;
            try
            {
                using (SqlDataReader reader = Command.ExecuteReader())
                    while (reader.Read())
                    {
                        teamId = reader.GetInt32(0);
                        teamName = reader.GetString(1);
                        string concat = teamId.ToString() + ". " + teamName;
                        teams[index] = concat;
                        index++;
                    }                       
            }
            catch (Exception)
            {
                Command.Connection.Close();
                return new string[1] { "default" };
            }

            Command.Connection.Close();
            if (teams.Length > 0) return teams;
            else return new string[1] { "default" };
        }

        public int GetTeamID()
        {
            decimal teamIdOfCurrentLeader = 1;
            int ledearId = 1;
            if (Entity.user != null)
                ledearId = user.RoleId;

            Command.Connection.Open();
            Command.CommandText = "SELECT teamId from ledear where ledearId =" + ledearId;
            try
            {
                using (SqlDataReader reader = Command.ExecuteReader())
                    while (reader.Read())
                        teamIdOfCurrentLeader = reader.GetDecimal(0);
            }
            catch (Exception)
            {
                Command.Connection.Close();
                return 1;
            }
            Command.Connection.Close();
            return (int)teamIdOfCurrentLeader;
        }

        public int[] GetTeamMembersIdOfCurrentLeader()
        {

            decimal teamIdOfCurrentLeader = 1;
            int ledearId = 1;
            if (Entity.user != null)
                ledearId = user.RoleId;

            Command.Connection.Open();
            Command.CommandText = "SELECT teamId from ledear where ledearId like " + ledearId;
            try
            {
                using (SqlDataReader reader = Command.ExecuteReader())
                    while (reader.Read())
                        teamIdOfCurrentLeader = reader.GetDecimal(0);
            }
            catch (Exception)
            {
                Command.Connection.Close();
                return new int[1] { 1 };
            }
            Command.Connection.Close();

            Command.Connection.Open();
            Command.CommandText = "SELECT DISTINCT userId from userTable where teamId =" + (int)teamIdOfCurrentLeader;
            int counter = 0;
            try
            {
                using (SqlDataReader reader = Command.ExecuteReader())
                    while (reader.Read())
                        counter++;
            }
            catch (Exception)
            {
                Command.Connection.Close();
                return new int[1] { 1 };
            }
            Command.Connection.Close();

            Command.Connection.Open();
            Command.CommandText = "SELECT DISTINCT userId from userTable where teamId =" + teamIdOfCurrentLeader;
            int[] users = new int[counter];
            int index = 0;
            try
            {
                using (SqlDataReader reader = Command.ExecuteReader())
                    while (reader.Read())
                    {
                        users[index] = (int)reader.GetDecimal(0);
                        index++;
                    }

            }
            catch (Exception)
            {
                Command.Connection.Close();
                return new int[1] { 1 };
            }
            Command.Connection.Close();
            if (users.Length > 0) return users;
            else return  new int[1] { 1 };

        }
            public string[] GetTeamMembersOfCurrentLeader()
        {
            decimal teamIdOfCurrentLeader = 1;
            int ledearId = 1;
            if (Entity.user != null)
                ledearId = user.RoleId;

            Command.Connection.Open();
            Command.CommandText = "SELECT teamId from ledear where ledearId like " + ledearId;
            try
            {
                using (SqlDataReader reader = Command.ExecuteReader())
                    while (reader.Read())
                        teamIdOfCurrentLeader = reader.GetDecimal(0);
            }
            catch (Exception)
            {
                Command.Connection.Close();
                return new string[1] { "user1" };
            }
            Command.Connection.Close();

            Command.Connection.Open();
            Command.CommandText = "SELECT DISTINCT userName from userTable where teamId =" + (int)teamIdOfCurrentLeader;
            int counter = 0;
            try
            {
                using (SqlDataReader reader = Command.ExecuteReader())
                    while (reader.Read())
                        counter++;
            }
            catch (Exception)
            {
                Command.Connection.Close();
                return new string[1] { "user1" };
            }
            Command.Connection.Close();

            Command.Connection.Open();
            Command.CommandText = "SELECT DISTINCT userName from userTable where teamId =" + teamIdOfCurrentLeader;
            string[] users = new string[counter];
            int index = 0;
            try
            {
                using (SqlDataReader reader = Command.ExecuteReader())
                    while (reader.Read())
                    {
                        users[index] = reader.GetString(0);
                        index++;
                    }

            }
            catch (Exception)
            {
                Command.Connection.Close();
                return new string[1] { "user1" };
            }
            Command.Connection.Close();
            if (users.Length > 0) return users;
            else return new string[1] { "user1" };
        }
    }
}
