using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DAMS_Proiect
{
    public class CSV_Serializer : ISerializationStrategy
    {
        public override string SerializeProjectInstance(DataGridView dtDataTable, string strFilePath)
        {
            if (dtDataTable==null) return "csv";
            ToCSV(dtDataTable, strFilePath);
            return "csv";
        }

        public CSV_Serializer() { }

        public  bool ToCSV( DataGridView dtDataTable, string strFilePath)
        {
            try
            {
                StreamWriter sw = new StreamWriter(strFilePath, false);

                for (int i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    string colName = dtDataTable.Columns[i].HeaderText.ToString();
                    sw.Write(colName);
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);
                for (int i = 0; i < dtDataTable.Rows.Count; i++)
                {
                    for (int j = 0; j < dtDataTable.Columns.Count; j++)
                    {
                        Task t= Entity.project.tasksList.Find(task => task.TaskId == i + 1);
                        if (dtDataTable.Rows[i] != null && t!=null )
                        {

                            if (dtDataTable.Rows[i].Cells["dataGridViewTextBoxColumn" + (j + 1)] != null)
                            {
                                var currentCellValue = dtDataTable.Rows[i].Cells["dataGridViewTextBoxColumn" + (j + 1)].Value.ToString() + ((j == 3) ? "%" : "") + ((j == 2) ? "h" : "");

                                if (!Convert.IsDBNull(currentCellValue))
                                {
                                    string value = currentCellValue.ToString();
                                    if (value.Contains(','))
                                    {
                                        value = String.Format("\"{0}\"", value);
                                        sw.Write(value);
                                    }
                                    else
                                    {
                                        sw.Write(currentCellValue.ToString());
                                    }
                                }
                                if (i < dtDataTable.Columns.Count - 1)
                                {
                                    sw.Write(",");
                                }
                            }
                        }
                    }
                    sw.Write(sw.NewLine);
                }
                sw.Close();
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

    }
}
