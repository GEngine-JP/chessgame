using UnityEngine;
using System.Collections;
using Assets.Scripts.Hall.View.RecordWindows;

namespace Assets.Scripts.Tea.Page
{
    public class TeaTotalRecordWindow : TotalRecordWindow
    {
        protected override void OnAwake()
        {
            base.OnAwake();
        }

        [Tooltip("key茶馆口令")]
        public string KeyId = "id";

        protected override void SetActionDic()
        {
            base.SetActionDic();
            ActionParam[KeyId] = TeaUtil.CurTeaId;
        }
    }
}