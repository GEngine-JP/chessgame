using System;
using Assets.Scripts.Game.jh.EventII;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.jh.ui
{
    public class JhTableView : MonoBehaviour
    {

        public EventObject EventObj;

        public UILabel SingleBeat;

        public UILabel TotalBeat;

        public GameObject ReadyBtn;

        public GameObject KaiFang;

        public GameObject YuLe;

        public GameObject StartBtn;

        public JhTableInfo TableInfo;

        public GameObject WxShareBtn;

        public JhReadyTip ReadyTip;
        public void OnReceive(EventData data)
        {
            switch (data.Name)
            {
                case "Status":
                    OnStatus(data.Data);
                    break;
                case "Ready":
                    ReadyBtn.SetActive(false);
                    break;
                case "Bet":
                    int totalBet = (int) data.Data;
                    TotalBeat.text = "￥" + YxUtiles.ReduceNumber(totalBet);
                    break;
                case "Result":
                    TotalBeat.text = "￥" + 0;

                    break;
                case "CurrPlayer":
                    OnCurrPlayer(data.Data);
                    break;
                case "Start":
                    OnStart(data.Data);
                    break;
                case "ReadyCountDown":
                    OnReadyCountDown(data.Data);
                    break;
            }
        }

        private void OnReadyCountDown(object p)
        {
            if (ReadyTip != null)
            {
                ReadyTip.Show((int)p);
            }
        }

        private void OnStart(object data)
        {
            bool show = (bool) data;
            StartBtn.SetActive(show);
        }

        protected void OnCurrPlayer(object data)
        {
            ISFSObject eData = (SFSObject)(data);
            int singleBeat = eData.GetInt("SingleBet");
            if (SingleBeat != null)
            {
                SingleBeat.text = YxUtiles.ReduceNumber(singleBeat);    
            }
            
            int curlun = eData.GetInt("CurLun");
            int maxlun = eData.GetInt("MaxLun");
            TableInfo.SetLunShu(curlun,maxlun);
        }

        protected void OnStatus(object data)
        {
            ISFSObject eData = (SFSObject)(data);
            int singleBeat = eData.GetInt("SingleBet");
            if (SingleBeat != null)
            {
                SingleBeat.text = YxUtiles.ReduceNumber(singleBeat);
            }
            TotalBeat.text = "￥" + Convert.ToString(0);
            if (eData.ContainsKey("ShowReady"))
            {
                ReadyBtn.SetActive(true);
            }
            else
            {
                ReadyBtn.SetActive(false);
            }

            if (eData.ContainsKey("ShowWx"))
            {
                if (WxShareBtn!=null)
                    WxShareBtn.SetActive(true);
            }
            else
            {
                if (WxShareBtn != null)
                    WxShareBtn.SetActive(false);
            }

            if (eData.ContainsKey("TotalBeat"))
            {
                int allBeat = eData.GetInt("TotalBeat");
                TotalBeat.text = "￥" + YxUtiles.ReduceNumber(allBeat);
            }

            if (eData.ContainsKey("ShowStart"))
            {
                bool show = eData.GetBool("ShowStart");
                StartBtn.SetActive(show);
            }

            bool isKaiFang = eData.GetBool("IsFangKa");

            if(isKaiFang)
            {
                KaiFang.SetActive(true);
                YuLe.SetActive(false);
                if (TableInfo == null)
                {
                    TableInfo = KaiFang.GetComponent<JhTableInfo>();
                }
                
                int maxju = eData.GetInt("MaxJu");
                int roomId = eData.GetInt("RoomId");
                int curju = eData.GetInt("CurJu");
                int ante = eData.GetInt("Ante");

                TableInfo.SetRoomId(roomId);
                TableInfo.SetJuShu(curju,maxju);
                TableInfo.SetDiFen(ante);
            }
            else
            {
                KaiFang.SetActive(false);
                YuLe.SetActive(true);
                if (TableInfo == null)
                {
                    TableInfo = YuLe.GetComponent<JhTableInfo>();
                }
            }

            int curlun = eData.GetInt("CurLun");
            int maxlun = eData.GetInt("MaxLun");

            TableInfo.SetLunShu(curlun,maxlun);

            if (eData.ContainsKey("HideReadyTip"))
            {
                if (ReadyTip != null)
                {
                    ReadyTip.Hide();
                }
            }

        }
        public void OnBackBtnClick()
        {
            EventObj.SendEvent("GameManagerEvent", "Quit", null);
        }

        public void OnReadyBtnClick()
        {

            EventObj.SendEvent("ServerEvent", "ReadyReq", null);
            ReadyBtn.SetActive(false);
        }

        public void OnChangeRoomBtnClick()
        {
            EventObj.SendEvent("GameManagerEvent", "ChangeRoom", null);
        }

        public void OnSettingBtnClick()
        {
            EventObj.SendEvent("SettingEvent", "Show", null);
        }

        public void OnHelpBtnClick()
        {
            EventObj.SendEvent("HelpEvent", "Show", null);
        }

        public void OnRecordBtnClick()
        {
            EventObj.SendEvent("TableEvent","ShowResult",null);
//            EventObj.SendEvent("ReultViewEvent", "Show", null);
        }

        public void OnKaiFangBackClick()
        {
            EventObj.SendEvent("ServerEvent", "HupReq", 2);
        }

        public void OnRuleInfpClick()
        {
            EventObj.SendEvent("RuleInfoEvent", "Show", null);
        }

        public void OnStartBtnClick()
        {
            StartBtn.SetActive(false);
            EventObj.SendEvent("ServerEvent", "StartReq", null);
        }

        public void OnHallHelpBtnClick()
        {
            
        }

        public void OnWxShare()
        {
            EventObj.SendEvent("ServerEvent", "WXShare", null);
        }
    }
}
