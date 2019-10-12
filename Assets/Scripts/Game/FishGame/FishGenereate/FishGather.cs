using Assets.Scripts.Game.FishGame.Common.core;
using Assets.Scripts.Game.FishGame.Common.external.NemoFileIO;
using Assets.Scripts.Game.FishGame.Fishs;
using Assets.Scripts.Game.FishGame.Users;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.FishGame.FishGenereate
{
    /// <summary>
    /// ���ռ�ϵͳ
    /// </summary>
    /// <remarks>
    /// ע��:
    /// 1.���ռ��㲻��ͬһ��Ļͬʱ��������,Ҫ��������������ʱ������(��һ��).
    /// </remarks>
    public class FishGather : MonoBehaviour {
        //�¼�
        public delegate void Evt_PlayerGatherFish(Player player, Fish fish,int gatherIdx);
        public delegate void Evt_PayBonus(Player player,int bouns);

        //�� ��ʼ�����ĳ�������ռ�״̬ ʱ����
        public Evt_PlayerGatherFish EvtPlayerGatherFishInited;

        //�� ����ռ���ָ���� ʱ����
        public Evt_PlayerGatherFish EvtPlayerGatherFishActive;

        //�� ����ռ��������� ʱ����,����:fishΪ��,gatherIdxΪ-1
        public Evt_PlayerGatherFish EvtPlayerGatheredAllFish;
    
        //�ɷ�����ʱ
        public Evt_PayBonus EvtPayBonus;
    
        //����
        public Fish[] Prefab_GatherFishAry;//�ռ������б�(��PersistentData<uint, uint> PlayerGatheredFishRecs���Ӧ,���32��)
        public int OddAdditivePerFish = 10;//ÿ�����⽱��,�������̫��,����Ǹо��������ĳ̶�
        public float TimeDelayBonus = 4F;//������ʱʱ��
        [System.NonSerialized]
        private PlayerBatterys PlayersBatterys;
        public PersistentData<uint, uint>[] PlayerGatheredFishRecs;//���ռ����¼,�ṹ(���ҵ���һ��bit�������ռ�����Ӧ��������)
        public PersistentData<int, int>[] PlayerGatheredScore;//���ռ�����

        public Dictionary<int, object>[] PlayerGatheredFish;//��PlayerGatheredFishRecs����,keyֵ���ռ����б������
        private Dictionary<int, int> mPrefabNeedGatherFishCache;//��Prefab_GatherFishAry����,�ռ����б�, key:��typeIdx ,value:�ռ�����

        private bool mCanEditFishRecored = true;//�Ƿ���޸����¼״̬(���ڷ�ֹ��ͬһ֡��ɱ��������,�������ɲ��������������ռ��������[��һ���])
        /// <summary>
        /// ��Ҫ�ռ�����Ŀ
        /// </summary>
        public int CountFishNeedGather
        {
            get { return Prefab_GatherFishAry.Length; }
        }
        void Awake () {
            HitProcessor.AddFunc_Odd(Func_GetFishOddAddtiveForDieRatio,null); 
            PlayersBatterys = GameMain.Singleton.PlayersBatterys;
            mPrefabNeedGatherFishCache = new Dictionary<int, int>();
            for (int i = 0; i != Prefab_GatherFishAry.Length; ++i)
                mPrefabNeedGatherFishCache.Add(Prefab_GatherFishAry[i].TypeIndex, i);

            //����PersistentData
            if (PlayerGatheredFishRecs == null)
            {
                PlayerGatheredFishRecs = new PersistentData<uint, uint>[Defines.MaxNumPlayer];
                for (int i = 0; i != Defines.MaxNumPlayer; ++i)
                    PlayerGatheredFishRecs[i] = new PersistentData<uint, uint>("PlayerGatheredFishRecs" + i.ToString());
            }
            if (PlayerGatheredScore == null)
            {
                PlayerGatheredScore = new PersistentData<int, int>[Defines.MaxNumPlayer];
                for (int i = 0; i != Defines.MaxNumPlayer; ++i)
                    PlayerGatheredScore[i] = new PersistentData<int, int>("PlayerGatheredScore" + i.ToString());
            }
            //���� PlayerGatheredFish
            PlayerGatheredFish = new Dictionary<int, object>[Defines.MaxNumPlayer];
        

            //��PlayerGatheredFishRec�ó��ֵ�PlayerGatheredFish
            for (int playerIdx = 0; playerIdx != Defines.MaxNumPlayer; ++playerIdx)
            {
                PlayerGatheredFish[playerIdx] = new Dictionary<int, object>();

                for (int gIdx = 0; gIdx != Prefab_GatherFishAry.Length; ++gIdx)//�ռ�����
                {
                    if (((PlayerGatheredFishRecs[playerIdx].Val >> gIdx) & 1) == 1)
                    {
                        PlayerGatheredFish[playerIdx].Add(gIdx,null);
                        //_tmpAddViewAniFish(gIdx, Players[playerIdx];
                    }
                }
            }


        }

        void Start()
        {
            //������start�����¼�,�����awake����,�������п�����Ӧ����
            for (int playerIdx = 0; playerIdx != Defines.MaxNumPlayer; ++playerIdx)
            {
                foreach (int gIdx in PlayerGatheredFish[playerIdx].Keys)
                {
                    if (EvtPlayerGatherFishInited != null)
                        EvtPlayerGatherFishInited(PlayersBatterys[playerIdx], Prefab_GatherFishAry[gIdx], gIdx);
                }
            }
        }
        /// <summary>
        /// ����ĳ����ռ���ļ�¼
        /// </summary>
        /// <param name="playerIdx"></param>
        /// <param name="gatherIdx">�ռ�����</param>
        /// <param name="isClear">�Ƿ�����ռ���¼</param>
        void UpdatePlayerGatheredFishRecs(int playerIdx,int gatherIdx,bool isClear)
        {
            if(PlayerGatheredFish[playerIdx] == null)
            {
                YxDebug.LogError(string.Format("[FishGather] UpdatePlayerGatheredFishRecs ����ʧ��.PlayerGatheredFish[{0}] δ��ʼ��", playerIdx));
                return;
            }
            if(PlayerGatheredFishRecs[playerIdx] == null)
            {
                YxDebug.LogError(string.Format("[FishGather] UpdatePlayerGatheredFishRecs ����ʧ��.PlayerGatheredFishRecs[{0}] δ��ʼ��", playerIdx));
                return;
            }

            if (isClear)
            {
                PlayerGatheredFish[playerIdx].Clear();
                PlayerGatheredFishRecs[playerIdx].Val = 0;
                return;
            }

            PlayerGatheredFish[playerIdx].Add(gatherIdx, null);

            uint recordNew = 0;
            foreach(KeyValuePair<int,object> kvp in PlayerGatheredFish[playerIdx])
            {
                recordNew |= ((uint)1 << kvp.Key);
            }

            PlayerGatheredFishRecs[playerIdx].Val = recordNew;
        }

        /// <summary>
        /// ��Ӧ���ڼ������������ⱶ�ʵĺ���
        /// </summary>
        /// <param name="killer"></param>
        /// <param name="b"></param>
        /// <param name="f"></param>
        /// <param name="fCauser"></param>
        /// <returns></returns>
        /// <remarks>
        /// ע��:
        /// 1.����ȫ��ը������ͬ��ը��ͬʱɱ������ͬһTypeIdx����ʱ,�������ʶ�������,����ȷ����Ӧ����ֻ������һ�����ӱ���.�������:���ռ���ͬһ��Ļ����ͬʱ��������
        /// </remarks>
        HitProcessor.OperatorOddFix Func_GetFishOddAddtiveForDieRatio(Player killer, Bullet b, Fish f, Fish fCauser)
        {
            //�ж��ڲ�����Ҫ�ռ��б���
            if (!mPrefabNeedGatherFishCache.ContainsKey(f.TypeIndex))
                return null;
            //�ж��ڲ������ռ��б���
            int gIdx = mPrefabNeedGatherFishCache[f.TypeIndex];//��ǰ����ռ�����
            if (PlayerGatheredFish[killer.Idx].ContainsKey(gIdx))
                return null;
            //Debug.Log(string.Format("����{0} ���10�Ѷ�",f.name));
            //���ռ��б�,�Ҳ������ռ��б� ����Ӷ��ⱶ��
            return new HitProcessor.OperatorOddFix(HitProcessor.Operator.Add,OddAdditivePerFish);
        }

        /// <summary>
        /// ��Ӧ�������¼�
        /// </summary>
        /// <param name="killer"></param>
        /// <param name="b"></param>
        /// <param name="f"></param>
        void Evt_FishKilled(Player killer, Bullet b, Fish f)
        {
            if (!mCanEditFishRecored)
                return;

            //�ж��ڲ����ռ��б���
            if (!mPrefabNeedGatherFishCache.ContainsKey(f.TypeIndex))
                return;

            int gIdx = mPrefabNeedGatherFishCache[f.TypeIndex];//��ǰ����ռ�����

            //�ж��ڲ������ռ��б���
            if (PlayerGatheredFish[killer.Idx].ContainsKey(gIdx))
                return;

            //�ж��Ƿ��ռ������һ��,���������Ҫ���,���ų�����
            if (PlayerGatheredFish[killer.Idx].Count + 1 == Prefab_GatherFishAry.Length)
            {
                //�ų�����
                StartCoroutine(_Coro_DelayPayBonus(killer.Idx, PlayerGatheredScore[killer.Idx].Val + b.Score * OddAdditivePerFish));
            
                UpdatePlayerGatheredFishRecs(killer.Idx, 0, true);
                PlayerGatheredScore[killer.Idx].Val = 0;
                 
                //Debug.Log("���һ������,�ų�����,����ռ����¼");
                if (EvtPlayerGatherFishActive != null)
                    EvtPlayerGatherFishActive(PlayersBatterys[killer.Idx], Prefab_GatherFishAry[gIdx], gIdx);

                if (EvtPlayerGatheredAllFish != null)
                    EvtPlayerGatheredAllFish(PlayersBatterys[killer.Idx], null, -1);

                StartCoroutine(_Coro_CoolDownRecodeFish());
            }
            else
            {
                //���ռ��б��Ҳ������ռ��б��,���Ϊ���ռ�״̬����¼����
                UpdatePlayerGatheredFishRecs(killer.Idx, gIdx, false);
                PlayerGatheredScore[killer.Idx].Val += b.Score * OddAdditivePerFish;

                if (EvtPlayerGatherFishActive != null)
                    EvtPlayerGatherFishActive(PlayersBatterys[killer.Idx], Prefab_GatherFishAry[gIdx], gIdx);
            }
        }
        IEnumerator _Coro_CoolDownRecodeFish()
        {
            mCanEditFishRecored = false;
            yield return new WaitForSeconds(1F);
            mCanEditFishRecored = true;
        }
        /// <summary>
        /// ��Ӧ��̨(���δ���)�����������
        /// </summary>
        void Evt_BGClearAllData_Before()
        {
            for (int i = 0; i != Defines.MaxNumPlayer; ++i)
            {
                UpdatePlayerGatheredFishRecs(i, 0, true);
                PlayerGatheredScore[i].Val = 0;
            }
        }
         
        IEnumerator _Coro_DelayPayBonus(int playerIdx,int score)
        {
            yield return new WaitForSeconds(TimeDelayBonus);
            if (PlayersBatterys[playerIdx].Idx != playerIdx)
            {
                YxDebug.LogError("[FishGather] _Coro_DelayPayBonus,���playerIdx����.");
                yield break;
            }

            //Debug.Log(string.Format("���{0:d} �ɷ����� = {1:d}", playerIdx, score));
            PlayersBatterys[playerIdx].GainScore(score, 100, score / 100);

            if (EvtPayBonus != null)
                EvtPayBonus(PlayersBatterys[playerIdx], score);

        }
    }
}
