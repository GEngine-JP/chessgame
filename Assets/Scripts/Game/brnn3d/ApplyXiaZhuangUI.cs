using UnityEngine;

namespace Assets.Scripts.Game.brnn3d
{
    public class ApplyXiaZhuangUI : MonoBehaviour
    {
        public static ApplyXiaZhuangUI Instance;

        public Transform ApplyZhuangtf;
        public Transform XiaZhuangtf;
        public void Awake()
        {
            Instance = this;
        }

        //显示上庄的按钮
        public void ShowApplyZhuang()
        {
            if (!ApplyZhuangtf.gameObject.activeSelf)
                ApplyZhuangtf.gameObject.SetActive(true);
            if (XiaZhuangtf.gameObject.activeSelf)
                XiaZhuangtf.gameObject.SetActive(false);
        }

        //显示下庄的按钮
        public void ShowXiaZhuang()
        {
            if (ApplyZhuangtf.gameObject.activeSelf)
                ApplyZhuangtf.gameObject.SetActive(false);
            if (!XiaZhuangtf.gameObject.activeSelf)
                XiaZhuangtf.gameObject.SetActive(true);
        }

    }
}

