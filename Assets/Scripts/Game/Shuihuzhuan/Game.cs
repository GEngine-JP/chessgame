using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Manager;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.Shuihuzhuan.Scripts
{
    public class Game : MonoBehaviour
    {
        public static Game instance;
        /// <summary>
        /// 总钱数
        /// </summary>
        public Text myMoneyText;

        /// <summary>
        /// 当局所得
        /// </summary>
        public Text winMoneyText;

        /// <summary>
        /// 总押注数
        /// </summary>
        public Text betNumText;

        /// <summary>
        /// 压注倍数
        /// </summary>
        public Text betBaseText;

        /// <summary>
        /// 线数
        /// </summary>
        public Text LineText;
        public void Start()
        {
//            MusicManager.Instance.PlayBacksound("xiongdiwushu");
        }


        void Awake()
        {
            instance = this;
        }
        public void LotteryJudge()
        {

            StartCoroutine(ShowAwardEffect());
            GameStateUiControl.instance.StartWait();

        }
        public void GamePlay()
        {
            for (int y = 0; y < 9; y++)
            {
                if (App.GetGameData<GlobalData>().iLineImgid[y] >= 3)
                {
                    App.GetGameData<GlobalData>().changeState = true;
                    App.GetGameData<GlobalData>().m_LineType[y] = 1;
                    for (int i = 0; i < App.GetGameData<GlobalData>().iLineImgid[y]; i++)
                    {
                        App.GetGameData<GlobalData>().m_LineType[y] = 1;
                        App.GetGameData<GlobalData>().m_ResultArray[y, i] = 1;
                    }
                }
            }
            int w = 9;
            for (int q = 0; q < 9; q++)
            {
                App.GetGameData<GlobalData>().changeState = true;
                if (App.GetGameData<GlobalData>().iLineImgid[w] >= 3 && w < 18)
                {
                    App.GetGameData<GlobalData>().m_LineType[q] = 1;
                    for (int e = 4; e >= (5 - App.GetGameData<GlobalData>().iLineImgid[w]); e--)
                    {
                        App.GetGameData<GlobalData>().m_ResultArray[q, e] = 1;
                    }
                }
                w++;
            }
        }
        //显示开奖动画
        public IEnumerator ShowAwardEffect()
        {
            GamePlay();
            for (int i = 0; i < App.GetGameData<GlobalData>().m_LineType.Length; i++)
            {
                if (App.GetGameData<GlobalData>().m_LineType[i] == 1)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        if (App.GetGameData<GlobalData>().m_ResultArray[i, j] == 1)
                        {
                            App.GetGameData<GlobalData>().m_ShowSecAnimate[App.GetGameData<GlobalData>().m_TypeArray[i, j]] = 1;//显示动画的位置
                            TurnControl.instance.resultImages[App.GetGameData<GlobalData>().m_TypeArray[i, j]].gameObject.SetActive(false);//关闭组建
                            string aniStr = App.GetGameData<GlobalData>().iTypeImgid[App.GetGameData<GlobalData>().m_TypeArray[i, j]] + "_" + "0";
                            TurnControl.instance.resultImages[App.GetGameData<GlobalData>().m_TypeArray[i, j]].GetComponent<Animator>().enabled = true;
                            TurnControl.instance.resultImages[App.GetGameData<GlobalData>().m_TypeArray[i, j]].gameObject.SetActive(true);
                            if (!BottomUIControl.instance.skip_bool)
                            {
                                TurnControl.instance.resultImages[App.GetGameData<GlobalData>().m_TypeArray[i, j]].GetComponent<Animator>().Play(aniStr);
                            }
                            

                        }
                    }
                    if (!BottomUIControl.instance.skip_bool)
                    {
                        yield return new WaitForSeconds(1f);
                    }
                    else
                    {
                        yield return new WaitForSeconds(0.01f);
                    }
                }
            }
            for (int i = 0; i < 15; i++)
            {
                if (App.GetGameData<GlobalData>().m_ShowSecAnimate[i] == 1)
                {
                    TurnControl.instance.resultImages[i].gameObject.SetActive(false);
                    TurnControl.instance.resultImages[i].sprite =
                        TurnControl.instance.cardSprites[App.GetGameData<GlobalData>().iTypeImgid[i]];
                    string aniStr = App.GetGameData<GlobalData>().iTypeImgid[i] + "_" + "1";
                    TurnControl.instance.resultImages[i].GetComponent<Animator>().enabled = true;
                    TurnControl.instance.resultImages[i].gameObject.SetActive(true);
                    TurnControl.instance.resultImages[i].GetComponent<Animator>().Play(aniStr);
                }
            }
            App.GetGameData<GlobalData>().IsAotozhuangtai = true;
            MusicManager.Instance.Play("winsound");
            if (App.GetGameData<GlobalData>().iWinMoney > 0)//当前所得钱数是否大于0
            {
                if (!BottomUIControl.instance.skip_bool)
                {
                    yield return new WaitForSeconds(4f);
                }
                else
                {
                    yield return new WaitForSeconds(0.4f);
                }

                if (App.GetGameData<GlobalData>().isMary == false) //是小玛丽
                {
                    WinPanelControl.instance.ShowWinPanel();//打开动画
                }
                ChangeToBigSmall();
                ClearData();//清空数据

            }
        }


        public void ClearData()
        {
            for (int i = 0; i < 9; i++)
            {
                App.GetGameData<GlobalData>().m_LineType[i] = 0;
            }
            for (int o = 0; o < 15; o++)
            {
                App.GetGameData<GlobalData>().m_ShowSecAnimate[o] = 0;

            }
            for (int y = 0; y < 9; y++)
            {
                for (int j = 0; j < 5; j++)
                {
                    App.GetGameData<GlobalData>().m_ResultArray[y, j] = 0;
                } //逐元素赋值。
            }
            YxDebug.LogError("-----------清空存储数据-----------");
        }
        public void ChangeToBigSmall()
        {
            App.GetGameData<GlobalData>().changeState = false;
            if (App.GetGameData<GlobalData>().isMary) //是小玛丽
            {
                App.GetGameData<GlobalData>().IsAuto = false;
                GameStateUiControl.instance.ChangeToWait();
                Invoke("Mali", 3f);
            }
            else
            {
                ii();
            }
        }

        public void ii()
        {
            if (App.GetGameData<GlobalData>().IsAuto) //是自动
            {
                Invoke("IsAutoFun", 2f);
            }
            else
            {
                GameStateUiControl.instance.WinWait(); //赢了

            }
        }

        public void IsAutoFun()
        {
            GameStateUiControl.instance.Isaudt();
            BottomUIControl.instance.GetWinMoneyBtnFun();

        }

        public void Mali()
        {

            GameServer.Instance.MaLiFun();//服务器发送数据
            BottomUIControl.instance.Auto(false);
            LittleMaryControl.Instance.OpenMaryPanel();
            App.GetGameData<GlobalData>().MaliWinMony = 0;
            App.GetGameData<GlobalData>().MaliWinMony = App.GetGameData<GlobalData>().MaliWinMony + App.GetGameData<GlobalData>().iWinMoney;
            StartCoroutine(LittleMaryControl.Instance.TestFun());
            App.GetGameData<GlobalData>().IsAuto = false;

        }
        /// <summary>
        /// 输了刷新
        /// </summary>
        public void LostShuaxinFun()
        {
            App.GetGameData<GlobalData>().MainMoney = 0;
            App.GetGameData<GlobalData>().MainMoney = App.GetGameData<GlobalData>().iMainMoney;
            myMoneyText.text = App.GetGameData<GlobalData>().iMainMoney.ToString();
            winMoneyText.text = App.GetGameData<GlobalData>().iWinMoney.ToString();
        }
        /// <summary>
        /// 赢了刷新
        /// </summary>
        public void YingShuaxinFun()
        {
            App.GetGameData<GlobalData>().MainMoney = App.GetGameData<GlobalData>().iMainMoney -
                                                      App.GetGameData<GlobalData>().iWinMoney;
            myMoneyText.text = (App.GetGameData<GlobalData>().iMainMoney - App.GetGameData<GlobalData>().iWinMoney).ToString();

        }
        public void Theincome()
        {

            winMoneyText.text = App.GetGameData<GlobalData>().iWinMoney.ToString();
        }

    }
}











