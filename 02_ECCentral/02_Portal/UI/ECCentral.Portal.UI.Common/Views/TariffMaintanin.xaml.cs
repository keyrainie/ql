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
using System.Windows.Navigation;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.Common.Models;
using ECCentral.Portal.UI.Common.Facades;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Common;
using System.Text.RegularExpressions;

namespace ECCentral.Portal.UI.Common.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class TariffMaintanin : PageBase
    {

        TariffInfoVM VM;
        TariffFacade facade;
        public IPage Page
        {
            get
            {
                return CPApplication.Current.CurrentPage;
            }
        }
        public TariffMaintanin()
        {
            InitializeComponent();
            Loaded += TariffMaintanin_Loaded;
        }
        void TariffMaintanin_Loaded(object sender, RoutedEventArgs e)
        {
            //this.picker.SelectedCode = "0101";
            VM = new TariffInfoVM();
            facade = new TariffFacade(this);


            //VM.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_DownLoad_DownLoadYYCard);权限
            var select = this.Request.UserState as dynamic;
            if (!String.IsNullOrEmpty(this.Request.Param))
            {
                int temp = 0;
                if (int.TryParse(this.Request.Param, out temp))
                {
                    Init(temp);
                }

            }
            else
            {
                VM.Status = TariffStatus.Valid;
            }
            this.DataContext = VM;
            Loaded -= TariffMaintanin_Loaded;
        }

        public void Init(int SysNo)
        {
            facade.LoadTariffInfo(SysNo, (obj, args) =>
            {
                VM = EntityConverter<TariffInfo, TariffInfoVM>.Convert(
                                                 args.Result,
                                                 (s, t) =>
                                                 { });
                this.DataContext = VM;
            });

        }

        protected void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this.Layout))
            {
                return;
            }

            Regex price = new Regex(@"^\d{1,8}(\.\d{1,2})?$");//验证价差
            Regex Repercent = new Regex(@"^(\d{1,2}(\.\d{1,2})?|100(\.[0]{0,2})?)$");//验证百分比

            if (string.IsNullOrEmpty(VM.TariffRate))
            {
                VM.TariffRate = "0";
            }
            if (string.IsNullOrEmpty(VM.TariffPrice))
            {
                VM.TariffPrice = "0";
            }

            if (!price.IsMatch(VM.TariffPrice))
            {
                Window.Alert("请输入大于等于0的数字!");
                return;
            }

            if (!Repercent.IsMatch(VM.TariffRate))
            {
                Window.Alert("请输入0-100之间的数字!");
                return;
            }
     
            if (VM.SysNo != null)
            {
                facade.UpdateTariffInfo(VM, (args) =>
                {

                    Window.Alert("提示", "更新成功!", MessageType.Information, (obj2, args2) =>
                    {
                        if (args2.DialogResult == DialogResultType.Cancel)
                        {
                            Window.Navigate(string.Format("/ECCentral.Portal.UI.Common/TariffMaintanin/{0}", VM.SysNo), null, false);
                            Window.Refresh();
                        }
                    });
                });
            }
            else
            {
                facade.CreateTariffInfo(VM, (obj, args) =>
                {
                    int SysNo = args.Result.SysNo.Value;
                    Window.Alert("提示", "保存成功!", MessageType.Information, (obj2, args2) =>
                    {
                        if (args2.DialogResult == DialogResultType.Cancel)
                        {
                            Window.Navigate(string.Format("/ECCentral.Portal.UI.Common/TariffMaintanin/{0}", SysNo), null, true
                                );
                            Window.Close();
                        }
                    });
                });
            }
        }
    }
}
