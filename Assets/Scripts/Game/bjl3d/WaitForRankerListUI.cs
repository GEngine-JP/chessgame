using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.bjl3d
{

    public class WaitForRankerListUI : MonoBehaviour
    {
        public static WaitForRankerListUI Instance;

        public Transform RankerListDemo;

        /// <summary>
        /// 判断上下庄true=》可以上庄 false=》可以下庄
        /// </summary>
        public bool IsApplyRankerOrXiaRanker = false;

        private Animator ani;

        public Sprite szSprite;
        public Sprite xzSprite;

        public Image buttonImage;

        /// <summary>
        /// 获取UI操作控件
        /// </summary>
        private void Awake()
        {
            Instance = this;
        }

        //清理上下庄列表UI
        public void RankerListData()
        {
            foreach (Transform t in RankerListDemo.parent)
            {
                if (t == RankerListDemo) continue;
                Destroy(t.gameObject);
            }
        }
        /// <summary>
        /// UI显示相关函数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="money"></param>
        public void ShowRankerListUI(string name, string money)
        {
            GameObject item = Instantiate(RankerListDemo.gameObject);
            item.transform.FindChild("Name").GetComponent<Text>().text = name;
            item.transform.FindChild("Money").GetComponent<Text>().text = money;
            item.transform.parent = RankerListDemo.parent;
            item.transform.localPosition = new Vector3(item.transform.localPosition.x, item.transform.localPosition.y, 0);
            item.transform.localScale = Vector3.one;

            item.SetActive(true);
        }

        private void ShowApplyBtn(bool isShowApplyBtn)
        {
            CountDownUI.Instance.ShowS_X_Image(isShowApplyBtn);
        }
    }


}