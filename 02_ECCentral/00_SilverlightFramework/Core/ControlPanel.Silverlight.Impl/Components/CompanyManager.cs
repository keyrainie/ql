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
 
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Rest;
using Newegg.Oversea.Silverlight.Core.Components;
using Newegg.Oversea.Silverlight.Utilities;
using System.Collections.ObjectModel;

namespace Newegg.Oversea.Silverlight.Controls.Components
{
    public class CompanyManager : ICompanyManager
    {
        public void GetAllCompanies(int userSysNo, Action callback)
        {
            string serviceUrl = CPApplication.Current.CommonData["ECCentralServiceURL"].ToString();
            serviceUrl = serviceUrl.TrimEnd('/');
            string urlCompany = "/CommonService/Company/{0}";
            urlCompany = string.Format(urlCompany, CPApplication.Current.LoginUser.UserSysNo.Value.ToString());
            string urlWebChannel = "/CommonService/WebChannel/{0}/{1}";


            RestClient restClient = new RestClient(serviceUrl);
            restClient.Query<List<UICompany>>(urlCompany, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                List<UICompany> lr1 = args.Result;
                if (lr1 == null || lr1.Count == 0)
                {
                    throw new Exception("Have not found your company!");
                }
                CPApplication.Current.CompanyList = new ReadOnlyCollection<UICompany>(lr1);

                string curCompanyCode = UtilityHelper.GetCurrentCompanyCode();
                string curCompanyName = UtilityHelper.GetCurrentCompanyName();

                if (!string.IsNullOrEmpty(curCompanyCode) && !string.IsNullOrEmpty(curCompanyName))
                {
                    CPApplication.Current.CompanyCode = curCompanyCode;
                    CPApplication.Current.CompanyName = curCompanyName;
                }
                else
                {
                    CPApplication.Current.CompanyCode = CPApplication.Current.CompanyList[0].CompanyCode;
                    CPApplication.Current.CompanyName = CPApplication.Current.CompanyList[0].CompanyName;
                }

                UtilityHelper.SetCurrentCompanyCode(CPApplication.Current.CompanyCode);
                UtilityHelper.SetCurrentCompanyName(CPApplication.Current.CompanyName);

                urlWebChannel = string.Format(urlWebChannel, CPApplication.Current.CompanyCode, CPApplication.Current.LoginUser.UserSysNo.Value.ToString());
                restClient.Query<List<UIWebChannel>>(urlWebChannel, (obj2, args2) =>
                    {
                        if (args2.FaultsHandle())
                        {
                            return;
                        }
                        List<UIWebChannel> lr2 = args2.Result;
                        if (lr1 == null || lr1.Count == 0)
                        {
                            throw new Exception(string.Format("Have not found your WebChannel on Company:{0}!", CPApplication.Current.CompanyName));
                        }

                        CPApplication.Current.CurrentWebChannelList = new ReadOnlyCollection<UIWebChannel>(lr2);
                        callback();
                    });

            });
        }

        #region IComponent Members

        public string Name
        {
            get { return "CompanyManager"; }
        }

        public string Version
        {
            get { return "1.0.0.0"; }
        }

        public void InitializeComponent(IPageBrowser browser)
        {
        }

        public object GetInstance(TabItem tab)
        {
            return this;
        }

        public void Dispose()
        {

        }
        #endregion

        
         
    }
}
