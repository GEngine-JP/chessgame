namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class NetworkProtocol
    {
        public const int MJRequestTypeSelectPiao = 0x1;//弹出选择漂按钮
        public const int MJRequestTypeShowPiao = 0x2;//前台玩家选择漂、后台发送展示玩家选择的漂
        public const int MJRequestTypeShowRate = 0x3;//游戏底注 
        public const int MJRequestTypeCPG = 0x50;//普通回应，表示玩家受到服务器通知或者做了取消操作  
        public const int CPG_PengGang = 5;
        public const int MJRequestTypeAlloCate = 0x11;//分牌
        public const int MJRequestTypeGetInCard = 0x12;//玩家抓牌  
        public const int MJThrowoutCard = 0x13;//玩家打牌 
        public const int MJReqTypeZiMo = 0x15;//自摸 
        public const int MJOpreateType = 0x18;//用户操作吃碰杠胡
        public const int MJRequestTypeBao = 23;//宝
        public const int MJRequestTypeTing = 0x51;//Ting
        public const int MJRequestTypeHu = 0x54;
        public const int MJRequestTypeLiangDao = 0x14;//亮牌
        public const int MJRequestTypeZimo = 0x15;//玩家自摸 
        public const int MJRequestTypeMoreTime = 0x16;//申请更多时间 
        public const int MJResponseAllowResponse = 0x21;//服务器允许玩家请求
        public const int MjRequestTypeQiangGangHu = 0x23;//询问客户端胡不胡这个牌（玩家杠的时候，看看别人能不能抢杠胡） 
        public const int MjRequestTypeCheckCards = 0x40;
        public const int MJRequestTypeSelfGang = 0x55;//自己抓牌后 可以杠胡的
        public const int CPG_ZhuaGang = 4;
        public const int CPG_MingGang = 6;
        public const int CPG_AnGang = 7;
        public const int CPG_LaiziGang = 8;
        public const int MJRequestTypeXFG = 0x56;//旋风杠       
        public const int MJReqTypeLastCd = 0x5A;//流局
        public const int MJReqGetNeedCard = 0x20;
        public const int MJRequestTypeBuZhang = 0x30;//补张
        public const int MJRequestTypeBuZhangFinish = 0x31;
        public const int MJRequestTypeBuZhangGetIn = 0x32;
        public const int MJRequestTypeGetHuCards = 0x5E;//查询胡牌
        public const int MJRequestTypeDan = 0x5F;
        public const int MJChangeCards = 0x60;//开始换张      
        public const int MJRotateCards = 0x61;//开始旋转 
        public const int MJSelectColor = 0x64;//服务器通知开始选花色 
        public const int MJSelColorRst = 0x65;//把选中的花色发送给服务器       
        public const int MJGameResult = 0x66;//血战血流玩法的最后胡牌
        public const int MJRequestStartTing = 0x69;//起手报听
        public const int MJReponeseStartTing = 0x70;
        public const int MJRequestTypeJiaMa = 0x67;//加码
        public const int MJRequestTypeJiaMaFinish = 0x68;
        public const int MJRequestJueGang = 0x1a;//绝杠
        public const int CPG_AnJuegang = 0xb;
        public const int CPG_Ligang = 0x71;//立杠     
        public const int MJRequestFenZhang = 0x74;//分张
        public const int MJChoosePao = 0x75;
        public const int MJUserChoose = 0x76;
        public const int MJQiFei = 0x77;
        public const int MJDingshen = 0x78;
        public const int MJHuanshen = 0x79;
        public const int MJLaiZiGang = 0x81;
        public const int MJRequestTypeDaiGu = 0x83;//农安玩法  
        public const int MJQiangGangHuType = 1 << 30;
        public const int ResponseYoujin = 93;//游金 
        public const int MJRequestTypeNextBao = 0x84;
        public const int MJRequestLigang = 0x71;//立杠
    }

    /// <summary>
    /// 客戶端自用协议
    /// </summary>
    public class CustomClientProtocol
    {
        public const int CustomTypeCustomLogic = 100001;//自定义逻辑
        public const int CustomTypeReconnectLogic = 100002;//重连
        public const int CustomTypeReadyLogic = 100003;//准备
    }
}