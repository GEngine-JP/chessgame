using System.Collections.Generic;
using YxFramwork.ConstDefine;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public partial class C2SCustom
    {
        public bool ThrowoutCardOnDaigu(int card)
        {
            return Network.OnRequestC2S((sfs) =>
            {
                sfs.PutInt(RequestKey.KeyType, NetworkProtocol.MJRequestTypeDaiGu);
                sfs.PutInt(RequestKey.KeyOpCard, card);
                return sfs;
            });
        }

        public bool ThrowoutCardOnYoujin(int card)
        {
            return Network.OnRequestC2S((sfs) =>
            {
                sfs.PutInt(RequestKey.KeyType, NetworkProtocol.ResponseYoujin);
                sfs.PutInt(RequestKey.KeyOpCard, card);
                sfs.PutInt(RequestKey.KeyOpCard, card);
                return sfs;
            });
        }

        public void OnTing(HandcardStateTyps type)
        {
            var tingList = GameCenter.DataCenter.OneselfData.TingList;
            var groups = GameCenter.Scene.MahjongGroups;
            if (tingList.Count != 0)
            {
                ChooseCgArgs args = new ChooseCgArgs()
                {
                    Type = ChooseCgArgs.ChooseType.ChooseTing,
                    CancelTingAction = () => { groups.PlayerHand.SetHandCardState(HandcardStateTyps.Normal); }
                };
                //通知UI提示选择   
                GameCenter.EventHandle.Dispatch((int)UIEventProtocol.ShowChooseOperate, args);
                groups.PlayerHand.SetHandCardState(type, tingList);
            }
        }

        public void ThrowoutCardOnTing(int card)
        {
            Network.OnRequestC2S((sfs) =>
            {
                sfs.PutInt(RequestKey.KeyType, NetworkProtocol.MJRequestTypeTing);
                sfs.PutInt(RequestKey.KeyOpCard, card);
                return sfs;
            });
        }

        public void ChooseNiuTing(List<int[]> findList, int value)
        {
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
            ChooseCgArgs args = new ChooseCgArgs()
            {
                FindList = findList,
                ConfirmAction = sendCall,
                OutPutCard = value,
                Type = ChooseCgArgs.ChooseType.ChooseTing
            };
            //通知UI提示选择   
            GameCenter.EventHandle.Dispatch((int)UIEventProtocol.ShowChooseOperate, args);
        }

        public void NiuTing(int[] niuCards, int value)
        {
            Network.OnRequestC2S((sfs) =>
            {
                sfs.PutInt(RequestKey.KeyType, NetworkProtocol.MJRequestTypeTing);
                sfs.PutInt(RequestKey.KeyOpCard, value);
                sfs.PutIntArray("niu", niuCards);
                return sfs;
            });
        }

        public void Liangdao(int[] liangCards, int value)
        {
            Network.OnRequestC2S((sfs) =>
            {
                sfs.PutInt(RequestKey.KeyType, NetworkProtocol.MJRequestTypeLiangDao);
                sfs.PutIntArray(RequestKey.KeyCardsArr, liangCards);
                return sfs;
            });

            Network.OnRequestC2S((sfs) =>
            {
                sfs.PutInt(RequestKey.KeyType, NetworkProtocol.MJThrowoutCard);
                sfs.PutInt(RequestKey.KeyOpCard, value);
                return sfs;
            });
        }
    }
}