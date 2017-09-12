using System;
using System.Net;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ECCentral.BizEntity.Customer.Society
{
    /// <summary>
    /// 社团信息
    /// </summary>
    [Serializable]
    [DataContract]

    public class SocietyInfo
    {
        /// <summary>
        /// 社团编号
        /// </summary>
        [DataMember]
        public int SysNo { get; set; }

        /// <summary>
        /// 社团名字
        /// </summary>
        [DataMember]
        public string SocietyName { get; set; }
    }
}
