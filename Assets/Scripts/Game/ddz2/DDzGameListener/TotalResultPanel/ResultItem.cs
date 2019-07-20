using System;
using System.Globalization;
using Assets.Scripts.Game.ddz2.DDz2Common;
using Assets.Scripts.Game.ddz2.InheritCommon;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Framework.Core;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.ddz2.DDzGameListener.TotalResultPanel
{
    public class ResultItem : MonoBehaviour
    {
        /*
                /// <summary>
                /// 地主图标
                /// </summary>
                public GameObject Dizhu;
                /// <summary>
                /// 得到的钱
                /// </summary>
                public UILabel Money;
                /// <summary>
                /// 底分
                /// </summary>
                public UILabel Score;
                /// <summary>
                /// 春天
                /// </summary>
                public UILabel Spring;
                /// <summary>
                /// 炸弹
                /// </summary>
                public UILabel Boom;
                /// <summary>
                /// 火箭
                /// </summary>
                public UILabel Rocket;
 
                /// <summary>
                /// 加倍显示
                /// </summary>
                public UILabel AddRate;
         */


        /// <summary>
        /// 玩家名字
        /// </summary>
        public UILabel UserName;
        /// <summary>
        /// 用户头像
        /// </summary>
        public UITexture UserIcon;
        /// <summary>
        /// 最高得分
        /// </summary>
        public UILabel BestScore;
        /// <summary>
        /// 胜局败局显示
        /// </summary>
        public UILabel WinAndLose;
        /// <summary>
        /// 总积分
        /// </summary>
        public UILabel AllScore;

/*        /// <summary>
        /// 设置头像
        /// </summary>
        /// <param name="url">图片路径</param>
        public void SetIcon(string url)
        {
            //加载真实头像
            Facade.Instance<AsyncImage>().GetAsyncImage(url, tex =>
            {
                if (tex != null)
                {
                    UserIcon.mainTexture = tex;
                }
            });
        }*/

        /// <summary>
        /// 设置用户信息
        /// </summary>
        /// <param name="overinfo">总结算给的用户信息</param>
        public void SetUserInfo(ISFSObject overinfo)
        {
           var userSeat =  overinfo.GetInt("seat");
           var userInfo = App.GetGameData<GlobalData>().GetUserInfo(userSeat);
           if (userInfo==null) throw new Exception("GlobalData里没有相关座位用户信息");

           //名字
           UserName.text = userInfo.GetUtfString(RequestKey.KeyName); 
           
           //头像
           DDzUtil.LoadRealHeadIcon(userInfo.GetUtfString("avatar"), userInfo.GetShort("sex"), UserIcon);
           
           //单局最高得分
           int maxScore = overinfo.GetInt("max");

           BestScore.text =YxUtiles.GetShowNumber(maxScore).ToString(CultureInfo.InvariantCulture);

           //胜负局数
           WinAndLose.text = overinfo.GetInt("win") + "胜" + overinfo.GetInt("lost") + "负";

            //总积分
            int allScore = overinfo.GetInt("gold");
            AllScore.text = YxUtiles.GetShowNumber(allScore).ToString(CultureInfo.InvariantCulture);

        }
    }
}
