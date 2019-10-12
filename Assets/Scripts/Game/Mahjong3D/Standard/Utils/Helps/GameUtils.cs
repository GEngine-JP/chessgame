﻿using Assets.Scripts.Common.Utils;
using System.Collections.Generic;
using YxFramwork.Framework.Core;
using YxFramwork.Controller;
using YxFramwork.Manager;
using YxFramwork.Tool;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class GameUtils
    {
        /// <summary>
        /// 检测娱乐房 切换房间时停止所有任务
        /// </summary>
        /// <returns></returns>
        public static bool CheckStopTask()
        {
            return GameCenter.DataCenter.Room.RoomType == MahRoomType.YuLe && GameCenter.DataCenter.Room.YuLeBoutState;
        }

        /// <summary>
        /// 二进制检测是否包含查询数值
        /// </summary>
        /// <param name="value">需要检测的值</param>
        /// <param name="source">源</param>
        /// <returns></returns>
        public static bool BinaryCheck(int value, int source)
        {
            return (source & value) == value;
        }

        /// <summary>
        /// 收集麻将牌值 
        /// </summary>
        public static List<int> ConverToCards(IList<MahjongContainer> mahjongs)
        {
            var cards = new List<int>();
            for (int i = 0; i < mahjongs.Count; i++)
            {
                cards.Add(mahjongs[i].Value);
            }
            return cards;
        }

        /// <summary>
        /// 获取其他玩家相对自己的方向
        /// </summary>
        /// <param name="ownChair">own client chair</param>
        /// <param name="otherChair">other client chair</param>
        /// <returns></returns>
        public static RelativeSeat GetRelativeSeat(int ownChair, int otherChair)
        {
            int[] array = (int[])System.Enum.GetValues(typeof(RelativeSeat));
            //牌桌座位
            int ownTableChair = ownChair.ExChairC2T();
            int otherTableChair = otherChair.ExChairC2T();

            for (int i = 0; i < array.Length; i++)
            {
                var tableChair = (ownTableChair + array[i]) % 4;
                if (tableChair == otherTableChair)
                {
                    return (RelativeSeat)array[i];
                }
            }
            //默认对家
            return RelativeSeat.None;
        }

        /// <summary>
        /// 微信分享内容后台配置
        /// </summary>
        public static void WechatShareTempate()
        {
            var db = GameCenter.DataCenter;
            YxTools.ShareFriend(db.Room.RoomID.ToString(), db.Config.DefaultGameRule);
        }

        /// <summary>
        /// 微信分享内容固定格式
        /// </summary>
        public static void WechatShare()
        {
            var db = GameCenter.DataCenter;
            var dic = new Dictionary<string, object>();
            dic.Add("type", 0);
            dic.Add("sharePlat", 0);
            dic.Add("event", "findroom");
            dic.Add("roomid", db.Room.RoomID);
            dic.Add("roomRule", db.Config.DefaultGameRule);
            Facade.Instance<WeChatApi>().InitWechat(AppInfo.WxAppId);
            UserController.Instance.GetShareInfo(dic, (info) =>
            {
                info.ShareData["title"] = db.OneselfData.NickM + "-" + info.ShareData["title"];
                info.ShareData["content"] = "[麻将]房间号:[" + db.Room.RoomID + "]," + db.MaxPlayerCount + "缺" + (db.MaxPlayerCount - db.Players.CurrPlayerCount) + ";";
                info.ShareData["content"] += db.Config.DefaultGameRule + "。速来玩吧! (仅供娱乐，禁止赌博)";
                Facade.Instance<WeChatApi>().ShareContent(info);
            });
        }

        /// <summary>
        /// 微信分享游戏结果截图
        /// </summary>       
        public static void WeChatShareGameResult(string url)
        {
            Facade.Instance<WeChatApi>().InitWechat(AppInfo.WxAppId);
            UserController.Instance.GetShareInfo((info) =>
            {
                info.ImageUrl = url;
                info.ShareType = ShareType.Image;
                Facade.Instance<WeChatApi>().ShareContent(info, str =>
                {
                    var parm = new Dictionary<string, object>()
                    {
                        {"option", 2},
                        {"bundle_id", Application.bundleIdentifier},
                        {"share_plat", SharePlat.WxSenceTimeLine.ToString()},
                    };
                    SendAction("shareAwards", parm, null);
                });
            });
        }

        public static void SendAction(string mainCode, Dictionary<string, object> parm, TwCallBack callBack, bool hasMsgBox = true, TwCallBack errMsgEvent = null, bool hasWait = true, string cacheKey = null)
        {
            Facade.Instance<TwManager>().SendAction(mainCode, parm, callBack, hasMsgBox, errMsgEvent, hasWait, cacheKey);
        }

        #region 资源加载
        /// <summary>
        /// 获取资源
        /// </summary>
        public static T GetAssets<T>(string assetsName) where T : Object
        {
            var bundle = "Assets" + "/" + assetsName;
            return ResourceManager.LoadAsset<T>(YxFramwork.Common.App.GameKey, bundle, assetsName);
        }

        /// <summary>
        /// 获取资源实例化对象
        /// </summary>
        public static T GetInstanceAssets<T>(string assetsName) where T : Object
        {
            var assets = GetAssets<T>(assetsName);
            if (null != assets) return GameObject.Instantiate<T>(assets);
            return null;
        }

        /// <summary>
        /// 创建UIPanel资源
        /// 先取游戏bundle中取资源，如果没有再到公共资源
        /// </summary>
        public static T CreateUIPanelAsset<T>(string assetsName) where T : MonoBehaviour
        {
            return InstanceAssetsByPath<T>(assetsName, "UIPanel");
        }

        /// <summary>
        /// 创建MahjongEffect资源
        /// </summary>
        public static T CreateMahjongEffectAsset<T>(string assetsName) where T : MonoBehaviour
        {
            return InstanceAssetsByPath<T>(assetsName, "Effects");
        }

        /// <summary>
        /// 根据资源路径生成资源
        /// </summary>      
        /// <param name="assetsName">boundle name</param>
        /// <param name="path">文件夹名字</param>
        /// <returns></returns>
        private static T InstanceAssetsByPath<T>(string assetsName, string path) where T : MonoBehaviour
        {
            var asset = ResourceManager.LoadAsset(assetsName, path + "/" + assetsName);
            if (asset != null)
            {
                var obj = Object.Instantiate(asset);
                if (null != obj)
                {
                    obj.SetActive(true);
                    obj.name = assetsName;
                    return obj.GetComponent<T>();
                }
            }
            return null;
        }
        #endregion
    }
}
