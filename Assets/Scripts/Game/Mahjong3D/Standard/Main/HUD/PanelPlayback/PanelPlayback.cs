using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [UIPanelData(typeof(PanelPlayback), UIPanelhierarchy.Popup)]
    public class PanelPlayback : UIPanelBase, IPlaybackCycle
    {
        /// <summary>
        /// 播放时显示的按钮
        /// </summary>
        public Button PlayButton;
        /// <summary>
        /// 暂停按钮
        /// </summary>
        public Button StopButton;
        public Button NextFrameButton;
        public Button RestartButton;

        /// <summary>
        /// 播放速率
        /// </summary>
        public Slider RateSlider;
        public Text RoomId;
        public Text CreateTime;

        public UiLaiziLabel Laizi;
        public GameObject ButtonCtrl;

        protected void Awake()
        {
            OnReset();
        }

        public override void OnInit()
        {
            GameCenter.RegisterCycle(this);
            GameCenter.EventHandle.Subscriber((int)UIEventProtocol.PlaybackRestart, PlaybackRestart);
        }

        public void PlaybackRestart(EvtHandlerArgs args)
        {
            OnReset();
        }

        public void OnPlaybackCycle()
        {
            var replayData = GameCenter.Playback.ReplayData;

            RoomId.text = replayData.RoomId.ToString();
            CreateTime.text = replayData.CreateTime;
        }

        private void OnReset()
        {
            SetButtonState(true);
            Laizi.Hide();
        }

        public void OnPlayClick()
        {
            SetButtonState(false);
            GameCenter.Playback.PlayTask();
        }

        public void OnStoplick()
        {
            SetButtonState(true);
            GameCenter.Playback.PauseTask();
        }

        public void SetLaizi(int card)
        {
            Laizi.Value = card;
        }

        public void OnNextClick()
        {
            GameCenter.Playback.PlayNextFrame();
        }

        public void OnResetClick()
        {
            OnReset();
            GameCenter.EventHandle.Dispatch((int)UIEventProtocol.PlaybackRestart);
        }

        public void OnSliderClick()
        {
            float rate = RateSlider.value;
            GameCenter.Playback.SetRate(rate);
        }

        private void SetButtonState(bool isOn)
        {
            StopButton.gameObject.SetActive(!isOn);

            PlayButton.gameObject.SetActive(isOn);
            RestartButton.gameObject.SetActive(isOn);
            NextFrameButton.gameObject.SetActive(isOn);
        }

        public void OnOpenXialaClick()
        {
            ButtonCtrl.gameObject.SetActive(true);
        }

        public void OnCloseXialaClick()
        {
            ButtonCtrl.gameObject.SetActive(false);
        }

        public void OnReturnToHallClick()
        {
            GameCenter.Playback.ReplayManager.OnQuitGameClick();
        }
    }
}