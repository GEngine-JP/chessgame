namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public interface IMahHandCtrl
    {
        IMahHand OnIni();
    }

    public interface IMahPlayerHandCtrl
    {
        IMahPlayerHand OnIni();
    }

    public interface IMahHand
    {
        bool SetHandCardState(HandcardStateTyps state, params object[] args);
        MahjongContainer GetMahjongItemByValue(int value);
        void AddMahjong(MahjongContainer item);
        void SortMahjongForHand();
        void SortMahjong();
        void Reset();
    }

    public interface IMahPlayerHand : IMahHand
    {
        void SetMahjongNormalState(MahjongContainer item);
        MahjongContainer SetTingPaiNeedOutCard();
        void GetInMahjong(MahjongContainer item);
    }
}