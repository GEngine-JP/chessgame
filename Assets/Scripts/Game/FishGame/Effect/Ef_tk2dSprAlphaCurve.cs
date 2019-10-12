using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Effect
{
    /// <summary>
    /// �����߱任spr��alphaֵ
    /// </summary>
    /// <remarks>
    /// Ч�ʷǳ���,һ������ڿ���������Ҫ1MS.
    /// ������Ƚϼ򵥵Ķ���,ֱ��ʹ���㷨�ȽϺ�
    /// </remarks>
    public class Ef_tk2dSprAlphaCurve : MonoBehaviour
    {
        public AnimationCurve CurveAlpha;//alpha����
        public float TimeOneLoop = 1.5F;//һ��ѭ������ʱ��(��λ:��)

        private Color mColorCurrent;
        private tk2dSprite mSpr;

        void Awake () {
            mSpr = GetComponent<tk2dSprite>();
            if (mSpr == null)
            {
                Destroy(this);
                return;
            }

            mColorCurrent = mSpr.color;
        }
	
        void Update () {
            mColorCurrent.a = CurveAlpha.Evaluate((Time.time / TimeOneLoop) % 2F);
            //Debug.Log(((Time.time / TimeOneLoop)%2F));
            //mColorCurrent.a = Mathf.Cos(Time.time / TimeOneLoop * Mathf.PI)*0.5F+0.5F;
            mSpr.color = mColorCurrent;
        
        }
    }
}
