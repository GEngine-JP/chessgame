using System.Collections.Generic;
using Assets.Scripts.Game.jh.EventII;
using Assets.Scripts.Game.jh.Image;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Controller;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.jh.ui
{
    public class JhTtResultView : MonoBehaviour
    {
        public EventObject EventOjb;

        public GameObject View;

        public GameObject Item;

        public UIGrid Grid;

        public CompressImg Img;

        public UILabel RoomId;
        public UILabel Time;
        public UILabel Ju;
        public UILabel Ante;
        public UILabel MaxLun;
        public void OnRecieve(EventData data)
        {
            switch (data.Name)
            {
                case "TtResultInfo":
                    OnTtResultInfo(data.Data);
                    break;
                case "Show":
                    View.SetActive(true);
                    break;
            }
        }

        private void OnTtResultInfo(object data)
        {
            bool isHideResult = false;
            EventOjb.SendEvent("ReultViewEvent","IsHide", new EventDelegate(() =>
            {
                isHideResult = true;
            }));

            if (!isHideResult)
            {
                View.SetActive(true);
            }

            ISFSObject infoObj = (ISFSObject) data;

            string time = infoObj.GetUtfString("Time");
            int maxLun = infoObj.GetInt("MaxLun");
            int roomId = infoObj.GetInt("RoomID");
            int ante = infoObj.GetInt("Ante");
            int ju = infoObj.GetInt("Ju");

            if (Time != null)
            {
                Time.text = time;
            }
            if (MaxLun != null)
            {
                MaxLun.text = "" + maxLun;
            }
            if (RoomId != null)
            {
                RoomId.text = "" + roomId;
            }
            if (Ante != null)
            {
                Ante.text = "" + ante;
            }
            if (Ju != null)
            {
                Ju.text = "" + ju;
            }
            ISFSArray arr = infoObj.GetSFSArray("Users");

            List<Transform> itemBgList = Grid.GetChildList();

            for (int i = 0; i < arr.Count; i++)
            {
                ISFSObject obj = arr.GetSFSObject(i);
                string name = obj.GetUtfString("Name");
                string avatar = obj.GetUtfString("Avatar");
                int sex = obj.GetInt("Sex");
                int wincnt = obj.GetInt("WinCnt");
                int lostcnt = obj.GetInt("LostCnt");
                int bipaicnt = obj.GetInt("BipaiCnt");
                int gold = obj.GetInt("Gold");
                bool bigwinner = obj.GetBool("IsBigWinner");
                int uid = obj.GetInt("UID");

                GameObject item = Instantiate(Item);
                item.SetActive(true);
                if (itemBgList.Count>=i+1)
                {
                    item.transform.parent = Grid.GetChild(i);
                    item.transform.localPosition = Vector3.zero;
                }
                else
                {
                    Grid.AddChild(item.transform);    
                }
                item.transform.localScale = Vector3.one;
                JhTtResultItem itemscp = item.GetComponent<JhTtResultItem>();
                itemscp.SetInfo(name, avatar, sex, bigwinner, gold, wincnt, lostcnt, uid);
            }
        }


        public void OnBackToHall()
        {
            EventOjb.SendEvent("GameManagerEvent", "Quit", null);
        }

        public void OnShare()
        {
            YxWindowManager.ShowWaitFor();

            Facade.Instance<WeChatApi>().InitWechat();

            UserController.Instance.GetShareInfo(info =>
            {
                YxWindowManager.HideWaitFor();

                Img.DoScreenShot(new Rect(0, 0, Screen.width, Screen.height), imageUrl =>
                {
                    if (Application.platform == RuntimePlatform.Android)
                    {
                        imageUrl = "file://" + imageUrl;
                    }
                    info.ImageUrl = imageUrl;
                    info.ShareType = ShareType.Image;
                    Facade.Instance<WeChatApi>().ShareContent(info, str =>
                    {
                        var parm = new Dictionary<string, object>
                            {
                            {"option",2},
                            {"gamekey",App.GameKey},
                            {"bundle_id",Application.bundleIdentifier},
                            {"share_plat",SharePlat.WxSenceTimeLine.ToString() },
                        };
                        Facade.Instance<TwManager>().SendAction("shareAwards", parm, null);
                    });
                });
            });
        }
    }
}
