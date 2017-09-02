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

namespace Newegg.Oversea.Silverlight.Controls.Primitives
{
    public class ListBoxItemExt : ListBoxItem
    {
        private string m_currentState;
        private Image _deleteButton;
        private Image _addButton;
        private ExtType m_extType = ExtType.Remove;
        public ExtType Type
        {
            get
            {
                return m_extType;
            }
            set
            {
                m_extType = value;
            }
        }

        public event EventHandler RemoveItem;
        public event EventHandler AddItem;
        private const string ListBoxItemExt_elementAddImage = "ImageAdd";
        private const string ListBoxItemExt_elementDeleteImage = "ImageRemove";

        public double ColWidth
        {
            get { return (double)GetValue(ColWidthProperty); }
            set { SetValue(ColWidthProperty, value); }
        }

        public int ColIndex { get; set; }
       
        public static readonly DependencyProperty ColWidthProperty =
            DependencyProperty.Register("ColWidth", typeof(double), typeof(ListBoxItemExt), null);

        public ListBoxItemExt()
        {
            DefaultStyleKey = typeof(ListBoxItemExt);
        }

        public ListBoxItemExt(ExtType type)
            : this()
        {
            this.m_extType = type;           
        }

        public override void OnApplyTemplate()
        {
            if (this.Type == ExtType.Add)
            {
                this._addButton = GetTemplateChild(ListBoxItemExt_elementAddImage) as Image;
            }
            else
            {
                this._deleteButton = GetTemplateChild(ListBoxItemExt_elementDeleteImage) as Image;
            }
            if (_deleteButton != null)
            {                
                _deleteButton.MouseLeftButtonDown += new MouseButtonEventHandler(_deleteButton_MouseLeftButtonDown);
            }
            if (_addButton != null)
            {              
                _addButton.MouseLeftButtonDown += new MouseButtonEventHandler(_addButton_MouseLeftButtonDown);
            }
            base.OnApplyTemplate();
           
            this.MouseEnter += new MouseEventHandler(ListBoxItemExt_MouseEnter);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(ListBoxItemExt_MouseLeftButtonUp);
        }        
       
        void _addButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.AddItem != null)
            {
                AddItem(this, null);
            }
            e.Handled = true;
        }

        void _deleteButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.RemoveItem != null)
            {
                RemoveItem(this, null);
            }
            e.Handled = true;
        }

        void ListBoxItemExt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            switch (this.m_extType)
            {
                case ExtType.Add:
                    m_currentState = "SelectedAdd";
                    break;
                case ExtType.Remove:
                    m_currentState = "SelectedRemoved";
                    break;
                default:
                    m_currentState = "Normal";
                    break;
            }
            ChangeState();
            e.Handled = true;
        }

        void ListBoxItemExt_MouseEnter(object sender, MouseEventArgs e)
        {
            switch (this.m_extType)
            {
                case ExtType.Add:
                    m_currentState = "MouseOverAdd";
                    break;
                case ExtType.Remove:
                    m_currentState = "MouseOverRemove";
                    break;
                default:
                    m_currentState = "Normal";
                    break;
            }
            ChangeState();            
        }       

        void ChangeState()
        {
            VisualStateManager.GoToState(this, m_currentState, true);
        }
    }

    public enum ExtType
    {
        Add,
        Remove
    }    
}
