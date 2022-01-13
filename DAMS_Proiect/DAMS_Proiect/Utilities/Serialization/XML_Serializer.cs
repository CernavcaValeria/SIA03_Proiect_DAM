using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace DAMS_Proiect
{
    public class XML_Serializer : ISerializationStrategy
    {
        public override string SerializeProjectInstance(DataGridView dtDataTable, string strFilePath)
        {
            if (dtDataTable == null) return "xml";
            SerializeTasks(Entity.project);
            return "xml";
        }

        public XML_Serializer() { }
        public void SerializeTasks( Project project)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Task>));

                TextWriter writer = new StreamWriter(@project.xmlPath);

                if (project.tasksList.Count > 0)
                {
                    serializer.Serialize(writer, project.tasksList);
                }

            }
            catch (Exception ex)
            {
                //ex
            }
        }
    }
};
