using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DAMS_Proiect;

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
            if (getData.CheckUserNameExistence(CurrentAccount.UserName)) result =  false;
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
}
