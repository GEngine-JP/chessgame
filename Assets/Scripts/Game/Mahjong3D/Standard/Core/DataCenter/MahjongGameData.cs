using YxFramwork.Common.Model;
using YxFramwork.Common.Utils;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahjongGameData : YxGameData
    {
        protected override void InitGameData(ISFSObject gameInfo)
        {
            base.InitGameData(gameInfo);         
        }

        protected override YxBaseGameUserInfo OnInitUser(ISFSObject userData)
        {
            var userInfo = new MahjongUserInfo();
            userInfo.Parse(userData);
            return userInfo;
        }
    }
}