using UnityEngine;
using System.Collections.Generic;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.FishGame.Common.core
{
    public class GameOdds  {
    
        public static float GainRatio 
        {
            get { return mGainRatio; }
            set { mGainRatio = value > 0 ? value * DifficultFactor : value; }//��ˮ�ʴ���0ʱ�Ѷ����Ӳ���Ч
        }
   


        public static float DifficultFactor = 1F;//�Ѷ�ϵ��(�ڳ�ˮ�ʴ���0ʱ��Ч)
        public static float[] GainRatios = new float[] { 0.0005F, 0.001F, 0.002F, 0.02F, 0.04F };//��ͬ�Ѷ��µ�����
        public static float[] GainRatioConditFactors = new float[] { 1F, 0.5F, 0.2F };//= new float[] { 1F, 0.5F, 0.01F };//��ͬ��������(ArenaType)�µ��������������.(��:����mReducegainOddLineʱ :GainRatio *= GainRatiosReduceFactor[ArenaType];)
        public static int[] CoinPresents = new int[] { 21, 22, 23 };//���δ������ݳ������Ͳ�ͬ,���͵ı���.�����볡������(Arenatype)���,ֵ�ǳ�ˮ����Dat_GainAdjustIdx
        public static float GainRatioConditFactor = 1F;//��ǰ������ˮ����

        //public static float RatioGet_Lizi = 0.0033F;//�����ڻ�ü���
        private static float mGainRatio = 0.0005F;//ӯ����(-1F~1F)
        private static int mReduceGainOddLine = 100;//���ڸñ�������,���ճ�������������mGainRatio.(����߸߱�������������,ʹ�÷���������������)

        private static FishOddsData _fd = new FishOddsData(0,0);
        /// <summary>
        /// ���ֵ�Ƿ�����()
        /// </summary>
        /// <param name="r">0F~1F</param>
        /// <returns></returns>
        public static bool IsHitInOne(float r)
        {
            //return true;
#if UNITY_EDITOR
            if(GameMain.Singleton.OneKillDie)
            {
                return true;
            }
#endif
            return Random.Range(0F, 1F) < r;
            //return Random.Range(0, 10000000) < (int)( r * 10000000);
        }

        public static bool IsHitInOne2(float r)
        {
            return Random.Range(0F, 1F) < r;
            //return Random.Range(0, 10000000) < (int)( r * 10000000);
        }
        /// <summary>
        ///  һ�ӵ���÷�ֵ
        /// </summary>
        /// <returns>��÷�ֵ</returns>
        public static List<FishOddsData> OneBulletKillFish(int bulletScore, FishOddsData fishFirst, List<FishOddsData> otherFish)
        {
            List<FishOddsData> fishDieList = new List<FishOddsData>();
            //������    
            int oddsTotal = fishFirst.Odds;
            if (fishFirst.Odds <= 1)
            {
                YxDebug.LogWarning("��һ�����oddС�ڵ���һ�Ļ��͹���,��ʱ�����ܴ�����������!!,(ȫ����������ը��ͬ�³��������ܻ�����������)");
                return fishDieList ;
            }
            foreach (FishOddsData f in otherFish)
            {
                if (f.Odds <= 1)
                {
                    YxDebug.LogWarning("otherFish��oddС�ڵ���һ�Ļ��͹���,��ʱ�����ܴ�����������!!");
                    //return fishDieList;
                }

                oddsTotal += f.Odds;
            }
            //YxDebug.Log("oddsTotal = " + oddsTotal);

            float gainRatio = GainRatio;
            if (gainRatio > 0F && oddsTotal >= mReduceGainOddLine)
            {
                gainRatio *= GainRatioConditFactor;
            }

            //��һ���������� 
            float firstDieRatio = (1F - gainRatio) * (fishFirst.Odds + oddsTotal) / (2F * oddsTotal * fishFirst.Odds);
            //YxDebug.Log("2F * oddsTotal * fishFirst.Odds = " + firstDieRatio);
            //YxDebug.Log("firstDieRatio = " + firstDieRatio + "    odds =" + fishFirst.Odds);
            if (IsHitInOne(firstDieRatio))//��һ��������
            {
                fishDieList.Add(fishFirst);
                //Debug.Log("firstDieRatio = " + firstDieRatio+"    odds ="+fishFirst.Odds);
                //�����������Ƿ����� ,�������㼸��
                foreach (FishOddsData f in otherFish)
                {
                    float dieRatio = (1F - gainRatio - firstDieRatio * fishFirst.Odds) / (firstDieRatio * (oddsTotal - fishFirst.Odds));
                    //Debug.Log("otherDieRatio = " + dieRatio + "    odds =" + f.Odds);
                    if (IsHitInOne(dieRatio))
                        fishDieList.Add(f);
                }
            }

            return fishDieList;
        }

        public static FishOddsData OneBulletKillFish(int bulletScore, FishOddsData fishFirst)
        {
        
            //������    
            int oddsTotal = fishFirst.Odds;
            if (fishFirst.Odds <= 1)
            {
                Debug.LogWarning("��һ�����oddС�ڵ���һ�Ļ��͹���,��ʱ�����ܴ�����������!!,(ȫ����������ը��ͬ�³��������ܻ�����������)");
                return _fd;
            }
       

            float gainRatio = GainRatio;
            if (gainRatio > 0F && oddsTotal >= mReduceGainOddLine)
            {
                gainRatio *= GainRatioConditFactor;
            }

            //��һ���������� 
            float firstDieRatio = (1F - gainRatio) * (fishFirst.Odds + oddsTotal) / (2F * oddsTotal * fishFirst.Odds);
            //Debug.Log("2F * oddsTotal * fishFirst.Odds = " + firstDieRatio);
            //Debug.Log("firstDieRatio = " + firstDieRatio + "    odds =" + fishFirst.Odds);
            if (IsHitInOne(firstDieRatio))//��һ��������
            {
                return fishFirst;
            }
            return _fd;

        }
    }
}
