using Assets.Scripts.Game.FishGame.Common.external.NemoPoolGOs;
using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Effect
{
    /// <summary>
    /// ��Ч��
    /// </summary>
    /// <remarks>
    /// Ч����ʱ�����TimeWebScaleUp + TimeShake
    /// </remarks>
    public class Ef_WebBoom : MonoBehaviour {
        [System.NonSerialized]
        public GameObject Prefab_GoSpriteWeb; //����editor��ֵ,��Ef_WebGenerate��ֵ
        [System.NonSerialized]
        public string NameSprite;//spriteID,��Ef_WebGeenerate��ֵ
        [System.NonSerialized]
        public Color ColorInitialize = Color.white;//��ʼ����ɫ,��Ef_WebGenerate��ֵ
    
        public float ScaleTarget = 1F;//Ŀ������

        public float TimeWebScaleUp = 0.1F;
        public float TimeShake = 0.3F;//�𶯳���ʱ��
        public float RangeShake = 0.1F;//�𶯷�Χ
        public float IntervalShake = 0.07F;//�𶯼��

        public float TimeFadeOut = 0.3F;//����

 
        private Transform mTsWeb;
        tk2dSprite mSprWeb;
        private static Vector3[] mShakeDirects;


        //ʱ����
        //|�����Ŵ󡪡�|�����𶯡���|
        //                |��������|

        enum State1
        {
            ScaleUp,//�Ŵ�
            Shaking//��
        
        }

        State1 mState1;
        bool mState2_IsFadingOut;
        float mElapse;
        float mElapseShakeInterval;
        float mElapseFadeout;
        float mTimeWaitFadeout;//�ȴ���ʧ��ʱ��
        Color mColorWeb;
        int mIdxCurShakePos;
        void Start()
        {
            mState1 = State1.ScaleUp;
            mState2_IsFadingOut = false;
            mShakeDirects = new Vector3[]{
                new Vector3(-1F,1F,0F)
                ,new Vector3(1F,-1.2F,0F)
                ,new Vector3(1.2F,1F,0F)
                ,new Vector3(-1.2F,-0.8F,0F)
            };

            mSprWeb = Pool_GameObj.GetObj(Prefab_GoSpriteWeb).GetComponent<tk2dSprite>();
            if (NameSprite != null && NameSprite != "")
                mSprWeb.spriteId = mSprWeb.GetSpriteIdByName(NameSprite);
            Transform ts = mSprWeb.transform;
            mSprWeb.gameObject.SetActive(true);
            ts.parent = transform;
            ts.localPosition = Vector3.zero;
            Color c = ColorInitialize;
            c.a = 1F;
            mSprWeb.color = c;
            mTsWeb = ts;

            mTimeWaitFadeout = TimeWebScaleUp + TimeShake - TimeFadeOut;
            mColorWeb = mSprWeb.color;
        }

        void Update()
        {
            if (mState1 == State1.ScaleUp)
            {
                if (mElapse < TimeWebScaleUp)
                {
                    mTsWeb.localScale = Vector3.one;//(ScaleTarget * mElapse / TimeWebScaleUp) * 
                    mElapse += Time.deltaTime;
                }
                else
                {
                    mElapse = 0F;
                    mState1 = State1.Shaking;
                }
            }
            else if (mState1 == State1.Shaking)
            {
                if (mElapse < TimeShake)
                {
                
                
                    if (mElapseShakeInterval < IntervalShake)
                    {
                        mElapseShakeInterval += Time.deltaTime;
                    }
                    else
                    {
                        //float elapse = 0F;
                        //int curDirectIdx = 0;

                        mTsWeb.localPosition += (mShakeDirects[mIdxCurShakePos % mShakeDirects.Length] * RangeShake);
                        ++mIdxCurShakePos;

                        mElapseShakeInterval = 0;
                        //elapse += IntervalShake;
                    }
                    //yield return new WaitForSeconds(IntervalShake);
                }
                else
                {
                    mTsWeb.gameObject.SetActive(false);
                    Pool_GameObj.RecycleGO(Prefab_GoSpriteWeb, mTsWeb.gameObject);
                    Destroy(gameObject);
                }
            }

            if (!mState2_IsFadingOut)
            {
                if (mElapseFadeout < mTimeWaitFadeout)
                {
                    mElapseFadeout += Time.deltaTime;
                }
                else
                {
                    mState2_IsFadingOut = true;
                    mElapseFadeout = 0F;
                }
            }
            else
            {
                mColorWeb.a = 1F - mElapseFadeout / TimeFadeOut;
                mSprWeb.color = mColorWeb;
          
                //c.a = 0F;
                //mSprWeb.color = c;

                mElapseFadeout += Time.deltaTime;
            }
        }
        //// Use this for initialization
        //IEnumerator Start () { 
        //    tk2dSprite sprWeb = Pool_GameObj.GetObj(Prefab_GoSpriteWeb).GetComponent<tk2dSprite>();
        //    if (NameSprite != null && NameSprite != "")
        //        sprWeb.spriteId = sprWeb.GetSpriteIdByName(NameSprite);
        //    Transform ts = sprWeb.transform;
        //    sprWeb.gameObject.SetActive(true);
        //    ts.parent = transform;
        //    ts.localPosition = Vector3.zero;
        //    Color c = ColorInitialize;
        //    c.a = 1F;
        //    sprWeb.color = c;
        //    mTsWeb = ts;
        //    float elapse = 0F;
        //    //�Ŵ�
        //    while (elapse < TimeWebScaleUp)
        //    {
        //        mTsWeb.localScale = (ScaleTarget * elapse / TimeWebScaleUp) * Vector3.one;
        //        elapse += Time.deltaTime;
        //        yield return 0;
        //    }
        //    mTsWeb.localScale = Vector3.one * ScaleTarget;  
 
        //    //��
        //    StartCoroutine(_Coro_Shake());
        //    yield return new WaitForSeconds(TimeShake - TimeFadeOut);
        //    //����
        //    elapse = 0F; 

        //    while (elapse < TimeFadeOut)
        //    {
        //        c.a = 1F - elapse/TimeFadeOut;
        //        sprWeb.color = c;
        //        elapse += Time.deltaTime;
        //        yield return 0;
        //    }
        //    c.a = 0F;
        //    sprWeb.color = c;
        //    yield return new WaitForSeconds(0.5F);

        //    sprWeb.gameObject.SetActive(false);
        //    Pool_GameObj.RecycleGO(Prefab_GoSpriteWeb, sprWeb.gameObject);
        //    Destroy(gameObject);
        //}
     

        //IEnumerator _Coro_Shake()
        //{
        //    if (mShakeDirects == null)
        //    {
        //        mShakeDirects = new Vector3[]{
        //            new Vector3(-1F,1F,0F)
        //            ,new Vector3(1F,-1.2F,0F)
        //            ,new Vector3(1.2F,1F,0F)
        //            ,new Vector3(-1.2F,-0.8F,0F)
        //        };
        //    }
        //    float elapse = 0F;
        //    int curDirectIdx = 0;
        //    while (elapse < TimeShake)
        //    {
        //        mTsWeb.localPosition += (mShakeDirects[curDirectIdx%mShakeDirects.Length] * RangeShake);
        //        ++curDirectIdx;
        //        elapse += IntervalShake;
        //        yield return new WaitForSeconds(IntervalShake);
        //    }

        //}

    
        //void OnGUI()
        //{
        //    if (GUILayout.Button("boom"))
        //    {
        //        StartCoroutine(Start1());
        //    }
        //}
    }
}
