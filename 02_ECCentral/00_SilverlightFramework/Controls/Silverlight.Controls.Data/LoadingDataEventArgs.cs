using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Newegg.Oversea.Silverlight.Controls.Data
{
    public class LoadingDataEventArgs : EventArgs
    {
        public object QueryCriteria { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SortField { get; set; }
        public BindActionType ActionType { get; set; }

        public LoadingDataEventArgs(int pageIndex, int pageSize, string sortField, object queryCriteria)
            : this(pageIndex, pageSize, sortField, queryCriteria, BindActionType.Other)
        {

        }

        public LoadingDataEventArgs(int pageIndex, int pageSize, string sortField, object queryCriteria, BindActionType type)
        {
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
            this.SortField = sortField;
            this.QueryCriteria = queryCriteria;
            this.ActionType = type;
        }

    }

    /// <summary>
    /// 用于区分在调用bind方法是，是由那个action引起
    /// </summary>
    public enum BindActionType
    {
        Sort,
        Paging,
        Other
    }
}
