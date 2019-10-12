//using Sfs2X.Entities.Data;
//namespace Assets.Scripts.Game.Mahjong3D.Standard
//{
//    [S2CResponseLogic]
//    public class GameLogic_Ready : AbsGameLogicBase
//    {
//        [S2CResponseHandler(CustomClientProtocol.CustomTypeReadyLogic, GameMisc.BbmjKey)]
//        public void OnReady_Bbmj(ISFSObject data)
//        {
//            if (DataCenter.Room.CurrRound == 1 && DataCenter.MaxPlayerCount != DataCenter.Players.CurrPlayerCount)
//            {
//                var args = new ShowTitleMessageArgs()
//                {
//                    TitleType = (int)TitleMessageType.BbmjTip
//                };
//                GameCenter.EventHandle.Dispatch((int)UIEventProtocol.ShowTitleMessage, args);
//            }
//        }
//    }
//}