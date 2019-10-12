using Assets.Scripts.Game.FishGame.Fishs;
using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Game.FishGame.Common.core
{
    public class FishEx_FishEatter : MonoBehaviour
    {
        public delegate void Evt_BeforeEatFish(Fish eatter,Fish beEattedFish);
        public Evt_BeforeEatFish EvtBeforeEatFish;

        public Fish[] FishCanEat;
        public tk2dTextMesh Text_Odds;

        public Fish _Fish
        {
            get
            {
                if(mFish == null)
                    mFish = transform.parent.GetComponent<Fish>();

                return mFish;
            }
        }
        //private bool mEattable = true;//��ǰ״̬,���ڱ���OnTriggerEnter������֮����Ȼ����
        private Fish mFish;
        private readonly static int OddsAccumulMin = 40;//�ۼƷ�����ʼֵ
        private readonly static int OddsAccumulMax = 300;//�ۼƷ������ֵ

        private static Dictionary<int, Fish> mFishCanEatDict;
        private static int mOddsAccumul = OddsAccumulMin;//֮ǰ�ۼƵķ���
    
        void Awake()
        {
            if (mFishCanEatDict == null)
            {
                mFishCanEatDict = new Dictionary<int, Fish>();
                foreach (Fish f in FishCanEat)
                {
                    mFishCanEatDict.Add(f.TypeIndex, null);
                }
            }

            mFish = transform.parent.GetComponent<Fish>();
            mFish.Odds = mOddsAccumul;
            mFish.EvtFishKilled += Handle_FishKilled;
        }
        void Start()
        {
            Text_Odds.text = mOddsAccumul.ToString();
            Text_Odds.Commit();
        }

        void Handle_FishKilled(Player killer, int bulletScore, int fishOddBonus, int bulletOddsMulti, Fish fish, int reward)
        {
            mOddsAccumul = OddsAccumulMin; 
        }

        void OnTriggerEnter(Collider other)
        {
 
            Fish fishCollide = other.GetComponent<Fish>();
            //�����������,�ų�
            if (fishCollide == null)
                return;

            //Ŀ����������,�ų�
            if (!fishCollide.Attackable)
                return;

            //���ڿɳ����б���,�ų�
            if (!mFishCanEatDict.ContainsKey(fishCollide.TypeIndex))
                return;

            //�ۼƷִ����������,�ų�
            if (mOddsAccumul >= OddsAccumulMax)
                return;

            //����������
            if (mFish == null || !mFish.Attackable)
                return;

            if (EvtBeforeEatFish != null)
                EvtBeforeEatFish(mFish,fishCollide);

            //�ӷ�
            mOddsAccumul += fishCollide.Odds;

            mFish.Odds = mOddsAccumul;

            //���Ŀ����
            fishCollide.Clear();

            //���·����� 
            Text_Odds.text = mOddsAccumul.ToString();
            Text_Odds.Commit();


            //YxDebug.Log("Fish =" + fishCollide.name + "   Odds = " + fishCollide.Odds + " totalOdds = " + mOddsAccumul);
        }
    }
}
