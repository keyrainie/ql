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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.IM;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.Basic.Components.UserControls.BrandPicker
{
    public partial class UCBrandPicker : UserControl
    {
        #region [Properties]

        private readonly DependencyProperty SelectedBrandSysNoProperty =
    DependencyProperty.Register("SelectedBrandSysNo", typeof(string), typeof(UCBrandPicker), new PropertyMetadata(null, (s, e) =>
    {
        var uc = s as UCBrandPicker;
        uc.txtBrandSysNo.Text = (e.NewValue ?? "").ToString().Trim();
    }));

        private readonly DependencyProperty SelectedBrandNameProperty =
  DependencyProperty.Register("SelectedBrandName", typeof(string), typeof(UCBrandPicker), new PropertyMetadata(null, (s, e) =>
  {
      var uc = s as UCBrandPicker;
      uc.txtBrandNameLocal.Text = (e.NewValue ?? "").ToString().Trim();
  }));

        public event EventHandler<EventArgs> selectedBrandCompletedHandler;

        public string SelectedBrandSysNo
        {
            get
            {
                return ((string)base.GetValue(SelectedBrandSysNoProperty));
            }
            set
            {
                base.SetValue(SelectedBrandSysNoProperty, value);
            }
        }

        public string SelectedBrandName
        {
            get
            {
                return ((string)base.GetValue(SelectedBrandNameProperty));
            }
            set
            {
                base.SetValue(SelectedBrandNameProperty, value);
            }
        }

        public string ManufacturerName { get; set; }

        public int ManufacturerSysNo { get; set; }

        #endregion
        public BrandQueryFacade serviceFacade;
        public BrandQueryFilter queryFilter;
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
        public UCBrandPicker()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(UCBrandPicker_Loaded);
        }

        void UCBrandPicker_Loaded(object sender, RoutedEventArgs e)
        {
            serviceFacade = new BrandQueryFacade(CurrentPage);
            queryFilter = new BrandQueryFilter();
            this.Loaded -= UCBrandPicker_Loaded;
        }


        private void txtBrandNameLocal_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtBrandNameLocal.Text.Trim() == "")
            {
                txtBrandNameLocal.Text = "";
                txtBrandSysNo.Text = "";
                this.SelectedBrandSysNo = null;
                this.SelectedBrandName = null;
                return;
            }

            queryFilter.BrandNameLocal = this.txtBrandNameLocal.Text.Trim();
            queryFilter.PagingInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = 10,
                PageIndex = 0
            };
            serviceFacade.QueryBrands(queryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                var brandList = args.Result.Rows;
                try
                {
                    this.txtBrandSysNo.Text = brandList[0]["SysNo"].ToString();
                    this.txtBrandNameLocal.Text = brandList[0]["BrandName_Ch"].ToString();
                    this.SelectedBrandSysNo = brandList[0]["SysNo"].ToString();
                    this.SelectedBrandName = brandList[0]["BrandName_Ch"].ToString();
                }
                catch
                {
                    SelectedBrandName = null;
                    SelectedBrandSysNo = null;
                    this.txtBrandNameLocal.Text = string.Empty;
                    this.txtBrandSysNo.Text = string.Empty;
                    return;
                }
            });
        }

        private void txtBrandSysNo_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtBrandSysNo.Text.Trim() == "")
            {
                txtBrandNameLocal.Text = "";
                txtBrandSysNo.Text = "";
                this.SelectedBrandSysNo = null;
                this.SelectedBrandName = null;
                return;
            }

            int getInputBrandSysNo;
            if (!int.TryParse(this.txtBrandSysNo.Text.Trim(), out getInputBrandSysNo))
            {
                CurrentWindow.Alert("无效的品牌编号!");
                SelectedBrandName = null;
                SelectedBrandSysNo = null;
                this.txtBrandNameLocal.Text = string.Empty;
                this.txtBrandSysNo.Text = string.Empty;
                return;
            }
            queryFilter.BrandSysNo = getInputBrandSysNo;
            queryFilter.PagingInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = 10,
                PageIndex = 0
            };
            serviceFacade.QueryBrands(queryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                var brandList = args.Result.Rows;
                try
                {
                    this.txtBrandSysNo.Text = brandList[0]["SysNo"].ToString();
                    this.txtBrandNameLocal.Text = brandList[0]["BrandName_Ch"].ToString();
                    this.SelectedBrandSysNo = brandList[0]["SysNo"].ToString();
                    this.SelectedBrandName = brandList[0]["BrandName_Ch"].ToString();
                }
                catch
                {
                    CurrentWindow.Alert("无效的品牌编号!");
                    SelectedBrandName = null;
                    SelectedBrandSysNo = null;
                    this.txtBrandNameLocal.Text = string.Empty;
                    this.txtBrandSysNo.Text = string.Empty;
                    return;
                }
            });
        }

        private void ImageProductPicker_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UCBrandQuery selectDialog = new UCBrandQuery();
            selectDialog.Dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog("查询品牌", selectDialog, (obj, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    DynamicXml getSelectedBrand = args.Data as DynamicXml;
                    if (null != getSelectedBrand)
                    {
                        this.SelectedBrandSysNo = getSelectedBrand["SysNo"].ToString();
                        this.SelectedBrandName = string.IsNullOrEmpty(getSelectedBrand["BrandName_Ch"].ToString()) ? getSelectedBrand["BrandName_En"].ToString() : getSelectedBrand["BrandName_Ch"].ToString();
                        ManufacturerSysNo = Convert.ToInt32(getSelectedBrand["ManufacturerSysNo"]);
                        ManufacturerName = getSelectedBrand["ManufacturerName"].ToString();
                        if (selectedBrandCompletedHandler != null)
                        {
                            EventArgs argse = new EventArgs();
                            selectedBrandCompletedHandler(sender, argse);
                        }
                    }
                }

            }, new Size(750, 600));
        }

        private void txtBrandSysNo_KeyUp(object sender, KeyEventArgs e)
        {
            int no = 0;
            int.TryParse(txtBrandSysNo.Text.Trim(), out no);
            if (e.Key == Key.Enter && no != 0)
            {
                GetBrand(no, null);
                e.Handled = true;
            }
        }

        private void GetBrand(int? brandSysNo, string brandName)
        {
            BrandQueryFacade serviceFacade = new BrandQueryFacade(CurrentPage);
            BrandQueryFilter queryFilter = new BrandQueryFilter();
            if (!string.IsNullOrEmpty(brandName))
            {
                queryFilter.BrandNameLocal = brandName;
            }
            if (brandSysNo.HasValue)
            {
                queryFilter.BrandSysNo = brandSysNo;
            }

            queryFilter.PagingInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = 10,
                PageIndex = 0
            };

            serviceFacade.QueryBrands(queryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                var vendorList = args.Result.Rows;
                if (vendorList != null)
                {
                    List<dynamic> rows = vendorList.ToList();
                    if (rows.Count > 0)
                    {
                        var row = rows[0];
                        this.SelectedBrandSysNo = row.SysNo.ToString();
                        this.SelectedBrandName = row.BrandName_Ch;
                        ManufacturerSysNo = row.ManufacturerSysNo;
                        ManufacturerName = row.ManufacturerName;
                        if (selectedBrandCompletedHandler != null)
                        {
                            EventArgs argse = new EventArgs();
                            selectedBrandCompletedHandler(obj, args);
                        }
                    }
                    else
                    {
                        txtBrandNameLocal.Text = "";
                        txtBrandSysNo.Text = "";
                        this.SelectedBrandSysNo = "";
                        this.SelectedBrandName = "";
                    }
                }
                else
                {
                    txtBrandNameLocal.Text = "";
                    txtBrandSysNo.Text = "";
                    this.SelectedBrandSysNo = "";
                    this.SelectedBrandName = "";
                }

            });
        }

        private void txtBrandNameLocal_KeyUp(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter && txtBrandNameLocal.Text.Trim().Length != 0)
            {
                GetBrand(null, txtBrandNameLocal.Text.Trim());
                e.Handled = true;
            }
        }
    }
}
