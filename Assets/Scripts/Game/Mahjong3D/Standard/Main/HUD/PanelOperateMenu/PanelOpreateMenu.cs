using System.Collections.Generic;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [UIPanelData(typeof(PanelOpreateMenu), UIPanelhierarchy.Popup)]
    public class PanelOpreateMenu : UIPanelBase, IUIPanelControl<OpreateMenuArgs>
    {
        public List<OperateButtonItem> Btns;

        public override void OnContinueGameUpdate()
        {
            HideButtons();
        }

        public override void OnReadyUpdate()
        {
            HideButtons();
        }

        public void HideButtons()
        {
            base.Close();
            for (int i = 0; i < Btns.Count; i++)
            {
                Btns[i].gameObject.SetActive(false);
            }
        }

        public void Open(OpreateMenuArgs args)
        {
            base.Open();
            if (null == args) return;
            List<KeyValuePair<int, bool>> opMenu = args.OpMenu;
            for (int i = 0; i < Btns.Count; i++)
            {
                Btns[i].SetActive(opMenu);
            }
        }

        public override void Close()
        {
            base.Close();
            GameCenter.EventHandle.Dispatch((int)UIEventProtocol.QueryHuCard, new QueryHuArgs() { PanelState = false });
        }

        public void OnChiClick()
        {
            GameCenter.Network.C2S.Custom<C2SCustom>().OnChi();
            Close();
        }

        public void OnPengClick()
        {
            GameCenter.Network.C2S.Custom<C2SCustom>().OnPeng();
            Close();
        }

        public void OnGangClick()
        {
            GameCenter.Network.C2S.Custom<C2SCustom>().OnGang();
            Close();
        }

        public void OnHuClick()
        {
            GameCenter.Network.C2S.Custom<C2SCustom>().OnHu();
            //关闭托管
            GameCenter.EventHandle.Dispatch((int)GameEventProtocol.AiAgency, new AiAgencyArgs() { State = false });
            Close();
        }

        public void OnGuoClick()
        {
            GameCenter.Network.C2S.Custom<C2SCustom>().OnGuo();
            Close();
        }

        public void OnTingClick()
        {
            GameCenter.Network.C2S.Custom<C2SCustom>().OnTing(HandcardStateTyps.ChooseTingCard);
            //关闭托管
            GameCenter.EventHandle.Dispatch((int)GameEventProtocol.AiAgency, new AiAgencyArgs() { State = false });
            Close();
        }

        public void OnTingDaiguClick()
        {
            GameCenter.Network.C2S.Custom<C2SCustom>().OnTing(HandcardStateTyps.Daigu);
            Close();
        }

        public void OnTingNiuClick()
        {
            GameCenter.Network.C2S.Custom<C2SCustom>().OnTing(HandcardStateTyps.ChooseNiuTing);
            Close();
        }

        public void OnYoujinClick()
        {
            GameCenter.Network.C2S.Custom<C2SCustom>().OnTing(HandcardStateTyps.Youjin);
            Close();
        }

        public void OnLaiZiGangClick()
        {
            GameCenter.Network.C2S.Custom<C2SCustom>().OnLaiZiGang();
            Close();
        }

        public void OnXjfdClick()
        {
            GameCenter.Network.C2S.Custom<C2SCustom>().OnXjfd();
            Close();
        }

        public void OnJueGangClick()
        {
            GameCenter.Network.C2S.Custom<C2SCustom>().OnJueGnag();
            Close();
        }
    }
}