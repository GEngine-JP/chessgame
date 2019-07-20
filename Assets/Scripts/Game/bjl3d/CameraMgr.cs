using UnityEngine;

namespace Assets.Scripts.Game.bjl3d
{
    public class CameraMgr : MonoBehaviour
    {
        public static CameraMgr Instance;

        public CameraPathBezierAnimator[] PathAnimator;

        private CameraPathBezierAnimator _playAni;

        protected void Awake()
        {
            Instance = this;
        }

        public void CameraMoveByPath(int pathId)
        {
            if (_playAni != null)
                _playAni.Stop();
            _playAni = PathAnimator[pathId];
            if (_playAni != null)
            {
                _playAni.Play();
            }
        }
    }
}

