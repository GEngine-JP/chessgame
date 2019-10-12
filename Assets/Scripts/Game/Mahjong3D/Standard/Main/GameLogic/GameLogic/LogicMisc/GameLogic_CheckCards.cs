using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [S2CResponseLogic]
    public partial class GameLogic_CheckCards : AbsGameLogicBase   
    {
        [S2CResponseHandler(NetworkProtocol.MjRequestTypeCheckCards)]
        public void CheckCards(ISFSObject data)
        {
            var cards = data.GetIntArray("cards");
            string output = "";
            for (int i = 0; i < cards.Length; i++)
            {
                output += " " + cards[i];
            }
            YxDebug.LogEvent("错误后同步的手牌 :" + output);
        }
    }
}
