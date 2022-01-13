using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;
using System.Xml.Serialization;

namespace DAMS_Proiect
{
    public class User
    {
        public Account PersonalAccount
        {
            get;
            set;
        } = new Account();

        public int Id;

        public string Name
        {
            get;
            set;
        } = "";

        public List<Project> Projects
        {
            get;
            set;
        } = new List<Project>();


        public Departamanet UserDepartamanet
        {
            get;
            set;
        } = Departamanet.Default;

        public string Addres
        {
            get;
            set;
        } = "";

        public string PhoneNumber
        {
            get;
            set;
        } = "";

        public string Email
        {
            get;
            set;
        } = "";

        public enum Departamanet
        {
            Default,
            Marketing,
            Design,
            FrontEnd,
            BackEnd,
            Tester,
            Security,
            DBAdmin,
            HR,
            Finances,
            Accounting,
            Director
        }

        public int TeamId
        {
            get;
            set;
        } = 0;

        public int RoleId
        {
            get;
            set;
        } = 0;

        public User(Account personalAccount, int id, string name, List<Project> projects, Departamanet role, string addres, string phoneNumber, string email)
        {
            PersonalAccount = personalAccount;
            Id = id;
            Name = name;
            Projects = projects;
            UserDepartamanet = role;
            Addres = addres;
            PhoneNumber = phoneNumber;
            Email = email;
        }

        public User() { }

        public override bool Equals(object obj)
        {
            return obj is User user &&
                   EqualityComparer<Account>.Default.Equals(PersonalAccount, user.PersonalAccount) &&
                   Id == user.Id &&
                   Name == user.Name &&
                   EqualityComparer<List<Project>>.Default.Equals(Projects, user.Projects) &&
                   UserDepartamanet == user.UserDepartamanet &&
                   Addres == user.Addres &&
                   PhoneNumber == user.PhoneNumber &&
                   Email == user.Email;
        }

        public override int GetHashCode()
        {
            int hashCode = -333846327;
            hashCode = hashCode * -1521134295 + EqualityComparer<Account>.Default.GetHashCode(PersonalAccount);
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<Project>>.Default.GetHashCode(Projects);
            hashCode = hashCode * -1521134295 + UserDepartamanet.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Addres);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(PhoneNumber);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Email);
            return hashCode;
        }

        public static Departamanet GetDepartamanetByName(string d)
        {
            if (d.Equals("Marketing")) return Departamanet.Marketing;
            if (d.Equals("Design")) return Departamanet.Design;
            if (d.Equals("FrontEnd")) return Departamanet.FrontEnd;
            if (d.Equals("BackEnd")) return Departamanet.BackEnd;
            if (d.Equals("BDAdmin")) return Departamanet.DBAdmin;
            if (d.Equals("Accounting")) return Departamanet.Accounting;
            if (d.Equals("Tester")) return Departamanet.Tester;
            if (d.Equals("Security")) return Departamanet.Security;
            if (d.Equals("Director")) return Departamanet.Director;
            return Departamanet.Default;
        }
    }
}
