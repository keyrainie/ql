
using Newegg.Oversea.Framework.JobConsole.Client;


namespace ShiftBacketDeleteOver48H
{
    public class Biz : IJobAction
    {

        #region IJobAction Members

        public void Run(JobContext context)
        {
            context.Message += "ShiftBasketDeleteOver48H Start...";
            context.Message += string.Format("{0} Records Has Been Deleted...", DoJob());
            context.Message += "ShiftBasketDeleteOver48H Successed";
        }

        #endregion

        public static int DoJob()
        {            
            return DA.Delete();
        }
    }
}