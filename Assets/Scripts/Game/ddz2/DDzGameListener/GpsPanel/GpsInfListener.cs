using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Assets.Scripts.Game.ddz2.InheritCommon;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.ddz2.DDzGameListener.GpsPanel
{
    public class GpsInfListener : MonoBehaviour {
        readonly Dictionary<int,UserInfoStruct> _userinfoDic = new Dictionary<int, UserInfoStruct>();
        /// <summary>
        /// 左边玩家gps地址信息
        /// </summary>
        [SerializeField]
        protected UILabel LeftGpsInfo;
        /// <summary>
        /// 右边玩家Gps地址信息
        /// </summary>
        [SerializeField]
        protected UILabel RightGpsInfo;


        /// <summary>
        /// 距离信息
        /// </summary>
        [SerializeField]
        protected UILabel DistanceLabel;

        /// <summary>
        /// gps信息面板
        /// </summary>
       [SerializeField]
        protected GameObject GpsUiGob;

        void Awake()
        {
            Ddz2RemoteServer.AddOnGameInfoEvt(OnGameInfo);
            Ddz2RemoteServer.AddOnGetRejoinDataEvt(OnRejoinGame);
            Ddz2RemoteServer.AddOnUserJoinRoomEvt(OnUserJoinRoom);
            Ddz2RemoteServer.AddOnGpsInfoReceiveEvt(OnGpsInfoEvt);
        }

        private void OnUserJoinRoom(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            if(!data.ContainsKey(RequestKey.KeyUser)) return;

            var userdata = data.GetSFSObject(RequestKey.KeyUser);
            _userinfoDic[userdata.GetInt(RequestKey.KeySeat)] = SetUserData(userdata);
        }

        void Start()
        {

        }

        private void OnGpsInfoEvt(object sender, DdzbaseEventArgs args)
        {
            var userData = args.IsfObjData;
            var userId = userData.GetInt("uid");

            foreach (UserInfoStruct userinfoStruct in _userinfoDic.Values.Where(userinfoStruct => userId == userinfoStruct.Id))
            {
                JustSetGpsData(userinfoStruct, userData);
                break;
            }
        }

        private void OnRejoinGame(object sender, DdzbaseEventArgs args)
        {
            InitUserDic(args.IsfObjData);
        }

        private void OnGameInfo(object sender, DdzbaseEventArgs args)
        {
            InitUserDic(args.IsfObjData);
        }


        //显示GpsInfo
        public void OnShowGpsInfo()
        {
            var leftSeat = App.GetGameData<GlobalData>().GetLeftPlayerSeat;
            var rightSeat = App.GetGameData<GlobalData>().GetRightPlayerSeat;
            if (!_userinfoDic.ContainsKey(leftSeat)|| !_userinfoDic.ContainsKey(rightSeat)) return;

            GpsUiGob.SetActive(true);


            foreach (var seat in _userinfoDic.Keys)
            {
                if (seat == leftSeat)
                {
                    LeftGpsInfo.text = string.Format("IP:{0}\nID:{1}\n所在地:{2}\n{3}", _userinfoDic[seat].Ip,
                        _userinfoDic[seat].Id, _userinfoDic[seat].Country, (_userinfoDic[seat].GpsX!=-1f) ? "已经提供位置信息" : "未提供位置信息\n请开启位置服务,并给予应用相应权限");
                }else if (seat == rightSeat)
                {
                    RightGpsInfo.text = string.Format("IP:{0}\nID:{1}\n所在地:{2}\n{3}", _userinfoDic[seat].Ip,
                        _userinfoDic[seat].Id, _userinfoDic[seat].Country, (_userinfoDic[seat].GpsX != -1f) ? "已经提供位置信息" : "未提供位置信息\n请开启位置服务,并给予应用相应权限");
                }
            }

            if(_userinfoDic[leftSeat].GpsX==-1 || _userinfoDic[rightSeat].GpsX==-1)return;

            var distance = Distince(_userinfoDic[leftSeat].GpsX, _userinfoDic[leftSeat].GpsY, _userinfoDic[rightSeat].GpsX, _userinfoDic[rightSeat].GpsY);

            string des = "";
            if (distance < 1000)
            {
                if (distance < 100)
                {
                    des = "<=100m";
                }
                else
                {
                    des = string.Format("距离：{0}M", distance);
                }

            }
            else
            {
                des = string.Format("距离：{0} KM", distance / 1000f);
            }
            DistanceLabel.text = des;
        }

        /// <summary>
        /// 隐藏GpsInfo
        /// </summary>
        public void OnCloseGpsInfo()
        {
            GpsUiGob.SetActive(false);
        }


        class UserInfoStruct
        {
            public int Id;

            /// <summary>
            /// 地里位置
            /// </summary>
            public string Country;
            /// <summary>
            /// Ip地址
            /// </summary>
            public string Ip;

            /// <summary>
            /// 纬度
            /// </summary>
            public float GpsX=-1;

            /// <summary>
            /// 经度
            /// </summary>
            public float GpsY=-1;

        }

        /// <summary>
        /// 设置用户信息
        /// </summary>
        /// <param name="userData"></param>
        UserInfoStruct SetUserData(ISFSObject userData)
        {
            var userinfoStruct = new UserInfoStruct();

            if (userData.ContainsKey(RequestKey.KeyId))
                userinfoStruct.Id = userData.GetInt(RequestKey.KeyId);

            if (userData.ContainsKey("ip"))
                userinfoStruct.Ip = userData.GetUtfString("ip");

            if (userData.ContainsKey("country"))
                userinfoStruct.Country = userData.GetUtfString("country");

            //获取gpsx; gpsy
            if ((userData.ContainsKey("gpsx") && userData.ContainsKey("gpsy")) ||
                (userData.ContainsKey("x") && userData.ContainsKey("y")))
            {
                userinfoStruct.GpsX = userData.ContainsKey("gpsx") ? userData.GetFloat("gpsx") : userData.GetFloat("x");
                userinfoStruct.GpsY = userData.ContainsKey("gpsy") ? userData.GetFloat("gpsy") : userData.GetFloat("y");
            }
            else
            {
                userinfoStruct.GpsX = -1f;
                userinfoStruct.GpsY = -1f;
            }
            return userinfoStruct;
        }


        private void JustSetGpsData(UserInfoStruct userinfoStruct,ISFSObject userData)
        {

            if (userData.ContainsKey("ip"))
                userinfoStruct.Ip = userData.GetUtfString("ip");

            if (userData.ContainsKey("country"))
                userinfoStruct.Country = userData.GetUtfString("country");

            //获取gpsx; gpsy
            if ((userData.ContainsKey("gpsx") && userData.ContainsKey("gpsy")) ||
                (userData.ContainsKey("x") && userData.ContainsKey("y")))
            {
                userinfoStruct.GpsX = userData.ContainsKey("gpsx") ? userData.GetFloat("gpsx") : userData.GetFloat("x");
                userinfoStruct.GpsY = userData.ContainsKey("gpsy") ? userData.GetFloat("gpsy") : userData.GetFloat("y");
            }
            else
            {
                userinfoStruct.GpsX = -1f;
                userinfoStruct.GpsY = -1f;
            }
        }


        private void InitUserDic(ISFSObject data)
        {
            if (!data.ContainsKey(RequestKey.KeyUserList)) throw new Exception("此isfobj data  不能获得玩家其他玩家的数据集合");
            //其他玩家数据集合
            var otherUsers = data.GetSFSArray(RequestKey.KeyUserList);
            foreach (ISFSObject user in otherUsers)
            {
                if (user.ContainsKey(RequestKey.KeySeat))
                {
                    _userinfoDic[user.GetInt(RequestKey.KeySeat)] = SetUserData(user);
                }
            }
        }


        /// <summary>
        /// 根据Gps信息获得2个点之间距离
        /// </summary>
        /// <param name="a1">n</param>
        /// <param name="a2">e</param>
        /// <param name="b1">n</param>
        /// <param name="b2">e</param>
        /// <returns></returns>
        public static double Distince(float a1, float a2, float b1, float b2)
        {
            double R = 6371004;
            double PI_RM = 180 / Math.PI;
            double C = 1 - (Math.Pow((Math.Sin((90 - a2) / PI_RM) * Math.Cos(a1 / PI_RM) - Math.Sin((90 - b2) / PI_RM) * Math.Cos(b1 / PI_RM)), 2) + Math.Pow((Math.Sin((90 - a2) / PI_RM) * Math.Sin(a1 / PI_RM) - Math.Sin((90 - b2) / PI_RM) * Math.Sin(b1 / PI_RM)), 2) + Math.Pow((Math.Cos((90 - a2) / PI_RM) - Math.Cos((90 - b2) / PI_RM)), 2)) / 2;
            return R * Math.Acos(C);
        }
    }
}
