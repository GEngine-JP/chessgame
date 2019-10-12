using YxFramwork.View;

namespace Assets.Scripts.Common.Views
{
    /// <summary>
    /// ÄÚÇ¶ÍøÒ³Ãæ°å
    /// </summary>
    public class WebPanel : YxBasePanel
    {
        public string Url;

        protected override void OnAwake()
        {
            base.OnAwake();
            if (Data == null)
            {
                Data = Url;
            }
        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
            if (Data == null) return;
            var url = Data.ToString();
        }
    }
}
