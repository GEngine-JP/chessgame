using YxFramwork.ConstDefine;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    //麻将有旋转，z轴控制了上下移动，并且z变小的时候，麻将上升
    public class MouseRoll : MonoBehaviour
    {
        public Transform Target;
        private MahjongContainer mContainer;

        private void Awake()
        {
            mContainer = GetComponent<MahjongContainer>();
            Target = mContainer.transform;
        }

        public void RollUp()
        {
            if (null != mContainer && mContainer.IsTingCard)
            {
                GameCenter.Network.OnRequestC2S((sfs) =>
                {
                    sfs.PutInt(RequestKey.KeyType, NetworkProtocol.MJRequestTypeGetHuCards);
                    sfs.PutInt("card", mContainer.Value);
                    if (GameCenter.DataCenter.Config.QueryTingInRate)
                    {
                        sfs.PutBool("rate", true);
                    }
                    return sfs;
                });
            }
            mContainer.Tweener.ActionMahRise(0.02f);
            GameCenter.Scene.MahjongGroups.OnFlagMahjong(mContainer.Value);
        }

        public void RollDown()
        {
            mContainer.Tweener.ActionMahDropDown(0.02f);
            GameCenter.Scene.MahjongGroups.OnClearFlagMahjong();
            //关闭查听
            GameCenter.EventHandle.Dispatch((int)UIEventProtocol.QueryHuCard, new QueryHuArgs() { PanelState = false });
        }

        public void ResetPos()
        {
            mContainer.Tweener.ActionMahDropDown(0.02f);
            GameCenter.Scene.MahjongGroups.OnClearFlagMahjong();
        }
    }
}