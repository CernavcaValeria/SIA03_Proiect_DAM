using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;
using System.Xml.Serialization;


namespace DAMS_Proiect
{
    [Serializable]
    [XmlRoot("List", Namespace = "", IsNullable = false)]
    public class Project
    {
        public List<Task> tasksList;
        public int Id;
        public User Ow
        {
            get;
            set;
        } = new User();

        public string Name
        {
            get;
            set;
        } = "";

        public Project()
        {
            tasksList = new List<Task>();
        }


        public void DisplayTasksListInformation()
        {
            Console.WriteLine("\nThe total number of existing tasks:{0}\n------------------------------------", tasksList.Count);
            foreach (Task task in tasksList)
            {
                task.DisplayTaskInformation();
            }
        }

    };
};
