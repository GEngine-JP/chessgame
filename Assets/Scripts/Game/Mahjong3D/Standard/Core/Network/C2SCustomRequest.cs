using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public interface IC2SCustomRequest { }

    public partial class C2SCustomRequest
    {
        private IC2SCustomRequest mCustom;

        public T Custom<T>() where T : class, IC2SCustomRequest, new()
        {
            if (mCustom == null)
            {
                mCustom = new T();
            }
            return mCustom as T;
        }

        //出牌
        public void ThrowoutCard(int card)
        {
            GameCenter.Network.OnRequestC2S((sfs) =>
            {
                sfs.PutInt(RequestKey.KeyType, NetworkProtocol.MJThrowoutCard);
                sfs.PutInt(RequestKey.KeyOpCard, card);
                return sfs;
            });
        }

        //重新游戏开始
        public void RestartGame()
        {
            GameCenter.GameProcess.ChangeState<StateGameReady>();
        }

        //玩家准备
        public void Ready()
        {
            var network = GameCenter.Network;
            network.OnRequestC2S((sfs) =>
            {
                string key = MahjongUtility.GameKey + "." + RequestCmd.Ready;
                network.SendRequest(key, sfs);
                return null;
            });
        }

        //解散房间
        public void OnDismissRoom(int dismissType)
        {
            var network = GameCenter.Network;
            network.OnRequestC2S((sfs) =>
            {
                var dataCenter = GameCenter.DataCenter;
                if (dataCenter.IsGamePlaying)
                {
                    sfs.PutUtfString("cmd", "dismiss");
                    sfs.PutInt(RequestKey.KeyType, dismissType);
                    network.SendRequest("hup", sfs);
                }
                else
                {
                    if (dataCenter.OneselfData.IsOwner)
                    {
                        network.SendRequest("dissolve", sfs);
                    }
                    else
                    {
                        MahjongUtility.ReturnToHall();
                    }
                }
                return null;
            });
        }
    }
}