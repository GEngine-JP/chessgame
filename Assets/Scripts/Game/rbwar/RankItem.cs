using Assets.Scripts.Common.Adapters;
using UnityEngine;
using YxFramwork.Common.DataBundles;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.rbwar
{
    public class RankItem : MonoBehaviour
    {
        public GameObject NormalBg;
        public GameObject SpecialBg;
        public UISprite UserRankSpecial;
        public UISprite UserRankNomalNo;
        public UISprite UserRankNomalS;
        public UISprite UserRankNomalG;
        public NguiTextureAdapter UserHead;
        public UILabel UserName;
        public UILabel UserGold;
        public UILabel AboutAround;
        public UILabel BetGold;
        public UILabel WinAround;

        public void SetRankData(int rankNum,RbwarUserInfo userInfo,int aboutAround)
        {
            if (rankNum <= 1)
            {
                SpecialBg.SetActive(true);
            }
            else
            {
                NormalBg.SetActive(true);
            }

            if (rankNum < 9)
            {
                UserRankSpecial.gameObject.SetActive(true);
                UserRankSpecial.spriteName =string.Format("coinNum{0}", rankNum);
                UserRankSpecial.MakePixelPerfect();
            }
            else
            {
                if (rankNum == 9)
                {
                    UserRankNomalNo.gameObject.SetActive(true);
                    UserRankNomalS.gameObject.SetActive(true);
                    UserRankNomalS.spriteName = "num9";
                }
                else
                {
                    UserRankNomalNo.gameObject.SetActive(true);
                    UserRankNomalS.gameObject.SetActive(true);
                    UserRankNomalG.gameObject.SetActive(true);

                    var sNum = rankNum / 10 % 10;
                    UserRankNomalS.spriteName =string.Format("num{0}", sNum);

                    var gNum = rankNum % 10;
                    UserRankNomalG.spriteName =string.Format("num{0}", gNum);
                }
            }
            PortraitDb.SetPortrait(userInfo.AvatarX,UserHead,userInfo.SexI);
            UserName.text = userInfo.NickM;
            UserGold.text =YxUtiles.ReduceNumber(userInfo.CoinA);

            AboutAround.text = string.Format("近{0}局", aboutAround);

            BetGold.text = YxUtiles.ReduceNumber(userInfo.TwentyBet);
            WinAround.text = userInfo.TwentyWin.ToString();
        }

    }
}
