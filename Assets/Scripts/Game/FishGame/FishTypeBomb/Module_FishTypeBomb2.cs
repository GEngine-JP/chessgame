using Assets.Scripts.Game.FishGame.Common.core;
using Assets.Scripts.Game.FishGame.Fishs;
using UnityEngine;

namespace Assets.Scripts.Game.FishGame.FishTypeBomb
{
    public class Module_FishTypeBomb2 : MonoBehaviour {
      
        void Start()
        {
            HitProcessor.AddFunc_Odd(Func_GetFishOddAdditive, Func_GetFishOddAdditive);
        }


        HitProcessor.OperatorOddFix Func_GetFishOddAdditive(Player killer, Bullet b, Fish f, Fish fCauser)
        {
            //if (fCauser.HittableTypeS == "SameTypeBomb")//��������ͬ��ը��.����������С��,��ʹû�д�FishEx_OddsMulti,Ҳ��Ҫ�˱� 
            if (fCauser.HittableType == HittableType.SameTypeBomb)
            {
                var cpOddMulti = fCauser.GetComponent<FishEx_OddsMulti>();

                if (cpOddMulti == null || cpOddMulti.OddsMulti == 1)
                    return null;

                return new HitProcessor.OperatorOddFix(HitProcessor.Operator.LastModule, cpOddMulti.OddsMulti);
            }
            //if (f.HittableTypeS == "SameTypeBomb")//�п��ܳ���causer��SameTypeBombEx,��ΪSameTypeBombEx����SameTypeBomb�Ĵ�����
            if (f.HittableType == HittableType.SameTypeBomb)
            {
                var cpOddMulti = f.GetComponent<FishEx_OddsMulti>();

                if (cpOddMulti == null || cpOddMulti.OddsMulti == 1)
                    return null;

                return new HitProcessor.OperatorOddFix(HitProcessor.Operator.LastModule, cpOddMulti.OddsMulti);
            } 
            return null;
        
        }
    }
}
