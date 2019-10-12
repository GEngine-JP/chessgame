using Assets.Scripts.Common.YxPlugins.Gps.Managers;
using YxPlugins.Interfaces;
using YxPlugins.Managers;

namespace Assets.Scripts.Common.YxPlugins
{
    public class YxPluginsFactory : IBasePluginsFactory {
        public YxBaseLocationManager CreateGps()
        {
            return new YxLocationManager();
        }
    }
}
