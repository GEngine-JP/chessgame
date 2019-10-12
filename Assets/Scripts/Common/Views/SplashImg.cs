using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using YxFramwork.Common.Adapters;

namespace Assets.Scripts.Common.Views
{
    /// <summary>
    /// ��������
    /// </summary>
    public class SplashImg : MonoBehaviour
    {
        /// <summary>
        /// �ȴ�ʱ��
        /// </summary>
        public float WaitTime = 1.5f;
        /// <summary>
        /// ǰ��
        /// </summary>
        public YxBaseTextureAdapter Foreground;
        /// <summary>
        /// ����
        /// </summary>
        public YxBaseTextureAdapter Background;

        void Awake()
        {
            if (Foreground != null)
            {
                Foreground.MakePixelPerfect();
            }
        }

        IEnumerator Start ()
        {
            var wait = new WaitForSeconds(WaitTime);
            yield return wait;
            SceneManager.LoadScene(1);
        }
    }
}
