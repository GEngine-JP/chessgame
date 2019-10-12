using System.Collections.Generic;
using Assets.Scripts.Game.jh.EventII;
using com.yxixia.utile.YxDebug;
using UnityEngine;

namespace Assets.Scripts.Game.jh.ui
{
    public class JhPlayerPosition : MonoBehaviour
    {

        public List<Transform> PostionObjectList;

        public List<Transform> PlayerObject;

        public List<string> PositionInfo;


        public void Start()
        {
            
        }


        public void OnRecive(EventData data)
        {
            string name = data.Name;
            if(name.Equals("ResetPos"))
            {
                int cnt = (int) data.Data;
                OnResetPos(cnt);
            }
        }

        protected void OnResetPos(int cnt)
        {
            
        }

    }
}

