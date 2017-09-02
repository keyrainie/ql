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
using ECCentral.Portal.UI.MKT.NeweggCN.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.Portal.UI.MKT.UserControls;
using System.Collections;

namespace ECCentral.Portal.UI.MKT.NeweggCN.UserControls
{
    public partial class UCBigArea : UserControl
    {
        public UCBigArea()
        {
            InitializeComponent();
            //BindDataSource();

            this.lstBigAreas.SelectionChanged += new SelectionChangedEventHandler(lstBigAreas_SelectionChanged);

            this.DataContextChanged += new DependencyPropertyChangedEventHandler(UCBigArea_DataContextChanged);

            this.Loaded += new RoutedEventHandler(UCBigArea_Loaded);
        }

        void UCBigArea_Loaded(object sender, RoutedEventArgs e)
        {
            BindDataSource();
        }

        void UCBigArea_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.DataContext != null)
            {
                this.lstBigAreas.SelectedValue = this.BigProvinceSysNo;
            }
        }

        void lstBigAreas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int bigProvinceSysNo;
            if (this.lstBigAreas.SelectedValue != null
                && int.TryParse(this.lstBigAreas.SelectedValue.ToString(), out bigProvinceSysNo))
            {
                SetValue(BigProvinceSysNoProperty, bigProvinceSysNo.ToString());
            }
            else
            {
                SetValue(BigProvinceSysNoProperty, null);
            }
            
        }

        public static readonly DependencyProperty BigProvinceSysNoProperty =
            DependencyProperty.Register("BigProvinceSysNo", typeof(string), typeof(UCBigArea), new PropertyMetadata(null));

        public enum BigAreaEdiMode
        {
            Query,
            Maintain
        }

        /// <summary>
        ///业务模式，查询，维护 
        /// </summary>
        private BigAreaEdiMode _bizMode;
        public BigAreaEdiMode BizMode
        {
            get
            {
                return _bizMode;
            }
            set
            {
                _bizMode = value;
                BindDataSource();
            }
        }

        /// <summary>
        /// 绑定大区列表。
        /// </summary>
        private void BindDataSource()
        {
            if (BigAreaFacade.AllBigAreas == null)
            {

                BigAreaFacade facade = new BigAreaFacade(CPApplication.Current.CurrentPage);
                facade.GetAllBigAreas((s, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    var rows = args.Result.Rows.ToList();

                    if (BigAreaFacade.AllBigAreas == null)
                    {
                        BigAreaFacade.AllBigAreas = rows;
                    }

                    BindSourceToGrid(BigAreaFacade.AllBigAreas);
                });
            }
            else
            {
                BindSourceToGrid(BigAreaFacade.AllBigAreas);
            }
        }

        /// <summary>
        /// 将数据源绑定到列表。
        /// </summary>
        /// <param name="dataSource"></param>
        private void BindSourceToGrid(object dataSource)
        {
            var source = dataSource as dynamic;

            if (source == null)
            {
                return;
            }

            List<CodeNamePair> items = new List<CodeNamePair>();

            for (int i = 0; i < source.Count; i++)
            {
                CodeNamePair item = new CodeNamePair();
                item.Code = source[i].BigProvinceSysNo.ToString();
                item.Name = source[i].BigProvinceName;

                items.Add(item);
            }

            if (BizMode == BigAreaEdiMode.Query)
            {
                items.Insert(0, new CodeNamePair { Name = ResCommonEnum.Enum_All });
            }
            else
            {
                items.Insert(0, new CodeNamePair { Name = ResCommonEnum.Enum_All, Code = "0" });
            }

            this.lstBigAreas.ItemsSource = items;
            this.lstBigAreas.SelectedValue = "0";
        }


        /// <summary>
        /// 选择的大区SysNo
        /// </summary>
        public string BigProvinceSysNo
        {
            get
            {
                //int bigProvinceSysNo;
                //if (this.lstBigAreas.SelectedValue != null
                //    && int.TryParse(this.lstBigAreas.SelectedValue.ToString(), out bigProvinceSysNo))
                //{
                //    return bigProvinceSysNo.ToString();
                //}
                //else
                //{
                //    //return null;
                //    return (string)GetValue(BigProvinceSysNoProperty);
                //}
                return (string)GetValue(BigProvinceSysNoProperty);
            }
            set
            {
                this.lstBigAreas.SelectedValue = value;
                SetValue(BigProvinceSysNoProperty, value);
            }
        }
    }
}
