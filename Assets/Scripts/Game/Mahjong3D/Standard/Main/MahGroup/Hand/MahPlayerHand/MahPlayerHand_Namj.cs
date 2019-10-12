using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahPlayerHand_Namj : MahPlayerHand
    {
        public override bool SetHandCardState(HandcardStateTyps state, params object[] args)
        {
            if (base.SetHandCardState(state, args))
            {
                if (state == HandcardStateTyps.Daigu)
                {
                    SwitchDaiguState(args);
                }
            }
            return true;
        }

        private void SwitchDaiguState(params object[] args)
        {
            MahjongContainer item;
            List<int> tingList = args[0] as List<int>;
            if (tingList == null || tingList.Count == 0) return;
            var list = PlayerHand.MahjongList;
            for (int i = 0; i < list.Count; i++)
            {
                item = list[i];
                item.ResetPos();
                if (!tingList.Contains(item.Value))
                {
                    item.Lock = true;                  
                    item.RemoveMahjongScript();
                }
                else
                {
                    item.SetMahjongScript();
                    item.SetThowOutCall(DaiguClickEvent);
                }
            }
            UserContorl.ClearSelectCard();
        }

        /// <summary>
        /// Daigu click event
        /// </summary>      
        private void DaiguClickEvent(Transform transf)
        {
            MahjongContainer item;
            var Mj = transf.GetComponent<MahjongContainer>();
            if (!Mj.Laizi && !Mj.Lock)
            {
                PlayerHand.HasToken = false;
                bool flag = GameCenter.Network.C2S.Custom<C2SCustom>().ThrowoutCardOnDaigu(Mj.Value);
                if (flag)
                {
                    Mj.ResetPos();
                    var list = PlayerHand.MahjongList;
                    for (int i = 0; i < list.Count; i++)
                    {
                        item = list[i];
                        item.SetMahjongScript();
                        item.SetThowOutCall(ThrowCardClickEvent);
                    }
                }
            }
        }
    }
}