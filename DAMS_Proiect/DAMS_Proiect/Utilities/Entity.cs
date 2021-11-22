using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAMS_Proiect
{
    public static class Entity
    {
        public static User user;
        public static Project project;
        public static Account account;

        public static class Acces
        {
            public static bool IsUserLoggedIn = false;
            public static bool IsFillingFromDataBase = false;
        }
    }
}
