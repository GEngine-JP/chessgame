using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.rbwar
{
    public class CardCtrl : MonoBehaviour
    {
        public Transform CardPts;

        public CardItem CardItem;

        public List<CardItem> AllCardItems;

        public UISprite BlcakCardType;

        public UISprite RedCardType;

        private CardValue _cardValue;

        private RbwarGameManager _gmanager
        {
            get { return App.GetGameManager<RbwarGameManager>(); }
        }

        public void CreatCards()
        {
            CardPts.transform.localPosition = Vector3.zero;

            _gmanager.LaterSend = true;
            for (int i = 0; i < AllCardItems.Count; i++)
            {
                AllCardItems[i].GetComponent<TweenPosition>().onFinished.Clear();
                AllCardItems[i].GetComponent<TweenPosition>().PlayForward();
                AllCardItems[i].GetComponent<TweenPosition>().AddOnFinished(() =>
                {
                    Facade.Instance<MusicManager>().Play("sendcard");
                });
                if (i == AllCardItems.Count - 1)
                {
                    _gmanager.LaterSend = false;
                }

            }
//            foreach (var cardItem in AllCardItems)
//            {
//                cardItem.GetComponent<TweenPosition>().onFinished.Clear();
//                cardItem.GetComponent<TweenPosition>().PlayForward();
//                cardItem.GetComponent<TweenPosition>().AddOnFinished(() =>
//                {
//                    Facade.Instance<MusicManager>().Play("sendcard");
//                });
//            }
        }


        public void ShowCardValue(CardValue cardValue)
        {
            _gmanager.LaterSend = true;
            _cardValue = cardValue;
          RatateCard(cardValue.BlackCards,0,3,() =>
          {
              BlcakCardType.gameObject.SetActive(true);
              BlcakCardType.transform.GetChild(0).gameObject.SetActive(_cardValue.BlackCardType != 0);
              var cardName=CardTypeValue(_cardValue.BlackCardType);
              BlcakCardType.spriteName = cardName;
              Facade.Instance<MusicManager>().Play(cardName);
          });

          RatateCard(cardValue.RedCards,3,6,() =>
          {
              RedCardType.gameObject.SetActive(true);
              RedCardType.transform.GetChild(0).gameObject.SetActive(_cardValue.RedCardType != 0);
              var cardName = CardTypeValue(_cardValue.RedCardType);
              RedCardType.spriteName = cardName;
              Facade.Instance<MusicManager>().Play(cardName);
          });
        }

        private void RatateCard(int[] cardValues,int index,int length, EventDelegate.Callback callback=null)
        {
           
            for (int i = index; i < length; i++)
            {
                if (i == length - 1)
                {
                    AllCardItems[i].RotateCardBg(cardValues[i%3], callback, true);
                    if (i == 5)
                    {
                        _gmanager.LaterSend = false;
                    }
                }
                else
                {
                    AllCardItems[i].RotateCardBg(cardValues[i%3]);
                }
            }
           
        }


        private string CardTypeValue(int type)
        {
            var cardType = "";
            switch (type)
            {
                case (int)CardType.DanPai:
                    cardType = "danzhang";
                    break;
                case (int)CardType.DuiZi:
                    cardType = "duizi";
                    break;
                case (int)CardType.ShunZi:
                    cardType = "shunzi";
                    break;
                case (int)CardType.JinHua:
                    cardType = "jinhua";
                    break;
                case (int)CardType.ShunJin:
                    cardType = "shunjin";
                    break;
                case (int)CardType.BaoZi:
                    cardType = "baozi";
                    break;
            }

            return cardType;
        }

        public void Clear()
        {
            _gmanager.LaterSend = true;
            BlcakCardType.gameObject.SetActive(false);
            RedCardType.gameObject.SetActive(false);

            CardPts.transform.localPosition=new Vector3(0,500,0);

            for (int i = 0; i < AllCardItems.Count; i++)
            {
                AllCardItems[i].GetComponent<TweenPosition>().onFinished.Clear();
                AllCardItems[i].GetComponent<TweenPosition>().PlayReverse();
                AllCardItems[i].Clear();
                if (i == AllCardItems.Count - 1)
                {
                    _gmanager.LaterSend = false;
                }
            }


//            foreach (var cardItem in AllCardItems)
//            {
//                cardItem.GetComponent<TweenPosition>().onFinished.Clear();
//                cardItem.GetComponent<TweenPosition>().PlayReverse();
//                cardItem.Clear();
//            }
        }
    }

    public enum CardType
    {
        DanPai,
        DuiZi,
        ShunZi,
        JinHua,
        ShunJin,
        BaoZi
    }
}
