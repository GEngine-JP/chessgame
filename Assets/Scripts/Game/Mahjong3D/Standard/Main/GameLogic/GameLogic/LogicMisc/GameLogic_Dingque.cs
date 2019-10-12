using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [S2CResponseLogic]
    public partial class GameLogic_Dingque : AbsGameLogicBase
    {
        /// <summary>
        /// 定缺开始
        /// </summary>
        [S2CResponseHandler(NetworkProtocol.MJSelectColor, GameMisc.XzmjKey)]
        [S2CResponseHandler(NetworkProtocol.MJSelectColor, GameMisc.XlmjKey)]
        public void OnSelectDingqueStart(ISFSObject data)
        { 
            //开启托管权限
            GameCenter.Shortcuts.SwitchCombination.Close((int)GameSwitchType.PowerAiAgency);           
            //定缺时禁止出牌
            Game.MahjongGroups.PlayerToken = false;
            var mahHand = Game.MahjongGroups.PlayerHand;
            mahHand.SetHandCardState(HandcardStateTyps.Dingqueing);
            var xzMahHand = mahHand.GetMahHandComponent<MahPlayerHand_Xzmj>();
            GameCenter.EventHandle.Dispatch((int)UIEventProtocol.SetPlayerFlagState, new PlayerStateFlagArgs()
            {
                SecletColor = xzMahHand.LeastColor,
                CtrlState = true,
                StateFlagType = (int)PlayerStateFlagType.Selecting
            });
        }

        /// <summary>
        /// 定缺结束
        /// </summary>
        [S2CResponseHandler(NetworkProtocol.MJSelColorRst, GameMisc.XzmjKey)]
        [S2CResponseHandler(NetworkProtocol.MJSelColorRst, GameMisc.XlmjKey)]
        public void OnSelectDingqueEnd(ISFSObject data)
        {
            //停止定时任务
            GameCenter.Scene.TableManager.StopTimer();
            var eventManger = GameCenter.EventHandle;
            eventManger.Dispatch((int)UIEventProtocol.SetPlayerFlagState, new PlayerStateFlagArgs()
            {
                CtrlState = false
            });
            int[] colors = data.GetIntArray("color");
            eventManger.Dispatch((int)UIEventProtocol.ShowDingqueFlag, new HuanAndDqArgs()
            {
                Type = 0,
                DingqueColors = colors
            });
            //设置手牌断门
            int color = colors[DataCenter.OneselfData.Seat];
            Game.MahjongGroups.PlayerHand.SetHandCardState(HandcardStateTyps.DingqueOver, color);
            Game.MahjongGroups.PlayerToken = true;
            //是庄家，提示打牌
            if (DataCenter.BankerChair == 0)
            {
                var list = Game.MahjongGroups.PlayerHand.MahjongList;
                var item = list[list.Count - 1];
                Game.MahjongGroups.PlayerHand.SetLastCardPos(item);
                GameCenter.EventHandle.Dispatch((int)UIEventProtocol.TipBankerPutCard);
            }
            //开启托管权限
            GameCenter.Shortcuts.SwitchCombination.Open((int)GameSwitchType.PowerAiAgency);
        }
    }
}