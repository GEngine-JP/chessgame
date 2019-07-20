using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using YxFramwork.Common;
using YxFramwork.Common.Utils;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.bjl3d
{
    public class Plane5 : MonoBehaviour
    {
        public static Plane5 Instance;

        protected void Awake()
        {
            Instance = this;

        }
        public bool IsPointerOverUIObject()
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }

        public void OnMouseDown()
        {
            MusicManager.Instance.Play("Bet");
            var gdata = App.GetGameData<GlobalData>();
            if (IsPointerOverUIObject())
                return;
            if (gdata.Allow[5]!=0&&gdata.GoldNum[UserInfoUI.Instance.GameConfig.CoinType] > gdata.Allow[5])
            {
                GameUI.Instance.NoteText_Show("下注已经达到上限！！！");
                return;
            }
            if (UserInfoUI.Instance.GameConfig.GameState != 5)
            {
                GameUI.Instance.NoteText_Show("此时不能下注！！！");
                return;
            }
            if (gdata.CurrentUser.Seat == gdata.B)
            {
                GameUI.Instance.NoteText_Show("自己是庄家，不能下注！！！");
                return;
            }
            if (gdata.CurrentUser.Gold < gdata.GoldNum[UserInfoUI.Instance.GameConfig.CoinType])
            {
                GameUI.Instance.NoteText_Show("金币不足！！！");
                return;
            }
            App.GetRServer<GameServer>().UserBet(5, gdata.GoldNum[UserInfoUI.Instance.GameConfig.CoinType]);
            gdata.CurrentUser.Gold -= gdata.GoldNum[UserInfoUI.Instance.GameConfig.CoinType];
            App.GameData.GStatus = GameStatus.PlayAndConfine;   
        }


    }
}
