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
using ECCentral.Portal.UI.MKT.UserControls;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.QueryFilter.MKT;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.MKT.Resources;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class PollItemGroupMaintain : PageBase
    {
        public int SysNo { get; set; }
        private PollListVM model;
        private PollFacade facade;

        /// <summary>
        /// 投票问题
        /// </summary>
        private PollItemGroupVM groupVM;

        public PollItemGroupMaintain()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            model = new PollListVM();
            facade = new PollFacade(this);
            groupVM = new PollItemGroupVM();

            comStatus.ItemsSource = EnumConverter.GetKeyValuePairs<ADStatus>(EnumConverter.EnumAppendItemType.All);
            List<KeyValuePair<PollType?, string>> types = EnumConverter.GetKeyValuePairs<PollType>();
            types.RemoveAt(3);
            comPollType.ItemsSource = types;

            SysNo = int.Parse(this.Request.Param);
            facade.LoadPollMaster(SysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                model = args.Result.Convert<PollMaster, PollListVM>();
                model.ChannelID = "1";
                PollBaseInfo.DataContext = model;

                this.ucPageType.PageTypeLoadCompleted += new EventHandler(ucPageType_PageTypeLoadCompleted);
                this.ucPageType.PageLoadCompleted += new EventHandler(ucPageType_PageLoadCompleted);

                List<PollItemGroupVM> list = new List<PollItemGroupVM>();
                foreach (PollItemGroup item in model.PollItemGroupList)
                {
                    list.Add(item.Convert<PollItemGroup, PollItemGroupVM>());
                }
                PollGroupInfoGrid.ItemsSource = list;
            });         

            PollGroupInfo.DataContext = groupVM;          
        }

        public void ucPageType_PageLoadCompleted(object sender, EventArgs e)
        {
            ucPageType.SetPageID(model.PageID);
        }

        public void ucPageType_PageTypeLoadCompleted(object sender, EventArgs e)
        {
            ucPageType.SetPageType(model.PageType);
            if (model.PageType.HasValue && model.PageType.Value == 4)//搜索结果页
            {
                comPollType.ItemsSource = EnumConverter.GetKeyValuePairs<PollType>();
            }
            groupVM.Type = PollType.Single;
        }

        private void PollGroupInfoGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            facade.GetPollItemGroupList(SysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                List<PollItemGroupVM> list = new List<PollItemGroupVM>();
                foreach (PollItemGroup item in args.Result)
                {
                    list.Add(item.Convert<PollItemGroup, PollItemGroupVM>());
                }
                PollGroupInfoGrid.ItemsSource = list;
            });
        }


        private void tbnCreateGroupName_Click(object sender, RoutedEventArgs e)
        {

            ValidationManager.Validate(this.PollGroupInfo);
            if (this.groupVM.HasValidationErrors)
            {
                return;
            }

            PollItemGroup item = EntityConvertorExtensions.ConvertVM<PollItemGroupVM, PollItemGroup>(groupVM, (v, t) =>
            {
                t.GroupName = new BizEntity.LanguageContent(ConstValue.BizLanguageCode, v.GroupName);
            });
            item.PollSysNo = SysNo;
            facade.CreatePollItemGroup(item, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;
                PollGroupInfoGrid.Bind();
            });

        }
        

        /// <summary>
        /// 更新单个选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlUpdate_Click(object sender, RoutedEventArgs e)
        {

            PollItemGroupVM vm = this.PollGroupInfoGrid.SelectedItem as PollItemGroupVM;

            ValidationManager.Validate(this.PollGroupInfoGrid);
            if (vm.HasValidationErrors)
            {
                return;
            }

            PollItemGroup item = EntityConvertorExtensions.ConvertVM<PollItemGroupVM, PollItemGroup>(vm, (v, t) =>
            {
                t.GroupName = new BizEntity.LanguageContent(ConstValue.BizLanguageCode, v.GroupName);
            });

            facade.UpdatePollItemGroup(item, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;
                Window.Alert(ResNewsInfo.Information_UpdateSuccessful, MessageType.Information);
                PollGroupInfoGrid.Bind();
            });
        }

        /// <summary>
        /// 删除单个选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlDelete_Click(object sender, RoutedEventArgs e)
        {
            PollItemGroupVM vm = this.PollGroupInfoGrid.SelectedItem as PollItemGroupVM;
            if (vm != null)
            {
                facade.DeletePollItemGroup(vm.SysNo.Value, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    PollGroupInfoGrid.Bind();
                });
            }
        }
        /// <summary>
        /// 创建单个选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlCreateItem_Click(object sender, RoutedEventArgs e)
        {
            PollItemGroupVM vm = this.PollGroupInfoGrid.SelectedItem as PollItemGroupVM;

            UCEditPollOption usercontrol = new UCEditPollOption();
            usercontrol.SysNo = vm.SysNo.Value;
            usercontrol.pollType = vm.Type;
            usercontrol.Dialog = Window.ShowDialog(ResNewsInfo.Title_CreatePollItemOption, usercontrol, (obj, args) =>
            {
            });
        }

    }

}
