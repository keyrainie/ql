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
using ECCentral.Portal.UI.Common.Facades;
using ECCentral.BizEntity.Common;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.Common.UserControls
{
    public partial class UCReasonCodeEdit : UserControl
    {
        int sysNo;
        int? parentSysNo;
        string desc;
        bool isUpdate;
        string path;
        public IDialog Dialog { get; set; }
        ReasonCodeFacade m_facade;

        public UCReasonCodeEdit(int sysNo, int? parentSysNo,string desc,string path,bool isUpdate )
        {
            InitializeComponent();
            this.sysNo = sysNo;
            this.parentSysNo = parentSysNo;
            this.desc = desc;
            this.isUpdate = isUpdate;
            this.path = path;

            m_facade = new ReasonCodeFacade();

            this.Loaded += new RoutedEventHandler(UCReasonCodeEdit_Load);
        }

        void UCReasonCodeEdit_Load(object sender, RoutedEventArgs e)
        {
            this.tbxReasonCodePath.Text = path;
            if (isUpdate)
            {
                this.tbxDesc.Text = desc;
            }


        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (!isUpdate)
            {
                string desc=tbxDesc.Text;

                if (string.IsNullOrEmpty(desc))
                {
                    tbxDesc.SetValidation("不能为空");
                    tbxDesc.RaiseValidationError();
                    return;
                }

                ReasonCodeEntity node=new ReasonCodeEntity{
                    Description=desc
                    , ParentNodeSysNo=this.sysNo
                    , ReasonCodeID=""
                    , Status=1
                    , LanguageCode=CPApplication.Current.LanguageCode
                    , InUser=CPApplication.Current.LoginUser.DisplayName
                    , CompanyCode=CPApplication.Current.CompanyCode
                };

                m_facade.InsertReasonCode(node, (o, arg) => {

                    if (arg.FaultsHandle())
                    {
                        return;
                    }

                    this.Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                    this.Dialog.Close(true);

                });
 
            }
            else
            {
                string desc = tbxDesc.Text;

                if (string.IsNullOrEmpty(desc))
                {
                    tbxDesc.SetValidation("不能为空");
                    tbxDesc.RaiseValidationError();
                    return;
                }


                ReasonCodeEntity node = new ReasonCodeEntity
                {
                    SysNo=this.sysNo,
                    Description = desc,
                    EditUser = CPApplication.Current.LoginUser.DisplayName,
                    CompanyCode = CPApplication.Current.CompanyCode
                };

                m_facade.UpdateReasonCode(node, (o, arg) =>
                {

                    if (arg.FaultsHandle())
                    {
                        return;
                    }

                    this.Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                    this.Dialog.Close(true);

                });

            }


        }

        private void btnCancle_Click(object sender, RoutedEventArgs e)
        {
            this.Dialog.ResultArgs.DialogResult = DialogResultType.Cancel;
            this.Dialog.Close(true);

        }






    }
}
