using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DAMS_Proiect.Entity;

namespace DAMS_Proiect
{
    static public class Convertor
    {
        public static Task RowToTaskConvert(DataGridViewRow row)
        {
            int.TryParse(row.Cells["dataGridViewTextBoxColumn1"].Value.ToString(), out int id);
            Task task = new Task(id)
            {
                Name = row.Cells["dataGridViewTextBoxColumn2"].Value.ToString()
            };

            int.TryParse(row.Cells["dataGridViewTextBoxColumn3"].Value.ToString(), out int duration);
            task.Duration = duration;

            int.TryParse(row.Cells["dataGridViewTextBoxColumn4"].Value.ToString(), out int complete);
            task.Complete = complete;

            DateTime.TryParse(row.Cells["dataGridViewTextBoxColumn5"].Value.ToString(), out DateTime start);
            task.Start = start;

            DateTime.TryParse(row.Cells["dataGridViewTextBoxColumn6"].Value.ToString(), out DateTime finish);
            task.Finish = finish;

            task.Priority = row.Cells["dataGridViewTextBoxColumn7"].Value.ToString();
            task.Resource = row.Cells["dataGridViewTextBoxColumn8"].Value.ToString();

            return task;
        }
    }
}
