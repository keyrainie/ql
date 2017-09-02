using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nesoft.Utility.DataAccess.RealTime
{
    public class RealTimeData<T> where T : class
    {
        /// <summary>
        /// 主键，如订单信息的SO#
        /// </summary>
        public int BusinessKey { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>
        public int CustomerSysNo { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public BusinessDataType BusinessDataType { get; set; }

        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 详细信息，为XML Message
        /// </summary>        
        public T Body { get; set; }

        public ChangeType ChangeType { get; set; }
    }

    /// <summary>
    /// 数据类型
    /// </summary>
    public enum BusinessDataType
    {
        SO, 
        RMA,        
        CustomerMaster
    }

    public enum ChangeType
    {
        Add,
        Update,
        Delete
    }

    public interface IRealTimePersister
    {
        void Persiste<T>(RealTimeData<T> data) where T:class;
    }

    public interface IRealTimeExtensionPersister
    {
        void Persiste<T>(T t) where T : class;
    }
}
