using UnityEngine;

namespace Assets.Scripts.Game.brnn3d
{
    public class CardMachine : MonoBehaviour
    {
        public static CardMachine Instance;
        public Animator ZhuanAni;
        public void Awake()
        {
            Instance = this;
        }

        //发牌器的转动
        public void CardMachinPlay()
        {
            ZhuanAni.Play("fapq");
        }
    }
}

