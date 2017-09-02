using System.ComponentModel;

namespace ECCentral.BizEntity.IM
{
    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum ProductIsTakePicture
    {
        [Description("拍照")]
        Yes = 'Y',
        [Description("未拍照")]
        No = 'N'
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum ProductIsVirtualPic
    {
        [Description("虚库图片")]
        Yes = 'Y',
        [Description("不是虚库图片")]
        No = 'N'
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum ProductIsAccessoryShow
    {
        [Description("显示")]
        Yes = 'Y',
        [Description("不显示")]
        No = 'N'
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum ProductInfoFinishStatus
    {
        [Description("商品资料已完善")]
        Yes = 'Y',
        [Description("商品资料未完善")]
        No = 'N'
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum ThirdPartner
    {
        [Description("亨众")]
        Hengzhong = 'H',
        [Description("西街")]
        Xijie = 'X',
        [Description("百丽")]
        Belle = 'B',
        [Description("亦可")]
        Yieke = 'Y',
        [Description("英迈")]
        Ingrammicro = 'I'
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum StockRules
    {
        [Description("限制数量上限")]
        Limit = 'L',
        [Description("直接调用合作方库存数值")]
        Direct = 'U',
        [Description("自定义数量")]
        Customer = 'C'
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum ProductMappingStatus
    {
        [Description("有效")]
        Active = 'A',
        [Description("无效")]
        DeActive = 'D'
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum IsUseAlipayVipPrice
    {
        [Description("使用")]
        Yes = 'Y',
        [Description("不使用")]
        No = 'N'
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum IsAutoAdjustPrice
    {
        [Description("自动调价")]
        Yes = 1,
        [Description("不自动调价")]
        No = 0
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum OperatorType
    {
        [Description("<=")]
        LessThanOrEqual=0,
        [Description("=")]
        Equal,
        [Description(">=")]
        MoreThanOrEqual
       
    }
}
