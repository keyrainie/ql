using System;
using System.Linq;

using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.UI.MKT.Resources;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = false)]
    public partial class SaleAdvTemplateMaintain : PageBase
    {
        private int sysNo;

        public SaleAdvertisementVM VM
        {
            get
            {
                return this.DataContext as SaleAdvertisementVM;
            }
            private set
            {
                this.DataContext = value;
            }
        }       
 
        public SaleAdvTemplateMaintain()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            string no = Request.Param;
            if (!string.IsNullOrEmpty(no))
            {               
                //this.btnMaintainItems.Visibility = System.Windows.Visibility.Visible;

                if (int.TryParse(no, out sysNo))
                {
                    new SaleAdvTemplateFacade(this).Load(sysNo, (obj, args) =>
                    {
                        this.VM = args.Result;
                        this.VM.ChannelID = "1";

                        if (!string.IsNullOrEmpty(this.VM.IsHold) && this.VM.IsHold.ToUpper() == "Y")
                        {
                            this.btnUnLock.Visibility = System.Windows.Visibility.Visible;
                        }
                        else
                        {
                            this.btnLock.Visibility = System.Windows.Visibility.Visible;
                        }
                    });
                }
            }
            else
            {
                this.VM = new SaleAdvertisementVM();
                this.VM.ChannelID = "1";
            }         
        }

        private void btnSave_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.VM.IsTableType)
            {
                this.VM.Type = ShowType.Table;
            }
            if (this.VM.IsImageTextType)
            {
                this.VM.Type = ShowType.ImageText;
            }
            if (ValidationManager.Validate(this.LayoutRoot))
            {
                if (this.VM.BeginDate == null)
                {
                    Window.Alert(ResSaleAdvTemplateMaintain.Validate_StartDateRequired, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Warning);
                    return;
                }
                if (this.VM.EndDate == null)
                {
                    Window.Alert(ResSaleAdvTemplateMaintain.Validate_EndDateRequired, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Warning);
                    return;
                }
                if (this.VM.BeginDate > this.VM.EndDate)
                {
                    Window.Alert("开始时间不能大于结束时间!", Newegg.Oversea.Silverlight.Controls.Components.MessageType.Warning);
                    return;
                }

                if (this.VM.SysNo == null)
                {
                    new SaleAdvTemplateFacade(this).Create(this.VM, (obj, args) =>
                    {
                        Window.Alert(ResSaleAdvTemplateMaintain.Info_Sucessfully);
                    });
                }
                else if (this.VM.SysNo > 0)
                {
                    new SaleAdvTemplateFacade(this).Update(this.VM, (obj, args) =>
                    {
                        Window.Alert(ResSaleAdvTemplateMaintain.Info_Sucessfully);
                    });
                }
            }
        }

        private void btnNew_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.VM = new SaleAdvertisementVM();
            this.VM.ChannelID = "1";

            btnLock.Visibility = System.Windows.Visibility.Collapsed;
            btnUnLock.Visibility = System.Windows.Visibility.Collapsed;
            //btnMaintainItems.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void btnLock_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            new SaleAdvTemplateFacade(this).Lock(this.VM.SysNo.Value, (obj, args) =>
            {
                btnLock.Visibility = System.Windows.Visibility.Collapsed;
                btnUnLock.Visibility = System.Windows.Visibility.Visible;
                Window.Alert(ResSaleAdvTemplateMaintain.Info_Sucessfully);
            });
        }

        private void btnUnLock_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            new SaleAdvTemplateFacade(this).UnLock(this.VM.SysNo.Value, (obj, args) =>
            {
                btnLock.Visibility = System.Windows.Visibility.Visible;
                btnUnLock.Visibility = System.Windows.Visibility.Collapsed;
                Window.Alert(ResSaleAdvTemplateMaintain.Info_Sucessfully);
            });
        }

        private void btnMaintainItems_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Window.Navigate(string.Format(ConstValue.MKT_SaleAdvTemplateItemMaintainUrlFormat, this.VM.SysNo));
        }
    }
}