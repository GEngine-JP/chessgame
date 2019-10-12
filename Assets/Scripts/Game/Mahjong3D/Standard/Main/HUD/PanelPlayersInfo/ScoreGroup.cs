using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class ScoreGroup : MonoBehaviour
    {
        public List<ScoreEffectItem> ScoreEffectList = new List<ScoreEffectItem>();

        public float ShowTime = 0;
        private float mTimer;
        private bool mFlag;

        public ScoreEffectItem this[int index]
        {
            get { return MahjongUtility.GetItemByChair(ScoreEffectList, index); }
        }

        private void Update()
        {
            if (mFlag)
            {
                mTimer += Time.deltaTime;
                if (mTimer >= ShowTime)
                {
                    Hide();
                }
            }
        }

        public void Play()
        {
            GetComponent<TweenPosition>().Do((cmp) =>
            {
                cmp.ResetToBeginning();
                cmp.PlayForward();
            });
            mFlag = true;
        }

        public void Hide()
        {
            mTimer = 0;
            mFlag = false;
            for (int i = 0; i < ScoreEffectList.Count; i++)
            {
                ScoreEffectList[i].Hide();
            }
        }
    }
}