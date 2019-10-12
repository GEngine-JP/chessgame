using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Tea
{
    /// <summary>
    /// ����¼�֮��û�д�����ݵ���ʾ�����������ֱ�ӽ����Լ��Ĳ��
    /// </summary>
    public class TeaCreatCtrl : MonoBehaviour
    {

        public void OnCreatJoinTeaWindow()
        {
            Facade.Instance<TwManager>().SendAction("group.teaOwnerData", new Dictionary<string, object>(), GetOwnerTea);
        }

        private void GetOwnerTea(object msg)
        {
            var dic = msg as Dictionary<string, object>;
            if (dic != null)
            {
                var obj = YxWindowManager.OpenWindow("TeaPanel");
                var panel = obj.GetComponent<TeaPanel>();
                panel.UpdateView(dic);
                panel.IsQuickJoin = true;
            }
            else
            {
                YxWindowManager.OpenWindow("TeaCreateWindow");
            }
        }
    } 
}
