using System;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.SO;

namespace ECCentral.Service.SO.BizProcessor
{
    internal static class SOActionFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="soInfo"></param>
        /// <param name="soType"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public static SOAction Create(SOAction.SOCommandInfo command)
        {
            //订单出库目前只处理SellerPortal发来请求
            if (command == null || command.SOInfo == null || !command.SOInfo.BaseInfo.SOType.HasValue)
            {
                BizExceptionHelper.Throw("SO_SOIsNotExist");
            }

            SOType soType = SOType.General;
            soType = command.SOInfo.BaseInfo.SOType.Value;
            //默认都使用 普通订单 的行为方法，如果某些订单类型有特定的行为方法，请在下 switch 代码中设置。

            string filterSOType = SOType.General.ToString();
            switch (command.Command)
            {
                case SOAction.SOCommand.Create:
                    switch (soType)
                    {
                        case SOType.PhysicalCard:
                        case SOType.ElectronicCard:
                        case SOType.Gift:
                            filterSOType = soType.ToString();
                            break;
                    }
                    break;
                case SOAction.SOCommand.Update:
                    switch (soType)
                    {
                        case SOType.PhysicalCard:
                        case SOType.ElectronicCard:
                        case SOType.Gift:
                        case SOType.GroupBuy:
                            filterSOType = soType.ToString();
                            break;
                    }
                    break;
                case SOAction.SOCommand.Audit:
                    switch (soType)
                    {
                        case SOType.PhysicalCard:
                        case SOType.ElectronicCard:
                        case SOType.Gift:
                        case SOType.GroupBuy:
                            filterSOType = command.SOInfo.BaseInfo.SOType.Value.ToString();
                            break;
                    }
                    break;
                case SOAction.SOCommand.Abandon:
                    switch (soType)
                    {
                        case SOType.ElectronicCard:
                        case SOType.PhysicalCard:
                            filterSOType = command.SOInfo.BaseInfo.SOType.Value.ToString();
                            break;
                        case SOType.General:
                            if (command.SOInfo.BaseInfo.SpecialSOType != SpecialSOType.Normal)
                            {
                                filterSOType = ThirdPartSOFilter;
                            }
                            break;
                    }
                    break;
                case SOAction.SOCommand.Job:
                    switch (soType)
                    {
                        case SOType.GroupBuy:
                            filterSOType = soType.ToString();
                            break;
                    }
                    break;
            }

            SOAction action = ObjectFactory<SOAction>.NewInstance(new string[] { filterSOType, command.Command.ToString() });
            action.CurrentSO = command.SOInfo;
            action.Parameter = command.Parameter;
            return action;
        }

        internal const string ThirdPartSOFilter = "ThirdPartSO";
        internal const string UnicomSOFilter = "UnicomSO";
    }
}