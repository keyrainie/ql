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
using ECCentral.Portal.UI.IM.Facades;
using Newegg.Oversea.Silverlight.Controls.Data;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.BizEntity.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System.Windows.Media.Imaging;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.IM.Resources;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class BrandAuthorizedMaintain : UserControl
    {
        BrandFacade facade;
        public int BrandSysNo { private get; set; }
        private List<BrandAuthorizedVM> BrandAuthorizedList;
        private BrandAuthorizedVM model;
        private bool isExist=false; //图片地址是否存在
        public BrandAuthorizedMaintain()
        {
            InitializeComponent();
            this.BrandAuthorizedResult.LoadingDataSource += new EventHandler<LoadingDataEventArgs>(BrandAuthorizedResult_LoadingDataSource);
            this.Loaded += (e, sender) =>
            {
                facade = new BrandFacade();
                model = new BrandAuthorizedVM();
                this.DataContext = model;
                this.BrandAuthorizedResult.Bind();
            };
        }



        void BrandAuthorizedResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            facade.GetBrandAuthorizedByBrandSysNo(BrandSysNo, e.PageSize, e.PageIndex, e.SortField, (obj, arg) =>
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                this.BrandAuthorizedResult.ItemsSource = arg.Result.Rows.ToList("IsChecked", false); ;
                this.BrandAuthorizedResult.TotalCount = arg.Result.TotalCount;
            });
        }


        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb != null)
            {
                dynamic viewlist = this.BrandAuthorizedResult.ItemsSource as dynamic;
                if (viewlist != null)
                {
                    foreach (var item in viewlist)
                    {
                        item.IsChecked = cb.IsChecked != null ? cb.IsChecked.Value : false;
                    }
                }
            }
        }

        private void hyperlinkBrandID_Click(object sender, RoutedEventArgs e)
        {
            dynamic d = this.BrandAuthorizedResult.SelectedItem as dynamic;
            this.DataContext = new BrandAuthorizedVM()
            {
                AuthorizedAcive = d.Status == AuthorizedStatus.Active ? true : false,
                AuthorizedDeAcive = d.Status == AuthorizedStatus.DeActive ? true : false,
                Category1SysNo = d.Category1SysNo,
                Category2SysNo = d.Category2SysNo,
                Category3SysNo = d.Category3SysNo,
                ImageName = d.ImageName,
                StartActiveTime = d.StartActiveTime,
                EndActiveTime = d.EndActiveTime,
                SysNo = d.SysNo,
                BrandSysNo = d.BrandSysNo,
                ReferenceSysNo = d.ReferenceSysNo

            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            BrandAuthorizedList = new List<BrandAuthorizedVM>();
            BrandAuthorizedVM vm = this.DataContext as BrandAuthorizedVM;
            if (!ValidationManager.Validate(this))
            {
                return;
            }
            if (vm.StartActiveTime != null && vm.EndActiveTime != null)
            {
                if (vm.EndActiveTime.Value.CompareTo(vm.StartActiveTime) < 0)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResBrandAuthorizedMaintain.EndTimeNotLessStartTime, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Error);
                    return;
                }
            }
            if (vm.Category2SysNo == null)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("授权限制最低为二级类别!", Newegg.Oversea.Silverlight.Controls.Components.MessageType.Error);
                return;
            }
            if (vm.Category2SysNo > 0 && vm.Category3SysNo == null) //只选择了二级类别
            {

                //找到选择的二级类别
                var category = (from p in myUCCategoryPicker.Category2List where p.SysNo == vm.Category2SysNo select p).FirstOrDefault();
              

                
                //将二级类别放在list中 然后一起处理
                BrandAuthorizedList.Add(GetBrandAuthorizedVM(2, vm.Category2SysNo,vm,category.CategoryName.Content));
                CPApplication.Current.CurrentPage.Context.Window.Confirm("是否批量保存到二级类别下相关的三级类别授权牌！", (obj, arg) =>
                {
                    if (arg.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                    {
                        var list = from p in myUCCategoryPicker.Category3List where p.ParentSysNumber == vm.Category2SysNo select p; //找到该2级类别下所有的3级类别
                        //将所有三级类别放在list中
                        foreach (var item in list)
                        {
                            BrandAuthorizedList.Add(GetBrandAuthorizedVM(3, item.SysNo, vm,item.CategoryName.Content));
                        }
                        ExecAuthorized(0);
                    }
                });

            }
            if (vm.Category2SysNo > 0 && vm.Category3SysNo > 0)
            {
                //找到选择的三级类别
                var category = (from p in myUCCategoryPicker.Category3List where p.SysNo == vm.Category3SysNo select p).FirstOrDefault();
                //在将选择的三级类放在list中
                BrandAuthorizedList.Add(GetBrandAuthorizedVM(3, vm.Category3SysNo, vm,category.CategoryName.Content));
                ExecAuthorized(0);


            }
        }

        /// <summary>
        /// 批量设置有效
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            List<BrandAuthorizedVM> list = GetCheckedList(true);
            if (list.Count > 0)
            {
                facade.UpdateBrandAuthorized(list, (obj, arg) =>
                {
                    if (arg.FaultsHandle())
                    {
                        return;
                    }
                    CPApplication.Current.CurrentPage.Context.Window.Alert("提交成功");
                    this.BrandAuthorizedResult.Bind();
                });
            }
            else
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("没有选择记录", Newegg.Oversea.Silverlight.Controls.Components.MessageType.Error);
            }
        }

        /// <summary>
        /// 批量设置无效
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            List<BrandAuthorizedVM> list = GetCheckedList(false);
            if (list.Count > 0)
            {
                facade.UpdateBrandAuthorized(list, (obj, arg) =>
                {
                    if (arg.FaultsHandle())
                    {
                        return;
                    }
                    CPApplication.Current.CurrentPage.Context.Window.Alert("提交成功");
                    this.BrandAuthorizedResult.Bind();
                });
            }
            else
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("没有选择记录", Newegg.Oversea.Silverlight.Controls.Components.MessageType.Error);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            dynamic d = this.BrandAuthorizedResult.ItemsSource as dynamic;
            List<int> list = new List<int>();
            foreach (var item in d)
            {
                if (item.IsChecked == true)
                {
                    list.Add(item.SysNo);
                }
            }
            if (list.Count > 0)
            {

                facade.DeleteBrandAuthorized(list, (obj, arg) =>
                {
                    if (arg.FaultsHandle())
                    {
                        return;
                    }
                    CPApplication.Current.CurrentPage.Context.Window.Alert("提交成功");
                    this.BrandAuthorizedResult.Bind();
                });
            }
            else
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("没有选择记录", Newegg.Oversea.Silverlight.Controls.Components.MessageType.Error);
            }
        }
        /// <summary>
        /// 选中数据集合
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        private List<BrandAuthorizedVM> GetCheckedList(bool Acive)
        {
            dynamic d = this.BrandAuthorizedResult.ItemsSource as dynamic;
            List<BrandAuthorizedVM> list = new List<BrandAuthorizedVM>();
            foreach (var item in d)
            {
                if (item.IsChecked == true)
                {
                    list.Add(new BrandAuthorizedVM()
                    {
                        AuthorizedAcive=Acive,
                        AuthorizedDeAcive=!Acive,
                        SysNo = item.SysNo,
                    });
                }
            }
            return list;
        }

        /// <summary>
        /// 递归执行授权
        /// </summary>
        private void ExecAuthorized(int index)
        {

            facade.IsExistBrandAuthorized(BrandAuthorizedList[index], (objs, args) => //授权是否存在
            {
                if (args.Result == true) //授权存在
                {
                    BrandAuthorizedList[index].IsExist = true;
                    CPApplication.Current.CurrentPage.Context.Window.Confirm(string.Format("该品牌与类别：{0}已经授权,是否覆盖！", BrandAuthorizedList[index].ReferenceName), (o, a) => //是否覆盖授权
                    {
                        if (a.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK) //覆盖
                        {
                            facade.InsertBrandAuthorized(BrandAuthorizedList[index], (mobj, marg) =>
                            {
                                if (marg.FaultsHandle())
                                {
                                    return;
                                }
                                index = index + 1;
                                if (index < BrandAuthorizedList.Count)
                                {
                                    ExecAuthorized(index);
                                }
                                else
                                {
                                    CPApplication.Current.CurrentPage.Context.Window.Alert("提交成功");
                                    this.BrandAuthorizedResult.Bind();
                                }
                            });
                        }
                    });
                }
                else
                {
                    BrandAuthorizedList[index].IsExist = false;
                    facade.InsertBrandAuthorized(BrandAuthorizedList[index], (mobj, marg) =>
                    {
                        if (marg.FaultsHandle())
                        {
                            return;
                        }
                        index = index + 1;
                        if (index < BrandAuthorizedList.Count)
                        {
                            ExecAuthorized(index);
                        }
                        else
                        {
                            CPApplication.Current.CurrentPage.Context.Window.Alert("提交成功");
                            this.BrandAuthorizedResult.Bind();
                        }
                    });
                }
            });

        }

        /// <summary>
        /// 得到VM
        /// </summary>
        /// <param name="type">三级类或二级类</param>
        /// <param name="ReferenceSysNo">关联类别的SysNo</param>
        /// <returns></returns>
        private BrandAuthorizedVM GetBrandAuthorizedVM(int type, int? ReferenceSysNo,BrandAuthorizedVM vm,string name)
        {
            return new BrandAuthorizedVM()
            {
                AuthorizedAcive = vm.AuthorizedAcive,
                AuthorizedDeAcive = vm.AuthorizedDeAcive,
                BrandSysNo = BrandSysNo,
                EndActiveTime = vm.EndActiveTime,
                ImageName = vm.ImageName,
                ReferenceSysNo = ReferenceSysNo,
                StartActiveTime = vm.StartActiveTime,
                SysNo = vm.SysNo,
                Type = type,
                ReferenceName=name
            };
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            BrandAuthorizedVM vm = this.DataContext as BrandAuthorizedVM;
            if (string.IsNullOrEmpty(vm.ImageName))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("图片路径不能为空!", Newegg.Oversea.Silverlight.Controls.Components.MessageType.Error);
                return;
            }
            //Ocean.20130514, Move to ControlPanelPConfiguration
            string urlFormat = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_ExternalSYS, ConstValue.ConfigKey_External_BrandAuthorizedImageUrl);
            string url = string.Format(urlFormat, vm.ImageName);
            facade.UrlIsExist(url, (obj, arg) => 
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                if (arg.Result)
                {
                    this.spImage.Visibility = Visibility.Visible;
                    this.img.Source = new BitmapImage(new Uri(url, UriKind.RelativeOrAbsolute));
                }
                else
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("获取图片失败!", Newegg.Oversea.Silverlight.Controls.Components.MessageType.Error);
                }
            });
           
             
        }
        

    }

}
