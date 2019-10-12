using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.rbwar
{
    public class TrendCtrl : MonoBehaviour
    {
        public UIGrid SpotGrid;
        public UISprite SpotItem;

        public UIGrid CardTypeGrid;
        public UISprite CardTypeItem;


        public void SetRecord(bool isSmooth=false)
        {
            var gdata = App.GetGameData<RbwarGameData>();
            var recordSpot = gdata.RecordSpot; 
            var recordCardType = gdata.RecordCardType;
//            var recordWinValue = gdata.RecordWinValue;

          var spotCout = recordSpot.Count;
            var cardTypeCout = recordCardType.Count;

            if (spotCout == 0|| cardTypeCout == 0) return;

            var spotEnough = false;

            if (SpotGrid.transform.childCount == 20)
            {
                spotEnough = true;
            }
            else
            {
                while (SpotGrid.transform.childCount > 0)
                {
                    DestroyImmediate(SpotGrid.transform.GetChild(0).gameObject);
                }
            }

            var index = 0;

            if (spotCout > 20)
            {
                spotCout -= 20;
                for (int i = spotCout; i < recordSpot.Count; i++)
                {
                    if (spotEnough)
                    {
                        var item = SpotGrid.transform.GetChild(index).GetComponent<UISprite>();
                        item.spriteName = recordSpot[i] == 0 ? "redSpot" : "blackSpot";
                        item.name = i.ToString();
                        index++;
                    }
                    else
                    {
                        var item = YxWindowUtils.CreateItem(SpotItem, SpotGrid.transform);
                        item.spriteName = recordSpot[i] == 0 ? "redSpot" : "blackSpot";
                        item.name = i.ToString();
                    }
                }
            }
            else
            {
                
                for (int i = 0; i <spotCout ; i++)
                {
                    var item = YxWindowUtils.CreateItem(SpotItem, SpotGrid.transform);
                    item.spriteName = recordSpot[i] == 0 ? "redSpot" : "blackSpot";
                    item.name = i.ToString();
                }
            }

          

            var cardtypeEnoufh = false;
            if (CardTypeGrid.transform.childCount == 7)
            {
                cardtypeEnoufh = true;
            }
            else
            {
                while (CardTypeGrid.transform.childCount > 0)
                {
                    DestroyImmediate(CardTypeGrid.transform.GetChild(0).gameObject);
                }
            }

            index = 0;

            if (cardTypeCout > 7)
            {
               
                cardTypeCout -= 7;
                for (int i = cardTypeCout; i < recordCardType.Count; i++)
                {
                    if (cardtypeEnoufh)
                    {
                        var item = CardTypeGrid.transform.GetChild(index).GetComponent<UISprite>();
                        item.spriteName = WinCardType(recordCardType[i]);
                        item.name = i.ToString();
                        index++;
                    }
                    else
                    {
                        var item = YxWindowUtils.CreateItem(CardTypeItem, CardTypeGrid.transform);
                        item.spriteName = WinCardType(recordCardType[i]);
                        item.name = i.ToString();
                    }
                }
            }
            else
            {
                for (int i = 0; i< cardTypeCout; i++)
                {
                    var item = YxWindowUtils.CreateItem(CardTypeItem, CardTypeGrid.transform);
                    item.spriteName = WinCardType(recordCardType[i]);
                    item.name = i.ToString();
                }
            }
            
            if (isSmooth)
            {
                SpotGrid.animateSmoothly = true;
                CardTypeGrid.animateSmoothly = true;
            }

            SpotGrid.repositionNow = true;
            CardTypeGrid.repositionNow = true;
        }


        private string WinCardType(int type)
        {
            var cardType = "";
            switch (type)
            {
                case (int)CardType.DanPai:
                    cardType = "h_";
                    cardType += "danzhang";
                    break;
                case (int)CardType.DuiZi:
                    cardType = "l_";
                    cardType += "duizi";
                    break;
                case (int)CardType.ShunZi:
                    cardType = "l_";
                    cardType += "shunzi";
                    break;
                case (int)CardType.JinHua:
                    cardType = "l_";
                    cardType += "jinhua";
                    break;
                case (int)CardType.ShunJin:
                    cardType = "l_";
                    cardType += "shunjin";
                    break;
                case (int)CardType.BaoZi:
                    cardType = "l_";
                    cardType += "baozi";
                    break;
            }

            return cardType;
        }

    }
}
