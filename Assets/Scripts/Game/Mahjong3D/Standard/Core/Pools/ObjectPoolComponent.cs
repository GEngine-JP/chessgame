using System.Collections.Generic;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class ObjectPoolComponent : ObjectPoolBase
    {
        private LinkedList<ObjectBase> mPool = new LinkedList<ObjectBase>();

        public virtual T Pop<T>(string name, Func<T, bool> comparer) where T : ObjectBase
        {
            ObjectBase obj = null;
            if (mPool.First != null)
            {
                LinkedListNode<ObjectBase> node = mPool.First;
                while (null != node)
                {
                    if (comparer == null)
                    {
                        break;
                    }
                    if (comparer(node.Value as T))
                    {
                        obj = node.Value;
                        break;
                    }
                    node = node.Next;
                }
                if (obj != null)
                {
                    mPool.Remove(node);
                    return obj.ExCompShow() as T;
                }
                else
                {
                    return Spawn<T>(name);
                }
            }
            else { return Spawn<T>(name); }
        }

        public override void Push(ObjectBase obj)
        {
            if (obj != null)
            {
                obj.ExSetParent(transform).ExCompHide();
                mPool.AddFirst(obj);
            }
        }

        protected virtual T Spawn<T>(string name) where T : ObjectBase
        {
            T temp = GameUtils.CreateMahjongEffectAsset<T>(name).Do((o) =>
            {
                o.Init(this);
                o.ExCompShow();
            });
            if (temp != null)
            {
                return temp.Do((o) => { o.name = name; });
            }
            return null;
        }
    }
}