using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class TypeBinderManager : MonoBehaviour
    {
        protected Dictionary<Type, Type> mSuppliers = new Dictionary<Type, Type>();

        /// <summary>
        /// 根据绑定类型获取实例
        /// </summary>
        /// <typeparam name="TInject">注入类型</typeparam>
        /// <param name="binderType">绑定类型</param>
        /// <returns></returns>
        public TInject GetInstance<TInject>(Type binderType) where TInject : class
        {
            Type injectType;
            if (mSuppliers.TryGetValue(binderType, out injectType))
            {
                var instance = Activator.CreateInstance(injectType) as TInject;
                if (instance != null) return instance;
            }
            return null;
        }

        /// <summary>
        /// 类型绑定
        /// </summary>
        protected void Binder<TBinder, TInjector>()
        {
            var binderType = typeof(TBinder);
            var injectorType = typeof(TInjector);
            if (!mSuppliers.ContainsKey(binderType))
            {
                mSuppliers[binderType] = injectorType;
            }
        }

        /// <summary>
        /// 解除绑定类型
        /// </summary>
        protected void RemoveBinder<TBinder>()
        {
            var binderType = typeof(TBinder);
            if (mSuppliers.ContainsKey(binderType))
            {
                mSuppliers.Remove(binderType);
            }
        }

        public virtual List<Type> CollectionGameLogicType()
        {
            //S2C 命令注册
            //Binder<CommandThrowoutCard, ActionThrowoutCard>();           
            //C2S 命令注册
            //Binder<CommandOperateRequest, ActionOperateRequest>();          
            //玩家数据
            //Binder<MahjongGameData, MahjongUserInfo>();
            //return mSuppliers.Keys.ToList();
            return null;
        }

        /// <summary>
        /// All Panel
        /// </summary>
        /// <returns></returns>
        public virtual List<Type> AllPanelTypes()
        {
            return new List<Type>()
            {
                typeof(PanelChooseOperate),
                typeof(PanelExhibition),
                typeof(PanelGameExplain),
                typeof(PanelGameInfo),
                typeof(PanelGameRule),
                typeof(PanelGameTriggers),
                typeof(PanelGM),
                typeof(PanelHandup),
                typeof(PanelHuanAndDq),
                typeof(PanelInviteFriends),
                typeof(PanelOpreateMenu),
                typeof(PanelPlayersInfo),
                typeof(PanelQueryHuCard),
                typeof(PanelScoreDouble),
                typeof(PanelPlayback),
                typeof(PanelSetting),
                typeof(PanelSingleResult),
                typeof(PanelTitleMessage),
                typeof(PanelTotalResult),
                typeof(PanelPlaybackResult),
                typeof(PanelZhaniao),
                typeof(PanelLiangdao),
                typeof(PanelOtherHuTip),
            };
        }

        /// <summary>
        /// 默认显示panel
        /// </summary>
        public virtual List<Type> DefaultShowPanelTypes()
        {
            var list = new List<Type>()  {
                typeof(PanelGM),
                typeof(PanelGameInfo),
                typeof(PanelPlayersInfo),
                typeof(PanelGameTriggers),
                typeof(PanelGameRule),
            };

            string gameKey = App.GameKey;
            //血流血战麻将 添加换张定缺panel
            if (gameKey.Equals(GameMisc.XlmjKey) || gameKey.Equals(GameMisc.XzmjKey))
            {
                list.Add(typeof(PanelHuanAndDq));
            }

            return list;
        }

        #region 回放
        /// <summary>
        /// 收集回放逻辑类型
        /// </summary>
        /// <returns></returns>
        public virtual List<Type> PlaybackLogicTypes()
        {
            Binder<CommandThrowoutCard, ActionThrowoutCard>();
            Binder<CommandCpgPlayback, ActionCpgPlayback>();
            Binder<CommandHuPlayback, ActionHuPlayback>();
            Binder<CommandSendCard, ActionSendCard>();
            Binder<CommandGetCard, ActionGetCard>();
            Binder<CommandLaizi, ActionLaizi>();

            return mSuppliers.Keys.ToList();
        }

        /// <summary>
        /// 收集回放逻辑类型
        /// </summary>
        /// <returns></returns>
        public virtual List<Type> PlaybackShowPanelTypes()
        {
            return new List<Type>()
            {
                typeof(PanelPlayback),
                typeof(PanelPlayersInfo)
            };
        }
        #endregion

        /// <summary>
        /// 扩展工具收集
        /// </summary>
        public virtual List<Type> ExtensionToolCollection()
        {
            return new List<Type>()
            {
                typeof(AiAgency),
                typeof(MahjongQuery),
            };
        }
    }
}
