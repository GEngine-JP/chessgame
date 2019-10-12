using YxFramwork.ConstDefine;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public partial class GameLogic_Cpg : AbsGameLogicBase
    {
        /// <summary>
        /// 赖子杠 
        /// </summary>     
        [S2CResponseHandler(NetworkProtocol.MJLaiZiGang, GameKey = GameMisc.QjmjKey)]
        public void OnLaiZiGang(ISFSObject data)
        {
            GameCenter.DataCenter.CurrOpSeat = data.TryGetInt(RequestKey.KeySeat);
            int chair = GameCenter.DataCenter.CurrOpChair;
            int card = data.TryGetInt(RequestKey.KeyCard);
            var item = SetLaiZiGangCard(chair, card);
            item.Laizi = MahjongUtility.MahjongFlagCheck(card);

            if (chair != 0)
            {
                //麻将记录
                GameCenter.Shortcuts.MahjongQuery.AddRecordMahjong(card);
            }
            else
            {
                DataCenter.Players[0].HardCards.Remove(card);
            }
        }

        private MahjongContainer SetLaiZiGangCard(int chair, int card)
        {
            var obj = Game.MahjongCtrl.PopMahjong(card);
            var item = obj.GetComponent<MahjongContainer>();
            Game.MahjongGroups.MahjongOther[chair].GetInMahjong(item);
            Game.MahjongGroups.MahjongHandWall[chair].ThrowOut(item.Value);
            obj.gameObject.SetActive(true);
            return item;
        }
    }
}
