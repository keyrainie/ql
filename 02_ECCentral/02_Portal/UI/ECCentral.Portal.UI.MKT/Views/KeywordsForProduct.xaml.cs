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
using ECCentral.Portal.UI.MKT.UserControls.Keywords;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.UI.MKT.Resources;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.MKT.Views
{
    /// <summary>
    /// 关键字对应商品
    /// </summary>
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class KeywordsForProduct : PageBase
    {
        private KeyWordsForProductQueryFacade facade;
        private KeyWordsForProductQueryFilter filter;
        private List<KeyWordsForProductQueryVM> gridVM;
        private KeyWordsForProductQueryVM model;
        private KeyWordsForProductQueryFilter filterVM;

        public KeywordsForProduct()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            facade = new KeyWordsForProductQueryFacade(this);
            filter = new KeyWordsForProductQueryFilter();
            model = new KeyWordsForProductQueryVM();
            model.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
            //comEditUserList
            //comCreateUserType.ItemsSource = EnumConverter.GetKeyValuePairs<KeywordsOperateUserType>(EnumConverter.EnumAppendItemType.All);
            cbShowStatus.ItemsSource = new List<KeyValuePair<ADStatus?, string>>() 
            {
                new KeyValuePair<ADStatus?, string>(null,"--所有--"),
                new KeyValuePair<ADStatus?, string>(ADStatus.Active,"有效"),
                new KeyValuePair<ADStatus?, string>(ADStatus.Deactive,"无效"),
            };
            model.ChannelID = "1";
            QuerySection.DataContext = model;
            base.OnPageLoad(sender, e);
        }
        /// <summary>
        /// 数据全部导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QueryResultGrid_ExportAllClick(object sender, EventArgs e)
        {
            if (filterVM == null || this.QueryResultGrid.TotalCount < 1)
            {
                Window.Alert(ResKeywords.Information_ExportFailed);
                return;
            }
            ColumnSet col = new ColumnSet(this.QueryResultGrid);
            filter = model.ConvertVM<KeyWordsForProductQueryVM, KeyWordsForProductQueryFilter>();
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };
            facade.ExportExcelFile(filterVM, new ColumnSet[] { col });
        }

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            facade.QueryKeyWordsForProduct(QueryResultGrid.QueryCriteria as KeyWordsForProductQueryFilter, e.PageSize, e.PageIndex, e.SortField, (s, args) =>
            {
                if (args.FaultsHandle())
                    return;

                gridVM = DynamicConverter<KeyWordsForProductQueryVM>.ConvertToVMList<List<KeyWordsForProductQueryVM>>(args.Result.Rows);
                QueryResultGrid.ItemsSource = gridVM;
                QueryResultGrid.TotalCount = args.Result.TotalCount;
            });	
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddItem_Click(object sender, RoutedEventArgs e)
        {
            UCAddKeywordsForProduct usercontrol = new UCAddKeywordsForProduct();
            usercontrol.Dialog = Window.ShowDialog(ResKeywords.Title_CreateKeywordsForProduct, usercontrol, (obj, args) =>
            {
                filter = model.ConvertVM<KeyWordsForProductQueryVM, KeyWordsForProductQueryFilter>();
                filterVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<KeyWordsForProductQueryFilter>(filter);
                QueryResultGrid.QueryCriteria = this.filter;
                QueryResultGrid.Bind();
            });
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlEdit_Click(object sender, RoutedEventArgs e)
        {
            KeyWordsForProductQueryVM item = this.QueryResultGrid.SelectedItem as KeyWordsForProductQueryVM;
            UCAddKeywordsForProduct usercontrol = new UCAddKeywordsForProduct();
            usercontrol.VM = item;
           
            usercontrol.Dialog = Window.ShowDialog(ResKeywords.Title_EditKeywordsForProduct, usercontrol, (obj, args) =>
            {
                QueryResultGrid.Bind();
            });
        }

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this.QuerySection))
            {
                return;
            }
                filter = model.ConvertVM<KeyWordsForProductQueryVM, KeyWordsForProductQueryFilter>();
                filterVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<KeyWordsForProductQueryFilter>(filter);
                QueryResultGrid.QueryCriteria = this.filter;
                QueryResultGrid.Bind();
            
        }

        private void ckbSelectRow_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb != null)
            {
                dynamic viewlist = this.QueryResultGrid.ItemsSource as dynamic;
                if (viewlist != null)
                {
                    foreach (var item in viewlist)
                    {
                        item.IsChecked = cb.IsChecked != null ? cb.IsChecked.Value : false;
                    }
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            List<int> list = new List<int>();
            dynamic viewlist = this.QueryResultGrid.ItemsSource as dynamic;
            foreach (var item in viewlist)
            {
                if (item.IsChecked == true)
                {
                    list.Add(item.SysNo);
                }
            }
            if (list.Count > 0)
            {
                Window.Confirm("是否删除?", (objs, args) => 
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        facade.DeleteProductKeywords(list, (obj, arg) =>
                        {
                            if (arg.FaultsHandle())
                            {
                                return;
                            }
                            Window.Alert("删除成功");
                            QueryResultGrid.Bind();
                        });
                    }
                });
               
            }
            else
            {
                Window.Alert("请先选择");
            }
        }

        private void btnChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            List<ProductKeywordsInfo> list = new List<ProductKeywordsInfo>();
            dynamic viewlist = this.QueryResultGrid.ItemsSource as dynamic;
            foreach (var item in viewlist)
            {
                if (item.IsChecked == true)
                {
                    list.Add(new ProductKeywordsInfo() { SysNo = item.SysNo, Status = ADStatus.Deactive, User = new BizEntity.Common.UserInfo() { SysNo = CPApplication.Current.LoginUser.UserSysNo,UserName=CPApplication.Current.LoginUser.DisplayName } });
                }
            }
            if (list.Count > 0)
            {
                facade.ChangeProductKeywordsStatus(list, (obj, arg) =>
                      {
                          if (arg.FaultsHandle())
                          {
                              return;
                          }
                          Window.Alert("屏蔽成功!");
                          QueryResultGrid.Bind();
                      });

             }
            else
            {
                Window.Alert("请先选择");
            }
        }

        //private void ckbSelectRow_Click(object sender, RoutedEventArgs e)
        //{
        //    var checkBoxAll = sender as CheckBox;
        //    if (gridVM == null || checkBoxAll == null)
        //        return;
        //    gridVM.ForEach(item =>
        //    {
        //        item.IsChecked = checkBoxAll.IsChecked ?? false;
        //    });
        //}
    }

}
