using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.Game.FishGame.ScenePreludes
{
    public class ScenePrelude_TransitionX : FishGame.ScenePreludes.ScenePrelude
    {
        public float TimeLimit = 60F;//ʱ������
        //public float InAdvanceGenFishTime = 0F;//��ǰ����ʱ��
        public Transform[] TsShoalOfFish;
        public float Speed = 1F;


        private bool mIsEnded = false;
        //private bool mIsDestroyed = false;
        private List<Transform> mMovingShoals;
        private Transform[] mShoalParents;

        //private float elapse = 0F;
 
        public override void Go()
        {
            int screenNum = GameMain.Singleton.ScreenNumUsing;
         
            mShoalParents = new Transform[GameMain.Singleton.ScreenNumUsing];
            //for (int i = 0; i != screenNum; ++i)
            //{
            //    mShoalParents[i] = new GameObject("shoalParent").transform;
            //    mShoalParents[i].parent = transform;
            //    mShoalParents[i].localPosition = Vector3.zero;
            //}
            mShoalParents[0] = new GameObject("shoalParent").transform;
            mShoalParents[0].parent = transform;
            mShoalParents[0].localPosition = Vector3.zero;
            //�����úõ������ȥ����0λ��
            foreach(Transform ts in TsShoalOfFish)
            {
                ts.parent = mShoalParents[0];
            }

            //���ƺ��������
            for (int i = 1; i != screenNum; ++i)
            {
                mShoalParents[i] = Instantiate(mShoalParents[0]) as Transform;
                mShoalParents[i].parent = transform;
            }

            //����λ��
            for (int i = 0; i != screenNum; ++i)
            {
                mShoalParents[i].localPosition =
                    new Vector3(GameMain.Singleton.WorldDimension.x + GameMain.Singleton.WorldDimension.width / screenNum * (0.5F + i)
                                , 0F, 0F);
            }

            mMovingShoals = new List<Transform>();
            foreach(Transform shoalParent in mShoalParents)
            {
                foreach (Transform shoal in shoalParent)
                {
                    mMovingShoals.Add(shoal);
                }
            } 
            StartCoroutine(_Coro_MoveShoalFish());
            StartCoroutine(_Coro_WaitNullFish());
            StartCoroutine(_Coro_TimeCountdown());
            //StartCoroutine(_Coro_EndTime());

        }

        void EndPrelude()
        {
            if (!mIsEnded)
            {
                mIsEnded = true;
                if (Evt_PreludeEnd != null)
                    Evt_PreludeEnd();
                GameMain.Singleton.FishGenerator.KillAllImmediate();//��Destroy(gameObject);֮ǰɾ�������ڳ���,��ֹ©ɾ
                Destroy(gameObject);
            }
        }


        IEnumerator _Coro_TimeCountdown()
        {
            yield return new WaitForSeconds(TimeLimit);
            EndPrelude();
        }

        public IEnumerator _Coro_WaitNullFish()
        {
            //yield return new WaitForSeconds(TimeLimit * 0.5F);//һ��ʱ��֮�����ж�.һ��ʼû��
        
            while (GameMain.Singleton.NumFishAlive <= 2 )
            {
                yield return 0;
            }

            //�ȴ�����,����ʱ������
            while (GameMain.Singleton.NumFishAlive != 0)
            {
                yield return 0;
            }

            EndPrelude();
        }



        public IEnumerator _Coro_MoveShoalFish()
        { 
            //for (int i = 0; i != TsShoalOfFish.Length; ++i)
            while (true)
            {
                foreach (Transform ts in mMovingShoals)
                {
                    ts.position += ts.right * Speed * Time.deltaTime;
                }
                yield return 0;
            }
        }

        //public IEnumerator _Coro_EndTime()
        //{
        //    float elapse = 0F;
        //    float EndTime = TimeLimit - InAdvanceGenFishTime;
        //    while (true) 
        //    {
        //        elapse += Time.deltaTime;
        //        if (elapse > EndTime)
        //        {
        //            Debug.Log("AdvanceGenFishTim");
        //            EndPrelude();
        //            yield break;
        //        }
        //        yield return 0;
        //    }
        //}

    }
}
