using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class GameRuntimeDataAttribute : Attribute
    {
        public string GameKey;
        public RuntimeDataType DataType;

        public GameRuntimeDataAttribute(RuntimeDataType type, string gamekey = "")
        {
            DataType = type;
            GameKey = gamekey;
        }
    }
}