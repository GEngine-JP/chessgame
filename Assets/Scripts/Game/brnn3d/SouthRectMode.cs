using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Utils;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.brnn3d
{
    public class SouthRectMode : MonoBehaviour
    {
        public static SouthRectMode Instance;
        protected void Awake()
        {
            Instance = this;
        }
        public void OnMouseDown()
        {
            MusicManager.Instance.Play("chouma");
            App.GetGameData<GlobalData>().isOut = false;
            
            if (App.GetGameData<GlobalData>().IsBet == false)
            {
                NoteUI.Instance.Note("此时不能下注！");
                return;
            }
            if (EastRectMode.Instance.IsPointerOverUIObject())
                return;

            if (App.GetGameData<GlobalData>().CurrentBanker.Seat == App.GetGameData<GlobalData>().CurrentUser.Seat)
            {
                NoteUI.Instance.Note("庄家是自己，不能下注！");
                return;
            }
            if (App.GetGameData<GlobalData>().BankList == null || App.GetGameData<GlobalData>().BankList.Size() == 0)
            {
                NoteUI.Instance.Note("没有庄家不能下注！");
                return;
            }
            if (App.GetGameData<GlobalData>().I64ChoumaValue[App.GetGameData<GlobalData>().ChouMaType] >
                App.GetGameData<GlobalData>().CurrentUser.Gold * 0.1)
            {
                NoteUI.Instance.Note("最大下注数是您拥有钱的1/10！");
            }

            if (App.GetGameData<GlobalData>().CurrentUser.Gold < App.GetGameData<GlobalData>().I64ChoumaValue[App.GetGameData<GlobalData>().ChouMaType])
            {
                NoteUI.Instance.Note("已到最大下注数！");
            }
            App.GetGameData<GlobalData>().BetPosSelf = 1;
            App.GetRServer<GameServer>().UserBet(1, (int)App.GetGameData<GlobalData>().I64ChoumaValue[App.GetGameData<GlobalData>().ChouMaType]);
            //BetMode.Instance.InstanceCoinDemo(GameConfig.Instance.iChouMaType, 1, App.GetGameData<GlobalData>().CurrentUser.Seat);
            App.GameData.GStatus = GameStatus.PlayAndConfine;
        }
    }
}

