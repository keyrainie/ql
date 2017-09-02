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
using ECCentral.Portal.UI.Customer.NeweggCN.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System.Collections.Generic;
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.Customer.Models
{
    public class QCSubjectQueryVM : ModelBase
    {
        public QCSubjectQueryVM()
        {
            this.QCSubjectStatusList = EnumConverter.GetKeyValuePairs<QCSubjectStatus>(EnumConverter.EnumAppendItemType.Select);
            this.ParentList = new List<QCSubjectVM>();
        }
        public List<KeyValuePair<QCSubjectStatus?, string>> QCSubjectStatusList { get; set; }

        private QCSubjectVM _QCSubject;

        public QCSubjectVM QCSubject
        {
            get { return _QCSubject; }
            set { base.SetValue("QCSubject", ref _QCSubject, value); }
        }

        private List<QCSubjectVM> parentList;

        public List<QCSubjectVM> ParentList
        {
            get { return parentList; }
            set { base.SetValue("ParentList", ref parentList, value); }
        }

    }
}
