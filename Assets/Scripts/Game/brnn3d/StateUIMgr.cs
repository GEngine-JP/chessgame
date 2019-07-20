using UnityEngine;

namespace Assets.Scripts.Game.brnn3d
{
    public class StateUIMgr : MonoBehaviour
    {
        public static StateUIMgr Instance;
        protected void Awake()
        {
            Instance = this;
        }
        //无庄、空闲、下注、开牌、结算===0、1、2、3、4
        private int _stateId;

        void GetStateId(int stateId)
        {
            switch (stateId)
            {
                case 0:
                    _stateId = 0;
                    break;
                case 1:
                    _stateId = 0;
                    break;
                case 20:
                    _stateId = 1;
                    break;
                case 21:
                    _stateId = 2;
                    break;
                case 22:
                    _stateId = 3;
                    break;
                case 23:
                    _stateId = 4;
                    break;
                case 24:
                    _stateId = 1;
                    break;
            }
        }

        void SetStateUIDateEx()
        {
            StateUI.Instance.SetStateUI(_stateId);
        }

    }
}

