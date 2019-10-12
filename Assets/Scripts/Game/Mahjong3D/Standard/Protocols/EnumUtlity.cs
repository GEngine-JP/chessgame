namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    /// <summary>
    /// 解散房间类型
    /// </summary>
    public enum DismissFeedBack
    {
        Refuse = -1,
        None = 0,
        ApplyFor = 2,
        Agree = 3,
    }

    //游戏循环类型 
    public enum MahGameLoopType
    {
        Round,      //用局算
        Circle,     //用圈算
    }

    //房间类型
    public enum MahRoomType
    {
        FanKa,
        YuLe,
    }

    /// <summary>
    /// 牌值枚举
    /// </summary>
    public enum MahjongValue
    {
        None = 0,
        Wan_1 = 17,
        Wan_2,
        Wan_3,
        Wan_4,
        Wan_5,
        Wan_6,
        Wan_7,
        Wan_8,
        Wan_9,
        Tiao_1 = 33,
        Tiao_2,
        Tiao_3,
        Tiao_4,
        Tiao_5,
        Tiao_6,
        Tiao_7,
        Tiao_8,
        Tiao_9,
        Bing_1 = 49,
        Bing_2,
        Bing_3,
        Bing_4,
        Bing_5,
        Bing_6,
        Bing_7,
        Bing_8,
        Bing_9,
        Dong = 65,
        Nan = 68,
        Xi = 71,
        Bei = 74,
        Zhong = 81,
        Fa = 84,
        Bai = 87,
        //花牌
        ChunF = 96,
        XiaF,
        QiuF,
        DongF,
        MeiF,
        LanF,
        ZuF,
        JuF,
        //苏州麻将
        Other = 7 << 4,
        Laoshu = Other + 1,
        Mao = Other + 4,
        Caishen = Other + 7,
        Jubao = Other + 10,
        //百搭
        Baida = (8 << 4) + 1,
        //大白板
        BigBai = Baida + 4,
    }
}
