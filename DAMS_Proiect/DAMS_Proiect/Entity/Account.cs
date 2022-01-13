using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAMS_Proiect
{
    
    public class Account
    {
        public int UserId
        {
            get;
            set;
        }

        public string UserPassword
        {
            get;
            set;
        } = "";
        public string UserRepeatPassword
        {
            get;
            set;
        } = "";

        public string UserName
        {
            get;
            set;
        } = "";

        public Account(int userId, string userPassword, string userName)
        {
            UserId = userId;
            UserPassword = userPassword;
            UserName = userName;
        }

        public Account(int userId, string userPassword)
        {
            UserId = userId;
            UserPassword = userPassword;
        }

        public Account(string userPassword, string userName)
        {
            UserPassword = userPassword;
            UserName = userName;
        }

        public Account() { }

        public override bool Equals(object obj)
        {
            return obj is Account account &&
                   UserId == account.UserId &&
                   UserPassword == account.UserPassword &&
                   UserName == account.UserName;
        }


        public override int GetHashCode()
        {
            int hashCode = 33911103;
            hashCode = hashCode * -1521134295 + UserId.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(UserPassword);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(UserName);
            return hashCode;
        }
    }
}
