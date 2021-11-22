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
                int roleid = 1;
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
                            teamid = (int)reader.GetDecimal(5);
                            roleid = (int)reader.GetDecimal(6);
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


        public DataGridView OpenProjectFromDB( DataGridView dg)
        {
            Entity.project.tasksList.Clear();

            Entity.Acces.IsFillingFromDataBase = true;
            dg.Columns[0].ReadOnly = false;

            Command.Connection.Open();
            Command.CommandText = "SELECT taskId , taskName," +
                                " duration , complete, start, finish,priority, resources from task " +
                                " where projectId="+ Entity.project.Id;

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
    }
}
