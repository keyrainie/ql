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
using System.Windows.Navigation;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.UI.MKT.Facades;
using Newegg.Oversea.Silverlight.Controls.Data;
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Views
{
       [View(IsSingleton = true, SingletonType = SingletonTypes.Url, NeedAccess = false)]
    public partial class PurgeToolManagement : PageBase
    {
           private PurgeToolVM model;
           private PurgeToolFacade facade;
           private ClearType type;
        public PurgeToolManagement()
        {
            InitializeComponent();
            this.PurgeToolResult.LoadingDataSource += new EventHandler<LoadingDataEventArgs>(PurgeToolResult_LoadingDataSource);
            this.Loaded += (sender, e) => 
            {
                facade = new PurgeToolFacade();
                model = new PurgeToolVM();
                this.spInfo.DataContext = model;
            };
          
        }

        void PurgeToolResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            facade.GetPurgeToolByQuery(type,e.PageSize,e.PageIndex,e.SortField,(obj,arg)=>
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                this.PurgeToolResult.ItemsSource = arg.Result.Rows;
                this.PurgeToolResult.TotalCount = arg.Result.TotalCount;
            });
        }

        private void btnWaitClear_Click(object sender, RoutedEventArgs e)
        {
            type = ClearType.WaitClear;
            this.PurgeToolResult.Bind();
        }

        private void btnCompleteClear_Click(object sender, RoutedEventArgs e)
        {
            type = ClearType.CompleteClear;
            this.PurgeToolResult.Bind();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(spInfo))
            {
                return;
            }
            facade.CreatePurgeTool(model,(obj,arg)=>
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                this.PurgeToolResult.Bind();
            });
           
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            model.ClearDate = null;
            model.Priority = string.Empty;
            model.UrlList = string.Empty;
            
        }
     }
}
