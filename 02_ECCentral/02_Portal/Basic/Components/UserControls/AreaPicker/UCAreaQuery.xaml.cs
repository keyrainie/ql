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
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.Basic.Components.UserControls.AreaPicker
{
    public partial class UCAreaQuery : UserControl
    {
        private AreaQueryVM m_dataContext;
        private AreaQueryFacade m_Facade = null;
        public IDialog DialogHandler { get; set; }
        private IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }
        public List<AreaQueryVM> listInfo;
        public UCAreaQuery()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(UCCategoryQuery_Loaded);
        }
        void UCCategoryQuery_Loaded(object sender, RoutedEventArgs e)
        {
            m_dataContext=new AreaQueryVM();
            m_Facade = new AreaQueryFacade();
            this.DataContext = m_dataContext;
        }

        private void Query_Click(object sender, RoutedEventArgs e)
        {
            gridResult.Bind();
        }

        private void gridResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            m_Facade.QueryAreaList(m_dataContext, e.PageSize, e.PageIndex, e.SortField, (f, args) =>
            {
                gridResult.ItemsSource = args.Result.Rows.ToList("IsChecked", false);
                gridResult.TotalCount = args.Result.TotalCount;
            });
        }

        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            CheckBox ck=(CheckBox)sender;
            dynamic rows = gridResult.ItemsSource;
            if (rows != null)
            {
                foreach (var row in rows)
                {
                    if (row != null)
                    {
                        row.IsChecked = ck.IsChecked.Value;
                    }
                }
            }
        }

        private void ButtonCloseDialog_Click(object sender, RoutedEventArgs e)
        {
            this.DialogHandler.ResultArgs.DialogResult = DialogResultType.Cancel;
            this.DialogHandler.Close();
        }

        private void ButtonConfirmSelected_Click(object sender, RoutedEventArgs e)
        {
            //关闭对话框并返回数据
            listInfo = new List<AreaQueryVM>();
            dynamic rows=gridResult.ItemsSource;
            if (rows != null)
            {
                foreach (var row in rows)
                {
                    if (row.IsChecked)
                    {
                        AreaQueryVM item = new AreaQueryVM();
                        item.SysNo = row.SysNo;
                        item.ProvinceName = row.ProvinceName;
                        item.CityName = row.CityName;
                        listInfo.Add(item);
                    }
                }
            }
            if (listInfo.Count <= 0)
            {
                this.CurrentWindow.Alert("请选择数据！", MessageType.Warning);
                return;
            }
            this.DialogHandler.ResultArgs.Data = listInfo;
            this.DialogHandler.ResultArgs.DialogResult = DialogResultType.OK;
            this.DialogHandler.Close();
        }
    }
}
