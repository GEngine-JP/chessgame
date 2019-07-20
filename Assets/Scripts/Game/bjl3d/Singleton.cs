

namespace Assets.Scripts.Game.bjl3d
{

    public class Singleton<T> where T :class
    {

        #region 私有变量

        private static T g_instance;                                                // 静态实例句柄
        private static readonly object g_instanceLock = new object();               // 锁，控制多线程冲突

        #endregion

        #region 公开属性

        /// <summary>
        /// 获取单例
        /// </summary>
        /// <remarks>即单例模式中所谓的全局唯一公共访问点</remarks>
        public static T Instance
        {
            get
            {
                // 此处双重检测，确保多线程无冲突
                if (g_instance == null)
                {
                    lock (g_instanceLock)
                    {
                        if (g_instance == null)
                        {
                            g_instance = (T)System.Activator.CreateInstance(typeof(T), true);
                            System.Console.WriteLine("hnu3d.........unity......create instance:" + g_instance.ToString());

                        }
                    }
                }

            

                return g_instance;
            }
        }



        #endregion



        #region 销毁对象

        public void DelMe()
        {
            // 此处双重检测，确保多线程无冲突
            if (g_instance != null)
            {
                lock (g_instanceLock)
                {
                    if (g_instance != null)
                    {                  
//                        Logger.Log ("..delet---:" + g_instance.ToString());
                        g_instance = null;
                    }
                }
            }

        }

        #endregion

    }
}
