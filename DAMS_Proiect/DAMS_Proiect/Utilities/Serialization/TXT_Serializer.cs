using System;
using System.Collections.Generic;
using System.Text;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using System.IO;
using Syncfusion.Drawing;
using System.Data;
using System.Windows.Forms;

namespace DAMS_Proiect
{
   public class TXT_Serializer : ISerializationStrategy
    {
        public override string SerializeProjectInstance(DataGridView dtDataTable, string strFilePath)
        {
            if (dtDataTable == null) return "txt";
            DataTable dt = new DataTable();
            foreach (DataGridViewColumn col in dtDataTable.Columns)
            {
                dt.Columns.Add(col.Name);
            }

            foreach (DataGridViewRow row in dtDataTable.Rows)
            {
                DataRow dRow = dt.NewRow();
                foreach (DataGridViewCell cell in row.Cells)
                {
                    dRow[cell.ColumnIndex] = cell.Value;
                }
                dt.Rows.Add(dRow);
            }
            CreateTXTFile(dt, strFilePath);
            return "txt";
        }

        public TXT_Serializer() { }
        public void CreateTXTFile(DataTable dt, string path)
        {
            StreamWriter swExtLogFile = new StreamWriter(path, true);
            StringBuilder sb = new StringBuilder();
            for (int k = 0; k < dt.Rows.Count; k++)
            {
                StringBuilder rowStringBuilder = new StringBuilder();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    string esynonewline = i == 0 ? "\n" : "\n\t";
                    rowStringBuilder.Append(esynonewline);
                    if (k < dt.Rows.Count)
                        rowStringBuilder.Append(dt.Columns[i].ToString() + ": " + dt.Rows[k][i]);
                }
                rowStringBuilder.Append("\n\n");
                sb.Append(rowStringBuilder);
            }
            swExtLogFile.Write(sb.ToString());
            swExtLogFile.Flush();
            swExtLogFile.Close();

        }
    }
}

