using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [GameRuntimeData(RuntimeDataType.Players, GameMisc.QdjtKey)]
    public class ExtPlayerData_Qdjt : AbsDataExtension
    {
        public override void SetData(ISFSObject data, MahjongUserInfo userInfo)
        {
            if (data.ContainsKey("visibleCards"))
            {
                var hasLiang = data.TryGetBool(ProtocolKey.KeyHasLiang);
                mParams["liangdaoCards"] = new VarIntArray(data.GetIntArray("visibleCards"));
                mParams["hasLiang"] = new VarBool(hasLiang);

                userInfo.IsAuto = hasLiang;
            }
        }
    }
}
