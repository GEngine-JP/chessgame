using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahjongSceneComponent : BaseComponent, IGameInfoICycle, IGameReadyCycle, IReconnectedCycle, IPlaybackCycle
    {
        public MahjongCamera MahjongCamera;
        public MahjongTableManager TableManager;
        public MahjongGroupsManager MahjongGroups;
        public MahjongContainerManager MahjongCtrl;

        private bool mInitSceneFlag;

        public bool HandMahTouchEnable { get; set; }

        public void OnGameInfoICycle()
        {
            if (!mInitSceneFlag)
            {
                mInitSceneFlag = true;
                var db = GameCenter.DataCenter;
                MahjongCtrl.InitalizationMahjong(db.Room.SysCards);
                MahjongGroups.GetContainer(db.MaxPlayerCount);
                MahjongGroups.SetMahjongWallCapacity(db.Room.MahjongCount);
            }
        }

        public void OnPlaybackCycle()
        {
            int count = GameCenter.DataCenter.MaxPlayerCount;
            MahjongGroups.GetContainer(count);
            var dnxb = TableManager.GetParts<MahjongTableDnxb>(TablePartsType.DnxbDirection);
            dnxb.SetDriectionMaterial();
        }

        public void OnGameReadyCycle()
        {
            MahjongGroups.OnReset();
            TableManager.OnReset();
            MahjongCtrl.OnReset();
        }

        public void OnReconnectedCycle()
        {
            MahjongGroups.OnReset();
            TableManager.OnReset();
            MahjongCtrl.OnReset();
        }

        public override void OnInitalization()
        {
            HandMahTouchEnable = true;
            GameCenter.RegisterCycle(this);
            MahjongGroups.OnInitalization();
            MahjongCamera.OnInitalization();
            TableManager.OnInitalization();
            MahjongCtrl.OnInitalization();
        }

        public void PlaybackRestart()
        {
            MahjongGroups.ResetMahjongGroup();
            TableManager.OnReset();
        }

        /// <summary>
        /// 播放3d特效
        /// </summary>  
        public void PlayPlayerEffect(int chair, PoolObjectType type)
        {
            Transform effectPos = MahjongGroups.EffectposGroup[chair];
            effectPos.Do((o) =>
            {
                string name = type.ToString();
                EffectObject obj = GameCenter.Pools.GetPool<ObjectPoolComponent>(PoolUitlity.Po_EffectObject)
                .Pop<EffectObject>(EffectObject.AssetsNamePrefix + name, (go) =>
                {
                    return go.Type == type;
                });
                if (obj != null)
                {
                    obj.ExSetParent(o);
                    obj.Execute();
                }
            });
        }

        /// <summary>
        /// 更新玩家出牌方位, 改变当前用户倒计时以及东西南北
        /// </summary>      
        public void ChangeDirection(int currOpSeat, int oldOpSeat)
        {
            MahjongGroups.PlayerToken = 0 == MahjongUtility.GetChair(currOpSeat);
            if (currOpSeat != oldOpSeat)
            {
                TableManager.SwitchDirection(currOpSeat);
                TableManager.StartTimer(GameCenter.DataCenter.Config.TimeOutcard);
            }
        }

        /// <summary>
        /// 设置骰子点数
        /// </summary>
        /// <param name="saiziPonit"></param>
        public void OnSetSaiziPoint(int[] saiziPonit)
        {
            MahjongGroups.SetCatchCardStartPos(saiziPonit);
            TableManager.PlaySaiziAnimation((byte)saiziPonit[0], (byte)saiziPonit[1]);
        }

        /// <summary>
        /// 设置发牌点和庄家方向
        /// </summary>
        public void OnSetBanker(int bankerSeat, int playerSeat)
        {
            MahjongGroups.SetSendCardSPos(bankerSeat, playerSeat);
            TableManager.SwitchDirection(playerSeat);
        }
    }
}