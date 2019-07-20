using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Manager;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.bjl3d
{
    public class BetHowMuchPromptUI : MonoBehaviour//  g 11.15
    {
        public static BetHowMuchPromptUI Instance;
        private Text _brankerText;
        private Text _freeHomeText;
        private Text _flatText;
        private Transform _brankMode;
        private Transform _freeMode;
        private Text[] BrankModeTexts = new Text[12];
        private Text[] FreeModeTexts = new Text[12];

        public static BetHowMuchPromptUI Intance;

        /// <summary>
        /// 获取UI操作控件
        /// </summary>
        protected void Awake()
        {
            Instance = this;
            Transform tf = transform.FindChild("Branker_Text");
            if (tf == null)
                YxDebug.LogError("No Such Object");//没有该物体    
            if (tf != null) _brankerText = tf.GetComponent<Text>();
            if (_brankerText == null)
                YxDebug.LogError("No Such  Component");//没有该组件

            tf = transform.FindChild("FreeHome_Text");
            if (tf == null)
                YxDebug.LogError("No Such Object");//没有该物体    
            if (tf != null) _freeHomeText = tf.GetComponent<Text>();
            if (_freeHomeText == null)
                YxDebug.LogError("No Such  Component");//没有该组件

            tf = transform.FindChild("Flat_Text");
            if (tf == null)
                YxDebug.LogError("No Such Object");//没有该物体    
            if (tf != null) _flatText = tf.GetComponent<Text>();
            if (_flatText == null)
                YxDebug.LogError("No Such  Component");//没有该组件


            _brankMode = transform.FindChild("BrankMode");
            if (_brankMode == null)
                YxDebug.LogError("No Such Object");//没有该物体 
            for (int i = 0; i < 12; i++)
            {
                int index = i + 1;
                if (_brankMode != null) tf = _brankMode.FindChild("Text_" + index);
                if (tf == null)
                    YxDebug.LogError("No Such Object");//没有该物体 
                if (tf != null) BrankModeTexts[i] = tf.GetComponent<Text>();
                if (BrankModeTexts[i] == null)
                    YxDebug.LogError("No Such  Component");//没有该组件
            }

            _freeMode = transform.FindChild("FreeMode");
            if (_freeMode == null)
                YxDebug.LogError("No Such Object");//没有该物体 
            for (int i = 0; i < 12; i++)
            {
                int index = i + 1;
                if (_freeMode != null) tf = _freeMode.FindChild("Text_" + index);
                if (tf == null)
                    YxDebug.LogError("No Such Object");//没有该物体 
                FreeModeTexts[i] = tf.GetComponent<Text>();
                if (FreeModeTexts[i] == null)
                    YxDebug.LogError("No Such  Component");//没有该组件
            }
            Intance = this;
        }

        private int[] BrankNum = new int[12];
        private int[] FreeNum = new int[12];

        /// <summary>
        /// 初始化历史纪录的点数
        /// </summary>
        /// <param name="history"></param>
        public void SetLuziInfo(int[] history)
        {
            // 
            for (int i = 0; i < history.Length; i++)
            {
                FreeNum[i] = history[(i + App.GetGameData<GlobalData>().Hisidx)%12] & 0xf;
                //Debug.Log("闲家点数" + i + ":" + FreeNum[i]);
                BrankNum[i] = history[(i + App.GetGameData<GlobalData>().Hisidx) % 12] >> 4 & 0xf;
                //Debug.Log("庄家点数" + i + ":" + BrankNum[i]);
                //Debug.Log("历史记录中的数据" + i + ":" + history[i]);
            }
            for (int i = 0; i < history.Length; i++)
            {
                BrankModeTexts[i].text = BrankNum[i] + "";
                FreeModeTexts[i].text = FreeNum[i] + "";
            }
        }
        public int TotolIndex;
        private bool IsInit=true;
        
        /// <summary>
        /// 游戏底部的初始化历史纪录显示
        /// </summary>
        public void BottomLuzi()
        {
            int HistoryData=0;
            if (IsInit)
            {
                for (int i = 0; i < 12; i++)
                {
                    int HistoryData1 = 0;
                    if (FreeNum[i] > BrankNum[i])
                    {
                        HistoryData1 = 1;
                    }
                    if (FreeNum[i] < BrankNum[i])
                    {
                        HistoryData1 = 2;
                    }
                    if (FreeNum[i] == BrankNum[i])
                    {
                        HistoryData1 = 3;
                    }
                    UserInfoUI.Instance.GameConfig.LuziInfo.Add(HistoryData1);
                    IsInit = false;
                }
                return;
            }
                if (App.GetGameData<GlobalData>().XianValue > App.GetGameData<GlobalData>().ZhuangValue)
                {

                    StartCoroutine(PlaySoundDianShu(App.GetGameData<GlobalData>().XianValue, App.GetGameData<GlobalData>().ZhuangValue, false));
                    HistoryData = 1;
                }
                if (App.GetGameData<GlobalData>().XianValue < App.GetGameData<GlobalData>().ZhuangValue)
                {

                    StartCoroutine(PlaySoundDianShu(App.GetGameData<GlobalData>().XianValue, App.GetGameData<GlobalData>().ZhuangValue, true));
                    HistoryData = 2;
                }
                if (App.GetGameData<GlobalData>().XianValue == App.GetGameData<GlobalData>().ZhuangValue)
                {
                    HistoryData = 3;
                }
                UserInfoUI.Instance.GameConfig.LuziInfo.Add(HistoryData);
          
        }

        IEnumerator PlaySoundDianShu(int xianValue,int zhuangValue,bool isZhuang)
        {
            yield return new WaitForSeconds(6f);
            if (isZhuang)
            {
                MusicManager.Instance.Play("zhuangbet");
                yield return new WaitForSeconds(2f);
                MusicManager.Instance.Play("zhuang" + zhuangValue);
                yield return new WaitForSeconds(2f);
                MusicManager.Instance.Play("xian" + xianValue);
            }
            else
            {
                MusicManager.Instance.Play("xianbet");
                yield return new WaitForSeconds(1f);
                MusicManager.Instance.Play("xian" + xianValue);
                yield return new WaitForSeconds(1f);
                MusicManager.Instance.Play("zhuang" + zhuangValue);
            }
               
        }
        private int _myIndex = 1;

        /// <summary>
        /// 历史纪录的信息现实的点数
        /// </summary>
        public void Data()
        {
            FreeNum[(_myIndex - 1) % 12] = App.GetGameData<GlobalData>().XianValue;
            BrankNum[(_myIndex - 1) % 12] = App.GetGameData<GlobalData>().ZhuangValue;
            for (int i = 0; i < BrankNum.Length; i++)
            {
                BrankModeTexts[i].text = BrankNum[(_myIndex + i) % 12] + "";
                FreeModeTexts[i].text = FreeNum[(_myIndex + i) % 12] + "";
            }
            _myIndex++;

        }
        /// <summary>
        /// 初始化筹码面板上的可下注钱数
        /// </summary>
        public void HowMuchPrompt()
        {
            if (App.GetGameData<GlobalData>().Allow[3] == 0)
            {
                _brankerText.text = App.GetGameData<GlobalData>().Maxante + "";
            }
            else
            {
                _brankerText.text = App.GetGameData<GlobalData>().Allow[3] + "";
            }
            if (App.GetGameData<GlobalData>().Allow[0] == 0)
            {
                _freeHomeText.text = App.GetGameData<GlobalData>().Maxante + "";
            }
            else
            {
                _freeHomeText.text = App.GetGameData<GlobalData>().Allow[0] + "";
            }
            if (App.GetGameData<GlobalData>().Allow[6] == 0)
            {
                _flatText.text = App.GetGameData<GlobalData>().Maxante + "";
            }
            else
            {
                _flatText.text = App.GetGameData<GlobalData>().Allow[6] + "";
            }
        }
    }
}