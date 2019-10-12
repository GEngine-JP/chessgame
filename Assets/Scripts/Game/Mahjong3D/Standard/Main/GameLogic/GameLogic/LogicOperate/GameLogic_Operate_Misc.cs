using System.Collections.Generic;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public partial class GameLogic_Operate : AbsGameLogicBase
    {
        [S2CResponseHandler(NetworkProtocol.MJOpreateType, GameMisc.BbmjKey)]
        public void OnOperate_Bbmj(ISFSObject data)
        {
            if (ParseOperate(data))
            {
                if (data.ContainsKey("youjin"))
                {
                    //隐藏过按钮
                    mOpMenuCache.Add(new KeyValuePair<int, bool>((int)OperateMenuType.OpreateHideGuo, false));
                    //显示游金按钮
                    mOpMenuCache.Add(new KeyValuePair<int, bool>((int)OperateMenuType.OperateYoujin, true));
                    DataCenter.Players[0].SetTinglist(data.TryGetIntArray("youjin"));
                    //禁止出牌
                    Game.MahjongGroups.PlayerToken = false;
                }
                Dispatch();
            }
        }

        [S2CResponseHandler(NetworkProtocol.MJOpreateType, GameMisc.NamjKey)]
        public void OnOperate_Namj(ISFSObject data)
        {
            if (ParseOperate(data))
            {
                if (data.ContainsKey("qiduitings"))
                {
                    var list = DataCenter.OneselfData.TingList;
                    DataCenter.OneselfData.SetTinglist(data.GetIntArray("qiduitings"));
                    GameCenter.Shortcuts.MahjongQuery.Do(p => p.ShowQueryTip(list));
                }
                Dispatch();
            }
        }

        [S2CResponseHandler(NetworkProtocol.MJOpreateType, GameMisc.QjmjKey)]
        [S2CResponseHandler(NetworkProtocol.MJOpreateType, GameMisc.MtfmjKey)]
        [S2CResponseHandler(NetworkProtocol.MJOpreateType, GameMisc.QyqhhmjKey)]
        public void OnOperate_Qjmj(ISFSObject data)
        {
            if (ParseOperate(data))
            {
                int op = DataCenter.OperateMenu;
                if (GameUtils.BinaryCheck((int)OperateMenuType.OpreateLaiZiGang, op))
                {
                    mOpMenuCache.Add(new KeyValuePair<int, bool>((int)OperateMenuType.OpreateLaiZiGang, true));
                }
                Dispatch();
            }
        }

        [S2CResponseHandler(NetworkProtocol.MJOpreateType, GameMisc.YzwmjKey)]
        [S2CResponseHandler(NetworkProtocol.MJOpreateType, GameMisc.YzmjKey)]
        [S2CResponseHandler(NetworkProtocol.MJOpreateType, GameMisc.XtmjKey)]
        public void OnOperate_Xtmj(ISFSObject data)
        {
            if (ParseOperate(data))
            {
                //单王调 双王调
                if (data.ContainsKey("wangdiao"))
                {
                    int type = data.GetInt("wangdiao");
                    if (type == 1)
                    {
                        mOpMenuCache.Add(new KeyValuePair<int, bool>((int)OperateMenuType.OperateWangdiao, true));
                    }
                    else if (type == 2)
                    {
                        mOpMenuCache.Add(new KeyValuePair<int, bool>((int)OperateMenuType.OperateSWangdiao, true));
                    }
                }
                Dispatch();
            }
        }
    }
}