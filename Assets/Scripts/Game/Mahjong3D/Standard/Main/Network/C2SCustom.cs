using System.Collections.Generic;
using YxFramwork.ConstDefine;
using System.Linq;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public partial class C2SCustom : IC2SCustomRequest
    {
        private NetworkComponent mNetwork;

        public NetworkComponent Network
        {
            get
            {
                if (null == mNetwork) { mNetwork = GameCenter.Network; }
                return mNetwork;
            }
        }

        public void OnHu()
        {
            Network.OnRequestC2S((sfs) =>
            {
                if (GameCenter.DataCenter.SelfCurrOp)
                {
                    sfs.PutInt(RequestKey.KeyType, NetworkProtocol.MJReqTypeZiMo);
                }
                else
                {
                    sfs.PutInt(RequestKey.KeyType, NetworkProtocol.MJRequestTypeCPG);
                    sfs.PutInt(ProtocolKey.KeyTType, NetworkProtocol.MJRequestTypeHu);
                }
                return sfs;
            });
        }

        public void OnGuo()
        {
            //清理听牌提示
            if (GameMisc.DbsmjKey == MahjongUtility.GameKey)
            {
                GameCenter.Scene.MahjongGroups.PlayerHand.OnQueryMahjong(null);
            }
            var db = GameCenter.DataCenter;
            //听牌时点过按钮
            if (db.CurrOpChair == 0 && db.OneselfData.IsAuto && GameUtils.BinaryCheck(OperateKey.OpreateHu, db.OperateMenu))
            {
                GameCenter.Network.C2S.ThrowoutCard(db.GetInMahjong);
            }
            else
            {
                if (db.CurrOpChair == 0 && GameCenter.Shortcuts.SwitchCombination.IsOpen((int)GameSwitchType.AiAgency))
                {
                    GameCenter.Network.C2S.ThrowoutCard(db.GetInMahjong);
                }
                else
                {
                    Network.OnRequestC2S((sfs) =>
                    {
                        sfs.PutInt(RequestKey.KeyType, NetworkProtocol.MJRequestTypeCPG);
                        sfs.PutInt(ProtocolKey.KeyTType, OperateKey.OpreateNone);
                        return sfs;
                    });
                }
            }
        }

        //--------------------碰牌------------------------------------
        public void OnPeng()
        {
            Network.OnRequestC2S((sfs) =>
            {
                int[] find = FindCanPeng();
                sfs.PutInt(RequestKey.KeyType, NetworkProtocol.MJRequestTypeCPG);
                sfs.PutInt(ProtocolKey.KeyTType, OperateKey.OpreatePeng);
                sfs.PutIntArray(RequestKey.KeyCards, find);
                return sfs;
            });
        }

        private int[] FindCanPeng()
        {
            var dataCenter = GameCenter.DataCenter;
            int[] arr = null;
            int outcard = dataCenter.ThrowoutCard;
            List<int> cards = dataCenter.OneselfData.HardCards;
            MahjongUtility.SortMahjong(cards);
            Dictionary<int, int> dic = GetCardAmount(cards);
            if (dic.ContainsKey(outcard) && dic[outcard] >= 2)
            {
                arr = new[] { outcard, outcard };
            }
            return arr;
        }

        //--------------------吃牌------------------------------------
        public void OnChi()
        {
            var dataCenter = GameCenter.DataCenter;
            List<int[]> findList = FindCanChi();
            Action<int> sendCall = (index) =>
            {
                Network.OnRequestC2S((sfs) =>
                {
                    sfs.PutInt(RequestKey.KeyType, NetworkProtocol.MJRequestTypeCPG);
                    sfs.PutInt(ProtocolKey.KeyTType, OperateKey.OpreateChi);
                    sfs.PutIntArray(RequestKey.KeyCards, findList[index]);
                    return sfs;
                });
            };
            if (findList.Count > 1)
            {
                ChooseCgArgs args = new ChooseCgArgs()
                {
                    FindList = findList,
                    ConfirmAction = sendCall,
                    OutPutCard = dataCenter.ThrowoutCard,
                    Type = ChooseCgArgs.ChooseType.ChooseCg,
                };
                //通知UI提示选择   
                GameCenter.EventHandle.Dispatch((int)UIEventProtocol.ShowChooseOperate, args);
            }
            else
            {
                sendCall(0);
            }
        }

        private List<int[]> FindCanChi()
        {
            var dataCenter = GameCenter.DataCenter;
            var findList = new List<int[]>();
            int outcard = dataCenter.ThrowoutCard;
            int laizi = dataCenter.Game.LaiziCard;
            List<int> cards = dataCenter.OneselfData.HardCards;
            MahjongUtility.SortMahjong(cards);
            Dictionary<int, int> dic = GetCardAmount(cards);
            if (dic.ContainsKey(outcard - 1) && outcard - 1 != laizi)
            {
                if (dic.ContainsKey(outcard - 2) && outcard - 2 != laizi)
                {
                    findList.Add(new[] { outcard - 2, outcard - 1 });
                }
                if (dic.ContainsKey(outcard + 1) && outcard + 1 != laizi)
                {
                    findList.Add(new[] { outcard - 1, outcard + 1 });
                }
            }
            if (dic.ContainsKey(outcard + 1) && dic.ContainsKey(outcard + 2) && outcard + 1 != laizi && outcard + 2 != laizi)
            {
                findList.Add(new[] { outcard + 1, outcard + 2 });
            }
            return findList;
        }

        //--------------------杠牌------------------------------------
        public void OnGang()
        {
            var dataCenter = GameCenter.DataCenter;
            var findList = FindCanGang();
            Action<int> sendCall = (index) =>
            {
                Network.OnRequestC2S((sfs) =>
                {
                    if (findList[index].type != MiscUtility.DefInt)
                        sfs.PutInt(RequestKey.KeyType, findList[index].type);
                    if (findList[index].ttype != MiscUtility.DefInt)
                        sfs.PutInt(ProtocolKey.KeyTType, findList[index].ttype);
                    if (findList[index].cards != null && findList[index].ttype != NetworkProtocol.CPG_ZhuaGang)
                        sfs.PutIntArray(RequestKey.KeyCards, findList[index].cards);
                    else if (findList[index].cards != null && findList[index].ttype == NetworkProtocol.CPG_ZhuaGang)
                        sfs.PutInt(RequestKey.KeyOpCard, findList[index].cards[0]);
                    return sfs;
                });
            };
            //如果找到的杠 大于1
            if (findList.Count > 1)
            {
                var gangcard = dataCenter.GangCard;
                if (gangcard.Count > 0)
                {
                    //根据server 指定杠牌
                    var tempFindList = new List<FindGangData>();
                    for (int i = 0; i < gangcard.Count; i++)
                    {
                        var gang = findList.Find(d => d.cards[0] == gangcard[i]);
                        tempFindList.Add(gang);
                    }
                    findList = tempFindList;
                }
                gangcard.Clear();

                //如果手中又四张赖子牌， 过滤赖子牌
                int laizi = GameCenter.DataCenter.Game.LaiziCard;
                if (findList.Exists(d => d.cards[0] == laizi))
                {
                    findList.RemoveAll(d => d.cards[0] == laizi);
                } 

                if (findList.Count == 1)
                {
                    sendCall(0);
                }
                else
                {
                    ChooseCgArgs args = new ChooseCgArgs()
                    {
                        ConfirmAction = sendCall,
                        OutPutCard = dataCenter.ThrowoutCard,
                        Type = ChooseCgArgs.ChooseType.ChooseCg,
                    };
                    args.FindList = GetGangList(findList);
                    //通知UI提示选择   
                    GameCenter.EventHandle.Dispatch((int)UIEventProtocol.ShowChooseOperate, args);
                }
            }
            else
            {
                sendCall(0);
            }
        }

        protected virtual List<int[]> GetGangList(List<FindGangData> list)
        {
            var arr = new List<int[]>();
            for (int i = 0; i < list.Count; i++)
            {
                arr.Add(list[i].cards);
            }
            return arr;
        }

        protected virtual List<FindGangData> FindCanGang()
        {
            int checkValue;
            var dataCenter = GameCenter.DataCenter;
            var findList = new List<FindGangData>();
            int laizi = dataCenter.Game.LaiziCard;
            List<int> cards = dataCenter.OneselfData.HardCards;
            MahjongUtility.SortMahjong(cards);
            Dictionary<int, int> dic = GetCardAmount(cards);
            Func<int, bool> checkPengGang = (value) =>
            {
                if (dic.ContainsKey(value) && dic[value] >= 3)
                {
                    int[] temp = new int[dic[value]];
                    for (int i = 0; i < dic[value]; i++)
                    {
                        temp[i] = value;
                    }
                    var data = new FindGangData
                    {
                        type = NetworkProtocol.MJRequestTypeCPG,
                        ttype = NetworkProtocol.CPG_PengGang,
                        cards = temp
                    };
                    findList.Add(data);
                    return true;
                }
                return false;
            };
            Func<bool> checkHandCardGang = () =>
            {
                bool ret = false;
                foreach (KeyValuePair<int, int> keyValuePair in dic)
                {
                    if (keyValuePair.Value > 3 && (laizi != keyValuePair.Value))
                    {
                        ret = true;
                        var data = new FindGangData
                        {
                            type = NetworkProtocol.MJRequestTypeSelfGang,
                            ttype = NetworkProtocol.CPG_AnGang,
                            cards = new[] { keyValuePair.Key, keyValuePair.Key, keyValuePair.Key, keyValuePair.Key }
                        };
                        findList.Add(data);
                    }
                }
                return ret;
            };
            Func<int, bool> checkZhuaGang = (value) =>
            {
                CpgData temp;
                var findResult = false;
                List<CpgData> cpgs = dataCenter.Players[0].CpgDatas;
                for (int i = 0; i < cpgs.Count; i++)
                {
                    temp = cpgs[i];
                    if (temp.Type == EnGroupType.Peng && (value == temp.Card || dic.ContainsKey(temp.Card)))
                    {
                        var data = new FindGangData
                        {
                            type = NetworkProtocol.MJRequestTypeSelfGang,
                            ttype = NetworkProtocol.CPG_ZhuaGang,
                            cards = new[] { temp.Card, temp.Card, temp.Card, temp.Card }
                        };
                        findList.Add(data);
                        findResult = true;
                    }
                }
                return findResult;
            };
            Func<bool> checkXFGang = () =>
            {
                //旋风杠-只有第一轮生效
                if (dataCenter.Game.IsOutPutCard)
                    return false;
                bool zfbNoLaiZi = laizi != 81 && laizi != 84 && laizi != 87;
                //中发白-优先于-中发白
                if (dic.ContainsKey(81) && dic.ContainsKey(84) && dic.ContainsKey(87) && zfbNoLaiZi)
                {
                    var data = new FindGangData
                    {
                        type = NetworkProtocol.MJRequestTypeXFG,
                        ttype = MiscUtility.DefInt,
                        cards = new[] { 81, 84, 87 }
                    };
                    findList.Add(data);
                    return true;
                }
                //东南西北
                if (dic.ContainsKey(65) && dic.ContainsKey(68) && dic.ContainsKey(71) && dic.ContainsKey(74))
                {
                    var data = new FindGangData
                    {
                        type = NetworkProtocol.MJRequestTypeXFG,
                        ttype = MiscUtility.DefInt,
                        cards = new[] { 65, 68, 71, 74 }
                    };
                    findList.Add(data);
                    return true;
                }
                return false;
            };
            //如果是 当前用户
            if (0 == dataCenter.CurrOpChair)
            {
                //旋风杠 最优先 直接返回 不需要用户选择
                if (dataCenter.ConfigData.XFGang && checkXFGang())
                {
                    return findList;
                }
                checkValue = dataCenter.GetInMahjong;
                checkHandCardGang();
                checkZhuaGang(checkValue);
            }
            else
            {
                checkValue = dataCenter.ThrowoutCard;
                checkPengGang(checkValue);
            }
            return findList;
        }
        //--------------------------------------------------------

        /// <summary>
        /// 获取牌的数量
        /// </summary>
        private Dictionary<int, int> GetCardAmount(List<int> values)
        {
            Dictionary<int, int> typeDic = new Dictionary<int, int>();
            int len = values.Count;
            int singleNum = -1;
            for (int i = 0; i < len; i++)
            {
                if (values[i] != singleNum)
                {
                    singleNum = values[i];
                    typeDic[singleNum] = 1;
                }
                else
                {
                    typeDic[singleNum] += 1;
                }
            }
            return typeDic;
        }
    }
}