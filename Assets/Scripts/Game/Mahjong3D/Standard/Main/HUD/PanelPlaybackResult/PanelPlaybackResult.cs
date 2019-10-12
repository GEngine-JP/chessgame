namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [UIPanelData(typeof(PanelPlaybackResult), UIPanelhierarchy.Popup)]
    public class PanelPlaybackResult : UIPanelBase
    {
        public SingleResultItem[] PlayersItem;

        public void OnRestartClick()
        {
            GameCenter.EventHandle.Dispatch((int)UIEventProtocol.PlaybackRestart);
            Close();
        }

        public void Open(PlaybackResultDate args)
        {
            base.Open();
            for (int i = 0; i < PlayersItem.Length; i++)
            {
                PlayersItem[i].gameObject.SetActive(false);
            }

            SingleResultItem item;
            var datas = args.ResultDic;
            for (int i = 0; i < datas.Count; i++)
            {
                item = PlayersItem[i];
                item.gameObject.SetActive(true);

                var playData = datas[i];
                // 设置cpg
                item.SetCpgCard(playData.CpgModels);
                // 设置手牌               
                item.SetCards(playData.HardCards);
                // 设置胡牌 
                item.SetCards(playData.HuCards);
                //排序
                item.SortCardGroup();

                item.Name = playData.UserData.Name;
                var socre = playData.UserData.Gold;
                //总分
                //info = MahjongUtility.GetShowNumberFloat(args.Result[i].Gold).ToString();
                item.SetItem(TextType.TotalSocre, socre);
                //杠分               
                //info = MahjongUtility.GetShowNumberFloat(args.Result[i].GangGlod).ToString();
                socre = playData.UserData.Gang;
                item.SetItem(TextType.GangScore, socre);
            }
        }
    }
}