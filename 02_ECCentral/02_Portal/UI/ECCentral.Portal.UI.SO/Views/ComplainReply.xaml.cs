using System;
using System.Threading;
using System.Linq;

using ECCentral.BizEntity.SO;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.SO.Facades;
using ECCentral.Portal.UI.SO.Models;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Data;
using System.Collections.Generic;
using System.Windows.Controls;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.Basic.Components.UserControls.PMPicker;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.SO.Resources;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System.Windows;
using ECCentral.Portal.Basic.Components.UserControls.ReasonCodePicker;
using ECCentral.BizEntity.Common;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.SO.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class ComplainReply : PageBase
    {
        SOComplainFacade m_facade;
        CommonDataFacade m_commFacade;

        SOComplaintInfo m_data;

        List<CodeNamePair> m_csDeptList;

        SOComplaintInfoVM DataVM
        {
            get {
                return formData.DataContext as SOComplaintInfoVM;
            }
        }

        public ComplainReply()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            m_facade = new SOComplainFacade(this);
            m_commFacade = new CommonDataFacade(this);
            BindData();

            btnSave.Visibility = AuthKeyControlMgr.GetVisibilityByRight(AuthKeyConst.SO_Complain_ComplainFull);
        }

        private void BindData()
        {
            int loadCompletedCount = 0;
            int wellLoadedCount = 3;

            this.cmbStatus.ItemsSource = EnumConverter.GetKeyValuePairs<SOComplainStatus>();
            this.cmbResponsibleConfirm.ItemsSource = BooleanConverter.GetKeyValuePairs(EnumConverter.EnumAppendItemType.Select);
            this.cmbApproach.ItemsSource = EnumConverter.GetKeyValuePairs<SOComplainReplyType>();

            //获取投诉详细部门集合
            CodeNamePairHelper.GetList(ConstValue.DomainName_SO
                , ConstValue.Key_SOResponsibleDept
                , CodeNamePairAppendItemType.Select
                , (o, p) =>
            {
                m_csDeptList = p.Result;
                Interlocked.Increment(ref loadCompletedCount);
                if (loadCompletedCount == wellLoadedCount)
                {
                    BindPage();
                }
            });

            //读取下拉框值
            CodeNamePairHelper.GetList(ConstValue.DomainName_SO
                , new string[] { ConstValue.Key_ComplainType, ConstValue.Key_SOComplainSourceType }
                , (o, p) =>
                {
                    this.cmbComplainType.ItemsSource = p.Result[ConstValue.Key_ComplainType];
                    this.cmbComplainSourceType.ItemsSource = p.Result[ConstValue.Key_SOComplainSourceType];
                    Interlocked.Increment(ref loadCompletedCount);
                    if (loadCompletedCount == wellLoadedCount)
                    {
                        BindPage();
                    }
                });

            //获取值
            m_facade.Get(int.Parse(Request.Param), (o, e) =>
            {
                if (!e.FaultsHandle())
                {
                    m_data = e.Result;
                    Interlocked.Increment(ref loadCompletedCount);
                    if (loadCompletedCount == wellLoadedCount)
                    {
                        BindPage();
                    }
                }
            });
        }

        private void BindPage()
        {
            //vmchange
            var dataVM = m_data.Convert<SOComplaintInfo, SOComplaintInfoVM>();
            formData.DataContext = dataVM;
            dataGridLogList.Bind();
            //是否需要发送邮件
            if (m_data.ProcessInfo.ReplyType == SOComplainReplyType.Email
                && !string.IsNullOrEmpty(m_data.ComplaintCotentInfo.CustomerEmail))
            {
                btnEmail.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                btnEmail.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void dataGridLogList_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            this.dataGridLogList.ItemsSource = m_data.ReplyHistory;
        }

        //保存
        private void btnSave_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //验证输入
            ValidationManager.Validate(formData);
            var vm = formData.DataContext as SOComplaintInfoVM;
            if (vm.ValidationErrors.Count != 0) return;
            var req = vm.ConvertVM<SOComplaintInfoVM, SOComplaintInfo>();
            m_facade.Update(req, (o, args) =>
            {
                if (!args.FaultsHandle())
                {
                    Window.Alert(ResSO.Msg_SaveSuccess);
                    m_data = args.Result;
                    BindPage();
                    ////是否需要发送邮件
                    //if (m_data.ProcessInfo.ReplyType == SOComplainReplyType.Email
                    //    && !string.IsNullOrEmpty(m_data.ComplaintCotentInfo.CustomerEmail))
                    //{
                    //    btnEmail.Visibility = System.Windows.Visibility.Visible;
                    //}
                }
            });
        }

        //关闭当前页
        private void btnClose_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Window.Close();
        }

        //商品所属Domain加载事件
        private void txtProductID_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            LoadDomainProduct();
        }

        //读取商品所属Domain
        private void LoadDomainProduct()
        {
            string productID = txtProductID.Text.Trim();
            if (productID.Length > 0)
            {
                (new OtherDomainQueryFacade()).QueryCategoryC1ByProductID(productID, (o, args) =>
                {
                    if (!args.FaultsHandle())
                    {
                        tbConfirmCategory1.Text = string.Format("{0}{1}({2})", ResComplain.TextBlock_ConfirmCategory1, args.Result.CategoryName,args.Result.SysNo);
                        DataVM.ProcessInfo.DomainSysNo = args.Result.SysNo;
                    }
                    else
                    {
                        tbConfirmCategory1.Text = "";
                        DataVM.ProcessInfo.DomainSysNo = null;
                    }
                });
            }
        }

        #region 页面帮助方法

        private string GetComplainTypeOrDetailByReasonCodePath(string reasonCodePath, int index)
        {
            string dept = string.Empty;
            string[] pathList = reasonCodePath.Split('>');
            if (pathList.Length >= index)
            {
                dept = pathList[index - 1];
            }
            return dept;
        }

        #endregion

        private void txtProductID_TextChanged(object sender, TextChangedEventArgs e)
        {
            //只有第一次加载的时候才需要读取
            txtProductID.TextChanged -= new TextChangedEventHandler(txtProductID_TextChanged);
            LoadDomainProduct();
        }

        private void btnEmail_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            m_facade.SendReplyEmail(m_data.SysNo.ToString(), (o, args) =>
            {
                if (!args.FaultsHandle())
                {
                    //发送成功
                    Window.Alert(ResSO.Msg_SendEmailSuccess);
                }
            });
        }

        private void SelectPath_Click(object sender, RoutedEventArgs e)
        {
            UCReasonCodePicker uc = new UCReasonCodePicker();
            uc.ReasonCodeType = ReasonCodeType.Complain;

            IDialog dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog(ResSOInternalMemo.hlb_SlelectReasonCode, uc, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    if (args.Data != null)
                    {
                        KeyValuePair<string, string> item = (KeyValuePair<string, string>)args.Data;
                        if (item.Key.Length > 0)
                        {
                            this.DataVM.ProcessInfo.ReasonCodeSysNo = int.Parse(item.Key);
                        }
                    }
                }
            });
            uc.Dialog = dialog;
        }

        private void txtReasonCodePath_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (txtReasonCodePath.Text.Length > 0)
            {
                string departmentName = GetComplainTypeOrDetailByReasonCodePath(txtReasonCodePath.Text, 4);
                if (departmentName.Length > 0)
                {
                    var department = m_csDeptList.Where(p => p.Name == departmentName).FirstOrDefault();
                    /*
                    * 不同部门不同加载，有影响的如下，界面变化点如下:
                    * 1.商品输入界面
                    * 2.部门人员加载
                    */
                    //有可能数据源已被清空
                    //加载部门人员
                    if (department != null)
                    {
                        string[] arrDept = department.Code.Split(',');
                        switch (arrDept[0])
                        {
                            case "Stock":
                                //获取所有仓库
                                m_commFacade.GetStockList(false, (s, args) =>
                                {
                                    var list = args.Result.Select(p => new CodeNamePair()
                                    {
                                        Code = p.SysNo.ToString()
                                        ,
                                        Name = p.StockName
                                    }
                                    );
                                    cmbResponsibleUser.ItemsSource = list;
                                });
                                break;
                            case "Dept":
                                //获取部门
                                if (arrDept[1] == "100")
                                {
                                    (new PMQueryFacade(this)).QueryPMList((new QueryFilter.IM.ProductManagerQueryFilter()
                                    {
                                        CompanyCode = CPApplication.Current.CompanyCode,
                                        PMQueryType = "AllValid"
                                    }), (s, args) =>
                                    {
                                        var list = args.Result.Select(p => new CodeNamePair()
                                        {
                                            Code = p.UserInfo.UserDisplayName
                                            ,
                                            Name = p.UserInfo.UserDisplayName
                                        }
                                        );
                                        cmbResponsibleUser.ItemsSource = list;
                                        //部门选中的不为空
                                        //如果选择的是PM的部门，那需要显示
                                        spProductID.Visibility = System.Windows.Visibility.Visible;
                                    });
                                    return;
                                }
                                else
                                {
                                    m_commFacade.GetUserInfoByDepartmentId(int.Parse(arrDept[1]), (s, args) =>
                                    {
                                        cmbResponsibleUser.ItemsSource = args.Result
                                            .Select(p => new CodeNamePair()
                                            {
                                                Code = p.UserDisplayName
                                                ,
                                                Name = p.UserDisplayName
                                            }
                                        ); ;
                                    });
                                }
                                break;
                            case "AllUser":
                                m_commFacade.GetAllCS(CPApplication.Current.CompanyCode, (s, args) =>
                                {
                                    var list = args.Result.Select(p => new CodeNamePair()
                                    {
                                        Code = p.UserName
                                        ,
                                        Name = p.UserName
                                    }
                                        );
                                    cmbResponsibleUser.ItemsSource = list;
                                });
                                break;
                            default:
                                //其他没有逻辑读取
                                break;
                        }
                        cmbResponsibleUser.ItemsSource = null;
                        spProductID.Visibility = System.Windows.Visibility.Collapsed;
                    }
                }
                else
                {
                    cmbResponsibleUser.ItemsSource = null;
                    spProductID.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        private void txtReasonCodeSysNo_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataVM.ProcessInfo.ReasonCodeSysNo.HasValue)
            {
                m_commFacade.GetReasonCodePath(this.DataVM.ProcessInfo.ReasonCodeSysNo.Value,CPApplication.Current.CompanyCode, (o, p) =>
                {
                    this.txtReasonCodePath.Text = p.Result;
                });
            }
        }
    }
}
