using System;
using System.Windows;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.ExternalSYS.Facades;
using ECCentral.Portal.UI.ExternalSYS.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Data;
using System.Collections.Generic;
using System.Windows.Controls;
using ECCentral.Portal.UI.ExternalSYS.UserControls;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.ExternalSYS.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class AdvertisingManagement : PageBase
    {
        #region 属性
        public AdvertisingQueryVM FilterVM
        {
            get
            {
                return this.expSearchCondition.DataContext as AdvertisingQueryVM;
            }
            set
            {
                this.expSearchCondition.DataContext = value;
            }
        }
        #endregion

        #region 初始化加载

        public AdvertisingManagement()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            this.FilterVM = new AdvertisingQueryVM();

            AdvertisingFacade service = new AdvertisingFacade(this);
            List<CodeNamePair> codeNames = new List<CodeNamePair>();
            List<CodeNamePair> imgSizes = new List<CodeNamePair>();
            List<CodeNamePair> atSizes = new List<CodeNamePair>();
            service.GetAllProductLineCategory((s, args) =>
            {
                if (!args.FaultsHandle())
                {
                    codeNames.Insert(0, new CodeNamePair { Code = null, Name = ResCommonEnum.Enum_All });
                    foreach (var item in args.Result.Rows)
                    {
                        codeNames.Add(new CodeNamePair { Code = item.SysNo.ToString(), Name = item.Name });
                    }
                    this.cbProductLineCategory.ItemsSource = codeNames;
                }
            });

            new ImageSizeFacade(this).GetAllImageSize(100, 0, "", (obj, arg) =>
            {
                if (!arg.FaultsHandle())
                {
                    imgSizes.Insert(0, new CodeNamePair { Code = null, Name = ResCommonEnum.Enum_All });
                    string sizeTmp = string.Empty;
                    foreach (var item in arg.Result.Rows)
                    {
                        sizeTmp = item.ImageWidth+"*"+item.ImageHeight;
                        imgSizes.Add(new CodeNamePair { Code = sizeTmp, Name = sizeTmp });
                    }
                    this.cbImageSize.ItemsSource = imgSizes;
                    this.cbImageSize.SelectedIndex = 0;
                }
            });
            cbAdvertisingType.ItemsSource = this.FilterVM.AdvertisingTypeList;

            cbProductLineName.ItemsSource = new List<CodeNamePair>() { new CodeNamePair() { Code = null, Name = ResCommonEnum.Enum_All } };
        }



        #endregion

        #region 查询绑定
        private void btnAdvertisingSearch_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
            {
                return;
            }
            if (String.IsNullOrEmpty(wmBeginDateTime.Text)) { dtBeginDateTime.SelectedDateTime = null; }
            if (String.IsNullOrEmpty(wmEndDateTime.Text)) { dtEndDateTime.SelectedDateTime = null; }
            if (this.cbImageSize.SelectedIndex == 0) { FilterVM.ImageWidth = null; FilterVM.ImageHeight = null; }
            this.dgAdvertisingQueryResult.QueryCriteria = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<AdvertisingQueryVM>(this.FilterVM);
            this.dgAdvertisingQueryResult.Bind();
        }

        private void dgAdvertisingQueryResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            if (Newegg.Oversea.Silverlight.Utilities.Validation.ValidationManager.Validate(this.SeachBuilder))
            {
                new AdvertisingFacade(this).Query(this.dgAdvertisingQueryResult.QueryCriteria as AdvertisingQueryVM, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
                {
                    if (!args.FaultsHandle())
                    {
                        this.dgAdvertisingQueryResult.ItemsSource = args.Result.Rows.ToList();
                        this.dgAdvertisingQueryResult.TotalCount = args.Result.TotalCount;
                    }
                });
            }
        }
        #endregion

        #region 页面内按钮处理事件

        #region 界面事件

        private void cbProductLineCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cbProductLineCate = sender as Combox;
            if (cbProductLineCate.SelectedIndex != 0)
            {
                int sysNo = 0;
                if (int.TryParse(cbProductLineCate.SelectedValue.ToString(), out sysNo))
                {
                    List<CodeNamePair> codeNames = new List<CodeNamePair>();
                    new AdvertisingFacade(this).GetProductLineByProductLineCategorySysNo(sysNo, (s, args) =>
                    {
                        if (!args.FaultsHandle())
                        {
                            codeNames.Insert(0, new CodeNamePair { Code = null, Name = ResCommonEnum.Enum_All });
                            foreach (var item in args.Result.Rows)
                            {
                                if (item.flag == 1)
                                {
                                    codeNames.Add(new CodeNamePair { Code = item.ProductLineSysNo.ToString(), Name = item.ProductLineName });
                                }
                            }
                            this.cbProductLineName.ItemsSource = codeNames;
                        }
                    });
                }
            }
        }


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlAdvertisingDelete_Click(object sender, RoutedEventArgs e)
        {
            CPApplication.Current.CurrentPage.Context.Window.Confirm("是否删除?", (obj, arg) =>
            {
                if (arg.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    HyperlinkButton link = (HyperlinkButton)sender;
                    int sysNo = (int)(link.Tag ?? 0);
                    new AdvertisingFacade(this).Delete(sysNo, (objs, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        Window.MessageBox.Show("删除成功!", Newegg.Oversea.Silverlight.Controls.Components.MessageBoxType.Success);
                        this.dgAdvertisingQueryResult.Bind();
                    });
                }
            });
        }

        #endregion

        #endregion

        #region 跳转

        private void btnAdvertisingNew_Click(object sender, RoutedEventArgs e)
        {
            AdvertisingMaintain item = new AdvertisingMaintain();
            item.IsEdit = false;
            item.Dialog = Window.ShowDialog("添加", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    this.dgAdvertisingQueryResult.Bind();
                }
            }, new Size(500, 600));
        }

        private void hlAdvertisingEdit_Click(object sender, RoutedEventArgs e)
        {
            dynamic d = this.dgAdvertisingQueryResult.SelectedItem as dynamic;
            AdvertisingVM vm = new AdvertisingVM()
            {
                Type = d.Type,
                Status = d.Status,
                ProductLineCategorySysNo = d.ProductLineCategorySysNo,
                ProductLineSysNo = d.ProductLineSysNo,
                SharedText = d.SharedText,
                Text = d.Text,
                EventCode = d.EventCode,
                AdCode = d.AdCode,
                Url = d.Url,
                ImageUrl = d.ImageUrl,
                ImageHeight = d.ImageHeight,
                ImageWidth = d.ImageWidth,
                SysNo = d.SysNo
            };
            AdvertisingMaintain item = new AdvertisingMaintain();
            item.IsEdit = true;
            item.Data = vm;
            item.Dialog = Window.ShowDialog("编辑", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    this.dgAdvertisingQueryResult.Bind();
                }
            }, new Size(500, 600));
        }

        #endregion


    }
}
