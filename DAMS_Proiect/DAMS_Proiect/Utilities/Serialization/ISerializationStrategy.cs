using System;
using System.Windows.Forms;

namespace DAMS_Proiect
{
    public abstract class ISerializationStrategy
    {
        public abstract string SerializeProjectInstance(DataGridView dtDataTable, string strFilePath);
    }
}
