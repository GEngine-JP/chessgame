using System.Collections.Generic;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public partial class GameLogic_Hu : AbsGameLogicBase
    {
        [S2CResponseHandler(NetworkProtocol.MJReqTypeZiMo, GameMisc.BbmjKey)]
        public void OnHu_Zimo_Bbmj(ISFSObject data)
        {
            OnHu(data);
            if (null == mZimoTask)
            {
                mZimoTask = ContinueTaskManager.NewTask()
                .AppendFuncTask(() => ZimoTask_Bbmj())
                .AppendFuncTask(() => HandcardCtrlTask())
                .AppendFuncTask(() => ZhaNiaoAnimation())
                .AppendActionTask(ActionCallback, Config.TimeHuAniInterval);
            }
            mZimoTask.Start();
        }

        private IEnumerator<float> ZimoTask_Bbmj()
        {
            if (!GameUtils.CheckStopTask())
            {
                yield return Config.TimeHuAniInterval;
                var huCard = mArgs.HuCard;
                var huChair = MahjongUtility.GetChair(mArgs.HuSeats[0]);
                //如果本家自摸，移除本家自摸的牌
                if (huChair == 0)
                {
                    Game.MahjongGroups.PlayerHand.PopMahjong();
                }
                //游金了之后，抓到白板胡，叫白燕白
                if (huCard == 87 && DataCenter.Players[huChair].IsAuto)
                {
                    MahjongUtility.PlayPlayerSound(huChair, "baiyanbai");
                    GameCenter.Scene.PlayPlayerEffect(huChair, PoolObjectType.hu);
                }
                else
                {
                    MahjongUtility.PlayOperateEffect(huChair, PoolObjectType.hu);
                }
                SetHuCard(huChair, huCard).Laizi = MahjongUtility.MahjongFlagCheck(huCard);
            }          
        }
    }
}