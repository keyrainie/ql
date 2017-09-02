
using Newegg.Oversea.Framework.DataAccess;

namespace ShiftBacketDeleteOver48H
{
    public class DA
    {

        public static int Delete()
        {
            
            DataCommand command = DataCommandManager.GetDataCommand("Delete");

            return command.ExecuteNonQuery();
        }
    
    }
}
