using UnityEngine;
using YxFramwork.Manager;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.bjl3d
{
    public class CoinTypeInfoUI : MonoBehaviour//G 11.15
    {
        public static CoinTypeInfoUI Intance;

        protected void Awake()
        {
            Intance = this;
        }

        public void SelectClickeCoinTypeAudio(int btnIndex)
        {
            for (int i = 0; i < 7; i++)
            {
                Transform tf = transform.Find("CoinTypeBtn" + i + "/effect");
                if (tf == null)
                    YxDebug.LogError("No Such Compent");
                if (i == btnIndex)
                {
                    MusicManager.Instance.Play("movebutton");
                    //                    AudioClip clip = ResourcesLoader.instance.LoadAudio("music/movebutton");
                    //                    AudioManager.Instance.Play(clip, false, .8f);
                    if (tf != null) tf.gameObject.SetActive(true);
                }
                else
                {
                    if (tf != null) tf.gameObject.SetActive(false);
                }
            }
        }

        public void CoinTypeBtn_0()
        {
            UserInfoUI.Instance.GameConfig.CoinType = 0;
            SelectClickeCoinTypeAudio(0);
        }
        public void CoinTypeBtn_1()
        {
            UserInfoUI.Instance.GameConfig.CoinType = 1;
            SelectClickeCoinTypeAudio(1);
        }
        public void CoinTypeBtn_2()
        {
            UserInfoUI.Instance.GameConfig.CoinType = 2;
            SelectClickeCoinTypeAudio(2);
        }
        public void CoinTypeBtn_3()
        {
            UserInfoUI.Instance.GameConfig.CoinType = 3;
            SelectClickeCoinTypeAudio(3);
        }
        public void CoinTypeBtn_4()
        {
            UserInfoUI.Instance.GameConfig.CoinType = 4;
            SelectClickeCoinTypeAudio(4);
        }
        public void CoinTypeBtn_5()
        {
            UserInfoUI.Instance.GameConfig.CoinType = 5;
            SelectClickeCoinTypeAudio(5);
        }
        public void CoinTypeBtn_6()
        {
            UserInfoUI.Instance.GameConfig.CoinType = 6;
            SelectClickeCoinTypeAudio(6);
        }
    }
}