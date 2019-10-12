using Assets.Scripts.Game.FishGame.Common.Utils;
using UnityEngine;
using System.Collections;
using YxFramwork.Common;

namespace Assets.Scripts.Game.FishGame.Common.core
{
    /// <summary>
    /// ��ʱֹͣ�ָ�
    /// </summary>
    /// <remarks>���������Ҫ����ָ�����:
    /// 1.�ָ���ʱ�����������Ϸʱ����.
    /// 2.�ָ���ʱ����ڹ���(sweep)��,:�ָ�����,��Ϊ�����ӵ���������BUG
    /// 4.��ʼ��ʱ����ڹ�����,�ȴ��ӵ���ʧ������->�����ӵ����ж�ʱը�� : ֱ�ӻָ�,ԭ��ͬ��
    /// </remarks>
    public class FreezeBombRecover : MonoBehaviour
    { 
        void Awake()
        {
            StartCoroutine(_Coro_DelayRecover());  
        } 

        /// <summary>
        /// ����ը���ָ����������ƶ�
        /// </summary>
        /// <returns></returns>
        IEnumerator _Coro_DelayRecover()
        { 
            yield return new WaitForSeconds(10F); 

            Recover();
        
            Destroy(gameObject);
        }

        //�ָ�����,������ƶ�
        void Recover()
        { 
            GameMain.IsMainProcessPause = false;
            var gdata = App.GetGameData<FishGameData>();
            if (gdata.EvtFreezeBombDeactive != null)
                gdata.EvtFreezeBombDeactive(); 
        }
    }
}
