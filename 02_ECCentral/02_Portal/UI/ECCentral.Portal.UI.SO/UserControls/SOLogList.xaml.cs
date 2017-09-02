using System.Windows;
using System.Windows.Controls;

using ECCentral.Portal.UI.SO.Facades;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.SO;

using Newegg.Oversea.Silverlight.Controls.Data;
using System.Xml.Linq;
using System.Linq;

namespace ECCentral.Portal.UI.SO.UserControls
{
    public partial class SOLogList : UserControl
    {
        public int SOSysNo { get; set; }

        public SOLogList()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(SOLogList_Loaded);
        }

        void SOLogList_Loaded(object sender, RoutedEventArgs e)
        {
            Bind();
        }

        private void Bind()
        {
            dataGridSOLog.Bind();
        }

        private void dataGridSOLog_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            SOLogQueryFilter query = new SOLogQueryFilter();
            query.SOSysNo = SOSysNo;
            query.PagingInfo = new PagingInfo()
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };
            SOQueryFacade facade = new SOQueryFacade();
            facade.QuerySOSystemLog(query, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                dataGridSOLog.TotalCount = args.Result.TotalCount;

                var list = args.Result.Rows;
                foreach (var item in list)
                {
                    //note需要特殊处理
                    item["Note"] = GetSOLogNote((string)item["Note"]);
                }
                dataGridSOLog.ItemsSource = list;

            });
        }

        public static string GetSOLogNote(string xmlNote)
        {
            try
            {
                XDocument xmlDoc = XDocument.Parse(xmlNote);
                var actionNameNotes = xmlDoc.Descendants("ActionName");
                return actionNameNotes.First().Value;
            }
            catch
            {
                //简单处理屏蔽异常
            }
            return xmlNote;
        }
    }
}
