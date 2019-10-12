using Assets.Scripts.Game.FishGame.Common.Utils;
using Assets.Scripts.Game.FishGame.Common.core;
using Assets.Scripts.Game.FishGame.Fishs;
using UnityEngine;
using System.Collections;
using YxFramwork.Common;

namespace Assets.Scripts.Game.FishGame.Effect
{
    public class Ef_FishDieEffectAdditive : MonoBehaviour {
     
        public AnimationClip Ani_SceneBGShaker; 

        public FlashEffect PerfabSameTypeEffect;


        //private Dictionary<int, Object> mFishAffectCache;
        void Start()
        {
            var gdata = App.GetGameData<FishGameData>();
            gdata.EvtFishKilled += Handle_FishKilled;  
        }

        /// <summary>
        /// ���������Ч
        /// </summary>
        /// <param name="killer"></param>
        /// <param name="bulletScore">�ӵ�����</param>
        /// <param name="fishOddBonus">����</param>
        /// <param name="bulletOddsMulti"></param>
        /// <param name="fish"></param>
        /// <param name="reward"></param>
        void Handle_FishKilled(Player killer, int bulletScore, int fishOddBonus, int bulletOddsMulti, Fish fish,int reward)
        {
            if (fish.TriggerEffect == null) return;
            ShakeBG();
            Bomb(fish.TriggerEffect, fish.transform.position);
        }
 
        /// <summary>
        /// �𶯱���
        /// </summary>
        public void ShakeBG()
        {
            var background = GameMain.Singleton.SceneBGMgr.Background;
            background.GetComponent<Animation>().Play();
        } 

        /// <summary>
        /// ��������Ч
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="worldPos"></param>
        void Bomb(GameObject prefab,Vector3 worldPos)
        {
            var goParFishBomb = Instantiate(prefab);
            worldPos.z = Defines.GlobleDepth_BombParticle;
            goParFishBomb.transform.position = worldPos;
            goParFishBomb.transform.parent = transform;
        }
    }
}
