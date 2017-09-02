using System.Windows;
using System.Windows.Controls;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class CategoryPropertyMaintainDetail : UserControl
    {
        #region 属性以及字段
        private CategoryPropertyFacade _facade;
        public IDialog Dialog { get; set; }
        List<ValidationEntity> validationCondition1;
        List<ValidationEntity> validationCondition2;

        public int SysNo { get; set; }

        public CategoryPropertyMaintainDetail()
        {
            InitializeComponent();
            this.Loaded += UserControl_Loaded;
        }
        #endregion

        #region 初始化加载
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            BuildValidateCondition();
            BindPage();
        }
        #endregion

        #region 查询绑定
        private void BindPage()
        {
            if (SysNo >= 0)
            {
                _facade = new CategoryPropertyFacade();
                _facade.GetCategoryPropertyBySysNo(SysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    if (args.Result == null)
                    {
                        CPApplication.Current.CurrentPage.Context.Window.Alert("获取属性分类失败");
                        return;
                    }
                    var vm = args.Result.Convert<CategoryProperty, CategoryPropertyVM>();
                    vm.Property = args.Result.Property.Convert<PropertyInfo, PropertyVM>();
                    vm.PropertyGroup = args.Result.PropertyGroup.Convert<PropertyGroupInfo, PropertyGroupInfoVM>
                        ((v, t) =>
                          {
                              t.PropertyGroupName = v.PropertyGroupName.Content;
                          });
                    DataContext = vm;
                });
            }
            else
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("获取属性分类失败");
                return;
            }
        }
        #endregion

        #region 页面内按钮处理事件

        #region 界面事件

        private void BuildValidateCondition()
        {
            validationCondition1 = new List<ValidationEntity>();
            validationCondition2 = new List<ValidationEntity>();
            validationCondition1.Add(new ValidationEntity(ValidationEnum.IsInteger, this.tb_Priority.Text.Trim(), ResPropertyValueMaintain.Msg_IsInteger));
            validationCondition2.Add(new ValidationEntity(ValidationEnum.IsInteger, this.tb_SearchPriority.Text.Trim(), ResPropertyValueMaintain.Msg_IsInteger));
        }

        private bool ValidateCondition()
        {
            bool ret = true;

            if (!ValidationHelper.Validation(this.tb_Priority, validationCondition1))
            {
                ret = false;
            }
            if (!ValidationHelper.Validation(this.tb_SearchPriority, validationCondition2))
            {
                ret = false;
            }
            return ret;
        }


        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            //有效性验证
            if (!ValidateCondition())
            {
                return;
            }

            var vm = DataContext as CategoryPropertyVM;
            if (vm == null)
            {
                return;
            }
            ValidationManager.Validate(this);
            if (vm.ValidationErrors.Count != 0) return;

            if (vm.PropertyGroup == null || vm.PropertyGroup.PropertyGroupName == null)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("属性分类组名不能为空");
                return;
            }
            _facade = new CategoryPropertyFacade();
            _facade.UpdateCategoryProperty(vm, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    var errorMsg = args.Error.Faults[0].ErrorDescription;
                    CPApplication.Current.CurrentPage.Context.Window.Alert(errorMsg);
                    return;
                }
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResBrandMaintain.Info_SaveSuccessfully);
                CloseDialog(DialogResultType.OK);
            });
        }

        private void CloseDialog(DialogResultType dialogResult)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = dialogResult;
                Dialog.Close();
            }
        }


        #endregion

        #endregion




    }
}
