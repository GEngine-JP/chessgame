﻿using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public partial class GameLogic_Reconnect : AbsGameLogicBase
    {
        [S2CResponseHandler(CustomClientProtocol.CustomTypeReconnectLogic, GameMisc.QdjtKey)]
        public void OnReconnect_Qdjt(ISFSObject data)
        {
            //更新牌
            UpdateMahjongGroup();
            //更新牌桌
            UpdateMahjongTable();
            MahjongQueryHuTip();
            //开启允许托管权限
            GameCenter.Shortcuts.SwitchCombination.Open((int)GameSwitchType.PowerAiAgency);

            MahjongUserInfo userInfo;
            var groups = GameCenter.Scene.MahjongGroups;
            var panel = GameCenter.Hud.GetPanel<PanelOtherHuTip>();
            for (int i = 0; i < DataCenter.MaxPlayerCount; i++)
            {
                userInfo = DataCenter.Players[i];
                int[] liangCards = userInfo.ExtData.Get<VarIntArray>("liangdaoCards");
                if (liangCards == null || liangCards.Length < 1) continue;
                groups.MahjongHandWall[i].SetHandCardState(HandcardStateTyps.TingAndShowCard, liangCards);

                //UI显示胡牌            
                var chair = MahjongUtility.GetChair(userInfo.Seat);
                panel.Open(liangCards, chair);
            }

            //设置手牌状态
            SetReconectCardState();
        }
    }
}
