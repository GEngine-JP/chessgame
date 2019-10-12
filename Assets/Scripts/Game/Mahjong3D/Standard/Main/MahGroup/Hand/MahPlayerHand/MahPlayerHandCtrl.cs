using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahPlayerHandCtrl : MonoBehaviour, IMahPlayerHandCtrl
    {
        public IMahPlayerHand OnIni()
        {
            IMahPlayerHand playerHand;
            switch (MahjongUtility.GameKey)
            {
                case GameMisc.ZhmjKey:
                case GameMisc.DltdhKey:
                case GameMisc.TdDltdhKey:
                case GameMisc.DlmjKey: playerHand = gameObject.AddComponent<MahPlayerHand_Dlmj>(); break;
                case GameMisc.XzmjKey:
                case GameMisc.XlmjKey: playerHand = gameObject.AddComponent<MahPlayerHand_Xzmj>(); break;
                case GameMisc.BbmjKey: playerHand = gameObject.AddComponent<MahPlayerHand_Bbmj>(); break;
                case GameMisc.NamjKey: playerHand = gameObject.AddComponent<MahPlayerHand_Namj>(); break;
                case GameMisc.CcmjKey: playerHand = gameObject.AddComponent<MahPlayerHand_Ccmj>(); break;
                case GameMisc.SzwmmjKey:
                case GameMisc.SzmjKey: playerHand = gameObject.AddComponent<MahPlayerHand_Szmj>(); break;
                case GameMisc.QdjtKey: playerHand = gameObject.AddComponent<MahPlayerHand_Qdjt>(); break;
                                   
                //case GameMisc.DbsmjKey: playerHand = gameObject.AddComponent<MahPlayerHand_Dbsmj>(); break;
                default: playerHand = gameObject.AddComponent<MahPlayerHand>(); break;
            }
            return playerHand;
        }
    }
}