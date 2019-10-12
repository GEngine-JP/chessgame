using System.Collections.Generic;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public abstract class AbsDataExtension
    {
        protected Dictionary<string, Variable> mParams = new Dictionary<string, Variable>();

        public Variable this[string key]
        {
            get
            {
                if (mParams.ContainsKey(key)) return mParams[key];
                return null;
            }
            set
            {
                mParams[key] = value;
            }
        }

        public bool TryGetValue<T>(string Key, out T value) where T : Variable
        {
            Variable var = null;
            if (mParams.TryGetValue(Key, out var))
            {
                value = var as T;
                return true;
            }
            value = null;
            return false;
        }

        public T Get<T>(string key) where T : Variable
        {
            if (mParams.ContainsKey(key))
            {
                return mParams[key] as T;
            }
            return null;
        }

        public virtual void Reset() { mParams.Clear(); }

        public abstract void SetData(ISFSObject datam, MahjongUserInfo userInfo);
    }
}