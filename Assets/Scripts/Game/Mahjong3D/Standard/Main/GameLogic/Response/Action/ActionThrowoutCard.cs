namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class ActionThrowoutCard : AbsCommandAction
    {
        public void PlaybackThrowoutCard(PlaybackFrameData data)
        {
            var card = data.Cards[0];
            var seat = data.OpChair;
            Game.MahjongGroups.MahjongHandWall[seat].ThrowOut(card);

            var item = Game.MahjongGroups.MahjongThrow[seat].GetInMahjong(card);
            Game.TableManager.ShowOutcardFlag(item);

            GameCenter.Controller.PlaybackPlaySound(card);
        }
    }
}
