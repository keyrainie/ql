using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.MKT.Models;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Resources;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ProductRecommendMaintain : PageBase
    {
        ProductRecommendVM _currentVM;
        /// <summary>
        /// 标识界面是否处于编辑记录模式
        /// </summary>
        private bool _isEditing;

        private string _positionID;

        public ProductRecommendMaintain()
        {
            InitializeComponent();
           
         }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            //初始化页面数据
            this.lstChannel.ItemsSource = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            this.rbInvalid.Content = EnumConverter.GetDescription(ADStatus.Deactive, typeof(ADStatus));
            this.rbValid.Content = EnumConverter.GetDescription(ADStatus.Active, typeof(ADStatus));
            //初始化DataContext
            _isEditing = !string.IsNullOrWhiteSpace(this.Request.Param);
            if (_isEditing)
            {
                new ProductRecommendFacade(this).Load(this.Request.Param, (s, args) =>
                {
                    if (args.FaultsHandle()) return;
                    _currentVM = args.Result.Convert<ProductRecommendInfo, ProductRecommendVM>((info, vm) =>
                    {
                        vm.LocationVM = info.Location.Convert<ProductRecommendLocation, ProductRecommendLocationVM>();
                        if (info.WebChannel != null)
                        {
                            vm.ChannelID = info.WebChannel.ChannelID;
                        }
                    });
                    this.ucPageType.PageTypeLoadCompleted += new EventHandler(ucPageType_PageTypeLoadCompleted);
                    this.ucPageType.PageLoadCompleted += new EventHandler(ucPageType_PageLoadCompleted);
                    this.ucPageType.PagePositionLoadCompleted += new EventHandler(ucPageType_PagePositionLoadCompleted);
                    this.Grid.DataContext = _currentVM;
                    _positionID = _currentVM.LocationVM.PositionID;
                    lstPosition.SelectionChanged -= lstPosition_SelectionChanged;
                });
            }
            else
            {
                _currentVM = new ProductRecommendVM();
                this.Grid.DataContext = _currentVM;
                this.lstChannel.SelectedIndex = 0;
                this.lstPosition.SelectedIndex = 0;
                this.ucPageType.ExtendC3Visibility = System.Windows.Visibility.Visible;
            }
        }

        void ucPageType_PageLoadCompleted(object sender, EventArgs e)
        {
            this.ucPageType.SetPageID(_currentVM.LocationVM.PageID);
        }

        void ucPageType_PageTypeLoadCompleted(object sender, EventArgs e)
        {
            this.ucPageType.SetPageType(_currentVM.LocationVM.PageType);
        }

        void ucPageType_PagePositionLoadCompleted(object sender, EventArgs e)
        {
            if (_currentVM.LocationVM != null && _isEditing)
            {
                lstPosition.SelectedValue = _positionID;
                lstPosition.SelectionChanged += lstPosition_SelectionChanged;
            }
        }
        private void ButtonNew_Click(object sender, RoutedEventArgs e)
        {
            this.ButtonSave.IsEnabled = true;
            _currentVM = new ProductRecommendVM();
            this.Grid.DataContext = _currentVM;
            _isEditing = false;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            _currentVM.LocationVM.PageType = ucPageType.PageType;
            _currentVM.LocationVM.PageID = ucPageType.PageID;
            _currentVM.IsExtendValid = ucPageType.IsExtendValid;

            //输入验证不通过，则直接返回
            if (!ValidationManager.Validate(this)) return;

            var beginDate = Convert.ToDateTime(_currentVM.BeginDate);
            var endDate = Convert.ToDateTime(_currentVM.EndDate);
            if (beginDate > endDate)
            {
                this.Window.Alert("生效日期小于失效日期!");
                return; 
            }
            if (string.IsNullOrWhiteSpace(_currentVM.ProductID))
            {
                this.Window.Alert(ResProductRecommend.Info_PleaseSelectProduct);
                return;
            }

            var facade = new ProductRecommendFacade(this);
            if (_isEditing)
            {
                facade.Update(_currentVM, (s, args) =>
                {
                    if (args.FaultsHandle()) return;
                    this.Window.Alert(ResProductRecommend.Info_EditSuccess);
                });
            }
            else
            {
                facade.Create(_currentVM, (s, args) =>
                {
                    if (args.FaultsHandle()) return;
                    this.Window.Alert(ResProductRecommend.Info_AddSuccess);
                    this.ButtonSave.IsEnabled = false;
                });
            }
        }

        private void lstPosition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var kv = this.lstPosition.SelectedItem as CodeNamePair;
            if (kv == null)
            {
                this.txtDescription.Text = "";
                return;
            }
            string selected = kv.Name;
            var index = selected.IndexOf("--");
            var modelName = string.Empty;
            if (index < 0)
            {
                modelName = selected;
            }
            else
            {
                modelName = selected.Substring(index + 2);
            }
            this.txtDescription.Text = modelName;
        }

        /// <summary>
        /// 初始化模块名
        /// </summary>
        /// <returns></returns>
        private void InitModuleName()
        {
            if (_currentVM.LocationVM != null && _isEditing)
            {
                var description = _currentVM.LocationVM.Description;
                this.txtDescription.Text = description;
            }
        }

     
    }

}
