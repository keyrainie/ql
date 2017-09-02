using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.Basic.Components.UserControls.PMPicker
{
    public partial class UCPMPicker : UserControl
    {
        /// <summary>
        /// 用于加载完数据，设置默认值
        /// </summary>
        /// <param name="sender">control</param>
        public delegate void SetDefaultValue(object sender);
        public SetDefaultValue SetDefaultValueHandler;
        public event EventHandler<EventArgs> PMLoaded;
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;
        private int _pmUserCondition=0;
        public int PMUserCondition
        {
            get { return _pmUserCondition; }
        }

        /// <summary>
        ///  外抛的属性(页面名称): 用于判断 每个页面所要加载PM的查询类型
        /// </summary>
        private string _queryQueryPage;
        public string QueryPage
        {
            get { return _queryQueryPage; }
            set { _queryQueryPage = value; }
        }

        private static readonly DependencyProperty SelectedPMSysNoProperty =
             DependencyProperty.Register("SelectedPMSysNo", typeof(int?), typeof(UCPMPicker), new PropertyMetadata(null, (s, e) =>
             {
                 var uc = s as UCPMPicker;
                 uc.cmbPMList.SelectedValue = (e.NewValue ?? "").ToString().Trim();
             }));
        private static readonly DependencyProperty SelectedPMNameProperty =
          DependencyProperty.Register("SelectedPMName", typeof(string), typeof(UCPMPicker), null);

        public static readonly DependencyProperty IsAutoChooseCurrentLoginUserProperty =
    DependencyProperty.Register("IsAutoChooseCurrentLoginUser", typeof(bool), typeof(UCPMPicker), new PropertyMetadata(true));

        private static readonly DependencyProperty PMQueryTypeProperty =
           DependencyProperty.Register("PMQueryType", typeof(PMQueryType?), typeof(UCPMPicker), new PropertyMetadata(null));


        /// <summary>
        /// 当前选中的PM系统编号
        /// </summary>
        public int? SelectedPMSysNo
        {
            get
            {
                return this.cmbPMList.SelectedValue == null ? default(int?) : int.Parse(cmbPMList.SelectedValue.ToString());
            }
            set
            {
                base.SetValue(SelectedPMSysNoProperty, value);
            }
        }

        /// <summary>
        /// 当前选中的PM名称
        /// </summary>
        public string SelectedPMName
        {
            get
            {
                ProductManagerInfo pm = this.cmbPMList.SelectedItem as ProductManagerInfo;
                return (pm.UserInfo.UserDisplayName == ResCommonEnum.Enum_All ? string.Empty : pm.UserInfo.UserDisplayName);
            }
            set
            {
                base.SetValue(SelectedPMNameProperty, value);
            }
        }

        /// <summary>
        /// 备份PM列表
        /// </summary>
        public string BackupUserList
        {
            get
            {
                ProductManagerInfo pm = this.cmbPMList.SelectedItem as ProductManagerInfo;
                return pm.BackupUserList;
            }
        }

        /// <summary>
        /// 是否默认加载完成后控件自动选中当前登录用户(默认为选中)
        /// </summary>
        public bool IsAutoChooseCurrentLoginUser
        {
            get { return (bool)GetValue(IsAutoChooseCurrentLoginUserProperty); }
            set { SetValue(IsAutoChooseCurrentLoginUserProperty, value); }
        }


        public PMQueryType? PMQueryType
        {
            get
            {
                return ((PMQueryType?)base.GetValue(PMQueryTypeProperty));
            }
            set
            {
                base.SetValue(PMQueryTypeProperty, value);
            }
        }

        /// <summary>
        /// All:所有，Select:请选择
        /// </summary>
        public string SelectMode { get; set; }

        public PMQueryFacade serviceFacade;
        public ProductManagerQueryFilter queryFilter;
        public List<ProductManagerInfo> itemList;

        public UCPMPicker()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(UCPMPicker_Loaded);
            SelectMode = "All";
        }

        private void UCPMPicker_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCPMPicker_Loaded);
            var exp = this.GetBindingExpression(UCPMPicker.SelectedPMSysNoProperty);
            if (exp != null && exp.ParentBinding != null)
            {
                string path = exp.ParentBinding.Path.Path;
                Binding binding = new Binding();
                binding.Path = new PropertyPath(path);
                binding.Mode = BindingMode.TwoWay;
                binding.NotifyOnValidationError = true;
                cmbPMList.SetBinding(Newegg.Oversea.Silverlight.Controls.Combox.SelectedValueProperty, binding);
            }

            if (null == queryFilter)
            {
                queryFilter = new ProductManagerQueryFilter()
                {
                    UserName = CPApplication.Current.LoginUser.LoginName,
                    CompanyCode = CPApplication.Current.CompanyCode
                };
                serviceFacade = new PMQueryFacade(CPApplication.Current.CurrentPage);

                //如果指定了查询的QueryType
                if (PMQueryType.HasValue)
                {
                    queryFilter.PMQueryType = PMQueryType.Value.ToString();
                    _pmUserCondition = 1;
                }
                else
                {
                    #region [Ray.L.Xing 新需求 取消了PM 查询PM的权限 改为通过商品的产品线来过滤单据]
                    _pmUserCondition = 3;
                    queryFilter.PMQueryType = ECCentral.BizEntity.Common.PMQueryType.All.ToString();
                    #endregion

                    //Ray.L.Xing 注销旧需求（因为新需求 取消了PM 查询PM的权限 改为通过商品的产品线来过滤单据）
                    #region [  如果没有指定查询的QueryType,根据当前用户查询PM的权限来获取PM List]
                    //if (AuthMgr.HasFunctionAbsolute(AuthKeyConst.IM_SeniorPM_Query))
                    //{
                    //    _pmUserCondition = 3;
                    //    if (!string.IsNullOrEmpty(QueryPage))//据有高级权限的PM ，访问不同页面看到的PM也有可能是不相同的，所以需要根据页面来单独初始查询类型
                    //    {                           
                    //        switch (QueryPage)
                    //        {
                    //            case "TransferStockingCenter": // 备货中心      高级权限 查询 所有有效状态的PM      
                    //            case "PMMonitoringPerformanceIndicators":// PM工作指标监控    高级权限 查询 所有有效状态的PM
                    //                 queryFilter.PMQueryType = ECCentral.BizEntity.Common.PMQueryType.AllValid.ToString();
                    //                 break;
                    //            default:// 默认高级权限 查询 所有的PM 包括无效状态
                    //                 queryFilter.PMQueryType = ECCentral.BizEntity.Common.PMQueryType.All.ToString();
                    //                 break;
                    //        }                             
                    //    }
                    //    else
                    //    {
                    //        queryFilter.PMQueryType = ECCentral.BizEntity.Common.PMQueryType.All.ToString();
                    //    }                        
                    //}
                    //else if (AuthMgr.HasFunctionAbsolute(AuthKeyConst.IM_IntermediatePM_Query))
                    //{
                    //    _pmUserCondition = 2;
                    //    queryFilter.PMQueryType = ECCentral.BizEntity.Common.PMQueryType.Team.ToString();                       
                    //}
                    //else if (AuthMgr.HasFunctionAbsolute(AuthKeyConst.IM_JuniorPM_Query))
                    //{
                    //    _pmUserCondition = 1;
                    //    queryFilter.PMQueryType = ECCentral.BizEntity.Common.PMQueryType.Self.ToString();
                    //}
                    //else
                    //{
                    //    queryFilter.PMQueryType = ECCentral.BizEntity.Common.PMQueryType.None.ToString();
                    //}
                    #endregion
                }

                BindComboBoxData();
            }
        }

        public void AddAppendItem(int index, int val, string txt)
        {
            if (null != itemList)
            {
                itemList.Insert(index, new ProductManagerInfo()
                {
                    SysNo = val,
                    UserInfo = new UserInfo
                        {
                            SysNo = val,
                            UserDisplayName = txt
                        }
                });
            }
        }

        private void BindComboBoxData()
        {
            serviceFacade.QueryPMList(queryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                itemList = new List<ProductManagerInfo>();
                itemList = args.Result;
                var blankList = itemList.Where(i => i.UserInfo.SysNo == null || !i.UserInfo.SysNo.HasValue).ToList();
                ProductManagerInfo blackInfo = new ProductManagerInfo();

                string username = string.Empty;
                if(SelectMode=="Select")
                {
                    username = ResCommonEnum.Enum_Select;
                }
                else
                {
                    username = ResCommonEnum.Enum_All;
                }

                if (queryFilter.PMQueryType != ECCentral.BizEntity.Common.PMQueryType.None.ToString())
                {
                    blackInfo.UserInfo = new UserInfo()
                    {
                        SysNo = null,
                        UserDisplayName = username
                    };
                }
                else
                {
                    blackInfo.UserInfo = new UserInfo()
                    {
                        SysNo = 0,
                        UserDisplayName = username
                    };
                }
                itemList.Insert(0, blackInfo);
                this.cmbPMList.ItemsSource = itemList;
                if (!SelectedPMSysNo.HasValue && IsAutoChooseCurrentLoginUser)
                {
                    //该登陆用户需时PM用户
                    if (itemList.Count(p => p.SysNo == CPApplication.Current.LoginUser.UserSysNo) > 0)
                    {
                        SelectedPMSysNo = CPApplication.Current.LoginUser.UserSysNo;
                        SelectedPMName = CPApplication.Current.LoginUser.DisplayName;
                    }
                }
                OnPMLoaded();
                //用于加载完数据，设置默认值
                if (SetDefaultValueHandler != null)
                    SetDefaultValueHandler(cmbPMList);
            });
        }

        private void OnPMLoaded()
        {
            var handler = PMLoaded;
            if (handler != null)
            {
                EventArgs e = new EventArgs();
                handler(this, e);
            }
        }

        private void cmbPMList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.SelectionChanged != null)
            {
                SelectionChanged(sender, e);
            }
            var contrl = (Combox)sender;
            if (sender != null)
            {
                if (contrl.SelectedValue != null)
                {
                    var pm = contrl.SelectedItem as ProductManagerInfo;
                    if (pm != null) SelectedPMName = pm.UserInfo.UserDisplayName;
                }
                else
                {
                    SelectedPMName = "";
                }
            }
            else
            {
                SelectedPMName = "";
            }
        }
    }
}
