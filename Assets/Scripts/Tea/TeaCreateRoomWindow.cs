using System.Collections.Generic;
using Assets.Scripts.Hall.View.AboutRoomWindows;
using YxFramwork.Controller;
using YxFramwork.Enums;

namespace Assets.Scripts.Tea
{
    public class TeaCreateRoomWindow : CreateRoomWindow
    {
        public TeaPanel teaPanel;
        public string MessageBoxName;

        protected override void OnAwake()
        {
            base.OnAwake();
            CreateType = YxECreateRoomType.Caff;
        }

        protected override void SendCreateRoom(Dictionary<string, object> data)
        {
            if (teaPanel != null)
            {
                string teaid = teaPanel.Code.text;
                data["tea_id"] = teaid;
            }
            RoomListController.Instance.CreatGroupRoom(data, CreateRoomBack); 
        }

        private void CreateRoomBack(object obj)
        {
            TeaUtil.GetBackString(obj, MessageBoxName);
            teaPanel.GetTableList(false);
            Close();
        }
    }
}
