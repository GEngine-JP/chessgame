using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.bjl3d
{
    public class VoicePnl : MonoBehaviour
    {
        public void SliderTest(float even)
        {
            MusicManager.Instance.EffectVolume = even;
        }
    }
}
