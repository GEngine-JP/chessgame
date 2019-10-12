using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class EmptySpace : MonoBehaviour
    {
        public void OnClickEmptySpace()
        {
            if (GameCenter.GameProcess == null || GameCenter.GameProcess == null) return;
            if (GameCenter.GameProcess.IsCurrState<StateGamePlaying>())
            {
                var panel = GameCenter.Hud.GetPanel<PanelQueryHuCard>();
                if (null != panel) panel.Close();
                var PlayerHand = GameCenter.Scene.MahjongGroups.PlayerHand;
                var mahPlayer = PlayerHand.GetComponent<MahPlayerHand>();
                if (mahPlayer.CurrState == HandcardStateTyps.Normal || mahPlayer.CurrState == HandcardStateTyps.DingqueOver)
                {
                    PlayerHand.HandCardsResetPos();
                }
            }
        }
    }
}