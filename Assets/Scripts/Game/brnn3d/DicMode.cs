using UnityEngine;
using System.Collections;
using YxFramwork.Common;

namespace Assets.Scripts.Game.brnn3d
{
    public class DicMode : MonoBehaviour
    {
        public static DicMode Instance;
        public Transform DicTf;
        //点数
        int _pN = 0;
        protected void Awake()
        {
            Instance = this;
        }

        //玩骰子
        public void PlayDic()
        {
            _pN = App.GetGameData<GlobalData>().DicNum;
            if (DicTf.gameObject.activeSelf)
                DicTf.gameObject.SetActive(false);
            DicTf.gameObject.SetActive(true);
            ShowPNumber();
        }

        //显示骰子点数
        public void ShowPNumber()
        {
            switch (_pN)
            {
                case 1:
                    DicTf.localEulerAngles = new Vector3(0, 0, 0);
                    break;
                case 2:
                    DicTf.localEulerAngles = new Vector3(0, 0, 90);
                    break;
                case 3:
                    DicTf.localEulerAngles = new Vector3(-90, 0, 0);
                    break;
                case 4:
                    DicTf.localEulerAngles = new Vector3(0, 0, 180);
                    break;
                case 5:
                    DicTf.localEulerAngles = new Vector3(0, 0, 270);
                    break;
            }
        }

        //停止骰子
        public void StopDic()
        {
            StartCoroutine("HideDic", 2f);
        }

        IEnumerator HideDic(float s)
        {
            yield return new WaitForSeconds(s);
            if (DicTf.gameObject.activeSelf)
                DicTf.gameObject.SetActive(false);
        }
    }
}

