using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahPlayerHand_Dlmj : MahPlayerHand
    {
        public override bool SetHandCardState(HandcardStateTyps state, params object[] args)
        {
            if (base.SetHandCardState(state, args))
            {
                if (state == HandcardStateTyps.ChooseNiuTing)
                {
                    SwitchChooseNiuTingState(args);
                }
            }
            return true;
        }

        protected void SwitchChooseNiuTingState(params object[] args)
        {
            List<int> tingList = args[0] as List<int>;
            if (tingList == null || tingList.Count == 0) return;
            MahjongContainer item;
            UserContorl.ClearSelectCard();
            var list = PlayerHand.MahjongList;
            for (int i = 0; i < list.Count; i++)
            {
                item = list[i];
                item.ResetPos();
                if (!tingList.Contains(item.Value))
                {
                    item.Lock = true;
                    item.RemoveMahjongScript();
                }
            }
            for (int i = 0; i < tingList.Count; i++)
            {
                for (int j = 0; j < list.Count; j++)
                {
                    item = list[j];
                    if (item.Value == tingList[i])
                    {
                        item.SetMahjongScript();
                        item.SetThowOutCall(NiuTingClickEvent);
                    }
                }
            }
        }

        private void NiuTingClickEvent(Transform transf)
        {
            if (PlayerHand.HasToken)
            {
                MahjongContainer item;
                var Mj = transf.GetComponent<MahjongContainer>();
                if (!Mj.Laizi && !Mj.Lock)
                {
                    PlayerHand.HasToken = false;
                    //if (MahjongUtility.GameKey == GameMisc.DltdhKey)
                    DoSelectTdhNiuClick(Mj);
                    //else
                    //DoSelectNiuClick(Mj);
                    var list = PlayerHand.MahjongList;
                    for (int i = 0; i < list.Count; i++)
                    {
                        item = list[i];
                        item.SetMahjongScript();
                        item.SetThowOutCall(ThrowCardClickEvent);
                    }
                }
            }
        }

        #region 大连推到胡部分
        private void DoSelectTdhNiuClick(MahjongContainer mjItem)
        {
            var list = PlayerHand.MahjongList;
            List<int> cardValueList = new List<int>();
            for (int i = 0; i < list.Count; i++)
            {
                cardValueList.Add(list[i].Value);
            }
            int index = 0;
            for (int i = 0; i < cardValueList.Count; i++)
            {
                if (mjItem.Value == cardValueList[i])
                {
                    index = i;
                    break;
                }
            }
            int[] handCard = RemoveListItem(cardValueList.ToArray(), index, 1);
            Array.Sort(handCard);
            int isQiDui = CheckQiDui(handCard);
            if (isQiDui > 0)
            {
                int[] finalNiu = { isQiDui };
                GameCenter.Network.C2S.Custom<C2SCustom>().NiuTing(finalNiu, mjItem.Value);
                //如果打出的牌是抬起状态放下
                mjItem.ResetPos();
                return;
            }
            int[] findNiu = FindAutoLiang(handCard);
            GameCenter.Network.C2S.Custom<C2SCustom>().NiuTing(findNiu, mjItem.Value);
            //如果打出的牌是抬起状态放下
            mjItem.ResetPos();
        }

        private int CheckQiDui(int[] list)
        {
            Array.Sort(list);
            if (list.Length == 13)
            {
                int single = 0;
                int card = 0;
                for (int i = 0; i < 12; i++)
                {
                    if (list[i] == list[i + 1])
                    {
                        i++;
                    }
                    else
                    {
                        card = list[i];
                        single++;
                    }
                    if (single > 1)
                    {
                        return 0;
                    }
                }
                return card == 0 ? list[list.Length - 1] : card;
            }
            return 0;
        }

        private int[] FindAutoLiang(int[] cards, bool jianGang = true)
        {
            int[] handCards = new int[cards.Length];
            Array.Copy(cards, handCards, cards.Length);
            Array.Sort(handCards);
            int[] huarr = GetHuList(handCards);

            int i;
            //移将
            for (i = 0; i < handCards.Length - 1; i++)
            {
                if (handCards[i] == handCards[i + 1])
                {
                    int[] temp_arr = RemoveListItem(handCards, i, 2);
                    int[] temp_hu = GetHuList(temp_arr);
                    if (EquitArr(temp_hu, huarr))
                    {
                        if (temp_hu.Length != 0)
                        {
                            handCards = temp_arr;
                        }
                        break;
                    }
                }
            }
            //AAA
            for (i = 0; i < handCards.Length - 2; i++)
            {
                if (handCards[i] == handCards[i + 2])
                {
                    int[] temp_arr = RemoveListItem(handCards, i, 3);
                    int[] temp_hu = GetHuList(temp_arr);
                    if (EquitArr(temp_hu, huarr))
                    {
                        handCards = temp_arr;
                        i--;
                    }
                }
            }

            // ABC
            //for (i = 0; i < handCards.Length - 2; i++)
            //{
            //    if (handCards[i] == handCards[i + 1])
            //    {
            //        continue;
            //    }
            //    for (int j = i + 2; j < handCards.Length; j++)
            //    {
            //        if (handCards[j] - handCards[i] > 2)
            //        {
            //            break;
            //        }
            //        if (handCards[j] - handCards[i] == 2 && handCards[j] != handCards[j - 1])
            //        {
            //            if (FindOtherTwoSameCards(handCards, handCards[j - 1])
            //                || FindOtherTwoSameCards(handCards, handCards[j])
            //                || FindOtherTwoSameCards(handCards, handCards[i]))
            //            {
            //                break;
            //            }
            //            if (FindFive(handCards[j - 1], huarr)
            //                || FindFive(handCards[i], huarr)
            //                || FindFive(handCards[j], huarr))
            //            {
            //                break;
            //            }
            //            int[] temp_arr = RemoveListItem(handCards, j - 1, 2);
            //            temp_arr = RemoveListItem(temp_arr, i, 1);
            //            int[] temp_hu = GetHuList(temp_arr);
            //            if (EquitArr(temp_hu, huarr))
            //            {
            //                handCards = temp_arr;
            //                i = -1;
            //                break;
            //            }
            //        }
            //    }
            //}

            // ABC
            //收集能亮的牌值
            List<int> collection = new List<int>();
            //现有手牌
            List<int> list = new List<int>(handCards);
            for (int j = 0; j < huarr.Length; j++)
            {
                FilterUncorrelated(huarr[j], list, collection);
            }
            //确定要亮得牌
            List<int> temp = new List<int>();
            for (int k = 0; k < collection.Count; k++)
            {
                for (int j = 0; j < list.Count; j++)
                {
                    if (collection[k] == list[j])
                    {
                        temp.Add(list[j]);
                    }
                }
            }
            return temp.ToArray();
        }

        /// <summary>
        /// 过滤 与胡牌不连续得牌
        /// </summary>
        private void FilterUncorrelated(int card, List<int> list, List<int> collection)
        {
            int count = list.Count;
            int temp = card;
            //手牌中是否有胡牌
            for (int i = 0; i < count; i++)
            {
                if (list[i] == temp)
                {
                    if (!collection.Contains(temp))
                    {
                        collection.Add(temp);
                    }
                }
            }
            //向后搜索
            temp = card - 1;
            for (int i = count - 1; i >= 0; i--)
            {
                //找到连续得牌
                if (list[i] == temp)
                {
                    if (!collection.Contains(temp))
                    {
                        collection.Add(temp);
                    }
                    //是否与下一张牌 相同
                    if (i > 0 && (list[i - 1] != temp))
                    {
                        temp--;
                    }
                }
            }
            temp = card + 1;
            //向前搜索
            for (int i = 0; i < count; i++)
            {
                //找到连续得牌
                if (list[i] == temp)
                {
                    if (!collection.Contains(temp))
                    {
                        collection.Add(temp);
                    }
                    //是否与下一张牌 相同
                    if (count - 1 > i && (list[i + 1] != temp))
                    {
                        temp++;
                    }
                }
            }
        }

        private int[] GetHuList(int[] cards)
        {
            List<int> hulist = new List<int>();
            int last = 0, last1 = 0;
            for (int i = 0; i < cards.Length; i++)
            {
                int testCard = cards[i];
                //这张牌就是上一张牌
                if (testCard == last)
                {
                    continue;
                }
                //这张牌不是上一张+1
                if (testCard - 1 != last1 && testCard - 1 != last)
                {
                    if (CheckCardValueRight(testCard - 1) && MakeArrToCheckHu(cards, testCard - 1))
                    {
                        hulist.Add(testCard - 1);
                    }
                }
                if (testCard != last1)
                {
                    if (CheckCardValueRight(testCard) && MakeArrToCheckHu(cards, testCard))
                    {
                        hulist.Add(testCard);
                    }
                }

                last1 = testCard + 1;
                if (CheckCardValueRight(last1) && MakeArrToCheckHu(cards, last1))
                {
                    hulist.Add(last1);
                }
                last = testCard;
            }
            return hulist.ToArray();
        }

        private bool CheckToHu(int[] list, bool checkJiang)
        {
            Array.Sort(list);
            int size = list.Length;
            if (size == 0)
            {
                return true;
            }
            int i;
            //将
            if (checkJiang)
            {
                for (i = 0; i < size - 1; i++)
                {
                    if (list[i] == list[i + 1])
                    {
                        if (CheckToHu(RemoveListItem(list, i, 2), false))
                        {
                            return true;
                        }
                        i++;
                    }
                }
            }
            //AAA
            for (i = 0; i < size - 2; i++)
            {
                if (list[i] == list[i + 2])
                {
                    if (CheckToHu(RemoveListItem(list, i, 3), false))
                    {
                        return true;
                    }
                }
            }
            //ABC
            for (i = 0; i < size - 2; i++)
            {
                if (list[i] == list[i + 1])
                {
                    continue;
                }
                for (int j = i + 2; j < size; j++)
                {
                    if (list[j] - list[i] > 2)
                    {
                        break;
                    }
                    if (list[j] - list[i] == 2 && list[j] != list[j - 1])
                    {
                        int[] cpList = RemoveListItem(list, j - 1, 2);
                        cpList = RemoveListItem(cpList, i, 1);
                        if (CheckToHu(cpList, false))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool CheckCardValueRight(int card)
        {
            return card % 16 > 0 && card % 16 < 10 || card == 74;
        }

        private bool MakeArrToCheckHu(int[] cards, int hu)
        {
            int[] m_cards = new int[cards.Length + 1];
            Array.Copy(cards, m_cards, cards.Length);
            m_cards[m_cards.Length - 1] = hu;
            return CheckToHu(m_cards, m_cards.Length % 3 == 2);
        }

        private bool FindOtherTwoSameCards(int[] handCards, int same)
        {
            int count = 0;
            for (int i = 0; i < handCards.Length; i++)
            {
                if (same == handCards[i])
                {
                    count++;
                }
            }
            return count > 2;
        }

        private bool FindFive(int card, int[] huarr)
        {
            if (!huarr.Contains(card) || card % 16 != 5)
            {
                return false;
            }
            foreach (var hucard in huarr)
            {
                if (hucard == card)
                {
                    return true;
                }
            }
            return false;
        }

        private bool EquitArr(int[] arr1, int[] arr2)
        {
            if (arr1.Length != arr2.Length)
            {
                return false;
            }
            for (int i = 0; i < arr1.Length; i++)
            {
                if (arr1[i] != arr2[i])
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        private int[] RemoveListItem(int[] list, int index, int num)
        {
            int[] mList = new int[list.Length - num];
            for (int i = 0; i < index; i++)
            {
                mList[i] = list[i];
            }
            for (int i = index + num; i < list.Length; i++)
            {
                mList[i - num] = list[i];
            }
            return mList;
        }

        #region 大连穷胡部分
        /// <summary>
        /// 大连穷胡
        /// </summary>
        /// <param name="mjItem"></param>
        //private void DoSelectNiuClick(MahjongContainer mjItem)
        //{
        //    var list = PlayerHand.MahjongList;
        //    List<int> _cardValueList = new List<int>();
        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        _cardValueList.Add(list[i].Value);
        //    }
        //    int index = 0;
        //    for (int i = 0; i < _cardValueList.Count; i++)
        //    {
        //        if (mjItem.Value == _cardValueList[i])
        //        {
        //            index = i;
        //            break;
        //        }
        //    }
        //    int[] handCard = RemoveListItem(_cardValueList.ToArray(), index, 1);
        //    Array.Sort(handCard);
        //    int[] arr1 = CheckTingPai(handCard, true, 0);
        //    if (arr1.Length == 1 && arr1[0] == -1)
        //        arr1 = CheckTingPai(handCard, false, 0);
        //    int[] arr2 = new int[1];
        //    if (arr1.Length > 1)
        //    {
        //        arr2 = CheckTingPai(handCard, true, arr1[2]);
        //        arr1[2] = 0;
        //    }
        //    if (arr2.Length > 1 && arr1.Length > 1)
        //    {
        //        List<int[]> findList = new List<int[]>();
        //        findList.Add(arr1);
        //        findList.Add(arr2);
        //        GameCenter.NetworkComponent.C2S.Custom<C2SCustom>().ChooseNiuTing(findList, mjItem.Value);
        //    }
        //    else if (arr1.Length == 1)
        //    {
        //        int[] finalNiu = arr1;
        //        GameCenter.NetworkComponent.C2S.Custom<C2SCustom>().NiuTing(finalNiu, mjItem.Value);
        //        //如果打出的牌是抬起状态放下
        //        if (mjItem.Roll != null)
        //        {
        //            mjItem.Roll.ResetPos();
        //        }
        //    }
        //    else
        //    {
        //        int[] finalNiu = { arr1[0], arr1[1] };
        //        GameCenter.NetworkComponent.C2S.Custom<C2SCustom>().NiuTing(finalNiu, mjItem.Value);
        //        //如果打出的牌是抬起状态放下
        //        if (mjItem.Roll != null)
        //        {
        //            mjItem.Roll.ResetPos();
        //        }
        //    }
        //}

        /// <summary>
        /// 大连穷胡判断听牌
        /// </summary>
        /// <param name="list"></param>
        /// <param name="checkJiang"></param>
        /// <param name="jiang"></param>
        /// <returns></returns>
        //private int[] CheckTingPai(int[] list, bool checkJiang, int jiang)
        //{
        //    int size = list.Length;
        //    if (size == 1)
        //    {
        //        return list;
        //    }
        //    if (size == 2)
        //    {
        //        if (list[1] - list[0] == 2 || (list[1] - list[0] == 1 && list[0] % 16 == 1) || (list[1] - list[0] == 1 && list[0] % 16 == 8))
        //        {
        //            return list;
        //        }
        //    }
        //    int i;
        //    int[] checkArr;
        //    //将
        //    if (checkJiang)
        //    {
        //        for (i = 0; i < size - 1; i++)
        //        {
        //            if (jiang == list[i])
        //            {
        //                continue;
        //            }
        //            if (list[i] == list[i + 1])
        //            {
        //                checkArr = CheckTingPai(RemoveListItem(list, i, 2), false, 0);
        //                if (checkArr[0] == -1)
        //                {
        //                    continue;
        //                }
        //                else if (checkArr.Length > 1)
        //                {
        //                    checkArr = new[] { checkArr[0], checkArr[1], list[i] };
        //                    return checkArr;
        //                }
        //                i++;
        //            }
        //        }
        //    }
        //    //AAA
        //    for (i = 0; i < size - 2; i++)
        //    {
        //        if (list[i] == list[i + 2])
        //        {
        //            checkArr = CheckTingPai(RemoveListItem(list, i, 3), false, 0);
        //            if (checkArr[0] == -1)
        //            {
        //                continue;
        //            }
        //            else if (checkArr.Length > 1)
        //            {
        //                checkArr = new[] { checkArr[0], checkArr[1], list[i] };
        //                return checkArr;
        //            }
        //            else if (checkArr.Length == 1)
        //            {
        //                return checkArr;
        //            }
        //        }
        //    }
        //    //ABC
        //    for (i = 0; i < size - 2; i++)
        //    {
        //        if (list[i] == list[i + 1])
        //        {
        //            continue;
        //        }
        //        for (int j = i + 2; j < size; j++)
        //        {
        //            if (list[j] - list[i] > 2)
        //            {
        //                break;
        //            }
        //            if (list[j] - list[i] == 2 && list[j] != list[j - 1])
        //            {
        //                int[] cpList = RemoveListItem(list, j - 1, 2);
        //                cpList = RemoveListItem(cpList, i, 1);
        //                checkArr = CheckTingPai(cpList, false, 0);
        //                if (checkArr[0] == -1)
        //                {
        //                    continue;
        //                }
        //                else if (checkArr.Length > 1)
        //                {
        //                    checkArr = new[] { checkArr[0], checkArr[1], list[i] };
        //                    return checkArr;
        //                }
        //                else if (checkArr.Length == 1)
        //                {
        //                    return checkArr;
        //                }
        //            }
        //        }
        //    }
        //    return new[] { -1 };
        //}

        #endregion
    }
}