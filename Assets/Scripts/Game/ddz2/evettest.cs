using System;
using System.Collections;
using System.Linq;
using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Assets.Scripts.Game.ddz2.InheritCommon;
using Assets.Scripts.Game.ddz2.PokerCdCtrl;
using Assets.Scripts.Game.ddz2.PokerRule;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using YxFramwork.ConstDefine;
using YxFramwork.View;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.ddz2
{
    public class evettest : MonoBehaviour {
    
        private void Awake()
        {
            //StartCoroutine(dddssf());
        }


        private IEnumerator dddssf()
        {
            yield return new WaitForSeconds(2f);
         
            //解散房间
           // YxMessageBox.Show("ddz2","MessageBox","sadklfjlfsdasff",null,null,false,1,null);
            YxMessageBox.Show("确定要解散房间么？","", (box, btnName) =>
            {
                if (btnName == YxMessageBox.BtnLeft)
                {
                    YxDebug.LogError("btnleft");
                }
            }, true,YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle);
               
        }

        private void oneventTrggler(object obj,DdzbaseEventArgs args)
        {
            /*        if (args.GetServDataType == ServDataType.OnGetGameInfo)
                    {
                        var isfobjData = args.IsfsObjData;
                        YxDebug.LogError(isfobjData.GetDump());
                    }*/
        }

        public void DDOosumulate()
        {
            gameObject.SetActive(true);
            var particalSyStem = gameObject.GetComponent<ParticleSystem>();
            particalSyStem.Stop();
            particalSyStem.Play();
        }


        /** 判断是否排序好 */
        private static bool isSequenceArr(List<int> list)
        {
            bool result = false;

            int count = list.Count;
            if (count != 0)
            {
                int first = list[0];
                int last = list[count - 1];
                if (Math.Abs(first - last) == count - 1 && (last < 15) && first < 15)
                {
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// 查找可能组成的最大连续牌值组（这里的龙指的是 链子的情况，可能包括 2连，3连 4连.... 取决于lianNumLimt）的牌值下限
        /// </summary>
        /// <param name="sortedCdvalues"></param>
        /// <param name="lianLen"></param>
        /// <returns></returns>
        protected int[] FindBestLian(int[] sortedCdvalues, int lianLen)
        {
            //能的组成龙的牌值上限
            const int longCdHighLimitValue = 14;
            //能的组成龙的牌值下限
            const int longCdLowLimitValue = 3;

            var minvalue = sortedCdvalues[0];
            var maxvlaue = sortedCdvalues[sortedCdvalues.Length - 1];

            //排除不可能组成龙的情况
            if (sortedCdvalues.Any(cd => cd > longCdHighLimitValue)) return null;

            //排除最小值和最大值的区间太大 无法组成龙的情况
            if (maxvlaue - minvalue + 1 > lianLen) return null;

            //找到最大龙的龙下限
            while (true)
            {
                if (minvalue + lianLen - 1 > longCdHighLimitValue) minvalue--;
                else break;
            }
            if (minvalue < longCdLowLimitValue) return null;

            var lian = new int[lianLen];
            var j = 0;
            for (var i = minvalue; i < minvalue+lianLen; i++)
            {
                lian[j++] = i;
            }
            return lian;
        }

        /// <summary>
        /// 打印一组牌
        /// </summary>
        /// <param name="cds"></param>
        private void Debugcds(IEnumerable<int> cds)
        {
            var str = cds.Aggregate("", (current, cd) => current + (cd + " "));

            YxDebug.LogError("牌组：" + str);
        }

        /// <summary>
        /// 获得一组大顺（cds）中可能分解出的所有小顺子
        /// </summary>
        /// <param name="cds">一组顺牌</param>
        /// <param name="allShun">存储顺牌的容器</param>
        /// <param name="minlenLimit">连牌的最小长度</param>
        private void FindAllPosbShun(int[] cds,List<int[]> allShun,int minlenLimit)
        {
          
            var cdsLen = cds.Length;
            if (cdsLen < minlenLimit) return;

            int lasti = minlenLimit;
        
            while (lasti <= cdsLen)
            {
                var shunCds = new int[lasti];
                for (int i = 0; i < lasti; i++)
                {
                    shunCds[i] = cds[i];
                }
                allShun.Add(shunCds);
                lasti++;
            }
            var newCds = new int[cdsLen - 1];
            Array.Copy(cds, 1, newCds, 0, cdsLen - 1);
            FindAllPosbShun(newCds, allShun, minlenLimit);
        }


        private void DebugCdsTypesDic(Dictionary<int[], CardType> dic)
        {
            YxDebug.LogError("字典长度：" + dic.Count);

            foreach (var cards in dic.Keys)
            {
                var str = dic[cards].ToString()+":";
                str += cards.Aggregate("", (current, cd) => current + (cd + " "));
                YxDebug.LogError(str);
            }
        }

        // Use this for initialization
        void Start ()
        {
/*            var cardManager = new CardManager();
            var dic = cardManager.GetAllLegalCdsTypeDic(new int[] { 3, 3, 3, 4, 4, 5, 5, 6, 6, 8, 8, 7, 9, 9, 9, 9, 10, 10, 11, 11, 12, 12, 12 ,PokerRuleUtil.SmallJoker,PokerRuleUtil.BigJoker});
            DebugCdsTypesDic(dic);*/


            /*            var clist = new List<int>();
                        clist.AddRange(new int[]{1,2,3});


                        var clist2 = new List<int>();
                        clist2.AddRange(clist);

                        clist2[0] = 9;
                        clist2[1] = 9;
                        clist2[2] = 9;

                        foreach (var i in clist)
                        {
                            YxDebug.LogError(i);
                        }*/
            /*
                        CdSplitStruct cdSplit = new CdSplitStruct(new int[]{3,3,3,4,4,5,5,6,6,8,8,7,9,9,9,9,10,10,11,11,12,12,12});

                        var listTemp = new List<int>();
                        listTemp.AddRange(cdSplit.DuiCds);
                        listTemp.AddRange(cdSplit.ThreeCds);
                        listTemp.AddRange(cdSplit.FourCds);

                        var allshunList = PokerRuleUtil.AnalyAllPartOfShun(listTemp.ToArray(), 3);
                        foreach (var cds in allshunList)
                        {
                            Debugcds(cds);
                        }*/

            // var cds = new int[] { 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 };
            /*            var ddd = PokerRuleUtil.AnalyAllPartOfShun(new int[] {3, 4,  6, 7, 8, 9, 10, 11, 12, 13, 14}, 5);
                        YxDebug.LogError("总长度："+ddd.Count);
                        foreach (var cds in ddd)
                        {
                            Debugcds(cds);
                        }*/
            /*
                        var posbShunList = PokerRuleUtil.GetAllPossibleShun(new int[] { 2, 3, 4, 5, 6}, 3);
                        var allShun = new List<int[]>();

                        foreach (var cds in posbShunList)
                        {
                            FindAllPosbShun(cds, allShun,3);
         
                        }
                        YxDebug.LogError("allshun长度：" + allShun.Count);
                        foreach (var intse in allShun)
                        {
                            Debugcds(intse);
                        }
            */

            //int[] cds = { 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 };


            //cds.
/*            var cds = new int[] { 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 };
            var allShun = new List<int[]>();

            FindAllPosbShun(cds,allShun);
            foreach (var intse in allShun)
            {
                Debugcds(intse);
            }*/

            /*           CheckCardTool ddds = new CheckCardTool(PokerRuleUtil.GetCdsType);

                       var posbShunList = ddds.GetAllPossibleShun(new int[] {3, 4, 5, 6, 7, 8,  10, 11, 12, 13, 14}, 5);

                        foreach (var intse in posbShunList)
                        {
                            string str = "";
                            foreach (var i in intse)
                            {
                                str += " " + i;
                            }
                            YxDebug.LogError("shun: " + str);
                        } 
                      */




            // YxDebug.LogError(PokerRuleUtil.GetColor(PokerRuleUtil.SmallJoker));
            // CardManager.test();
            //var type = PokerRuleUtil.GetCdsType(compcds);
            /*            var cds3 = new int[] { 1, 2, 3, 4 };
                        var cdd = new int[5];

                        cds3.CopyTo(cdd,0);
                        YxDebug.LogError("数组长度："+cdd.Length);
                        for (int i = 0; i < cdd.Length; i++)
                        {
                            YxDebug.LogError(cdd[i]);
                        }*/
            //Debug.LogError(PokerRuleUtil.GetCdsType(new int[] { 3, 5, 4, 6, 7 }));

            // CardManager.test();
            /*            var cds = FindBestLian(new int[] {8,14}, 12);

                        if (cds == null)
                        {
                            YxDebug.LogError("找不到连牌的可能");
                            return;
                        }
           

                        string str = "";
                        for (int i = 0; i < cds.Length; i++)
                        {
                            str += cds[i] + " ";
                        }

                        YxDebug.LogError(str);*/


            /*            int OpHu = 1 << 3;
                            YxDebug.LogError(OpHu & (1 << 3));
                            OpHu = OpHu ^ (1 << 3);

                            YxDebug.LogError(OpHu);*/

            // YxDebug.LogError((1 << 3) ^ (1 << 3));


            /*            var ds = new Dictionary<int[], CardType>();

                        var cds1 = new int[] {1, 2, 3};
                        var cds2 = new int[] { 1, 2, 3 };
                        var cds3 = new int[] { 1, 2, 3 ,4};

                        ds.Add(cds1,CardType.C1);
                        ds.Add(cds2, CardType.C1);
                        ds.Add(cds3, CardType.C2);

                        foreach (var cds in ds.Keys.Where(cds => ds[cds] == CardType.C1))
                        {
                            foreach (var cd in cds)
                            {
                                YxDebug.LogError(cd);
                            }
                            YxDebug.LogError("--------------");
                            //ds.Remove(cds);
                        }*/

            /*            switch (CardType.C32)
                        {
                            case CardType.C31:
                            case CardType.C32:
                            case CardType.C11122234:
                            case CardType.C1112223344:
                                YxDebug.LogError("dds");
                                break;
                            default:
                                YxDebug.LogError("1111ffff");
                                break;
                        }*/


            /*            YxDebug.LogError(ds.Count);

                        YxDebug.LogError(ds[cds3]);*/

            /*            var outCds = new int[] { 0x1a, 0x2a, 0x3a, 0x1a,5,5,6,6};//
                        var LastOutCds = new int[] { 0x19, 0x29, 0x39, 0x39,6,6,7,7 };

                        YxDebug.LogError(PokerRuleUtil.JustCompareCds(outCds, LastOutCds));*/


            /*            var cdsValueDb = new int[] { 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
                        foreach (var i in cdsValueDb)
                        {
                            YxDebug.LogError(PokerRuleUtil.GetColor(i));
                        }
                    */


            // YxDebug.LogError(PokerRuleUtil.GetCdsType(new int[] { 0x15, 0x25, 0x35, 0x16, 0x26, 0x36, 0x17, 0x27, 0x37, 0x19, 0x29, 0x39 })); 
            // YxDebug.LogError(0x13);
            /*
                        CardManager cdmanger = new CardManager();
                        var list = new List<int>();
                        list.AddRange(new int[] { 0x17, 0x27, 0x37 });
                        var cardTypes = cdmanger.GetAllPosbLzTransTyps(new int[] { 0x17, 0x27, 0x37, 0x71 });
                        foreach (var cardType in cardTypes.Keys)
                        {
                            var cdarray = cardType;
                            string str = "";
                            foreach (var value in cdarray)
                            {
                                str += " " + value;
                            }

                            YxDebug.LogError(cardTypes[cardType] + ":" + str);
                        }*/

            /*            var arr2 = new List<int>();
                        arr2.AddRange(new int[] {  4,5,6,9 });

                        int count = arr2.Count;

                        int starti = 0;
                        var testList = new List<int>();
                        bool findShun = false;

                        while (starti < count)
                        {
                            int tail = count;
                            while (tail > starti+1)
                            {

                                for (int i = starti; i < tail; i++)
                                {
                                    testList.Add(arr2[i]);
                                }

                                if (isSequenceArr(testList))
                                {
                                    findShun = true;
                                    break;
                                }
                                tail--;
                                testList.Clear();
                            }
                            if (findShun)
                            {
                                YxDebug.LogError("starti:" + starti + "  tail:" + tail);
                                break;
                            }

                            starti++;
                        }*/



            /*            var list = new List<int>();
                        list.AddRange(new int[]{3,5});

                        YxDebug.LogError(isSequenceArr(list));*/

            //

            //  YxDebug.LogError(0x1a.ToString("x").Substring(1, 1));
            /*          CardManager cardManager = new CardManager();
                      cardManager.UpdateCardList(new int[] { 0x13, 0x14, 0x19, 0x29, 0x39, 0x49, 0x1a, 0x2a, 0x3a, 0x1b, 0x2b, 0x3b, });
                        var list = new List<int>();
                        list.AddRange(new int[] { 0x19, 0x29, 0x39, 0x49, 0x1a, 0x2a, 0x3a, 0x1b, 0x2b, 0x3b, });
                        YxDebug.LogError(    cardManager.MatchCard(list,
                                              null));*/
            //  var _cManager = new CardManager();
            // _cManager.Init();

            /*            var list = new List<int>();
                        int[] num = new int[] {0x46,0x17, 0x13,0x23,0x35,};
                        foreach (var i in num)
                        {
                            list.Add(i);
                        }

                        _cManager.SetCardList(list);
                        YxDebug.LogError("--------------------************************************--------------------------");
                        var list2 = new List<int>();
                        num = new int[] { 0x46, 0x17 };
                        foreach (var i in num)
                        {
                            list2.Add(i);
                        }
                        _cManager.RemoveCardList(list2);*/
            /*            int[] num = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 1000 };
                        var n = from number in num where number % 2 == 0 select number;
                        foreach (int a in n)
                        {
                            YxDebug.LogError(a.ToString());
                        }


                        ArrayList arr = new ArrayList();
                        arr.Add(new Student { Name = "张三", Age = 12, Addresss = "莆田" });
                        arr.Add(new Student { Name = "王三", Age = 12, Addresss = "福州" });
                        var data = from Student stu in arr where stu.Name == "王三" select stu;
                        foreach (Student item in data)
                        {
                            YxDebug.LogError(item.Name);
                        }*/



            /*        int[] cds = new int[] { 3, 3, 3, 4, 4,4,5,5 };
                    List<Card> cddd = new List<Card>();
                    for (int i = 0; i < cds.Length; i++)
                    {
                        cddd.Add(Card.DeskToAi(cds[i]));
                    }

                    var result =   PokerRuleUtil.getCardType(cddd);
                    YxDebug.LogError(result.getType());*/


            /*        CheckCardTool tool = new CheckCardTool(PokerRuleUtil.GetCdsType);

                    int[] cds = new int[] { 0x13, 0x23, 0x33, 0x14, 0x24,0x34,0x15,0x25 };

                    var result = tool.GetcdsWithOutCompare(new int[] {  0x15 }, cds);*/
            /*	    foreach (var cd in result)
                    {
            
                        YxDebug.LogError("cd:" +cd + "  cdType:" + (cd>>4).ToString(CultureInfo.InvariantCulture) + "  value:" + cd % 16);
                    }*/
        }
	
        // Update is called once per frame
        void Update () {
	
        }

        public void LoadScene()
        {
            SceneManager.LoadScene("GameIndex");
        }

        public void DismisRoom()
        {
            GlobalData.ServInstance.StartHandsUp(2);
        }
    }

    /*

        internal abstract class CompareCds
        {
            private readonly CardType _otCdsType;
            private readonly int[] _outCds;

            private readonly CardType _lastOtCdsType;
            private readonly int[] _lastOutCds;

            protected CompareCds(int[] outCds, int[] lastOutCds)
            {
                YxDebug.LogError("CompareCds1111");
                _otCdsType = PokerRuleUtil.GetCdsType(outCds);
                _lastOtCdsType = PokerRuleUtil.GetCdsType(lastOutCds);
            }

            protected CompareCds(int[] outCds, CardType otCdsType, int[] lastOutCds, CardType lastOtCdsType)
            {

                YxDebug.LogError("CompareCds222");
                _outCds = outCds;
                _otCdsType = otCdsType;

                _lastOutCds = lastOutCds;
                _lastOtCdsType = lastOtCdsType;
            }

            /// <summary>
            /// 比较牌
            /// </summary>
            /// <returns></returns>
            public abstract bool DoCompare();

        }

        /// <summary>
        /// 如果上次出的牌型为单牌
        /// </summary>
        internal class LastOtcdsC1ComareCds : CompareCds
        {
            public LastOtcdsC1ComareCds(int[] outCds, int[] lastOutCds)
                : base(outCds, lastOutCds)
            {
                YxDebug.LogError("dfsdaf33");
            }

            public LastOtcdsC1ComareCds(int[] outCds, CardType otCdsType, int[] lastOutCds, CardType lastOtCdsType)
                : base(outCds, otCdsType, lastOutCds, lastOtCdsType)
            {
         
            }

            public override bool DoCompare()
            {
                return false;
            }
        }
    */



    public class Student
     {
         public string Name { get; set; }
         public int Age { get; set; }
         public string Addresss { get; set; }
 
     }
}
