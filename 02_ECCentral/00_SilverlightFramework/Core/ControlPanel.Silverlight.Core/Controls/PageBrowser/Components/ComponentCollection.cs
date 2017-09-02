using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Newegg.Oversea.Silverlight.Controls.Components
{
    public class ComponentCollection : ObservableCollection<IComponent>
    {
        public ComponentCollection()
            : base()
        { }

        protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems.Count > 0 && e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                List<IComponent> removeList = new List<IComponent>();
                ComponentItem item;
                foreach (IComponent component in e.NewItems)
                {
                    item = component as ComponentItem;
                    if (item != null)
                    {
                        if (item.Component == null)
                        {
                            removeList.Add(item);
                        }
                        else
                        {
                            this.SetItem(this.IndexOf(item), item.Component);
                        }
                    }
                }

                foreach (IComponent component in removeList)
                {
                    this.Remove(component);
                }

                removeList.Clear();
            }
            base.OnCollectionChanged(e);
        }
    }

    public class ComponentItem : Control, IComponent
    {
        public static readonly DependencyProperty ComponentProperty;

        static ComponentItem()
        {
            ComponentProperty = DependencyProperty.Register("Component", typeof(IComponent), typeof(ComponentItem), null);
        }

        public ComponentItem()
            : base()
        { }

        [System.ComponentModel.TypeConverter(typeof(StringToComponentConverter))]
        public IComponent Component
        {
            get
            {
                object value = this.GetValue(ComponentProperty);

                if (value != null)
                {
                    return value as IComponent;
                }

                return null;
            }

            set
            {
                this.SetValue(ComponentProperty, value);
            }
        }

        #region IComponent Members

        string IComponent.Name
        {
            get { return null; }
        }

        string IComponent.Version
        {
            get { return null; }
        }

        void IComponent.InitializeComponent(IPageBrowser browser)
        {
        }

        public object GetInstance(TabItem tab)
        {
            throw new NotImplementedException();
        }



        public void Dispose()
        {

        }
        #endregion
    }

    public class StringToComponentConverter : System.ComponentModel.TypeConverter
    {
        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType.IsAssignableFrom(typeof(string));
        }

        public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            string text = value as string;
            if (text != null)
            {
                Type type = Type.GetType(text);

                if (type != null)
                {
                    return System.Activator.CreateInstance(type);
                }
            }
            return null;
        }
    }
}
