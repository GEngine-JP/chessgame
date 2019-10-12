namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    /// <summary>
    /// 锚点类型
    /// </summary>
    public enum Anchor
    {
        /// <summary>
        /// 正上方
        /// </summary>
        MarginTop,
        //上
        TopLeft,
        TopCenter,
        TopRight,
        //中
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        //下
        BottomLeft,
        BottomCenter,
        BottomRight,
    }

    /// <summary>
    /// 相对本家，其他玩家的座位
    /// </summary>
    public enum RelativeSeat
    {
        None = 0,
        /// <summary>
        /// 下家
        /// </summary>
        Behind = 1,
        /// <summary>
        /// 对家
        /// </summary>
        Opposite = 2,
        /// <summary>
        /// 上家
        /// </summary>
        Front = 3,
    }
}
