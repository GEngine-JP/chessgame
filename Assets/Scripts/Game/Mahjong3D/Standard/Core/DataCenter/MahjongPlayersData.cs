using System.Collections.Generic;
using YxFramwork.Common.Model;
using YxFramwork.ConstDefine;
using Sfs2X.Entities.Data;
using UnityEngine;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    /// <summary>
    /// YxPlayerData初始化之后，初始化该类
    /// </summary>
    [GameRuntimeData(RuntimeDataType.Players)]
    public class MahjongPlayersData : IRuntimeData
    {
        /// <summary>
        /// 保存本局玩家的数据
        /// </summary>
        private List<MahjongUserInfo> mBackupUserInfoList = new List<MahjongUserInfo>();

        //房主id       
        public int OwnerId;
        public int mOwnerSeat = -1;
        public int OwnerSeat
        {
            get
            {
                if (mOwnerSeat == -1)
                {
                    for (int i = 0; i < CurrPlayerCount; i++)
                    {
                        var player = this[i];
                        if (player != null && OwnerId == player.Id)
                        {
                            mOwnerSeat = player.Seat;
                        }
                    }
                }
                return mOwnerSeat;
            }
        }

        /// <summary>
        /// 每一局开始备份 userinfo 信息
        /// </summary>
        public void BackupUserInfo()
        {
            if (GameCenter.Instance.GameType == GameType.Playback) return;

            mBackupUserInfoList.Clear();
            var dic = MahjongUtility.GetYxGameData().UserInfoDict;
            foreach (var item in dic)
            {
                mBackupUserInfoList.Add(item.Value as MahjongUserInfo);
            }
        }

        private MahjongUserInfo GetUserInfoFormList(List<MahjongUserInfo> list, int chair)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var currChair = MahjongUtility.GetChair(list[i].Seat);
                if (currChair == chair)
                {
                    return list[i];
                }
            }
            return null;
        }

        public MahjongUserInfo GetUserInfoFormBackup(int chair)
        {
            return GetUserInfoFormList(mBackupUserInfoList, chair);
        }

        public bool IsOwer(int seat)
        {
            for (int i = 0; i < CurrPlayerCount; i++)
            {
                var player = this[i];
                if (player != null && player.Seat == seat)
                {
                    return OwnerId == player.Id;
                }
            }
            return false;
        }

        public int CurrPlayerCount
        {
            get { return MahjongUtility.GetYxGameData().UserInfoDict.Count; }
        }

        /// <summary>
        /// 获取玩家头像
        /// </summary>      
        public Texture GetPlayerHead(int chair)
        {
            return MahjongUtility.GetYxGameData().PlayerList[chair].HeadPortrait.GetTexture();
        }

        public bool SearchPlayer(Func<YxBaseGameUserInfo, bool> compare)
        {
            for (int i = 0; i < CurrPlayerCount; i++)
            {
                var player = this[i];
                if (!player.ExIsNullOjbect())
                {
                    if (compare(player))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 根据本地座位号取数据
        /// </summary>
        /// <param name="chair">本地座位号</param>
        /// <returns></returns>
        public MahjongUserInfo this[int chair]
        {
            get
            {
                var dic = MahjongUtility.GetYxGameData().UserInfoDict;
                if (dic.ContainsKey(chair))
                {
                    return dic[chair] as MahjongUserInfo;
                }
                else
                {
                    if (!GameCenter.GameProcess.IsCurrState<StateGameReady>())
                    {
                        var info = GetUserInfoFormBackup(chair);
                        if (info == null)
                        {
                            var oldData = MahjongUtility.GetYxGameData().GetLastGamePlayerInfo(chair);
                            if (null != oldData)
                            {
                                return oldData as MahjongUserInfo;
                            }
                        }
                        return info;
                    }
                }
                return null;
            }
        }

        public void ResetData()
        {
            for (int i = 0; i < CurrPlayerCount; i++)
            {
                var player = this[i];
                if (!player.ExIsNullOjbect())
                {
                    player.Reset();
                }
            }
            mOwnerSeat = -1;
        }

        public void ResetReadyState()
        {
            for (int i = 0; i < CurrPlayerCount; i++)
            {
                var player = this[i];
                if (!player.ExIsNullOjbect())
                {
                    player.ResetReadyState();
                }
            }
        }

        //是否发牌了
        public bool IsSende()
        {
            return this[0].HardCards.Count > 0;         
        }

        public void SetData(ISFSObject data)
        {
            //获取房主座位号
            OwnerId = data.TryGetInt("ownerId");
        }

        public void AddHandCardData(int chair, int[] cards)
        {
            this[chair].HardCards.AddRange(cards);
            if (chair == 0)
            {
                GameCenter.Shortcuts.MahjongQuery.Do(p => p.AddRecordMahjongs(cards));
            }
        }

        public void AddRecordMahjongs()
        {
            GameCenter.Shortcuts.MahjongQuery.Clear();
            GameCenter.Shortcuts.MahjongQuery.Do(p => p.AddRecordMahjongs(this[0].HardCards));
        }

        public void RemoveHandCardData(int chair, int[] cards)
        {
            for (int i = 0; i < cards.Length; i++)
            {
                RemoveHandCardData(chair, cards[i]);
            }
        }

        public void RemoveHandCardData(int chair, int card)
        {
            this[chair].HardCards.Remove(card);
        }   

        /// <summary>
        /// 所有杠的数量
        /// </summary>
        public int GetGangAount
        {
            get
            {
                int aount = 0;
                for (int i = 0; i < CurrPlayerCount; i++)
                {
                    var cpgs = this[i].CpgDatas;
                    for (int k = 0; k < cpgs.Count; k++)
                    {
                        if (cpgs[k].MahjongCount == 4)
                        {
                            aount++;
                        }
                    }
                }
                return aount;
            }
        }
    }

    public class MahjongUserInfo : YxBaseGameUserInfo
    {
        public bool IsHu;//胡牌
        public bool IsAuto;//听
        public bool IsReady;
        public bool VipUser;
        public bool IsOutPutCard;
        public int UserHardCardNum;
        public int FenzhangCard;
        public int ScoreDouble;
        public bool IsTuiDan;
        public AbsDataExtension ExtData;//扩展数据

        public int[] BuzhangCards;
        public List<int> HucardList = new List<int>();
        public List<int> TingList = new List<int>();
        public List<int> OutCards = new List<int>();
        public List<int> HardCards = new List<int>();
        public List<CpgData> CpgDatas = new List<CpgData>();
        public List<CpgModel> CpgModels = new List<CpgModel>();

        /// <summary>
        /// 是房主
        /// </summary>
        public bool IsOwner
        {
            get { return GameCenter.DataCenter.Players.OwnerId == Id; }
        }

        public int Chair
        {
            get { return MahjongUtility.GetChair(Seat); }
        }

        public override void Parse(ISFSObject userData)
        {
            base.Parse(userData);
            IsReady = userData.GetBool(RequestKey.KeyState);
            IsAuto = userData.TryGetBool(ProtocolKey.KeyTing);
            int num = userData.TryGetInt(ProtocolKey.KeyHardNum);
            //分数
            if (userData.ContainsKey(RequestKey.KeyGold))
            {
                CoinA += userData.GetLong(RequestKey.KeyGold);
            }
            //手牌
            UserHardCardNum = num > 0 ? num : 0;
            if (userData.ContainsKey(ProtocolKey.KeyHandCards))
            {
                int[] values = userData.GetIntArray(ProtocolKey.KeyHandCards);
                HardCards.Clear();
                HardCards.AddRange(values);
            }
            else
            {
                HardCards.Clear();
                HardCards.AddRange(new int[UserHardCardNum]);
            }
            //Cpg
            if (userData.ContainsKey(ProtocolKey.KeyMjGroup))
            {
                ISFSArray Groups = userData.GetSFSArray(ProtocolKey.KeyMjGroup);
                CpgDatas.Clear();
                for (int j = 0; j < Groups.Count; j++)
                {
                    var cpg = MahjongUtility.CreateCpg(Groups.GetSFSObject(j));
                    CpgDatas.Add(cpg);
                }
            }
            //出牌
            if (userData.ContainsKey(ProtocolKey.KeyOutCard))
            {
                int[] values = userData.GetIntArray(ProtocolKey.KeyOutCard);
                OutCards.Clear();
                OutCards.AddRange(values);
            }
            //补张 
            if (userData.ContainsKey("buHua"))
            {
                BuzhangCards = userData.GetIntArray("buHua");
            }
            //听牌
            if (userData.ContainsKey("tingout"))
            {
                SetTinglist(userData.GetIntArray("tingout"));
            }
            //不带听玩法时候 tinglist
            if (userData.ContainsKey("tingoutlist"))
            {
                SetTinglist(userData.TryGetIntArray("tingoutlist"));
            }
            //Op
            if (userData.ContainsKey(ProtocolKey.KeyOp) && Chair == 0)
            {
                GameCenter.DataCenter.OperateMenu = userData.GetInt(ProtocolKey.KeyOp);
            }
            IsTuiDan = userData.ContainsKey("tuidan");
            //听 
            IsAuto = userData.TryGetBool("hasTing");
            //vip权限
            VipUser = userData.TryGetBool("vipuser");
            //下注
            ScoreDouble = userData.TryGetInt("piao");
            //设置扩展类数据
            var extDatas = GameCenter.DataCenter.GetExtDataMap;
            if (extDatas.ContainsKey(RuntimeDataType.Players))
            {
                Type type = extDatas[RuntimeDataType.Players].GetType();
                ExtData = Activator.CreateInstance(type) as AbsDataExtension;
                if (null != ExtData)
                {
                    ExtData.SetData(userData, this);
                }
            }
            if (Chair == 0)
            {
                //根据server数据 指定杠牌
                var array = userData.TryGetIntArray("gangcard");
                if (array != null)
                {
                    GameCenter.DataCenter.GangCard.AddRange(array);
                }
            }
        }

        public void ClearCpgData()
        {
            CpgModels.Clear();
            CpgDatas.Clear();
        }

        public void AddCpgData(CpgData data)
        {
            if (data == null) return;
            CpgDatas.Add(data);
        }

        public T GetExtData<T>() where T : AbsDataExtension
        {
            if (ExtData != null)
            {
                return ExtData as T;
            }
            return null;
        }

        public void ResetReadyState()
        {
            IsReady = false;
        }

        public void Reset()
        {
            ScoreDouble = 0;
            FenzhangCard = 0;
            UserHardCardNum = 0;

            IsHu = false;
            IsAuto = false;
            IsOutPutCard = false;

            OutCards.Clear();
            TingList.Clear();
            CpgDatas.Clear();
            HardCards.Clear();
            CpgModels.Clear();
            if (null != ExtData)
            {
                ExtData.Reset();
            }
        }

        public void SetTinglist(int[] tings)
        {
            TingList.Clear();
            TingList.AddRange(tings);
        }
    }
}