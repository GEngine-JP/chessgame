using Assets.Scripts.Game.ddz2.InheritCommon;
using UnityEngine;

namespace Assets.Scripts.Game.ddz2.DDz2Common
{
    public class MsgChatUiCtrl : MonoBehaviour
    {

        /// <summary>
        /// 父节点，用于隐藏整个聊天界面
        /// </summary>
        [SerializeField] 
        protected GameObject UiGob;
        
        /// <summary>
        /// 常用聊天信息原型
        /// </summary>
        [SerializeField]
        protected GameObject MsgLabelOrg;

        [SerializeField] 
        protected GameObject MsgGrid;

        [SerializeField]
        protected GameObject FaceGrid;
        /// <summary>
        /// 发送聊天按钮
        /// </summary>
        [SerializeField]
        protected GameObject SendTalkBtn;
        /// <summary>
        /// 发送俩天的label
        /// </summary>
        [SerializeField] 
        protected UILabel TapTalklabel;
        /// <summary>
        /// 常用语
        /// </summary>
        public readonly string[] TalkStrs =
        {
            "大家好,很高心见到各位",
            "和你合作真是太愉快了",
            "快点啊,等到花儿都都谢了",
            "你的牌打的也太好了",
            "不要吵了,不要吵了,专心玩游戏吧",
            "怎么又断线,网络怎么这么差啊",
            "不好意思,我要离开一会",
            "不要走,决战到天亮",
            "你是MM还是GG",
            "交个朋友吧,能告诉我联系方式吗",
            "再见了,我会想念大家的"
        };

        // Use this for initialization
        void Start ()
        {
            InitTalk();
            InitFace();
        }

        /// <summary>
        /// 初始化talk
        /// </summary>
        void InitTalk()
        {
            //常用聊天初始化
            var len = TalkStrs.Length;
            for (var i = 0; i < len; i++)
            {
                var gob = NGUITools.AddChild(MsgGrid, MsgLabelOrg);
                gob.SetActive(true);
                gob.GetComponent<UILabel>().text = TalkStrs[i];
                UIEventListener.Get(gob).onClick = OnClickTalk;
            }
            MsgGrid.GetComponent<UIGrid>().repositionNow = true;

        }

        /// <summary>
        /// 初始化face
        /// </summary>
        void InitFace()
        {
            //初始化face
            var faceTransForm = FaceGrid.transform;
            var faceCunt = faceTransForm.childCount;
            for (var i = 0; i < faceCunt; i++)
            {
                UIEventListener.Get(faceTransForm.GetChild(i).gameObject).onClick = OnClickExp;
            }
        }

        /// <summary>
        /// 当点击常用语聊天
        /// </summary>
        public void OnClickTalk(GameObject obj)
        {
            var label = obj.GetComponent<UILabel>();
            GlobalData.ServInstance.UserTalk(label.text);
            CloseChatUi();
        }

        /// <summary>
        /// 当点击发送聊天按钮
        /// </summary>
        public void OnClickSendTalk()
        {
            GlobalData.ServInstance.UserTalk(TapTalklabel.text);
            CloseChatUi();
        }

        /// <summary>
        /// 当点击表情
        /// </summary>
        /// <param name="obj"></param>
        public void OnClickExp(GameObject obj)
        {
            var expId = int.Parse(obj.name);
            GlobalData.ServInstance.UserTalk(expId);
            CloseChatUi();
        }


/*        /// <summary>
        /// 当程序切换时，显示离线，回来时恢复在线
        /// </summary>
        /// <param name="isPause"></param>
        public void OnApplicationPause(bool isPause)
        {
            GlobalData.ServInstance.UserTalk(isPause ? GlobalConstKey.AppPauseExpId : GlobalConstKey.AppComBackExpId);
        }*/

        /// <summary>
        /// 关闭聊天界面
        /// </summary>
        public void CloseChatUi()
        {
            UiGob.SetActive(false);
        }

        //打开聊天界面
        public void OpenChatUi()
        {
            UiGob.SetActive(true);
        }

    }
}
