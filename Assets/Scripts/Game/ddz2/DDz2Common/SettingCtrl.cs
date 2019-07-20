using Assets.Scripts.Game.ddz2.DDzGameListener;
using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Assets.Scripts.Game.ddz2.InheritCommon;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Game.ddz2.DDz2Common
{
    public class SettingCtrl : ServEvtListener
    {
        [SerializeField]
        protected UISlider MUiSlider;

        [SerializeField]
        protected UISlider SoundSlider;

        [SerializeField]
        protected UIToggle VoiceChatToggle;

        [SerializeField] protected MessageBoxScroll MesboxScorll;

        [SerializeField] protected GameObject TableColorSetUi;

        /// <summary>
        /// tablecolorset界面的关闭按钮
        /// </summary>
        [SerializeField] protected GameObject CloseTableColrBtn;
        /// <summary>
        /// 解散房间设置
        /// </summary>
        [SerializeField]
        protected GameObject DissmissObj;
        /// <summary>
        /// 设置按钮与解散按钮控制Grid，娱乐房不显示解散房间
        /// </summary>
        [SerializeField]
        protected UIGrid BtnGrid;
        
        /// <summary>
        /// setting界面的关闭按钮
        /// </summary>
        [SerializeField]
        protected GameObject CloseBtn;

        public delegate void SoundValueChange(float value);
        private static SoundValueChange _onSoundValueChangeEvt;
        public static SoundValueChange OnSoundValueChangeEvt
        {
            set { _onSoundValueChangeEvt += value; }
        }

        /// <summary>
        /// 场景销毁后，重置静态变量
        /// </summary>
        public void OnDestroy()
        {
            _onSoundValueChangeEvt = null;
        }

        /// <summary>
        /// 音乐大小
        /// </summary>
        public const string MusicValueKey = "MusicKey20171844";
        /// <summary>
        /// 音效大小
        /// </summary>
        public const string SoundValueKey = "sdvalueKey20171844";
        /// <summary>
        /// 是否播放语音key
        /// </summary>
        public const string VoiceChatKey = "voickechat20170420";

        private GlobalData _globalData;

        public void Awake()
        {
            Ddz2RemoteServer.AddOnHandUpEvt(OnHandUpEvt);
        }

        /// <summary>
        /// 投票事件激发 2发起 3同意 -1拒绝
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnHandUpEvt(object sender, DdzbaseEventArgs args)
        {
            MesboxScorll.HideMessageBox();
        }

        public void Start()
        {
            _globalData = App.GetGameData<GlobalData>();
        }

        public void OnEnable()
        {
            MUiSlider.value = MusicManager.Instance.MusicVolume;//PlayerPrefs.GetFloat(MusicValueKey, 1);
            SoundSlider.value = MusicManager.Instance.EffectVolume;//PlayerPrefs.GetFloat(SoundValueKey, 1);
            VoiceChatToggle.value = PlayerPrefs.GetInt(VoiceChatKey, 1) == 1;
            if (VoiceChatToggle)
            {
                
            }
            if (DissmissObj)
            {
                DissmissObj.SetActive(App.GetGameData<GlobalData>().IsRoomGame);

                if (BtnGrid)
                {
                    BtnGrid.repositionNow = true;
                }
            }
        }


        /// <summary>
        /// 滑动MusicSlider
        /// </summary>
        public void OnMusicValueChange()
        {
            
            if (MUiSlider != null) if (_globalData != null) _globalData.MusicAudioSource.volume = MUiSlider.value;
        }


        /// <summary>
        /// 滑动SoundSlider
        /// </summary>
        public void OnSoundValueChange()
        {
            if (_onSoundValueChangeEvt != null) _onSoundValueChangeEvt(SoundSlider.value);
        }

        public void OnToggle()
        {
            App.GetGameData<GlobalData>().IsPlayVoiceChat = VoiceChatToggle.value;
        }

        /// <summary>
        /// 当点击关闭按钮
        /// </summary>
        public void OnCloseBtnClick()
        {
            //PlayerPrefs.SetFloat(MusicValueKey, MUiSlider.value);
            //PlayerPrefs.SetFloat(SoundValueKey, SoundSlider.value);
            MusicManager.Instance.MusicVolume = MUiSlider.value;
            MusicManager.Instance.EffectVolume = SoundSlider.value;
            PlayerPrefs.SetInt(VoiceChatKey, VoiceChatToggle.value ? 1 : 0);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// 点击解散房间按钮
        /// </summary>
        public void OnClickDismisRoomBtn()
        {
            if (!App.GetGameData<GlobalData>().IsStartGame)
            {
                //App.GetGameData<GlobalData>().ClearParticalGob();

                if (App.GetGameData<GlobalData>().IsFangZhu)
                    GlobalData.ServInstance.DismissRoom();
                else
                    GlobalData.ServInstance.LeaveRoom();

                return;
            }

            GlobalData.ServInstance.StartHandsUp(2);
        }

        /// <summary>
        /// 打开界面颜色调节界面
        /// </summary>
        public void OpenTableColorSetUi()
        {
            CloseBtn.SetActive(false);
            CloseTableColrBtn.SetActive(true);
            gameObject.SetActive(false);
            TableColorSetUi.SetActive(true);
        }

        /// <summary>
        /// 关闭界面调节界面
        /// </summary>
        public void CloseTalbeColorSetui()
        {
            CloseBtn.SetActive(true);
            CloseTableColrBtn.SetActive(false);
            gameObject.SetActive(true);
            TableColorSetUi.SetActive(false);
        }

        public override void RefreshUiInfo()
        {
        
        }
    }
}
