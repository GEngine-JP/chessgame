using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Manager;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.Shuihuzhuan.Scripts
{
    public class LittleMaryControl : MonoBehaviour
    {
        public static LittleMaryControl Instance;
        /// <summary>
        /// 跑马的24个图片
        /// </summary>
        public Transform[] paoma;
        /// <summary>
        /// 四个图片
        /// </summary>
        public Image[] images;
        /// <summary>
        /// 次数
        /// </summary>
        public Text remainText;

        public Text myMoneyText;

        public Text winMoneyText;

        public Text betNumText;
        /// <summary>
        /// 玛丽
        /// </summary>
        public GameObject maryPanel;
        /// <summary>
        /// 比倍和得分
        /// </summary>
        public GameObject quitPanel;
        /// <summary>
        /// 接受服务器图片
        /// </summary>
        private int[] imageArray;

        private int curretImg = 0;

        private float curretTimer = 0f;

        private int nImg = 0;

        private float MarqueeInterval = .03f;
        /// <summary>
        /// 马力次数
        /// </summary>
        private int turnNum = 0;

        private int resultNum = 80;

        private int result = 24;

        private bool canMove = false;

        public void Awake()
        {
            Instance = this;

        }

        public void OpenMaryPanel()
        {
            maryPanel.SetActive(true);
        }
        public void CloseMaryPanel()
        {
            quitPanel.SetActive(false);
            maryPanel.SetActive(false);
        }

        public void GetMoneyBtnFun() //得分
        {
            result = 24;

            App.GetGameData<GlobalData>().iWinMoney = 0;
            if (turnNum == 0)//没有玛丽次数
            {   GameStateUiControl .instance .LostWait();
               BottomUIControl.instance.TheincomeMali();
                CloseMaryPanel();
                WinPanelControl.instance.winPanel.SetActive(false);
            }
            else//还有马力次数
            {
                App.GetGameData<GlobalData>().isMary = true;
                myMoneyText.text = App.GetGameData<GlobalData>().iMainMoney.ToString();
                winMoneyText.text = App.GetGameData<GlobalData>().iWinMoney.ToString();
                GameServer.Instance.MaLiFun();
                quitPanel.SetActive(false);
            }
        }
    
        public void BiBeiBtnFun()//比倍
        {
            App.GetGameData<GlobalData>().Malizhuantai = true;
            result = 24;
            BottomUIControl.instance.BigSmallBtnFun();
            CloseMaryPanel();
        }
        public void MaryResultFun()
        {
            RefeshTexts();
            turnNum = App.GetGameData<GlobalData>().iMaliGames ;
            result = App.GetGameData<GlobalData>().iMaliZhuanImage;//转的图片
            resultNum = App.GetGameData<GlobalData>().iMaliZhuanImage + 48;
            App.GetGameData<GlobalData>().isMary = true;

            if (maryPanel.activeSelf == false)
            {
                OpenMaryPanel();
            }
            for (int i = 0; i < images.Length; i++)
            {
                images[i].sprite = TurnControl.instance.cardSprites[App.GetGameData<GlobalData>().iMaliImage[i]];
            }
            canMove = true;          
            remainText.text = (turnNum).ToString();

        }

      

        public void RefeshTexts()
        {

            myMoneyText.text = App.GetGameData<GlobalData>().MainMoney .ToString();
            winMoneyText.text = App.GetGameData<GlobalData>().MaliWinMony .ToString();
            betNumText.text = App.GetGameData<GlobalData>().BetNum.ToString();
        }

        public IEnumerator TestFun()
        {
            if (App.GetGameData<GlobalData>().isMary)//有马力
            {
                App.GetGameData<GlobalData>().isMary = false ;
            }
           
            if (result == 0 || result == 6 || result == 12 || result == 18)
            {  yield return new WaitForSeconds(2);
               quitPanel.SetActive(true);
               App.GetGameData<GlobalData>().isMary =false ;
            }
            yield return new WaitForSeconds(3);
            if (result !=0 && result != 6&& result != 12 && result != 18)
            {
                Debug.Log("发送数据！");
                GameServer.Instance.MaLiFun();
            }

            

        }
        public void ClearPaomadeng()
        {
            for (int i = 0; i < paoma.Length; i++)
            {
                paoma[i].gameObject.SetActive(false);
            }
        }
        private void HidePaoma(int curtImg)
        {
            paoma[curtImg].gameObject.SetActive(false);
        }
        void Paoma(int curtImg)
        {
            MusicManager.Instance.Play("gundong");
            paoma[curtImg].gameObject.SetActive(true);
            int temp = curtImg == 0 ? paoma.Length - 1 : curtImg - 1;
            StartCoroutine("HidePaoma", temp);
        }

        private float addTime = 0f;
        void Update()
        {
            curretTimer += Time.deltaTime;
            if (curretTimer > MarqueeInterval && canMove == true)
            {
                curretImg = nImg % 24;
                if (nImg == resultNum)
                {
                    YxDebug.LogError(result + "转的图片");
                    Paoma(curretImg);
                    nImg = curretImg;
                    resultNum = -5;
                    canMove = false;
                    RefeshTexts();
                    StartCoroutine(TestFun());

                }
                else if (nImg < resultNum)
                {
                    Paoma(curretImg);
                    nImg++;
                    curretTimer = 0f;
                }
                else
                {
                    nImg = curretImg;
                }
            }


        }
    }

}