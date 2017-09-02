using System.Runtime.Serialization;

namespace ECCentral.BizEntity.IM
{
    public partial class ProductBasicInfo
    {
        /// <summary>
        /// 是否拍照
        /// </summary>
        [DataMember]
        public ProductIsTakePicture IsTakePicture { get; set; }

        /// <summary>
        /// 是否虚库图片
        /// </summary>
        [DataMember]
        public ProductIsVirtualPic IsVirtualPic { get; set; }

        /// <summary>
        /// 配件是否前台显示
        /// </summary>
        [DataMember]
        public ProductIsAccessoryShow IsAccessoryShow { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string Note { get; set; }

        /// <summary>
        /// 规格参数
        /// </summary>
        [DataMember]
        public string Performance { get; set; }

        [DataMember]
        public ProductInfoFinishStatus ProductInfoFinishStatus { get; set; }
    }
}
