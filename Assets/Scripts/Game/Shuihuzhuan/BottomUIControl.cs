using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Manager;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.Shuihuzhuan.Scripts
{
    public class BottomUIControl : MonoBehaviour
    {

        public static BottomUIControl instance;

        public Button autoBtn;

        public Button handBtn;
        
        //跳过按钮
        public Button skip;

        //跳过按钮图片
        public Sprite skip_true;

        public Sprite skip_false;
        

        //跳过动画开关
        public bool skip_bool= false;

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

        private int everyTimeNum = 0;
        private int iWinMoney = 0;
        private int MainMoney = 0;
        private bool Kongzhi = false;
        private void Awake()
        {
            instance = this;
//            RefreshBetNum();
        }

        /// <summary>
        /// 游戏开始--向服务器发送数据
        /// </summary>
        public void BeginTurn()
        {
//            winMoneyText.text = 0.ToString();
//            myMoneyText.text = App.GetGameData<GlobalData>().iMainMoney.ToString();
            MusicManager.Instance.Play("gundong2");
            App.GetGameData<GlobalData>().MainMoney = App.GetGameData<GlobalData>().iMainMoney;
            if (App.GetGameData<GlobalData>().iMainMoney >= App.GetGameData<GlobalData>().BetNum)
            {
                //                YxDebug.LogError("---------------向服务器发送数据--------------------");
                GameServer.Instance.MyGameStart(App.GetGameData<GlobalData>().BetBaseNum); //服务器发送数据
//                App.GetGameData<GlobalData>().ZhuanState = 1;
                if (App.GetGameData<GlobalData>().IsAuto)
                {
                    GameStateUiControl.instance.Isaudt();
                }
                else
                {
                    GameStateUiControl.instance.StartWait(); //点击开始关闭所有
                }      
            }
            else//如果是自动而且自己的金币少于加注钱，那么就把自动关闭
            {
                if (App.GetGameData<GlobalData>().IsAuto)
                {
                    App.GetGameData<GlobalData>().IsAuto = false;
                    Auto(false);
                    GameStateUiControl.instance.LostWait();
                }

            }
        }
        /// <summary>
        /// 大小和数据刷新
        /// </summary>
        public void DaxiaoheFun()
        {
            myMoneyText.text = App.GetGameData<GlobalData>().iMainMoney.ToString();
            winMoneyText.text = App.GetGameData<GlobalData>().iWinMoney.ToString();

        }

        /// <summary>
        ///压注总钱数和我的钱数变化
        /// </summary>
        public void ShuaXin()
        {
            int i = 0;
            i = App.GetGameData<GlobalData>().MainMoney - App.GetGameData<GlobalData>().BetNum;
            App.GetGameData<GlobalData>().MainMoney = 0;
            App.GetGameData<GlobalData>().MainMoney = i;
            myMoneyText.text = i.ToString();

         
        }

        /// <summary>
        /// 设置自动
        /// </summary>
        public void AutoBtnFun()
        {
            Auto(true);
            App.GetGameData<GlobalData>().IsAuto = true;
            if (App.GetGameData<GlobalData>().IsAotozhuangtai)//如果在运行动画就不能进行自动
            {
                YxDebug.LogError("执行");
              if (App.GetGameData<GlobalData>().ZhuanState == 1) //刚下注
               {
                   BeginTurn();
                }
            
             if (App.GetGameData<GlobalData>().ZhuanState == 2) //输了
             {
                BeginTurn();
             }
             if (App.GetGameData<GlobalData>().ZhuanState == 3) //赢了
             {
                 if (App.GetGameData<GlobalData>().iWinMoney == 0)
                 {
                     BeginTurn();
                 }
                 else { GetWinMoneyBtnFun(); }
                
             }
        }
    }
        /// <summary>
        /// 点击比倍
        /// </summary>
        public void BigSmallBtnFun()
        {

            BigOrSmallControl.Instance.ThisPlaneFun();
            BigOrSmallControl.Instance.bibeiFun();
            WinPanelControl.instance.winPanel.SetActive(false);
          
        }
        /// <summary>
        /// 点击加注
        /// </summary>
        public void AddBetNumFun()
        {
            App.GetGameData<GlobalData>().ZhuanState = 1;
            if (App.GetGameData<GlobalData>().BetBaseNum < (App.GetGameData<GlobalData>().iXiazhushangxian *10))//压注数不能大于9
            {
                App.GetGameData<GlobalData>().BetBaseNum = App.GetGameData<GlobalData>().BetBaseNum+App.GetGameData<GlobalData>().iXiazhushangxian;
            }
            else App.GetGameData<GlobalData>().BetBaseNum = App.GetGameData<GlobalData>().iXiazhushangxian;//否则押注数是1
            RefreshBetNum();
            if (App.GetGameData<GlobalData>().BetBaseNum != 0)//初始化的时候判断
            {
                GameStateUiControl.instance.BeginButton.interactable = true;//如果是不等于0 那就打开开始
                GameStateUiControl.instance.ZiDongButton.interactable = true;//如果是不等于0 那就打开自动

            }
        }
        /// <summary>
        /// 点击得分
        /// </summary>
        public void GetWinMoneyBtnFun()
        {
            MusicManager.Instance.Play("defen");
            GameStateUiControl.instance.BiBeiButton.interactable = false;
            everyTimeNum = 0;
            iWinMoney = 0;
            MainMoney = 0;
            iWinMoney = App.GetGameData<GlobalData>().iWinMoney;
            MainMoney = App.GetGameData<GlobalData>().MainMoney;
            if (iWinMoney >= 10)
            {
                everyTimeNum = App.GetGameData<GlobalData>().iWinMoney/10;
                Kongzhi = true;
            }
            else Kongzhi = false;
            GetMoney();
        }

        public void GetMoney() //得分动画
        {
            if (iWinMoney >= 10 || Kongzhi)
            {
                iWinMoney -= everyTimeNum;
                MainMoney += everyTimeNum;
            }
            else
            {
                MainMoney = MainMoney + iWinMoney;
                iWinMoney = 0;
            }
            RefreshMoney();
            if (iWinMoney <= 0)
            {
                myMoneyText.text =App.GetGameData<GlobalData>().iMainMoney.ToString( );
                winMoneyText.text = 0.ToString();
                GameStateUiControl.instance.DeFenWait();//显示赢了
               
                if (App.GetGameData<GlobalData>().IsAuto)
                {
                    GameStateUiControl.instance.StopButton.interactable = false;
                }
                WinPanelControl.instance.winPanel.SetActive(false);
                if (App.GetGameData<GlobalData>().IsAuto)//是否为自动
                {
                    BeginTurn();

                }
                return;
             }
             Invoke("GetMoney",0.06f);
          
        }
        public void RefreshMoney()//显示金钱和当局所得
        {
            myMoneyText.text = MainMoney.ToString();
            WinPanelControl.instance.winText.text =iWinMoney.ToString();
            winMoneyText.text = iWinMoney.ToString();
 
        }
        /// <summary>
        /// 手动
        /// </summary>
        public void HandBtnFun()
        {
            Auto(false);
            App.GetGameData<GlobalData>().IsAuto = false ;
            GameStateUiControl.instance.Esc.interactable = true;
           

        }
        /// <summary>
        /// 自动和手动设置
        /// </summary>
        /// <param name="isAuto"></param>
        public void Auto(bool isAuto)
        {
            autoBtn.gameObject.SetActive(!isAuto);
            handBtn.gameObject.SetActive(isAuto);
        }
        /// <summary>
        /// 刷新倍数
        /// </summary>
        public void RefreshBetNum()
        {
            int i = App.GetGameData<GlobalData>().BetBaseNum;
            betBaseText.text = i.ToString();
            App.GetGameData<GlobalData>().BetNum = App.GetGameData<GlobalData>().BetLineNum * i;//总压注钱数
            betNumText.text = App.GetGameData<GlobalData>().BetNum.ToString();
        }
        public void SetMoney()
        {
            myMoneyText.text = App.GetGameData<GlobalData>().MainMoney.ToString();
            LineText.text = App.GetGameData<GlobalData>().BetLineNum.ToString();
            betBaseText.text = App.GetGameData<GlobalData>().BetBaseNum.ToString();
            betNumText .text =
                (App.GetGameData<GlobalData>().BetBaseNum * App.GetGameData<GlobalData>().BetLineNum).ToString();
        }
        /// <summary>
        /// 显示当前所应的钱数大小和
        /// </summary>
        public void Theincome()
        {

            winMoneyText.text = 0.ToString();
            myMoneyText.text = App.GetGameData<GlobalData>().MainMoney.ToString();
        }
        /// <summary>
        /// 显示当前所应的钱数大小和
        /// </summary>
        public void TheincomeMali()
        {

            winMoneyText.text = 0.ToString();
            myMoneyText.text = App.GetGameData<GlobalData>().iMainMoney.ToString();
        }
        public void Escfun()
        {
            GameServer.Instance.OnQuitGame();

        }
        /// <summary>
        /// 停止按钮
        /// </summary>
        /// 
        
        public void BtnStop()
        {
            //print(App.GetGameData<GlobalData>().MainMoney.ToString());
            //for (int i = 0; i < TurnControl.instance.turnItems.Length; i++)
            //{
            //    TurnControl.instance.turnItems[i].GetComponent<TurnItemControl>().canMove = false;
            //}
            
            //for (int i = 0; i < TurnControl.instance.resultImages.Length; i++)
            //{
            //    TurnControl.instance.resultImages[i].enabled = true; 
            //}
            skip_bool = true;
            Invoke("SkipToFalse",2);
        }

        void SkipToFalse()
        {
            skip_bool = false;
        }

        public void SkipAni()
        {
            skip_bool = skip_bool ? false : true;
            if (skip_bool)
            {
                skip.GetComponent<Image>().sprite = skip_true;
            }
            else
            {
                skip.GetComponent<Image>().sprite = skip_false;

            }
        }
    }
}
