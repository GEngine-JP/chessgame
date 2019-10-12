using System.Collections.Generic;
using YxFramwork.ConstDefine;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    /// <summary>
    /// 自定义逻辑
    /// 响应服务器消息，op菜单逻辑之后执行相关逻辑
    /// </summary>
    [S2CResponseLogic]
    public partial class GameLogic_Custom : AbsGameLogicBase
    {
        protected List<MahjongContainer> PlayerHand;
        protected List<MahjongCpg> PengList;

        [S2CResponseHandler(CustomClientProtocol.CustomTypeCustomLogic, GameMisc.DsqmjKey)]
        [S2CResponseHandler(CustomClientProtocol.CustomTypeCustomLogic, GameMisc.YkmjKey)]
        public void OCustomLogic_ykmj(ISFSObject data)
        {
            if (!ConfigData.Jue) return;

            int type = data.ContainsKey(RequestKey.KeyType) ? data.GetInt(RequestKey.KeyType) : -1;
            var seat = data.ContainsKey(RequestKey.KeySeat) ? data.GetInt(RequestKey.KeySeat) : -1;
            int value = data.ContainsKey(RequestKey.KeyOpCard) ? data.GetInt(RequestKey.KeyOpCard) : -1;
            if (seat == -1 && DataCenter.IsReconect) seat = DataCenter.SelfSeat;
            if (GameCenter.DataCenter.SelfSeat != seat) return;
            PlayerHand = Game.MahjongGroups.MahjongHandWall[0].MahjongList;
            PengList = Game.MahjongGroups.MahjongCpgs[0].CpgList;
            switch (type)
            {
                case NetworkProtocol.MJRequestTypeCPG:
                    EnGroupType cpgType = data.ContainsKey(ProtocolKey.KeyTType)
                        ? (EnGroupType)data.GetInt(ProtocolKey.KeyTType)
                        : EnGroupType.None;
                    if (cpgType == EnGroupType.Peng)
                    {
                        int card = data.ContainsKey(RequestKey.KeyCard) ? data.GetInt(RequestKey.KeyCard) : -1;
                        OnPengCard(card);
                    }
                    break;
                case NetworkProtocol.MJRequestTypeGetInCard:
                    OnGetCard(value);
                    break;
                case NetworkProtocol.MJThrowoutCard:
                    OnOutPutCard(value);
                    break;
                case -1:
                    if (DataCenter.IsReconect)
                    {
                        CheckJueCard();
                    }
                    break;
            }
        }

        private Dictionary<int, int> GetCardAmount(List<MahjongContainer> mahjongs)
        {
            Dictionary<int, int> typeDic = new Dictionary<int, int>();
            for (int i = 0; i < mahjongs.Count; i++)
            {
                var value = mahjongs[i].Value;
                if (typeDic.ContainsKey(value))
                {
                    typeDic[value] += 1;
                }
                else
                {
                    typeDic[value] = 1;
                }
            }
            return typeDic;
        }

        protected void OnOutPutCard(int value)
        {
            for (int j = 0; j < PlayerHand.Count; j++)
            {
                if (PlayerHand[j].Value == value)
                {
                    if (PlayerHand[j].GetOther())
                    {
                        //取消绝图标
                        PlayerHand[j].SetOtherSign(Anchor.TopRight, false);
                    }
                }
            }
        }

        protected void OnPengCard(int value)
        {
            for (int i = 0; i < PlayerHand.Count; i++)
            {
                if (PlayerHand[i].Value == value && !MahjongUtility.MahjongFlagCheck(value))
                {
                    SetJue(PlayerHand[i], true);
                }
            }
        }

        protected void OnGetCard(int card)
        {
            var dic = GetCardAmount(PlayerHand);
            foreach (var item in dic)
            {
                if (item.Value == 4 && item.Key == card)
                {
                    int count = PlayerHand.Count - 1;
                    for (int i = count; i >= 0; i--)
                    {
                        if (PlayerHand[i].Value == item.Key)
                        {
                            SetJue(PlayerHand[i], true);
                            break;
                        }
                    }
                }
            }

            for (int i = 0; i < PengList.Count; i++)
            {
                var type = PengList[i].Data.Type;
                if (type == EnGroupType.Peng)
                {
                    int value = PengList[i].Data.Card;
                    if (value == card)
                    {
                        for (int j = 0; j < PlayerHand.Count; j++)
                        {
                            if (PlayerHand[j].Value == value)
                            {
                                SetJue(PlayerHand[j], true);
                            }
                        }
                    }
                }
            }
        }

        protected void CheckJueCard()
        {
            var dic = GetCardAmount(PlayerHand);
            foreach (var item in dic)
            {
                if (item.Value == 4)
                {
                    int count = PlayerHand.Count - 1;
                    for (int i = count; i >= 0; i--)
                    {
                        if (PlayerHand[i].Value == item.Key)
                        {
                            SetJue(PlayerHand[i], true);
                            break;
                        }
                    }
                }
            }

            for (int i = 0; i < PengList.Count; i++)
            {
                var type = PengList[i].Data.Type;
                if (type == EnGroupType.Peng)
                {
                    int card = PengList[i].Data.Card;
                    for (int j = 0; j < PlayerHand.Count; j++)
                    {
                        if (PlayerHand[j].Value == card)
                        {
                            SetJue(PlayerHand[j], true);
                        }
                    }
                }
            }
        }

        private void SetJue(MahjongContainer mahjong, bool isOn)
        {
            mahjong.SetOtherSign(Anchor.TopRight, isOn);
        }
    }
}