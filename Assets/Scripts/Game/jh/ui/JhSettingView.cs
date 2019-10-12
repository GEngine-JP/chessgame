using UnityEngine;
using YxFramwork.Framework;
using YxFramwork.Manager;
using Assets.Scripts.Game.jh.EventII;

namespace Assets.Scripts.Game.jh.ui
{
    public class JhSettingView : MonoBehaviour
    {

        public YxWindow Window;

        public EventObject EventObject;
        public void Show()
        {
            if (Window == null)
            {
                Window = YxWindowManager.OpenWindow("SettingWindow");
            }
            if (EventObject != null)
            {
                Window.ShowWithData(EventObject);    
            }
            else
            {
                Window.Show();
            }
        }

        public void OnRecieve(EventData data)
        {
            switch (data.Name)
            {
                case "Show":
                    Show();
                    break;
            }
        }
    }
}
