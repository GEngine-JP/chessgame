using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public partial class MahjongUtility
    {
        /// <summary>
        /// 通过Hex的值获取32位颜色
        /// </summary>
        /// <param name="Hex">十六进制数0xFFFFFF</param>
        /// <returns></returns>
        public static Color ChangeToColor(uint Hex)
        {
            float R = (Hex >> 16) & 0x0000FF;
            float G = (Hex >> 8) & 0x0000FF;
            float B = Hex & 0x0000FF;
            return new Color(R / 255, G / 255, B / 255, 1);
        }

        /// <summary>
        /// 把所有孩子的layer都变成父亲的layer
        /// </summary>
        /// <param name="tf"></param>
        public static void ChangeLayer(Transform tf)
        {
            if (tf == null || tf.parent == null) return;
            ChangeLayer(tf, tf.parent.gameObject.layer);
        }

        /// <summary>
        /// 把所有孩子的layer都变成指定的层
        /// </summary>     
        public static void ChangeLayer(Transform transform, int layer)
        {
            transform.gameObject.layer = layer;
            if (transform.childCount > 0)
            {
                for (int i = transform.childCount - 1; i >= 0; i--)
                {
                    ChangeLayer(transform.GetChild(i), layer);
                }
            }
        }

        public static void DoScreenShot(MonoBehaviour mono, Rect rect, System.Action<string> onFinish)
        {
            var Compress = new CompressImg();
            Compress.DoScreenShot(mono, rect, onFinish);
        }
    }
}