using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public partial class GameLogic_Reconnect : AbsGameLogicBase
    {
        [S2CResponseHandler(CustomClientProtocol.CustomTypeReconnectLogic, GameMisc.XlmjKey)]
        public void OnReconnect_Xlmj(ISFSObject data)
        {
            var currChair = DataCenter.CurrOpChair;
            //设置发牌点
            Game.MahjongGroups.SetSendCardSPos(DataCenter.BankerSeat, DataCenter.OneselfData.Seat);
            //设置抓牌点
            Game.MahjongGroups.SetCatchCardStartPos(DataCenter.SaiziPoint);
            int removeCardNum = DataCenter.Room.MahjongCount - DataCenter.LeaveMahjongCnt;
            Game.MahjongGroups.PopMahFromCurrWall(removeCardNum);
            //设置麻将牌
            MahjongUserInfo player;
            for (int i = 0; i < DataCenter.MaxPlayerCount; i++)
            {
                player = DataCenter.Players[i];
                //设置出牌
                Game.MahjongGroups.MahjongThrow[i].GetInMahjong(player.OutCards.ToArray());
                //设置吃碰杠牌
                Game.MahjongGroups.MahjongCpgs[i].SetCpgArray(player.CpgDatas.ToArray());
                //设置手牌 
                Game.MahjongGroups.MahjongHandWall[i].GetInMahjong(player.HardCards);
                //听牌
                if (DataCenter.Players[i].IsAuto)
                {
                    Game.MahjongGroups.MahjongHandWall[i].SetHandCardState(HandcardStateTyps.Ting);
                }
                //设置补张
                if (!player.BuzhangCards.ExIsNullOjbect() && player.BuzhangCards.Length > 0)
                {
                    int num = player.BuzhangCards.Length;
                    DataCenter.LeaveMahjongCnt -= num;
                    Game.MahjongGroups.PopMahFromCurrWall(num);
                    Game.MahjongGroups.MahjongOther[i].GetInMahjong(player.BuzhangCards);
                }
            }
            //设置自己手中的赖子牌
            Game.MahjongGroups.PlayerHand.SetLaizi(DataCenter.Game.LaiziCard);
            //更新牌桌
            UpdateMahjongTable();

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
                            int[] hucardlist = userInfo.ExtData.Get<VarIntArray>("hucardlist");
                            userInfo.HucardList.AddRange(hucardlist);
                            if (hucardlist.ExIsNullOjbect()) continue;
                            for (int j = 0; j < hucardlist.Length; j++)
                            {
                                var item = Game.MahjongCtrl.PopMahjong(hucardlist[j]).GetComponent<MahjongContainer>();
                                Game.MahjongGroups.MahjongOther[i].GetInMahjong(item);
                                item.gameObject.SetActive(true);
                            }

                            DataCenter.Players[i].IsAuto = true;
                            Game.MahjongGroups.MahjongHandWall[i].SetHandCardState(HandcardStateTyps.SingleHu);
                            DataCenter.LeaveMahjongCnt -= hucardlist.Length;
                            Game.MahjongGroups.PopMahFromCurrWall(hucardlist.Length);
                        }
                        break;
                }
            }

            SetReconectCardState();
            MahjongQueryHuTip();
            GameCenter.Shortcuts.SwitchCombination.Open((int)GameSwitchType.PowerAiAgency);
        }
    }
}
