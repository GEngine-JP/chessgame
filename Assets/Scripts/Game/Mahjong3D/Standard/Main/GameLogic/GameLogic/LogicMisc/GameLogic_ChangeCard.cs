using System.Collections.Generic;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [S2CResponseLogic]
    public partial class GameLogic_ChangeCard : AbsGameLogicBase
    {
        private HuanAndDqData mHuanData;
        private ContinueTaskContainer mHuanCardTask;

        /// <summary>
        /// 换张选牌开始
        /// </summary>
        [S2CResponseHandler(NetworkProtocol.MJChangeCards, GameMisc.XzmjKey)]
        [S2CResponseHandler(NetworkProtocol.MJChangeCards, GameMisc.XlmjKey)]
        public void OnChangeCardsStart(ISFSObject data)
        {
            //关闭开启托管权限
            GameCenter.Shortcuts.SwitchCombination.Close((int)GameSwitchType.PowerAiAgency);
            //换张时禁止出牌
            Game.MahjongGroups.PlayerToken = false;
            //显示UI
            GameCenter.EventHandle.Dispatch((int)UIEventProtocol.SetPlayerFlagState, new PlayerStateFlagArgs()
            {
                CtrlState = true,
                StateFlagType = (int)PlayerStateFlagType.SelectCard
            });
            //切换手牌状态为换牌
            Game.MahjongGroups.PlayerHand.SetHandCardState(HandcardStateTyps.ExchangeCards, 3);
        }

        /// <summary>
        /// 换张选牌结束
        /// </summary>
        [S2CResponseHandler(NetworkProtocol.MJRotateCards, GameMisc.XzmjKey)]
        [S2CResponseHandler(NetworkProtocol.MJRotateCards, GameMisc.XlmjKey)]
        public void OnChangeCardsEnd(ISFSObject data)
        {
            mHuanData.SetData(data);
            GameCenter.Network.SetDelayTime(3.5f);
            GameCenter.EventHandle.Dispatch((int)UIEventProtocol.SetPlayerFlagState, new PlayerStateFlagArgs()
            {
                CtrlState = false
            });
            GameCenter.EventHandle.Dispatch((int)UIEventProtocol.ChangeCardTip, new HuanAndDqArgs()
            {
                HuanType = mHuanData.HuanType
            });
            //停止定时任务
            GameCenter.Scene.TableManager.StopTimer();
            //移除旧手牌数据
            for (int i = 0; i < mHuanData.ChangeCards.Length; i++)
            {
                DataCenter.OneselfData.HardCards.Remove(mHuanData.ChangeCards[i]);
            }
            //添加新手牌数据
            for (int i = 0; i < mHuanData.NewCards.Length; i++)
            {
                DataCenter.OneselfData.HardCards.Add(mHuanData.NewCards[i]);
            }
            //所有玩家牌扣到桌子上
            var mahHand = Game.MahjongGroups.PlayerHand;
            var xzMahHand = mahHand.GetMahHandComponent<MahPlayerHand_Xzmj>();
            for (int i = 0; i < DataCenter.MaxPlayerCount; i++)
            {
                if (i == 0)
                {
                    //超时扣牌
                    if (Game.MahjongGroups.SwitchGorup[0].MahjongList.Count == 0)
                    {
                        xzMahHand.SwitchCardsTimeOut(mHuanData.ChangeCards);
                        Game.MahjongGroups.SwitchGorup[0].AddMahToSwitch(mHuanData.ChangeCards);
                    }
                }
                else
                {
                    //其他家扣牌
                    int[] array = new int[mHuanData.ChangeCards.Length];
                    var mahList = Game.MahjongGroups.MahjongHandWall[i].MahjongList;
                    for (int j = 0; j < array.Length; j++)
                    {
                        var item = mahList[j];
                        item.gameObject.SetActive(false);
                        array[j] = item.Value;
                    }
                    Game.MahjongGroups.SwitchGorup[i].AddMahToSwitch(array);
                }
            }          
            //3人血战，换牌动画设置
            if (DataCenter.MaxPlayerCount == 3)
            {
                var switchGorup = Game.MahjongGroups.SwitchGorup;
                //将麻将牌放到组中             
                for (int i = 0; i < DataCenter.MaxPlayerCount; i++)
                {
                    for (int j = 0; j < switchGorup[i].MahjongCnt; j++)
                    {
                        if (mHuanData.HuanType == 0)
                        {
                            if (i < 2) switchGorup[i].MahjongList[j].transform.SetParent(switchGorup.Group1);
                            else switchGorup[i].MahjongList[j].transform.SetParent(switchGorup.Group2);
                        }
                        else
                        {
                            if (i == 1) switchGorup[i].MahjongList[j].transform.SetParent(switchGorup.Group2);
                            else switchGorup[i].MahjongList[j].transform.SetParent(switchGorup.Group1);
                        }
                    }
                }
            }
            Game.MahjongGroups.SwitchGorup.StartRotation(mHuanData.HuanType);
            if (mHuanCardTask == null)
            {
                mHuanCardTask = ContinueTaskManager.NewTask().AppendFuncTask(HuanCardTask);
            }
            mHuanCardTask.Start();
            //手牌记录
            DataCenter.Players.AddRecordMahjongs();
        }

        private IEnumerator<float> HuanCardTask()
        {
            yield return 1.5f;
            var game = GameCenter.Scene;
            for (int i = 0; i < DataCenter.MaxPlayerCount; i++)
            {
                Game.MahjongGroups.SwitchGorup[i].OnReset();
                if (i == 0)
                {
                    for (int j = 0; j < mHuanData.NewCards.Length; j++)
                    {
                        game.MahjongGroups.MahjongHandWall[0].GetInMahjong(mHuanData.NewCards[j]);
                    }
                }
                else
                {
                    var mahList = Game.MahjongGroups.MahjongHandWall[i].MahjongList;
                    for (int j = 0; j < mahList.Count; j++)
                    {
                        mahList[j].gameObject.SetActive(true);
                    }
                }
            }
            //开启托管权限
            GameCenter.Shortcuts.SwitchCombination.Open((int)GameSwitchType.PowerAiAgency);
            game.MahjongGroups.MahjongHandWall[0].SortHandMahjong();
            Game.MahjongGroups.PlayerHand.SetHandCardState(HandcardStateTyps.Normal);
            Game.MahjongGroups.PlayerToken = true;
        }

        public struct HuanAndDqData : IData
        {
            public int HuanType;// 换牌类型 0顺时针 1逆时针 2对家
            public int[] ChangeCards;// 扣下去的牌数组
            public int[] NewCards;// 换过来的牌数组           
            public void SetData(ISFSObject data)
            {
                HuanType = data.GetInt("huanType");
                ChangeCards = data.GetIntArray("changecards");
                NewCards = data.GetIntArray("cards");
            }
        }
    }
}