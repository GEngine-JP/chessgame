using System;
using System.Collections.Generic;
using Assets.Scripts.Game.Shuihuzhuan.Scripts;
using Sfs2X;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Utils;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.Shuihuzhuan
{
    public class GameServer : RemoteController
    {
        public static GameServer Instance;
        private SmartFox _smart;
        public override void Init(Dictionary<string, Action<ISFSObject>> callBackDic)
        {
            App.GameData.GStatus = GameStatus.PlayAndConfine;
            Instance = this;
//            GameStateUiControl.instance.ChangeToWait();
        }
        /// <summary>
        /// 获取游戏信息
        /// </summary>
        /// <param name="gameInfo"></param>
        protected override void OnGetGameInfo(ISFSObject gameInfo)
        {
            App.GetGameData<GlobalData>().show = gameInfo.GetBool("show");
            App.GetGameData<GlobalData>().BetLineNum = gameInfo.GetInt("line");
            App.GetGameData<GlobalData>().iMainMoney = (int)gameInfo.GetSFSObject(RequestKey.KeyUser).GetLong("ttgold");
            App.GetGameData<GlobalData>().MainMoney = (int)gameInfo.GetSFSObject(RequestKey.KeyUser).GetLong("ttgold");
            App.GetGameData<GlobalData>().iXiazhushangxian = gameInfo.GetInt("ante");
            App.GetGameData<GlobalData>().BetBaseNum = gameInfo.GetInt("ante");
            App.GetGameData<GlobalData>().BetNum = App.GetGameData<GlobalData>().BetLineNum* App.GetGameData<GlobalData>().BetBaseNum;
        }
        /// <summary>
        /// 加入的数据
        /// </summary>
        /// <param name="data"></param>
        protected override void OnGetRejoinData(ISFSObject data)
        {
            OnGetGameInfo(data);
        }
        /// <summary>
        /// 交互信息
        /// </summary>
        /// <param name="data"></param>
        protected override void OnServerResponse(ISFSObject responseData)
        {
            if (responseData.GetInt("type")==1 )//运行水浒传
            {
                Array.Copy(responseData.GetIntArray("lines"), 0, App.GetGameData<GlobalData>().iLineImgid, 0, 18);
                Array.Copy(responseData.GetIntArray("images"), 0, App.GetGameData<GlobalData>().iTypeImgid, 0, 15);
                if (responseData.GetSFSObject(RequestKey.KeyUser).GetInt("win") > 0)
                    {
                        App.GetGameData<GlobalData>().iWinMoney = responseData.GetSFSObject(RequestKey.KeyUser).GetInt("win");
                        App.GetGameData<GlobalData>().iMainMoney = (int)responseData.GetSFSObject(RequestKey.KeyUser).GetLong("ttgold");
                        Scripts.Game.instance .YingShuaxinFun();
                    }
                    else
                    {
                        App.GetGameData<GlobalData>().iMainMoney = (int)responseData.GetSFSObject(RequestKey.KeyUser).GetLong("ttgold");
                        App.GetGameData<GlobalData>().iWinMoney = 0;
                        Scripts.Game.instance.LostShuaxinFun();
                    }
             ShowResult.instance.SetResultSprite();
             App.GetGameData<GlobalData>().IsAotozhuangtai = false;//按钮的状态
             TurnControl.instance.GameResultFun();
            }
            if (responseData.GetInt("type") == 2)
            {
                App.GetGameData<GlobalData>().iDice1 = 0;
                App.GetGameData<GlobalData>().iDice2 = 0;
                App.GetGameData<GlobalData>().iDice1 = responseData.GetInt("dice1");
                App.GetGameData<GlobalData>().iDice2 = responseData.GetInt("dice2");
                App.GetGameData<GlobalData>().iWinMoney = 0;
                App.GetGameData<GlobalData>().iWinMoney = responseData.GetSFSObject(RequestKey.KeyUser).GetInt("bwin") ;
                if (responseData.GetSFSObject(RequestKey.KeyUser).GetInt("bwin") > 0) //赢了
                {
                    App.GetGameData<GlobalData>().iMainMoney = (int)
                        responseData.GetSFSObject(RequestKey.KeyUser).GetLong("ttgold");
                }
                else //输了 
                {
                    App.GetGameData<GlobalData>().MainMoney =(int)
                        responseData.GetSFSObject(RequestKey.KeyUser).GetLong("ttgold");
                }
                BigOrSmallControl.Instance.GameBiBeiResultFun();
                
                YxDebug.LogError("服务器=》------大小和----------");
            }
            if (responseData.GetInt("mali")>0)//判断是否有马力
            {
                App.GetGameData<GlobalData>().isMary = true;
            }
            if (responseData.GetInt("type") == 3)//进入玛丽
            {
                Array.Copy(responseData.GetIntArray("items"), 0, App.GetGameData<GlobalData>().iMaliImage, 0, 4);
                int i = 0;
                i = App.GetGameData<GlobalData>().MaliWinMony;
                App.GetGameData<GlobalData>().MaliWinMony = responseData.GetSFSObject(RequestKey.KeyUser)
                                                                            .GetInt("mali");
                //                YxDebug.LogError(App.GetGameData<GlobalData>().MaliWinMony);
                App.GetGameData<GlobalData>().MaliWinMony = App.GetGameData<GlobalData>().MaliWinMony + i;//玛丽所赢
                App.GetGameData<GlobalData>().iMaliZhuanImage = responseData.GetInt("idx");//转的图片
                App.GetGameData<GlobalData>().iMaliGames = responseData.GetInt("mali");//马力次数
                if (responseData.GetSFSObject(RequestKey.KeyUser).GetInt("gold") > 0)
                {
                         App.GetGameData<GlobalData>().iMainMoney =
                        (int) responseData.GetSFSObject(RequestKey.KeyUser).GetLong("ttgold");
                }
                LittleMaryControl.Instance.MaryResultFun();
                //                YxDebug.LogError(App.GetGameData<GlobalData>().MaliWinMony);
                YxDebug.LogError("服务器=》-------玛丽----------");
            }

        }
        /// <summary>
        /// 开始服务器发送数据
        /// </summary>
        /// <param name="bettgold">压注的钱</param>
        public void MyGameStart(int betting)
        {
           
            SFSObject sfsObject = new SFSObject();
            sfsObject.PutInt("type", 1);
            sfsObject.PutInt("gold", betting);
           SendRequest(new ExtensionRequest(GameKey + RequestCmd.Request, sfsObject));
        }

        /// <summary>
        /// 大小和服务器发送数据
        /// </summary>
        /// <param name="bettgold">压注的钱</param>
        public void MyDaXiaoHe(int betting,int Dxh)
        {
            YxDebug.LogError(betting);
            SFSObject sfsObject = new SFSObject();
            sfsObject.PutInt("type", 2);
            sfsObject.PutInt("gold", betting);
            sfsObject.PutInt("dx", Dxh);
            SendRequest(new ExtensionRequest(GameKey + RequestCmd.Request, sfsObject));
        }
        /// <summary>
        /// 玛丽向服务器发送数据
        /// </summary>
        public void MaLiFun()
        {   
            SFSObject sfsObject = new SFSObject();
            sfsObject.PutInt("type", 3);
            SendRequest(new ExtensionRequest(GameKey + RequestCmd.Request, sfsObject));
            //            YxDebug.LogError("客户端=》-------玛丽----------");
        }
        public void OnQuitGame()
        {
            App.QuitGameWithMsgBox();
        }
        protected override void OnUserOut(int seat)
        {
        }

        protected override void OnUserIdle(int serverSeat)
        {
        }

        protected override void OnUserOnLine(int serverSeat)
        {
        }
    }
      
}