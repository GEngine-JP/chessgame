using UnityEngine;
namespace Assets.Scripts.Game.brnn3d
{
    public class LuziInfoUIMgr : MonoBehaviour
    {
        public static LuziInfoUIMgr Instance;
        protected void Awake()
        {
            Instance = this;
        }
        private bool[] luzi_info;
        private int luziSize = -1;

        private int _tmpLuzi = -1;

        public LuziInfoUIMgr(bool[] luziInfo)
        {
            luzi_info = luziInfo;
        }

        public void SetLuziInfoUIData()
        {
            _tmpLuzi = luziSize;
            SetLuziInfoUIDataEx();
        }
        //index > 0 往右 反之往左
        public void SetLuziInfoUIData(int index)
        {
            if (luzi_info == null)
            {
                return;
            }
            if (index > 0)
            {
                if (_tmpLuzi < luziSize)
                    _tmpLuzi++;
            }
            else
            {
                if (_tmpLuzi > 10)
                    _tmpLuzi--;
            }
            SetLuziInfoUIDataEx();
        }
        void SetLuziInfoUIDataEx()
        {
            if (_tmpLuzi < 0 || _tmpLuzi > 71)
            {
                return;
            }
            PaiMode.Instance.History();
        }


        public void SetLuziInfoUIDataEy(int id, int index, bool data)
        {
            switch (id)
            {
                case 0:
                    {
                        LuziInfoUI.Instance.SetEastImg(index, data);
                    }
                    break;
                case 1:
                    {
                        LuziInfoUI.Instance.SetSouthImg(index, data);
                    }
                    break;
                case 2:
                    {
                        LuziInfoUI.Instance.SetWestImg(index, data);
                    }
                    break;
                case 3:
                    {
                        LuziInfoUI.Instance.SetNorthImg(index, data);
                    }
                    break;
            }
        }

    }
}

