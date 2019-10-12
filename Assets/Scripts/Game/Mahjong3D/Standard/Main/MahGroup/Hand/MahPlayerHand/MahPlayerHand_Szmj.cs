using System.Collections.Generic;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahPlayerHand_Szmj : MahPlayerHand
    {
        private void Start()
        {
            //设置出牌过滤条件
            mPutOutFunc = (item) => { return item.MahjongCard.Value >= (int)MahjongValue.Zhong; };
        }
    }
}
