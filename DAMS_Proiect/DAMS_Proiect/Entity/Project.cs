using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;
using System.Xml.Serialization;


namespace DAMS_Proiect
{
    public class Project
    {
        static Project instance;

        public List<Task> tasksList;
        public string xmlPath;
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
        private static object locker = new object();

        public static Project GetProjectInstance()
        {
            if (instance == null)
            {
                lock (locker)
                {
                    if (instance == null)
                    {
                        instance = new Project();
                    }
                }
            }
            return instance;
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
