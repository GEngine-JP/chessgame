using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using YxFramwork.Common;

namespace Assets.Scripts.Game.bjl3d
{

    public class PlanScene : MonoBehaviour
    {

        public static PlanScene Instance;

        public TextMesh[] SelfNoteTexts;
        public TextMesh[] QuyuNoteTexts;

        public Transform[] Planes;
       
        /// <summary>
        /// 获取UI操作控件
        /// </summary>
        protected void Awake()
        {
            Instance = this;
        }
        public bool IsPointerOverUIObject()
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }
      
        public void UserNoteDataFun()
        {
            if (App.GetGameData<GlobalData>().UserSeat == App.GetGameData<GlobalData>().CurrentUser.Seat)
                ShowSelfNoteInfo(App.GetGameData<GlobalData>().P, App.GetGameData<GlobalData>().GoldOne);
            ShowquyuNoteInfo(App.GetGameData<GlobalData>().P, App.GetGameData<GlobalData>().GoldOne);

            //筹码显示
            ShowChouMaZhu(App.GetGameData<GlobalData>().P, App.GetGameData<GlobalData>().GoldOne);
        }

        public void ShowChouMaZhu(int iArea, long money)
        {
            int index = 0;
            switch (money)
            {
                case 100:
                    index = 0;
                    break;
                case 1000:
                    index = 1;
                    break;
                case 10000:
                    index = 2;
                    break;
                case 100000:
                    index = 3;
                    break;
                case 1000000:
                    index = 4;
                    break;
                case 5000000:
                    index = 5;
                    break;
                case 10000000:
                    index = 6;
                    break;
            }
            Plan plan = Planes[iArea].GetComponent<Plan>();
            if (plan == null) return;
            plan.XiaZhuChouMaXianShi(index + 1, iArea);
        }

        /// <summary>
        /// 显示注意信息
        /// </summary>
        public void ShowSelfNoteInfo(int area, int gold)
        {

            BetMoneyUI.Intance.BetMoneySelfNoteInfo(area, gold);
        }

        /// <summary>
        /// 显示
        /// </summary>
        void ShowquyuNoteInfo(int area, int gold)
        {
            BetMoneyUI.Intance.BetMoneyquyuNoteInfo(area, gold);
        }

        /// <summary>
        /// 清空下注筹码
        /// </summary>
        public void QingKongChouma()
        {
            for (int i = 0; i < Planes.Length; i++)
            {
                foreach (Transform t in Planes[i])
                {
                    if (t.name.Contains("coin"))
                        Destroy(t.gameObject);
                    Plan plan = Planes[i].GetComponent<Plan>();
                    if (plan == null) return;
                    plan.CoinDic.Clear();
                }
            }
        }

    }
}
