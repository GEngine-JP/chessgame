using UnityEngine;

namespace Assets.Scripts.Hall.View.AboutRoomWindows
{
    /// <summary>
    /// �����ķ����б���ÿ��Item��Ϣ
    /// </summary>
    public class CreateRoomListItem : MonoBehaviour
    {
        public UILabel RoomId;
        public UILabel PlayRule;
        public UILabel DiFen;
        public UILabel RoundNum;
        public UILabel PlayerNum;
        public UILabel GameKey;
        public UIButton BtnWeiChat;

        public void InitData(string roomId, string playRule, string ante, string round, string playerNum, string gameKey)
        {
            RoomId.text = roomId;
            PlayRule.text = playRule;
            DiFen.text = ante;
            RoundNum.text = round;
            PlayerNum.text = playerNum;
            GameKey.text = gameKey;
        }

    }
}
