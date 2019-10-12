using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.rbwar
{
    public class CardItem : MonoBehaviour
    {
        public UISprite CardBg;
        public UISprite CardMan;
        public UISprite CardValue;
        public UISprite CardColorTop;
        public UISprite CardColorCenter;

        private string _cardColor;
        private string _cardMan;
        private bool _isHasMan;

        private RbwarGameManager _gmanager
        {
            get { return App.GetGameManager<RbwarGameManager>(); }
        }

        public void RotateCardBg(int value,EventDelegate.Callback callback=null, bool isBig = false)
        {
            CardBg.GetComponent<TweenRotation>().PlayForward();

            if (isBig&& _gmanager.ResponseQueue.Count > 0)
            {
                gameObject.GetComponent<TweenScale>().PlayForward();
            }

            CardBg.gameObject.GetComponent<TweenRotation>().AddOnFinished(() =>
            {
                Facade.Instance<MusicManager>().Play("flipcard");
                if (gameObject.GetComponent<TweenScale>())
                {
                    gameObject.GetComponent<TweenScale>().PlayReverse();
                    gameObject.GetComponent<TweenScale>().AddOnFinished(callback);
                }

                CardShow(CardValue, GetCardValue(value));

                CardShow(CardBg, "cardfront");

                if (_isHasMan)
                {
                    CardShow(CardMan, _cardMan);
                }
//                else
//                {
                    CardShow(CardColorTop, _cardColor + "t");
                    CardShow(CardColorCenter, _cardColor + "c");
//                }
            });
        }


        private void CardShow(UISprite sprite, string value)
        {
            sprite.spriteName = value;
            sprite.gameObject.SetActive(true);
        }

        public string GetCardValue(int cardValue)
        {
            var color = cardValue & 0xF0;
            var value = cardValue & 0x0F;

            var cardName = "";
            _cardColor = "";
            _cardMan = "";
            switch (color)
            {
                case 0x10:
                    cardName += "red_";
                    _cardColor += "f_";
                    _cardMan += "f_";
                    break;
                case 0x20:
                    cardName += "red_";
                    _cardColor += "r_";
                    _cardMan += "f_";
                    break;
                case 0x30:
                    cardName += "black_";
                    _cardColor += "m_";
                    _cardMan += "m_";
                    break;
                case 0x40:
                    cardName += "black_";
                    _cardColor += "b_";
                    _cardMan += "f_";
                    break;
            }

            if (value > 10)
            {
                if (value != 14 && value != 15)
                {
                    _isHasMan = true;
                }
                else
                {
                    _isHasMan = false;
                }
            }
            else
            {
                _isHasMan = false;
            }
            cardName += string.Format("{0:D2}", value);
            _cardMan += string.Format("{0:D2}", value);
            return cardName;
        }

        public void Clear()
        {
            CardBg.GetComponent<TweenRotation>().PlayReverse();
            CardBg.gameObject.GetComponent<TweenRotation>().onFinished.Clear();
            CardBg.spriteName = "cardback";
            CardMan.gameObject.SetActive(false);
            CardValue.gameObject.SetActive(false);
            CardColorTop.gameObject.SetActive(false);
            CardColorCenter.gameObject.SetActive(false);
        }
    }
}
