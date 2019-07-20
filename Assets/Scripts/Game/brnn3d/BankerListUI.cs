using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.brnn3d
{
    public class BankerListUI : MonoBehaviour
    {
        public static BankerListUI Instance;
        protected void Awake()
        {
            Instance = this;
        }
        public Transform BankerItem;

        //清理上下庄列表UI
        public void DeleteBankerListUI()
        {
            foreach (Transform t in BankerItem.parent)
            {
                if (t == BankerItem) continue;
                Destroy(t.gameObject);
            }
        }

        //设置庄家列表的UI
        public void SetBankerListUI(string name, string money)
        {
            GameObject item = Instantiate(BankerItem.gameObject);
            item.transform.FindChild("Name").GetComponent<Text>().text = name;
            item.transform.FindChild("Money").GetComponent<Text>().text = money;
            item.transform.parent = BankerItem.parent;
            item.transform.localPosition = new Vector3(item.transform.localPosition.x, item.transform.localPosition.y, 0);
            item.transform.localScale = Vector3.one;

            item.SetActive(true);
        }

    }
}

