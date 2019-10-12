using System;
using UnityEngine;

namespace Assets.Scripts.Game.jh.ui.plen_8
{
    public class JhHupUpItem8 : JhHupUpItem
    {

        public UILabel Info;

        public override void SetInfo(string texture, string name, int sex, int icon = 0)
        {
            string content = "【" + name + "】";
            Info.text = content;
            SetIcon(icon);
        }

        public override void SetIcon(int icon)
        {
            string text = Info.text;
            int index = text.IndexOf("】", StringComparison.Ordinal);
            string name = text.Substring(0, index+1);
            switch (icon)
            {
                case 0:
                    name += "等待选择";
                    break;
                case 2:
                case 3:
                    name += "同意解散";
                    break;
                case -1:
                    name += "拒绝解散";
                    break;
            }

            Info.text = name;
        }
    }
}
