using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.brnn3d
{
    public class ResetChip : MonoBehaviour
    {
        public List<GameObject> ChipList = new List<GameObject>();
        public GameObject selectIcon;
        private int _chipCnt = 6;
        private int money;
        public void SetChip(int minNum)
        {
            int showChipCnt = 0;
            for (int i = 0; i < ChipList.Count; )
            {
                GameObject obj = ChipList[i];

                if (showChipCnt >= _chipCnt)
                {
                    ChipList.Remove(obj);
                    Destroy(obj);
                    continue;
                }
                if (int.Parse(obj.name) >= minNum)
                {
                    money = int.Parse(obj.name);
                    showChipCnt++;
                    i++;
                }
                else
                {
                    ChipList.Remove(obj);
                    Destroy(obj);
                }
            }
            Invoke("setPos", 0.03f);
        }

        public GameObject gird;
        private void setPos()
        {

            selectIcon.transform.position = ChipList[0].transform.position;
            CommonObject.CurrentSelectChip = ChipList[0];
        }
    }
}