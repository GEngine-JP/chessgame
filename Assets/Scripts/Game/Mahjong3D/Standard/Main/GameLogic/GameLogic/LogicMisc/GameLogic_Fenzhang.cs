using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public partial class GameLogic_Hu : AbsGameLogicBase
    {
        [S2CResponseHandler(NetworkProtocol.MJRequestFenZhang)]
        public void OnFenzhang(ISFSObject data)
        {
            if (data.ContainsKey("cards"))
            {
                DataCenter.Game.FenzhangFlag = true;
                var arr = data.GetSFSArray("cards");
                DataCenter.LeaveMahjongCnt -= arr.Count;
                for (int i = 0; i < arr.Count; i++)
                {
                    var obj = arr.GetSFSObject(i);
                    if (obj != null)
                    {
                        var value = obj.GetInt("card");
                        var chair = MahjongUtility.GetChair(obj.GetInt("seat"));
                        if (value > 0)
                        {
                            Game.MahjongGroups.CurrGetMahjongWall.PopMahjong();
                            var temp = Game.MahjongCtrl.PopMahjong(value);
                            if (temp != null)
                            {
                                var mahjong = temp.GetComponent<MahjongContainer>();
                                mahjong.Laizi = MahjongUtility.MahjongFlagCheck(value);
                                Game.MahjongGroups.MahjongOther[chair].GetInMahjong(mahjong);
                            }
                        }
                        DataCenter.Players[chair].FenzhangCard = value;
                    }
                }
                //隐藏箭头
                Game.TableManager.HideOutcardFlag();
            }
        }

        [S2CResponseHandler(NetworkProtocol.MJRequestFenZhang, GameKey = GameMisc.DsqmjKey)]
        [S2CResponseHandler(NetworkProtocol.MJRequestFenZhang, GameKey = GameMisc.DbsmjKey)]
        public void OnFenzhang_Dbsmj(ISFSObject data)
        {
            if (data.ContainsKey("cards"))
            {
                var value = 0;
                int chair = 0;
                DataCenter.Game.FenzhangFlag = true;
                int[] cardsBySeat = data.GetIntArray("cards");
                for (int i = 0; i < cardsBySeat.Length; i++)
                {
                    chair = MahjongUtility.GetChair(i);
                    value = cardsBySeat[i];
                    if (value > 0)
                    {
                        Game.MahjongGroups.CurrGetMahjongWall.PopMahjong();
                        var temp = Game.MahjongCtrl.PopMahjong(value);
                        if (temp != null)
                        {
                            var mahjong = temp.GetComponent<MahjongContainer>();
                            mahjong.Laizi = MahjongUtility.MahjongFlagCheck(value);
                            Game.MahjongGroups.MahjongOther[chair].GetInMahjong(mahjong);
                        }
                    }
                    DataCenter.Players[chair].FenzhangCard = value;
                }
                //隐藏箭头
                Game.TableManager.HideOutcardFlag();
            }
        }
    }
}