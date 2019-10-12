using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.DataBundles;

namespace Assets.Scripts.Game.jh.ui
{
    public class JhTtResultItem : MonoBehaviour {

        public YxBaseTextureAdapter Head;
        public UILabel Name;
        public UISprite Icon;
        public UISprite LostIcon;
        public UILabel Gold;

        public UILabel WinCnt;

        public UILabel LostCnt;

        public UILabel Uid;
        public void SetInfo(string uname,string head,int sex,bool bigwinner,int gold,int wincnt,int lostcnt,int id)
        {
            if (Head != null)
            {
                PortraitDb.SetPortrait(head, Head, sex);
            }
            if (Name != null)
            {
                Name.text = uname;
            }
            if (Icon != null)
            {
                Icon.gameObject.SetActive(bigwinner);
            }
            if (LostIcon != null)
            {
                LostIcon.gameObject.SetActive(!bigwinner);
            }
            if (Gold != null)
            {
                Gold.text = "" + gold;
            }
            if (WinCnt != null)
            {
                WinCnt.text = "" + wincnt;
            }
            if (LostCnt != null)
            {
                LostCnt.text = "" + lostcnt;
            }
            if (Uid != null)
            {
                Uid.text = "" + id;
            }
            
        }

    }
}
