using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.WPMessage.Facades;

namespace ECCentral.Portal.UI.WPMessage
{
    public class CheckWPMessage
    {
        public void CheckCurrentUserWPMessage()
        {
            //CPApplication.Current.CurrentPage.Context.Window.Alert("aa");
            try
            {
                if (CPApplication.Current != null && CPApplication.Current.LoginUser != null && CPApplication.Current.LoginUser.UserSysNo.HasValue)
                {
                    WPMessagFacade facade = new WPMessagFacade();
                    facade.CheckCurrentUserHasWPMessage(CPApplication.Current.LoginUser.UserSysNo.Value.ToString(), (s, args) =>
                    {
                        // if (args.FaultsHandle())
                        if (args.Error != null && args.Error.StatusCode == 404)
                        {
                            CPApplication.Current.SetRequestIcon(false);
                            return;
                        }
                        CPApplication.Current.SetRequestIcon(args.Result);
                    });
                }
            }
            catch { }
        }
    }
}
