using Assets.Scripts.Game.FishGame.Common.core;
using Assets.Scripts.Game.FishGame.FishGenereate;
using Assets.Scripts.Game.FishGame.Fishs;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.Game.FishGame.ScenePreludes
{
    public class ScenePrelude_TwoCircleRotateSin : FishGame.ScenePreludes.ScenePrelude
    {
        [System.Serializable]
        public class GenerateData
        {
            public Fish Prefab_Fish;
            //public float IntervalGenerate = 0.3F;
            public float NumToGenerate = 12;//��������
            public float EllipseA = 0.6F;
            public float RunOutDelay = 1F;
            //public float EllipseB = 0.35F;
        }

        [System.Serializable]
        public class GenerateDatas
        {
            public GenerateData[] GDs;//һȦ��������
        }
        public Vector3[] Locals_Center;
        public Fish[] Prefabs_FishCenter;

        public GenerateDatas[] DataGenFishs;//Ȧ����

        public float OneCircleTime = 12F;//��һȦʱ��,,��ʱ������Ȧ�뿪ʼɢ��
        public float[] CenterFishRunoutTime;//�м����γ�ʱ��
        //public float MoGuiRunoutTime = 4F;
        public float FishRunoutSpeed = 0.6F;
        public float TimeLimit = 40F;//ʱ������

        private int[] mNumLinesOneCircle;//һ��Ȧ�ж��� ����,(����������:Ȧ����)
        private List<Swimmer>[][] mAllSwimmer;
        private float mRunRound = 2.5F;
        private bool mIsEnded = false;

        public override void Go()
        {
            StartCoroutine(_Coro_Process());
            StartCoroutine(_Coro_TimeCountdown());

        }
        IEnumerator _Coro_TimeCountdown()
        {
            yield return new WaitForSeconds(TimeLimit);
            EndPrelude();
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

        public IEnumerator _Coro_Process()
        {
            yield return 0;
            int numCircle = Prefabs_FishCenter.Length;
            mAllSwimmer = new List<Swimmer>[numCircle][];
            mNumLinesOneCircle = new int[numCircle];//Ȧ�� 
            for (int i = 0; i != mNumLinesOneCircle.Length; ++i)
                mNumLinesOneCircle[i] = 4;

            //�������
            if (Random.Range(0, 2) == 1)//0,1��λ
            {
                Vector3 tempPos = Locals_Center[0];
                Locals_Center[0] = Locals_Center[1];
                Locals_Center[1] = tempPos;
            }



            for (int circleIdx = 0; circleIdx != numCircle; ++circleIdx)
            {

                StartCoroutine(_Coro_CenterFishGenerateAndMove(circleIdx));

                mAllSwimmer[circleIdx] = new List<Swimmer>[mNumLinesOneCircle[circleIdx]];
                for (int j = 0; j != mNumLinesOneCircle[circleIdx]; ++j)
                {
                    mAllSwimmer[circleIdx][j] = new List<Swimmer>();
                }

                for (int i = 0; i != mNumLinesOneCircle[circleIdx]; ++i)
                {
                    _Coro_GenerateFish(circleIdx, i);
                    StartCoroutine(_Coro_FishMoveProcess(circleIdx, i));
                }

            }

            yield return 0;
            //Debug.Break();
            //�ȴ�����//����ʱ������
            while (GameMain.Singleton.NumFishAlive != 0)
            {
                yield return 0;
            }

            EndPrelude();
        }

        IEnumerator _Coro_CenterFishGenerateAndMove(int circleIdx)
        {
            float elapse = 0F;
            float rotateSpeed = 6.283185F / OneCircleTime;//����/s
            float useTime = OneCircleTime * mRunRound + CenterFishRunoutTime[circleIdx];
            Fish fishCenter = Instantiate(Prefabs_FishCenter[circleIdx]) as Fish;
            Swimmer centerSwimmer = fishCenter.swimmer;
            fishCenter.ClearAi();
            fishCenter.transform.parent = transform;
            fishCenter.transform.localPosition = new Vector3(Locals_Center[circleIdx].x, Locals_Center[circleIdx].y, Locals_Center[circleIdx].z );
            fishCenter.transform.localRotation = Quaternion.identity;

            centerSwimmer.RotateSpd = 6.283185F;
            while (elapse < useTime)
            {

                if (centerSwimmer == null || fishCenter == null || !fishCenter.Attackable)//��ʱ��attackable��ȷ��������
                {
                    break;
                }
                //�м���ת��
                centerSwimmer.transform.localRotation = Quaternion.AxisAngle(Vector3.forward, centerSwimmer.RotateSpd);
                centerSwimmer.RotateSpd -= rotateSpeed * Time.deltaTime;

                elapse += Time.deltaTime;
                yield return 0;
            }
            if (centerSwimmer != null)
            {
                centerSwimmer.Speed = FishRunoutSpeed * 0.7F;
                centerSwimmer.Go();
            }
        }
        IEnumerator _Coro_FishMoveProcess(int circleIdx, int lineIdx)
        {
            //��Ⱥ��ת\
            GenerateData gd = DataGenFishs[circleIdx].GDs[lineIdx];
            float elapse = 0F;
            float rotateSpeed = 6.283185F / OneCircleTime;//����/s
            float useTime = OneCircleTime * mRunRound + gd.RunOutDelay;

            while (elapse < useTime)
            {

                foreach (Swimmer s in mAllSwimmer[circleIdx][lineIdx])
                {
                    if (s == null)
                        continue;
                    //ʹ����ת�ٶȼ�¼��ǰ��ת���Ƕ�
                    s.transform.localPosition = new Vector3(Locals_Center[circleIdx].x + gd.EllipseA * Mathf.Cos(s.RotateSpd)
                                                            , Locals_Center[circleIdx].y + gd.EllipseA /** 0.75F */* Mathf.Sin(s.RotateSpd)
                                                            , s.transform.localPosition.z);

                    s.transform.localRotation = Quaternion.AngleAxis(-90F + s.RotateSpd * Mathf.Rad2Deg, Vector3.forward);

                    s.RotateSpd -= rotateSpeed * Time.deltaTime;

                }
                elapse += Time.deltaTime;
                yield return 0;
            }


            //��Ⱥɢ��
            foreach (Swimmer s in mAllSwimmer[circleIdx][lineIdx])
            {
                if (s != null)
                {
                    s.Speed = FishRunoutSpeed;
                    s.Go();
                }
            }
        }

        void _Coro_GenerateFish(int circleIdx, int lineIdx)
        {
            //Vector3 newPos = new Vector3(DataGenFish[lineIdx].EllipseA * Mathf.Cos(f.RotateSpd), DataGenFish[lineIdx].EllipseA * 0.75F * Mathf.Sin(f.RotateSpd), 0F);
            //f.transform.localPosition = newPos;
            //f.transform.localRotation = Quaternion.AngleAxis(-90F + f.RotateSpd * Mathf.Rad2Deg, Vector3.forward);

            //f.RotateSpd -= rotateSpeed * Time.deltaTime;

            GenerateData gd = DataGenFishs[circleIdx].GDs[lineIdx];
            float rotateRadAdvance = 6.283185F / gd.NumToGenerate;//��������
            float curRotateRad = 6.283185F;//��ǰ��ת
            FishGenerator fishGenerator = GameMain.Singleton.FishGenerator;
            float depthFish = 1F;

            //��������
            for (int i = 0; i != gd.NumToGenerate; ++i)
            {
                Fish f = Instantiate(gd.Prefab_Fish) as Fish;
                Swimmer s = f.swimmer;
                f.ClearAi();
                //f.AniSprite.clipTime = 0F;
                f.AniSprite.Play();

                s.RotateSpd = curRotateRad;//ʹ����ת�ٶȼ�¼��ǰ��ת���Ƕ�,ת��:360��0��

                s.transform.parent = transform;
                s.transform.localPosition = Locals_Center[circleIdx]
                                            + new Vector3(gd.EllipseA * Mathf.Cos(s.RotateSpd), gd.EllipseA /** 0.75F*/ * Mathf.Sin(s.RotateSpd), depthFish);
                depthFish += 0.1F;
                s.transform.localRotation = Quaternion.AngleAxis(-90F + s.RotateSpd * Mathf.Rad2Deg, Vector3.forward);

                curRotateRad -= rotateRadAdvance;
                mAllSwimmer[circleIdx][lineIdx].Add(s);
            }
        }
    }
}
