using UnityEngine;
namespace Assets.Scripts.Game.ddz2.DDzGameListener.InfoPanel
{
    public class UserInfoDetail : ServEvtListener
    {
        [SerializeField]
        private UILabel UserIP;
        [SerializeField]
        private UILabel UserID;
        [SerializeField]
        private UILabel UserName;
        [SerializeField]
        private UITexture _userHead;
        public GameObject ShowParent;

        protected override void OnAwake()
        {
            if (ShowParent)
            {
                ShowParent.SetActive(false);
            }
        }

        public void OnClickDisDetail()
        {
            ShowParent.SetActive(false);
        }
        /// <summary>
        /// 显示信息
        /// </summary>
        /// <param name="name">玩家名称</param>
        /// <param name="userid">玩家id</param>
        /// <param name="ip">玩家ip</param>
        public void ShowInfo(string name,string userid,string ip,UITexture head)
        {
            ShowParent.SetActive(true);
            UserName.text = name;
            UserID.text = userid;
            UserIP.text = ip;
            _userHead.mainTexture = head.mainTexture;
        }
        /// <summary>
        /// 
        /// </summary>
        public override void RefreshUiInfo()
        {
        }
    }
}
