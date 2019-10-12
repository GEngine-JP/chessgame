using Assets.Scripts.Game.FishGame.Common.core;
using Assets.Scripts.Game.FishGame.Fishs;
using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Game.FishGame.ScenePreludes
{
    public class ScenePrelude_FlowerEmit : FishGame.ScenePreludes.ScenePrelude {
        [System.Serializable]
        public class FlowerEmitData
        {
            public Fish PrefabFish;
            public float SwimSpd = 0.16F;//�ζ��ٶ�
            public float EmitInterval = 0.1F;//������
            public float EmitUseTime = 0.5F;//����ʹ��ʱ��
        }
        public tk2dSpriteAnimator AniSpr_YunMu;
        public Fish Prefab_FirstFish;
        public float SpeedFirstFish = 153.6F;
        public FlowerEmitData[] FlowerEmitDatas;//���������һ�ֳ�
        public float TimeLimit = 32F;//ʱ������

        private float mYunMuCurrentAlpha = 1F;
        private bool mIsEnded = false;
    

    
        //void Start()
        //{
        //    Go();
        //}
        public override void Go()
        {
            //StartCoroutine(_Coro_Process());
            //StartCoroutine(_Coro_TimeCountdown());

            int numScreen = GameMain.Singleton.ScreenNumUsing;
            ScenePrelude_FlowerEmit[] preludes = new ScenePrelude_FlowerEmit[numScreen];
            preludes[0] = this;
            for (int i = 1; i != preludes.Length; ++i)
            {
                GameObject newPresudeInst = (Instantiate(gameObject) as GameObject);
                preludes[i] = newPresudeInst.GetComponent<ScenePrelude_FlowerEmit>();
                preludes[i].transform.parent = transform.parent;
            }

            for (int i = 0; i != preludes.Length; ++i)
            {
                preludes[i].transform.localPosition =
                    new Vector3(GameMain.Singleton.WorldDimension.x + GameMain.Singleton.WorldDimension.width / numScreen * (0.5F + i)
                                , 0F, Defines.GMDepth_Fish);
                preludes[i]._Go();
            }

 
        }

        //�ڲ�����,��ֹ�ݹ�
        void _Go()
        {
            StartCoroutine(_Coro_Process());
            StartCoroutine(_Coro_TimeCountdown());
        }


        void EndPrelude()
        {
            if (!mIsEnded)
            {
                mIsEnded = true;
                if (Evt_PreludeEnd != null)
                    Evt_PreludeEnd();
                GameMain.Singleton.FishGenerator.KillAllImmediate();
                Destroy(gameObject);
            }
        }

        IEnumerator _Coro_TimeCountdown()
        {
            yield return new WaitForSeconds(TimeLimit);
            EndPrelude();
        }


        IEnumerator _Coro_Process()
        {
            tk2dSpriteAnimation s;
        
            //��ĸ����
            AniSpr_YunMu.AnimationEventTriggered += Handle_YunMuAnimating;

            //�������� ���� ����45��һ��
            int hudieNum = 3;
            int hudieInstedNum = 0;

            int[] rndArray = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };
            System.Array.Sort(rndArray, (a, b) => { return Random.Range(0, 3) - 1; });
        
            while (hudieInstedNum < hudieNum)
            {
                yield return new WaitForSeconds(1F);
                Fish fishHuDie = Instantiate(Prefab_FirstFish) as Fish;
                fishHuDie.ClearAi();
                fishHuDie.transform.parent = transform;
                fishHuDie.transform.localPosition = Vector3.zero;
            
                fishHuDie.transform.rotation = Quaternion.AngleAxis(45F * rndArray[hudieInstedNum], Vector3.forward);


                //����汾���һ����������ƫ��
                if (hudieInstedNum == hudieNum - 1)
                {
                    fishHuDie.transform.rotation *= Quaternion.AngleAxis(-15F, Vector3.forward);
                }


                fishHuDie.swimmer.Go();
                fishHuDie.swimmer.Speed = SpeedFirstFish;
                ++hudieInstedNum;
            }



            //��ĸfadeOut
            yield return new WaitForSeconds(1F);
            StartCoroutine(_Coro_YunMuAlphaDown());

 
            //ɢ������
            int flowEmitNum = FlowerEmitDatas.Length;
            int curflowEmitNum = 0;
            //rndArray = new int[FlowerEmitDatas.Length];
            //for(int i = 0; i != rndArray.Length; ++i)
            //    rndArray[i] = i;
            //System.Array.Sort(rndArray,(a,b)=>{return Random.Range(0,3)-1;});
            float baseRotateAngle = 90F; 
        
            float depth = 0F;
            while (curflowEmitNum < flowEmitNum)
            {
                FlowerEmitData emitData = FlowerEmitDatas[curflowEmitNum];
   
                float emitElapse = 0F;
                //һȺ
                while (emitElapse < emitData.EmitUseTime)
                {
                    //6����һ���
                    for (int i = 0; i != 6; ++i )
                    {
                        Fish f = Instantiate(emitData.PrefabFish) as Fish;
                        f.ClearAi();
                        f.transform.parent = transform;
                    
                        f.transform.localRotation = Quaternion.AngleAxis(baseRotateAngle + 60F * i + Random.Range(-15F, 15F), Vector3.forward);
                        f.transform.localPosition = new Vector3(0F, 0F, depth) + f.transform.up * 0.02F ;
                        depth += 0.001F;

                        f.swimmer.Speed = emitData.SwimSpd;
                        f.swimmer.Go();
                    }

                    emitElapse += emitData.EmitInterval;
                    yield return new WaitForSeconds(emitData.EmitInterval);
                }

                baseRotateAngle += 45F;
                ++curflowEmitNum;
                yield return new WaitForSeconds(1F);
            }

 
            //�ȴ�����,[����]����ʱ��������
            while (GameMain.Singleton.NumFishAlive != 0)
            {
                yield return 0;
            }
            EndPrelude();
        
        }

        IEnumerator _Coro_YunMuAlphaDown()
        {
            float useTime = 0.5F;
            float elapse = 0F;
            while (elapse < useTime)
            {
                mYunMuCurrentAlpha = 1F - elapse / useTime;
                elapse += Time.deltaTime;
                yield return 0;
            }
            Destroy(AniSpr_YunMu.gameObject);
        
        }

        void Handle_YunMuAnimating(tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip, int frameNum)
        {
            tk2dSprite mspr  = sprite.GetComponent<tk2dSprite>( );
            Color c = mspr.color;
            c.a = mYunMuCurrentAlpha;
            mspr.color = c;
        }
    }
}
