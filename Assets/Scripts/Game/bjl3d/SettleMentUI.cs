using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.bjl3d
{
    public class SettleMentUI : MonoBehaviour// 改完11.14
    {
        public Text[] jiesuanTexts;
        public Text TotalText;
        public Text Xpoint;
        public Text Zpoint;

        /// <summary>
        /// 比赛结果
        /// </summary>
        public void GameResultFun()
        {
            MusicManager.Instance.Play("JieSuan");
            //AudioClip clip = ResourcesLoader.instance.LoadAudio("music/JieSuan");
            //            AudioManager.Instance.Play(clip, false, .8f);
            var gdata = App.GetGameData<GlobalData>();
            for (int i = 0; i < gdata.BetJiesuan.Length; i++)
            {
                if (gdata.B == gdata.CurrentUser.Seat)
                {
                    jiesuanTexts[i].text = gdata.BetMoney[i].ToString();
                }
                else
                {
                    if (gdata.BetJiesuan[i] == 0)
                    {
                        jiesuanTexts[i].text = -gdata.BetMoney[i] + "";
                    }
                    else
                    {
                        jiesuanTexts[i].text = gdata.BetJiesuan[i] * gdata.BetMoney[i] + "";
                    }
                }
            }
            TotalText.text = gdata.Win + "";
            Xpoint.text = gdata.XianValue + "";
            Zpoint.text = gdata.ZhuangValue + "";

        }

    }
}