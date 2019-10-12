namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    /// <summary>
    /// 事件分发器参数
    /// </summary>
    public class EvtHandlerArgs : EvtArgs { }

    /// <summary>
    /// 事件分发组件
    /// </summary>
    public class EventHandlerComponent : BaseComponent
    {
        private EventHandlerSystem<EvtHandlerArgs> EventComponent = new EventHandlerSystem<EvtHandlerArgs>();

        public void Subscriber(int key, EvtHandler<EvtHandlerArgs> action)
        {
            EventComponent.Subscriber(key, action);
        }

        public void Unsubscriber(int key, EvtHandler<EvtHandlerArgs> action)
        {
            EventComponent.Unsubscriber(key, action);
        }

        public void Dispatch(int key, EvtHandlerArgs args = null)
        {
            EventComponent.Dispatch(key, args);
        }
    }
}