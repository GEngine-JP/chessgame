using Assets.Scripts.Game.lswc.Item;
using Assets.Scripts.Game.lswc.Manager;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common.Utils;
using YxFramwork.ConstDefine;
using System.Collections.Generic;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.lswc.Data
{
    public class GlobalData:YxGameData
    {

        public delegate void LSDataEvent();

        public LSDataEvent OnBetNumChange;

        /// <summary>
        /// 金币数量
        /// </summary>
        private long _totalGold;

        public long TotalGold
        {
            get { return _totalGold; }
        }

        /// <summary>
        /// 总押注
        /// </summary>
        private long _totalBets = 0;

        public long TotalBets
        {
            get { return _totalBets; }
        }

        /// <summary>
        /// 显示剩余时间(在下注阶段为显示时间，等待阶段为等待总时间)
        /// </summary>
        public long ShowTime = 0;

        /// <summary>
        /// 等待时间
        /// </summary>
        public long WaitTime = 0;

        /// <summary>
        /// 历史纪录
        /// </summary>
        public LSResult[] HistoryResults;

        /// <summary>
        /// 当前历史的索引（本地存储的历史纪录中的当前的存储位置）
        /// </summary>
        public int History_Index;

        /// <summary>
        /// 当前游戏的状态
        /// </summary>
        public GameState GlobalGameStatu = GameState.WaitState;

        /// <summary>
        /// 本局的下注情况
        /// </summary>
        public int[] Bets;

        /// <summary>
        /// 上局下注的数量
        /// </summary>
        public int[] PerBets;

        /// <summary>
        /// 下注的挡位
        /// </summary>
        public int Ante = 0;

        /// <summary>
        /// 下注挡位，目前有八个挡位
        /// </summary>
        private int[] BetAntes = new int[] { 100, 200, 500, 1000, 5000, 10000, 100000, 1000000 };

        /// <summary>
        /// 下注挡位15个区域，与索引对应
        /// </summary>
        public int[] PeiLvs;

        /// <summary>
        /// 动物顺序
        /// </summary>
        public LSAnimalType[] Animals;

        /// <summary>
        /// 动物原始位置5,1,0,3,2,1,4,1,3,2,1,0,7,0,2,3,0,3,6,2,1,0,3,2
        /// </summary>
        public int[] AnimalStartPos;

        /// <summary>
        /// 指针的指向
        /// </summary>
        public int[] TurnTablePoints;

        /// <summary>
        /// 颜色顺序
        /// </summary>
        public LSColorType[] Colors;

        /// <summary>
        /// 发送消息后是否接收到了结果
        /// </summary>
        public bool ISGetResult;

        /// <summary>
        /// 追后一局的游戏结果，在返回游戏结果时处理
        /// </summary>
        public LS_Detail_Result LastResult;

        /// <summary>
        /// 动物随机种子
        /// </summary>
        public int AnimalRandomSeed;

        /// <summary>
        /// 游戏开始时间
        /// </summary>
        public long StartTime;

        /// <summary>
        /// 当前的时间
        /// </summary>
        public long NowTime;

        /// <summary>
        /// CD时间
        /// </summary>
        public int CDTime;

        /// <summary>
        /// 倍数
        /// </summary>
        public int Mulpitle;

        /// <summary>
        /// 最后一次结果中的指针索引
        /// </summary>
        public int LastTurnTableIndex;

        /// <summary>
        /// 是否进入下一局
        /// </summary>
        public bool ReadyToNext;

        /// <summary>
        /// 数据是否初始化
        /// </summary>
        public bool GameInfoInit=false;
        /// <summary>
        /// 初始化游戏信息
        /// </summary>
        /// <param name="data"></param>
        public void InitGameInfo(ISFSObject data)
        {
            ISFSObject user = data.GetSFSObject(RequestKey.KeyUser);

            //金币
            _totalGold = user.GetLong(RequestKey.KeyTotalGold);

            //总下注
            _totalBets = 0;

            HistoryResults = new LSResult[LSConstant.History_Lenth];
            //历史索引
            History_Index  = data.GetInt(LSConstant.KeyHistoryIndex); 
            //Debug.LogError("历史索引是"+History_Index);

            //开始时间
            StartTime = data.GetLong(LSConstant.KeyGameStartTime);

            //现在时间
            NowTime = data.GetLong(RequestCmd.GetServerTime);

            //游戏CD
            CDTime = data.GetInt(LSConstant.KeyCDTime);
            
            //获得实际剩余时间
            GetShowTime();

            //ShowTime = 29;

            //当前的游戏状态
            GlobalGameStatu = (GameState)data.GetInt(LSConstant.KeyGameStatus);

            //GlobalGameStatu = GameState.Empyt;

            //历史记录
            AnalysisHistorys(data.GetIntArray(LSConstant.KeyHistoryResult));

            //动物位置初始化（只在游戏开始时初始化一次）
            AnimalStartPos = new int[LSConstant.Num_AnimalItemNumber];
            AnimalStartPos = data.GetIntArray(LSConstant.KeyAnimalsPosition);
            Animals = new LSAnimalType[LSConstant.Num_AnimalItemNumber];
            AnimalRandomSeed = data.GetInt(LSConstant.KeyRound);
            GetAnimalRandomNumber();

            //颜色区域数据初始化
            Colors = new LSColorType[LSConstant.Num_ColorItemNumber];
            SetColorsPosition(data.GetIntArray(LSConstant.KeyColorPosition));

            //倍率初始化
            PeiLvs = new int[LSConstant.Num_BetNumber];
            SetPeilvs(data.GetIntArray(LSConstant.KeyRates));

            //下注区域初始化
            Bets = new int[LSConstant.Num_BetNumber];
            PerBets = new int[LSConstant.Num_BetNumber];

            //上局倍数
            Mulpitle = data.GetInt(LSConstant.KeyMultiple);

            //最后结果初始化
            LastResult = new LS_Detail_Result();
            LastResult.ShowResults=new List<int>();

            LSColorItemControl.Instance.InitItems();
            LSAnimalItemControl.Instance.InitItems();
            LSUIManager.Instance.InitUImanager();

            SetDetialResult(LastTurnTableIndex, ShowTime,Mulpitle, 0);

            GameInfoInit = true;
        }

        /// <summary>
        /// 接收到新的游戏结果后的处理
        /// </summary>
        /// <param name="data"></param>
        public void InitNewResult(ISFSObject data)
        {

            System.Array.Copy(Bets, PerBets, Bets.Length);

            //最后一局结果
            int res = data.GetInt(LSConstant.KeyRes);
            AddHistory(res);
            //时间
            ShowTime = data.GetInt(LSConstant.KeyCDTime);

            //倍数
            Mulpitle = data.GetInt(LSConstant.KeyMultiple);


            ISFSObject user = data.GetSFSObject(RequestKey.KeyUser);

            
            int winGold = user.GetInt(RequestKey.KeyGold);

            //最后的指针位置
            LastTurnTableIndex = data.GetInt(LSConstant.KeyPointIndex);

            SetDetialResult(LastTurnTableIndex, ShowTime, Mulpitle,winGold);

            if (user.IsNull("win"))
            {
                YxDebug.Log("本局未下注，所以赢得金币为0");    
            }
            else
            {
                YxDebug.Log("服务器返回下注获得金币为（不包括下注金币）："+user.GetInt("win")); 
            }

        }

        /// <summary>
        /// 新的一局
        /// </summary>
        /// <param name="data"></param>
        public void OnNewPage(ISFSObject data)
        {
            YxDebug.Log("刷新数据");
            SetColorsPosition(data.GetIntArray(LSConstant.KeyColorPosition));
            SetPeilvs(data.GetIntArray(LSConstant.KeyRates));
            ShowTime = data.GetInt(LSConstant.KeyCDTime);
            ClearBets();
            ReadyToNext = true;
        }

        /// <summary>
        /// 倍率数据初始化
        /// </summary>
        /// <param name="arr"></param>
        public void SetPeilvs(int[] arr)
        {
            //Debug.LogError("获得赔率长度是："+arr.Length);
            System.Array.Copy(arr, PeiLvs, arr.Length);
        }

        /// <summary>
        /// 初始移动后，根据指针索引，获得对应的动物索引
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public int GetRealAnimalIndex(int position)
        {
            return (position + AnimalRandomSeed) % LSConstant.Num_AnimalItemNumber;
        }

        /// <summary>
        /// 动物移动位置伪随机
        /// </summary>
        public void GetAnimalRandomNumber()
        {
            AnimalRandomSeed = (AnimalRandomSeed + 2011) * 15 % 24;
            YxDebug.Log("本次的随机位置是：" + AnimalRandomSeed);
            int changeIndex;
            if (AnimalStartPos == null)
            {
                YxDebug.LogError("动物位置未初始化");
                return;
            }
            //Debug.LogError("动物顺序是：");
            for (int i = 0; i < AnimalStartPos.Length; i++)
            {
                changeIndex = (AnimalRandomSeed + i) % 24;
                Animals[i] = (LSAnimalType)AnimalStartPos[changeIndex];
                //Debug.LogError(Animals[i]);
            }
        }
        public void SetColorsPosition(int[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                Colors[i] = (LSColorType) arr[i];
            }
        }

        /// <summary>
        /// 获得当前显示的剩余时间
        /// </summary>
        public long GetShowTime()
        {
            ShowTime = StartTime + CDTime - NowTime;
            return ShowTime;
        }

        /// <summary>
        /// 下注
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool AddBet(int index)
        {
            bool addState = false;
            if (GlobalGameStatu == GameState.BetState)
            {
                int addNum = BetAntes[Ante];
                if (_totalGold - _totalBets - addNum > 0)
                {
                    _totalBets += addNum;
                    Bets[index] += addNum;
                    if (OnBetNumChange != null)
                    {
                        OnBetNumChange();
                    }
                    addState = true;
                }
            }
            return addState;
        }


        #region 界面的公用部分
        /// <summary>
        /// 获得界面上方庄和闲的图片名称
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetBankerOrSpriteName(LSBankerType type)
        {
            switch (type)
            {
                case LSBankerType.BANKER:
                    return LSConstant.Banker_Zhuang;
                case LSBankerType.EQUAL:
                    return LSConstant.Banker_He;
                case LSBankerType.PLAYER:
                    return LSConstant.Banker_Xian;
                default:
                    YxDebug.LogError("Type is not exist, name is: " + type.ToString());
                    return null;
            }
        }

        /// <summary>
        /// 设置庄和闲
        /// </summary>
        /// <returns></returns>
        public Sprite SetLastBanker()
        {
            LSResult finalResult = GetHistoryResult(LSConstant.History_Lenth - 1);
            //Debug.LogError("最后的结果是: "+finalResult.Banker+" "+finalResult.ResultType+" "+finalResult.BetIndex);
            return LSResourseManager.Instance.GetSprite(GetBankerOrSpriteName(finalResult.Banker));
        }
        #endregion

        #region 历史相关
        /// <summary>
        /// 解析全部的历史记录
        /// </summary>
        /// <param name="history"></param>
        private void AnalysisHistorys(int[] history)
        {
            //Debug.LogError("最新的历史记录的值是: "+history[History_Index]);
            //int result = history[History_Index];
            //int animalIndex = result & 0xf;
            //result = result >> 4;
            //int rewardType = result & 0xf;
            //result = result >> 4;
            //int banker = result & 0xf;
            //Debug.Log("结果是： "+(LSBankerType)banker+" "+(LSRewardType)rewardType+" "+(LSAnimalType)animalIndex+" "+(LSColorType)(history[History_Index]%3));
            int lenth = history.Length;
            for (int i = 0; i<lenth; i++)
            {
                HistoryResults[i]=AnalysisResult(history[i]);
            }
        }

        /// <summary>
        /// 新增一条历史记录
        /// </summary>
        /// <param name="res"></param>
        private void AddHistory(LSResult res)
        {
            HistoryResults[History_Index] = res;
            History_Index++;
            if (History_Index == LSConstant.History_Lenth)
            {
                History_Index = 0;
            }  
        }

        /// <summary>
        /// 新增一条历史记录
        /// </summary>
        /// <param name="result"></param>
        public void AddHistory(int result)
        {
            AddHistory(AnalysisResult(result));
        }

        /// <summary>
        /// 根据索引获得当前的历史记录
        /// </summary>
        /// <param name="index">0~9，9为最新的历史记录</param>
        /// <returns></returns>
        public LSResult GetHistoryResult(int index)
        {
            int realIndex = (History_Index + index) % LSConstant.History_Lenth;
            return HistoryResults[realIndex];
        }

        /// <summary>
        /// 解析一条历史数据
        /// </summary>
        /// <param name="result">服务器返回的历史数据</param>
        /// <returns></returns>
        private LSResult AnalysisResult(int result)
        {
            //Debug.LogError("历史结果是:" + result);
            int animalIndex = result & 0xf;
            result = result >> 4;
            int rewardType = result & 0xf;
            result = result >> 4;
            int banker = result & 0xf;
            //Debug.LogError("解析出来的结果是(从左到右)：" + (LSBankerType)banker + "  " + (LSRewardType)rewardType + " " + (LSAnimalSpriteType)animalIndex);
            LSResult res = new LSResult(animalIndex, rewardType, banker);
            return res;
        }
        /// <summary>
        /// 查找满足条件的第一个颜色区域的索引，用于确定
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private int FindColorIndex(LSColorType color)
        {
            for (int i = 0; i < Colors.Length; i++)
            {
                if (Colors[i] == color)
                {
                    return i;
                }
            }
            YxDebug.LogError("Such type is not exist " + color.ToString());
            return -1;
        }

        private int FindAnimalIndex(LSAnimalType type)
        {
            for (int i = 0; i < Animals.Length; i++)
            {
                if (Animals[i] == type)
                {
                    return i;
                }
            }
            YxDebug.LogError("Such type is not exist " + type.ToString());
            return -1;
        }

        /// <summary>
        /// 获得最新的历史记录的详细信息
        /// </summary>
        /// <returns></returns>
        public LS_Detail_Result SetDetialResult(int TurnTablePosition, float Time, int multiple, int addGold)
        {
            //Debug.LogError("本局结果是");
            LSResult res = GetHistoryResult(LSConstant.History_Lenth - 1);
            LSColorType resultColor = Colors[TurnTablePosition];
            LSAnimalType resultAnimal = (LSAnimalType)AnimalStartPos[TurnTablePosition];
            LastResult.Banker = res.Banker;
            LastResult.TurnTablePosition = TurnTablePosition;
            LastResult.BetIndex = res.BetIndex;
            LastResult.Reward = res.ResultType;
            //LastResult.Reward = LSRewardType.LIGHTING;
            LastResult.Cor = (LSColorType)(res.BetIndex % 3);
            LastResult.Ani = (LSAnimalType)(res.BetIndex / 3);
            YxDebug.Log("通过历史数据解析的结果为：动物 " + LastResult.Ani + " 颜色是:" + LastResult.Cor);
            LastResult.Cor = resultColor;
            LastResult.Ani = resultAnimal;
            LastResult.Multiple = multiple;
            //LastResult.Multiple = 3;
            _totalGold += addGold;
            LastResult.WinBets = addGold;
            YxDebug.Log("结果类型：" + LastResult.Reward);
            YxDebug.Log("指针：" + LastResult.TurnTablePosition);
            YxDebug.Log("动物：" + LastResult.Ani);
            YxDebug.Log("颜色: " + LastResult.Cor);
            YxDebug.Log("庄和闲： " + LastResult.Banker);
            YxDebug.Log("增加金币" + addGold);
            LastResult.ToColor = LSColorItemControl.Instance.GetCheckColorItem(LastResult.TurnTablePosition) as LSColorItem;
            LastResult.ToAngle = LastResult.ToColor.transform.localEulerAngles;
            LastResult.ToAnimal = LSAnimalItemControl.Instance.GetCheckColorItem((LastResult.TurnTablePosition - AnimalRandomSeed + LSConstant.Num_AnimalItemNumber) % 24) as LSAnimalItem;
            LastResult.lastResultVoice=GetResultVoice();
            return LastResult;
        }

        /// <summary>
        /// 获得结果音效
        /// </summary>
        /// <returns></returns>
        public string GetResultVoice()
        {
            LastResult.ShowResults.Clear();
            string playVoice = "";
            switch (LastResult.Reward)
            {
                case LSRewardType.NORMAL:
                case LSRewardType.LIGHTING:
                case LSRewardType.SENDLAMP:
                    LastResult.ShowResults.Add(LastResult.BetIndex);
                    playVoice = NormalVoice();
                    break;
                case LSRewardType.BIG_THREE:
                    //LSBigThreeReward.Instance.SetCurrentAnimal(LastResult.Ani);

                    playVoice = BigThreeVoice();
                    break;
                case LSRewardType.BIG_FOUR:
                    playVoice = BigFourVoice();
                    break;
                case LSRewardType.HANDSEL:
                    playVoice = LSConstant.HandselVoice;
                    LastResult.ShowResults.Add(LastResult.BetIndex);
                    break;
            }
            //庄和闲
            LastResult.ShowResults.Add((int)(LastResult.Banker)+12);
            return playVoice;
        }

        /// <summary>
        /// 常规声音
        /// </summary>
        /// <returns></returns>
        private string NormalVoice()
        {
            string voiceName = "";
            switch (LastResult.Cor)
            {
                case LSColorType.GREEN:
                    voiceName += "g_";
                    break;
                case LSColorType.RED:
                    voiceName += "r_";
                    break;
                case LSColorType.YELLOW:
                    voiceName += "y_";
                    break;
            }
            switch (LastResult.Ani)
            {
                case LSAnimalType.TZ:
                case LSAnimalType.GOLD_TZ:
                    voiceName += "rabbit";
                    break;
                case LSAnimalType.HZ:
                case LSAnimalType.GOLD_HZ:
                    voiceName += "monkey";
                    break;
                case LSAnimalType.XM:
                case LSAnimalType.GOLD_XM:
                    voiceName += "panda";
                    break;
                case LSAnimalType.SZ:
                case LSAnimalType.GOLD_SZ:
                    voiceName += "lion";
                    break;
            }
            return voiceName;
        }

        /// <summary>
        /// 大三元声音
        /// </summary>
        /// <returns></returns>
        private string BigThreeVoice()
        {
            string voiceName = "";
            int animalType=0;
            switch (LastResult.Ani)
            {
                case LSAnimalType.TZ:
                case LSAnimalType.GOLD_TZ:
                    voiceName = "ani_rabbit";
                    animalType = 0;
                    break;
                case LSAnimalType.HZ:
                case LSAnimalType.GOLD_HZ:
                    voiceName = "ani_monkey";
                    animalType = 1;
                    break;
                case LSAnimalType.XM:
                case LSAnimalType.GOLD_XM:
                    voiceName = "ani_panda";
                    animalType = 2;
                    break;
                case LSAnimalType.SZ:
                case LSAnimalType.GOLD_SZ:
                    voiceName = "ani_lion";
                    animalType = 3;
                    break;
            }
            for (int i = 0; i < 3; i++)
            {
               LastResult.ShowResults.Add(3*animalType+i);
            }
            return voiceName;
        }

        /// <summary>
        /// 大四喜声音
        /// </summary>
        /// <returns></returns>
        private string BigFourVoice()
        {
            string voiceName = "";

            switch (LastResult.Cor)
            {
                case LSColorType.GREEN:
                    voiceName = "color_g";
                    break;
                case LSColorType.RED:
                    voiceName = "color_r";
                    break;
                case LSColorType.YELLOW:
                    voiceName = "color_y";
                    break;
            }
            for (int i = 0; i < 4; i++)
            {
                LastResult.ShowResults.Add(3*i+(int)(LastResult.Cor));
            }
            
            return voiceName;
        }

        /// <summary>
        /// 获得历史记录中动物的图片
        /// </summary>
        /// <param name="spriteIndex"></param>
        /// <returns></returns>
        public Sprite SetHistoryAnimal(LSResult res)
        {
            string name = "";

            switch(res.ResultType)
            {
                case LSRewardType.BIG_THREE:
                    switch(res.BetIndex/3)
                    {
                        case (int)LSAnimalType.TZ:
                        case (int)LSAnimalType.GOLD_TZ:
                            name = "tz_c";
                            break;
                        case (int)LSAnimalType.HZ:
                        case (int)LSAnimalType.GOLD_HZ:
                            name = "hz_c";
                            break;
                        case (int)LSAnimalType.XM:
                        case (int)LSAnimalType.GOLD_XM:
                            name = "xm_c";
                            break;
                        case (int)LSAnimalType.SZ:
                        case (int)LSAnimalType.GOLD_SZ:
                            name = "sz_c";
                            break;
                    }
                    break;
                case LSRewardType.BIG_FOUR:
                    switch(res.BetIndex%3)
                    {
                        case (int)LSColorType.RED:
                            name = "all_r";
                            break;
                        case (int)LSColorType.GREEN:
                            name = "all_g";
                            break;
                        case (int)LSColorType.YELLOW:
                            name = "all_y";
                            break;
                    }
                    break;
                case LSRewardType.LIGHTING:
                case LSRewardType.HANDSEL:
                case LSRewardType.NORMAL:
                    name = ((LSAnimalSpriteType) res.BetIndex).ToString();
                           break;
                case LSRewardType.SENDLAMP:
                           name = "point";
                    break;
            }
            return LSResourseManager.Instance.GetSprite(name);
        }

        /// <summary>
        /// 设置历史记录中的庄和闲
        /// </summary>
        /// <returns></returns>
        public Sprite SetHistoryBanker(LSBankerType type)
        {
            string spriteName = null;
            switch (type)
            {
                case LSBankerType.BANKER:
                    spriteName = LSConstant.History_Banker_Zhuang;
                    break;
                case LSBankerType.EQUAL:
                    spriteName = LSConstant.History_Banker_He;
                    break;
                case LSBankerType.PLAYER:
                    spriteName = LSConstant.History_Banker_Xian;
                    break;
                default:
                    YxDebug.LogError("Such type is not exist :" + type.ToString());
                    break;
            }
            return GetSprite(spriteName);
        }

        /// <summary>
        /// 获得sprite
        /// </summary>
        /// <param name="spriteName"></param>
        /// <returns></returns>
        public Sprite GetSprite(string spriteName)
        {
           return LSResourseManager.Instance.GetSprite(spriteName);
        }

        #endregion

        #region 下注相关
        /// <summary>
        /// 全下
        /// </summary>
        public bool BetAll()
        {
            bool isBet = false;
            for (int i = 0; i < Bets.Length; i++)
            {
                isBet = AddBet(i);
            }
            return isBet;
        }

        /// <summary>
        /// 续压
        /// </summary>
        public bool BetAgain()
        {
            if (PerBets.Equals(Bets))
            {
                return false;
            }
            bool addState = true;
            int totalAdd = 0;
            for (int i = 0; i < PerBets.Length; i++)
            {
                int addNum = PerBets[i];
                if (_totalGold - _totalBets - addNum > 0)
                {
                    Bets[i] = addNum;
                    totalAdd += addNum;
                    if (OnBetNumChange != null)
                    {
                        OnBetNumChange();
                    }       
                }
                else
                {
                    addState = false;
                    _totalBets = totalAdd;
                    return addState;
                }
            }
            _totalBets = totalAdd;
            return addState;
        }

        /// <summary>
        /// 清空下注
        /// </summary>
        public void ClearBets()
        {
            _totalBets = 0;
            System.Array.Clear(Bets, 0, Bets.Length);
        }

        /// <summary>
        /// 更换下注倍率
        /// </summary>
        public void ChangeAnte()
        {
            Ante++;
            if (Ante >= BetAntes.Length)
            {
                Ante = 0;
            }
        }

        /// <summary>
        /// 获得当前的注值
        /// </summary>
        /// <returns></returns>
        public string GetNowAnte()
        {
            var num = BetAntes[Ante];

            string str;

            if (num >= LSConstant.Num_OneHanred_Million)
            {
                str = (num / LSConstant.Num_OneHanred_Million).ToString() + "亿";
            }
            else if (num >= LSConstant.Num_TenThousand)
            {
                str = ((num / LSConstant.Num_TenThousand)).ToString() + "万";
            }
            else
            {
                str = num.ToString();
            }
            return str;
        }

        /// <summary>
        /// 获得当前注值的图片
        /// </summary>
        /// <returns></returns>
        public Sprite GetNowAnteSprite()
        {
            string spriteName = ((LSAnteType)Ante).ToString();

            return LSResourseManager.Instance.GetSprite(spriteName);
        }

        public Material GetColorMaterial(LSColorType type)
        {
            string materialName;
            switch (type)
            {
                case LSColorType.DEFAULT:
                    materialName = LSConstant.ColorItem_Default;
                    break;
                case LSColorType.GREEN:
                    materialName = LSConstant.ColorItem_Green;
                    break;
                case LSColorType.RED:
                    materialName = LSConstant.ColorItem_Red;
                    break;
                case LSColorType.YELLOW:
                    materialName = LSConstant.ColorItem_Yellow;
                    break;
                default:
                    YxDebug.LogError("Color is not exist, name is " + type.ToString());
                    materialName = null;
                    break;
            }
            return LSResourseManager.Instance.GetMaterial(materialName);
        }

        #endregion

        /// <summary>
        /// 获得一个1000到30000之间的数，用于显示红利
        /// </summary>
        /// <returns></returns>
        public int GetRandomNum()
        {
            return Random.Range(LSConstant.Num_Bonus_FlLimited, LSConstant.Num_Bonus_UpLimited);
        }

        private int _bankerRandom = 0;
        /// <summary>
        /// 随机显示庄和闲
        /// </summary>
        /// <returns></returns>
        public Sprite GetRandomBanker()
        {
            _bankerRandom++;
            return LSResourseManager.Instance.GetSprite(GetBankerOrSpriteName((LSBankerType)(_bankerRandom % 3)));
        }


    }
}
