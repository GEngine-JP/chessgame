using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public enum XzmjGameStatue
    {
        free,
        huanZhanging,
        huanZhangOver,
        duanMening,
        duanMenOver,
        end,
        hu,
    }

    public partial class GameLogic_Reconnect : AbsGameLogicBase
    {
        [S2CResponseHandler(CustomClientProtocol.CustomTypeReconnectLogic, GameMisc.XzmjKey)]
        public void OnReconnect_Xzmj(ISFSObject data)
        {
            OnReconnect(data);
            SetPlayerState();
            MahjongUserInfo userInfo;
            for (int i = 0; i < DataCenter.MaxPlayerCount; i++)
            {
                userInfo = DataCenter.Players[i];
                int state = userInfo.ExtData.Get<VarInt>("state");
                switch ((XzmjGameStatue)state)
                {
                    case XzmjGameStatue.hu:
                        {
                            //设置手牌状态
                            Game.MahjongGroups.MahjongHandWall[i].SetHandCardState(HandcardStateTyps.SingleHu);
                            //设置胡牌
                            int hucard = userInfo.ExtData.Get<VarInt>("hcard");
                            var item = Game.MahjongCtrl.PopMahjong(hucard);
                            Game.MahjongGroups.MahjongOther[i].GetInMahjong(item);
                            item.gameObject.SetActive(true);
                        }
                        break;
                }
            }
        }

        private void SetPlayerState()
        {
            MahjongUserInfo userInfo;
            var mahHand = Game.MahjongGroups.PlayerHand;
            var xzMahHand = mahHand.GetMahHandComponent<MahPlayerHand_Xzmj>();
            for (int i = 0; i < DataCenter.MaxPlayerCount; i++)
            {
                userInfo = DataCenter.Players[i];
                int state = userInfo.ExtData.Get<VarInt>("state");
                switch ((XzmjGameStatue)state)
                {
                    case XzmjGameStatue.huanZhanging:
                        {
                            if (i == 0)
                            {
                                GameCenter.EventHandle.Dispatch((int)UIEventProtocol.SetPlayerFlagState, new PlayerStateFlagArgs()
                                {
                                    CtrlState = true,
                                    StateFlagType = (int)PlayerStateFlagType.SelectCard
                                });
                                Game.MahjongGroups.PlayerHand.SetHandCardState(HandcardStateTyps.ExchangeCards, 3);
                            }
                        }
                        break;
                    case XzmjGameStatue.huanZhangOver:
                        {
                            if (i == 0)
                            {
                                //换张之后的重连需要显示扣下的牌                                
                                Game.MahjongGroups.PopMahFromCurrWall(3);
                                Game.MahjongGroups.SwitchGorup[0].AddMahToSwitch(new int[3] { 17, 18, 19 });
                            }
                        }
                        break;
                    case XzmjGameStatue.duanMening:
                        {
                            if (i == 0)
                            {
                                mahHand.SetHandCardState(HandcardStateTyps.Dingqueing);
                                GameCenter.EventHandle.Dispatch((int)UIEventProtocol.SetPlayerFlagState, new PlayerStateFlagArgs()
                                {
                                    CtrlState = true,
                                    StateFlagType = (int)PlayerStateFlagType.Selecting
                                });
                            }
                        }
                        break;
                    case XzmjGameStatue.duanMenOver:
                    case XzmjGameStatue.end:
                        {
                            if (i == 0)
                            {
                                int color = userInfo.ExtData.Get<VarInt>("htype");
                                // 本家缺门花色置灰
                                xzMahHand.ChangeHandMahGray(color);
                                //到自己出牌时，提示打牌                             
                                if (DataCenter.ReconectCardState == 0 && DataCenter.SelfCurrOp)
                                {
                                    Game.MahjongGroups.MahjongHandWall[0].SetLastCardPos(DataCenter.GetInMahjong);
                                }
                            }
                        }
                        break;
                }
            }
        }
    }
}