using Assets.Scripts.Game.bjl3d.Scripts;
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Manager;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.bjl3d
{
    public class LuziInfoUI : MonoBehaviour//G 11.15
    {
        public static LuziInfoUI Instance;
        private Image[] LuziBrankListUIs = new Image[12];
        private Image[] LuziFreeListUIs = new Image[12];
        private Image[] LuziFlatListUIs = new Image[12];

        private Text _rankerStaticText;
        private Text _freeStaticText;
        private Text _flatStaticText;

        private int _luziIndex=-2;
        private int _luziIndexLength=-2;
        private LuziData[] _luziDataList;   //路子信息

        private Transform _leftBtnClickEff;
        private Transform _rightBtnClickEff;

        /// <summary>
        /// 获取UI操作控件
        /// </summary>
        protected void Awake()
        {
            Instance = this;
            Transform tf;
            for (int i = 0; i < 12; i++)
            {
                tf = transform.FindChild("LuziRankList/ok_" + i);
                if (tf == null)
                    YxDebug.LogError("No Such Object");//没有该物体    
                if (tf != null) LuziBrankListUIs[i] = tf.GetComponent<Image>();
                if (LuziBrankListUIs[i] == null)
                    YxDebug.LogError("No Such  Component");//没有该组件
            }

            for (int i = 0; i < 12; i++)
            {
                tf = transform.FindChild("LuziFreeList/ok_" + i);
                if (tf == null)
                    YxDebug.LogError("No Such Object");//没有该物体    
                if (tf != null) LuziFreeListUIs[i] = tf.GetComponent<Image>();
                if (LuziFreeListUIs[i] == null)
                    YxDebug.LogError("No Such  Component");//没有该组件
            }

            for (int i = 0; i < 12; i++)
            {
                tf = transform.FindChild("LuziFlatList/ok_" + i);
                if (tf == null)
                    YxDebug.LogError("No Such Object");//没有该物体    
                if (tf != null) LuziFlatListUIs[i] = tf.GetComponent<Image>();
                if (LuziFlatListUIs[i] == null)
                    YxDebug.LogError("No Such  Component");//没有该组件
            }

            tf = transform.FindChild("RankStatis_Text");
            if (tf == null)
                YxDebug.LogError("No Such Object");//没有该物体    
            if (tf != null) _rankerStaticText = tf.GetComponent<Text>();
            if (_rankerStaticText == null)
                YxDebug.LogError("No Such  Component");//没有该组件

            tf = transform.FindChild("FreeStatis_Text");
            if (tf == null)
                YxDebug.LogError("No Such Object");//没有该物体    
            if (tf != null) _freeStaticText = tf.GetComponent<Text>();
            if (_freeStaticText == null)
                YxDebug.LogError("No Such  Component");//没有该组件

            tf = transform.FindChild("FlatStatis_Text");
            if (tf == null)
                YxDebug.LogError("No Such Object");//没有该物体    
            if (tf != null) _flatStaticText = tf.GetComponent<Text>();
            if (_flatStaticText == null)
                YxDebug.LogError("No Such  Component");//没有该组件

            _leftBtnClickEff = transform.FindChild("Particle System_muxie_left");
            if (_leftBtnClickEff == null)
                YxDebug.LogError("No Such Object");//没有该物体    


            _rightBtnClickEff = transform.FindChild("Particle System_muxie_right");
            if (_rightBtnClickEff == null)
                YxDebug.LogError("No Such Object");//没有该物体    
        }
        /// <summary>
        /// 单击向右
        /// </summary>
        public void ClickToRight()
        {
            if (_luziIndex != _luziIndexLength)
            {
                if (_luziIndex < IndexTemp-1)
                {
                    _luziIndex =_luziIndex +1;
                    ShowLuziEx(_luziIndex);
                    if (_rightBtnClickEff.gameObject.activeSelf)
                        _rightBtnClickEff.gameObject.SetActive(false);
                    _rightBtnClickEff.gameObject.SetActive(true);//特效
                    MusicManager.Instance.Play("pageOver");
                }
                else
                {
                    MusicManager.Instance.Play("pagebtn");
                }
            }
        }

        private int IndexTemp;
        private int IndexTemp2;
        private bool isShow;

        /// <summary>
        /// 单击向左
        /// </summary>
        public void ClickToLeft()
        {
            if (isShow)
            {
                _luziIndex = IndexTemp-1;
                isShow = false;
            }
            if (_luziIndex > -1)
            {
                _luziIndex = _luziIndex - 1;
                ShowLuziEx(_luziIndex);
                if (_leftBtnClickEff.gameObject.activeSelf)
                    _leftBtnClickEff.gameObject.SetActive(false);
                _leftBtnClickEff.gameObject.SetActive(true);//特效
                MusicManager.Instance.Play("pageOver");
            }
            else
            {
                MusicManager.Instance.Play("pagebtn");
            }
            
        }

        /// <summary>
        /// 初始化的时候显示历史记录的效果
        /// </summary>
        public void ShowHistoryEx()
        {
            isShow = true;
            _luziIndexLength = -2;
            ShowLuziEx(IndexTemp2);
            IndexTemp2++;
            IndexTemp = IndexTemp2;
            int zTotal = 0, xTotal = 0, fTotal = 0;
            for (int i = 0; i < UserInfoUI.Instance.GameConfig.LuziInfo.Count; i++)
            {
                if (UserInfoUI.Instance.GameConfig.LuziInfo[i] == 1)
                {
                    xTotal += 1;
                }
                if (UserInfoUI.Instance.GameConfig.LuziInfo[i] == 2)
                {
                    zTotal += 1;
                }
                if (UserInfoUI.Instance.GameConfig.LuziInfo[i] == 3)
                {
                    fTotal += 1;
                }
            }
            _rankerStaticText.text = zTotal + "";
            _freeStaticText.text = xTotal + "";
            _flatStaticText.text = fTotal + "";

        }

        public void ShowLuziEx(int index)
        {
            for (int i = 1; i < App.GetGameData<GlobalData>().History.Length; i++)
            {
                if (UserInfoUI.Instance.GameConfig.LuziInfo[index + i] == 1)
                {
                    LuziBrankListUIs[i].transform.FindChild("gou").gameObject.SetActive(false);
                    LuziFreeListUIs[i].transform.FindChild("gou").gameObject.SetActive(true);
                    LuziFlatListUIs[i].transform.FindChild("gou").gameObject.SetActive(false);

                }
                if (UserInfoUI.Instance.GameConfig.LuziInfo[index + i] == 2)
                {
                    LuziBrankListUIs[i].transform.FindChild("gou").gameObject.SetActive(true);
                    LuziFreeListUIs[i].transform.FindChild("gou").gameObject.SetActive(false);
                    LuziFlatListUIs[i].transform.FindChild("gou").gameObject.SetActive(false);
                }
                if (UserInfoUI.Instance.GameConfig.LuziInfo[index + i] == 3)
                {
                    LuziBrankListUIs[i].transform.FindChild("gou").gameObject.SetActive(false);
                    LuziFreeListUIs[i].transform.FindChild("gou").gameObject.SetActive(false);
                    LuziFlatListUIs[i].transform.FindChild("gou").gameObject.SetActive(true);
                }
            }
        }

    }
}