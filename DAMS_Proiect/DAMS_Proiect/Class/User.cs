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


        public Departamanet Role
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
            Accounting
        }

        public User(Account personalAccount, int id, string name, List<Project> projects, Departamanet role, string addres, string phoneNumber, string email)
        {
            PersonalAccount = personalAccount;
            Id = id;
            Name = name;
            Projects = projects;
            Role = role;
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
                   Role == user.Role &&
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
            hashCode = hashCode * -1521134295 + Role.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Addres);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(PhoneNumber);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Email);
            return hashCode;
        }
    }
}
