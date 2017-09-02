using System.ComponentModel;

namespace IPP.OrderMgmt.JobV31.AppEnum
{
    public enum MailSubject:int
    {
        [Description("虚库申请单运行出错")]
        RunError=1,
        [Description("虚库申请单关闭出错")]
        CloseError=2,
        [Description("BaiYin")]
        BaiYin=3,
        [Description("HuangJin")]
        HuangJin=4,
        [Description("ZuanShi")]
        ZuanShi=5,
        [Description("HuangGuan")]
        HuangGuan=6,
        [Description("ZhiZun")]
        ZhiZun=7
    }

 
       
}

