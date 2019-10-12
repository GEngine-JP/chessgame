using System;
using System.Collections.Generic;
using Assets.Scripts.Game.jh.EventII;
using DG.Tweening.Plugins;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.jh.Modle
{
    public class JhTtResultInfo : MonoBehaviour
    {

        public EventObject EventObj;

        public class TtResultItem
        {
            public string Name;
            public string Avatar;
            public int Sex;
            public int WinCnt;
            public int LostCnt;
            public int BipaiCnt;
            public int Gold;
            public bool IsBigWinner;
            public int UId;

            public ISFSObject GetSfsObj()
            {
                ISFSObject obj = SFSObject.NewInstance();
                obj.PutUtfString("Name",Name);
                obj.PutUtfString("Avatar", Avatar);
                obj.PutInt("Sex",Sex);
                obj.PutInt("WinCnt", WinCnt);
                obj.PutInt("LostCnt", LostCnt);
                obj.PutInt("BipaiCnt", BipaiCnt);
                obj.PutInt("Gold", Gold);
                obj.PutBool("IsBigWinner",IsBigWinner);
                obj.PutInt("UID", UId);
                return obj;
            }
        }

        protected List<TtResultItem> ItemList = new List<TtResultItem>();

        public void OnRecieve(EventData data)
        {
            switch (data.Name)
            {
                case "TtResult":
                    OnTtResult(data.Data);
                    break;
            }
        }

        private void OnTtResult(object data)
        {
            ItemList.Clear();

            JhGameTable GameData = App.GetGameData<JhGameTable>();
            
            ISFSObject obj = (ISFSObject) data;

            ISFSArray usersArr = obj.GetSFSArray("users");
            long time = obj.GetLong("svt");
            for (int i = 0; i < usersArr.Count; i++)
            {
                ISFSObject objItem = usersArr.GetSFSObject(i);
                if (objItem.GetKeys().Length > 0)
                {
                    var rItem = new TtResultItem
                    {
                        Name = objItem.GetUtfString("nick"),
                        Avatar = objItem.GetUtfString("avatar"),
                        Sex = objItem.GetShort("sex"),
                        Gold = objItem.GetInt("gold"),
                        WinCnt = objItem.GetInt("win"),
                        LostCnt = objItem.GetInt("lose"),
                        BipaiCnt = objItem.GetInt("abandon"),
                        UId =  objItem.GetInt("id")
                    };
                    ItemList.Add(rItem);
                }
            }
            int bigWiner = 0;
            int cnt = 0;
            for(int i = 0;i<ItemList.Count;i++)
            {
                TtResultItem item = ItemList[i];
                if (item.WinCnt > cnt)
                {
                    bigWiner = i;
                    cnt = item.WinCnt;
                }
            }

            ItemList[bigWiner].IsBigWinner = true;

            ISFSArray sendArr = SFSArray.NewInstance();
            foreach (TtResultItem item in ItemList)
            {
                sendArr.AddSFSObject(item.GetSfsObj());
            }
            ISFSObject sendObj = SFSObject.NewInstance();
            sendObj.PutSFSArray("Users",sendArr);
            sendObj.PutUtfString("Time",DateTime.FromBinary(time).ToString());
            sendObj.PutInt("MaxLun", GameData.maxRound);
            sendObj.PutInt("RoomID",GameData.CreateRoomInfo.RoomId);
            sendObj.PutInt("Ante",GameData.AnteRate[0]);
            sendObj.PutInt("Ju",GameData.CreateRoomInfo.CurRound);
            EventObj.SendEvent("TtResultViewEvent", "TtResultInfo", sendObj);
        }
    }
}
