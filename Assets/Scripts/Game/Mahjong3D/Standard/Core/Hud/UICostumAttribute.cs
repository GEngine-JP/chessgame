using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class UICostumAttribute : Attribute
    {
        public string Gamekey = MiscUtility.DefName;

        public UICostumAttribute() { }

        public UICostumAttribute(string gameKey) { Gamekey = gameKey; }
    }

    public interface IUICostum
    {
        void OnInin();
    }
}
