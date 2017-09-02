using System.Windows;
using System.Windows.Controls;
using ECCentral.BizEntity.IM;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.IM.Resources;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ManufacturerRelationMaintain : UserControl
    {
        private ManufacturerRelationFacade facade;

        public ManufacturerRelationVM Data { get; set; }
        public IDialog Dialog { get; set; }
        /// <summary>
        /// 编辑生产商的SysNo
        /// </summary>
        public int LocalManufacturerSysNo { get; set; }


        public ManufacturerRelationMaintain()
        {
            InitializeComponent();
            this.Loaded += (sender, e) =>
            {
                facade = new ManufacturerRelationFacade();

                facade.GetManufacturerRelationInfoByLocalManufacturerSysNo(LocalManufacturerSysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    if (args.Result == null)
                    {
                        return;
                    }
                    ManufacturerRelationVM vm = new ManufacturerRelationVM()
                    {
                        SysNo = args.Result.SysNo,
                        LocalManufacturerSysNo = args.Result.LocalManufacturerSysNo,
                        NeweggManufacturer = args.Result.NeweggManufacturer,
                        AmazonManufacturer = args.Result.AmazonManufacturer,
                        EBayManufacturer = args.Result.EBayManufacturer,
                        OtherManufacturerSysNo = args.Result.OtherManufacturerSysNo                        
                    };
                    this.DataContext = vm;
                });
            };
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
        }
        private void CloseDialog(DialogResultType dialogResult)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = dialogResult;
                Dialog.Close();
            }
        }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this.expander1))
            {
                return;

            }
            ManufacturerRelationVM model = this.DataContext as ManufacturerRelationVM;
            //if (string.IsNullOrEmpty(model.ManufacturerBriefName) && string.IsNullOrEmpty(model.ManufacturerName))
            //{
            //    CPApplication.Current.CurrentPage.Context.Window.Alert("生产商中文名称和英文名称必填一个!");
            //    return;
            //}
            //if (model.BrandStoreType == BrandStoreType.FlagshipStore && string.IsNullOrEmpty(model.BrandImage))
            //{
            //    CPApplication.Current.CurrentPage.Context.Window.Alert("店铺类型为旗舰店的时,品牌店广告图不能为空");
            //    return;
            //}

            CPApplication.Current.CurrentPage.Context.Window.Confirm("是否同步生产商下所有信息", (obj, arg) =>
            {
                facade.UpdateManufacturer(model, (o, a) =>
                {
                    if (a.FaultsHandle())
                    {
                        return;
                    }
                    CPApplication.Current.CurrentPage.Context.Window.Alert("提交成功");
                    CloseDialog(DialogResultType.OK);
                });


            });




        }
    }
}
