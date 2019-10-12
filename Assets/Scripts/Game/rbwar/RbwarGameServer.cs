using Sfs2X.Entities.Data;
using YxFramwork.Controller;

namespace Assets.Scripts.Game.rbwar
{
    public class RbwarGameServer : YxGameServer
    {

        public void UserBet(string pos,int gold)
        {
            ISFSObject betData = new SFSObject();
            betData.PutUtfString("p",pos);
            betData.PutInt("gold", gold);
            betData.PutInt("type",107);
            SendGameRequest(betData);
        }
    }
}
