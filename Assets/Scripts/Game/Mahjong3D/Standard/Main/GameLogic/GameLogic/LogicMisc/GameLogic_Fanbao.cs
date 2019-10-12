using YxFramwork.ConstDefine;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [S2CResponseLogic]
    public partial class GameLogic_Fanbao : AbsGameLogicBase
    {
        [S2CResponseHandler(NetworkProtocol.MJRequestTypeBao)]
        public void OnFanbao(ISFSObject data)
        {
            FanbaoData fanData = new FanbaoData();
            fanData.SetData(data);
            //设置宝牌      
            var tempCard = Game.TableManager.GetParts<MahjongDisplayCard>(TablePartsType.DisplayCard).DisplayMahjong;
            //宝牌是否显示

            bool isShowCard = !DataCenter.ConfigData.AnBao && DataCenter.OneselfData.IsAuto;
            if (tempCard != null)
            {
                if (fanData.IsHuanbao)
                {
                    FanbaoAnimation(fanData);
                    var card = Game.TableManager.SetShowBao(fanData.Bao, isShowCard);
                    var obj = GameCenter.Pools.GetPool<ObjectPoolComponent>(PoolUitlity.Po_EffectObject).Pop<EffectObject>(EffectObject.AssetsNamePrefix + PoolObjectType.huanbao, (go) =>
                    {
                        return go.Type == PoolObjectType.huanbao;
                    });
                    if (null != obj)
                    {
                        obj.transform.position = card.transform.position;
                        obj.Execute();
                    }
                }
                else if (DataCenter.CurrOpSeat == DataCenter.OneselfData.Seat && !fanData.Filter)
                {
                    Game.TableManager.SetShowBao(fanData.Bao, isShowCard);
                }
            }
            else
            {
                FanbaoAnimation(fanData);
                //第一次翻宝
                Game.TableManager.SetShowBao(fanData.Bao, isShowCard);
            }
        }

        private void FanbaoAnimation(FanbaoData data)
        {
            //延迟接收消息
            GameCenter.Network.SetDelayTime(1.5f);
            DataCenter.LeaveMahjongCnt--;
            Game.TableManager.StopTimer();
            System.Action action = () =>
            {
                //设置计时器               
                Game.TableManager.StartTimer(Config.TimeOutcard);
                Game.MahjongGroups.OnFanbaoRmoveMahjong(data.BaoIndex);
                var cpgLogic = GameCenter.Network.GetGameResponseLogic<GameLogic_Cpg>();
                if (data.IsHuanbao && data.LastBao > 0 && !cpgLogic.IsGangBao)
                {
                    //将旧的宝牌放在出牌的牌墙中    
                    var item = Game.MahjongGroups.MahjongThrow[DataCenter.CurrOpChair].GetInMahjong(data.LastBao);
                    item.SetOtherSign(Anchor.TopRight, true);
                    Game.TableManager.ShowOutcardFlag(item);
                }
            };
            //打骰子
            Game.TableManager.PlaySaiziAnimation((byte)data.Saizi, action);
        }

        public struct FanbaoData : IData
        {
            public bool Filter;//如果宝牌不变，不操作
            public bool IsHuanbao;
            public int BaoIndex;
            public int LastBao;
            public int Saizi;
            public int Bao;

            public void SetData(ISFSObject data)
            {
                IsHuanbao = false;
                GameCenter.DataCenter.CurrOpSeat = data.TryGetInt(RequestKey.KeySeat);
                Bao = data.TryGetInt(ProtocolKey.CardBao);
                BaoIndex = data.TryGetInt("baoindex");
                LastBao = data.TryGetInt("lastbao");
                Saizi = data.TryGetInt("saizi");
                IsHuanbao = data.ContainsKey("lastbao") && BaoIndex != Bao;
                Filter = GameCenter.DataCenter.Game.BaoCard == Bao;
                //确定宝牌
                if (Bao == 0)
                {
                    Bao = 17;
                }
                else
                {
                    GameCenter.DataCenter.Game.BaoCard = Bao;
                }
            }
        }
    }
}