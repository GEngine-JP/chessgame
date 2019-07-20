using UnityEngine;
using System.Collections.Generic;
using YxFramwork.Common;

namespace Assets.Scripts.Game.brnn3d
{
    public class PaiModeMgr : MonoBehaviour
    {
        public static PaiModeMgr Instance;
        private int[] _pais;

        private int _paiIndex;

        private Dictionary<int, int> beefPoint = new Dictionary<int, int>();

        protected void Awake()
        {
            Instance = this;
            _pais = new int[5];
        }
        //设置发牌数据
        public void SetPaiModeDataEx()
        {
            int tmp = App.GetGameData<GlobalData>().SendCardPosition;
            _paiIndex = 0;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 1; j < 6; j++)
                {
                    PaiMode.Instance.InstancePai(i, tmp/*, (tmp + i)%5*/, _paiIndex);
                    tmp += 1;
                    if (tmp > 4)
                        tmp = 0;
                    _paiIndex += 1;
                }
            }
        }
        void GetBeefPoint(int area, int paiP)
        {
            if (beefPoint.ContainsKey(area))
            {
                if (paiP > 10) paiP = 10;
                beefPoint[area] += paiP;
            }
            else
            {
                beefPoint.Add(area, paiP);
            }
        }
    }
}

