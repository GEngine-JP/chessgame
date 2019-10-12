using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public partial class GameLogic_Cpg : AbsGameLogicBase
    {
        private int _oldThrowoutCard;

        [S2CResponseHandler(NetworkProtocol.MJRequestTypeDan)]
        public void OnResponseCpg_Xjfd(ISFSObject data)
        {
            XjfdLogic(data);
        }

        private void XjfdLogic(ISFSObject data)
        {
            if (data.ContainsKey(ProtocolKey.KeyTType))
            {
                var ttype = data.GetInt(ProtocolKey.KeyTType);
                switch (ttype)
                {
                    case 1:
                        ttype = (int)EnGroupType.XFDan;
                        break;
                    case 2:
                        ttype = (int)EnGroupType.ZFBDan;
                        break;
                    case 3:
                        ttype = (int)EnGroupType.YaoDan;
                        break;
                    case 4:
                        ttype = (int)EnGroupType.JiuDan;
                        break;
                }
                data.PutInt(ProtocolKey.KeyTType, ttype);
            }
            mData.SetData(data);
            var xjfdData = (CpgXfdGang)mData.CpgData;
            MahjongGroupsManager group = Game.MahjongGroups;
            if (xjfdData.GetHardCards().Count >= 3)
                xjfdData.Ok = true;
            _oldThrowoutCard = 0;
            if (!xjfdData.Ok)
            {
                if (xjfdData.GetHardCards().Count == 1)
                {
                    var targetList = xjfdData.GetHardCards();
                    group.MahjongHandWall[mData.CurrOpChair].ThrowOut(targetList[0]);
                    group.MahjongThrow[mData.CurrOpChair].GetInMahjong(targetList[0]);
                    _oldThrowoutCard = DataCenter.ThrowoutCard;
                    DataCenter.ThrowoutCard = targetList[0];
                }
                return;
            }
            if (xjfdData.GetHardCards().Count == 1)
            {
                var cpgList = group.MahjongCpgs[mData.CurrOpChair].CpgList;
                for (int i = 0; i < cpgList.Count; i++)
                {

                    if (xjfdData.Type == cpgList[i].Data.Type)
                    {
                        var xjfdGang = (MahjongCpgXjfdGang)cpgList[i];
                        var item = group.MahjongThrow[mData.CurrOpChair].GetLastMj();
                        xjfdGang.AddXjfd(item);
                        group.MahjongThrow[mData.CurrOpChair].PopMahjong();
                        group.MahjongCpgs[mData.CurrOpChair].SortGpg();
                        if (_oldThrowoutCard != 0) DataCenter.ThrowoutCard = _oldThrowoutCard;
                        PlayEffect(data);
                        return;
                    }
                }
            }
            else
            {
                var tempList = xjfdData.GetHardCards();
                DataCenter.Players[mData.CurrOpChair].CpgDatas.Add(xjfdData);
                for (int i = 0; i < tempList.Count; i++)
                {
                    DataCenter.Players[mData.CurrOpChair].HardCards.Remove(tempList[i]);
                }
                group.MahjongHandWall[mData.CurrOpChair].RemoveMahjong(xjfdData.GetHardCards());
            }
            //����Ǳ��˴������
            if (xjfdData.GetOutPutCard() != MiscUtility.DefValue)
            {
                group.MahjongThrow[mData.OldOpChair].PopMahjong(xjfdData.Card);
                //���ؼ�ͷ
                Game.TableManager.HideOutcardFlag();
            }
            //�����齫               
            if (mData.CurrOpChair == 0)
            {
                group.PlayerHand.SortHandMahjong();
            }
            group.MahjongCpgs[mData.CurrOpChair].SetCpg(xjfdData);
            //���������֮�� cpg �� �������� ���� �������� ��Ҫ�����������һ��
            if (group.MahjongCpgs[mData.CurrOpChair].GetHardMjCnt() + group.MahjongHandWall[mData.CurrOpChair].MahjongList.Count > DataCenter.Config.HandCardCount)
            {
                group.MahjongHandWall[mData.CurrOpChair].SetLastCardPos(MiscUtility.DefValue);
            }
            //�齫��¼
            RecordMahjong(xjfdData);
            PlayEffect(data);
        }
    }
}