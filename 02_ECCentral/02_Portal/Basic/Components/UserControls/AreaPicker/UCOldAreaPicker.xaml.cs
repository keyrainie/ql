using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
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

namespace ECCentral.Portal.Basic.Components.UserControls.AreaPicker
{
    public partial class UCOldAreaPicker : UserControl
    {
        /***********************************************************************************************************************************************
         地域Area为省，市，区的一种
         ***********************************************************************************************************************************************/

        #region 依赖属性
        //地区
        public static readonly DependencyProperty SelectedAreaSysNoProperty =
            DependencyProperty.Register("SelectedAreaSysNo", typeof(string), typeof(UCOldAreaPicker), new PropertyMetadata(null));
        //省
        public static readonly DependencyProperty SelectedProvinceSysNoProperty =
            DependencyProperty.Register("SelectedProvinceSysNo", typeof(string), typeof(UCOldAreaPicker), new PropertyMetadata(null));
        //市
        public static readonly DependencyProperty SelectedCitySysNoProperty =
            DependencyProperty.Register("SelectedCitySysNo", typeof(string), typeof(UCOldAreaPicker), new PropertyMetadata(null));
        //区
        public static readonly DependencyProperty SelectedDistrictSysNoProperty =
            DependencyProperty.Register("SelectedDistrictSysNo", typeof(string), typeof(UCOldAreaPicker), new PropertyMetadata(null));
        //市名称
        public static readonly DependencyProperty SelectedCityNameProperty =
    DependencyProperty.Register("SelectedCityName", typeof(string), typeof(UCOldAreaPicker), new PropertyMetadata(null));
        //省名称
        public static readonly DependencyProperty SelectedProvinceNameProperty =
            DependencyProperty.Register("SelectedProvinceName", typeof(string), typeof(UCOldAreaPicker), new PropertyMetadata(null));

        /// <summary>
        /// 选择的地区编号
        /// </summary>
        public string SelectedAreaSysNo
        {
            get
            {
                return (string)GetValue(SelectedAreaSysNoProperty);
            }
            set
            {
                SetValue(SelectedAreaSysNoProperty, value);
            }
        }

        /// <summary>
        /// 选择省的编号
        /// </summary>
        public string SelectedProvinceSysNo
        {
            get
            {
                return (string)GetValue(SelectedProvinceSysNoProperty);
            }
            set
            {
                SetValue(SelectedProvinceSysNoProperty, value);
            }
        }
        /// <summary>
        /// 选择市的编号
        /// </summary>
        public string SelectedCitySysNo
        {
            get
            {
                return (string)GetValue(SelectedCitySysNoProperty);
            }
            set
            {
                SetValue(SelectedCitySysNoProperty, value);
            }
        }
        /// <summary>
        /// 选择区的编号
        /// </summary>
        public string SelectedDistrictSysNo
        {
            get
            {
                return (string)GetValue(SelectedDistrictSysNoProperty);
            }
            set
            {
                SetValue(SelectedDistrictSysNoProperty, value);
            }
        }
        /// <summary>
        /// 选择的省名称
        /// </summary>
        public string SelectedProvinceName
        {
            get
            {
                return (string)GetValue(SelectedProvinceNameProperty);
            }
            set
            {
                SetValue(SelectedProvinceNameProperty, value);
            }
        }
        /// <summary>
        /// 选择的市名称
        /// </summary>
        public string SelectedCityName
        {
            get
            {
                return (string)GetValue(SelectedCityNameProperty);
            }
            set
            {
                SetValue(SelectedCityNameProperty, value);
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 获取地域编号
        /// </summary>
        public int? QueryAreaSysNo
        {
            get
            {
                int? result = default(int?);
                //查询地区
                if (cmbArea.SelectedValue != null)
                {
                    result = Convert.ToInt32(cmbArea.SelectedValue);
                }
                else
                {
                    //查询城市
                    if (cmbCity.SelectedValue != null)
                    {
                        result = Convert.ToInt32(cmbCity.SelectedValue);
                    }
                    else
                    {
                        //查询省
                        if (cmbProvince.SelectedValue != null)
                        {
                            result = Convert.ToInt32(cmbProvince.SelectedValue);
                        }
                    }
                }
                return result;
            }
        }
        #endregion

        #region 预定义

        private bool needChange = true;
        private List<KeyValuePair<string, string>> source;

        /// <summary>
        /// Model
        /// </summary>
        private AreaVM_Old m_dataContext;
        private AreaVM_Old ViewModel
        {
            get
            {
                if (m_dataContext == null)
                {
                    m_dataContext = new AreaVM_Old();
                }
                return m_dataContext;
            }
            set
            {
                m_dataContext = value;
            }
        }
        /// <summary>
        /// facade
        /// </summary>
        private AreaQueryFacade m_Facade = null;
        private AreaQueryFacade Facade
        {
            get
            {
                if (m_Facade == null)
                {
                    m_Facade = new AreaQueryFacade();
                }
                return m_Facade;
            }
        }

        /// <summary>
        /// 请选择 数据源
        /// </summary>
        private List<KeyValuePair<string, string>> SingleSource
        {
            get
            {
                if (source == null)
                {
                    source = new List<KeyValuePair<string, string>>();
                    source.Insert(0, new KeyValuePair<string, string>(null, ResLinkableDataPicker.ComboBox_PleaseSelect));
                }
                return source;
            }
        }


        #endregion

        #region 事件及其相关方法

        /// <summary>
        /// 初始化
        /// </summary>
        public UCOldAreaPicker()
        {
            InitializeComponent();
            this.DataContextChanged += new DependencyPropertyChangedEventHandler(AreaPicker_DataContextChanged);
            Loaded += new RoutedEventHandler(AreaPicker_Loaded);

        }

        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AreaPicker_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(AreaPicker_Loaded);

            var AreaExp = this.GetBindingExpression(UCOldAreaPicker.SelectedAreaSysNoProperty);
            var ProvinceExp = this.GetBindingExpression(UCOldAreaPicker.SelectedProvinceSysNoProperty);
            var CityExp = this.GetBindingExpression(UCOldAreaPicker.SelectedCitySysNoProperty);

            if (AreaExp != null && AreaExp.ParentBinding != null)
            {
                cmbArea.SetBinding(Newegg.Oversea.Silverlight.Controls.Combox.SelectedValueProperty, AreaExp.ParentBinding);
            }
            if (ProvinceExp != null && ProvinceExp.ParentBinding != null)
            {
                cmbProvince.SetBinding(Newegg.Oversea.Silverlight.Controls.Combox.SelectedValueProperty, ProvinceExp.ParentBinding);
            }
            if (CityExp != null && CityExp.ParentBinding != null)
            {
                cmbCity.SetBinding(Newegg.Oversea.Silverlight.Controls.Combox.SelectedValueProperty, CityExp.ParentBinding);
            }
        }

        /// <summary>
        /// 绑定源变化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AreaPicker_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.DataContext != null)
            {
                BindData();
            }
        }

        public T GetParentObject<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);

            while (parent != null)
            {
                if (parent is T && (((T)parent).Name == name | string.IsNullOrEmpty(name)))
                {
                    return (T)parent;
                }

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }
        /// <summary>
        /// 绑定数据
        /// </summary>
        private void BindData()
        {

            string query = string.IsNullOrEmpty(this.SelectedAreaSysNo) ? "-999" : this.SelectedAreaSysNo;
            //MessageBox.Show("第一步" + this.SelectedAreaSysNo ?? "");
            if (query == "-999" && GetParentObject<UserControl>(this, "ucContactInfo") != null)
            {
                return;
            }
            Facade.QueryCurrentAreaStructure_Old(System.Convert.ToInt32(query), (obj, args) =>
            {

                //MessageBox.Show("第二步" + this.SelectedAreaSysNo ?? "");
                if (args.FaultsHandle())
                {
                    return;
                }
                ViewModel = AreaQueryTransform.Transform_Old(args.Result);

                string areaSysNo = null;
                if (!string.IsNullOrEmpty(this.SelectedAreaSysNo))
                //if (ViewModel.CurrentArea.SysNo.HasValue)
                {
                    areaSysNo = this.SelectedAreaSysNo;
                    //areaSysNo = ViewModel.CurrentArea.SysNo.ToString();
                }

                this.needChange = false;

                cmbProvince.ItemsSource = ViewModel.ProvinceAreaList;
                cmbCity.ItemsSource = ViewModel.CityeAreaList;
                //在给ComboBox的数据源赋值后，会触发SelectionChanged时间，导致SelectedAreaSysNo为null，所以需要把SelectedAreaSysNo先存起来
                cmbArea.ItemsSource = ViewModel.DistrictAreaList;
                //MessageBox.Show("第三步" + ViewModel.ProvinceAreaList.Count.ToString() + "," + ViewModel.CityeAreaList.Count.ToString() + "," + ViewModel.DistrictAreaList.Count.ToString());
                this.needChange = true;

                if (!string.IsNullOrEmpty(areaSysNo))
                {
                    //MessageBox.Show("第四步" + areaSysNo);
                    this.SelectedAreaSysNo = areaSysNo;
                }

                if (string.IsNullOrEmpty(this.SelectedAreaSysNo))
                {
                    cmbProvince.SelectedIndex = 0;
                }
                else
                {
                    this.needChange = false;

                    //MessageBox.Show("第五步" + ViewModel.CurrentArea.ProvinceSysNumber + "," + ViewModel.CurrentArea.CitySysNumber + "," + ViewModel.CurrentArea.SysNumber);
                    cmbProvince.SelectedValue = ViewModel.CurrentArea.ProvinceSysNumber;
                    cmbCity.SelectedValue = ViewModel.CurrentArea.CitySysNumber;
                    if (ViewModel.CurrentArea.DistrictName != null)
                    {
                        cmbArea.SelectedValue = ViewModel.CurrentArea.SysNumber;
                    }
                    else
                    {
                        cmbArea.SelectedIndex = 0;
                    }


                    this.needChange = true;
                }

            });


        }

        //public void BindSelectedData()
        //{
        //    if (!string.IsNullOrEmpty(this.SelectedAreaSysNo) && cmbArea.ItemsSource != null)
        //    {
        //        return;
        //    }
        //    string query = string.IsNullOrEmpty(this.SelectedAreaSysNo) ? "-999" : this.SelectedAreaSysNo;
        //    //MessageBox.Show("第一步" + this.SelectedAreaSysNo ?? "");

        //    Facade.QueryCurrentAreaStructure_Old(System.Convert.ToInt32(query), (obj, args) =>
        //    {
        //        // MessageBox.Show("第二步"+this.SelectedAreaSysNo??"");
        //        if (args.FaultsHandle())
        //        {
        //            return;
        //        }
        //        ViewModel = AreaQueryTransform.Transform_Old(args.Result);

        //        string areaSysNo = null;
        //        if (!string.IsNullOrEmpty(this.SelectedAreaSysNo))
        //        {
        //            areaSysNo = this.SelectedAreaSysNo;
        //        }

        //        this.needChange = false;

        //        cmbProvince.ItemsSource = ViewModel.ProvinceAreaList;
        //        cmbCity.ItemsSource = ViewModel.CityeAreaList;
        //        //在给ComboBox的数据源赋值后，会触发SelectionChanged时间，导致SelectedAreaSysNo为null，所以需要把SelectedAreaSysNo先存起来
        //        cmbArea.ItemsSource = ViewModel.DistrictAreaList;
        //        //MessageBox.Show("第三步"+ViewModel.ProvinceAreaList.Count.ToString() + "," + ViewModel.CityeAreaList.Count.ToString() + "," + ViewModel.DistrictAreaList.Count.ToString());
        //        this.needChange = true;

        //        if (!string.IsNullOrEmpty(areaSysNo))
        //        {
        //            //MessageBox.Show("第四步" + areaSysNo);
        //            this.SelectedAreaSysNo = areaSysNo;
        //        }

        //        if (string.IsNullOrEmpty(this.SelectedAreaSysNo))
        //        {
        //            cmbProvince.SelectedIndex = 0;
        //        }
        //        else
        //        {
        //            this.needChange = false;

        //            //MessageBox.Show("第五步" + ViewModel.CurrentArea.ProvinceSysNumber + "," + ViewModel.CurrentArea.CitySysNumber + "," + ViewModel.CurrentArea.SysNumber);
        //            cmbProvince.SelectedValue = ViewModel.CurrentArea.ProvinceSysNumber;
        //            cmbCity.SelectedValue = ViewModel.CurrentArea.CitySysNumber;
        //            if (ViewModel.CurrentArea.DistrictName != null)
        //            {
        //                cmbArea.SelectedValue = ViewModel.CurrentArea.SysNumber;
        //            }
        //            else
        //            {
        //                cmbArea.SelectedIndex = 0;
        //            }


        //            this.needChange = true;
        //        }
        //    });
        //}

        /// <summary>
        /// 选择省事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbProvince_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!this.needChange)
            {
                return;
            }
            if (cmbProvince.SelectedIndex > 0)
            {
                int provinceSysNumber = System.Convert.ToInt32(cmbProvince.SelectedValue);
                this.SelectedProvinceSysNo = provinceSysNumber.ToString();
                Facade.QueryCurrentAreaStructure_Old(provinceSysNumber, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    this.needChange = false;
                    ViewModel = AreaQueryTransform.Transform_Old(args.Result);
                    this.SelectedProvinceName = ViewModel.ProvinceAreaList.Where(f => f.Key == provinceSysNumber.ToString()).FirstOrDefault().Value;
                    cmbCity.ItemsSource = ViewModel.CityeAreaList;
                    this.needChange = true;
                    if (cmbCity.SelectedValue == null)
                    {
                        cmbArea.ItemsSource = this.SingleSource;
                        ClearValidationError(cmbArea);
                    }
                });
            }
            else
            {
                this.needChange = false;
                cmbCity.ItemsSource = SingleSource;
                cmbCity.SelectedIndex = 0;
                cmbArea.ItemsSource = SingleSource;
                cmbArea.SelectedIndex = 0;
                this.needChange = true;
            }
        }
        /// <summary>
        /// 选择市事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbCity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!this.needChange)
            {
                return;
            }
            if (cmbCity.SelectedIndex > 0)
            {
                int citySysNumber = System.Convert.ToInt32(cmbCity.SelectedValue);
                this.SelectedCitySysNo = citySysNumber.ToString();

                Facade.QueryCurrentAreaStructure_Old(citySysNumber, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    this.needChange = false;
                    ViewModel = AreaQueryTransform.Transform_Old(args.Result);
                    cmbArea.ItemsSource = ViewModel.DistrictAreaList;
                    this.SelectedCityName = ViewModel.CityeAreaList.Where(f => f.Key == citySysNumber.ToString()).FirstOrDefault().Value;
                    this.needChange = true;
                    ClearValidationError(cmbArea);
                });
            }
            else
            {
                this.needChange = false;
                cmbArea.ItemsSource = SingleSource;
                cmbArea.SelectedIndex = 0;
                this.needChange = true;
            }
        }
        /// <summary>
        /// 选择区域
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbArea_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbArea.SelectedIndex > 0)
            {
                this.SelectedDistrictSysNo = cmbArea.SelectedValue.ToString();
            }
            else
            {
                this.SelectedDistrictSysNo = string.Empty;
            }
        }

        /// <summary>
        /// 清理
        /// </summary>
        /// <param name="combBox"></param>
        private void ClearValidationError(ComboBox combBox)
        {
            var exp = this.GetBindingExpression(UCOldAreaPicker.SelectedAreaSysNoProperty);
            if (exp == null)
                return;

            string path = exp.ParentBinding.Path.Path;

            var model = combBox.DataContext as ModelBase;
            if (model != null)
            {
                var error = model.ValidationErrors.FirstOrDefault(p => p.MemberNames.Contains(path));
                if (error != null)
                {
                    model.ValidationErrors.Remove(error);
                }
            }
        }


        #endregion


    }
}
