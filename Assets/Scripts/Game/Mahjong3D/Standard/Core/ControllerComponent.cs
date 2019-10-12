using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    /// <summary>
    /// 语言，震动等开关
    /// </summary>
    public enum CtrlSwitchType
    {
        None = 0,
        Open = 1,
        Close = 2,
    }

    public class ControllerComponent : BaseComponent
    {

        #region 回放特效控制
        public void PlaybackPlaySound(int card)
        {
            PlaybackPlaySound(card.ToString());
        }

        public void PlaybackPlaySound(string soundName)
        {
            // 回放音效默认用男声
            var sound = "man";
            var source = sound;
            if (LanguageVoice == (int)CtrlSwitchType.Close)
            {
                source = App.GameKey + sound;
            }
            Facade.Instance<MusicManager>().Play(sound + "_" + soundName, source);
        }

        public void PlayOperateEffect(int chair, PoolObjectType effectType)
        {
            PlaybackPlaySound(effectType.ToString());
            GameCenter.Hud.UIPanelController.PlayPlayerUIEffect(chair, effectType);
        }
        #endregion

        public string GetSex(int sex)
        {
            return UserInfo.GetSexToMW(sex) == 0 ? "woman" : "man";
        }

        public void PlaySound(int chair, string soundName)
        {
            var data = GameCenter.DataCenter.Players[chair];
            var sound = GetSex(data.SexI);
            var source = sound;
            if (LanguageVoice == (int)CtrlSwitchType.Close)
            {
                source = App.GameKey + sound;
            }
            Facade.Instance<MusicManager>().Play(sound + "_" + soundName, source);
        }

        /// <summary>
        /// 语言设置控制
        /// </summary>
        public int LanguageVoice
        {
            get { return PlayerPrefs.GetInt(App.GameKey + "LanguageVoice"); }
            set
            {
                PlayerPrefs.SetInt(App.GameKey + "LanguageVoice", value);
                //设置快捷语和播放速率
                if (value == (int)CtrlSwitchType.Open)
                {
                    //切换快捷语
                    Facade.GetInterimManager<YxChatManager>().ChangeChatDbBundleName("ChatDb");
                    //设置音效播放速率
                    //Facade.Instance<MusicManager>().SoundSource.pitch = GameCenter.DataCenter.Config.TimeNormalAudioClipRate;
                }
                else if (value == (int)CtrlSwitchType.Close)
                {
                    Facade.GetInterimManager<YxChatManager>().ChangeChatDbBundleName(App.GameKey + "ChatDb");
                    //Facade.Instance<MusicManager>().SoundSource.pitch = GameCenter.DataCenter.Config.TimeLocalismAudioClipRate;
                }
            }
        }
    }
}