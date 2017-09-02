using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;
using System.IO;
using System.Globalization;
using Newegg.Oversea.Silverlight.Controls.Primitives;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Core.Components;

namespace Newegg.Oversea.Silverlight.Controls.Data
{
    public class DataPager : System.Windows.Controls.DataPager
    {
        #region Const

        private const string DATAPAGER_elementTotalPage = "TotalPageTextBlock";
        private const string DATAPAGER_elementTotalCount = "TotalCount";
        private const string DATAPAGER_elementPageInfo = "PageInfo";
        private const string DATAPAGER_elementPerPage = "PerPage";
        private const string DATAPAGER_elementPageSize = "PageSize";
        private const string DATAPAGER_elementExportExcel = "ExportExcel";
        private const string DATAPAGER_elementExportTitle = "ExportTitle";
        private const string DATAPAGER_elementExportAllTitle = "ExportAllTitle";
        private const string DATAPAGER_elementExportBorder = "ExportBorder";
        private const string DATAPAGER_elementExportAll = "ExportAll";
        private const string DATAPAGER_elementPreviousButton = "PreviousPageButton";        
        private const string DATAPAGER_elementNextPageButton = "NextPageButton";
        private const string DATAPAGER_elementNextCurrentPageTextBox = "CurrentPageTextBox1";
        private const string DATAPAGER_elementGoButton = "GoButton";
        private const string DATAPAGER_elementFirstPageButton = "FirstPageButton";
        private const string DATAPAGER_elementLastPageButton = "LastPageButton";

        private const int Default_PageSize = 25;

        #endregion

        #region Private Fields

        private int _requestedPageIndex;

        private TextBlock _tbTotalCount;
        private TextBlock _tbPageInfo;
        private ComboBox _cmbPageSize;
        private HyperlinkButton _hlExportExcel;
        private Border _exportBorder;
        private HyperlinkButton _hlExportAll;
        
        private SaveFileDialog _dialog;
        private Stream _saveFileStream;
        private TextBlock _tbTotalPage;
        private TextBlock _tbPerPage;
        private TextBox _txtCurrentPage;
        private ComboBoxItem _cmbPageSizeAll;

        private Button _btnPreviousPage;
        private Button _btnNextPage;
        private Button _btnGo;
        private Button _firstPageButton;
        private Button _lastPageButton;

        #endregion

        internal DataGrid OwningGrid { get; set; }
        
        public new int PageSize
        {
            get
            {
                return (int)GetValue(PageSizeProperty);
            }

            set
            {
                SetValue(PageSizeProperty, value);

                //Sync PageSize ComboBox's SelectedItem
                if (_cmbPageSize != null)
                {
                    foreach (var item in _cmbPageSize.Items)
                    {
                        var ele = item as ComboBoxItem;
                        if (int.Parse(ele.Tag.ToString()) == value)
                        {
                            _cmbPageSize.SelectedItem = ele;
                            break;
                        }
                    }
                }

                _tbTotalPage.Text = string.Format(Resource.Pager_TotalPage, this.PageCount);
            }
        }

        public int TotalCount
        {
            set
            {
                var source = this.Source as PagedCollectionView;
                if (this._tbTotalCount != null)
                {
                    if (this.OwningGrid.IsTopCountMode && value > 0)
                    {
                        this._tbTotalCount.Text = string.Format(Resource.Pager_TotalTopCount, value);
                    }
                    else
                    {
                        this._tbTotalCount.Text = string.Format(Resource.Pager_TotalCount, value);
                    }
                }
                _tbTotalPage.Text = string.Format(Resource.Pager_TotalPage, this.PageCount);
                _txtCurrentPage.Text = (this.PageIndex + 1).ToString(CultureInfo.CurrentCulture);

                this._btnNextPage.IsEnabled = source != null &&
               (this.IsTotalItemCountFixed || source.TotalItemCount == -1 || this.PageIndex < this.PageCount - 1);
            }
        }

        public DataPager()
        {
            DefaultStyleKey = typeof(DataPager);
            this.PageIndexChanged += new System.EventHandler<System.EventArgs>(CustomDataPager_PageIndexChanged);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this._btnPreviousPage = GetTemplateChild(DATAPAGER_elementPreviousButton) as Button;
            (this._btnPreviousPage.Content as TextBlock).Text = Resource.Pager_PreviousPage;
            this._btnPreviousPage.Click += new RoutedEventHandler(_btnPreviousPage_Click);

            this._btnNextPage = GetTemplateChild(DATAPAGER_elementNextPageButton) as Button;
            (this._btnNextPage.Content as TextBlock).Text = Resource.Pager_NextPage;
            this._btnNextPage.Click += new RoutedEventHandler(_btnNextPage_Click);
            this._btnNextPage.IsEnabled = false;

            this._btnGo = GetTemplateChild(DATAPAGER_elementGoButton) as Button;
            this._btnGo.Content = Resource.Pager_Go;
            this._btnGo.Click += new RoutedEventHandler(_btnGo_Click);

            this._firstPageButton = GetTemplateChild(DATAPAGER_elementFirstPageButton) as Button;
            this._firstPageButton.Click += new RoutedEventHandler(_firstPageButton_Click);

            this._lastPageButton = GetTemplateChild(DATAPAGER_elementLastPageButton) as Button;
            this._lastPageButton.Click += new RoutedEventHandler(_lastPageButton_Click);

            this._tbPerPage = GetTemplateChild(DATAPAGER_elementPerPage) as TextBlock;
            this._tbPerPage.Text = Resource.Pager_PerPage;

            this._txtCurrentPage = GetTemplateChild(DATAPAGER_elementNextCurrentPageTextBox) as TextBox;
            this._txtCurrentPage.Text = "0";
            this._txtCurrentPage.KeyDown += new KeyEventHandler(_txtCurrentPage_KeyDown);
            this._tbTotalPage = GetTemplateChild(DATAPAGER_elementTotalPage) as TextBlock;            
            this._tbTotalCount = GetTemplateChild(DATAPAGER_elementTotalCount) as TextBlock;
            this._tbPageInfo = GetTemplateChild(DATAPAGER_elementPageInfo) as TextBlock;
            
            if (this._tbPageInfo != null)
            {
                SetCurrentPage();
            }
            if (_cmbPageSize != null)
            {
                _cmbPageSize.SelectionChanged -= new SelectionChangedEventHandler(_cmbPageSize_SelectionChanged);
            }
            this._cmbPageSize = GetTemplateChild(DATAPAGER_elementPageSize) as ComboBox;

            if (this._cmbPageSize != null)
            {
                foreach (var item in _cmbPageSize.Items)
                {
                    var comboBoxItem = item as ComboBoxItem;
                    if (this.OwningGrid != null && int.Parse(comboBoxItem.Tag.ToString()) == this.OwningGrid.PageSize)
                    {
                        _cmbPageSize.SelectedItem = comboBoxItem;
                    }
                }
                _cmbPageSize.SelectionChanged += new SelectionChangedEventHandler(_cmbPageSize_SelectionChanged);
            }

            this._hlExportExcel = GetTemplateChild(DATAPAGER_elementExportExcel) as HyperlinkButton;
            this._exportBorder = GetTemplateChild(DATAPAGER_elementExportBorder) as Border;
            if (this._hlExportExcel != null)
            {
                var block = _hlExportExcel.FindName(DATAPAGER_elementExportTitle) as TextBlock;
                block.Text = Resource.Pager_Export;
                this._hlExportExcel.Click += new RoutedEventHandler(_hlExportExcel_Click);
            }
            if (this._hlExportAll != null)
            {
                this._hlExportAll.Click -= new RoutedEventHandler(_hlExportAll_Click);
            }
            this._hlExportAll = GetTemplateChild(DATAPAGER_elementExportAll) as HyperlinkButton;
            if (this._hlExportAll != null)
            {
                var tb = _hlExportAll.FindName(DATAPAGER_elementExportAllTitle) as TextBlock;
                tb.Text = Resource.Pager_ExportAll;
                this._hlExportAll.Click += new RoutedEventHandler(_hlExportAll_Click);
            }

            this.SetExportButtonState(false);

            this.TotalCount = 0;

            if (this.OwningGrid != null)
            {
                this.OwningGrid.ExportAllDataCompleted -= new EventHandler<ExportAllDataCompletedEventArgs>(OwningGrid_ExportAllDataCompleted);
                this.OwningGrid.ExportAllDataCompleted += new EventHandler<ExportAllDataCompletedEventArgs>(OwningGrid_ExportAllDataCompleted);
            }
        }

        void _btnNextPage_Click(object sender, RoutedEventArgs e)
        {
            this.OwningGrid.m_tracker.TraceEvent(string.Format("Click:{0}", "Next"), string.Format("Grid:{0}", this.OwningGrid.GetLabel()));
        }

        void _btnPreviousPage_Click(object sender, RoutedEventArgs e)
        {
            this.OwningGrid.m_tracker.TraceEvent(string.Format("Click:{0}", "Previous"), string.Format("Grid:{0}", this.OwningGrid.GetLabel()));
        }

        void _lastPageButton_Click(object sender, RoutedEventArgs e)
        {
            this.OwningGrid.m_tracker.TraceEvent(string.Format("Click:{0}", "Last Page"), string.Format("Grid:{0}", this.OwningGrid.GetLabel()));
        }

        void _firstPageButton_Click(object sender, RoutedEventArgs e)
        {
            this.OwningGrid.m_tracker.TraceEvent(string.Format("Click:{0}", "First Page"), string.Format("Grid:{0}", this.OwningGrid.GetLabel()));
        }

        void _btnGo_Click(object sender, RoutedEventArgs e)
        {
            this.OwningGrid.m_tracker.TraceEvent(string.Format("Click:{0}", "Go"), string.Format("Grid:{0}", this.OwningGrid.GetLabel()));

            MoveCurrentPageToTextboxValue();
        }


        void _txtCurrentPage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MoveCurrentPageToTextboxValue();
                e.Handled = true;
            }
        }

        internal void SetExportButtonState(bool isEnabled)
        {
            if (this._hlExportAll != null)
            {
                this._hlExportAll.IsEnabled = isEnabled;
            }
            if (this._hlExportExcel != null)
            {
                this._hlExportExcel.IsEnabled = isEnabled;
            }
        }

        void _hlExportExcel_Click(object sender, RoutedEventArgs e)
        {
            if (this.OwningGrid != null)
            {
                Exporter.ExportDataGrid(this.OwningGrid);                
            }
        }

        internal void _hlExportAll_Click(object sender, RoutedEventArgs e)
        {
            this._dialog = new SaveFileDialog()
            {
                DefaultExt = "zip",
                Filter = "zip (*.zip)|*.zip",
                FilterIndex = 1
            };
            if (_dialog.ShowDialog() == true)
            {
                this.OwningGrid.OnExportAllData();
            }
        }

        void _cmbPageSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.Source != null && this.ItemCount > 0)
            {
                var combo = sender as ComboBox;
                var comboBoxItem = combo.SelectedItem as ComboBoxItem;
                if (comboBoxItem != null)
                {
                    var pageSize = int.Parse(comboBoxItem.Tag.ToString());

                    if (pageSize != Default_PageSize)
                    {
                        SavePageSize(pageSize);
                    }                    
                                        
                    if (this.PageIndex == 0)
                    {
                        this.PageSize = pageSize;
                        //this.OwningGrid.Bind();
                    }
                    else
                    {
                        this.PageSize = pageSize;
                    }
                }
            }
        }

        void CustomDataPager_PageIndexChanged(object sender, System.EventArgs e)
        {
            this._txtCurrentPage.Text = (base.PageIndex + 1).ToString();
            if (_btnNextPage != null)
            {
                this._btnNextPage.IsEnabled = !(this.PageIndex == this.PageCount - 1);
            }                 
        }              

        void SetCurrentPage()
        {
            this._tbPageInfo.Text = string.Format(Resource.Pager_Pages, this.PageIndex + 1, this.PageCount);
        }

        void SavePageSize(int pageSize)
        {            
            var allSettings = this.OwningGrid.AllGridSettings;
            allSettings.ForEach(p =>
            {
                if (p.GridGuid == this.OwningGrid.GridID)
                {
                    p.PageSize = pageSize;
                }
            });           

            this.OwningGrid.m_userProfile.Set(GridKeys.KEY_UP_GRIDSETTINGS, allSettings, true);
        }

        void OwningGrid_ExportAllDataCompleted(object sender, ExportAllDataCompletedEventArgs e)
        {
            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                this._saveFileStream = this._dialog.OpenFile();
                using (this._saveFileStream)
                {
                    Exporter.ExportAllData(e.Buffer, this._saveFileStream);
                }
            });
        } 

        //Added by Hax at 2011
        /// <summary>
        /// Attempts to move the current page index to the value
        /// in the current page textbox.
        /// </summary>
        private void MoveCurrentPageToTextboxValue()
        {
            if (this._txtCurrentPage.Text != (this.PageIndex + 1).ToString(CultureInfo.CurrentCulture))
            {
                if (this.Source != null && this.TryParseTextBoxPage())
                {
                    this.MoveToRequestedPage();
                }
                this._txtCurrentPage.Text = (this.PageIndex + 1).ToString(CultureInfo.CurrentCulture);
            }
        }

        /// <summary>
        /// Given the new value of _requestedPageIndex, this method will attempt a page move 
        /// and set the _currentPageIndex variable accordingly.
        /// </summary>
        private void MoveToRequestedPage()
        {
            if (this._requestedPageIndex >= 0 && this._requestedPageIndex < this.PageCount)
            {
                // Requested page is within the known range
                this.PageIndex = this._requestedPageIndex;
            }
            else if (this._requestedPageIndex >= this.PageCount)
            {
                if (this.IsTotalItemCountFixed && this.ItemCount != -1)
                {
                    this.PageIndex = this.PageCount - 1;
                }
                else
                {
                    this.PageIndex = this._requestedPageIndex;
                }
            }
        }

        private bool TryParseTextBoxPage()
        {
            // 

            bool successfullyParsed = int.TryParse(this._txtCurrentPage.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out this._requestedPageIndex);

            if (successfullyParsed)
            {
                // Subtract one to make it zero-based.
                this._requestedPageIndex--;
            }

            return successfullyParsed;
        }
    }
}
