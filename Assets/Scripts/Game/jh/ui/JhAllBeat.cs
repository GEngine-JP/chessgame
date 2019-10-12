using UnityEngine;

namespace Assets.Scripts.Game.jh.ui
{
    public class JhAllBeat : MonoBehaviour
    {

        public UILabel BeatCnt;

        private int _cnt;

        public void SetBeat(int num)
        {
            _cnt = num;
            if (_cnt > 0)
            {
                gameObject.SetActive(true);
                BeatCnt.text = _cnt.ToString();
            }
        }

        public void Reset()
        {
            _cnt = 0;
            BeatCnt.text = "0";
            gameObject.SetActive(false);
        }
    }
}
