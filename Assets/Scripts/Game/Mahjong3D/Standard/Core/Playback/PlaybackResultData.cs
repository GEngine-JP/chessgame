using System.Collections.Generic;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class PlaybackPlayerDate
    {
        public List<int> HuCards;
        public List<int> HardCards;
        public List<CpgModel> CpgModels;

        public ReplayUserData UserData;

        public PlaybackPlayerDate(int index)
        {
            //玩家信息
            var datas = GameCenter.Playback.ReplayData;
            UserData = datas.GetUserData(index);
        }
    }

    public class PlaybackResultDate
    {
        public Dictionary<int, PlaybackPlayerDate> ResultDic = new Dictionary<int, PlaybackPlayerDate>();

        public void SetHandCard(List<int> cards, int index)
        {
            var data = GetPlayerDate(index);
            data.HardCards = cards;
        }

        public void SetCpgModels(List<CpgModel> models, int index)
        {
            var data = GetPlayerDate(index);
            data.CpgModels = models;
        }

        public void SetHucardList(List<int> cards, int index)
        {
            var data = GetPlayerDate(index);
            data.HuCards = cards;
        }

        private PlaybackPlayerDate GetPlayerDate(int index)
        {
            PlaybackPlayerDate data;
            if (!ResultDic.TryGetValue(index, out data))
            {
                data = new PlaybackPlayerDate(index);
                ResultDic[index] = data;
            }
            return data;
        }
    }
}
