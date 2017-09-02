using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.SOPipeline
{
    public abstract class ExtensibleObject :  ICloneable
    {
        private Dictionary<string, object> m_Properties = new Dictionary<string, object>();

        public object this[string propertyName]
        {
            get
            {
                object val;
                if (m_Properties.TryGetValue(propertyName, out val))
                {
                    return val;
                }
                return null;
            }
            set
            {
                if (m_Properties.ContainsKey(propertyName))
                {
                    m_Properties[propertyName] = value;
                }
                else
                {
                    m_Properties.Add(propertyName, value);
                }
            }
        }

        public bool HasProperty(string propertyName)
        {
            return m_Properties.ContainsKey(propertyName);
        }

        public object GetPropertyValue(string propertyName, bool throwIfNotHasTheProperty = false)
        {
            object val;
            if (m_Properties.TryGetValue(propertyName, out val))
            {
                return val;
            }
            if (throwIfNotHasTheProperty)
            {
                throw new KeyNotFoundException("Not found property with the name '" + propertyName + "'.");
            }
            return null;
        }

        public T GetPropertyValue<T>(string propertyName, bool throwIfNotHasTheProperty = false)
        {
            object val;
            if (m_Properties.TryGetValue(propertyName, out val))
            {
                return (T)val;
            }
            if (throwIfNotHasTheProperty)
            {
                throw new KeyNotFoundException("Not found property with the name '" + propertyName + "'.");
            }
            return default(T);
        }

        public bool TryGetPropertyValue(string propertyName, out object value)
        {
            return m_Properties.TryGetValue(propertyName, out value);
        }

        public bool TryGetPropertyValue<T>(string propertyName, out T value)
        {
            object val;
            bool has = m_Properties.TryGetValue(propertyName, out val);
            if (has)
            {
                value = (T)val;
            }
            else
            {
                value = default(T);
            }
            return has;
        }

        //public IEnumerator<ExProperty> GetEnumerator()
        //{
        //    return new PropertyEnumerator(m_Properties.GetEnumerator());
        //}

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return new PropertyEnumerator(m_Properties.GetEnumerator());
        //}

        //private struct PropertyEnumerator : IEnumerator<ExProperty>, IDisposable, IEnumerator
        //{
        //    private Dictionary<string, object>.Enumerator m_DicEnumerator;
        //    public PropertyEnumerator(Dictionary<string, object>.Enumerator dicEnumerator)
        //    {
        //        m_DicEnumerator = dicEnumerator;
        //    }

        //    public ExProperty Current
        //    {
        //        get
        //        {
        //            var c = m_DicEnumerator.Current;
        //            if (c.Key == null)
        //            {
        //                return null;
        //            }
        //            return new ExProperty(c.Key, c.Value);
        //        }
        //    }

            //public void Dispose()
            //{
            //    m_DicEnumerator.Dispose();
            //}

            //object IEnumerator.Current
            //{
            //    get { return this.Current; }
            //}

            //public bool MoveNext()
            //{
            //    return m_DicEnumerator.MoveNext();
            //}

            //public void Reset()
            //{
            //    ((IEnumerator)m_DicEnumerator).Reset();
            //}
        //}

        public object Clone()
        {
            ExtensibleObject obj = CloneObject();
            obj.m_Properties = new Dictionary<string, object>(this.m_Properties.Count * 2);
            foreach (var entry in this.m_Properties)
            {
                object tmp = (entry.Value is ICloneable) ? ((ICloneable)entry.Value).Clone() : entry.Value;
                obj.m_Properties.Add(entry.Key, tmp);
            }
            return obj;
        }

        public abstract ExtensibleObject CloneObject();
    }

    // 表示一个自定义扩展属性
    public class ExProperty
    {
        internal ExProperty(string name, object value)
        {
            Name = name;
            Value = value;
        }

        // 属性名称
        public string Name { get; private set; }

        // 属性的值
        public object Value { get; private set; }
    }
}
