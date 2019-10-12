﻿using System.Collections.Generic;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common.Model;
using YxFramwork.Common.Utils;

namespace Assets.Scripts.Game.Ttzkf
{
    public class TtzGameData : YxGameData
    {
        /// <summary>
        /// 房间号
        /// </summary>
        public int RoomType;
        /// <summary>
        /// 当前局数
        /// </summary>
        public int CurrentRound;
        /// <summary>
        /// 最大的局数
        /// </summary>
        public int MaxRound;
        /// <summary>
        /// 是否是开放模式
        /// </summary>
        public bool IsKaiFang;

        [HideInInspector]
        public int HupTime = 300;
        [HideInInspector]
        public int Kind = -1;
        [HideInInspector]
        public int NewCard;
        [HideInInspector]
        public string Rule;
        [HideInInspector]
        public List<int> SelfCards;
        [HideInInspector]
        public int BankLimit;
        [HideInInspector]
        public bool IsInHandsUp;//是否正在投票解散


        protected override YxBaseGameUserInfo OnInitUser(ISFSObject userData)
        {
            var userInfo = new TtzUserInfo();
            userInfo.Parse(userData);
            return userInfo;
        }


        protected override void InitGameData(ISFSObject gameInfo)
        {
            base.InitGameData(gameInfo);
            var cargs2 = gameInfo.ContainsKey(InteractParameter.Cargs2) ? gameInfo.GetSFSObject(InteractParameter.Cargs2) : null;
            if (cargs2 != null)
            {
                var tout = cargs2.ContainsKey(InteractParameter.HupTime) ? cargs2.GetUtfString(InteractParameter.HupTime) : "300";
                HupTime = int.Parse(tout);
                var bankLimit = cargs2.ContainsKey(InteractParameter.Banklimit)
                    ? cargs2.GetUtfString(InteractParameter.Banklimit)
                    : "0";
                BankLimit = int.Parse(bankLimit) * 10000;
            }
            RoomType = gameInfo.GetInt(InteractParameter.Rid);
            if (RoomType != 0)
            {
                IsKaiFang = true;
            }
            CurrentRound = gameInfo.GetInt(InteractParameter.Round);
            MaxRound = gameInfo.GetInt(InteractParameter.MaxRound);
        }
    }
}
