using System;
using UnityEngine;
using UnityEngine.UI;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public partial class GameLogic_Reconnect : AbsGameLogicBase
    {
        [S2CResponseHandler(CustomClientProtocol.CustomTypeReconnectLogic, GameMisc.NamjKey)]
        public void OnReconnect_Namj(ISFSObject data)
        {
            OnReconnect(data);
            MahjongUserInfo userInfo;
            for (int i = 0; i < DataCenter.MaxPlayerCount; i++)
            {
                userInfo = DataCenter.Players[i];
                var extData = userInfo.ExtData;
                VarInt value;
                if (extData.TryGetValue("guoval", out value))
                {
                    int[] array = new int[2] { value, value };
                    GameCenter.Scene.MahjongGroups.MahjongHandWall[i].SetHandCardState(HandcardStateTyps.TingAndShowCard, array);
                }
            }
        }

        [S2CResponseHandler(CustomClientProtocol.CustomTypeReconnectLogic, GameMisc.DsqmjKey)]
        [S2CResponseHandler(CustomClientProtocol.CustomTypeReconnectLogic, GameMisc.YkmjKey)]
        public void OnReconnect_Ykmj(ISFSObject data)
        {
            OnReconnect(data);
            Action<ISFSObject> handler = null;
            handler = GameCenter.Network.DispatchResponseHandlers(CustomClientProtocol.CustomTypeCustomLogic);
            if (null != handler)
            {
                handler(data);
            }
        }

        [S2CResponseHandler(CustomClientProtocol.CustomTypeReconnectLogic, GameMisc.QjmjKey)]
        [S2CResponseHandler(CustomClientProtocol.CustomTypeReconnectLogic, GameMisc.MtfmjKey)]
        [S2CResponseHandler(CustomClientProtocol.CustomTypeReconnectLogic, GameMisc.QyqhhmjKey)]
        public void OnReconnect_Qjmj(ISFSObject data)
        {
            OnReconnect(data);
            MahjongUserInfo userInfo;
            var groups = Game.MahjongGroups;
            for (int i = 0; i < DataCenter.MaxPlayerCount; i++)
            {
                userInfo = DataCenter.Players[i];
                int[] lzGang = userInfo.ExtData.Get<VarIntArray>("laiziGang");
                if (lzGang == null || lzGang.Length < 1) continue;
                for (int j = 0; j < lzGang.Length; j++)
                {
                    groups.PopMahFromCurrWall();
                    groups.MahjongOther[i].GetInMahjong(lzGang[j]).Laizi = MahjongUtility.MahjongFlagCheck(lzGang[j]);
                }
                //麻将记录
                GameCenter.Shortcuts.MahjongQuery.Do(p => p.AddRecordMahjongs(lzGang));
            }
        }

        [S2CResponseHandler(CustomClientProtocol.CustomTypeReconnectLogic, GameMisc.ZhmjKey)]
        [S2CResponseHandler(CustomClientProtocol.CustomTypeReconnectLogic, GameMisc.DlmjKey)]
        [S2CResponseHandler(CustomClientProtocol.CustomTypeReconnectLogic, GameMisc.DltdhKey)]
        [S2CResponseHandler(CustomClientProtocol.CustomTypeReconnectLogic, GameMisc.TdDltdhKey)]
        public void OnReconnect_Dlmj(ISFSObject data)
        {
            //更新牌
            UpdateMahjongGroup();
            //更新牌桌
            UpdateMahjongTable();
            MahjongQueryHuTip();
            //开启允许托管权限
            GameCenter.Shortcuts.SwitchCombination.Open((int)GameSwitchType.PowerAiAgency);
            MahjongUserInfo userInfo;
            var groups = Game.MahjongGroups;
            for (int i = 0; i < DataCenter.MaxPlayerCount; i++)
            {
                userInfo = DataCenter.Players[i];
                int[] nius = userInfo.ExtData.Get<VarIntArray>("nius");
                if (nius == null || nius.Length < 1) continue;
                groups.MahjongHandWall[i].SetHandCardState(HandcardStateTyps.TingAndShowCard, nius);
            }
            //设置手牌状态
            SetReconectCardState();
        }

        [S2CResponseHandler(CustomClientProtocol.CustomTypeReconnectLogic, GameMisc.WmbbmjKey)]
        public void OnReconnect_Wmbbmj(ISFSObject data)
        {
            OnReconnect(data);

            var arr = data.TryGetIntArray("caipiao");
            if (arr != null)
            {
                var panel = GameCenter.Hud.GetPanel<PanelPlayersInfo>();
                var go = GameUtils.GetAssets<GameObject>("Caipiao");
                var sprite = go.GetComponent<Image>().sprite;

                var tempArr = new int[arr.Length];
                for (int i = 0; i < arr.Length; i++)
                {
                    var chair = MahjongUtility.GetChair(i);
                    panel[chair].SetHeadOtherImage(arr[i] > 0, sprite);
                    tempArr[chair] = arr[i];
                }

                var throwoutCard = GameCenter.Network.GetGameResponseLogic<GameLogic_ThrowoutCard>();
                throwoutCard.SetCaipiaoChairs(tempArr);
                var flag = throwoutCard.SetCaipiaoState();
                if (flag)
                {
                    var handWall = Game.MahjongGroups.MahjongHandWall[0];
                    var item = handWall.GetLastMj();
                    var playerHand = handWall.GetMahHandComponent<MahPlayerHand>();
                    playerHand.SetMahjongNormalState(item);
                }
            }
        }
    }
}