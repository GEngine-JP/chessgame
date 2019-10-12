using System.Collections.Generic;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public partial class C2SCustom
    {
        private bool[] _hasDan;
        private List<int> _canChoose;

        public void OnXjfd()
        {
            var mahHand = GameCenter.Scene.MahjongGroups.PlayerHand;
            var ccMahHand = mahHand.GetMahHandComponent<MahPlayerHand_Ccmj>();
            ccMahHand.ChooseXjfdOnHand();
        }

        public Dictionary<int, int> GetCardAmountOnXjfd(List<int> list)
        {
            Dictionary<int, int> dic = GetCardAmount(list);
            return dic;
        }
    }
}