using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.brnn3d
{
    public class DownUIController : MonoBehaviour
    {
        public static DownUIController Instance;
        public GameObject CoinBtnUI;
        public GameObject Select;

        public void OnBetClick(GameObject go)
        {
            Select.transform.position = go.transform.position;
            CommonObject.CurrentSelectChip = go;
        }

        //点击事件
        public void OnClick(int num)
        {
            App.GetGameData<GlobalData>().ChouMaType = num;
        }

        public void Awake()
        {
            Instance = this;
        }

        public void ResetChip(ISFSObject responseData)
        {
            if (!responseData.ContainsKey("ante")) return;
            var ante = responseData.GetInt("ante");
            CoinBtnUI.GetComponent<ResetChip>().SetChip(ante);
            for (int i = 0; i < App.GetGameData<GlobalData>().I64ChoumaValue.Length; i++)
            {
                if (App.GetGameData<GlobalData>().I64ChoumaValue[i] >= ante)
                {
                    App.GetGameData<GlobalData>().ChouMaType = i;
                    break;
                }
              
//                var slect = transform.Find("App.GetGameData<GlobalData>().I64ChoumaValue");
//               Select.transform.position = slect.transform.position;
//               CommonObject.CurrentSelectChip =slect.gameObject;
//               
            }
           
        }

        //路子信息左点击
        public void LuziLeftBtn()
        {
            PaiMode.Instance.SetLuziInfoUIData(-1);
        }

        //路子信息右点击
        public void LuziRightBtn()
        {
            PaiMode.Instance.SetLuziInfoUIData(1);
        }

        //显示历史纪录的面板
        public void DownUILeftUIOn_OffClicked(bool isKai)
        {
            App.GetGameData<GlobalData>().IsKai = isKai;
            DownUILeftUIOn_OffClicked();
        }

        public void DownUILeftUIOn_OffClicked()
        {
            if (App.GetGameData<GlobalData>().IsKai)
            {
                DownUILeftBg.Instance.HideDownUILeftBg();
                DownUILeftUIOn_Off.Instance.ShowKaiBtn();
                App.GetGameData<GlobalData>().IsKai = false;
            }
            else
            {
                DownUILeftBg.Instance.ShowDownUILeftBg();
                DownUILeftUIOn_Off.Instance.ShowGuanBtn();
                App.GetGameData<GlobalData>().IsKai = true;
            }
        }
    }


}

