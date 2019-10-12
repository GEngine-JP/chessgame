using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.DataBundles;

namespace Assets.Scripts.Game.jh.ui.plen_8
{
    public class JhPkHeadInfo : MonoBehaviour {

        public YxBaseTextureAdapter Head;

        public UILabel Name;

        public UILabel Gold;

        public GameObject WinLostAnimation;

        public void SetInfo(Texture getTexture, string value, string s)
        {
            Head.SetTexture(getTexture);
            Name.text = value;
            Gold.text = "" + s;
        }
    }
}
