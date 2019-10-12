using System.Collections.Generic;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public partial class GameLogic_Hu : AbsGameLogicBase
    {
        [S2CResponseHandler(NetworkProtocol.MJReqTypeZiMo, GameMisc.XlmjKey)]
        [S2CResponseHandler(NetworkProtocol.MJRequestTypeHu, GameMisc.XlmjKey)]
        public void OnHu_Single_Xlmj(ISFSObject data)
        {
            //隐藏听箭头
            GameCenter.Scene.MahjongGroups.PlayerHand.OnQueryMahjong(null);
            GameCenter.Network.SetDelayTime(1f);
            mSingleHuData.SetData(data);
            if (null == mHuTask)
            {
                mHuTask = ContinueTaskManager.NewTask().AppendFuncTask(() => SingleHuTask_Xlmj());
            }
            mHuTask.Start();
        }

        private IEnumerator<float> SingleHuTask_Xlmj()
        {
            var huSeats = mSingleHuData.HuSeats;
            MahjongUserInfo userInfo;
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
                userInfo = DataCenter.Players[chair];
                SetHuCard(chair, mSingleHuData.HuCard);
                userInfo.IsHu = true;
                userInfo.IsAuto = true;
                userInfo.HucardList.Add(mSingleHuData.HuCard);
                Game.MahjongGroups.MahjongHandWall[chair].SetHandCardState(HandcardStateTyps.SingleHu);
            }

            //隐藏听箭头
            GameCenter.Scene.MahjongGroups.PlayerHand.OnQueryMahjong(null);
            var currChair = DataCenter.CurrOpChair;
            //移除牌，播放特效
            if (mSingleHuData.HuType != NetworkProtocol.MJReqTypeZiMo)
            {

                var effect = MahjongUtility.PlayMahjongEffectAndAudio(PoolObjectType.shandian);
                effect.transform.position = Game.MahjongGroups.MahjongThrow[currChair].GetLastMjPos();
                effect.Execute();
                yield return 0.8f;
                Game.MahjongGroups.MahjongThrow[currChair].PopMahjong();
            }
            else
            {
                if (currChair == 0)
                {
                    //胡的牌从手牌中移除
                    DataCenter.OneselfData.HardCards.Remove(mSingleHuData.HuCard);
                }
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
        }
    }
}