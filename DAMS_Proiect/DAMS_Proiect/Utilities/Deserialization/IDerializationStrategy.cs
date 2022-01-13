using System.Windows.Forms;

namespace DAMS_Proiect
{
    public abstract class IDerializationStrategy
    {
        public abstract Project DeserializeProjectInstance( string strFilePath);
    }
}
