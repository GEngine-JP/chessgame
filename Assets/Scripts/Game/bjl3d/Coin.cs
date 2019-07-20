using UnityEngine;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.bjl3d
{
    public class Coin : MonoBehaviour
    {
        private int areaID;

        public static Coin GetInstance(int num, Vector3 pos)
        {
            //Transform obj = ResourcesLoader.instance.Load("coin" + type, GameScene.Instance.CoinList).transform;
            var go = ResourceManager.LoadAsset("coin" + num).transform;
            var go1 = Instantiate(go);
            Transform obj = go1.transform;
            if (obj == null) return null;
            obj.gameObject.SetActive(true);
            obj.position = pos;
            Coin coin = obj.GetComponent<Coin>();
        
            return coin;
        }

        public void Init(int iAreaId)
        {
            areaID = iAreaId;
        }

        void ItweenAnimation()
        {
            Destroy(this.gameObject);
        }
    }

}