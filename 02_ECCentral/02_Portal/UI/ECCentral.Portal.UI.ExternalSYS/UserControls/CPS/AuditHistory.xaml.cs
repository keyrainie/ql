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
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.ExternalSYS.Facades;
using ECCentral.Portal.UI.ExternalSYS.Models;

namespace ECCentral.Portal.UI.ExternalSYS.UserControls
{

    public partial class AuditHistory : UserControl
    {
        public IDialog Dialog { get; set; }
        public int SysNo { get; set; }
        private CpsUserFacade facade;
        private AuditHistoryVM model;
        public AuditHistory()
        {
            InitializeComponent();
            this.Loaded += (sender, e) => 
            {
                facade = new CpsUserFacade();
                model = new AuditHistoryVM();
                facade.GetAuditHistory(SysNo, (obj, arg) =>
                    {
                        if (arg.FaultsHandle())
                        {

                            return;
                        }
                        foreach (var item in arg.Result.Rows)
                        {
                            model.Note = item.Note;
                            model.OpertionUser=item.InUser;
                            model.OpertionDate = item.InDate;
                        }
                        this.DataContext = model;
                    });
                
            };
        }
    }
}
