using Assets.Scripts.Game.FishGame.Common.Utils;
using Assets.Scripts.Game.FishGame.Common.external.NemoFileIO;
using Assets.Scripts.Game.FishGame.Fishs;
using UnityEngine;
using System.Collections;
using YxFramwork.Common;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.FishGame.Common.core
{
    /// <summary>
    /// ���ˮ����
    /// </summary>
    /// <remarks>���ˮ����(0-20) 
    /// (0)     "������״",
    /// (1-10)  "��ˮһǧ","��ˮ��ǧ","��ˮ��ǧ","��ˮ��ǧ","��ˮ��ǧ","��ˮ��ǧ","��ˮһ��","��ˮ����","��ˮ����","��ˮʮ��" 
    /// (11-20) "��ˮһǧ","��ˮ��ǧ","��ˮ��ǧ","��ˮ��ǧ","��ˮ��ǧ","��ˮ��ǧ","��ˮһ��","��ˮ����","��ˮ����","��ˮʮ��" 
    /// </remarks>
    public class GainAdjuster : MonoBehaviour {
        public PersistentData<int, int> GainAdjust_GainScoreRT;//��̨ʵʱ��÷�
        public PersistentData<int, int> GainAdjust_LossScoreRT;//��̨ʵʱ�����
        public int GiftCoinSmall = 15000;
        public int GiftCoinMedium = 20000;
        public int GiftCoinLarge = 25000;


        private BackStageSetting mBss;

        private int mScoreGain;
        private int mScoreLoss;
        private bool mIsLossOrProfit = true;//true:��ˮ״̬:false:��ˮ״̬
        private int mCurLossProfitScore = 0;//��ǰ���ˮ��Ŀ,��λ:��

        private static float GainRatio_Loss = -0.01F;//��ˮʱ�ĳ�ˮ��
        private static float GainRatio_Profit = 0.01F;//��ˮʱ�ĳ�ˮ��
        private static int[] LossProfitData = //���ˮ������:��λ:��
            {
                0
                ,1000,2000,3000,4000,5000,8000,10000,20000,50000,100000//��ˮֵ1-10
                ,1000,2000,3000,4000,5000,8000,10000,20000,50000,100000//��ˮֵ11-20
                ,15000,20000,25000//���δ����ˮ,����С�д󳡵ط�ˮ����21-23
                ,10//��ʱֵ24
            };

        void Awake()
        {
            mBss = GameMain.Singleton.BSSetting;
        }
        // Use this for initialization
        void Start () {
            LossProfitData[21] = GiftCoinSmall;
            LossProfitData[22] = GiftCoinMedium;
            LossProfitData[23] = GiftCoinLarge;


            var gdata = App.GetGameData<FishGameData>();
            gdata.EvtBgClearAllDataBefore += Handle_BackGroundClearAllDataBefore;//�ں�̨����������0��ʱ��.�������ڼ�����ˮ��ӯ������
            //GameMain.EvtBGChangeArenaType += Handle_BackGroundChangeArenaType;
            if (mBss.Dat_GainAdjustIdx.Val == 0)//û�г��ˮ����
            {
                return;
            }else if (mBss.Dat_GainAdjustIdx.Val >= 1 && mBss.Dat_GainAdjustIdx.Val <= 10)//��ˮ
            {
                mIsLossOrProfit = true;
            }

            else if (mBss.Dat_GainAdjustIdx.Val >= 11 && mBss.Dat_GainAdjustIdx.Val <= 20)//��ˮ
            {
                mIsLossOrProfit = false;
            }
            else if (mBss.Dat_GainAdjustIdx.Val >= 21 && mBss.Dat_GainAdjustIdx.Val <= 24)//���δ����ˮ(21-23) + ��ˮ��ʱֵ(24)
            {
                mIsLossOrProfit = true;
            }
            else
            {
                YxDebug.LogError("Dat_GainAdjustIdx ����Чֵ��Χ����.");
                return;
            }


            gdata.EvtPlayerGunFired += Handle_PlayerFire;
            gdata.EvtPlayerGainScoreFromFish += Handle_PlayerGainScoreFromFish;
        

            if (GainAdjust_GainScoreRT == null) GainAdjust_GainScoreRT = new PersistentData<int, int>("GainAdjust_GainScoreRT");
            if (GainAdjust_LossScoreRT == null) GainAdjust_LossScoreRT = new PersistentData<int, int>("GainAdjust_LossScoreRT"); 
            mScoreGain = GainAdjust_GainScoreRT.Val;
            mScoreLoss = GainAdjust_LossScoreRT.Val;

            mCurLossProfitScore = LossProfitData[mBss.Dat_GainAdjustIdx.Val] * mBss.InsertCoinScoreRatio.Val;

            //YxDebug.Log("mCurLossProfitScore = " + mCurLossProfitScore);
            StartCoroutine("_Coro_SaveGainData");
            StartCoroutine("_Coro_LossOrProfitprocess");
        }

        /// <summary>
        /// ���÷�ˮֵ
        /// </summary>
        /// <remarks>С�ڵ���0��ֵ����</remarks>
        /// <param name="small"></param>
        /// <param name="medium"></param>
        /// <param name="large"></param>
        public void SetGiftCoin(int small, int medium, int large)
        {
        
            if (small >= 0 )
                LossProfitData[21] = GiftCoinSmall = small;
            if( medium >= 0)
                LossProfitData[22] = GiftCoinMedium = medium;
            if(large >= 0 )
                LossProfitData[23] = GiftCoinLarge = large;

            mCurLossProfitScore = LossProfitData[mBss.Dat_GainAdjustIdx.Val] * mBss.InsertCoinScoreRatio.Val;
        }
        /// <summary>
        /// �ⲿ�����Զ����ˮֵ
        /// </summary>
        /// <param name="coin">��ֵ</param>
        public void FreeGainCustom(int coin)
        {
            if(mBss == null)
                mBss = GameMain.Singleton.BSSetting;

            if (mBss.Dat_GainAdjustIdx.Val != 0)
            {
                YxDebug.LogWarning("�ڳ��ˮ״̬�µ����Զ�����ˮ,FreeGainCustom()������Ч");
                return;
            }

            mBss.Dat_GainAdjustIdx.Val = 24;
            LossProfitData[24] = coin;

            var gdata = App.GetGameData<FishGameData>();
            gdata.EvtPlayerGunFired += Handle_PlayerFire;
            gdata.EvtPlayerGainScoreFromFish += Handle_PlayerGainScoreFromFish;


            if (GainAdjust_GainScoreRT == null) GainAdjust_GainScoreRT = new PersistentData<int, int>("GainAdjust_GainScoreRT");
            if (GainAdjust_LossScoreRT == null) GainAdjust_LossScoreRT = new PersistentData<int, int>("GainAdjust_LossScoreRT");
            mScoreGain = GainAdjust_GainScoreRT.Val = 0;
            mScoreLoss = GainAdjust_LossScoreRT.Val = 0;

            mCurLossProfitScore = LossProfitData[mBss.Dat_GainAdjustIdx.Val] * mBss.InsertCoinScoreRatio.Val;

            StopCoroutine("_Coro_SaveGainData");
            StartCoroutine("_Coro_SaveGainData");

            StopCoroutine("_Coro_LossOrProfitprocess");
            StartCoroutine("_Coro_LossOrProfitprocess");

        }
        void Handle_PlayerFire(Player owner, Gun gun, int useScore, bool isLock, int bulletId)
        {
            //YxDebug.Log((mIsLossOrProfit ? "��ˮ��" : "��ˮ��") + "  GainAdjustIdx =" + mBss.Dat_GainAdjustIdx.Val +"    gainOdd = "+GameOdds.GainRatio);
            mScoreGain += useScore;
        }

        void Handle_PlayerGainScoreFromFish(Player p, int scoreGetted, Fish firstFish, int bulletScore)
        {
            mScoreLoss += scoreGetted;
        }

        IEnumerator _Coro_LossOrProfitprocess()
        {
            while (true)
            {
                yield return new WaitForSeconds(1F);
                GameOdds.GainRatio = mIsLossOrProfit ? GainRatio_Loss : GainRatio_Profit;

                if (mIsLossOrProfit)// ��ˮ
                {
                    if ((mScoreLoss - mScoreGain) > mCurLossProfitScore)//(��̨���� - ��̨ӯ��) �ﵽ��ˮ��
                    {
                        StopCoroutine("_Coro_SaveGainData");
                        ReleaseAllHook();
                        mBss.Dat_GainAdjustIdx.Val = 0;//��ԭ���ˮ���
                        GameOdds.GainRatio = GameOdds.GainRatios[(int)(mBss.GameDifficult_.Val)];//��ԭ����
                        GainAdjust_GainScoreRT.Val = 0;
                        GainAdjust_LossScoreRT.Val = 0;
                        yield break;
                        //YxDebug.Log("��ˮ��� = " + (mScoreLoss - mScoreGain));
                        //Destroy(gameObject);
                    }
                }
                else// ��ˮ
                {
                    if ((mScoreGain - mScoreLoss) > mCurLossProfitScore)//(��̨ӯ�� - ��̨����) �ﵽ��ˮ��
                    {
                        StopCoroutine("_Coro_SaveGainData");
                        ReleaseAllHook();
                        mBss.Dat_GainAdjustIdx.Val = 0;
                        GameOdds.GainRatio = GameOdds.GainRatios[(int)(mBss.GameDifficult_.Val)];//��ԭ����
                        GainAdjust_GainScoreRT.Val = 0;
                        GainAdjust_LossScoreRT.Val = 0;
                        yield break;
                        //Destroy(gameObject);
                    }
                }
            }

        }
  

        /// <summary>
        /// ��Ӧ��̨�����������(���δ���),��ʹ�ü�¼������0
        /// </summary>
        void Handle_BackGroundClearAllDataBefore()
        {
            if (GainAdjust_GainScoreRT == null) GainAdjust_GainScoreRT = new PersistentData<int, int>("GainAdjust_GainScoreRT");
            if (GainAdjust_LossScoreRT == null) GainAdjust_LossScoreRT = new PersistentData<int, int>("GainAdjust_LossScoreRT"); 

            //_Coro_SaveGainData����һֱ������
            StopCoroutine("_Coro_SaveGainData");
            StopCoroutine("_Coro_LossOrProfitprocess");
            mScoreGain = 0;
            mScoreLoss = 0;
            GainAdjust_GainScoreRT.SetImmdiately(0);
            GainAdjust_LossScoreRT.SetImmdiately(0);

            mBss.Dat_GainAdjustIdx.SetImmdiately(GameOdds.CoinPresents[(int)mBss.ArenaType_.Val]);
            //YxDebug.Log("set gain loss score zero");
        }

        void ReleaseAllHook()
        {
            var gdata = App.GetGameData<FishGameData>();
            gdata.EvtPlayerGunFired -= Handle_PlayerFire;
            gdata.EvtPlayerGainScoreFromFish -= Handle_PlayerGainScoreFromFish;
        }
        IEnumerator _Coro_SaveGainData()
        {
            while (true)
            {
                yield return new WaitForSeconds(5F);
                GainAdjust_GainScoreRT.Val = mScoreGain;
                GainAdjust_LossScoreRT.Val = mScoreLoss;
                //Debug.Log("_Coro_SaveGainData gainRatio = " + GameOdds.GainRatio + "   GainAdjust_GainScoreRT = " + GainAdjust_GainScoreRT.Val + "  GainAdjust_LossScoreRT = " + GainAdjust_LossScoreRT.Val);
            } 
        }

        //void OnApplicationQuit()
        //{
        //    mBss = null;
        //}
    }
}
