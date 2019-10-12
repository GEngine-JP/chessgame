using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.sssjp.ImgPress.Main;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.sssjp.skin1
{
    public class MatchMgrSkin1 : MatchMgr
    {

        public SpecialPlayerInfo SpecialPlayer;

        public int Sequence;

        private bool _hasSwat;
        public override void MatchCards(List<UserMatchInfo> matchInfoList)
        {
            foreach (UserMatchInfo matchInfo in matchInfoList)
            {
                if (matchInfo.Special > (int) CardType.none)
                {
                    SpecialList.Add(matchInfo);
                    

                    DunScore dunScore = new DunScore
                    {
                        Seat = matchInfo.Seat,
                        NormalScore = new List<int>(),
                        AddScore = new List<int>()
                    };
                    ShootScoreArray = matchInfo.ShootScore;
                    for (int i = 0; i < matchInfo.AddScore.Count; i++)
                    {
                        dunScore.NormalScore.Add(matchInfo.NormalScores[i]);
                        dunScore.AddScore.Add(matchInfo.AddScore[i]);
                    }
                    continue;
                }

                MatchInfoList.Add(matchInfo);

                if (matchInfo.Shoot != null && matchInfo.Shoot.ShootCount > 0)
                {
                    
                    ShootList.Add(matchInfo.Shoot);
                }
               

                ShootScoreArray = matchInfo.ShootScore;
                for (int i = 0; i < matchInfo.AddScore.Count; i++)
                {
                    DunScoreList.Add(new DunScore()
                    {
                        Seat = matchInfo.Seat,
                        NormalScore = matchInfo.NormalScores,
                        AddScore = matchInfo.AddScore
                    });
                }

                if (matchInfo.Swat)
                {
                    _hasSwat = true;
                }
            }

            MatchBegin(); //开始比牌
        }

        protected override IEnumerator MatchPlayerCards()
        {
            var main = App.GetGameManager<SssjpGameManager>();
            var gdata = App.GetGameData<SssGameData>();
            int selfSeat = gdata.SelfSeat;


            //每行开始展示
            if (SpecialList.Count > 0) //特殊牌型不参与比牌,给出特殊牌型提示
            {
                for (int i = 0; i < SpecialList.Count; i++)
                {
                    gdata.GetPlayer<SssPlayer>(SpecialList[i].Seat, true).HandCardsType.SetSpecialMarkActive(true);
                }
            }


            for (int i = 0; i < 3; i++) //展示手牌的行数
            {
                SortList(MatchInfoList, i); //对列表进行排序,这样可以依次展示手牌

                for (int j = 0; j < MatchInfoList.Count; j++)
                {
                    UserMatchInfo tempInfo = MatchInfoList[j];
                    var panel = gdata.GetPlayer<SssPlayer>(tempInfo.Seat, true);
                    //展示玩家手牌
                    panel.ShowHandPoker(i, tempInfo);
                    panel.ShowCardType(i, tempInfo);

                    yield return new WaitForSeconds(0.6f);
                }
                //main.TurnRes.ShowResultItem(i, DunScoreList[0].NormalScore[i], DunScoreList[0].AddScore[i]);
                ShowAllPlayerLineTotalScore(i);
            }
          
            //播放打枪
            if (gdata.HaveShoot && ShootList.Count > 0)
            {
                ShootList.Sort((s1, s2) => Sequence*(s1.ShootCount - s2.ShootCount));
                foreach (ShootInfo item in ShootList)
                {
                    if (item.ShootTargs == null || item.ShootTargs.Length <= 0)
                    {
                        //ShootList.Remove(item);
                        continue;
                    }

                    for (int i = 0; i < item.ShootTargs.Length; i++)
                    {
                        int serverTargSeat = item.ShootTargs[i];
                        int serverShootSeat = item.Seat;

                        Facade.Instance<MusicManager>().Play("beforeshoot");
                        yield return new WaitForSeconds(.7f);
                        gdata.GetPlayer<SssPlayer>(serverShootSeat, true)
                            .ShootSomeone(gdata.GetPlayer<SssPlayer>(serverTargSeat, true));

                        //打枪需要修改总分
                        if (serverShootSeat == selfSeat) //说明是自己打枪,获取额外得分
                        {
                            //设置总分数
                            int shootScore = gdata.ShootScore == 0 ? ShootScoreArray[serverTargSeat] : gdata.ShootScore;
                            main.TurnRes.ResultTotal.SetValue(shootScore);
                        }
                        if (serverTargSeat == selfSeat) //说明自己被打枪,扣除额外分数
                        {
                            int shootScore = gdata.ShootScore == 0
                                ? ShootScoreArray[serverShootSeat]
                                : -gdata.ShootScore;
                            main.TurnRes.ResultTotal.SetValue(shootScore);
                        }
                       
                        yield return new WaitForSeconds(.7f);
                    }
                }
            }
           
            //播放特殊牌型
            if (SpecialList.Count > 0)
            {
                foreach (UserMatchInfo item in SpecialList)
                {
                    Renderer component = SpecialLabelParticle.GetComponent<Renderer>();
                    CardType cardType = (CardType) item.Special;
                    string typeName = cardType.ToString();
                    component.material.mainTexture = SpecialTextureList.Find(tex => tex.name == typeName);

                    //播放特殊牌型效果(特效)
                    ShowSpecialPar();
                    Facade.Instance<MusicManager>().Play(typeName,"special");   //播放音效
                    SpecialPlayer.ShowSpecialPlayerInfo(item);  //显示玩家信息
                    yield return new WaitForSeconds(3f);
                    SpecialPlayer.HideSpecialPlayerInfo();
                    StopParticle(SpecialParticle);
                    var player = gdata.GetPlayer<SssPlayer>(item.Seat, true);
                    player.HandCardsType.SetSpecialMarkActive(false);
                    player.ShowAllHandPoker(item);
                    player.SetSpecialType(item.Special);
                }
            }

            if (_hasSwat)
            {
                main.ShowSwatAnim();
                Facade.Instance<MusicManager>().Play("swat");
                yield return new WaitForSeconds(1f);
            }

            ShowAllPlayerTotalScore();

            IsMatching = false;
        }

        private void ShowAllPlayerTotalScore()
        {
            ShowListPlayersTotalScore(MatchInfoList);
            ShowListPlayersTotalScore(SpecialList);
            ShowAllPlayerShootInfo(MatchInfoList);
            ShowAllPlayerShootInfo(SpecialList);
        }

        private void ShowAllPlayerShootInfo(List<UserMatchInfo> matchInfoList)
        {
            ShowListPlayersShootInfo(MatchInfoList);
        }

        private void ShowListPlayersShootInfo(List<UserMatchInfo> matchInfoList)
        {
            var gdata = App.GameData;
            foreach (var info in matchInfoList)
            {
                var panel = gdata.GetPlayer<SssPlayer>(info.Seat, true);
                panel.HandCardsType.ShowShootInfo(info.Shoot.ShootCount, info.Shoot.BeShootCount);
            }
        }

        void ShowListPlayersTotalScore(List<UserMatchInfo> list)
        {
            var gdata = App.GameData;
            foreach (var player in list)
            {
                var panel = gdata.GetPlayer<SssPlayer>(player.Seat, true);
                panel.HandCardsType.ShowTotalScore(player.TtScore);
            }
        }

        void ShowAllPlayerLineTotalScore(int line)
        {
            var playerList = App.GameData.PlayerList;
            foreach (var player in playerList)
            {
                if (player.Info == null || !player.ReadyState) continue;
                var panel = (SssPlayer) player;
                panel.HandCardsType.SetLineTotalScore(line);
            }
        }

        /// <summary>
        /// 播放特殊牌型特效
        /// </summary>
        void ShowSpecialPar()
        {
            StartParticle(SpecialParticle);
            StartParticle(SpecialLabelParticle);
        }

        public override void Reset()
        {
            base.Reset();
            _hasSwat = false;
        }

    }
}
