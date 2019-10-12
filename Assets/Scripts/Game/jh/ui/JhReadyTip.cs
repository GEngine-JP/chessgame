using System;
using com.yxixia.utile.YxDebug;
using UnityEngine;

namespace Assets.Scripts.Game.jh.ui
{
    public class JhReadyTip : MonoBehaviour
    {

        public UILabel Text;
        public String Content;

        protected int _time;

        protected bool _isStart;

        public void Show(int time)
        {
            gameObject.SetActive(true);
            _time = time;
            if (!_isStart)
            {
                _isStart = true;
                String text = Content.Replace("#", _time.ToString());
                Text.text = text.Replace("\\n", "\n");
                InvokeRepeating("SetText", 1f, 1f);    
            }
            
        }

        protected void SetText()
        {
            _time--;
            if (_time <= 0)
            {
                Hide();
            }
            String text = Content.Replace("#", _time.ToString());
            Text.text = text.Replace("\\n", "\n");
        }

        public void Hide()
        {
            _isStart = false;
            CancelInvoke();
            gameObject.SetActive(false);
        }
    }
}
