
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.IM.Models
{
    public interface ICategroyKPI
    {
        /// <summary>
        /// 属性
        /// </summary>
        int SysNo { get; set; }

        /// <summary>
        /// 保存
        /// </summary>
        void Save();

        /// <summary>
        /// 绑定
        /// </summary>
        /// <param name="data"></param>
        void Bind(ModelBase data);
    }
}
