using Assets.Scripts.Game.FishGame.Common.core;
using Assets.Scripts.Game.FishGame.Fishs;
using UnityEngine;

namespace Assets.Scripts.Game.FishGame.DragonSlayers
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// ע��:
    /// 1.�ýű����ܺ�fish����ͬһ���ű�,��ΪFish���Ҫ��ӦOnTriggerEnter�¼��Ļ���Ҫ��collider,����collider�Ļ��ͻ���ӵ�����
    /// 
    /// </remarks>
    public class FishEx_DragonSlayer : MonoBehaviour {
        //public Fish[] SlayFish;//ɱ������,
        public Player Owner;//������
        public DragonSlayer Creator;//������

        void OnTriggerEnter(Collider other)
        {
            Fish f = other.GetComponent<Fish>();
            if (f != null)
            {
                Creator.On_FishExDragonSlayerTouchFish(this,f, Owner);
            }
        }

        public void Clear()
        {
            Fish f = transform.parent.GetComponent<Fish>();
        
            f.Clear();
            //Destroy(transform.parent.gameObject);
        }

    }
}
