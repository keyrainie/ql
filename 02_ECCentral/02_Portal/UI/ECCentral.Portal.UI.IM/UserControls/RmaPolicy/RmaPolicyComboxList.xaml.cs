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
using ECCentral.BizEntity.IM;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls.Containers;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System.Threading;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class RmaPolicyComboxList : UserControl
    {
        #region private
        private RmaPolicyFacade facade;
        #endregion

        /// <summary>
        /// 公布select事件
        /// </summary>
        public event EventHandler RmaSelectChange;
        #region Property
        public RmaPolicyComBoxVM VM { get; set; }
        public Boolean IsEdit { get; set; }
        public int? SelectValue { get; set; }
        public RmaPolicyVM SelectRmaPolicy { get; set; }
        protected List<RmaPolicyVM> RmaPolicyVMS { get; set; }
        #endregion

        #region Method
        public RmaPolicyComboxList()
        {
            InitializeComponent();
            this.Loaded += RmaPolicyComboxList_Loaded;
        }


        public void BingSelectValue(int? selectValue)
        {
            SelectValue = selectValue;
            BindPage();
        }
        public void RmaPolicyComboxList_Loaded(object sender, RoutedEventArgs e)
        {
            facade = new RmaPolicyFacade(CPApplication.Current.CurrentPage);
            BindPage();

        }
        private void BindPage()
        {
            if (RmaPolicyVMS != null)
            {
                SetDataContext(RmaPolicyVMS);
            }
            else
            {
                facade.GetRmaPolicyComboxList((obj, arg) =>
                {
                    RmaPolicyVMS = new List<RmaPolicyVM>();
                    if (arg.FaultsHandle()) return;
                    arg.Result.ForEach(s =>
                    {
                        RmaPolicyVMS.Add(new RmaPolicyVM()
                        {
                            SysNo = s.SysNo,
                            Status = s.Status,
                            RMAPolicyName = s.RMAPolicyName,
                            IsRequest = s.IsOnlineRequest == IsOnlineRequst.YES,
                            ChangeDate = Convert.ToString(s.ChangeDate),
                            ReturnDate = Convert.ToString(s.ReturnDate),
                            RmaType = s.RmaType,
                            IsDisPlay = Visibility.Visible
                        });
                    });
                    SetDataContext(RmaPolicyVMS);
                });
            }
        }
        

        private void cbRma_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            SelectRmaPolicy = this.cbRma.SelectedValue as RmaPolicyVM;
            if (SelectRmaPolicy != null)
                SelectValue = SelectRmaPolicy.SysNo;
            if (RmaSelectChange != null)
                RmaSelectChange(sender, e);
        }
        private void SetDataContext(List<RmaPolicyVM> RmaPolicyVMs)
        {
            this.DataContext = null;
            VM = new RmaPolicyComBoxVM();
            VM.Data = RmaPolicyVMS;
            if (VM.Data.Where(p=>p.SysNo==0).Count()==0)
            {
                VM.Data.Insert(0, new RmaPolicyVM()
                {
                    RMAPolicyName = IsEdit ? "---请选择---" : "---所有---",
                    SysNo = 0,
                    IsDisPlay = Visibility.Collapsed
                });
            }           
            VM.RmaPolicy = IsEdit
                ? SelectValue != null
                        ? (from p in VM.Data where p.SysNo == SelectValue select p).First()
                        : (from p in VM.Data where p.SysNo == 0 select p).First()
                : (from p in VM.Data where p.SysNo == 0 select p).First();
            this.DataContext = VM;
        }
        #endregion
    }
}
