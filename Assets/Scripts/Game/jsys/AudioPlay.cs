using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.jsys
{
    public class AudioPlay : MonoBehaviour
    {


        public static AudioPlay Instance;
        public void Start()
        {
            Instance = this;
            MusicManager.Instance.EffectVolume = 1;
            MusicManager.Instance.MusicVolume = 1;
//            MusicManager.Instance.PlayBacksound("Beijing");
        }
        

        public void PlayClocks(string audioname)
        {
            MusicManager.Instance.Play(audioname);
        }
        public void PlayPersons(string audioname)
        {
            MusicManager.Instance.Play(audioname);
        }

        public void PlaySounds(string audioname)
        {
            MusicManager.Instance.Play(audioname);
        }

        protected void OnDestroy()
        {
//            MusicManager.Instance.UnLoad(App.CurrentGameName);
        }

    }
}

