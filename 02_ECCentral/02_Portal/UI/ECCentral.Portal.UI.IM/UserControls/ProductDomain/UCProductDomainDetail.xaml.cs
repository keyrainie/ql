using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

using ECCentral.BizEntity.Enum.Resources;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.QueryFilter.IM;
using ECCentral.Portal.Basic.Utilities;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.IM;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class UCProductDomainDetail : UserControl
    {
        private int loadCompletedCount = 0;
        private bool hasSaved;

        public IDialog Dialog { get; set; }

        public ProductDomainVM VM { get; private set; }        

        public UCProductDomainDetail()
        {
            InitializeComponent();

            Loaded += new RoutedEventHandler(UCProductDomainDetail_Loaded);
        }

        public UCProductDomainDetail(ProductDomainVM vm)
            : this()
        {
            this.VM = vm;
        }

        void UCProductDomainDetail_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCProductDomainDetail_Loaded);

            var queryFilter = new ProductManagerQueryFilter()
               {
                   PMQueryType = ECCentral.BizEntity.Common.PMQueryType.All.ToString(),
                   UserName = CPApplication.Current.LoginUser.LoginName,
                   CompanyCode = CPApplication.Current.CompanyCode
               };
            new PMQueryFacade(CPApplication.Current.CurrentPage).QueryPMList(queryFilter, (obj, args) =>
            {
                args.Result.ForEach(p =>
                {
                    DepartmentMerchandiserVM merchandiserVM = new DepartmentMerchandiserVM
                    {                                  
                        SysNo = p.SysNo,
                        DisplayName=p.UserInfo.UserDisplayName
                    };
                    var sysNo = this.VM.DepartmentMerchandiserSysNoList.FirstOrDefault(q => q == merchandiserVM.SysNo.Value);
                    if (sysNo != null)
                    {
                        merchandiserVM.IsChecked = true;
                    }
                    this.VM.DepartmentMerchandiserListForUI.Add(merchandiserVM);
                });                                

                Interlocked.Increment(ref loadCompletedCount);

                SetDataContext();
            });

            new PMQueryFacade(CPApplication.Current.CurrentPage).QueryPMLeaderList((obj, args) =>
            {
                //此处因为Oversea的Combox控件的SelectedValuePath不支持多级(类似UserInfo.SysNo)，绑定会报错，所以用dynamic对象替代
                List<PMLeaderInfo> list = new List<PMLeaderInfo>();
                args.Result.ForEach(p =>
                {
                    PMLeaderInfo leader = new PMLeaderInfo { SysNo = p.UserInfo.SysNo, UserDisplayName = p.UserInfo.UserDisplayName };
                    list.Add(leader);
                });
                list.Insert(0, (new PMLeaderInfo { SysNo = default(int?), UserDisplayName = ResCommonEnum.Enum_All }));

                cmbPMLeaders.ItemsSource = list;

                Interlocked.Increment(ref loadCompletedCount);

                SetDataContext();
            });

            if (this.VM.SysNo > 0)
            {                
                //获取Category列表
                new ProductDomainFacade(CPApplication.Current.CurrentPage).LoadDomainCategorys(this.VM.SysNo.Value, (obj, args) =>
                {
                    if (args.Result != null)
                    {
                        args.Result.ForEach(p =>
                        {
                            this.VM.DepartmentCategoryList.Add(p.Convert<ProductDepartmentCategory, ProductDepartmentCategoryVM>());
                        });
                    }
                });
            }            
        }

        private void SetDataContext()
        {
            if (loadCompletedCount == 2)
            {
                this.DataContext = this.VM;
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var vm = (sender as HyperlinkButton).DataContext as ProductDepartmentCategoryVM;
            vm.ProductDomainSysNo = this.VM.SysNo;

            //DeepClone
            var clonedVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<ProductDepartmentCategoryVM>(vm);

            UCProductDepartmentCategory uc = new UCProductDepartmentCategory(clonedVM);
            IDialog dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog("修改分类信息", uc, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    var category = args.Data as ProductDepartmentCategoryVM;
                    var origin = this.VM.DepartmentCategoryList.FirstOrDefault(p => p.SysNo == category.SysNo);
                    if ( origin!= null)
                    {
                        int index = this.VM.DepartmentCategoryList.IndexOf(origin);
                        this.VM.DepartmentCategoryList.RemoveAt(index);
                        this.VM.DepartmentCategoryList.Insert(index, category);
                    }
                    else
                    {
                        this.VM.DepartmentCategoryList.Add(category);
                    }
                }
            });
            uc.Dialog = dialog;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var vm = (sender as HyperlinkButton).DataContext as ProductDepartmentCategoryVM;
            CPApplication.Current.CurrentPage.Context.Window.Confirm("确定要删除吗?", (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    new ProductDomainFacade(CPApplication.Current.CurrentPage).DeleteDomainCategory(vm.SysNo.Value, (o, a) =>
                    {
                        this.VM.DepartmentCategoryList.Remove(vm);
                        CPApplication.Current.CurrentPage.Context.Window.Alert("操作成功！");
                    });
                }
            });
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.gridContainer))
            {
                if ((this.VM.SysNo ?? 0) > 0)
                {
                    new ProductDomainFacade(CPApplication.Current.CurrentPage).UpdateDomain(this.VM, (obj, args) =>
                    {
                        this.hasSaved = true;

                        CPApplication.Current.CurrentPage.Context.Window.Alert("操作成功!");
                    });
                }
                else
                {
                    new ProductDomainFacade(CPApplication.Current.CurrentPage).CreateDomain(this.VM, (obj, args) =>
                    {
                        this.VM.SysNo = args.Result.SysNo;

                        this.hasSaved = true;

                        CPApplication.Current.CurrentPage.Context.Window.Alert("操作成功!");
                    });
                }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Dialog.ResultArgs = new ResultEventArgs { DialogResult = hasSaved ? DialogResultType.OK : DialogResultType.Cancel };
            this.Dialog.Close();
        }

        private void btnAddCategory_Click(object sender, RoutedEventArgs e)
        {
            if (!this.VM.SysNo.HasValue)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("必须保存ProductDomain信息后才能添加分类!", MessageType.Warning);
                return;
            }
            ProductDepartmentCategoryVM vm = new ProductDepartmentCategoryVM();
            vm.ProductDomainSysNo = this.VM.SysNo;
            UCProductDepartmentCategory uc = new UCProductDepartmentCategory(vm);
            IDialog dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog("添加分类信息", uc, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    this.VM.DepartmentCategoryList.Add(args.Data as ProductDepartmentCategoryVM);
                }
            });
            uc.Dialog = dialog;
        }
    }
}
