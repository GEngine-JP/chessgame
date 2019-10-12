using DG.Tweening;
using UnityEngine;

namespace Assets.Scripts.Game.jh.ui.plen_8
{
    public class JhPkHeadAnimation : MonoBehaviour
    {

        public GameObject Head1;

        public GameObject Head2;

        public EventDelegate OnFinish;

        public GameObject WinAnimation;

        public GameObject LostAnimation;

        public GameObject WinEffect;

        public void OnHeadAnimationFinish()
        {
            if (OnFinish != null && OnFinish.isEnabled)
            {
                OnFinish.Execute();
            }
        }


        public void OnWinFinish()
        {
            WinEffect.SetActive(true);
        }

        public void Start()
        {
            gameObject.SetActive(true);
            Head1.SetActive(true);
            Head2.SetActive(true);
            ResetAndPlayer(Head1);
            ResetAndPlayer(Head2);
        }

        protected void ResetAndPlayer(GameObject obj)
        {
            UITweener[] tweeners = obj.GetComponents<UITweener>();
            if (tweeners != null && tweeners.Length > 0)
            {
                foreach (UITweener tweener in tweeners)
                {
                    tweener.ResetToBeginning();
                    tweener.PlayForward();
                }
            }
        }

        public void StartWinLost(Vector3 posWin, Vector3 posLost)
        {
            WinAnimation.SetActive(true);
            LostAnimation.SetActive(true);
            WinAnimation.transform.localPosition = posWin;
            LostAnimation.transform.localPosition = posLost;
            TweenPosition winPos = WinAnimation.GetComponent<TweenPosition>();
            winPos.from.x = posWin.x;
            winPos.to.x = posWin.x;
            TweenPosition lostPos = LostAnimation.GetComponent<TweenPosition>();
            lostPos.from.x = posLost.x;
            lostPos.to.x = posLost.x;
            ResetAndPlayer(WinAnimation);
            ResetAndPlayer(LostAnimation);
        }

        public void ResetWinLost()
        {
            WinAnimation.SetActive(false);
            LostAnimation.SetActive(false);
            WinEffect.SetActive(false);
        }

        public void Reset()
        {
            gameObject.SetActive(false);
        }
    }
}
