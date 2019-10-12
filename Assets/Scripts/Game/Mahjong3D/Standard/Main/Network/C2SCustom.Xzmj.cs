using YxFramwork.ConstDefine;
using YxFramwork.View;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public partial class C2SCustom
    {
        public void OnConfirmChangeClick()
        {
            var game = GameCenter.Scene;
            var mahHand = game.MahjongGroups.PlayerHand;
            var xzMahHand = mahHand.GetMahHandComponent<MahPlayerHand_Xzmj>();
            var selectCards = xzMahHand.SelectMahList;
            // 校验
            int iSelCount = xzMahHand.SelectMahList.Count;
            if (3 != iSelCount)
            {
                YxMessageBox.Show("请选择{0}张牌相同花色牌！".ExFormat(3));
                return;
            }
            int iColor0 = selectCards[0].Value >> 4;
            int iColor1 = selectCards[1].Value >> 4;
            int iColor2 = selectCards[2].Value >> 4;
            if (iColor0 != iColor1 || iColor1 != iColor2)
            {
                YxMessageBox.Show("请选择{0}张牌相同花色牌！".ExFormat(3));
                return;
            }
            int[] array = new int[selectCards.Count];
            for (int i = 0; i < selectCards.Count; i++)
            {
                array[i] = selectCards[i].Value;
            }
            //发送请求
            GameCenter.Network.OnRequestC2S((sfs) =>
            {
                sfs.PutInt(RequestKey.KeyType, NetworkProtocol.MJChangeCards);
                sfs.PutIntArray("cards", array);
                return sfs;
            });
            //移除手牌 
            for (int i = 0; i < selectCards.Count; i++)
            {
                mahHand.RemoveMahjong(selectCards[i]);
            }
            // 扣下
            game.MahjongGroups.SwitchGorup[0].AddMahToSwitch(array);
            //整理手牌
            xzMahHand.AgainSortHandCards();
            //停止定时任务
            GameCenter.Scene.TableManager.StopTimer();
            GameCenter.EventHandle.Dispatch((int)UIEventProtocol.HideChangeTitleBtn);
        }
    }
}