using Assets.Scripts.Game.FishGame.FishGenereate;
using Assets.Scripts.Game.FishGame.Fishs;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.Game.FishGame.ScenePreludes
{
    /// <summary>
    /// ��Ⱥƽ����Ļ
    /// </summary>
    /// <remarks>
    ///  ����:
    ///    1.�ƶ�������transform.right
    ///    2.ԭ����Ҫ����Ⱥ���,��transform.position����
    /// </remarks>
    public class ScenePrelude_Transition : FishGame.ScenePreludes.ScenePrelude
    {
        [System.Serializable]
        public class EmitData
        {
            public Transform TsShoalOfFish;
            public FishGenerateWhenEnterWorld LastFishEnterWorld;
            public Transform[] TsPosStart;//��ʼλ��(ά������Ļ����)//todo:����ʹ��gamemain.worldDimemsion�����
        }
        public EmitData[] EmitDatas;

        //public Transform[] TsShoalOfFish;
        public float Speed = 1F;
        //public FishGenerateWhenEnterWorld[] LastFishEnterWorld;//�������������(������TsShoalOfFishһһ��Ӧ)
        public bool IsDepthAdvanceAuto = true;//�Զ���ȵ���
        private bool mIsEnded = false;

        public override void Go() 
        {
            StartCoroutine(_Coro_Transiting());
            StartCoroutine(_Coro_WaitNullFish());
        }

        public IEnumerator _Coro_Transiting()
        {
            //���ÿ�ʼλ�� 
            for (var i = 0; i != EmitDatas.Length; ++i)
            {
                var data = EmitDatas[i];
                var pos = data.TsPosStart[GameMain.Singleton.ScreenNumUsing - 1].localPosition;
                pos.z = 0;
                data.TsShoalOfFish.localPosition = pos;
            }

            //��ʼ�ƶ�
            Transform ts;
            while (true)
            {
                var nullNum = 0;
                var len = EmitDatas.Length;
                for (int i = 0; i != len; ++i)
                {
                    var data = EmitDatas[i];
                    if (data.TsShoalOfFish == null)
                    {
                        ++nullNum;
                        continue;
                    }

                    ts = data.TsShoalOfFish;
                    ts.position += ts.right * Speed * Time.deltaTime; 
                    if ((ts.right.x > 0F && ts.position.x > GameMain.Singleton.WorldDimension.xMax)//���󲢴ﵽ�����Ļ��
                        || (ts.right.x <= 0F && ts.position.x < GameMain.Singleton.WorldDimension.x))//�����ƶ����ﵽ�ұ���Ļ��
                    {
                        var fishToClear = new List<Fish>();

                        foreach (Transform tChild in ts)
                        {
                            var f = tChild.GetComponent<Fish>();
                            if (f != null && f.Attackable)
                            {
                                fishToClear.Add(f);
                            }
                        }
                        foreach (var f in fishToClear)
                        {
                            f.Clear();
 
                        }

                        Destroy(ts.gameObject);
                        data.TsShoalOfFish = null;
                    }
                }



                if (nullNum == EmitDatas.Length)
                {
                    EndPrelude();
                }

                yield return 0;
            }

        }

    
        public IEnumerator _Coro_WaitNullFish()
        {
            int numLastFishEnterWorld = 0;
            foreach (EmitData ed in EmitDatas)
            {
                ed.LastFishEnterWorld.EvtFishGenerated += (Fish f) =>
                    {
                        ++numLastFishEnterWorld;
                        //Debug.Log("lastFishGenerated");
                    };
            }

            while (numLastFishEnterWorld != EmitDatas.Length)
            {
                yield return 0;
            }
            //Debug.Log("waitZeroFish");

            while (GameMain.Singleton.NumFishAlive != 0)
            {
                yield return 0;
            }

            EndPrelude();
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
    }
}
