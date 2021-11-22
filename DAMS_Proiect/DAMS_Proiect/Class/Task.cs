using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;
using System.Xml.Serialization;

namespace DAMS_Proiect
{
    [Serializable]
    public class Task
    {
        [XmlElement(ElementName = "Id")]
        public int TaskId
        {
            get;
            set;
        } = 0;


        [XmlElement(ElementName = "Name")]
        public string Name
        {
            get;
            set;
        } = "";


        [XmlElement(ElementName = "Duration")]
        public double Duration
        {
            get;
            set;
        } = 1;


        [XmlElement(ElementName = "Start")]
        public DateTime Start
        {
            get;
            set;

        } = DateTime.Now;


        [XmlElement(ElementName = "Finish")]
        public DateTime Finish
        {
            get;
            set;

        } = DateTime.Now;

        [XmlElement(ElementName = "Priority")]
        public string Priority
        {
            get;
            set;
        } = "Medium";

        /*
                [XmlArray("Resources"), XmlArrayItem("name", typeof(string))]
                public List<String> ResourceNames
                {
                    get;
                    set;

                } = new List<string>();
        */

        [XmlElement(ElementName = "Resource")]
        public string Resource
        {
            get;
            set;
        } = "";

        public enum TaskMode
        {
            Yes,
            No
        }

        [XmlElement(ElementName = "CompeletePercent")]
        public float Complete
        {
            get;
            set;

        } = 0;


        public Task() { }

        public Task(int taskId, string str, string filedName)
        {
            TaskId = taskId;

            if (filedName.Equals("Name"))
            {
                Name = str;
            }
            else if (filedName.Equals("Priority"))
            {
                Priority = str;
            }
            else if (filedName.Equals("Resource"))
            {
                Resource = str;
            }
        }

        public Task(int taskId, float val, string flag)
        {
            if (flag.Equals("c"))
                Complete = val;
            else if (flag.Equals("d")) Duration = val;

            TaskId = taskId;

        }

        public Task(int taskId, DateTime date, string filedName)
        {
            TaskId = taskId;

            if (filedName.Equals("Start"))
            {
                Start = date;
            }
            else if (filedName.Equals("Finish"))
            {
                Finish = date;
            }
        }




        public Task(int taskId)
        {
            TaskId = taskId;
        }

        public void DisplayTaskInformation()
        {
            Console.WriteLine("\nTask ID={0} Information:\n\tName: {1}\n\tStart: {2}\n\tFinish: {3}\n\tDuration: {4}\n\tComplete%: {5}",
                    TaskId, Name, Start, Finish, Duration, Complete);

            /*
                        if (ResourceNames.Count > 0)
                        {
                            Console.WriteLine("\tResource Names:");
                            foreach (string resource in ResourceNames)
                            {
                                Console.WriteLine("\t\t{0}", resource);
                            }
                        }*/
            Console.WriteLine("___________________________________");

        }
    }
}
