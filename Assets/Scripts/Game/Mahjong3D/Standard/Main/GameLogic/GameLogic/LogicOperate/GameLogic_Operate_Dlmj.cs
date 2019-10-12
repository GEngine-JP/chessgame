using System.Collections.Generic;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public partial class GameLogic_Operate : AbsGameLogicBase
    {
        [S2CResponseHandler(NetworkProtocol.MJOpreateType, GameMisc.ZhmjKey)]
        [S2CResponseHandler(NetworkProtocol.MJOpreateType, GameMisc.DlmjKey)]
        [S2CResponseHandler(NetworkProtocol.MJOpreateType, GameMisc.DltdhKey)]
        [S2CResponseHandler(NetworkProtocol.MJOpreateType, GameMisc.TdDltdhKey)]
        public void OnOperate_Dlmj(ISFSObject data)
        {
            if (ParseOperate(data))
            {
                int op = DataCenter.OperateMenu;
                if (GameUtils.BinaryCheck((int)OperateMenuType.OpreateTing, op))
                {
                    if (DataCenter.ConfigData.NiuBi)
                    {
                        mOpMenuCache.Add(new KeyValuePair<int, bool>((int)OperateMenuType.OperateNiu, true));
                    }
                }
                Dispatch();
            }
        }       
    }
}
