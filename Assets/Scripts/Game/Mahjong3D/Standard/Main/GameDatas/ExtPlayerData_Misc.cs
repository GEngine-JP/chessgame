using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [GameRuntimeData(RuntimeDataType.Players, GameMisc.ZhmjKey)]
    [GameRuntimeData(RuntimeDataType.Players, GameMisc.DlmjKey)]
    [GameRuntimeData(RuntimeDataType.Players, GameMisc.DltdhKey)]
    [GameRuntimeData(RuntimeDataType.Players, GameMisc.TdDltdhKey)]
    public class ExtPlayerData_Dlmj : AbsDataExtension
    {
        public override void SetData(ISFSObject datam, MahjongUserInfo userInfo)
        {
            if (datam.ContainsKey("visibleCards"))
            {
                mParams["nius"] = new VarIntArray(datam.GetIntArray("visibleCards"));
            }
        }
    }

    [GameRuntimeData(RuntimeDataType.Players, GameMisc.QjmjKey)]
    [GameRuntimeData(RuntimeDataType.Players, GameMisc.MtfmjKey)]
    [GameRuntimeData(RuntimeDataType.Players, GameMisc.QyqhhmjKey)]
    public class ExtPlayerData_Qjmj : AbsDataExtension
    {
        public override void SetData(ISFSObject datam, MahjongUserInfo userInfo)
        {
            if (datam.ContainsKey("laiziGang"))
            {
                mParams["laiziGang"] = new VarIntArray(datam.GetIntArray("laiziGang"));
            }
        }
    }

    [GameRuntimeData(RuntimeDataType.Players, GameMisc.NamjKey)]
    public class ExtPlayerData_Namj : AbsDataExtension
    {
        public override void SetData(ISFSObject data, MahjongUserInfo userInfo)
        {
            if (data.ContainsKey("qiduitings"))
            {
                int[] array = data.GetIntArray("qiduitings");
                userInfo.SetTinglist(array);
            }
            if (data.ContainsKey("guoval"))
            {
                mParams["guoval"] = new VarInt(data.GetInt("guoval"));
            }
        }
    }

    [GameRuntimeData(RuntimeDataType.Players, GameMisc.BbmjKey)]
    public class ExtplayerData_Bbmj : AbsDataExtension
    {
        public override void SetData(ISFSObject data, MahjongUserInfo userInfo)
        {
            userInfo.IsAuto = data.ContainsKey("cards");
        }
    }

    [GameRuntimeData(RuntimeDataType.Players, GameMisc.CcmjKey)]
    public class ExtplayerData_Ccmj : AbsDataExtension
    {
        public override void SetData(ISFSObject data, MahjongUserInfo userInfo)
        {
            mParams["userdata"] = new VarIsfsobject(data);
        }
    }

    [GameRuntimeData(RuntimeDataType.Players, GameMisc.BdmjKey)]
    [GameRuntimeData(RuntimeDataType.Players, GameMisc.SzmjKey)]
    public class ExtplayerData_Szmj : AbsDataExtension
    {
        public override void SetData(ISFSObject data, MahjongUserInfo userInfo)
        {
            mParams["diling"] = new VarInt(data.TryGetInt("diling"));
        }
    }
}