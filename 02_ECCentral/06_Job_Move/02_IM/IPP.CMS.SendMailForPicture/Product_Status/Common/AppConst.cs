using System;

namespace IPP.ContentMgmt.Product_Status.Common
{
    public class AppConst
    {
        #region 系统中判断未赋值的判断，只可以用于比较判断，不能用于赋值

        public const string StringNull = "";

        public const int IntNull = -999999;
        public const decimal DecimalNull = -999999;
        public const float FloatNull = -999999;  //Add By Teracy
        public const float DoubleNull = -999999;  //Add By Teracy 


        public static DateTime DateTimeNull = DateTime.Parse("1900-01-01");

        #endregion

    }
}
