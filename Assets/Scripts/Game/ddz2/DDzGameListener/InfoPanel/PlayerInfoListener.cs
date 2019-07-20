using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.Game.ddz2.DDz2Common;
using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Assets.Scripts.Game.ddz2.InheritCommon;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.ddz2.DDzGameListener.InfoPanel
{
    public class PlayerInfoListener : ServEvtListener
    {
        public const string SpkBuJiao = "bujiao";
        public const string Spk1Fen = "1fen";
        public const string Spk2Fen = "2fen";
        public const string Spk3Fen = "3fen";
        public const string SpkBuChu = "buchu";

        public const string SpkJiabei = "jiabei";
        public const string SpkBuJiabei = "bujiabei";
        public const string SpkDoubleIcon = "DoubleIcon";

        protected override void OnAwake()
        {
            Ddz2RemoteServer.AddOnGameInfoEvt(SetUserInfo);
            Ddz2RemoteServer.AddOnGetRejoinDataEvt(OnRejoinGame);
            Ddz2RemoteServer.AddOnUserReadyEvt(OnUserReady);
            Ddz2RemoteServer.AddOnUserJoinRoomEvt(OnUserJoinRoom);
            Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypeGrab, OnTypeGrab);
            Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypeGrabSpeaker, OnTypeGrabSpeaker);


            Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypeAllocate, OnAlloCateCds);
            Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypeFirstOut, TypeFirstOut);
            Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypeGameOver, OnTypeGameOver);
            Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypeDoubleOver, OnDoubleOver);
        }

        void Start()
        {
            App.GetGameData<GlobalData>().OnUserScoreChanged = OnUserScoreChanged;
        }

        //用户信息缓存
        protected ISFSObject UserDataTemp;

        //静态信息-----------------------start
        [SerializeField]
        protected UITexture HeadTexture;
        [SerializeField]
        protected UILabel IdLabel;
        [SerializeField]
        protected UILabel NameLabel;
        [SerializeField] 
        protected UILabel ScoreLabel;
        [SerializeField]
        protected UserInfoDetail InfoDetial;
        //------------------------------end


        //动态信息-----------------------------start
        /// <summary>
        /// 玩家准备了
        /// </summary>
        [SerializeField] 
        protected UISprite GameReadySp;
        /// <summary>
        /// 显示玩家行动时要说的话
        /// </summary>
        [SerializeField]
        protected UISprite ShowSpeakSp;

        /// <summary>
        /// 显示玩家是否加倍
        /// </summary>
        [SerializeField]
        protected GameObject ShowJiaBeiSp;

        /// <summary>
        /// 地主身份sprite的 GameObject
        /// </summary>
        [SerializeField]
        protected GameObject DizhuSp;
        //--------------------------------end

        /// <summary>
        /// 标记地主座位
        /// </summary>
        protected int LandSeat = -1;

        /// <summary>
        /// 根据缓存的信息刷新用户信息ui
        /// </summary>
        public override void RefreshUiInfo()
        {
            if (UserDataTemp != null)
            {
                if (UserDataTemp.ContainsKey(RequestKey.KeyName))
                    NameLabel.text = UserDataTemp.GetUtfString(RequestKey.KeyName);

                if (UserDataTemp.ContainsKey(RequestKey.KeyId))
                    IdLabel.text = UserDataTemp.GetInt(RequestKey.KeyId).ToString(CultureInfo.InvariantCulture);


                short sex = 0;
                if (UserDataTemp.ContainsKey(NewRequestKey.KeySex)) sex = UserDataTemp.GetShort(NewRequestKey.KeySex);

                if (UserDataTemp.ContainsKey(NewRequestKey.KeyAvatar))
                {
                    DDzUtil.LoadRealHeadIcon(UserDataTemp.GetUtfString(NewRequestKey.KeyAvatar), sex,HeadTexture);
                }
                else
                {
                    DDzUtil.LoadDefaultHeadIcon(sex,HeadTexture);
                }

                if (UserDataTemp.ContainsKey(NewRequestKey.KeyTtGold))
                {
                    ScoreLabel.text = YxUtiles.GetShowNumber(UserDataTemp.GetLong(NewRequestKey.KeyTtGold)).ToString(CultureInfo.InvariantCulture);
                }


                if (!App.GetGameData<GlobalData>().IsStartGame
                    && UserDataTemp.ContainsKey(RequestKey.KeyState))
                    GameReadySp.gameObject.SetActive(UserDataTemp.GetBool(RequestKey.KeyState));
                else
                    GameReadySp.gameObject.SetActive(false);
            }

        }

        /// <summary>
        /// 获得这个玩家的手牌数量,获取不到时返回-1；
        /// </summary>
        public int ThisPlayerHdCdsNum
        {
            get { 
                if (UserDataTemp == null || !UserDataTemp.ContainsKey(NewRequestKey.KeyCardNum)) return -1;
                return UserDataTemp.GetInt(NewRequestKey.KeyCardNum);
            }
        }

        protected virtual void OnRejoinGame(object sender ,DdzbaseEventArgs args)
        {

            SetUserInfo(sender, args);

            var data = args.IsfObjData;

            //标记地主座位
            if (data.ContainsKey(NewRequestKey.KeyLandLord)) LandSeat = data.GetInt(NewRequestKey.KeyLandLord);

            //检查是否显示地主图标
            if (UserDataTemp != null && UserDataTemp.GetInt(RequestKey.KeySeat) == LandSeat)
            {
                //StartCoroutine(ShowDizhuSpLater());
                DizhuSp.SetActive(true);
                return;
            }

            CheckShowJiabeiIcon(data);
        }

         /// <summary>
        /// 检查是否显示加倍icon
         /// </summary>
        /// <param name="data">重连时获得的消息数据</param>
        private void CheckShowJiabeiIcon(ISFSObject data)
         {
             if (UserDataTemp != null)
             {
                 var thisUserSeat = UserDataTemp.GetInt(RequestKey.KeySeat);
             
                 //先查是不是自己
                 if (data.ContainsKey(RequestKey.KeyUser))
                 {
                     var user = data.GetSFSObject(RequestKey.KeyUser);
                     if (user.GetInt(RequestKey.KeySeat) == thisUserSeat && user.GetInt(NewRequestKey.KeyRate)>1)
                     {
                         ShowJiaBeiSp.SetActive(true);
                         return;
                     }
                 }

                 //检查显示加倍图标
                 if (data.ContainsKey(RequestKey.KeyUserList))
                 {
                     var users = data.GetSFSArray(RequestKey.KeyUserList);
                     var len = users.Count;
                     for (int i = 0; i < len; i++)
                     {
                         var user = users.GetSFSObject(i);
                         if (user.GetInt(RequestKey.KeySeat) != thisUserSeat) continue;
                         if (user.GetInt(NewRequestKey.KeyRate) > 1)
                         {
                             ShowJiaBeiSp.SetActive(true);
                         }
                     }
                 }
             }
         }

        protected virtual void SetUserInfo(object sender ,DdzbaseEventArgs args)
        {
            
        }

        protected virtual void OnUserReady(object sender, DdzbaseEventArgs args)
        {
            ShowJiaBeiSp.SetActive(false);
            DizhuSp.SetActive(false);
            ShowSpeakSp.gameObject.SetActive(false);
            var data = args.IsfObjData;
            if(UserDataTemp!=null && data.GetInt(RequestKey.KeySeat) == UserDataTemp.GetInt(RequestKey.KeySeat)) GameReadySp.gameObject.SetActive(true);
        }

        protected virtual void OnUserJoinRoom(object sender, DdzbaseEventArgs args)
        {

        }

        /// <summary>
        /// 当有人叫分抢地主时
        /// </summary>
        protected void OnTypeGrab(object sender,DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            if (!DDzUtil.IsServDataContainAllKey(
                     new[]
                        {
                            RequestKey.KeySeat, RequestKey.KeyScore
                        }, data))
            {
                YxDebug.LogError("有人叫分时，信息key不全");
                return;
            }

            var seat = data.GetInt(RequestKey.KeySeat);
            var score = data.GetInt(RequestKey.KeyScore);
            if (UserDataTemp != null && UserDataTemp.GetInt(RequestKey.KeySeat) == seat)
            {
                ShowSpeakSp.gameObject.SetActive(true);
                //叫的分值
                switch (score)
                {
                    case 0:
                        {
                            ShowSpeakSp.spriteName = SpkBuJiao;
                            break;
                        }
                    case 1:
                        {
                            ShowSpeakSp.spriteName = Spk1Fen;
                            break;
                        }
                    case 2:
                        {
                            ShowSpeakSp.spriteName = Spk2Fen;
                            break;
                        }
                    case 3:
                        {
                            ShowSpeakSp.spriteName = Spk3Fen;
                            YxDebug.LogError("3分");
                            break;
                        }
                    default:
                        {
                            YxDebug.LogError("叫了一个超出预估的分值，所以不显示");
                            ShowSpeakSp.gameObject.SetActive(false);
                            break;
                        }
                }
                ShowSpeakSp.MakePixelPerfect();

            }

        }

        /// <summary>
        /// 开始叫分时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected virtual void OnTypeGrabSpeaker(object sender, DdzbaseEventArgs args)
        {
            //进入叫分阶段后，隐藏掉准备状态
            GameReadySp.gameObject.SetActive(false);
        }


        /// <summary>
        /// 当收到服务TypeFirstOut器相应
        /// </summary>
        protected virtual void TypeFirstOut(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            if (data.ContainsKey(RequestKey.KeySeat))
                LandSeat = data.GetInt(RequestKey.KeySeat);

            OnGetDipai(sender, args);

            OnCheckSelectDouble(args.IsfObjData);
        }

        /// <summary>
        /// 检查是否在选择加倍
        /// </summary>
        /// <param name="data"></param>
        public virtual void OnCheckSelectDouble(ISFSObject data)
        {

        }

        /// <summary>
        /// 从服务器获得底牌信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public virtual void OnGetDipai(object sender, DdzbaseEventArgs args)
        {
            ShowSpeakSp.gameObject.SetActive(false);

            var data = args.IsfObjData;

            if (UserDataTemp==null) return;

            //判断是给这位玩家发底牌么
            if (data.GetInt(RequestKey.KeySeat) != UserDataTemp.GetInt(RequestKey.KeySeat)) return;
            var ttcdsNum = UserDataTemp.GetInt(NewRequestKey.KeyCardNum) + data.GetIntArray(RequestKey.KeyCards).Length;
            UserDataTemp.PutInt(NewRequestKey.KeyCardNum, ttcdsNum);

            //StartCoroutine(ShowDizhuSpLater());
            DizhuSp.SetActive(true);
        }

/*        private IEnumerator ShowDizhuSpLater()
        {
            yield return new WaitForSeconds(0.3f);
            DizhuSp.SetActive(true);
        }*/

        /// <summary>
        /// 当游戏结算时清空GameReadySp 和  ShowSpeakSp
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected virtual void OnTypeGameOver(object sender, DdzbaseEventArgs args)
        {
            if (GameReadySp != null) GameReadySp.gameObject.SetActive(false);
            if (ShowSpeakSp != null) ShowSpeakSp.gameObject.SetActive(false);
        }

        /// <summary>
        /// 当玩家分数改变时
        /// </summary>
        /// <param name="seat"></param>
        /// <param name="scoreGold"></param>
        private void OnUserScoreChanged(int seat, int scoreGold)
        {
            if (UserDataTemp != null && seat == UserDataTemp.GetInt(RequestKey.KeySeat))
            {
                var curGold = UserDataTemp.GetLong(NewRequestKey.KeyTtGold) + scoreGold;
                UserDataTemp.PutLong(NewRequestKey.KeyTtGold, curGold);
                YxDebug.LogError("App.ShowGoldRate:"+ App.ShowGoldRate);
                ScoreLabel.text = YxUtiles.GetShowNumber(curGold).ToString(CultureInfo.InvariantCulture);
            }
        }



        /// <summary>
        /// 当收到加倍已经结束的信息
        /// </summary>
        protected virtual void OnDoubleOver(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            var rates = data.GetIntArray("jiabei");
            var len = rates.Length;
            if (UserDataTemp != null)
            {
                var thisUserSeat = UserDataTemp.GetInt(RequestKey.KeySeat);
                ShowSpeakSp.gameObject.SetActive(true);
                for (int i = 0; i < len; i++)
                {
                    if (thisUserSeat != i) continue;
                    if (rates[i] > 1)
                    {
                        ShowSpeakSp.spriteName = SpkJiabei;
                        ShowJiaBeiSp.gameObject.SetActive(true);
                    }
                    else
                    {
                        ShowSpeakSp.spriteName = SpkBuJiabei;
                    }
                    ShowSpeakSp.MakePixelPerfect();

                    //如果是地主，则不显示加倍不加倍信息
                    //if(thisUserSeat==LandSeat)ShowSpeakSp.gameObject.SetActive(false);

                    break;
                }
            }

            StopCoroutine("HideJiabeiSp");
            StartCoroutine("HideJiabeiSp");
        }

        /// <summary>
        /// 隐藏加倍说话sp
        /// </summary>
        /// <returns></returns>
        private IEnumerator HideJiabeiSp()
        {
            yield return new WaitForSeconds(3f);
            if (ShowSpeakSp.spriteName.Equals(SpkJiabei) || ShowSpeakSp.spriteName.Equals(SpkBuJiabei))
                ShowSpeakSp.gameObject.SetActive(false);
        }



        /// <summary>
        /// 当给这个玩家发牌时
        /// </summary>
        protected virtual void OnAlloCateCds(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            //此处不用判断是不是给自己发牌，因为服务器实际只给 游戏玩家自己发牌，其他玩家不发，但是每个玩家的牌数发的是一样的。所以可以赋值，擦
            if (UserDataTemp == null) return;
            var cardsLen = data.GetIntArray(GlobalConstKey.C_Cards).Length;
            UserDataTemp.PutInt(NewRequestKey.KeyCardNum, cardsLen);
        }

        //--------以下子类用到的公共方法-------------------------------------------------------------------------
        /// <summary>
        /// 获得玩家最大人人数
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected int GetPlayerMaxNum(ISFSObject data)
        {
            if (!data.ContainsKey(NewRequestKey.KeyPlayerNum)) throw new Exception("此isfobj data  不能获得玩家最大人数");
            return data.GetInt(NewRequestKey.KeyPlayerNum);
        }

        /// <summary>
        /// 获得其他玩家的数据信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected Dictionary<int, ISFSObject> GetOtherUsesDic(ISFSObject data)
        {
            if (!data.ContainsKey(RequestKey.KeyUserList)) throw new Exception("此isfobj data  不能获得玩家其他玩家的数据集合");
            //其他玩家数据集合
            var otherUsers = data.GetSFSArray(RequestKey.KeyUserList);
            //座位号对应ISFSObject
            var dataDic = new Dictionary<int, ISFSObject>();
            foreach (var user in otherUsers)
            {
                var isfobj = user as ISFSObject;
                if (isfobj != null) dataDic[isfobj.GetInt(RequestKey.KeySeat)] = isfobj;
            }
            return dataDic;
        }

/*        /// <summary>
        /// 设置默认头像
        /// </summary>
        protected void LoadDefaultHeadIcon(short sex)
        {
            string assetName = sex == 0 ? "headtexture0" : "headtexture1";
            var textureGob = ResourceManager.LoadAsset(assetName, assetName);
            HeadTexture.mainTexture = textureGob.GetComponent<UITexture>().mainTexture;
        }*/

/*        /// <summary>
        /// 加载真实头像
        /// </summary>
        /// <param name="headImgUrl">头像地址</param>
        /// <param name="sex">按性别加载默认头像</param>
        protected void LoadRealHeadIcon(string headImgUrl,short sex)
        {
            //加载真实头像
            Facade.Instance<AsyncImage>()
                  .GetAsyncImage(headImgUrl, tex =>
                  {
                      if (tex != null)
                      {
                          HeadTexture.mainTexture = tex;
                      }
                      else
                      {
                          DDzUtil.LoadDefaultHeadIcon(sex, HeadTexture);
                      }
                  });
        }*/


        /// <summary>
        /// 获得此客户端玩家自己的data
        /// </summary>
        /// <returns></returns>
        protected ISFSObject GetHostUserData(ISFSObject data)
        {
            if (data.ContainsKey(RequestKey.KeyUser))
            {
                return data.GetSFSObject(RequestKey.KeyUser);
            }

            throw new Exception("得到了空的服务器信息");
        }

        public void ShowUserInfo()
        {
            if (UserDataTemp!=null)
            {
                string name = UserDataTemp.GetUtfString("name");
                int id = UserDataTemp.GetInt("id");
                string ip = UserDataTemp.GetUtfString("ip");
                InfoDetial.ShowInfo(name, string.Format("ID:{0}",id), string.Format("IP:{0}",ip), HeadTexture);
            }
        }
    }
}
