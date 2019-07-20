using Assets.Scripts.Game.lswc.Control.System;
using Assets.Scripts.Game.lswc.Core;
using UnityEngine;
using YxFramwork.Manager;
using UnityEngine.UI;

namespace Assets.Scripts.Game.lswc.Windows
{
    /// <summary>
    /// 声音设置窗口
    /// </summary>
    public class LSSettingWindow : InstanceControl
    {

        private static LSSettingWindow _instance;

        public static LSSettingWindow Instance
        {
            get { return _instance; }
        }
        private Slider _musicVolume;

        private Slider _effectVolume;

        private Button _sureBtn;

        private  Button _cancelBtn;

        private void Awake()
        {
            _instance = this;
            Find();
            InitListenr();
            _musicVolume.value = MusicManager.Instance.MusicVolume;
            _effectVolume.value = MusicManager.Instance.EffectVolume;
        }

        private void Find()
        {
            _sureBtn = transform.FindChild("Buttons/sure").GetComponent<Button>();
            _cancelBtn = transform.FindChild("Buttons/cancel").GetComponent<Button>();
            _musicVolume = transform.FindChild("musicSlider").GetComponent<Slider>();
            _effectVolume = transform.FindChild("SoundSlider").GetComponent<Slider>();
        }
        private void InitListenr()
        {
            _sureBtn.onClick.AddListener(OnClickSureBtn);
            _cancelBtn.onClick.AddListener(OnClickCancelBtn);
        }

        private void OnClickSureBtn()
        {
            LSSystemControl.Instance.PlaySuccess(true);
            MusicManager.Instance.MusicVolume = _musicVolume.value;
            MusicManager.Instance.EffectVolume = _effectVolume.value;
            Hide();
        }

        private void OnClickCancelBtn()
        {
            LSSystemControl.Instance.PlaySuccess(true);
            Hide();
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public override void OnExit()
        {
            _instance = null;
        }
    }
}
