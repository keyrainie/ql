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
using ECCentral.Portal.UI.MKT.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.BizEntity.Enum.Resources;

namespace ECCentral.Portal.UI.MKT.UserControls
{
    public partial class UCECCategory : UserControl
    {

        private ECCategoryListVM _allECCategoryList;

        public UCECCategory()
        {
            InitializeComponent();
        }

        /// <summary>
        /// C1选择改变事件
        /// </summary>
        public event EventHandler<C1SelectitonChangedEventArgs> C1SelectionChanged;

        /// <summary>
        /// C2选择改变事件
        /// </summary>
        public event EventHandler<C2SelectitonChangedEventArgs> C2SelectionChanged;

        /// <summary>
        /// C3选择改变事件
        /// </summary>
        public event EventHandler<C3SelectitonChangedEventArgs> C3SelectionChanged;

        /// <summary>
        /// 渠道编号
        /// </summary>
        public string ChannelID
        {
            get { return (string)GetValue(ChannelIDProperty); }
            set { SetValue(ChannelIDProperty, value); }
        }
        public static readonly DependencyProperty ChannelIDProperty =
            DependencyProperty.Register("ChannelID", typeof(string), typeof(UCECCategory), new PropertyMetadata(null, (d, e) =>
            {
                UCECCategory ucECCategory = d as UCECCategory;
                ucECCategory.LoadAllECCategory(ucECCategory.ChannelID, null);
            }));

        public ECCategoryLevel ShowLevel
        {
            get { return (ECCategoryLevel)GetValue(ShowLevelProperty); }
            set { SetValue(ShowLevelProperty, value); }
        }

        public static readonly DependencyProperty ShowLevelProperty =
            DependencyProperty.Register("ShowLevel", typeof(ECCategoryLevel), typeof(UCECCategory), new PropertyMetadata(ECCategoryLevel.Category1, (d, args) =>
            {
                var uc = d as UCECCategory;
                //先全部隐藏,然后根据设置选择性的显示
                uc.lstECCategory1.Visibility = Visibility.Collapsed;
                uc.lstECCategory2.Visibility = Visibility.Collapsed;
                uc.lstECCategory3.Visibility = Visibility.Collapsed;
                switch ((ECCategoryLevel)args.NewValue)
                {
                    case ECCategoryLevel.Category3:
                        uc.lstECCategory1.Visibility = Visibility.Visible;
                        uc.lstECCategory2.Visibility = Visibility.Visible;
                        uc.lstECCategory3.Visibility = Visibility.Visible;
                        break;
                    case ECCategoryLevel.Category2:
                        uc.lstECCategory1.Visibility = Visibility.Visible;
                        uc.lstECCategory2.Visibility = Visibility.Visible;
                        break;
                    default:
                        uc.lstECCategory1.Visibility = Visibility.Visible;
                        break;
                }
            }));

        /// <summary>
        ///业务模式，查询，维护 
        /// </summary>
        public BizMode BizMode
        {
            get;
            set;
        }

        /// <summary>
        /// 为输入列表加一个所有选项
        /// </summary>
        /// <param name="ecCategoryList">前台分类列表</param>
        private List<ECCategoryVM> AppendAllIfNeeded(List<ECCategoryVM> ecCategoryList)
        {
            var resultList = new List<ECCategoryVM>();
            resultList.AddRange(ecCategoryList);
            if (this.BizMode == UserControls.BizMode.Query)
            {
                resultList.Insert(0, new ECCategoryVM { SysNo = null, Name = ResCommonEnum.Enum_All });
            }
            else
            {
                resultList.Insert(0, new ECCategoryVM { SysNo = null, Name = ResCommonEnum.Enum_Select });
            }
            return resultList;
        }

        public void LoadAllECCategory(string channelID, Action callback)
        {
            //如果渠道编号为空则直接返回
            if (string.IsNullOrWhiteSpace(channelID)) return;
            SetSelectedCategory1SysNo(null);
            if (_allECCategoryList == null)
            {
                //加载所有的前台分类
                ECCategoryFacade facade = new ECCategoryFacade(CPApplication.Current.CurrentPage);
                facade.GetECCategory(CPApplication.Current.CompanyCode, channelID, (s, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    _allECCategoryList = new ECCategoryListVM();
                    _allECCategoryList.Category1List = args.Result.Category1List.Convert<ECCategory, ECCategoryVM>();
                    _allECCategoryList.Category2List = args.Result.Category2List.Convert<ECCategory, ECCategoryVM>();
                    _allECCategoryList.Category3List = args.Result.Category3List.Convert<ECCategory, ECCategoryVM>();
                    this.lstECCategory1.ItemsSource = AppendAllIfNeeded(_allECCategoryList.Category1List);
                    if (callback != null)
                    {
                        callback();
                    }
                    RaiseCategoryLoadCompleted();
                });
            }
            else
            {
                if (callback != null)
                {
                    callback();
                }
            }
        }

        private void lstECCategory1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var evt = C1SelectionChanged;
            if (evt != null)
            {
                var evtArgs = new C1SelectitonChangedEventArgs(this.SelectedCategory1SysNo, "");
                evt(sender, evtArgs);
            }
            LoadECCategory2();
        }

        private void LoadECCategory2()
        {
            //如果选择了一级分类就用一级分类过滤二级分类，否则显示所有二级分类
            int c1SysNo;
            object selectedC1SysNo = this.lstECCategory1.SelectedValue;
            List<ECCategoryVM> c2List;
            if (selectedC1SysNo == null || !int.TryParse(selectedC1SysNo.ToString(), out c1SysNo))
            {
                c2List = _allECCategoryList.Category2List;
            }
            else
            {
                c2List = _allECCategoryList.Category2List.Where(c2 => c2.ParentSysNo == c1SysNo).ToList();
            }
            this.lstECCategory2.ItemsSource = AppendAllIfNeeded(c2List);
        }

        private void lstECCategory2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var evt = C2SelectionChanged;
            if (evt != null)
            {
                var evtArgs = new C2SelectitonChangedEventArgs(this.SelectedCategory2SysNo, "");
                evt(sender, evtArgs);
            }
            SetSelectedCategory2SysNo(this.SelectedCategory2SysNo);
        }

        private void LoadCategory3()
        {
            object selectedC2SysNo = this.lstECCategory2.SelectedValue;
            object selectedC1SysNo = this.lstECCategory1.SelectedValue;
            int c1SysNo;
            int c2SysNo;
            List<ECCategoryVM> c3List;
            if (selectedC2SysNo != null && int.TryParse(selectedC2SysNo.ToString(), out c2SysNo))
            {
                c3List = _allECCategoryList.Category3List.Where(c3 => c3.ParentSysNo == c2SysNo).ToList();
            }
            else if (selectedC1SysNo != null && int.TryParse(selectedC1SysNo.ToString(), out c1SysNo))
            {
                c3List = (from c3 in _allECCategoryList.Category3List
                          join c2 in _allECCategoryList.Category2List
                              on c3.ParentSysNo equals c2.SysNo
                          join c1 in _allECCategoryList.Category1List
                              on c2.ParentSysNo equals c1.SysNo
                          where c1.SysNo == c1SysNo
                          select c3).ToList();
            }
            else
            {
                c3List = _allECCategoryList.Category3List;
            }
            this.lstECCategory3.ItemsSource = AppendAllIfNeeded(c3List);
        }

        private void lstECCategory3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var evt = C3SelectionChanged;
            if (evt != null)
            {
                var evtArgs = new C3SelectitonChangedEventArgs(this.SelectedCategory3SysNo, "");
                evt(sender, evtArgs);
            }
            SetSelectedCategory3SysNo(this.SelectedCategory3SysNo);
        }

        private void RemoveSelectionChangedEvent()
        {
            this.lstECCategory1.SelectionChanged -= new SelectionChangedEventHandler(lstECCategory1_SelectionChanged);
            this.lstECCategory2.SelectionChanged -= new SelectionChangedEventHandler(lstECCategory2_SelectionChanged);
            this.lstECCategory3.SelectionChanged -= new SelectionChangedEventHandler(lstECCategory3_SelectionChanged);
        }

        private void AddSelectionChangedEvent()
        {
            this.lstECCategory1.SelectionChanged += new SelectionChangedEventHandler(lstECCategory1_SelectionChanged);
            this.lstECCategory2.SelectionChanged += new SelectionChangedEventHandler(lstECCategory2_SelectionChanged);
            this.lstECCategory3.SelectionChanged += new SelectionChangedEventHandler(lstECCategory3_SelectionChanged);
        }

        /// <summary>
        /// 选中的一级分类编号
        /// </summary>
        public int? SelectedCategory1SysNo
        {
            get
            {
                if (this.lstECCategory1.SelectedValue != null)
                {
                    int c1SysNo;
                    int.TryParse(this.lstECCategory1.SelectedValue.ToString(), out c1SysNo);
                    return c1SysNo;
                }

                return null;
            }
        }

        public void SetSelectedCategory1SysNo(int? c1SysNo)
        {
            this.lstECCategory1.SelectedValue = c1SysNo;
        }
        /// <summary>
        /// 选中的二级分类编号
        /// </summary>
        public int? SelectedCategory2SysNo
        {
            get
            {
                if (this.lstECCategory2.SelectedValue != null)
                {
                    int c2SysNo;
                    int.TryParse(this.lstECCategory2.SelectedValue.ToString(), out c2SysNo);
                    return c2SysNo;
                }

                return null;
            }
        }

        public void SetSelectedCategory2SysNo(int? c2SysNo)
        {
            RemoveSelectionChangedEvent();
            //通过c2找到c1
            if (this._allECCategoryList==null
                ||this._allECCategoryList.Category2List == null
                ||this._allECCategoryList.Category2List.Count==0)
            {
                return;
            }
            var category2 = this._allECCategoryList.Category2List.FirstOrDefault(c2 => c2.SysNo == c2SysNo);
            int? c1SysNo = null;
            if (category2 != null)
            {
                c1SysNo = category2.ParentSysNo;
            }
            //1.改变c2后验证c2的c1是不是选中的。
            if (c1SysNo != null && this.SelectedCategory1SysNo != c1SysNo)
            {
                this.lstECCategory1.SelectedValue = c1SysNo;
                LoadECCategory2();
            }
            this.lstECCategory2.SelectedValue = c2SysNo;
            //2.改变c2后加载c3
            LoadCategory3();
            AddSelectionChangedEvent();
        }

        /// <summary>
        /// 选中的三级分类编号
        /// </summary>
        public int? SelectedCategory3SysNo
        {
            get
            {
                if (this.lstECCategory3.SelectedValue != null)
                {
                    int c3SysNo;
                    int.TryParse(this.lstECCategory3.SelectedValue.ToString(), out c3SysNo);
                    return c3SysNo;
                }

                return null;
            }
        }

        public void SetSelectedCategory3SysNo(int? c3SysNo)
        {
            RemoveSelectionChangedEvent();
            //通过c3找到c2,c1
            ECCategoryVM category3 = null;
            if (this._allECCategoryList!=null)
             category3 = this._allECCategoryList.Category3List.FirstOrDefault(c3 => c3.SysNo == c3SysNo);
            int? c2SysNo = null;
            int? c1SysNo = null;
            if (category3 != null)
            {
                var category2 = this._allECCategoryList.Category2List.FirstOrDefault(c2 => c2.SysNo == category3.ParentSysNo);
                c2SysNo = category3.ParentSysNo;
                if (category2 != null)
                {
                    c1SysNo = category2.ParentSysNo;
                }
            }
            //验证c3对应的c1是否选中
            if (c1SysNo != null && this.SelectedCategory1SysNo != c1SysNo)
            {
               this.lstECCategory1.SelectedValue = c1SysNo;
                LoadECCategory2();
               
            }
            //验证c3的c2是否选中
            if (c2SysNo != null && this.SelectedCategory2SysNo != c2SysNo)
            {
                this.lstECCategory2.SelectedValue = c2SysNo;
                LoadCategory3();
            }
            this.lstECCategory3.SelectedValue = c3SysNo;
            AddSelectionChangedEvent();
        }

        #region 自定义事件

        /// <summary>
        /// 分类列表加载完成事件
        /// </summary>
        public event EventHandler CategoryLoadCompleted;
        private void RaiseCategoryLoadCompleted()
        {
            var loadCompleted = CategoryLoadCompleted;
            if (loadCompleted != null)
            {
                loadCompleted(this, EventArgs.Empty);
            }
        }
        #endregion
    }

    public class C1SelectitonChangedEventArgs : EventArgs
    {
        private int? _categorySysNo;
        public string _categoryName;
        public C1SelectitonChangedEventArgs(int? categorySysNo, string categoryName)
        {
            _categorySysNo = categorySysNo;
            _categoryName = categoryName;
        }
    }

    public class C2SelectitonChangedEventArgs : EventArgs
    {
        private int? _categorySysNo;
        public string _categoryName;
        public C2SelectitonChangedEventArgs(int? categorySysNo, string categoryName)
        {
            _categorySysNo = categorySysNo;
            _categoryName = categoryName;
        }
    }

    public class C3SelectitonChangedEventArgs : EventArgs
    {
        private int? _categorySysNo;
        public string _categoryName;
        public C3SelectitonChangedEventArgs(int? categorySysNo, string categoryName)
        {
            _categorySysNo = categorySysNo;
            _categoryName = categoryName;
        }
    }
}
