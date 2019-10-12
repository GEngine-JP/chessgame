using System.Collections.Generic;
using YxFramwork.ConstDefine;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [S2CResponseLogic]
    public partial class GameLogic_Buzhang : AbsGameLogicBase
    {
        /// <summary>
        /// 游戏开始时补张缓存
        /// </summary>
        private Queue<BuZhangData> mBuZhangQueue = new Queue<BuZhangData>();
        /// <summary>
        /// 游戏开始时补张缓存
        /// </summary>
        private Queue<int> mGetCardBuzhangQueue = new Queue<int>();
        /// <summary>
        /// 游戏发牌时补张
        /// </summary>
        private ContinueTaskContainer mBuzhangTask;
        /// <summary>
        /// 补张的座位号
        /// </summary>
        private int mBuZhangChair;

        public Queue<int> BuzhangQueue
        {
            get { return mGetCardBuzhangQueue; }
        }

        public int BuZhangChair
        {
            get { return mBuZhangChair; }
        }

        [S2CResponseHandler(NetworkProtocol.MJRequestTypeBuZhang)]
        public void OnBuZhang(ISFSObject data)
        {
            var buData = new BuZhangData()
            {
                chair = MahjongUtility.GetChair(data),
                BuZhangCards = data.GetIntArray("buZhangCard"),
                Cards = data.GetIntArray(RequestKey.KeyCards),
            };
            mBuZhangQueue.Enqueue(buData);
        }

        [S2CResponseHandler(NetworkProtocol.MJRequestTypeBuZhangFinish)]
        public void OnBuZhangFinish(ISFSObject data)
        {
            GameCenter.Network.SetDelayTime(mBuZhangQueue.Count * (0.7f) + 0.2f);
            if (mBuzhangTask == null)
            {
                mBuzhangTask = ContinueTaskManager.NewTask().AppendFuncTask(BuZhangAnimation);
            }
            mBuzhangTask.Start();
        }

        [S2CResponseHandler(NetworkProtocol.MJRequestTypeBuZhangGetIn)]
        public void OnGetInBuZhang(ISFSObject data)
        {
            mBuZhangChair = MahjongUtility.GetChair(data);
            mGetCardBuzhangQueue.Enqueue(data.GetInt(RequestKey.KeyOpCard));
            GameCenter.Shortcuts.SwitchCombination.Open((int)GameSwitchType.HasBuzhang);
            //Game.MahjongGroups.PlayerToken = false;
        }

        public IEnumerator<float> BuZhangAnimation()
        {
            BuZhangData buData;
            while (mBuZhangQueue.Count > 0)
            {
                buData = mBuZhangQueue.Dequeue();
                //移除手牌中的
                Game.MahjongGroups.MahjongHandWall[buData.chair].RemoveMahjong(buData.BuZhangCards);
                //添加到胡牌中
                Game.MahjongGroups.MahjongOther[buData.chair].GetInMahjong(buData.BuZhangCards);
                yield return DataCenter.Config.TimeBuzhangAniDelay;
                //在墙中移除
                DataCenter.LeaveMahjongCnt -= buData.BuZhangCards.Length;
                Game.MahjongGroups.PopMahFromCurrWall(buData.BuZhangCards.Length);
                //添加到手牌中
                Game.MahjongGroups.MahjongHandWall[buData.chair].GetInMahjongWithRoat(buData.Cards);
                MahjongUtility.PlayPlayerSound(buData.chair, "buhua");
                //补张的牌 添加到手牌中
                if (buData.chair == 0)
                {                                      
                    //移除花牌
                    DataCenter.Players.RemoveHandCardData(0, buData.BuZhangCards);
                    //添加新牌           
                    DataCenter.Players.AddHandCardData(0, buData.Cards);
                    //重新展示听提示
                    var tingList = DataCenter.Players[0].TingList;
                    GameCenter.Shortcuts.MahjongQuery.ShowQueryTip(tingList);
                }
                yield return DataCenter.Config.TimeBuzhangAniDelay;
            }
        }

        public override void OnReset()
        {
            mBuZhangQueue.Clear();
            mGetCardBuzhangQueue.Clear();
        }
    }

    public class BuZhangData
    {
        public int chair;
        public int[] Cards;
        public int[] BuZhangCards;
    }
}