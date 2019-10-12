using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.DataBundles;

namespace Assets.Scripts.Hall.View.AlllShowRecordWindow
{
    /// <summary>
    /// ÿ����������ҵ�����Ϣ
    /// </summary>
    public class PlayerTotalItem : MonoBehaviour
    {
        public YxBaseTextureAdapter HeadTexture;
        public UILabel PlayerName;
        public UILabel PlayerGold;

        public void InitPlayerData(string playerName,string playerGold,string avatar="")
        {
            PlayerName.text = playerName;
            PlayerGold.text = playerGold;
            PortraitDb.SetPortrait(avatar, HeadTexture,1);
        }
    }
}
