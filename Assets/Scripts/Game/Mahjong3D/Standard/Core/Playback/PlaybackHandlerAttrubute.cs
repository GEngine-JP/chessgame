using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public enum PlaybackType
    {
        /// <summary>
        /// 正常播放
        /// </summary>
        Normal,
        /// <summary>
        /// 倒叙播放
        /// </summary>
        Return,
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class PlaybackHandlerAttrubute : Attribute
    {
        public int ProtocolKey;
        /// <summary>
        /// 是倒叙播放
        /// </summary>
        public PlaybackType Type;

        /// <summary>
        /// 监听回放事件
        /// </summary>
        /// <param name="key">协议</param>
        /// <param name="flag">倒叙播放</param>
        public PlaybackHandlerAttrubute(int key, PlaybackType type = PlaybackType.Normal)
        {
            ProtocolKey = key;
            Type = type;
        }
    }
}
