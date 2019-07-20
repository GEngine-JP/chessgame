using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.Shuihuzhuan.Scripts
{
    public class ShowResult : MonoBehaviour {

        public static ShowResult instance;

        public Transform[] endPositions;

        public Transform[] homePositions;

        public GameObject[] lines;
        private bool mAnimation = true;
        public Image[] resultImages;
        void Awake()
        {
            instance = this;

        }
        /// <summary>
        /// 开奖数据图片替换
        /// </summary>
        public void SetResultSprite()
        {
            for (int i = 0; i < TurnControl.instance.resultItems.Length; i++)
            {
                resultImages[i].sprite = TurnControl.instance.cardSprites[App.GetGameData<GlobalData>().iTypeImgid[i]];
            }
         
        }

        /// <summary>
        /// 旋转动画
        /// </summary>
        /// <param name="timeKind"></param>
        public void Moveing(int timeKind)
        {
           
            TurnControl.instance.resultItems[timeKind - 1].transform.localPosition =
                TurnControl.instance.homeTransforms[timeKind - 1].localPosition;

            if (timeKind == 15)
            {
                Invoke("GoBackHome", 1.5f);
            }
        }
        public void OnClick()
        {
            GameStateUiControl.instance.TiShi.interactable = false;
            Game.instance.ClearData();//清楚数据
            TurnControl.instance.canMove = true;
            for (int i = 0; i < 15; i++)
            {
                TurnControl.instance.resultImages[i].GetComponent<Animator>().enabled = false;
                App.GetGameData<GlobalData>().iTypeImgid[i] = 0;
                resultImages[i].sprite = TurnControl.instance.cardSprites[0];
            }
            for (int i = 0; i < TurnControl.instance.turnItems.Length; i++)
            {
                TurnControl.instance.turnItems[i].SetActive(true);
                TurnControl.instance.turnItems[i].GetComponent<Image>().sprite = TurnControl.instance.graySprites[0];
            }

           
            App.GetGameData<GlobalData>().changeState = false;
            for (int i = 0; i < 18; i++)
            {
                App.GetGameData<GlobalData>().iLineImgid[i] = 0;
            }
            
            for (int i = 0; i < 9; i++)
            {
                App.GetGameData<GlobalData>().m_LineType[i] = 0;
            }
            StartCoroutine(ShowAwardEffect());
            
                
        }

        /// <summary>
        /// 转动
        /// </summary>
        public void GoBackHome()
        {
           
            for (int i = 0; i < TurnControl.instance.resultItems.Length; i++)
            {
                int ccc = App.GetGameData<GlobalData>().iTypeImgid[i];

                TurnControl.instance.resultImages[i].sprite = TurnControl.instance.graySprites[ccc];
            }
            for (int i = 0; i < TurnControl.instance.resultItems.Length; i++)
            {
                TurnControl.instance.resultItems[i].transform.localPosition =
                    TurnControl.instance.bornTransforms[i/3].localPosition;
            }
            for (int i = 0; i < TurnControl.instance.turnItems.Length; i++)
            {
                TurnControl.instance.turnItems[i].SetActive(true);

            }
            if (App.GetGameData<GlobalData>().show)//开场动画运行一次
            {
                GameStateUiControl.instance.TiShi.interactable = false;
                App.GetGameData<GlobalData>().changeState = false;
                App.GetGameData<GlobalData>().show = false;
                StartCoroutine(ShowAwardEffect());
            }
            else
            {
                GameStateUiControl.instance.TiShi.interactable = true;
                MusicManager.Instance.Play("winsound");
                BottomUIControl.instance.SetMoney();//开始显示金钱和线数
                App.GetGameData<GlobalData>().IsAotozhuangtai = true;
                GameStateUiControl.instance.LostWait();
                Game.instance.ClearData();//清楚数据
            }
            //切换开始结束状态按钮
            TurnControl.instance.SetStop(true);
            //切换到赢了或输了状态
            if (App.GetGameData<GlobalData>().iWinMoney > 0)//赢了
            {
                App.GetGameData<GlobalData>().ZhuanState = 3;
                Game.instance.LotteryJudge(); //计算
                Game.instance.Theincome();//当前所赢的钱数
                GameStateUiControl.instance.ChangeToWait();//按钮全部关闭
               

            }

            if ( App.GetGameData<GlobalData>().iWinMoney == 0)//输了
            {
                App.GetGameData<GlobalData>().IsAotozhuangtai = true;
                   
                    App.GetGameData<GlobalData>().ZhuanState = 2;
                    Game.instance.ClearData();//清楚数据
                    if (App.GetGameData<GlobalData>().IsAuto)
                    {
                        Invoke("IsAutoFun", 2f);
                        GameStateUiControl.instance.BeginButton .interactable  = false;
                    }
                    else
                    {

                        GameStateUiControl.instance.LostWait();//输了的按钮
                        GameStateUiControl.instance.TiShi.interactable = true;
                    }
                }
           
            }
           
        public void IsAutoFun()
        {
            GameStateUiControl.instance.Isaudt();
            BottomUIControl.instance.BeginTurn();//向服务器发送数据

        }
        
        //显示开奖动画
        public IEnumerator ShowAwardEffect()
        {
            
            CheckLine();
            for (int i = 0; i < App.GetGameData<GlobalData>().m_LineType.Length; i++)
            {
                if (App.GetGameData<GlobalData>().m_LineType[i] == 1)
                {
                    
                    //                WaterMarginAudioManager.instance.PlayBeginWinAudio();
                    for (int j = 0; j < 5; j++)
                    {
                        if (App.GetGameData<GlobalData>().m_ResultArray[i, j] == 1)
                        {
                            App.GetGameData<GlobalData>().m_ShowSecAnimate[App.GetGameData<GlobalData>().m_TypeArray[i, j]] = 1;
                            TurnControl.instance.resultImages[App.GetGameData<GlobalData>().m_TypeArray[i, j]].gameObject
                                                                                                    .SetActive(false);

                            string aniStr = App.GetGameData<GlobalData>().iTypeImgid[App.GetGameData<GlobalData>().m_TypeArray[i, j]] + "_" +
                                            "0";
                            TurnControl.instance.resultImages[App.GetGameData<GlobalData>().m_TypeArray[i, j]]
                                .GetComponent<Animator>().enabled = true;
                            TurnControl.instance.resultImages[App.GetGameData<GlobalData>().m_TypeArray[i, j]].gameObject
                                                                                                    .SetActive(true);
                            TurnControl.instance.resultImages[App.GetGameData<GlobalData>().m_TypeArray[i, j]]
                                .GetComponent<Animator>().Play(aniStr);

                        }
                        else { App.GetGameData<GlobalData>().m_ShowSecAnimate[App.GetGameData<GlobalData>().m_TypeArray[i, j]] = 0; }
                    }
                   yield return new WaitForSeconds(1f);
                }
            }
            for (int i = 0; i < 15; i++)
            {
                if (App.GetGameData<GlobalData>().m_ShowSecAnimate[i] == 1)
                {
                    TurnControl.instance.resultImages[i].gameObject.SetActive(false);
                    TurnControl.instance.resultImages[i].sprite =
                        TurnControl.instance.cardSprites[App.GetGameData<GlobalData>().iTypeImgid[i]];
                    //                    string aniStr = App.GetGameData<GlobalData>().iTypeImgid[i] + "_" + "1";
                    TurnControl.instance.resultImages[i].GetComponent<Animator>().enabled = false;
                    TurnControl.instance.resultImages[i].gameObject.SetActive(true);
                    //                    TurnControl.instance.resultImages[i].GetComponent<Animator>().Play(aniStr);
                }
            }
            MusicManager.Instance.Play("winsound");
            BottomUIControl.instance.SetMoney();//开始显示金钱和线数
            App.GetGameData<GlobalData>().IsAotozhuangtai = true;
            GameStateUiControl.instance.LostWait();
            Game.instance.ClearData();//清楚数据
            
            for (int i = 0; i < TurnControl.instance.turnItems.Length; i++)
            {
                TurnControl.instance.turnItems[i].SetActive(true);
                TurnControl.instance.turnItems[i].GetComponent<Image>().sprite = TurnControl.instance.graySprites[0];
            }
            GameStateUiControl.instance.TiShi.interactable = true;
        }
      
        public void CheckLine()
        {
            for (int j = 0; j < 9; j++)
            {
                int CountSame = 0;

                int[] tempint = new int[6];

                for (int i = 0; i < 5; i++)
                {
                    tempint[i] = App.GetGameData<GlobalData>().iTypeImgid[App.GetGameData<GlobalData>().m_TypeArray[j, i]];
                }

                tempint[5] = 100;

                for (int i = 0; i < 5; i++)
                {
                    if (tempint[i] != 0 && tempint[i + 1] != 0 && tempint[i + 1] != 100)
                    {
                        if (tempint[i] == tempint[i + 1]) CountSame++;
                        else break;
                    }
                    else if (tempint[i] == 0 && tempint[i + 1] != 0 && tempint[i + 1] != 100)
                    {
                        CountSame++;
                    }
                    else if (tempint[i] == 0 && tempint[i + 1] == 0)
                    {
                        CountSame++;
                    }
                    else if (tempint[i] != 0 && tempint[i + 1] == 0)
                    {
                        CountSame++;
                        tempint[i + 1] = tempint[i];
                    }
                }

                //5连线
                if (CountSame == 4)
                {
                    App.GetGameData<GlobalData>().m_LineType[j] = 1;
                    for (int i = 0; i < 5; i++)
                    {
                        App.GetGameData<GlobalData>().m_ResultArray[j, i] = 1;
                    }

                }

                //4连线
                if (CountSame == 3)
                {
                    App.GetGameData<GlobalData>().m_LineType[j] = 1;
                    for (int i = 0; i < 4; i++)
                    {
                        App.GetGameData<GlobalData>().m_ResultArray[j, i] = 1;
                    }
                }
                //3连线
                if (CountSame == 2)
                {
                    App.GetGameData<GlobalData>().m_LineType[j] = 1;
                    for (int i = 0; i < 3; i++)
                    {
                        App.GetGameData<GlobalData>().m_ResultArray[j, i] = 1;
                    }
                }
                CountSame = 0;
                for (int i = 1; i < 6; i++)
                {
                    tempint[i] = App.GetGameData<GlobalData>().iTypeImgid[App.GetGameData<GlobalData>().m_TypeArray[j, i - 1]];
                }
                tempint[0] = 100;
                for (int i = 5; i >= 1; i--)
                {
                    if (tempint[i] != 0 && tempint[i - 1] != 0)
                    {
                        if (tempint[i] == tempint[i - 1])
                            CountSame++;
                        else break;
                    }
                    else if (tempint[i] == 0 && tempint[i - 1] != 0)
                    {
                        CountSame++;
                    }
                    else if (tempint[i] == 0 && tempint[i - 1] == 0)
                    {
                        CountSame++;

                    }
                    else if (tempint[i] != 0 && tempint[i - 1] == 0)
                    {
                        CountSame++;
                        tempint[i - 1] = tempint[i];
                    }
                }
                //4连线
                if (CountSame == 3)
                {
                    App.GetGameData<GlobalData>().m_LineType[j] = 1;
                    for (int i = 4; i > 0; i--)
                    {
                        App.GetGameData<GlobalData>().m_ResultArray[j, i] = 1;
                    }
                }
                //3连线
                if (CountSame == 2)
                {
                    App.GetGameData<GlobalData>().m_LineType[j] = 1;
                    for (int i = 4; i > 1; i--)
                    {
                        App.GetGameData<GlobalData>().m_ResultArray[j, i] = 1;
                    }
                }

            }
            //获取对应积分
        }
    }
}
