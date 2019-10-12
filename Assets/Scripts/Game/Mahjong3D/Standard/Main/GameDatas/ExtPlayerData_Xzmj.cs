using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [GameRuntimeData(RuntimeDataType.Players, GameMisc.XzmjKey)]
    public class ExtPlayerData_Xzmj : AbsDataExtension
    {
        public override void SetData(ISFSObject data, MahjongUserInfo userInfo)
        {
            //状态
            if (data.ContainsKey("xuezhanstatue"))
            {
                VarInt state = data.GetInt("xuezhanstatue");
                mParams["state"] = state;
            }
            //定缺
            if (data.ContainsKey("huanType"))
            {
                mParams["htype"] = new VarInt(data.GetInt("huanType"));
            }
            //胡牌
            if (data.ContainsKey("hucard"))
            {
                mParams["hcard"] = new VarInt(data.GetInt("hucard"));
            }        
        }
    }

    [GameRuntimeData(RuntimeDataType.Players, GameMisc.XlmjKey)]
    public class ExtPlayerData_Xlmj : ExtPlayerData_Xzmj
    {
        public override void SetData(ISFSObject data, MahjongUserInfo userInfo)
        {
            base.SetData(data, userInfo);
            //胡牌
            if (data.ContainsKey("hucardlist"))
            {
                mParams["hucardlist"] = new VarIntArray(data.GetIntArray("hucardlist"));
            }
        }
    }
}