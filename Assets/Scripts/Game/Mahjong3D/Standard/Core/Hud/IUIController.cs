namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public interface IUIController
    {
        void OnInit();

        void OnOperateUpdatePanel();

        void RefreshOtherPanelOnReconnected();

        void PlayUIEffect(PoolObjectType type);

        void PlayPlayerUIEffect(int chair, PoolObjectType type);

        T SetPanel<T>(T panel, UIPanelhierarchy hierarchy) where T : UIPanelBase;
    }
}
