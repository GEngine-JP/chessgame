using UnityEngine;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.bjl3d
{
    public class SoundPnl : MonoBehaviour {

        public void SliderTest(float even)
        {
            MusicManager.Instance.MusicVolume = even;
        }
    }
}
