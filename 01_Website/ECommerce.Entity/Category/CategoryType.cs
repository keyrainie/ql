using System;
using System.Runtime.Serialization;

namespace ECommerce.Entity.Category
{
    /// <summary>
    /// Category type
    /// </summary>
    [Serializable]
    [DataContract]
    public enum CategoryType
    {
        [EnumMember]
        TabStore = 1,

        [EnumMember]
        Category = 2,

        [EnumMember]
        SubCategory = 3
    }
}
