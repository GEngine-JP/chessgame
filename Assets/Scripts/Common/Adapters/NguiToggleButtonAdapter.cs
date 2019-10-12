using YxFramwork.Common.Adapters;
using YxFramwork.Enums;

namespace Assets.Scripts.Common.Adapters
{
    public class NguiToggleButtonAdapter : YxBaseButtonAdapter
    {
        private NguiToggleAdapter _toggle;
        protected NguiToggleAdapter Toggle
        {
            get
            {
                if (_toggle == null)
                {
                    _toggle = GetComponent<NguiToggleAdapter>();
                }
                return _toggle;
            }
        }

        public override YxEUIType UIType
        {
            get { return YxEUIType.Nguid; }
        }

        public override bool SetSkinName(string skinName)
        {
            var toggle = Toggle;
            return toggle.SetSkinName(skinName);
        }

        public override void SetLabel(string content)
        {
            var toggle = Toggle;
            toggle.SetLabel(content);
        }
    }
}
