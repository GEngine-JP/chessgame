using System.Collections.Generic;
using YxFramwork.ConstDefine;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public partial class GameLogic_Hu : AbsGameLogicBase
    {
        private SingleHuData mSingleHuData;

        [S2CResponseHandler(NetworkProtocol.MJGameResult, GameMisc.XzmjKey)]
        [S2CResponseHandler(NetworkProtocol.MJGameResult, GameMisc.XlmjKey)]
        [S2CResponseHandler(NetworkProtocol.MJReqTypeLastCd, GameMisc.XzmjKey)]
        [S2CResponseHandler(NetworkProtocol.MJReqTypeLastCd, GameMisc.XlmjKey)]
        public void OnHu_GameResult(ISFSObject data)
        {
            //隐藏听箭头
            GameCenter.Scene.MahjongGroups.PlayerHand.OnQueryMahjong(null);
            mResultType = SingleResultArgs.HuResultType.HuSingle;
            OnHu_LastCd(data);
        }

        /// <summary>
        /// 一玩家胡之后接着玩
        /// </summary>  
        [S2CResponseHandler(NetworkProtocol.MJReqTypeZiMo, GameMisc.XzmjKey)]
        [S2CResponseHandler(NetworkProtocol.MJRequestTypeHu, GameMisc.XzmjKey)]
        public void OnHu_Single_Xzmj(ISFSObject data)
        {
            //隐藏听箭头
            GameCenter.Scene.MahjongGroups.PlayerHand.OnQueryMahjong(null);
            GameCenter.Network.SetDelayTime(1.5f);
            mSingleHuData.SetData(data);
            if (null == mHuTask)
            {
                mHuTask = ContinueTaskManager.NewTask().AppendFuncTask(() => SingleHuTask());
            }
            mHuTask.Start();
        }

        private IEnumerator<float> SingleHuTask()
        {
            //移除牌，播放特效
            if (mSingleHuData.HuType != NetworkProtocol.MJReqTypeZiMo)
            {
                var currChair = DataCenter.CurrOpChair;
                var effect = MahjongUtility.PlayMahjongEffectAndAudio(PoolObjectType.shandian);
                effect.transform.position = Game.MahjongGroups.MahjongThrow[currChair].GetLastMjPos();
                effect.Execute();
                yield return 0.8f;
                Game.MahjongGroups.MahjongThrow[currChair].PopMahjong();
            }
            var huSeats = mSingleHuData.HuSeats;
            for (int i = 0; i < huSeats.Count; i++)
            {
                var chair = MahjongUtility.GetChair(huSeats[i]);
                if (mSingleHuData.HuType == NetworkProtocol.MJReqTypeZiMo)
                {
                    Game.MahjongGroups.MahjongHandWall[chair].PopMahjong();
                    MahjongUtility.PlayOperateEffect(chair, PoolObjectType.zimo);
                }
                else
                {                   
                    MahjongUtility.PlayOperateEffect(chair, PoolObjectType.hu);
                }
                SetHuCard(chair, mSingleHuData.HuCard);
                Game.MahjongGroups.MahjongHandWall[chair].SetHandCardState(HandcardStateTyps.SingleHu);
            }
            //抢杠胡
            if (mSingleHuData.Flag)
            {
                DataCenter.OneselfData.HardCards.Remove(mSingleHuData.HuCard);
                Game.MahjongGroups.PlayerHand.OnQiangganghu(mSingleHuData.HuCard);
            }
            //加分特效
            var huGlods = mSingleHuData.HuGolds;
            Dictionary<int, long> scoreList = new Dictionary<int, long>();
            for (int i = 0; i < huGlods.Length; i++)
            {
                int score = huGlods[i];
                if (score != 0)
                {
                    scoreList[MahjongUtility.GetChair(i)] = score;
                }
            }
            yield return 0.5f;
            GameCenter.EventHandle.Dispatch((int)UIEventProtocol.PlayAddScore, new SetScoreArgs()
            {
                DelayTime = 0f,
                ScoreDic = scoreList,
                Type = (int)SetScoreType.AddScoreAndEffect,
            });
            yield return 0.8f;
            //显示胡UI
            GameCenter.EventHandle.Dispatch((int)UIEventProtocol.SetSingleHuFlag, new HuanAndDqArgs()
            {
                Type = 2,
                HuSeats = huSeats,
            });
        }

        public struct SingleHuData : IData
        {
            public bool Flag;
            public int HuType;
            public int HuCard;
            public int[] HuGolds;
            public List<int> HuSeats;
            public List<int> HuCardList;//胡牌座位

            public void SetData(ISFSObject data)
            {
                Flag = false;
                HuSeats = new List<int>();
                if (data.ContainsKey("huseat"))
                {
                    HuSeats.Add(data.GetInt("huseat"));
                }
                else if (data.ContainsKey("huseatlist"))
                {
                    var iSeatList = data.GetIntArray("huseatlist");
                    for (int i = 0; i < iSeatList.Length; i++)
                    {
                        HuSeats.Add(iSeatList[i]);
                    }
                }
                HuType = data.GetInt(RequestKey.KeyType);
                HuGolds = data.GetIntArray("score");
                HuCard = data.GetInt("hucard");
                //抢杠胡
                if (data.ContainsKey("ctype"))
                {
                    int ctype = data.GetInt("ctype");
                    var db = GameCenter.DataCenter;
                    //抢杠胡
                    if ((ctype != 0) && ((ctype & NetworkProtocol.MJQiangGangHuType) != 0) && (db.CurrOpSeat == db.OneselfData.Seat))
                    {
                        Flag = true;
                    }
                }
            }
        }
    }
}