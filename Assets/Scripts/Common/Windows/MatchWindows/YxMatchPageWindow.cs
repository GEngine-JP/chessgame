using System.Collections.Generic;
using Assets.Scripts.Common.Windows.TabPages;
using UnityEngine;

namespace Assets.Scripts.Common.Windows.MatchWindows
{
    /// <summary>
    /// ��������
    /// </summary>
    public class YxMatchPageWindow : YxTabPageWindow
    {
        protected override void ActionCallBack()
        {
            base.ActionCallBack();
            var data = GetData<Dictionary<string,object>>();
            if (data == null) return;
            /*
             * {
                    "tabs" : [ { name: ""} ],
                    "matchList": [
                                    {
                                        id:       //����id
                                        name:"",  //��������
                                        pcount:"",//����
                                        gamename:""//��Ϸ����
                                        time:""//����ʱ��
                                        state: // ����״̬
                                    }
                                  ]
                                 
             
                }
             */
        }
    }
}
