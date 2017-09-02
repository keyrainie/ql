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
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.IM.Resources;
using ECCentral.BizEntity.IM;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic.Components.UserControls.Language;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.IM.UserControls
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class PropertyValueMaintain : PageBase
    {

        private bool _isEditing = false;
        private int _sysNo;
        private int _editingPropertySysNo;
        private List<PropertyValueVM> _vmList;
        public IWindow MyWindow { get; set; }

        List<ValidationEntity> validationCondition;

        public IDialog Dialog { get; set; }

        private Action CheckPVStatus;

        public PropertyValueMaintain()
        {
            InitializeComponent();
        }

        public void BeginEditing(int sysNo)
        {            
            _editingPropertySysNo = sysNo;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            BuildValidateCondition();
            CheckPVStatus = SetCheckPVStatus;
            dgPropertyValueQueryResult.Bind();
          
        }

        private void BuildValidateCondition()
        {
            validationCondition = new List<ValidationEntity>();
            validationCondition.Add(new ValidationEntity(ValidationEnum.IsInteger, this.tbPropertyPriority.Text.Trim(), ResPropertyValueMaintain.Msg_IsInteger));
        }

        private bool ValidateCondition()
        {
            bool ret = true;

            if (!ValidationHelper.Validation(this.tbPropertyPriority, validationCondition))
            {
                ret = false;
            }
            return ret;
        }

        private void btnPropertyValueSave_Click(object sender, RoutedEventArgs e)
        {
            //有效性验证
            if (!ValidateCondition())
            {
                return;
            }

            PropertyValueFacade facade = new PropertyValueFacade();
            PropertyValueVM vm = this.DataContext as PropertyValueVM;
            if (vm == null)
            {
                return;
            }
            //ValidationManager.Validate(ChildLayoutRoot);
            ValidationManager.Validate(this);
            if (vm.ValidationErrors.Count != 0) return;

            vm.SysNo = _sysNo;
            vm.PropertySysNo = Convert.ToInt32(_editingPropertySysNo);
            CheckPVStatus = null;
            if (_isEditing)
            {
                facade.UpdatePropertyValue(vm, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }

                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResPropertyMaintain.Info_SaveSuccessfully);
                    //_isEditing = true;
                    this.DataContext = new PropertyValueVM();
                    tbPropertyValueName.Text = "";
                    tbPropertyPriority.Text = "";
                    cbPropertyValueStatus.SelectedIndex = 0;
                    chk_SelectPropertyValueStatus.IsChecked = false;
                    dgPropertyValueQueryResult.Bind();
                });
            }
            else
            {
                facade.CreatePropertyValue(vm, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }

                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResPropertyValueMaintain.Info_SaveSuccessfully);
                    //_sysNo = args.Result.SysNo.Value;
                    //_isEditing = true;
                    this.DataContext = new PropertyValueVM();
                    tbPropertyValueName.Text = "";
                    tbPropertyPriority.Text = "";
                    cbPropertyValueStatus.SelectedIndex = 0;
                    chk_SelectPropertyValueStatus.IsChecked = false;
                    dgPropertyValueQueryResult.Bind();
                });
            }
        }

        private void dgPropertyValueQueryResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            if (_editingPropertySysNo > 0)
            {
                PropertyValueQueryVM model = new PropertyValueQueryVM();
                model.PropertySysNo = _editingPropertySysNo;

                PropertyValueQueryFacade facade = new PropertyValueQueryFacade();
                facade.QueryPropertyValueListByPropertySysNo(model, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
                {
                    _vmList = DynamicConverter<PropertyValueVM>.ConvertToVMList<List<PropertyValueVM>>(args.Result.Rows);
                    this.dgPropertyValueQueryResult.ItemsSource = _vmList;
                    this.dgPropertyValueQueryResult.TotalCount = args.Result.TotalCount;
                    if (CheckPVStatus!=null)
                    {
                        CheckPVStatus();
                    }
                    //首次加载页面
                    if (!_isEditing)
                    {
                        this.DataContext = new PropertyValueVM();
                        tbPropertyValueName.Text = "";
                        tbPropertyPriority.Text = "";
                        cbPropertyValueStatus.SelectedIndex = 0;
                    }

                });
            }
        }

        /// <summary>
        /// 编辑属性值链接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hyperlinkPropertyValueSysNo_Click(object sender, RoutedEventArgs e)
        {
            dynamic property = this.dgPropertyValueQueryResult.SelectedItem as dynamic;

            _sysNo = property.SysNo;
            tbPropertyValueName.Text = property.ValueDescription;
            tbPropertyPriority.Text = property.Priority.ToString();
            if (property.Status == "有效")
            { cbPropertyValueStatus.SelectedIndex = 0;  }
            else
            { cbPropertyValueStatus.SelectedIndex = 1;  }
            

            _isEditing = true;
        }

        /// <summary>
        /// 更新属性值多语言
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hyperlinkPropertyValueMultiLanguage_Click(object sender, RoutedEventArgs e)
        {
            dynamic property = this.dgPropertyValueQueryResult.SelectedItem as dynamic;

            UCMultiLanguageMaintain item = new UCMultiLanguageMaintain(property.SysNo, "PIM_PropertyValue");
            item.Dialog = MyWindow.ShowDialog("更新属性值多语言", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    this.dgPropertyValueQueryResult.Bind();
                }
            }, new Size(750, 600));
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
        }

        private void CloseDialog(DialogResultType dialogResult)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = dialogResult;
                Dialog.Close();
            }
        }

        private void chk_CheckPVStatus_Click(object sender, RoutedEventArgs e)
        {
            if(_vmList!=null)
            {
                List<PropertyValueVM> activeStatusList = _vmList.Where(p => p.Status == "有效").ToList();
                this.dgPropertyValueQueryResult.ItemsSource = activeStatusList;
                this.dgPropertyValueQueryResult.TotalCount = activeStatusList.Count;
            }
          
        }

        private void chk_UnCheckPVStatus_Click(object sender, RoutedEventArgs e)
        {
            this.dgPropertyValueQueryResult.ItemsSource = _vmList;
            this.dgPropertyValueQueryResult.TotalCount = _vmList.Count;
        }

        private  void SetCheckPVStatus()
        {
            chk_CheckPVStatus_Click(null, null);
        }
    }
}
