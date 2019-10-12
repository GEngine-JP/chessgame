using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [S2CResponseLogic]
    public partial class GameLogic_QueryHu : AbsGameLogicBase
    {
        [FilterOperateMenu]
        [S2CResponseHandler(NetworkProtocol.MJRequestTypeGetHuCards)]
        public void OnQueryHuCard(ISFSObject data)
        {
            var card = data.GetInt("card");
            var arr = data.GetIntArray("hulist");
            var ratelist = data.TryGetIntArray("ratelist");
            if (null != arr && arr.Length > 0)
            {
                GameCenter.EventHandle.Dispatch((int)UIEventProtocol.QueryHuCard, new QueryHuArgs()
                {
                    QueryCard = card,
                    AllowHuCards = arr,
                    RateArray = ratelist
                });
            }
        }
    }
}