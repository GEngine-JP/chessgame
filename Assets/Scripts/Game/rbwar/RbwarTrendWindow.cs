using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.rbwar
{
    public class RbwarTrendWindow : YxNguiWindow
    {
        public GameObject Parent;
        public UIGrid TopSpotGrid;
        public UISprite TopSpriteItem;
        public UISprite RedPercentSprite;
        public UISprite BlackPercentSprite;

        public GameObject TwentyRoundResult;

        public UILabel RedCountLabel;
        public UILabel BlackCountLabel;
        public UILabel TotaCountLabel;

        public UIGrid BottomCardTypeGrid;
        public UISprite BottomSpriteItem;

        public UIScrollBar DishRoadScrollBar;
        public UIGrid DishRoadGrid;
        public UISprite DishRoadSpriteItem;

        public UIScrollBar BigRoadScrollBar;
        public UIGrid BigRoadGrid;
        public Transform BigRoadParenTransform;
        public UISprite BigRoadSpriteItem;

        public UIScrollBar BigEyeRoadScrollBar;
        public UIGrid BigEyeRoadGrid;
        public Transform BigEyeRoadParenTransform;
        public UISprite BigEyeRoadSpriteItem;

        public UIScrollBar SmallRoadScrollBar;
        public UIGrid SmallRoadGrid;
        public Transform SmallRoadParenTransform;
        public UISprite SmallRoadSpriteItem;

        public UIScrollBar RoachRoadScrollBar;
        public UIGrid RoachRoadGrid;
        public Transform RoachRoadParentTransform;
        public UISprite RoachRoadSpriteItem;

        public UISprite NextBlackBigEye;
        public UISprite NextBlackSmall;
        public UISprite NextBlackRoach;

        public UISprite NextRedBigEye;
        public UISprite NextRedSmall;
        public UISprite NextRedRoach;

        private RbwarGameData Gdata
        {
            get { return App.GetGameData<RbwarGameData>(); }
        }

        public void OnClose()
        {
            Parent.SetActive(false);
        }

        private bool _isFirst;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            if (Data is bool)
            {
                _isFirst = (bool)Data;
            }
            Parent.SetActive(true);
            FreshTopAndBottom(); //刷新顶部的战绩
            DishRoadShow(); //珠盘路的战绩

            GetOtherRoad(ShowData(Gdata.RecordSpot));//其余几路
            GetNext();
        }

        private int[] ShowData(List<int> recordSpot, bool change = true)
        {
            int[] newInts;

            if (recordSpot.Count >= 70)
            {
                var count = recordSpot.Count - 70;
                var redCount = 0;
                var index = 0;
                newInts = new int[70];
                for (int i = count; i < recordSpot.Count - 1; i++)
                {

                    if (recordSpot[i] == 0)
                    {
                        redCount++;
                    }

                    newInts[index] = recordSpot[i];
                    index++;
                }

                if (change)
                {
                    RedCountLabel.text = string.Format("红 {0}", redCount);
                    RedCountLabel.gameObject.SetActive(true);
                    BlackCountLabel.text = string.Format("黑 {0}", 70 - redCount);
                    BlackCountLabel.gameObject.SetActive(true);
                    TotaCountLabel.gameObject.SetActive(true);
                }

            }
            else
            {
                newInts = recordSpot.ToArray();
            }

            return newInts;
        }


        public void FreshTopAndBottom()
        {
            var recordSpot = Gdata.RecordSpot;
            var recordCardType = Gdata.RecordCardType;

            var spotEnough = false;

            if (TopSpotGrid.transform.childCount == 20)
            {
                spotEnough = true;
            }
            else
            {
                while (TopSpotGrid.transform.childCount > 0)
                {
                    DestroyImmediate(TopSpotGrid.transform.GetChild(0).gameObject);
                    DestroyImmediate(BottomCardTypeGrid.transform.GetChild(0).gameObject);
                }
            }

            var index = 0;

            if (recordSpot.Count < 20)
            {
                for (int i = 0; i < recordSpot.Count; i++)
                {
                    var item1 = YxWindowUtils.CreateItem(TopSpriteItem, TopSpotGrid.transform);
                    item1.spriteName = recordSpot[i] == 0 ? "topRed" : "topBlack";
                    item1.name = i.ToString();

                    var item2 = YxWindowUtils.CreateItem(BottomSpriteItem, BottomCardTypeGrid.transform);
                    item2.spriteName = WinCardType(recordCardType[i]);
                    item2.name = i.ToString();
                }
            }
            else
            {
                var startIndex = recordSpot.Count - 20;

                TwentyRoundResult.SetActive(true); //满20局的时候显示20的胜负情况求出百分比
                var redCount = 0;
                for (int i = startIndex; i < recordSpot.Count; i++)
                {
                    if (recordSpot[i] == 0)
                    {
                        redCount++;
                    }
                }

                float redValue = redCount * 1.0f / 20;
                var redDot = Math.Round(redValue, 2, MidpointRounding.AwayFromZero);

                RedPercentSprite.width = int.Parse(Mathf.Floor(redValue * 591).ToString()) + 70;

                RedPercentSprite.GetComponentInChildren<UILabel>().text = string.Format("{0}%", redDot * 100);
                BlackPercentSprite.GetComponentInChildren<UILabel>().text = string.Format("{0}%", (1 - redDot) * 100);

                for (int i = startIndex; i < recordSpot.Count; i++)
                {
                    if (spotEnough)
                    {
                        var item1 = TopSpotGrid.transform.GetChild(index).GetComponent<UISprite>();
                        item1.spriteName = recordSpot[i] == 0 ? "topRed" : "topBlack";
                        item1.name = i.ToString();

                        var item2 = BottomCardTypeGrid.transform.GetChild(index).GetComponent<UISprite>();
                        item2.spriteName = WinCardType(recordCardType[i]);
                        item2.name = i.ToString();
                        index++;

                    }
                    else
                    {
                        var item1 = YxWindowUtils.CreateItem(TopSpriteItem, TopSpotGrid.transform);
                        item1.spriteName = recordSpot[i] == 0 ? "topRed" : "topBlack";
                        item1.name = i.ToString();

                        var item2 = YxWindowUtils.CreateItem(BottomSpriteItem, BottomCardTypeGrid.transform);
                        item2.spriteName = WinCardType(recordCardType[i]);
                        item2.name = i.ToString();
                    }
                }
            }

            BottomCardTypeGrid.transform.GetChild(BottomCardTypeGrid.transform.childCount - 1).GetChild(0).gameObject.SetActive(true);
            TopSpotGrid.repositionNow = true;
            BottomCardTypeGrid.repositionNow = true;
        }

        public string WinCardType(int type)
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

        public void DishRoadShow()
        {
            var recordSpot = Gdata.RecordSpot;

            var spotEnough = false;

            if (DishRoadGrid.transform.childCount == 20)
            {
                spotEnough = true;

            }
            else
            {
                while (DishRoadGrid.transform.childCount > 0)
                {
                    DestroyImmediate(DishRoadGrid.transform.GetChild(0).gameObject);
                }
            }

            var index = 0;

            if (recordSpot.Count <= 70)
            {
                if (recordSpot.Count >= 60)
                {
                    DishRoadScrollBar.value = 1;
                }
                for (int i = 0; i < recordSpot.Count; i++)
                {
                    var item = YxWindowUtils.CreateItem(DishRoadSpriteItem, DishRoadGrid.transform);
                    item.spriteName = recordSpot[i] == 0 ? "leftRed" : "leftBlack";
                    item.name = i.ToString();
                }
            }
            else
            {
                DishRoadScrollBar.value = 1;
                var startIndex = recordSpot.Count - 70;

                for (int i = startIndex; i < recordSpot.Count; i++)
                {
                    if (spotEnough)
                    {
                        var item1 = DishRoadGrid.transform.GetChild(index).GetComponent<UISprite>();
                        item1.spriteName = recordSpot[i] == 0 ? "leftRed" : "leftBlack";
                        item1.name = i.ToString();
                    }
                    else
                    {
                        var item1 = YxWindowUtils.CreateItem(DishRoadSpriteItem, DishRoadGrid.transform);
                        item1.spriteName = recordSpot[i] == 0 ? "leftRed" : "leftBlack";
                        item1.name = i.ToString();
                    }
                }
            }

            DishRoadGrid.repositionNow = true;

            if (!_isFirst)
            {
                StartCoroutine(FlashSprite(DishRoadGrid.transform));
            }
        }

        IEnumerator FlashSprite(Transform parent)
        {
            if (parent.childCount == 0)
            {
                yield return new WaitForSeconds(0);
            }
            var item = parent.GetChild(parent.childCount - 1)
                .GetComponent<TweenAlpha>();
            item.PlayForward();
            var needTime = item.duration;

            yield return new WaitForSeconds(needTime * 3);
            item.enabled = false;
        }


        public void GetOtherRoad(int[] data)
        {
            var road = new RoadNodeTable(data, 6);

            while (BigRoadParenTransform.transform.childCount > 0)
            {
                DestroyImmediate(BigRoadParenTransform.transform.GetChild(0).gameObject);
            }
            while (BigEyeRoadParenTransform.transform.childCount > 0)
            {
                DestroyImmediate(BigEyeRoadParenTransform.transform.GetChild(0).gameObject);
            }
            while (SmallRoadParenTransform.transform.childCount > 0)
            {
                DestroyImmediate(SmallRoadParenTransform.transform.GetChild(0).gameObject);
            }
            while (RoachRoadParentTransform.transform.childCount > 0)
            {
                DestroyImmediate(RoachRoadParentTransform.transform.GetChild(0).gameObject);
            }


            for (int i = 0; i < road.Nodes.Count; i++)
            {
                var item = YxWindowUtils.CreateItem(BigRoadSpriteItem, BigRoadParenTransform);
                item.spriteName = road.Nodes[i].IsRed ? "bigRed" : "bigBlack";
                item.transform.localPosition = new Vector3((road.Nodes[i].X - 1) * 18.3f, (road.Nodes[i].Y - 1) * -17.9f, 0);
            }

            var bigRoadLine = road.LineCount.Count;

            //            Debug.LogError("-------------------------bigRoadLine count"+ bigRoadLine);
            while (bigRoadLine > 32)
            {
                BigRoadScrollBar.value = 1;
                BigRoadGrid.transform.GetChild(bigRoadLine).gameObject.SetActive(true);
                bigRoadLine -= 1;
            }

            if (road.LineCount.Count <= 32)
            {
                for (int i = 32; i < 42; i++)
                {
                    if (BigRoadGrid.transform.GetChild(i).gameObject.activeSelf)
                    {
                        BigRoadGrid.transform.GetChild(i).gameObject.SetActive(false);
                    }
                }
            }

            BigRoadGrid.repositionNow = true;

            if (!_isFirst)
            {
                StartCoroutine(FlashSprite(BigRoadParenTransform));
            }

            var bigEyeRoad = new RoadNodeTable(road, EnumTrendType.BigEyeRoad, 6);
            for (int i = 0; i < bigEyeRoad.Nodes.Count; i++)
            {
                var item = YxWindowUtils.CreateItem(BigEyeRoadSpriteItem, BigEyeRoadParenTransform);
                item.spriteName = bigEyeRoad.Nodes[i].IsRed ? "smallYQRed" : "smallYQBlack";
                item.transform.localPosition = new Vector3((bigEyeRoad.Nodes[i].X - 1) * 9.1f, (bigEyeRoad.Nodes[i].Y - 1) * -9.2f, 0);
            }

            var bigEyeRoadLine = bigEyeRoad.LineCount.Count;

            //            Debug.LogError("-------------------------bigEyeRoadLine count" + bigEyeRoadLine);
            while (bigEyeRoadLine > 32)
            {
                BigEyeRoadScrollBar.value = 1;
                var bigEyeLine = (int)Math.Ceiling(bigEyeRoadLine * 1.0f / 2);
                BigEyeRoadGrid.transform.GetChild(bigEyeLine).gameObject.SetActive(true);
                bigEyeRoadLine -= 1;

            }

            if (bigEyeRoad.LineCount.Count <= 32)
            {
                for (int i = 16; i < 26; i++)
                {
                    if (BigEyeRoadGrid.transform.GetChild(i).gameObject.activeSelf)
                    {
                        BigEyeRoadGrid.transform.GetChild(i).gameObject.SetActive(false);
                    }
                }
            }

            BigEyeRoadGrid.repositionNow = true;

            if (!_isFirst)
            {
                StartCoroutine(FlashSprite(BigEyeRoadParenTransform));
            }

            var smallRoad = new RoadNodeTable(road, EnumTrendType.SmallRoad, 6);
            for (int i = 0; i < smallRoad.Nodes.Count; i++)
            {
                var item = YxWindowUtils.CreateItem(SmallRoadSpriteItem, SmallRoadParenTransform);
                item.spriteName = smallRoad.Nodes[i].IsRed ? "smallYSRed" : "smallYSBlack";
                item.transform.localPosition = new Vector3((smallRoad.Nodes[i].X - 1) * 9.1f, (smallRoad.Nodes[i].Y - 1) * -9.2f, 0);
            }

            var smallRoadLine = smallRoad.LineCount.Count;

            //            Debug.LogError("-------------------------smallRoadLine count" + smallRoadLine);

            while (smallRoadLine > 32)
            {
                SmallRoadScrollBar.value = 1;
                var smallLine = (int)Math.Ceiling(smallRoadLine * 1.0f / 2);
                SmallRoadGrid.transform.GetChild(smallLine).gameObject.SetActive(true);
                smallRoadLine -= 1;
            }

            if (smallRoad.LineCount.Count <= 32)
            {
                for (int i = 16; i < 26; i++)
                {
                    if (SmallRoadGrid.transform.GetChild(i).gameObject.activeSelf)
                    {
                        SmallRoadGrid.transform.GetChild(i).gameObject.SetActive(false);
                    }
                }
            }

            SmallRoadGrid.repositionNow = true;


            if (!_isFirst)
            {
                StartCoroutine(FlashSprite(SmallRoadParenTransform));
            }

            var roachRoad = new RoadNodeTable(road, EnumTrendType.RoachRoad, 6);
            for (int i = 0; i < roachRoad.Nodes.Count; i++)
            {
                var item = YxWindowUtils.CreateItem(RoachRoadSpriteItem, RoachRoadParentTransform);
                item.spriteName = roachRoad.Nodes[i].IsRed ? "smallSRed" : "smallSBalck";
                item.transform.localPosition = new Vector3((roachRoad.Nodes[i].X - 1) * 9.1f, (roachRoad.Nodes[i].Y - 1) * -9.2f, 0);
            }

            var roachRoadLine = roachRoad.LineCount.Count;

            //            Debug.LogError("-------------------------roachRoadLine count" + roachRoadLine);

            while (roachRoadLine > 32)
            {
                RoachRoadScrollBar.value = 1;
                var roachLine = (int)Math.Ceiling(roachRoadLine * 1.0f / 2);
                RoachRoadGrid.transform.GetChild(roachLine).gameObject.SetActive(true);
                roachRoadLine -= 1;
            }

            if (roachRoad.LineCount.Count <= 32)
            {
                for (int i = 16; i < 26; i++)
                {
                    if (RoachRoadGrid.transform.GetChild(i).gameObject.activeSelf)
                    {
                        RoachRoadGrid.transform.GetChild(i).gameObject.SetActive(false);
                    }
                }
            }

            RoachRoadGrid.repositionNow = true;


            if (!_isFirst)
            {
                StartCoroutine(FlashSprite(RoachRoadParentTransform));
            }

        }

        private void GetNext()
        {
            Gdata.RecordSpot.Add(1);
            var road = new RoadNodeTable(ShowData(Gdata.RecordSpot, false), 6);
            var bigEyeRoad = new RoadNodeTable(road, EnumTrendType.BigEyeRoad, 6);
            NextBlackBigEye.spriteName = bigEyeRoad.Nodes[bigEyeRoad.Nodes.Count - 1].IsRed ? "smallYQRed" : "smallYQBlack";
            NextRedBigEye.spriteName = !bigEyeRoad.Nodes[bigEyeRoad.Nodes.Count - 1].IsRed ? "smallYQRed" : "smallYQBlack";
            var smallRoad = new RoadNodeTable(road, EnumTrendType.SmallRoad, 6);
            NextBlackSmall.spriteName = smallRoad.Nodes[smallRoad.Nodes.Count - 1].IsRed ? "smallYSRed" : "smallYSBlack";
            NextRedSmall.spriteName = !smallRoad.Nodes[smallRoad.Nodes.Count - 1].IsRed ? "smallYSRed" : "smallYSBlack";
            var roachRoad = new RoadNodeTable(road, EnumTrendType.RoachRoad, 6);
            NextBlackRoach.spriteName = roachRoad.Nodes[roachRoad.Nodes.Count - 1].IsRed ? "smallSRed" : "smallSBalck";
            NextRedRoach.spriteName = !roachRoad.Nodes[roachRoad.Nodes.Count - 1].IsRed ? "smallSRed" : "smallSBalck";

            Gdata.RecordSpot.RemoveAt(Gdata.RecordSpot.Count - 1);
        }

    }
}
