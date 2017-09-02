using System;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.BizEntities
{
    public partial class ProfileEntity
    {
        public string ApplicationId
        {
            get
            {
                return (this.ApplicationId2 == null) ? null : this.ApplicationId2.Trim();
            }
            set
            {
                this.ApplicationId2 = (value == null) ? null : value.Trim();
            }
        }

        public string ProfileItemGuid
        {
            get
            {
                return (this.ProfileItemGuid2 == null) ? null : this.ProfileItemGuid2.Trim();
            }
            set
            {
                this.ProfileItemGuid2 = (value == null) ? null : value.Trim();
            }
        }
    }

    public class ProfileQueryEntity : QueryEntityBase
    {
        public string ApplicationId { get; set; }

        public string ProfileType { get; set; }

        public string ProfileItemGuid { get; set; }
 
        public string InUser { get; set; }
   }

    //public enum ProfileType
    //{
    //    /// <summary>
    //    /// Unknown
    //    /// </summary>
    //    Unknown,
    //    /// <summary>
    //    /// FavoriteLink
    //    /// </summary>
    //    FavoriteLink,
    //    /// <summary>
    //    /// GridSetting
    //    /// </summary>
    //    GridSetting,
    //    /// <summary>
    //    /// QuickLinks
    //    /// </summary>
    //    QuickLinks
    //}

    //public static class EntityHelper
    //{
    //    public const char ProfileType_FavoriteLink = 'F';
    //    public const char ProfileType_WidgetSetting = 'W';
    //    public const char ProfileType_GridSetting = 'G';
    //    public const char ProfileType_SearchCondition = 'S';
    //    public const char ProfileType_Unknown = 'U';

    //    public static char ToCode(this ProfileType type)
    //    {
    //        char code;

    //        switch (type)
    //        {
    //            case ProfileType.FavoriteLink:
    //                code = ProfileType_FavoriteLink;
    //                break;
    //            case ProfileType.WidgetSetting:
    //                code = ProfileType_WidgetSetting;
    //                break;
    //            case ProfileType.GridSetting:
    //                code = ProfileType_GridSetting;
    //                break;
    //            case ProfileType.SearchCondition:
    //                code = ProfileType_SearchCondition;
    //                break;
    //            case ProfileType.Unknown:
    //            default:
    //                code = ProfileType_Unknown;
    //                break;
    //        }

    //        return code;
    //    }

    //    public static ProfileType ToProfileType(this char code)
    //    {
    //        ProfileType type;

    //        switch (code)
    //        {
    //            case ProfileType_FavoriteLink:
    //                type = ProfileType.FavoriteLink;
    //                break;
    //            case ProfileType_WidgetSetting:
    //                type = ProfileType.WidgetSetting;
    //                break;
    //            case ProfileType_GridSetting:
    //                type = ProfileType.GridSetting;
    //                break;
    //            case ProfileType_SearchCondition:
    //                type = ProfileType.SearchCondition;
    //                break;
    //            case ProfileType_Unknown:
    //            default:
    //                type = ProfileType.Unknown;
    //                break;
    //        }

    //        return type;
    //    }
    //}
}
