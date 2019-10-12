using System.Collections.Generic;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class ActionHuPlayback : AbsCommandAction
    {
        protected bool mZhaniaoFlag;
        protected bool mHuFlag;

        public override void OnReset()
        {
            mZhaniaoFlag = false;
        }

        public void PlaybackTing(PlaybackFrameData data)
        {
            PlayEffect(data.OpChair, PoolObjectType.ting);
        }

        public void PlaybackLiuju(PlaybackFrameData data)
        {
            Game.TableManager.HideOutcardFlag();
        }

        public void PlaybackHu(PlaybackFrameData data)
        {
            var paoChair = data.LastFrameData.OpChair;
            var huChair = data.OpChair;
            var huCard = data.Cards[0];

            var effect = MahjongUtility.PlayMahjongEffect(PoolObjectType.shandian);
            var targetPos = Game.MahjongGroups.MahjongThrow[paoChair].GetLastMjPos();
            effect.transform.position = targetPos;
            effect.Execute();
            MahjongUtility.PlayEnvironmentSound("shandian");

            var item = Game.MahjongCtrl.PopMahjong(huCard);
            Game.MahjongGroups.MahjongOther[huChair].GetInMahjong(huCard);
            Game.TableManager.ShowOutcardFlag(item);
            item.gameObject.SetActive(true);

            PlayEffect(data.OpChair, PoolObjectType.hu);
        }

        public void PlaybackZimo(PlaybackFrameData data)
        {
            var huChair = data.OpChair;
            var huCard = data.Cards[0];

            Game.MahjongGroups.MahjongHandWall[huChair].PopMahjong();

            var item = Game.MahjongCtrl.PopMahjong(huCard);
            Game.MahjongGroups.MahjongOther[huChair].GetInMahjong(item);
            Game.TableManager.ShowOutcardFlag(item);
            item.gameObject.SetActive(true);

            PlayEffect(data.OpChair, PoolObjectType.zimo);
        }

        public void PlaybackZhaNiao(PlaybackFrameData data)
        {
            if (!mZhaniaoFlag)
            {
                mZhaniaoFlag = true;

                var zhongMaList = new List<int>();
                zhongMaList.AddRange(data.Cards);
                GameCenter.Hud.GetPanel<PanelExhibition>().Open(zhongMaList);
            }
        }

        private void PlayEffect(int chair, PoolObjectType type)
        {
            GameCenter.Controller.PlayOperateEffect(chair, type);
        }

        public void PlaybackGameOver(PlaybackFrameData data)
        {
            if (!mHuFlag)
            {
                mHuFlag = true;
                var playerCount = GameCenter.DataCenter.MaxPlayerCount;
                var resultData = new PlaybackResultDate();
                var group = Game.MahjongGroups;

                List<int> cards;
                List<MahjongContainer> mahjongs;
                //收集手牌
                for (int i = 0; i < playerCount; i++)
                {
                    mahjongs = group.MahjongHandWall[i].MahjongList;
                    cards = GameUtils.ConverToCards(mahjongs);
                    resultData.SetHandCard(cards, i);
                }
                //收集胡牌
                for (int i = 0; i < playerCount; i++)
                {
                    mahjongs = group.MahjongOther[i].MahjongList;
                    if (mahjongs.Count > 0)
                    {
                        cards = GameUtils.ConverToCards(mahjongs);
                        resultData.SetHucardList(cards, i);
                    }
                }
                //收集cpg
                for (int i = 0; i < playerCount; i++)
                {
                    var cpgItems = group.MahjongCpgs[i].CpgItemList;
                    if (cpgItems.Count > 0)
                    {
                        var cpgs = new List<CpgModel>();
                        for (int j = 0; j < cpgItems.Count; j++)
                        {
                            cpgs.Add(cpgItems[j].Model);
                        }
                        resultData.SetCpgModels(cpgs, i);
                    }
                }
                GameCenter.Hud.GetPanel<PanelPlaybackResult>().Open(resultData);
            }
            else
            {
                GameCenter.Hud.GetPanel<PanelPlaybackResult>().Open();
            }
        }
    }
}
