using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.UI.MKT.Resources;
using ECCentral.QueryFilter.Common;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System.Collections.ObjectModel;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class GroupBuyingCategoryQuery : PageBase
    {
        public GroupBuyingCategoryVM VM
        {
            get
            {
                return this.GridMaintain.DataContext as GroupBuyingCategoryVM;
            }
            set
            {
                this.GridMaintain.DataContext = value;
            }
        }

        private GroupBuyingFacade facade;       

        public GroupBuyingCategoryQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            this.VM = new GroupBuyingCategoryVM();
            this.facade = new GroupBuyingFacade(this);
            facade.GetAllGroupBuyingCategory((s, a) =>
            {
                if (a.FaultsHandle())
                {
                    return;
                }
                this.DataGrid.ItemsSource = new ObservableCollection<GroupBuyingCategoryInfo>(a.Result);
            });
        }
       
        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var source = this.DataGrid.ItemsSource as ObservableCollection<GroupBuyingCategoryInfo>;
            if (ValidationManager.Validate(this.GridMaintain))
            {
                if ((this.VM.SysNo ?? 0) > 0)
                {
                    facade.UpdateGroupBuyingCategory(this.VM, (o, a) =>
                    {
                        if (a.FaultsHandle())
                        {
                            return;
                        }

                        var item = source.FirstOrDefault(p => p.SysNo == VM.SysNo);
                        if (item != null)
                        {
                            int index = source.IndexOf(item);
                            source.RemoveAt(index);
                            source.Insert(index, a.Result);
                        }
                        //this.Window.Alert("保存成功！");                        
                        this.Window.Alert(ResGroupBuyingCategoryQuery.Info_SaveSuccess);                        
                    });
                }
                else
                {
                    facade.CreateGroupBuyingCategory(this.VM, (o, a) =>
                    {
                        if (a.FaultsHandle())
                        {
                            return;
                        }                        
                        if (source != null)
                        {
                            source.Add(a.Result);
                        }                        
                        //this.VM.SysNo = a.Result.SysNo;
                        this.VM = new GroupBuyingCategoryVM();
                        //this.Window.Alert("保存成功！");
                        this.Window.Alert(ResGroupBuyingCategoryQuery.Info_SaveSuccess);       
                    });
                }
            }
        }

        private void ButtonNew_Click(object sender, RoutedEventArgs e)
        {
            this.VM = new GroupBuyingCategoryVM();
        }

        private void hybtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var link = sender as HyperlinkButton;
            var info = link.DataContext as GroupBuyingCategoryInfo;
            var vm =EntityConverter<GroupBuyingCategoryInfo, GroupBuyingCategoryVM>.Convert(info);
            this.VM = vm;
        }       
    }
}
