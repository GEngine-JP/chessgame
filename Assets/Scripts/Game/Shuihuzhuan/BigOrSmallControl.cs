using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.Shuihuzhuan.Scripts
{
    public class BigOrSmallControl : MonoBehaviour
    {
        public static BigOrSmallControl Instance;
        /// <summary>
        /// 骰子
        /// </summary>
        public Sprite[] resultSprites;
        /// <summary>
        /// 大小和
        /// </summary>
        int[] history;

        /// <summary>
        /// 历史ui数组
        /// </summary>
        public Image[] history_img;
        /// <summary>
        /// 大小和十张图片
        /// </summary>
        public Sprite[] history_Sprite;
        /// <summary>
        /// 当前所得
        /// </summary>
        public Text winText;
        /// <summary>
        /// 我的钱数
        /// </summary>
        public Text myMoneyText;
        /// <summary>
        /// 下注倍数
        /// </summary>
        public Text betText;
        /// <summary>
        /// 小  按钮
        /// </summary>
        public Button smallBtn;
        /// <summary>
        /// 和 按钮
        /// </summary>
        public Button middleBtn;
        /// <summary>
        /// 大 按钮
        /// </summary>
        public Button bigBtn;
        /// <summary>
        /// 骰子位置
        /// </summary>
        public Image resultImage1;
        /// <summary>
        /// 骰子位置
        /// </summary>
        public Image resultImage2;
        /// <summary>
        ///  自己
        /// </summary>
        public GameObject thisPanel;
        /// <summary>
        /// 比倍和得分
        /// </summary>
        public GameObject nextPanel;

        private int iSone = 1;

        private int iSsec = 1;
        /// <summary>
        /// 中间的人
        /// </summary>
        public Image bossImage;
        /// <summary>
        /// 左边的人
        /// </summary>
        public Image leftImage;
        /// <summary>
        /// 右边的人
        /// </summary>
        public Image rightImage;

        public GameObject rightjinbi;
        public GameObject leftjinbi;
        public GameObject bossjinbi;

        private int winMoney = 0;
        private int Lishi = 0;
        private int Shaizi = 0;
        private void Awake()
        {

            Instance = this;
            history = new int[] { 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 };
            for (int i = 0; i < 10; i++)
            {
                history_img[i].enabled = false;
            }
        }

        /// <summary>
        /// 服务器初始化数据
        /// </summary>
        public void GameBiBeiResultFun()
        {
            Invoke("Chushihua", 2f);
        }

        private void Chushihua()
        {
            HideBetButtons();
            iSone = App.GetGameData<GlobalData>().iDice1;
            iSsec = App.GetGameData<GlobalData>().iDice2;
            bossImage.GetComponent<Animator>().Play("boss_3");
            AnimateFun();

        }

        //public void Lishijilu()
        //{
        //    if ((iSone + iSsec) > 7)
        //    {
        //        Shaizi = 0;
        //    }
        //    else if ((iSone + iSsec) < 7)
        //    {
        //        Shaizi = 2;
        //    }
        //    else
        //    {
        //        Shaizi = 1;
        //    }         
        //    //if (Lishi >= 10)
        //    //{
        //    //    Lishi = 0;
        //    //}
        //    //for (int i = 0; i < 10; i++)
        //    //{
        //    //    if (i > 10)
        //    //    {
        //    //        historyImages[i + 1].sprite = historyImages[i].sprite;
        //    //    }
        //    //    else historyImages[0].sprite = historySprites[Lishi];

        //    //}
        //}


        public void History()
        {
            int index;
            if ((iSone + iSsec) > 7)
            {
                index = 2;
            }
            else if ((iSone + iSsec) < 7)
            {
                index = 0;
            }
            else
            {
                index = 1;
            }

            for (int i = 0; i < 10; i++)
            {
                if (i < 9)
                {
                    history[i] = history[i + 1];
                }
                else
                {
                    history[i] = index;
                }
            }

            for (int i = 0; i < 10; i++)
            {
                if (history[i] == 0)
                {
                    history_img[i].sprite = history_Sprite[0];
                    history_img[i].enabled = true;
                }
                else if (history[i] == 1)
                {
                    history_img[i].sprite = history_Sprite[1];
                    history_img[i].enabled = true;
                }
                else if (history[i] == 2)
                {
                    history_img[i].sprite = history_Sprite[2];
                    history_img[i].enabled = true;
                }
            }
            for (int i = 0; i < 10; i++)
            {

            }
        }

        public void bibeiFun()
        {
            if (App.GetGameData<GlobalData>().Malizhuantai)
            {
                App.GetGameData<GlobalData>().Malizhuantai = false;
                //                YxDebug.LogError(App.GetGameData<GlobalData>().MaliWinMony );
                winText.text = App.GetGameData<GlobalData>().MaliWinMony.ToString();
                App.GetGameData<GlobalData>().iWinMoney = App.GetGameData<GlobalData>().MaliWinMony;
                myMoneyText.text = App.GetGameData<GlobalData>().MainMoney.ToString();
            }
            else
            {
                winText.text = App.GetGameData<GlobalData>().iWinMoney.ToString();
                myMoneyText.text = App.GetGameData<GlobalData>().MainMoney.ToString();
            }
            betText.text = App.GetGameData<GlobalData>().BetNum.ToString();

            bossImage.GetComponent<Animator>().Play("boss_1");
            Invoke("ShowBetButtons", 2);
            Invoke("ShowBigSmallBtnFun", 2);
            MusicManager.Instance.Play("yaosaizi");

        }

        /// <summary>
        /// 关闭大小和
        /// </summary>
        public void HideBetButtons()
        {

            smallBtn.interactable = false;
            bigBtn.interactable = false;
            middleBtn.interactable = false;
        }
        public void ShowBigSmallBtnFun()
        {
            MusicManager.Instance.Play("xia");
            smallBtn.gameObject.SetActive(true);
            bigBtn.gameObject.SetActive(true);
            middleBtn.gameObject.SetActive(true);
        }
        /// <summary>
        /// 比倍
        /// </summary>
        public void BiBeiBtnFun()
        {
            MusicManager.Instance.Play("yaosaizi");
            bossImage.GetComponent<Animator>().Play("boss_1");
            leftImage.GetComponent<Animator>().Play("left_1");
            rightImage.GetComponent<Animator>().Play("right_1");
            Invoke("ShowBetButtons", 2);
            Invoke("ShowBigSmallBtnFun", 2);
            nextPanel.SetActive(false);//比倍和得分关闭
            resultImage1.gameObject.SetActive(false);//骰子打开
            resultImage2.gameObject.SetActive(false);
        }
        /// <summary>
        /// 得分
        /// </summary>
        public void GetMoneyBtnFun()
        {
            // Lishijilu();
            if (App.GetGameData<GlobalData>().isMary == false)
            {

                BottomUIControl.instance.DaxiaoheFun();
                CloseSelf();
            }
            else
            {
                //                GameServer.Instance.MaLiFun();//服务器发送数据
                LittleMaryControl.Instance.OpenMaryPanel();
                App.GetGameData<GlobalData>().MainMoney = App.GetGameData<GlobalData>().iMainMoney;
                StartCoroutine(LittleMaryControl.Instance.TestFun());
                BottomUIControl.instance.Theincome();
                App.GetGameData<GlobalData>().IsAuto = false;
                CloseSelf();
            }

        }
        /// <summary>
        /// 小
        /// </summary>
        public void SmallBtnFun()
        {
            leftjinbi.SetActive(true);
            rightjinbi.SetActive(false);
            bossjinbi.SetActive(false);
            GameServer.Instance.MyDaXiaoHe(App.GetGameData<GlobalData>().iWinMoney,
                                           App.GetGameData<GlobalData>().Yazhu1);
            HideBetButtons();
        }
        /// <summary>
        /// 和
        /// </summary>
        public void MiddleBtnFun()
        {
            bossjinbi.SetActive(true);
            leftjinbi.SetActive(false);
            rightjinbi.SetActive(false);
            GameServer.Instance.MyDaXiaoHe(App.GetGameData<GlobalData>().iWinMoney,
                                             App.GetGameData<GlobalData>().Yazhu3);
            HideBetButtons();

        }
        /// <summary>
        /// 大
        /// </summary>
        public void BigBtnFun()
        {
            rightjinbi.SetActive(true);
            leftjinbi.SetActive(false);
            bossjinbi.SetActive(false);
            GameServer.Instance.MyDaXiaoHe(App.GetGameData<GlobalData>().iWinMoney,
                                             App.GetGameData<GlobalData>().Yazhu2);
            HideBetButtons();

        }
        public void AnimateFun()
        {
            StartCoroutine(ShowCard());

        }

        public IEnumerator ShowCard()
        {
            yield return new WaitForSeconds(0.5f);
            resultImage1.gameObject.SetActive(true);//骰子打开
            resultImage2.gameObject.SetActive(true);
            resultImage1.sprite = resultSprites[iSone - 1];//骰子的图片
            resultImage2.sprite = resultSprites[iSsec - 1];//骰子的图片
            winText.text = App.GetGameData<GlobalData>().iWinMoney.ToString();
            //            ShowHistory(App.GetGameData<GlobalData>().iHistory);
            if (App.GetGameData<GlobalData>().iWinMoney == 0)//输了
            {
                MusicManager.Instance.Play(iSone + iSsec + "dian");
                leftImage.GetComponent<Animator>().Play("left_3");
                rightImage.GetComponent<Animator>().Play("right_3");
                yield return new WaitForSeconds(2);
                MusicManager.Instance.Play("shu");
                Invoke("CloseSelf", 3);
                BottomUIControl.instance.Theincome();

            }
            else
            {
                MusicManager.Instance.Play(iSone + iSsec + "dian");
                leftImage.GetComponent<Animator>().Play("left_2");
                rightImage.GetComponent<Animator>().Play("right_2");
                HideBetButtons();

                MusicManager.Instance.Play("ying");
                Invoke("ReShowSelf", 3);
            }
            History();
        }
        public void CloseSelf()//输了
        {
            //Lishijilu();
            ReHideBetButtons();
            resultImage1.gameObject.SetActive(false);//骰子关闭
            resultImage2.gameObject.SetActive(false);
            nextPanel.SetActive(false);//比倍和得分关闭
            WinPanelControl.instance.winPanel.SetActive(false);
            GameStateUiControl.instance.LostWait();
            GameStateUiControl.instance.TiShi.interactable = true;
            thisPanel.SetActive(false);//自己关闭
            leftjinbi.SetActive(false);
            rightjinbi.SetActive(false);
            bossjinbi.SetActive(false);
            if (App.GetGameData<GlobalData>().isMary)//玛丽
            {
                App.GetGameData<GlobalData>().iWinMoney = 0;
                App.GetGameData<GlobalData>().MaliWinMony = 0;
                GameServer.Instance.MaLiFun();//服务器发送数据
                LittleMaryControl.Instance.OpenMaryPanel();
                StartCoroutine(LittleMaryControl.Instance.TestFun());
                App.GetGameData<GlobalData>().IsAuto = false;
                CloseSelf();
            }
        }
        public void ReShowSelf()
        {
            ReHideBetButtons();
            nextPanel.SetActive(true);//得分和比倍打开
        }
        public void ShowBetButtons()//大小和按钮点击事件打开
        {
            smallBtn.interactable = true;
            bigBtn.interactable = true;
            middleBtn.interactable = true;

        }
        public void ReHideBetButtons()
        {
            ShowBetButtons();
            smallBtn.gameObject.SetActive(false);//大小和按钮关闭
            bigBtn.gameObject.SetActive(false);
            middleBtn.gameObject.SetActive(false);
        }
        public void ThisPlaneFun()
        {
            thisPanel.SetActive(true);
        }
    }


}
