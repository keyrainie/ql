using System.Globalization;
using System.Threading;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.MKT
{
    public class App : IModule
    {
        public void Initialize()
        {
            //全局控制日期的格式
            if (Thread.CurrentThread.CurrentCulture != null)
            {
                Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
            }
        }
    }
}
