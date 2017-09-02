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
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.Resources;
using Newegg.Oversea.Silverlight.Utilities;
using System.Collections.Generic;

namespace Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.UserControls
{
    public class QuickLinksModel : INotifyPropertyChanged
    {
        private string _GUID;
        private string _QuickLinkName;
        private string _ViewStatus; 
        private int _OrderIndex; 
        private string _BaseUrl;
        private string _MenuID;
        private object _userState;
        private string _query;

        public  string QueryString
        {
            get
            {
                return _query;
            }
             set
            {
                _query = value;
                NotifyPropertyChanged("QueryString");
            }
        }

        public object UserState
        {
            get
            {
                return _userState;
            }
             set
            {
                _userState = value;
                NotifyPropertyChanged("UserState");
            }
        }


        public string MenuID
        {
            get
            {
                return _MenuID;
            }
            set
            {
                _MenuID = value;
                NotifyPropertyChanged("MenuID");
            }
        }

       private bool _isVisibable=true;

         public bool IsVisibable
        {
            get
            {
                return _isVisibable;
            }
            set
            {
                _isVisibable = value;
                NotifyPropertyChanged("IsVisibable");
            }
        }

       // private Visibility m_splitBarVisibable = Visibility.Collapsed;

        
        public Visibility SplitBarVisibable
        {
            get
            {
                if (IsVisibable)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
               
            }
            //set
            //{
            //    m_splitBarVisibable = value;
            //    NotifyPropertyChanged("SplitBarVisibable");
            //}
        }

        public string GUID
        {
            get
            {
                return _GUID;
            }
            set
            {
                _GUID = value;
                NotifyPropertyChanged("GUID");
            }
        }

       
        public string QuickLinkName 
        {
            get 
            {
                return _QuickLinkName;
            }
            set
            {
                if (value.IsNullOrEmpty())
                {
                    throw new ValidationException(PageResource.LbquicklinkNewnameTitle);
                } 

                _QuickLinkName = value;
                NotifyPropertyChanged("QuickLinkName");
            }
        }
       
        public string ViewStatus 
        {
            get
            {
                return _ViewStatus;
            }
            set
            {
                _ViewStatus = value;
                NotifyPropertyChanged("ViewStatus");
            }
        }

        public int OrderIndex
        {
            get
            {
                return _OrderIndex;
            }
            set
            {
                _OrderIndex = value;
                NotifyPropertyChanged("OrderIndex");
            }
        }

        public string BaseUrl
        {
            get
            {
                return _BaseUrl;
            }
            set
            {
                _BaseUrl = value;
                NotifyPropertyChanged("BaseUrl");
            }
        }

        public bool IsRename
        {
            get;
            set;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        
        #endregion

    }
}
