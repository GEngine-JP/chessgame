using System.Collections.Generic;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    /// <summary>
    /// 游戏中需要的数据
    /// </summary>
    [GameRuntimeData(RuntimeDataType.Game)]
    public class MahjongSceneData : IRuntimeData
    {
        public int BaoCard;//宝牌     
        public int FanCard;//翻牌     
        public int LaiziCard;//癞子       
        public int CurrOpSeat;//当前操作用户服务器座位号       
        public int OldOpSeat;//上一次操作用户服务器座位号      
        public int OwnerSeat;//房主       
        public int BankerSeat;//庄家       
        public int GetInMahjong;//抓到的麻将     
        public int ThrowoutCard;//打出的麻将      
        public int ReconnectTime;//重连时当前用户的时间 
        public bool IsOutPutCard;//是否玩家已经打过牌       
        public int FristBankerSeat;//第一次庄家位子，随机庄，计算圈用      
        public int ReconectState;//重连时手中牌状态,在手中0,已经打出1      
        public int LeaveMahjongCnt;//剩余麻将个数    
        public int[] BaoSaizisList;//翻宝时打的骰子
        public int[] BaoIndexList;//翻宝时移除的麻将牌index
        public int[] SaiziPoint = new int[2];//塞子的点数     
        public bool FirstGetCard = true;//第一次抓牌
        public bool FenzhangFlag;//分张
        public bool HuangZhuang;
        public List<TotalResult> TotalResult = new List<TotalResult>();

        // 玩嘛苏州麻将
        public int Diling;//滴零     
        public int Baozi;//豹子    

        //wmbbmj
        public int Laozhuang;
        public int LaozhuangId;

        /// <summary>
        /// 娱乐房时，上一局庄家退出， 新进入玩家牢庄应该是1开始
        /// </summary>
        /// <returns></returns>
        public void YuleSetLaozhuang()
        {
            var db = GameCenter.DataCenter;
            if (db.Room.RoomType == MahRoomType.YuLe)
            {
                var chair = db.BankerChair;
                var bank = db.Players[chair];
                if (LaozhuangId != bank.Id)
                {
                    Laozhuang = 1;
                }
                LaozhuangId = bank.Id;
            }
        }

        public void ResetData()
        {
            BaoCard = MiscUtility.DefValue;
            FanCard = MiscUtility.DefValue;
            LaiziCard = MiscUtility.DefValue;
            OldOpSeat = MiscUtility.DefValue;
            CurrOpSeat = MiscUtility.DefValue;
            BankerSeat = MiscUtility.DefValue;
            ThrowoutCard = MiscUtility.DefValue;
            GetInMahjong = MiscUtility.DefValue;
            ReconnectTime = MiscUtility.DefValue;
            ReconectState = MiscUtility.DefValue;
            IsOutPutCard = false;
            FenzhangFlag = false;
            FirstGetCard = true;

            Diling = MiscUtility.DefValue;
            Baozi = MiscUtility.DefValue;
        }

        public void ClearTotalResult()
        {
            TotalResult.Clear();
        }

        public void SetData(ISFSObject data)
        {
            FanCard = data.TryGetInt(ProtocolKey.CardFan);
            BaoCard = data.TryGetInt(ProtocolKey.CardBao);
            LaiziCard = data.TryGetInt(ProtocolKey.CardLaizi);
            GetInMahjong = data.TryGetInt(ProtocolKey.KeyLastIn);
            FristBankerSeat = data.TryGetInt(ProtocolKey.Keybank);
            LeaveMahjongCnt = data.TryGetInt(ProtocolKey.KeyCardLen);
            ThrowoutCard = data.TryGetInt(ProtocolKey.KeyLastOutCard);
            SaiziPoint = data.TryGetIntArray(ProtocolKey.KeyDiceArray);
            BaoSaizisList = data.TryGetIntArray(ProtocolKey.KeySaiziList);
            BaoIndexList = data.TryGetIntArray(ProtocolKey.KeyBaoIndexList);
            HuangZhuang = data.TryGetBool(ProtocolKey.KeyHuangZhuang);
            GameCenter.DataCenter.BankerSeat = data.TryGetInt(ProtocolKey.KeyStartP);
            GameCenter.DataCenter.CurrOpSeat = data.TryGetInt(ProtocolKey.KeyCurrentP);

            Diling = data.TryGetInt("diling");
            Baozi = data.TryGetInt("baozi");
            Laozhuang = data.TryGetInt("lzcnt");
        }

        public void SetTotalResult(ISFSObject data)
        {
            ISFSArray userDatas = data.TryGetSFSArray("users");
            if (userDatas == null) return;
            TotalResult result;
            for (int i = 0; i < userDatas.Size(); i++)
            {
                result = new TotalResult();
                ISFSObject obj = userDatas.GetSFSObject(i);
                result.Id = obj.TryGetInt("id");
                result.Hu = obj.TryGetInt("hu");
                result.Pao = obj.TryGetInt("pao");
                result.Zimo = obj.TryGetInt("zimo");
                result.Gang = obj.TryGetInt("gang");
                result.Seat = obj.TryGetInt("seat");
                result.Glod = obj.TryGetInt("gold");
                result.ZhaNiao = obj.TryGetInt("niao");
                result.AnGang = obj.TryGetInt("anGang");
                result.Gangkais = obj.TryGetInt("gangkai");
                result.Name = obj.TryGetString("nick");
                result.MoBao = obj.TryGetInt("mobao");
                result.ChBao = obj.TryGetInt("chbao");
                TotalResult.Add(result);
            }
        }
    }

    public class MahjongResult
    {
        public int Id;
        public int Seat;
        public int Chair;
        public int HuSeat = MiscUtility.DefInt;
        public int Gold;
        public int CType;//胡牌类型  
        public int HuCard;
        public int HuType;
        public int HuGold;
        public int PuGlod;
        public int GangGlod;
        public int PiaoGlod;
        public int NiaoGold;
        public int UserHuType;
        public int FujingArray;
        public int ZhengjingArray;
        public long TotalGold;
        public string Name;
        public string HuInfo;
        public bool HuFlag;
        public List<ScoreDetail> Deatils;

        public MahjongResult(int seat)
        {
            Seat = seat;
            Chair = MahjongUtility.GetChair(seat);
            MahjongUserInfo data = GameCenter.DataCenter.Players[Chair];
            if (null != data)
            {
                Id = data.Id;
                Name = data.NickM;
            }
        }

        public void SetDeatil(ISFSObject sfsObject)
        {
            if (sfsObject.ContainsKey("huDetails"))
            {
                Deatils = new List<ScoreDetail>();
                var array = sfsObject.GetSFSArray("huDetails");

                for (int i = 0; i < array.Count; i++)
                {
                    var data = array.GetSFSObject(i);
                    Deatils.Add(new ScoreDetail(data));
                }
            }
        }

        public class ScoreDetail
        {
            public int[] MatterSeats;
            public int AmountScore;
            public string Description;

            public ScoreDetail(ISFSObject obj)
            {
                AmountScore = obj.TryGetInt("gold");
                Description = obj.TryGetString("hname");
                MatterSeats = obj.TryGetIntArray("otherPlayer");
            }
        }
    }

    public class TotalResult
    {
        public int Id;
        public int Hu;
        public int Pao;
        public int Seat;
        public int Glod;
        public int Gang;
        public int Zimo;
        public int AnGang;
        public int ZhaNiao;
        public int Gangkais;
        public string Name;
        public int MoBao;
        public int ChBao;
    }
}