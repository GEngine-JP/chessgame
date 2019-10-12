using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahPlayerHand_Bbmj : MahPlayerHand
    {
        public override bool SetHandCardState(HandcardStateTyps state, params object[] args)
        {
            if (base.SetHandCardState(state, args))
            {
                if (state == HandcardStateTyps.Youjin)
                {
                    SwitchYoujinState(args);
                }
            }
            return true;
        }

        private void SwitchYoujinState(params object[] args)
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
                    item.SetThowOutCall(YoujinClickEvent);
                }       
            }
        }

        /// <summary>
        /// Daigu click event
        /// </summary>      
        private void YoujinClickEvent(Transform transf)
        {
            MahjongContainer item;
            var Mj = transf.GetComponent<MahjongContainer>();
            if (!Mj.Lock)
            {
                PlayerHand.HasToken = false;
                bool flag = GameCenter.Network.C2S.Custom<C2SCustom>().ThrowoutCardOnYoujin(Mj.Value);
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