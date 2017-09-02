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
using ECCentral.Portal.Basic.Components.UserControls.ProductPicker;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using System.Collections.ObjectModel;
using ECCentral.Portal.UI.IM.Facades;



namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductRelateBatSet : UserControl
    {
        private ObservableCollection<ProductVM> listSource;
        private ObservableCollection<ProductVM> listRelate;
        public IDialog Dialog { get; set; }
        private ProductRelatedFacade facade;
        public ProductRelateBatSet()
        {
            InitializeComponent();
            this.Loaded += (sender, e) => 
            {
                listSource = new ObservableCollection<ProductVM>();
                listRelate = new ObservableCollection<ProductVM>();
                facade = new ProductRelatedFacade();
            };
          
           
        }

      
        private void CloseDialog(DialogResultType dialogResult)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = dialogResult;
                Dialog.Close();
            }
        }

        /// <summary>
        /// 添加主商品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UCProductSearch item = new UCProductSearch();
            bool flag = false;
            item.SelectionMode = SelectionMode.Multiple;
            item.DialogHandler = CPApplication.Current.CurrentPage.Context.Window.ShowDialog("选择商品", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    if (listSource.Count == 0)
                    {
                        listSource = item._viewModel.SelectedProducts;
                    }
                    else
                    {
                        foreach (var product in item._viewModel.SelectedProducts)
                        {
                            flag = false;
                            foreach (var l in listSource)
                            {
                                if (product.SysNo == l.SysNo)
                                {
                                    flag = true;
                                    break;
                                }
                            }
                            if (!flag)
                            {
                                listSource.Add(product);
                            }
                        }
                    }
                    this.ItemRelatedQueryResult.ItemsSource = listSource;
                  
                }
            }, new Size(850, 600));

          
        }

        /// <summary>
        /// 添加相关商品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            UCProductSearch item = new UCProductSearch();
            bool flag = false;
            item.SelectionMode = SelectionMode.Multiple;
            item.DialogHandler = CPApplication.Current.CurrentPage.Context.Window.ShowDialog("选择商品", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    if (listRelate.Count == 0)
                    {
                        listRelate = item._viewModel.SelectedProducts;
                    }
                    else
                    {
                        foreach (var product in item._viewModel.SelectedProducts)
                        {
                            flag = false;
                            foreach (var l in listRelate)
                            {
                                if (product.SysNo == l.SysNo)
                                {
                                    flag = true;
                                    break;
                                }
                            }
                            if (!flag)
                            {
                                listRelate.Add(product);
                            }
                        }
                    }
                    this.ItemQueryResult.ItemsSource = listRelate;

                }
            }, new Size(850, 600));
        }

        /// <summary>
        /// 相关商品的移除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
             int sysNo=(int)((HyperlinkButton)sender).Tag;
            var datatemp = from p in listRelate where p.SysNo != sysNo select p;
              listRelate = new ObservableCollection<ProductVM>(datatemp);
             this.ItemQueryResult.ItemsSource = listRelate;
            
        }

        /// <summary>
        /// 主商品的移除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HyperlinkButton_Click_1(object sender, RoutedEventArgs e)
        {
            int sysNo = (int)((HyperlinkButton)sender).Tag;
            var datatemp = from p in listSource where p.SysNo != sysNo select p;
            listSource = new ObservableCollection<ProductVM>(datatemp);
            this.ItemRelatedQueryResult.ItemsSource = listSource;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            bool IsMultiple = this.IsMultiple.IsChecked==true?true:false; //CheckBox有null的情况
            List<ECCentral.Portal.UI.IM.Models.ProductRelatedVM> list = new List<ECCentral.Portal.UI.IM.Models.ProductRelatedVM>();
   
               foreach (var item in listSource)
                {
                   foreach (var r in listRelate)
	                {
                      
		              list.Add(new ECCentral.Portal.UI.IM.Models.ProductRelatedVM(){IsMutual=IsMultiple,ProductSysNo=item.SysNo.ToString(),RelatedProductSysNo=r.SysNo.ToString(),Priority="0",ProductID=item.ProductID,RelatedProductID=r.ProductID});
	                }
		        }
               if (list.Count > 0)
               {
                   facade.CreateItemRelatedByList(list, (obj, arg) =>
                   {
                       if (arg.FaultsHandle())
                       {
                           return;
                       }
                       CPApplication.Current.CurrentPage.Context.Window.Alert("批量设置成功!");
                   });
               }
               else
               {
                   CPApplication.Current.CurrentPage.Context.Window.Alert("没有记录",MessageType.Error);
               }

        }
    }
}
