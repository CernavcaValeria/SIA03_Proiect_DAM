using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace DAMS_Proiect
{
    public class XML_Deserializer : IDerializationStrategy
    {
        public override Project DeserializeProjectInstance(string strFilePath)
        {
            return DeserializeTasks(strFilePath);
        }
        public XML_Deserializer() { }

        public Project DeserializeTasks(string strFilePath)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Task>));

                FileStream fileStream = new FileStream(strFilePath, FileMode.Open);

                List<Task> deserializedList = (List<Task>)serializer.Deserialize(fileStream);

                if(Entity.project != null)
                {
                    Entity.project.tasksList = deserializedList.GetRange(0, deserializedList.Count);
                    return Entity.project;
                }
                else
                {
                    Project P = new Project();
                    P.tasksList = deserializedList.GetRange(0, deserializedList.Count);
                    return P;
                }
            }
            catch (Exception ex)
            {
                return Entity.project;
            }
        }
    }
}
