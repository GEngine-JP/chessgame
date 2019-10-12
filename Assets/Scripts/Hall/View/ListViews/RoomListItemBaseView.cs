using YxFramwork.Common.Model;
using YxFramwork.Framework;

namespace Assets.Scripts.Hall.View.ListViews
{
    /// <summary>
    /// �����б���ͼ��ʽ����
    /// </summary>
    public class RoomListItemBaseView : YxView
    {
        /// <summary>
        /// ������Ϣ
        /// </summary>
        protected RoomUnitModel RoomInfo;

        public void Init(RoomUnitModel roomModel)
        {
            RoomInfo = roomModel;
            OnInit();
        }

        protected virtual void OnInit()
        {
            
        }


        public void OnRoomClick(string index)
        {
            var roomItem = MainYxView as RoomListItem;
            if (roomItem == null) { return;}
            roomItem.OnRoomClick(index);
        }
    }
}
