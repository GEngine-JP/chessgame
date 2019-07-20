using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using YxFramwork.Common;
using YxFramwork.Manager;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.brnn3d
{
    public class PaiMode : MonoBehaviour
    {
        public static PaiMode Instance;
        public Transform[] PaiEasts = new Transform[5];
        public Transform[] PaiSouths = new Transform[5];
        public Transform[] PaiWests = new Transform[5];
        public Transform[] PaiNorths = new Transform[5];
        public Transform[] PaiZhuangs = new Transform[5];

        public Transform PaiFirstTf;
        public Transform PaiSecondTf;

        private int[] niuNum = new int[5];
        private bool[] Result = new bool[5];
        protected void Awake()
        {
            Instance = this;
        }

        public int JiSuan;

        public void BeginGiveCards()
        {
            StartCoroutine("GiveCards");
        }

        IEnumerator GiveCards()
        {
            yield return new WaitForSeconds(2f);
            var res = 0;
            for (int i = 0; i < niuNum.Length; i++)
            {
                niuNum[i] = App.GetGameData<GlobalData>().Nn.GetSFSObject(i).GetInt("niu");
                Result[i] = App.GetGameData<GlobalData>().Nn.GetSFSObject(i).GetBool("win");
                if (App.GetGameData<GlobalData>().Nn.GetSFSObject(i).GetBool("win"))
                {
                    res |= (1 << i);
                    //res = res |(1 << i);
                }
            }
            Store.Add(res);
            if (Replace == JiSuan)
            {
                Replace++;
            }
            JiSuan++;
            PaiModeMgr.Instance.SetPaiModeDataEx(); //设置发牌的数据
        }

        public int Replace;
        public int Pos = 0;
        public List<int> Store = new List<int>();
        public void History()
        {
            if (Replace == 0)
            {
                return;
            }

            LuziInfoUI.Instance.InitImg();

            int pos = Replace > 10 ? Replace - 10 : 0;
            for (int i = pos; i < Replace; i++)
            {
                var cur = Store[i];
                for (int j = 0; j < 4; j++)
                {
                    LuziInfoUIMgr.Instance.SetLuziInfoUIDataEy(j, i - pos, ((cur >> j + 1) & 1) == 1);
                }
            }

        }
        public void SetLuziInfoUIData(int index)
        {
            if (index > 0)
            {
                if (Replace < JiSuan)
                {
                    Replace++;
                }
            }
            else
            {
                if (Replace > 10)
                    Replace--;

            }
            History();
        }

        public void InstancePai(int paiIndex, int iArea,/* int paiPoint,*/ int pai25Indxe)
        {
            int[] cards = App.GetGameData<GlobalData>().Cards.GetIntArray(iArea);
            Transform areaTf = null;
            switch (iArea)
            {
                case 0:
                    areaTf = PaiZhuangs[paiIndex];
                    break;
                case 1:
                    areaTf = PaiEasts[paiIndex];
                    break;
                case 2:
                    areaTf = PaiSouths[paiIndex];
                    break;
                case 3:
                    areaTf = PaiWests[paiIndex];
                    break;
                case 4:
                    areaTf = PaiNorths[paiIndex];
                    break;
            }
            if (areaTf == null) YxDebug.LogError("No Such Object" + iArea);

            var go = ResourceManager.LoadAsset("Pai_0" + cards[paiIndex].ToString("X"), "brnnpai");
            var go1 = Instantiate(go);
            Transform obj = go1.transform;
            obj.gameObject.SetActive(true);
            if (areaTf != null) obj.transform.parent = areaTf.parent;
            obj.transform.localScale = new Vector3(0, 0, 0);
            if (paiIndex == 3)
            {
                obj.localEulerAngles = new Vector3(0, PaiFirstTf.localEulerAngles.y, 180);
                if (App.GetGameData<GlobalData>().PaiAllShow.ContainsKey(iArea))
                {
                    YxDebug.LogError("Error Here");
                }
                else
                {
                    App.GetGameData<GlobalData>().PaiAllShow.Add(iArea, obj);
                }
            }
            else
                obj.localEulerAngles = new Vector3(0, PaiFirstTf.localEulerAngles.y, 0);

            if (areaTf != null) obj.localPosition = areaTf.localPosition;

            Pai pai = obj.GetComponent<Pai>();
            if (pai == null) YxDebug.LogError("No Such Component");
            if (pai != null) pai.Show(pai25Indxe, iArea, paiIndex);
        }

        public struct Pp
        {
            public int AreaId;
            public Transform Tf;
            public float S;
        }
        //翻牌阶段显示中奖区域
        public void FanPaiFun()
        {
            int tmp = App.GetGameData<GlobalData>().SendCardPosition;
            StartCoroutine("ToShwoZhongJiangArea", 7.5f);
            for (int i = 0; i < 5; i++)
            {
                var pP = new Pp();
                pP.AreaId = tmp;
                pP.Tf = App.GetGameData<GlobalData>().PaiAllShow[tmp];
                pP.S = i * 1.2f;

                StartCoroutine("ToFanPai", pP);
                tmp += 1;
                if (tmp > 4)
                    tmp = 0;
            }
        }

        private IEnumerator ToFanPai(Pp p)
        {
            yield return new WaitForSeconds(p.S);
            p.Tf.localEulerAngles = new Vector3(0, 0, 0);
            Pai pai = p.Tf.GetComponent<Pai>();
            if (pai != null) pai.PlayFanPaiAni();
            yield return new WaitForSeconds(1.2f);
            NiuNumberUI.Instance.ShowNumberUI(niuNum);

            NiuNumberUI.Instance.ShowAreaNiu(p.AreaId);
            NiuNumberUI.Instance.PlayAudioNiuJi(p.AreaId, niuNum);
        }

        private IEnumerator ToShwoZhongJiangArea(float s)
        {
            yield return new WaitForSeconds(s);
            for (int i = 0; i < 4; i++)
            {
                ZhongJiangMode.Instance.ShowZhongJiangEffect(i, Result);
            }
        }
        //删除牌的列表
        public void DeletPaiList()
        {
            DeletePaiItemListFromParent(PaiEasts[0].parent);
            DeletePaiItemListFromParent(PaiSouths[0].parent);
            DeletePaiItemListFromParent(PaiWests[0].parent);
            DeletePaiItemListFromParent(PaiNorths[0].parent);
            DeletePaiItemListFromParent(PaiZhuangs[0].parent);
        }
        //从牌组的父物体下删除牌
        private void DeletePaiItemListFromParent(Transform parent)
        {
            foreach (Transform tf in parent)
            {
                if (tf.name.Contains("Pai1") || tf.name.Contains("Pai2") || tf.name.Contains("Pai3") ||
                    tf.name.Contains("Pai4") || tf.name.Contains("Pai5")) continue;
                Destroy(tf.gameObject);
            }
        }
    }
}


