using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DAMS_Proiect;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DAMS_Proiect.Entity;
using static DAMS_Proiect.Convertor;

namespace DAMS_Tests
{
    [TestClass]
    public class UserRegistrationTests
    {
        public static User CurrentUser = CreateUser();
        public static Account CurrentAccount;
        public static GetData getData = new GetData();
        public static CreateData createData = new CreateData();
        public static User CreateUser()
        {
            User user = new User
            {
                Name = "Stefan Popa",
                Addres = "Anrei Petre",
                PhoneNumber = "12345678",
                Email = "stefan@gmail.com",
                TeamId = 1,
                RoleId = 1,
                UserDepartamanet = User.Departamanet.Tester
            };
            return user;
        }
        public static Account CreateAccount(string userName, string passWord, string repeatPasswrod)
        {
            Account account = new Account
            {
                UserName = userName,
                UserPassword = passWord,
                UserRepeatPassword = repeatPasswrod
            };
            return account;
        }

        [TestMethod]
        public void RegisterUserWrongPasswordLength()
        {
            CurrentAccount = CreateAccount("stefan1", "123", "123");
            bool testResult = (CurrentAccount.UserPassword.Length > 6);
            Assert.IsFalse(testResult);
        }


        [TestMethod]
        public void RegisterUserWrongRepeatPassword()
        {
            CurrentAccount = CreateAccount("stefan1", "PASSWORD", "PASSWORD123");

            bool testResult = CurrentAccount.UserPassword.Equals(CurrentAccount.UserRepeatPassword);
            Assert.IsFalse(testResult);
        }

        [TestMethod]
        public void RegisterUserWrongUserName()
        {
            //username 'valeria' already exists
            CurrentAccount = CreateAccount("valeria", "PASSWORD", "PASSWORD123");

            bool testResult = !getData.CheckUserNameExistence(CurrentAccount.UserName);
            Assert.IsFalse(testResult);
        }
        [TestMethod]
        public void RegisterUser()
        {
            CurrentAccount = CreateAccount("stefan4", "password", "password");
            bool result;
            if (getData.CheckUserNameExistence(CurrentAccount.UserName)) result = false;
            else
                result = createData.RegisterUser(CurrentUser.Name, CurrentAccount.UserName, CurrentAccount.UserPassword,
                    CurrentUser.Email, CurrentUser.Addres, CurrentUser.PhoneNumber,
                    CurrentUser.UserDepartamanet.ToString());
            Assert.IsTrue(result);
        }
    }

    [TestClass]
    public class UserLogInTests
    {
        public static GetData getData = new GetData();
        [TestMethod]
        public void LoginWrongUserName()
        {
            string username = "ewasgdjtarshnbfwas";
            bool result = getData.CheckUserNameExistence(username);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void LoginWrongPasswordName()
        {
            string username = "valeria";
            string password = "ivkbjlnbjvchvkbl";
            bool result = getData.ValidatePassword(password, username);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void LoginSuccesDataInputs()
        {
            string username = "valeria";
            string password = "123456";
            bool result = false;

            if (getData.CheckUserNameExistence(username))
                if (getData.ValidatePassword(password, username))
                    result = true;

            Assert.IsTrue(result);
        }
    }

//========================================================================

    [TestClass]
    public class PersistenceTests
    {
        [TestMethod]
        public void SelecetProjectFromDataBase()
        {
            bool selectFromDataBaseSucces;
            string ConnString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\DAMS\DataBase\DAMS_DataBase.mdf;Integrated Security=True";
            SqlConnection Conn;
            SqlCommand Command;


            Conn = new SqlConnection
            {
                ConnectionString = ConnString
            };
            Command = new SqlCommand
            {
                Connection = Conn
            };

            Dictionary<int, string> projects = new Dictionary<int, string>();
            Command.Connection.Open();
            Command.CommandText = "SELECT [projectId], [projectName] from project " +
                                     " where userId=" + 1;
            try
            {
                using (SqlDataReader reader = Command.ExecuteReader())
                    while (reader.Read())
                        if (!projects.ContainsKey((int)reader.GetDecimal(0)))
                            projects.Add((int)reader.GetDecimal(0), reader.GetString(1).ToString());

                selectFromDataBaseSucces = true;

            }
            catch (Exception)
            {
                Command.Connection.Close();
                selectFromDataBaseSucces = false;
            }
            Command.Connection.Close();

            bool testResult = selectFromDataBaseSucces && projects.Count > 0;
            Assert.IsTrue(testResult);
        }
    }

    //========================================================================

    [TestClass]
    public class SerializationTests
    {
        [TestMethod]
        public void XMLDeserialization()
        {
            XML_Deserializer xmlDeserializer = new XML_Deserializer();
            var xmlfilePath = "D:\\DAMS\\DAMS_Proiect\\xml_exemple.xml";
            Project projectFromXml =  xmlDeserializer.DeserializeProjectInstance(xmlfilePath);

            bool testDeserializationResult = false;
            if (projectFromXml!= null)
            {
                if (projectFromXml.tasksList.Count > 0)
                    testDeserializationResult = true;
                else
                    testDeserializationResult = false;

            }
            Assert.IsTrue(testDeserializationResult);
        }
    }


 //========================================================================

        [TestClass]
    public class FrontEndPatternsTest
    {
        [TestMethod]
        public void ProjectSingleton()
        {
            Project p1 = Project.GetProjectInstance();
            Project p2 = Project.GetProjectInstance();
            // Test for same instance
            bool instancetestResult = (p1 == p2);
            Assert.IsTrue(instancetestResult);
        }

        [TestMethod]
        public void SerializationStrategy()
        {
            ISerializationStrategy xml = new XML_Serializer();
            ISerializationStrategy pdf = new PDF_Serializer();
            ISerializationStrategy csv = new CSV_Serializer();
            ISerializationStrategy txt = new TXT_Serializer();

            List<ISerializationStrategy> ISerializationStrategyList = new List<ISerializationStrategy>
            {
                xml,
                pdf,
                csv,
                txt
            };

            int counter;
            bool testResult = true;
            foreach ( var item in ISerializationStrategyList)
            {
                counter = 0;
                foreach (var item1 in ISerializationStrategyList)
                {
                    if (item.SerializeProjectInstance(null, "").Equals(item1.SerializeProjectInstance(null, "")))
                        counter++;
                }
                if (counter > 1) testResult = false;
            }
            Assert.IsTrue(testResult);
        }
    }
}
