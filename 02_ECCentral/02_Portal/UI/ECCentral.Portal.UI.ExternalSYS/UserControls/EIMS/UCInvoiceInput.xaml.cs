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
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.ExternalSYS.Models;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.ExternalSYS.Resources;
using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.Portal.UI.ExternalSYS.Facades;
using System.Text.RegularExpressions;
using System.Windows.Data;
using ECCentral.BizEntity.ExternalSYS;

namespace ECCentral.Portal.UI.ExternalSYS.UserControls.EIMS
{
    public partial class UCInvoiceInput : UserControl
    {
        #region 页面初始化

        public IDialog Dialog { get; set; }

        public IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        public IPage CurrentPage
        {
            get
            {
                return CPApplication.Current.CurrentPage;
            }
        }

        public UCInvoiceInput()
        {
            InitializeComponent();
        }

        private EIMSInvoiceEntryVM invoicInputView;

        public EIMSInvoiceEntryVM InvoicInputView
        {
            get { return invoicInputView; }
            set { invoicInputView = value; }
        }

        private EIMSInvoiceInfoEntityVM invoiceInfoEntityView;

        public EIMSInvoiceInfoEntityVM InvoiceInfoEntityView
        {
            get { return invoiceInfoEntityView; }
            set { invoiceInfoEntityView = value; }
        }

        private List<EIMSInvoiceInfoEntityVM> invoiceInfoEntityViewList;

        public List<EIMSInvoiceInfoEntityVM> InvoiceInfoEntityViewList
        {
            get { return invoiceInfoEntityViewList; }
            set { invoiceInfoEntityViewList = value; }
        }

        private List<EIMSInvoiceInfoEntityVM> oldInvoiceInfoEntityViewList;

        public List<EIMSInvoiceInfoEntityVM> OldInvoiceInfoEntityViewList
        {
            get { return oldInvoiceInfoEntityViewList; }
            set { oldInvoiceInfoEntityViewList = value; }
        }

        private EIMSOrderMgmtFacade facade;

        public EIMSOrderMgmtFacade Facade
        {
            get { return facade; }
            set { facade = value; }
        }

        private List<EIMSInvoiceInputExtendVM> invoiceInputExtendViewList;

        public List<EIMSInvoiceInputExtendVM> InvoiceInputExtendViewList
        {
            get { return invoiceInputExtendViewList; }
            set { invoiceInputExtendViewList = value; }
        }

        private EIMSInvoiceInfoVM invoiceInfoView;

        public EIMSInvoiceInfoVM InvoiceInfoView
        {
            get { return invoiceInfoView; }
            set { invoiceInfoView = value; }
        }

        private string type;

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        public UCInvoiceInput(EIMSInvoiceEntryVM view, string type)
        {
            InitializeComponent();
            this.InvoicInputView = view;
            this.Type = type;
            Loaded += new RoutedEventHandler(UCInvoiceInput_Loaded);
        }

        List<string> invoiceInputNoList;
        List<string> invoiceInputDateList;
        List<string> invoiceInputAmtList;

        //多条发票录入的数量
        int totalCount = 0;

        /// <summary>
        /// Load 事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void UCInvoiceInput_Loaded(object sender, RoutedEventArgs e)
        {
            BindTaxRate();
            InitControls();
            InvoiceInfoEntityView = new EIMSInvoiceInfoEntityVM();
            InvoiceInfoEntityViewList = new List<EIMSInvoiceInfoEntityVM>();
            InvoiceInputExtendViewList = new List<EIMSInvoiceInputExtendVM>();
            this.Facade = new EIMSOrderMgmtFacade();
            this.gridInfo.DataContext = InvoicInputView;
            if (Type == "Input")
                BindInputTxtDataContext();
            else if (Type == "Edit")
            {
                QueryInvoiceInputList();
                //BindEditTxtDataContext();
            }
        }

        /// <summary>
        /// 初始化控件状态
        /// </summary>
        private void InitControls()
        {
            this.txtBillNo.IsEnabled = this.txtBalanceType.IsEnabled = this.txtVendorName.IsEnabled = this.txtBillAmt.IsEnabled = false;
        }
        /// <summary>
        /// 给录入信息绑定DataContext
        /// </summary>
        private void BindInputTxtDataContext()
        {
            this.txtInvoiceInputSysNo.DataContext = this.txtInvoiceInputDate.DataContext =
                this.txtInvoiceAmt.DataContext = this.cmbTax.DataContext = InvoiceInfoEntityView;
        }

        /// <summary>
        /// 绑定编辑状态的信息
        /// </summary>
        private void BindEditTxtDataContext()
        {
            Binding binding = new Binding();
            binding.Path = new PropertyPath("InvoiceInputSysNo");
            binding.Mode = BindingMode.TwoWay;
            InvoicInputView.InvoiceInputSysNo.Replace(",", ";");
            this.txtInvoiceInputSysNo.SetBinding(TextBox.TextProperty, binding);
        }


        /// <summary>
        /// 绑定税率信息
        /// </summary>
        private void BindTaxRate()
        {
            CodeNamePairHelper.GetList(ConstValue.DomainName_ExternalSYS, ConstValue.Key_TaxRate, (obj, args) =>
            {
                if (args.FaultsHandle()) return;
                if (args.Result != null)
                {
                    this.cmbTax.ItemsSource = args.Result;
                    this.cmbTax.SelectedIndex = 0;
                }
            });
        }

        #endregion

        #region 按钮事件相关
        /// <summary>
        /// 确定按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtInvoiceInputSysNo.Text)
                || string.IsNullOrEmpty(this.txtInvoiceInputDate.Text)
                || string.IsNullOrEmpty(this.txtInvoiceAmt.Text))
            {
                this.CurrentWindow.Alert(ResEIMSInvoiceEntry.Msg_InfoNotNull);
            }
            else
            {
                if (CehckEIMSInvoiceInputList())
                {
                    //组织InvoiceInputExtend数据
                    BuildInvoiceInputExtendList();

                    //检验EIMSInvoiceInput_Ex数据完整性
                    CheckinvoiceExtend(InvoiceInputExtendViewList);

                    //组织InvoiceInfoEntityView数据
                    BuildInvoiceInfoViewList();

                    if (Type == "Input")
                    {
                        //检验单据是否已录入
                        CheckEIMSInvoiceInputNumber(InvoiceInfoEntityViewList);
                    }
                    else if (Type == "Edit")
                    {
                        UpdateEIMSInvoiceInput();
                    }
                }
            }
        }

        /// <summary>
        /// 防止两个异步方法同时执行，将编辑发票代码提取出来
        /// </summary>
        private void UpdateEIMSInvoiceInput()
        {
            List<EIMSInvoiceInfoEntityVM> list = InvoiceInfoEntityViewList;
            List<EIMSInvoiceInfoEntityVM> old = InvoiceInfoView.OldEIMSList;
            List<EIMSInvoiceInfoEntityVM> newList = new List<EIMSInvoiceInfoEntityVM>();

            //将新增项添加到查询到的数据中
            #region 检测修改项
            for (int i = 0; i < old.Count; i++)
            {
                int count = 0;
                if (list.Count > i)
                {
                    if (list[i].SysNo == old[i].SysNo)
                        count++;
                    if (count == 0)
                    {
                        old[i].Status = -1;
                        if (old[i].EIMSInvoiceInputExtendList != null
                            && old[i].EIMSInvoiceInputExtendList.Count > 0)
                        {
                            for (int j = 0; j < old[i].EIMSInvoiceInputExtendList.Count; j++)
                            {
                                old[i].EIMSInvoiceInputExtendList[j].Status = -1;
                            }
                        }
                    }
                    else
                    {
                        old[i].TaxRate = list[i].TaxRate;
                        old[i].InvoiceInputAmount = list[i].InvoiceInputAmount;
                        old[i].InvoiceDate = list[i].InvoiceDate;

                        if (list[i].EIMSInvoiceInputExtendList == null
                            || list[i].EIMSInvoiceInputExtendList.Count == 0
                            )
                        {
                            old[i].Status = -1;
                        }

                        if (old[i].EIMSInvoiceInputExtendList != null)
                        {
                            for (int j = 0; j < old[i].EIMSInvoiceInputExtendList.Count; j++)
                            {
                                var exSysNo = old[i].EIMSInvoiceInputExtendList[j].SysNo;
                                if (list[i].EIMSInvoiceInputExtendList == null)
                                {
                                    int m = 0;
                                    for (int k = 0; k < list[i].EIMSInvoiceInputExtendList.Count; k++)
                                    {
                                        if (list[i].EIMSInvoiceInputExtendList[i].SysNo == exSysNo)
                                            m++;
                                    }
                                    if (m == 0)
                                        old[i].EIMSInvoiceInputExtendList[j].Status = -1;
                                }
                            }
                        }
                    }
                }
                else
                {
                    //如果修改后，发票数量有减少，则让减少的发票状态无效
                    if (old[i].EIMSInvoiceInputExtendList != null)
                    {
                        for (int j = 0; j < old[i].EIMSInvoiceInputExtendList.Count; j++)
                        {
                            old[i].EIMSInvoiceInputExtendList[j].Status = -1;
                        }
                    }
                }
            }
            #endregion

            #region 检测新增项
            list.ForEach(newEntity =>
            {
                if (!old.Select(item => item.SysNo).Contains(newEntity.SysNo))
                {
                    newList.Add(newEntity);
                }
            });
            #endregion

            #region 组织修改项和新增项的数据

            //修改项
            List<EIMSInvoiceInfoEntityVM> editData = new List<EIMSInvoiceInfoEntityVM>(old.Count);

            //新增项
            List<EIMSInvoiceInfoEntityVM> newData = new List<EIMSInvoiceInfoEntityVM>(newList.Count);

            for (int i = 0; i < old.Count; i++)
            {
                EIMSInvoiceInfoEntityVM invoice = new EIMSInvoiceInfoEntityVM();
                editData.Add(old[i]);
            }

            for (int i = 0; i < newList.Count; i++)
            {
                EIMSInvoiceInfoEntityVM invoice = new EIMSInvoiceInfoEntityVM();
                newData.Add(newList[i]);
            }

            if (newData.Count > 0)
            {
                for (int i = 0; i < newData.Count; i++)
                {
                    CheckinvoiceExtend(newData[i].EIMSInvoiceInputExtendList);
                }
            }
            #endregion

            if (newList.Count > 0)
            {
                CreateInvoiceInput(newData, editData);
            }
            else
            {
                UpdateInvoiceInfo(editData);
            }
        }

        private void UpdateInvoiceInfo(List<EIMSInvoiceInfoEntityVM> editData)
        {
            if (editData.Count > 0)
            {
                facade.UpdateEIMSInvoiceInput(editData, (obj, args) =>
                {
                    if (args.FaultsHandle()) return;
                    if (args.Result.IsSucceed)
                    {
                        this.CurrentWindow.Alert(ResEIMSInvoiceEntry.Msg_EditSuccess);
                        InvoiceInfoEntityViewList.Clear();
                        this.Dialog.ResultArgs.isForce = true;
                        this.Dialog.Close(true);
                    }
                    else
                    {
                        this.CurrentWindow.Alert(args.Result.Error);
                        InvoiceInfoEntityViewList.Clear();
                    }

                });
            }
        }

        /// <summary>
        /// 防止两个异步方法同时执行，将录入发票代码提取出来
        /// </summary>
        private void CreateInvoiceInput(List<EIMSInvoiceInfoEntityVM> invoiceInfoEntityList, List<EIMSInvoiceInfoEntityVM> editData)
        {
            facade.CreateEIMSInvoiceInput(invoiceInfoEntityList, (obj, args) =>
            {
                if (args.FaultsHandle()) return;
                EIMSInvoiceResultVM result = args.Result.Convert<EIMSInvoiceResult, EIMSInvoiceResultVM>();
                if (result.IsSucceed)
                {
                    if (Type == "Input")
                    {
                        this.CurrentWindow.Alert(ResEIMSInvoiceEntry.Msg_InputSuccess);
                        InvoiceInfoEntityViewList.Clear();
                        this.Dialog.ResultArgs.isForce = true;
                        this.Dialog.Close();
                    }
                    else if (Type == "Edit")
                    {
                        if (editData != null)
                        {
                            UpdateInvoiceInfo(editData);
                        }
                    }
                }
                else
                {
                    this.CurrentWindow.Alert(result.Error);
                    InvoiceInfoEntityViewList.Clear();
                    return;
                }
            });
        }

        /// <summary>
        /// 预览按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPreview_Click(object sender, RoutedEventArgs e)
        {
            if (CehckEIMSInvoiceInputList())
            {
                BuildInvoiceInfoViewList();
                UCViewInvoice uc = new UCViewInvoice(InvoiceInfoEntityViewList, InvoicInputView, "Preview");
                uc.Dialog = CurrentWindow.ShowDialog("发票信息预览", uc, (obj, args) =>
                    {
                        InvoiceInfoEntityViewList.Clear();
                    }, new Size(700, 350));
            }
        }

        /// <summary>
        /// 关闭按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Dialog.ResultArgs.isForce = false;
            this.Dialog.Close();
        }

        #endregion

        #region 验证录入信息相关

        /// <summary>
        /// 验证发票号
        /// </summary>
        /// <param name="invoiceInputNoList"></param>
        /// <returns></returns>
        private bool CheckInvoiceNo(List<string> invoiceInputNoList)
        {
            #region 验证发票号
            List<string> inputNoList = new List<string>();
            string inputNo = string.Empty;
            invoiceInputNoList.ForEach(No =>
                {
                    if (Regex.IsMatch(@"^\d+$", No) || No.Length != 8)
                    {
                        inputNoList.Add(No);

                    }
                });
            if (inputNoList.Count > 0)
            {
                for (int i = 0; i < inputNoList.Count; i++)
                {
                    if (i > 0)
                    {
                        inputNo += ";";
                    }
                    inputNo += inputNoList[i];
                }
                this.CurrentWindow.Alert(string.Format(ResEIMSInvoiceEntry.Msg_InvoiceInputCheck, inputNo));
                return false;
            }
            else
            {
                for (int i = 0; i < invoiceInputNoList.Count; i++)
                {
                    inputNo = invoiceInputNoList[i];
                    for (int j = i + 1; j < invoiceInputNoList.Count; j++)
                    {
                        if (invoiceInputNoList[j] == inputNo)
                        {
                            this.CurrentWindow.Alert(string.Format(ResEIMSInvoiceEntry.Msg_RepeatIputNo, inputNo));
                            return false;
                        }
                    }
                }
                return true;
            }

            #endregion
        }

        /// <summary>
        /// 验证发票日期
        /// </summary>
        /// <param name="invoiceInputDateList"></param>
        /// <returns></returns>
        private bool CheckInvoiceDate(List<string> invoiceInputDateList)
        {
            #region 验证日期
            //日期格式"20111214"的验证
            List<string> inputDateList = new List<string>();
            string inputDate = string.Empty;
            if (string.IsNullOrEmpty(txtInvoiceInputDate.Text))
            {
                this.CurrentWindow.Alert(ResEIMSInvoiceEntry.Msg_DateNull);
                return false;
            }

            invoiceInputDateList.ForEach(data =>
                {
                    if (data.Length != 8)
                        inputDateList.Add(data);
                });
            if (inputDateList.Count > 0)
            {
                for (int i = 0; i < inputDateList.Count; i++)
                {
                    if (i > 0)
                    {
                        inputDate += ";";
                    }
                    inputDate += inputDateList[i];
                }
                this.CurrentWindow.Alert(string.Format(ResEIMSInvoiceEntry.Msg_DateError, inputDate));
                return false;
            }
            else
            {
                int year = 0;
                int month = 0;
                int day = 0;
                invoiceInputDateList.ForEach(data =>
                {
                    year = int.Parse(data.Substring(0, 4));
                    month = int.Parse(data.Substring(4, 2));
                    day = int.Parse(data.Substring(6, 2));
                    if (year < 1990)
                    {
                        inputDateList.Add(data);
                        return;
                    }
                    if (month < 1 || month > 12)
                    {
                        inputDateList.Add(data);
                        return;
                    }
                    switch (month)
                    {
                        case 1:
                        case 3:
                        case 5:
                        case 7:
                        case 8:
                        case 10:
                        case 12:
                            if (day > 31 || day < 1)
                            {
                                inputDateList.Add(data);
                                return;
                            }
                            break;
                        case 2:
                            var IsLeapYear = false;
                            if (year % 100 == 0 && year % 400 == 0)
                                IsLeapYear = true;
                            else if (year % 4 == 0 && year % 100 != 0)
                                IsLeapYear = true;
                            if (day > (IsLeapYear ? 29 : 28) || day < 1)
                            {
                                inputDateList.Add(data);
                                return;
                            }
                            break;
                        default:
                            if (day > 30 || day < 1)
                            {
                                inputDateList.Add(data);
                                return;
                            }
                            break;
                    }
                    try
                    {
                        var newDate = new DateTime(year, month, day);
                    }
                    catch
                    {
                        inputDateList.Add(data);
                        return;
                    }
                });
                if (inputDateList.Count > 0)
                {
                    for (int i = 0; i < inputDateList.Count; i++)
                    {
                        if (i > 0)
                        {
                            inputDate += ";";
                        }
                        inputDate += inputDateList[i];
                    }
                    this.CurrentWindow.Alert(string.Format(ResEIMSInvoiceEntry.Msg_DateError, inputDate));
                    return false;
                }
                else
                    return true;
            }
            #endregion
        }

        /// <summary>
        /// 验证单据金额
        /// </summary>
        /// <param name="invoiceInputAmtList"></param>
        /// <returns></returns>
        private bool CheckInvoiceAmt(List<string> invoiceInputAmtList)
        {
            #region 验证单据金额
            List<string> inputAmtList = new List<string>();
            decimal invoiceAmount = 0;
            string inputNo = string.Empty;
            invoiceInputAmtList.ForEach(amt =>
                {
                    if (!string.IsNullOrEmpty(amt))
                    {
                        if (Regex.IsMatch(amt, @"(^\d+$)|(^\d+.\d+$)"))
                        {
                            if (Convert.ToDecimal(amt) < 0.01m)
                            {
                                inputAmtList.Add(amt);
                            }
                        }                        
                    }
                });
            if (inputAmtList.Count > 0)
            {
                for (int i = 0; i < inputAmtList.Count; i++)
                {
                    if (i > 1)
                    {
                        inputNo += ";";
                    }
                    inputNo += invoiceInputNoList[i];
                }
                this.CurrentWindow.Alert(string.Format(ResEIMSInvoiceEntry.Msg_InvoiceError, inputNo));
                return false;
            }
            else
            {
                invoiceInputAmtList.ForEach(amt =>
                    {
                        inputAmtList.Add(amt);
                    });
                if (inputAmtList.Count > 0)
                {
                    for (int i = 0; i < inputAmtList.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(inputAmtList[i]))
                        {
                            if (Regex.IsMatch(inputAmtList[i], @"(^\d+$)|(^\d+.\d+$)"))
                            {
                                invoiceAmount += Convert.ToDecimal(inputAmtList[i]);
                            }
                            else
                                break;
                        }
                        else
                            invoiceAmount += 0m;
                    }
                    if (invoiceAmount != InvoicInputView.InvoiceAmount)
                    {
                        this.CurrentWindow.Alert(ResEIMSInvoiceEntry.Msg_InvoiceInputAmountNoEquels);
                        return false;
                    }
                    else
                        return true;
                }
                else
                {
                    this.CurrentWindow.Alert(ResEIMSInvoiceEntry.Msg_InvoiceAmtNull);
                    return false;
                }
            }
            #endregion
        }

        /// <summary>
        /// 检验输入的发票数据是否合理
        /// </summary>
        /// <returns></returns>
        private bool CehckEIMSInvoiceInputList()
        {
            invoiceInputNoList = this.txtInvoiceInputSysNo.Text.Trim().Split(';').ToList<string>();
            invoiceInputDateList = this.txtInvoiceInputDate.Text.Trim().Split(';').ToList<string>();
            invoiceInputAmtList = this.txtInvoiceAmt.Text.Trim().Split(';').ToList<string>();

            totalCount = Math.Max(Math.Max(invoiceInputNoList.Count, invoiceInputDateList.Count), invoiceInputAmtList.Count);
            if (invoiceInputNoList.Count == invoiceInputDateList.Count
                && invoiceInputNoList.Count == invoiceInputAmtList.Count
                && invoiceInputDateList.Count == invoiceInputAmtList.Count)
            {
                if (CheckInvoiceNo(invoiceInputNoList)
                && CheckInvoiceDate(invoiceInputDateList)
                && CheckInvoiceAmt(invoiceInputAmtList))
                    return true;
                else
                    return false;
            }
            else
            {
                this.CurrentWindow.Alert(ResEIMSInvoiceEntry.Msg_InvoceInfoError);
                return false;
            }

        }

        /// <summary>
        /// 检验单据是否已录入
        /// </summary>
        /// <param name="invoiceList"></param>
        private void CheckEIMSInvoiceInputNumber(List<EIMSInvoiceInfoEntityVM> invoiceList)
        {
            EIMSInvoiceEntryQueryFilter filter = new EIMSInvoiceEntryQueryFilter()
            {
                PagingInfo = new QueryFilter.Common.PagingInfo()
                {
                    PageIndex = 0,
                    PageSize = int.MaxValue
                }
            };
            int index = 0;
            string invoiceNumber = string.Empty;
            List<string> invoiceNumberList = new List<string>();
            invoiceList.ForEach(entity =>
            {
                entity.EIMSInvoiceInputExtendList.ForEach(ex =>
                {
                    if (!invoiceNumberList.Contains(ex.InvoiceNumber))
                    {
                        invoiceNumberList.Add(ex.InvoiceNumber);
                    }
                });
            });
            invoiceNumberList.ForEach(delegate(string number)
            {
                if (index > 0)
                {
                    invoiceNumber += ".";
                }
                invoiceNumber += number;
                index++;
            });
            filter.InvoiceNumber = invoiceNumber;

            invoiceNumberList.Clear();
            invoiceNumber = string.Empty;

            Facade.QueryInvoiceInfoList(filter, (obj, args) =>
            {
                if (args.FaultsHandle()) return;
                List<EIMSInvoiceEntryVM> result = DynamicConverter<EIMSInvoiceEntryVM>.ConvertToVMList(args.Result.Rows);
                result.ForEach(entity =>
                {
                    if (entity.InvoiceInputStatus == "已录入")
                    {
                        invoiceNumberList.Add(entity.InvoiceNumber);
                    }
                });
                if (invoiceNumberList.Count > 0)
                {
                    for (int i = 0; i < invoiceNumberList.Count; i++)
                    {
                        if (i > 0)
                        {
                            invoiceNumber += ";";
                        }
                        invoiceNumber += invoiceNumberList[i];
                    }

                    this.CurrentWindow.Alert((string.Format(ResEIMSInvoiceEntry.Msg_HadInputed, invoiceNumber)));
                }
                else
                {
                    CreateInvoiceInput(InvoiceInfoEntityViewList, null);
                }
            });
        }

        //检验EIMSInvoiceInput_Ex数据完整性
        private void CheckinvoiceExtend(List<EIMSInvoiceInputExtendVM> list)
        {
            list.ForEach(entity =>
            {
                if (entity == null)
                {
                    this.CurrentWindow.Alert("不能添加EIMSInvoiceInput_Ex空数据");
                }
                if (string.IsNullOrEmpty(entity.InvoiceNumber))
                {
                    this.CurrentWindow.Alert("单据号不能为空");
                }
                entity.Status = 0;
            });
        }

        #endregion

        #region 组织发票录入数据
        /// <summary>
        /// 组织InvoiceInfoEntityViewList数据
        /// </summary>
        private void BuildInvoiceInfoViewList()
        {
            for (int i = 0; i < totalCount; i++)
            {
                if ( OldInvoiceInfoEntityViewList != null 
                    && OldInvoiceInfoEntityViewList.Count > i
                    && OldInvoiceInfoEntityViewList[i].InvoiceInputNo == invoiceInputNoList[i])
                {
                    InvoiceInfoEntityViewList.Add(new EIMSInvoiceInfoEntityVM()
                    {
                        SysNo = (OldInvoiceInfoEntityViewList != null && OldInvoiceInfoEntityViewList.Count > i) ? OldInvoiceInfoEntityViewList[i].SysNo : 0,
                        InvoiceInputNo = invoiceInputNoList[i],
                        InvoiceDate = ChangeDateMode(invoiceInputDateList[i]),
                        InvoiceInputAmount = Convert.ToDecimal(invoiceInputAmtList[i]),
                        TaxRate = Convert.ToDecimal(cmbTax.SelectedValue),
                        EIMSInvoiceInputExtendList = (OldInvoiceInfoEntityViewList != null && OldInvoiceInfoEntityViewList.Count > i) ? OldInvoiceInfoEntityViewList[i].EIMSInvoiceInputExtendList : InvoiceInputExtendViewList,
                        VendorNumber = InvoicInputView.VendorNumber
                    });
                }
                else
                {
                    InvoiceInfoEntityViewList.Add(new EIMSInvoiceInfoEntityVM()
                    {
                        SysNo = 0,
                        InvoiceInputNo = invoiceInputNoList[i],
                        InvoiceDate = ChangeDateMode(invoiceInputDateList[i]),
                        InvoiceInputAmount = Convert.ToDecimal(invoiceInputAmtList[i]),
                        TaxRate = Convert.ToDecimal(cmbTax.SelectedValue),
                        EIMSInvoiceInputExtendList = InvoiceInputExtendViewList,
                        VendorNumber = InvoicInputView.VendorNumber
                    });
                }
            }
        }

        /// <summary>
        /// 组织InvoiceInputExtendList数据
        /// </summary>
        private void BuildInvoiceInputExtendList()
        {
            InvoiceInputExtendViewList.Add(new EIMSInvoiceInputExtendVM()
            {
                InvoiceNumber = InvoicInputView.InvoiceNumber
            });
        }

        /// <summary>
        /// 将格式如"20111214"转换为"2011-12-14"
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private DateTime ChangeDateMode(string date)
        {
            var year = date.Substring(0, 4);
            var month = date.Substring(4, 2);
            var day = date.Substring(6, 2);
            return Convert.ToDateTime(year + "-" + month + "-" + day);
        }

        /// <summary>
        /// 将时间如"2012/8/18"转换为"20120818"
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private string ChangeDateMode(DateTime? date)
        {
            string year = date.Value.Year.ToString();
            string month = date.Value.Month.ToString().PadLeft(2, '0');
            string day = date.Value.Day.ToString().PadLeft(2, '0');
            return year + month + day;
        }
        #endregion

        #region 编辑相关操作
        private void QueryInvoiceInputList()
        {
            facade.QueryInvoiceList(InvoicInputView.InvoiceNumber, (obj, args) =>
            {
                if (args.FaultsHandle()) return;
                InvoiceInfoView = args.Result.Convert<EIMSInvoiceInfo, EIMSInvoiceInfoVM>();
                OldInvoiceInfoEntityViewList = InvoiceInfoView.EIMSList;
                if (OldInvoiceInfoEntityViewList.Count > 0)
                {
                    for (int i = 0; i < OldInvoiceInfoEntityViewList.Count; i++)
                    {
                        if (i > 0)
                        {
                            this.txtInvoiceInputSysNo.Text += ";";
                            this.txtInvoiceInputDate.Text += ";";
                            this.txtInvoiceAmt.Text += ";";
                        }
                        this.txtInvoiceInputSysNo.Text += OldInvoiceInfoEntityViewList[i].InvoiceInputNo;
                        this.txtInvoiceInputDate.Text += ChangeDateMode(OldInvoiceInfoEntityViewList[i].InvoiceDate);
                        this.txtInvoiceAmt.Text += OldInvoiceInfoEntityViewList[i].InvoiceInputAmount.ToString();
                        this.cmbTax.SelectedValue = OldInvoiceInfoEntityViewList[i].TaxRate;
                    }
                }

            });
        }
        #endregion
    }
}
