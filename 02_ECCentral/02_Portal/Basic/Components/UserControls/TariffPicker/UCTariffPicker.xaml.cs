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
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Components.UserControls.CategoryPicker;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.Basic.Components.UserControls.TariffPicker
{
    public partial class UCTariffPicker : UserControl
    {
        private TariffInfoVM vm;

        public TariffInfoVM SelectModel
        {
            get { return vm ?? (vm = new TariffInfoVM()); }
            set { vm = value; }
        }

        private TariffPickerVM _currentVM;
        public TariffPickerVM CurrentVM
        {
            get { return _currentVM ?? (_currentVM = new TariffPickerVM()); }
            set { _currentVM = value; }
        }
        public string SelectedCode { get; set; }
        //private string selectedCode1 = string.Empty;
        //private string selectedCode2 = string.Empty;
        //private string selectedCode3 = string.Empty;
        private TariffFacade tariffFacade;
        private int isNeed = 0;
        //public Action OnLoadCategory;
        public UCTariffPicker()
        {
            InitializeComponent();
            this.DataContext = CurrentVM;
            this.Loaded += UCTariffPicker_Loaded;
        }
        private void SetSelectedTariff(string selectedCode)
        {
            string code1 = null;
            string code2 = null;
            string code3 = null;
            if (CurrentVM != null && !String.IsNullOrWhiteSpace(selectedCode))
            {
                var len = selectedCode.Length;
                // 解析 _selectedCode
                switch (len)
                {
                    case 2:
                        code1 = selectedCode;
                        break;
                    case 4:
                        code1 = selectedCode.Substring(0, 2);
                        code2 = selectedCode.Substring(0, 4);
                        break;
                    default:
                        code1 = selectedCode.Substring(0, 2);
                        code2 = selectedCode.Substring(0, 4);
                        code3 = selectedCode.Substring(0, 6);
                        break;
                }

                //CurrentVM.SelectedLevelCode1 = code1;
                //CurrentVM.SelectedLevelCode2 = code2;
                //CurrentVM.SelectedLevelCode3 = code3;
            }
            isNeed = 0;
            if (!string.IsNullOrWhiteSpace(code1))
            {
                isNeed++;
                SelectModel.Tcode = code1;
                tariffFacade.QueryTariffCategory("0", (o, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        var infos = args.Result;
                        infos.Insert(0,
                                     new TariffInfo()
                                         {
                                             Tcode = "",
                                             ItemCategoryName = ResCategoryPicker.ComboBox_SelectItem_DefaultText
                                         });
                        List<TariffInfoVM> tariffvmList =
                            infos.Convert<TariffInfo, TariffInfoVM, List<TariffInfoVM>>();
                        CurrentVM.Level1 = tariffvmList;
                        CurrentVM.SelectedLevelCode1 = code1;
                        cmbTariff1.SelectedValue = tariffvmList.SingleOrDefault(f => f.Tcode == code1);
                        if (!string.IsNullOrWhiteSpace(code2))
                        {
                            cmbTariff2.IsEnabled = true;
                        }
                    });
            }

            if (!string.IsNullOrWhiteSpace(code2))
            {
                isNeed++;
                SelectModel.Tcode = code2;
                tariffFacade.QueryTariffCategory(code1, (o2, args2) =>
                {
                    if (args2.FaultsHandle())
                    {
                        return;
                    }
                    var infos2 = args2.Result;
                    infos2.Insert(0, new TariffInfo() { Tcode = "", ItemCategoryName = ResCategoryPicker.ComboBox_SelectItem_DefaultText });
                    List<TariffInfoVM> tariffvmList2 = infos2.Convert<TariffInfo, TariffInfoVM, List<TariffInfoVM>>();
                    CurrentVM.Level2 = tariffvmList2;
                    CurrentVM.SelectedLevelCode2 = code2;
                    cmbTariff2.SelectedValue = tariffvmList2.SingleOrDefault(f => f.Tcode == code2);
                    if (!string.IsNullOrWhiteSpace(code3))
                    {
                        cmbTariff3.IsEnabled = true;
                    }
                });
            }
            if (!string.IsNullOrWhiteSpace(code3))
            {
                isNeed++;
                SelectModel.Tcode = code3;
                tariffFacade.QueryTariffCategory(code2, (o3, args3) =>
                {
                    if (args3.FaultsHandle())
                    {
                        return;
                    }
                    var infos3 = args3.Result;
                    infos3.Insert(0, new TariffInfo() { Tcode = "", ItemCategoryName = ResCategoryPicker.ComboBox_SelectItem_DefaultText });
                    List<TariffInfoVM> tariffvmList3 = infos3.Convert<TariffInfo, TariffInfoVM, List<TariffInfoVM>>();
                    CurrentVM.Level3 = tariffvmList3;
                    CurrentVM.SelectedLevelCode3 = code3;
                    cmbTariff3.SelectedValue = tariffvmList3.SingleOrDefault(f => f.Tcode == code3);
                });
            }

        }

        void UCTariffPicker_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= UCTariffPicker_Loaded;
            tariffFacade = new TariffFacade(CPApplication.Current.CurrentPage);

            InitializeCategoryComboBox();

        }


        private void InitializeCategoryComboBox()
        {
            cmbTariff2.IsEnabled = cmbTariff3.IsEnabled = false;
            tariffFacade.QueryTariffCategory("0", (o, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                var infos = args.Result;
                infos.Insert(0,
                             new TariffInfo()
                             {
                                 Tcode = "",
                                 ItemCategoryName = ResCategoryPicker.ComboBox_SelectItem_DefaultText
                             });
                List<TariffInfoVM> tariffvmList =
                    infos.Convert<TariffInfo, TariffInfoVM, List<TariffInfoVM>>();
                cmbTariff1.ItemsSource = CurrentVM.Level1 = tariffvmList;
                cmbTariff1.SelectedIndex = 0;
            });
            SetSelectedTariff(this.SelectedCode);
            //OnLoadingCategoryCompleted();
            //this.Content.
        }

        #region 税率选择
        private void CmbTariff1_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbTariff1.SelectedIndex > 0)
            {
                if (isNeed > 0)
                {
                    isNeed--;
                    return;
                }
                cmbTariff2.IsEnabled = cmbTariff3.IsEnabled = false;
                cmbTariff2.ItemsSource = cmbTariff3.ItemsSource = null;
                var info = cmbTariff1.SelectedItem as TariffInfoVM;

                //SelectModel = info;

                if (info != null)
                {
                    SelectModel.TariffCode = info.TariffCode;
                    SelectModel.ItemCategoryName = info.ItemCategoryName;
                    SelectModel.Tcode = info.Tcode;
                    SelectModel.SysNo = info.SysNo;


                    tariffFacade.QueryTariffCategory(info.Tcode, (o, args) =>
                    {

                        if (args.FaultsHandle())
                        {
                            return;
                        }

                        var infos = args.Result;
                        if (infos.Count > 0)
                        {
                            infos.Insert(0, new TariffInfo() { Tcode = "", ItemCategoryName = ResCategoryPicker.ComboBox_SelectItem_DefaultText });
                            var infoVms = infos.Convert<TariffInfo, TariffInfoVM, List<TariffInfoVM>>();
                            cmbTariff2.ItemsSource = infoVms;
                            cmbTariff2.SelectedIndex = 0;
                            CurrentVM.Level2 = infoVms;
                            cmbTariff2.IsEnabled = true;
                        }
                    });

                }
            }
        }

        private void CmbTariff2_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbTariff2.SelectedIndex > 0)
            {
                if (isNeed > 0)
                {
                    isNeed--;
                    return;
                }
                cmbTariff3.IsEnabled = false;
                cmbTariff3.ItemsSource = null;
                var info = cmbTariff2.SelectedItem as TariffInfoVM;
                if (info != null)
                {
                    SelectModel.TariffCode = info.TariffCode;
                    SelectModel.Tcode = info.Tcode;
                    SelectModel.ItemCategoryName = info.ItemCategoryName;
                    SelectModel.SysNo = info.SysNo;

                    tariffFacade.QueryTariffCategory(info.Tcode, (o, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        var infos = args.Result;
                        if (infos.Count > 0)
                        {
                            infos.Insert(0, new TariffInfo() { Tcode = "", ItemCategoryName = ResCategoryPicker.ComboBox_SelectItem_DefaultText });
                            var infoVms = infos.Convert<TariffInfo, TariffInfoVM, List<TariffInfoVM>>();
                            cmbTariff3.ItemsSource = infoVms;
                            CurrentVM.Level3 = infoVms;
                            cmbTariff3.SelectedIndex = 0;
                            cmbTariff3.IsEnabled = true;
                        }
                    });

                }
            }

        }

        private void CmbTariff3_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbTariff3.SelectedIndex > 0)
            {
                if (isNeed > 0)
                {
                    isNeed--;
                    return;
                }
                var info = cmbTariff3.SelectedItem as TariffInfoVM;
                if (info != null)
                {
                    SelectModel.TariffCode = info.TariffCode;
                    SelectModel.ItemCategoryName = info.ItemCategoryName;
                    SelectModel.SysNo = info.SysNo;
                    SelectModel.Tcode = info.Tcode;
                }
            }
        }
        #endregion
    }
}
