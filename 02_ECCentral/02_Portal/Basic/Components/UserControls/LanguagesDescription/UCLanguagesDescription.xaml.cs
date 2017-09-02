using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Configuration;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;


namespace ECCentral.Portal.Basic.Components.UserControls.LanguagesDescription
{
    /// <summary>
    /// 接收两个参数，获取多语言设置列表  业务对象类型以及业务对象编号(BizObjectType|BizObjectSysNo)
    /// </summary>
    public partial class UCLanguagesDescription : UserControl
    {
        public IDialog Dialog { get; set; }
        BizObjecLanguageDescVM vm;
        List<BizObjecLanguageDescVM> listvm;
        Facade facade;

        List<CodeNamePair> codeNamePairList;
        public string bizObjectType;
        public string bizObjectNo;

        public List<UCLanguageDescripttionSetting> ucList = new List<UCLanguageDescripttionSetting>();

        private IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        public UCLanguagesDescription(string BizObjectType, string BizObjectSysNo)
        {
            listvm = new List<BizObjecLanguageDescVM>();
            codeNamePairList = new List<CodeNamePair>();
            vm = new BizObjecLanguageDescVM();
            bizObjectType = BizObjectType;
            bizObjectNo = BizObjectSysNo;

            InitializeComponent();
            Loaded += UCLanguagesDescription_Loaded;
        }

        void UCLanguagesDescription_Loaded(object sender, RoutedEventArgs e)
        {
            //LanguageOptions=ConfigurationManager

            facade = new Facade(CPApplication.Current.CurrentPage);
            //获取业务对象类型
            CodeNamePairHelper.GetList(ConstValue.DomainName_Common, ConstValue.Key_BizObjectType, (o, p) =>
            {
                codeNamePairList = p.Result;

                CodeNamePair CodeName = codeNamePairList.Where(s => s.Code == bizObjectType).SingleOrDefault();
                vm.BizObjectType = CodeName.Code;
                if (vm.BizObjectType == "Product")
                {
                    vm.BizObjectId = bizObjectNo;
                }

                if (vm.BizObjectType == "Merchant")
                {
                    vm.BizObjectSysNo = int.Parse(bizObjectNo);
                }

                vm.ShowBizObjectTypeName = CodeName.Name;
                List<KeyValuePair<string, string>> languageList = GetLanguageList();
                facade.LoadBizObjectLanguageDescList(vm.BizObjectType, vm.BizObjectSysNo, vm.BizObjectId, (obj, args) =>
                    {
                        listvm = args.Result;
                        if (listvm != null && listvm.Count > 0)
                        {
                            for (int i = 0; i < listvm.Count; i++)
                            {
                                listvm[i].BizObjectSysNo = vm.BizObjectSysNo;
                                listvm[i].BizObjectType = vm.BizObjectType;
                                listvm[i].BizObjectId = vm.BizObjectId;
                                listvm[i].ShowLanguageTypeName = languageList.Where(s => s.Key == listvm[i].LanguageCode).SingleOrDefault().Value;
                                UCLanguageDescripttionSetting uc = new UCLanguageDescripttionSetting();
                                uc.DataContext = listvm[i];
                                Containner.Children.Add(uc);
                                languageList.Remove(languageList.Where(k => k.Key == listvm[i].LanguageCode).SingleOrDefault());
                            }
                        }
                        
                        if (languageList != null && languageList.Count > 0)
                            {
                                for (int i = 0; i < languageList.Count; i++)
                                {
                                    if (listvm==null || !listvm.Any(l => l.LanguageCode == languageList[i].Key))
                                    {
                                        BizObjecLanguageDescVM view = new BizObjecLanguageDescVM();
                                        view.BizObjectSysNo = vm.BizObjectSysNo;
                                        view.BizObjectType = vm.BizObjectType;
                                        view.BizObjectId = vm.BizObjectId;
                                        view.LanguageCode = languageList[i].Key;
                                        view.ShowLanguageTypeName = languageList[i].Value;
                                        UCLanguageDescripttionSetting uc = new UCLanguageDescripttionSetting();
                                        uc.DataContext = view;
                                        Containner.Children.Add(uc);
                                    }
                                }
                            }
                    });
                this.DataContext = vm;
            });


            this.Loaded -= UCLanguagesDescription_Loaded;
        }

        public List<KeyValuePair<string, string>> GetLanguageList()
        {
            string LanguageOptions = CPApplication.Current.Browser.Configuration.GetConfigValue(ConstValue.DomainName_Common,ConstValue.LanguageOptions);

            List<KeyValuePair<string, string>> languageItem = new List<KeyValuePair<string, string>>();
            if (string.IsNullOrEmpty(LanguageOptions))
            {
                languageItem.Add(new KeyValuePair<string, string>("zh-cn", "中文"));
            }
            else
            {
                string[] str = LanguageOptions.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in str)
                {
                    string[] ls = s.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (ls.Length != 2)
                    {
                        throw new Exception("LanguageOptions configuration is error in appsettings!");
                    }
                    languageItem.Add(new KeyValuePair<string, string>(ls[0], ls[1]));
                }
            }
            return languageItem;
        }
    }
}
