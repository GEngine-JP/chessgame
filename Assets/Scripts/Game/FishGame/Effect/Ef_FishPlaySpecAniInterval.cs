using Assets.Scripts.Game.FishGame.Fishs;
using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Effect
{
    /// <summary>
    /// ����ָ������,���ڴ����
    /// </summary>
    /// <remarks>��Ҫͬһ��������</remarks>
    public class Ef_FishPlaySpecAniInterval : MonoBehaviour
    {
        public float Interval = 15F;
        public string AniName = "��������⶯��0";

        private int mOriClipidx = 0;
        private Fish mFish ;
        private bool mIsPlaySpecAni;//�Ƿ��ڲ���ָ������

        private float mElapse;
        private float mSpecAniLength;
        //IEnumerator Start()
        //{
        //    mFish = GetComponent<Fish>();
        //    if (mFish == null)
        //        yield break;
        //    mOriClipidx = mFish.AniSprite.clipId;
        //    tk2dSpriteAnimationClip aniClip = mFish.AniSprite.anim.clips[mFish.AniSprite.anim.GetClipIdByName(AniName)];
        //    float aniLength = aniClip.frames.Length / aniClip.fps;

        //    while (true)
        //    {
        //        yield return new WaitForSeconds(Interval);
        //        mFish.AniSprite.Play(AniName,0F);

        //        yield return new WaitForSeconds(aniLength);
            
        //        mFish.AniSprite.Play(mOriClipidx,0F);
        //    } 
        //}

        void Awake()
        {
            mFish = GetComponent<Fish>();
            if (mFish == null)
                return;
            mOriClipidx = mFish.AniSprite.DefaultClipId;
            tk2dSpriteAnimationClip aniClip = mFish.AniSprite.Library.clips[mFish.AniSprite.Library.GetClipIdByName(AniName)];
            mSpecAniLength = aniClip.frames.Length / aniClip.fps;
         
        }
        void Update()
        {
            if (mIsPlaySpecAni)
            {
                if (mElapse > mSpecAniLength)//תΪ����ԭ����
                {
                    mElapse = 0F;
                    mIsPlaySpecAni = false;
                    mFish.AniSprite.DefaultClipId = mOriClipidx;
                    mFish.AniSprite.PlayFrom(mFish.AniSprite.DefaultClip ,0F);
                }
            }
            else
            {
                if (mElapse > Interval)
                {
                    mElapse = 0F;
                    mIsPlaySpecAni = true;
                    //mFish.AniSprite.Play(AniName, 0F);
                    mFish.AniSprite.PlayFrom(AniName, 0F);
                }
            }
            mElapse += Time.deltaTime;
        }

        void OnDisable()
        {
            if (GameMain.IsEditorShutdown)
                return;

            if (mFish == null || mFish.AniSprite == null)
                return;

            mFish.AniSprite.DefaultClipId = mOriClipidx;
            mFish.AniSprite.PlayFrom(mFish.AniSprite.DefaultClip, 0F);
            //mFish.AniSprite.Play(0F);
        }

    
    }
}
