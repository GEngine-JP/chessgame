namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public abstract class AbsGameLogicBase
    {
        private MahjongSceneComponent mGame;
        private DataCenterComponent mDataCenter;

        public virtual void OnInit() { }
        public virtual void OnReset() { }
       
        protected MahjongLocalConfig Config { get { return DataCenter.Config; } }
        protected MahjongConfigData ConfigData { get { return DataCenter.ConfigData; } }
        protected MahjongSceneComponent Game { get { if (null == mGame) { mGame = GameCenter.Scene; } return mGame; } }
        protected DataCenterComponent DataCenter { get { if (null == mDataCenter) { mDataCenter = GameCenter.DataCenter; } return mDataCenter; } }
    }

    public interface IData
    {
        void SetData(Sfs2X.Entities.Data.ISFSObject data);
    }
}