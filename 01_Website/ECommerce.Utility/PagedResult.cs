using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Collections;

namespace ECommerce.Utility
{
    [DataContract]
    [Serializable]
    public class PagedResult<T> : IEnumerable<T>, ICollection<T>
    {
        #region Public Static Fields
        /// <summary>
        /// 获取一个当前类型的空值。
        /// </summary>
        public static readonly PagedResult<T> Empty = new PagedResult<T>(0, 0, 0, new List<T>(0));
        #endregion

        #region Ctor
        /// <summary>
        /// 初始化一个新的<c>PagedResult{T}</c>类型的实例。
        /// </summary>
        public PagedResult()
        {
            CurrentPageData = new List<T>();
        }
        /// <summary>
        /// 初始化一个新的<c>PagedResult{T}</c>类型的实例。
        /// </summary>
        /// <param name="totalRecords">总记录数。</param>
        /// <param name="pageSize">页面大小。</param>
        /// <param name="pageNumber">页码。</param>
        /// <param name="data">当前页面的数据。</param>
        public PagedResult(int totalRecords, int pageSize, int pageNumber, List<T> data)
        {
            this.TotalRecords = totalRecords;
            this.PageSize = pageSize;
            this.PageNumber = pageNumber;
            this.CurrentPageData = data;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// 获取或设置总记录数。
        /// </summary>
        [DataMember]
        public int TotalRecords { get; set; }
        /// <summary>
        /// 获取或设置页面大小。
        /// </summary>
        [DataMember]
        public int PageSize { get; set; }
        /// <summary>
        /// 获取或设置页码。
        /// </summary>
        [DataMember]
        public int PageNumber { get; set; }
        /// <summary>
        /// 获取或设置当前页面的数据。
        /// </summary>
        [DataMember]
        public List<T> CurrentPageData { get; set; }
        /// <summary>
        /// 获取页数。
        /// </summary>
        public int TotalPages { get { return (PageSize == 0 || TotalRecords == 0) ? 0 : (TotalRecords / PageSize + (TotalRecords % PageSize == 0 ? 0 : 1)); } }
        #endregion

        #region Public Methods
        /// <summary>
        /// 获取或设置指定当前集合中序号位置的元素
        /// </summary>
        /// <param name="index">指定的序号</param>
        /// <returns>指定当前集合中序号的元素</returns>
        public T this[int index]
        {
            get { return CurrentPageData[index]; }
            set { CurrentPageData[index] = value; }
        }
        /// <summary>
        /// 确定指定的Object是否等于当前的Object。
        /// </summary>
        /// <param name="obj">要与当前对象进行比较的对象。</param>
        /// <returns>如果指定的Object与当前Object相等，则返回true，否则返回false。</returns>
        /// <remarks>有关此函数的更多信息，请参见：http://msdn.microsoft.com/zh-cn/library/system.object.equals。
        /// </remarks>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
            if (obj == (object)null)
                return false;
            var other = obj as PagedResult<T>;
            if (other == (object)null)
                return false;
            return this.TotalPages == other.TotalPages &&
                this.TotalRecords == other.TotalRecords &&
                this.PageNumber == other.PageNumber &&
                this.PageSize == other.PageSize &&
                this.CurrentPageData == other.CurrentPageData;
        }

        /// <summary>
        /// 用作特定类型的哈希函数。
        /// </summary>
        /// <returns>当前Object的哈希代码。</returns>
        /// <remarks>有关此函数的更多信息，请参见：http://msdn.microsoft.com/zh-cn/library/system.object.gethashcode。
        /// </remarks>
        public override int GetHashCode()
        {
            return this.TotalPages.GetHashCode() ^
                this.TotalRecords.GetHashCode() ^
                this.PageNumber.GetHashCode() ^
                this.PageSize.GetHashCode();
        }

        /// <summary>
        /// 确定两个对象是否相等。
        /// </summary>
        /// <param name="a">待确定的第一个对象。</param>
        /// <param name="b">待确定的另一个对象。</param>
        /// <returns>如果两者相等，则返回true，否则返回false。</returns>
        public static bool operator ==(PagedResult<T> a, PagedResult<T> b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if ((object)a == null || (object)b == null)
                return false;
            return a.Equals(b);
        }

        /// <summary>
        /// 确定两个对象是否不相等。
        /// </summary>
        /// <param name="a">待确定的第一个对象。</param>
        /// <param name="b">待确定的另一个对象。</param>
        /// <returns>如果两者不相等，则返回true，否则返回false。</returns>
        public static bool operator !=(PagedResult<T> a, PagedResult<T> b)
        {
            return !(a == b);
        }
        #endregion

        #region IEnumerable<T> Members
        /// <summary>
        /// 返回一个循环访问集合的枚举数。
        /// </summary>
        /// <returns>一个可用于循环访问集合的 IEnumerator 对象。</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return CurrentPageData.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members
        /// <summary>
        /// 返回一个循环访问集合的枚举数。 （继承自 IEnumerable。）
        /// </summary>
        /// <returns>一个可用于循环访问集合的 IEnumerator 对象。</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return CurrentPageData.GetEnumerator();
        }

        #endregion

        #region ICollection<T> Members
        /// <summary>
        /// 将某项添加到 ICollection{T} 中。
        /// </summary>
        /// <param name="item">要添加到 ICollection{T} 的对象。</param>
        public void Add(T item)
        {
            CurrentPageData.Add(item);
        }

        /// <summary>
        /// 从 ICollection{T} 中移除所有项。
        /// </summary>
        public void Clear()
        {
            CurrentPageData.Clear();
        }

        /// <summary>
        /// 确定 ICollection{T} 是否包含特定值。
        /// </summary>
        /// <param name="item">要在 ICollection{T} 中定位的对象。</param>
        /// <returns>如果在 ICollection{T} 中找到 item，则为 true；否则为 false。</returns>
        public bool Contains(T item)
        {
            return CurrentPageData.Contains(item);
        }

        /// <summary>
        /// 从特定的 Array 索引开始，将 ICollection{T} 的元素复制到一个 Array 中。
        /// </summary>
        /// <param name="array">作为从 ICollection{T} 复制的元素的目标的一维 Array。 Array 必须具有从零开始的索引。</param>
        /// <param name="arrayIndex">array 中从零开始的索引，从此索引处开始进行复制。</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            CurrentPageData.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 获取 ICollection{T} 中包含的元素数。
        /// </summary>
        public int Count
        {
            get { return CurrentPageData.Count; }
        }

        /// <summary>
        /// 获取一个值，该值指示 ICollection{T} 是否为只读。
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// 从 ICollection{T} 中移除特定对象的第一个匹配项。
        /// </summary>
        /// <param name="item">要从 ICollection{T} 中移除的对象。</param>
        /// <returns>如果已从 ICollection{T} 中成功移除 item，则为 true；否则为 false。 如果在原始 ICollection{T} 中没有找到 item，该方法也会返回 false。 </returns>
        public bool Remove(T item)
        {
            return CurrentPageData.Remove(item);
        }

        #endregion
    }
}
