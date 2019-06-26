using Assets.Scripts.Common;
using Assets.Scripts.Common.Interface;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Interface;
using YxFramwork.Common.Model;

namespace Assets.Scripts.Hall.Controller
{
    /**    
     *QQ:           765858558
     *Unity版本：   5.4.0f3 
     *创建时间:     2018-02-05  
     *历史记录: 
    */
    internal class HallEnter : EnterHall
    { 
        protected override ISysCfg GetSystemCfg()
        { 
            return new SysConfig();
        }

        protected override IUI GetUIImpl()
        {
            return new NguiImpl();
        } 
    }
}
