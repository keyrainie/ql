using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using ECCentral.BizEntity.Customer;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.Portal.UI.Customer.Resources;
using ECCentral.QueryFilter.Customer;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.Customer.UserControls
{
    public partial class CustomerPointsAddRequestDetailInfo : UserControl
    {
        public DynamicXml viewModel;
        private CustomerPointsAddQueryFacade serviceFacade;
        private CustomerPointsAddRequestFilter queryRequest;
        private CustomerPointsAddRequest actionRequest;

        public IDialog Dialog { get; set; }

        public CustomerPointsAddRequestDetailInfo(DynamicXml model, string type)
        {
            viewModel = new DynamicXml();
            serviceFacade = new CustomerPointsAddQueryFacade();
            actionRequest = new CustomerPointsAddRequest();
            queryRequest = new CustomerPointsAddRequestFilter()
            {
                PageInfo = new QueryFilter.Common.PagingInfo(),
                CompanyCode = CPApplication.Current.CompanyCode
            };

            if (null != model)
            {
                viewModel = model;
                if (!string.IsNullOrEmpty((string)viewModel["productID"]))
                {
                    (new OtherDomainQueryFacade()).QueryCategoryC1ByProductID((string)viewModel["productID"], (o, args) =>
                    {
                        if (!args.FaultsHandle())
                        {
                            viewModel["Category1Name"] = args.Result.CategoryName.Content;
                        }
                    });
                }
                this.DataContext = viewModel;
            }
            InitializeComponent();
            this.ProductsResultGrid.Bind();

            if (type == "Audit")
            {
                SwitchAuditButtons(true);
            }
            else
            {
                SwitchAuditButtons(false);
            }
        }

        private void DataGrid_ProductsResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            //绑定RequestItem:
            queryRequest.SystemNumber = Convert.ToInt32(this.viewModel["sysno"].ToString());

            serviceFacade.QueryCustomerPointsAddItem(queryRequest, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                var customerPointsAddRequestItemList = args.Result.Rows;
                int totalCount = args.Result.TotalCount;
                ProductsResultGrid.TotalCount = totalCount;
                ProductsResultGrid.ItemsSource = customerPointsAddRequestItemList;
            });

        }
        private bool ValidateInput(string status)
        {
            List<ValidationEntity> list = new List<ValidationEntity>();
            if (status == "AuditDenied")
            {
                list.Add(new ValidationEntity(ValidationEnum.IsNotEmpty, null, "备注信息不能为空"));
            }

            list.Add(new ValidationEntity(ValidationEnum.MaxLengthLimit, 200, ResCustomerPointsAddRequest.msg_confirmnotLength));
            return ValidationHelper.Validation(this.txtConfirmNote, list);
        }
        private void Button_AuditPointsAddRequest_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput("AuditPassed") || !ValidateRight())
                return;
            CPApplication.Current.CurrentPage.Context.Window.Confirm(ResCustomerPointsAddRequest.Confirm_Waring_Text, ResCustomerPointsAddRequest.Confirm_Areyousure_Text, (o, a) =>
            {
                if (a.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    serviceFacade.ConfirmCustomerPointsAddRequest(BuildAuditRequestMessage(CustomerPointsAddRequestStatus.AuditPassed), (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        CPApplication.Current.CurrentPage.Context.Window.Alert(ResCustomerPointsAddRequest.Confirm_Waring_Text, ResCustomerPointsAddRequest.Confirm_AuditSucc_Text, MessageType.Information, (ojb2, args2) =>
                        {
                            viewModel["validscore"] = int.Parse(viewModel["validscore"].ToString()) + int.Parse(viewModel["point"].ToString());
                            this.Dialog.ResultArgs.Data = viewModel;
                            SwitchAuditButtons(false);
                        });

                    });
                }
            });
        }

        private void Button_DeniedPointsAddRequest_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput("AuditDenied"))
                return;
            CPApplication.Current.CurrentPage.Context.Window.Confirm(ResCustomerPointsAddRequest.Confirm_Waring_Text, ResCustomerPointsAddRequest.Confirm_Areyousure_Text, (o, a) =>
            {
                if (a.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    serviceFacade.ConfirmCustomerPointsAddRequest(BuildAuditRequestMessage(CustomerPointsAddRequestStatus.AuditDenied), (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }

                        CPApplication.Current.CurrentPage.Context.Window.Alert(ResCustomerPointsAddRequest.Confirm_Waring_Text, ResCustomerPointsAddRequest.Confirm_AuditSucc_Text, MessageType.Information, (ojb2, args2) =>
                        {
                            this.Dialog.ResultArgs.Data = viewModel;
                            SwitchAuditButtons(false);
                        });

                    });
                }
            });
        }
        private bool ValidateRight()
        {
            int point = 0;
            if (int.TryParse(tbPoint.Text, out point))
            {
                if (Math.Abs(point) > 300 && !AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_PointAddRequest_Audit_NoLimit))
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResCustomerPointsAddRequest.rightMsg_NoRight_Audit_NoLimit);
                    return false;
                }
                else if (Math.Abs(point) <= 300 && Math.Abs(point) > 80
                    && !AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_PointAddRequest_Audit_NoLimit)
                    && !AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_PointAddRequest_Audit_Lessthan300P))
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResCustomerPointsAddRequest.rightMsg_NoRight_Audit_Lessthan300P);
                    return false;
                }
                else if (Math.Abs(point) <= 80
                        && !AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_PointAddRequest_Audit_NoLimit)
                        && !AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_PointAddRequest_Audit_Lessthan300P)
                        && !AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_PointAddRequest_Audit_Lessthan80P)
                        )
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResCustomerPointsAddRequest.rightMsg_NoRight_Audit_Lessthan80P);
                    return false;
                }
            }
            return true;
        }

        private CustomerPointsAddRequest BuildAuditRequestMessage(CustomerPointsAddRequestStatus targetStatus)
        {
            this.actionRequest = new BizEntity.Customer.CustomerPointsAddRequest()
            {
                SysNo = Convert.ToInt32(viewModel["sysno"]),
                Status = targetStatus,
                ConfirmUserSysNo = CPApplication.Current.LoginUser.UserSysNo.Value,
                CustomerSysNo = Convert.ToInt32(viewModel["customersysno"]),
                ConfirmNote = viewModel["confirmnote"].ToString(),
                PointType = Convert.ToInt32(viewModel["pointlogtype"]),
                SOSysNo = Convert.ToInt32(viewModel["sosysno"]),
                Point = Convert.ToInt32(viewModel["point"]),
                CreateUserSysNo = Convert.ToInt32(viewModel["createusersysno"]),
                NewEggAccount = viewModel["neweggaccount"].ToString()
            };
            return this.actionRequest;
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Dialog.ResultArgs.DialogResult = DialogResultType.OK;
            this.Dialog.Close(true);
        }

        private void SwitchAuditButtons(bool isAudit)
        {
            this.btnAudit.Visibility = (isAudit ? Visibility.Visible : Visibility.Collapsed);
            this.btnDeny.Visibility = (isAudit ? Visibility.Visible : Visibility.Collapsed);
            this.txtConfirmNote.IsEnabled = isAudit;
        }

        private void hlbSo_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel["sosysno"] != null)
            {
                CPApplication.Current.CurrentPage.Context.Window.Navigate(string.Format(ConstValue.SOMaintainUrlFormat, viewModel["sosysno"]), null, true);
            }
        }

    }
}
