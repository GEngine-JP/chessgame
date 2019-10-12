using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Linq;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class ShowXjfdListItem : MonoBehaviour
    {
        public ShowXjfdCard Item;
        public GridLayoutGroup Grid;
        public ShowXjfdCard[] Cards;

        private List<int> _cardArry = new List<int>();
        private List<ShowXjfdCard> _tempCards = new List<ShowXjfdCard>();

        private void OnDisable()
        {
            for (int i = 0; i < Cards.Length; i++)
            {
                Cards[i].gameObject.SetActive(false);
            }
            _cardArry.Clear();
            for (int i = 0; i < _tempCards.Count; i++)
            {
                _tempCards[i].gameObject.SetActive(false);
            }
        }

        public void SetData(int[] arry)
        {
            ShowXjfdCard item = null;
            _cardArry = arry.ToList();
            for (int i = 0; i < arry.Length; i++)
            {
                if (i < Cards.Length)
                {
                    item = Cards[i];
                }
                else if (i < Cards.Length + _tempCards.Count)
                {
                    item = _tempCards[i - Cards.Length];
                }
                else
                {
                    item = CreateItem();
                }
                item.gameObject.SetActive(true);
                item.SetCardData(arry[i]);
            }
        }

        public void OnSendClick()
        {
            if (_cardArry.Count < 0) return;
            var c2s = GameCenter.Network.C2S.Custom<C2SCustom>();
            Action<int[]> sendCall = (index) =>
            {
                c2s.Network.OnRequestC2S((sfs) =>
                {
                    sfs.PutInt(RequestKey.KeyType, NetworkProtocol.MJRequestTypeDan);
                    sfs.PutIntArray(RequestKey.KeyCards, index);
                    return sfs;
                });
            };
            sendCall(_cardArry.ToArray());
            var mahHand = GameCenter.Scene.MahjongGroups.PlayerHand;
            var ccMahHand = mahHand.GetMahHandComponent<MahPlayerHand_Ccmj>();
            ccMahHand.ResetPlayerHandMahjong();
        }

        private ShowXjfdCard CreateItem()
        {
            var obj = Instantiate(Item);
            obj.transform.ExSetParent(Grid.transform);
            obj.gameObject.SetActive(false);
            _tempCards.Add(obj);
            return obj;
        }
    }
}