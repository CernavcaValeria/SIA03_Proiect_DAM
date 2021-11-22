using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace DAMS_Proiect
{
    public static class ExtensionMethods
    {
        public static string SerializeTasks(this Project project)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Task>));

                TextWriter writer = new StreamWriter(@project.xmlPath);

                if (project.tasksList.Count > 0)
                {
                    serializer.Serialize(writer, project.tasksList);
                    return "File successfully converted to XML!";
                }
                return "Fail! The tasks list is empty";

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        public static string DeserializeTasks(this Project project)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Task>));

                FileStream fileStream = new FileStream(@project.xmlPath, FileMode.Open);

                List<Task> deserializedList = (List<Task>)serializer.Deserialize(fileStream);

                project.tasksList = deserializedList.GetRange(0, deserializedList.Count);

                return "File successfully converted!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

    };

};
