using UnityEngine;
using System.Collections.Generic;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.bjl3d
{
    public class Plan : MonoBehaviour
    {
        public static Plan Instance;
        protected void Awake()
        {
            Instance = this;
        }
        public Dictionary<int, int> CoinDic = new Dictionary<int, int>();

        public void XiaZhuChouMaXianShi(int zhuAreaId, int areaId)
        {
            if (zhuAreaId < 0 || zhuAreaId > 7) return;
            Transform obj = Instantiate(GameScene.Instance.ZhuMoveDemos[zhuAreaId - 1]);

            if (obj == null) return;
            Transform tf = transform.FindChild("Bet_" + zhuAreaId);
            if (tf == null) return;
            Animator ani = obj.GetComponent<Animator>();
            if (ani == null)
                YxDebug.LogError("No Such Animator");
            if (ani != null) ani.enabled = false;
            obj.gameObject.SetActive(true);
            obj.parent = tf.parent;
            obj.localEulerAngles = new Vector3(0, 0, 0);
            if (areaId == 7)
                obj.localScale = new Vector3(0.35f, 0.4f, 0.5f);
            else if (areaId == 6)
                obj.localScale = new Vector3(0.4f, 0.4f, 0.5f);
            else if (areaId == 1)
                obj.localScale = new Vector3(0.3f, 0.5f, 0.8f);
            else
                obj.localScale = new Vector3(0.3f, 0.4f, 0.6f);
            if (CoinDic.ContainsKey(zhuAreaId))
            {
                obj.localPosition = tf.localPosition + new Vector3(0f, 0.2f * CoinDic[zhuAreaId], 0f);
                CoinDic[zhuAreaId] += 1;
            }
            else
            {
                CoinDic.Add(zhuAreaId, 1);
                obj.localPosition = tf.localPosition + new Vector3(0f, 0f, 0f);
            }
        }

    }
}
