using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.SO;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.RMA.Facades;
using ECCentral.Portal.UI.RMA.Models;
using ECCentral.Portal.UI.RMA.Resources;
using ECCentral.Portal.UI.RMA.Views;
using ECCentral.Portal.Basic;

using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.RMA.UserControls
{
    public partial class UCSOBasicInfo : UserControl
    {
        private List<ValidationEntity> validationForSOSysNo;
        private ExternalFacade facade;

        public List<CodeNamePair> RMAReasons { get; set; }
        public CreateRMARequest Page { get; set; }

        public UCSOBasicInfo()
        {
            InitializeComponent();

            facade = new ExternalFacade();
            validationForSOSysNo = new List<ValidationEntity>();
            validationForSOSysNo.Add(new ValidationEntity(ValidationEnum.IsNotEmpty, this.txtSOSysNo.Text, ResCreateRequest.Validation_FieldRequired));
            validationForSOSysNo.Add(new ValidationEntity(ValidationEnum.IsInteger, this.txtSOSysNo.Text, ResCreateRequest.Validation_Integer));
            
            Loaded += new System.Windows.RoutedEventHandler(SOBasicInfo_Loaded);
        }

        void SOBasicInfo_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Loaded -= new System.Windows.RoutedEventHandler(SOBasicInfo_Loaded);

            CodeNamePairHelper.GetList(ConstValue.DomainName_RMA, "RMAReason", CodeNamePairAppendItemType.None, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                this.RMAReasons = args.Result;
            });    
        }

        private void txtSOSysNo_KeyDown(object sender, KeyEventArgs e)
        {
            var request = this.DataContext as RequestVM;
            if (e.Key == Key.Enter && ValidationHelper.Validation(this.txtSOSysNo, validationForSOSysNo))
            {
                LoadSoInfo(request);             
            }
        }

        private void cmbShipType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count > 0)
            {
                var vm = this.DataContext as RequestVM;
                if (vm.ShipType != "3" && !string.IsNullOrEmpty(vm.ShipType))//其他快递
                {
                    vm.ShipViaCode = vm.ShipTypes.FirstOrDefault(p => p.Code == vm.ShipType).Name;
                }
                else
                {
                    vm.ShipViaCode = string.Empty;
                }
            }
        }

        private void btnReflash_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var request = this.DataContext as RequestVM;
            if (ValidationHelper.Validation(this.txtSOSysNo, validationForSOSysNo))
            {
                LoadSoInfo(request);
            }
        }

        private void LoadSoInfo(RequestVM request)
        {
            request.ShipViaCode = null;
            request.ShipType = null;
            request.IsCancelVerifyDuplicate = false;
            request.IsRejectRMA = false;
            request.TrackingNumber = null;

            facade.GetSOInfo(int.Parse(this.txtSOSysNo.Text.Trim()), (obj, args) =>
            {
                SOInfo so = args.Result;
                if (so == null)
                {
                    request = new RequestVM
                    {
                        ShipTypes = request.ShipTypes,
                        SOSysNo = this.txtSOSysNo.Text
                    };
                    this.Page.DataContext = request;
                    request.ValidationErrors.Clear();

                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResCreateRequest.Warning_SONotExists, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Warning);
                    return;
                }
                //fix bug 90829   -by jack 2012-10-24
                //if (so.BaseInfo.Status != SOStatus.OutStock)
                //{
                //    CPApplication.Current.CurrentPage.Context.Window.Alert(ResCreateRequest.Warning_SOStatusInvalid, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Warning);
                //    return;
                //}
                var vm = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<RequestVM>(request);
                vm.SOSysNo = this.txtSOSysNo.Text;
                vm.ShipTypes = request.ShipTypes;
                vm.CustomerSysNo = so.BaseInfo.CustomerSysNo;
                vm.Phone = so.ReceiverInfo.Phone;
                vm.Contact = so.ReceiverInfo.Name;
                vm.AreaSysNo = so.ReceiverInfo.AreaSysNo;
                vm.Address = so.ReceiverInfo.Address;

                vm.Registers.Clear();
                so.Items.ForEach(p =>
                {
                    for (var i = 0; i < p.Quantity; i++)
                    {
                        //优惠卷不显示
                        if (p.ProductType == SOProductType.Coupon)
                        {
                            continue;
                        }
                        //by jack 2012-10-17 延保商品 应该挂上主商品的号
                        else if (p.ProductType == SOProductType.ExtendWarranty)
                        {
                            p.ProductSysNo = int.Parse(p.MasterProductSysNo);
                        }

                        RegisterVM register = new RegisterVM();
                        register.BasicInfo.ProductSysNo = p.ProductSysNo;
                        register.BasicInfo.SOItemType = p.ProductType;
                        if (p.ProductType != SOProductType.Product)
                        {
                            string tmpGiftName = string.Empty;
                            switch (p.ProductType)
                            {
                                case SOProductType.Gift:
                                    tmpGiftName = ResCreateRequest.ESOItemType_Gift_Factory;
                                    break;
                                case SOProductType.SelfGift:
                                    tmpGiftName = ResCreateRequest.ESOItemType_Gift_Newegg;
                                    break;
                                case SOProductType.Accessory:
                                    tmpGiftName = ResCreateRequest.ESOItemType_Accessory;
                                    break;
                                case SOProductType.ExtendWarranty:
                                    tmpGiftName = ResCreateRequest.ESOItemType_ExtendWarranty;
                                    break;
                                case SOProductType.Award:
                                    tmpGiftName = ResCreateRequest.ESOItemType_Prize;
                                    break;
                            }
                            if (!tmpGiftName.Equals(string.Empty))
                                register.BasicInfo.ProductName = string.Format("{0}[{1}]", p.ProductName, tmpGiftName);
                        }
                        else
                        {
                            register.BasicInfo.ProductName = p.ProductName;
                        }
                        this.RMAReasons.ForEach(r =>
                        {
                            register.BasicInfo.ReasonTypes.Add(r);
                        });

                        vm.Registers.Add(register);
                    }
                });

                this.Page.DataContext = vm;
                vm.ValidationErrors.Clear();

                facade.GetCustomerInfo(so.BaseInfo.CustomerSysNo.Value, (o, a) =>
                {
                    CustomerInfo customer = a.Result;
                    vm.CustomerID = customer.BasicInfo.CustomerID;
                    vm.CustomerName = customer.BasicInfo.CustomerName;
                });
            });
        }
    }
}
