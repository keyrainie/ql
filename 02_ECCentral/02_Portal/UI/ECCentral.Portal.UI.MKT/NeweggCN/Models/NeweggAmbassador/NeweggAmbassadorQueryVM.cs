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
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.MKT;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Enum.Resources;


namespace ECCentral.Portal.UI.MKT.Models
{
    public class NeweggAmbassadorQueryVM : ModelBase
    {
        public NeweggAmbassadorQueryVM()
        {
            this._AmbassadorStatusPairList = EnumConverter.GetKeyValuePairs<AmbassadorStatus>();
            this._AmbassadorStatusPairList.Insert(0, new KeyValuePair<AmbassadorStatus?, string>(null, ResCommonEnum.Enum_All));
            

        }

        private List<KeyValuePair<AmbassadorStatus?, string>> _AmbassadorStatusPairList;

        public List<KeyValuePair<AmbassadorStatus?, string>> AmbassadorStatusPairList
        {
            get
            {
                return _AmbassadorStatusPairList;
            }
        }

        private string _AmbassadorName;

        public string AmbassadorName
        {
            get { return _AmbassadorName; }
            set { base.SetValue("AmbassadorName", ref _AmbassadorName, value); }
        }

        private AmbassadorStatus? _Status;

        public AmbassadorStatus? Status
        {
            get { return _Status; }
            set { base.SetValue("Status", ref _Status, value); }
        }

        private int? _BigAreaSysNo;

        public int? BigAreaSysNo
        {
            get { return _BigAreaSysNo; }
            set { base.SetValue("BigAreaSysNo", ref _BigAreaSysNo, value); }
        }

        private string _AmbassadorID;

        public string AmbassadorID
        {
            get { return _AmbassadorID; }
            set { base.SetValue("AmbassadorID", ref _AmbassadorID, value); }
        }

        private string _AreaSysNo;

        public string AreaSysNo
        {
            get { return _AreaSysNo; }
            set
            {
                base.SetValue("AreaSysNo", ref _AreaSysNo, value);
            }
        }

        private string _provinceSysNo;

        public string ProvinceSysNo
        {
            get { return _provinceSysNo; }
            set
            {
                CitySysNo = null;
                AreaSysNo = null;
                base.SetValue("ProvinceSysNo", ref _provinceSysNo, value);
            }
        }

        private string _citySysNo;

        public string CitySysNo
        {
            get { return _citySysNo; }
            set
            {
                AreaSysNo = null;
                base.SetValue("CitySysNo", ref _citySysNo, value);
            }
        }

        public string GetAreaSysNo()
        {
            if (!string.IsNullOrEmpty(AreaSysNo))
            {
                return AreaSysNo;
            }
            if(!string.IsNullOrEmpty(_citySysNo))
            {
                return _citySysNo;
            }
            return ProvinceSysNo;
        }



    }
}
