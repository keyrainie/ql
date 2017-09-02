using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetEPortSOJob.BusinessEntities
{
    /// <summary>
    /// 返回信息
    /// </summary>
    public class Message
    {
        public Header header { get; set; }
        public Body body { get; set; }
    }

    public class Header
    {
        /// <summary>
        /// T：操作成功；F：操作失败
        /// </summary>
        public string Result { get; set; }
        /// <summary>
        /// 结果描述（操作失败时必需）
        /// </summary>
        public string ResultMsg { get; set; }
        /// <summary>
        /// 是否存在下一页（T:是；F：否）
        /// </summary>
        public string NextPage { get; set; }
    }
    public class Body
    {
        public List<Mft> MftList { get; set; }
    }
    
}
