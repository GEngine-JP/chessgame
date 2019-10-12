﻿namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [UIPanelData(typeof(PanelQueryHuCard), UIPanelhierarchy.Popup)]
    public class PanelQueryHuCard : UIPanelBase, IUIPanelControl<QueryHuArgs>
    {
        public StyleQueryHu StyleQueryHu;
              
        //public RectTransform Container;
        //public GridLayoutGroup Group;

        //public UIItemsManager Store { get; set; }

        public override void OnInit()
        {
            StyleQueryHu.OnInit();
            //Store = GetComponent<UIItemsManager>();
            //Store.Parent = Group.transform;
        }

        public override void OnEndGameUpdate()
        {
            Close();
        }

        public void Open(QueryHuArgs args)
        {
            base.Open();
            StyleQueryHu.Open(args);

            //Store.HideItems();
            //var rateArray = args.RateArray;
            //var huCards = args.AllowHuCards;
            //for (int i = 0; i < huCards.Length; i++)
            //{
            //    var item = Store.GetItem<QueryHuCardItem>(i);
            //    if (item != null)
            //    {
            //        var card = huCards[i];
            //        var num = 0;
            //        if (card > 0)
            //        {
            //            num = GameCenter.Shortcuts.MahjongQuery.Query(card);
            //        }

            //        item.SetData(card, num);
            //        if (rateArray != null) item.Rate = rateArray[i];
            //    }
            //}
            //SetGroupSize(huCards.Length);
        }

        //private void SetGroupSize(int number)
        //{
        //    if (number == 0) return;
        //    var num = number >= 7 ? 7 : number;
        //    var width = 260 + (num - 1) * 90;

        //    var RowNum = Mathf.FloorToInt(number / 7.1f);
        //    var high = 200 + (RowNum) * 115;

        //    Container.sizeDelta = new Vector2(width, high);
        //}
    }
}