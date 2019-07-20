using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Game.ddz2.DDz2Common
{

    public class MessageBoxScroll : MonoBehaviour
    {
        /// <summary>
        /// 显示的内容宽度
        /// </summary>
        [SerializeField] protected float ViewContentWidth = 600;

        [SerializeField] protected TweenWidth TweenWidth;

        [SerializeField] protected UIPanel UiPanel;

        // Update is called once per frame
        void Update () {
	        
        }


        private IEnumerator StartShowUiContent()
        {
            var panelx = UiPanel.baseClipRegion.x;
            var panely = UiPanel.baseClipRegion.y;
            var panelw = UiPanel.baseClipRegion.w;

            while (true)
            {
                yield return new WaitForEndOfFrame();
                UiPanel.baseClipRegion = new Vector4(panelx, panely, TweenWidth.value, panelw);
                if (UiPanel.baseClipRegion.z > ViewContentWidth) yield break;
            }
        }

        void OnEnable()
        {
            StopAllCoroutines();
            StartCoroutine(StartShowUiContent());
            TweenWidth.SetOnFinished(() =>
                {
                    UiPanel.baseClipRegion = new Vector4(UiPanel.baseClipRegion.x, UiPanel.baseClipRegion.y, 600, UiPanel.baseClipRegion.w);
                });
            TweenWidth.PlayForward();
        }

        void OnDisable()
        {
            TweenWidth.ResetToBeginning();
            UiPanel.baseClipRegion = new Vector4(UiPanel.baseClipRegion.x, UiPanel.baseClipRegion.y, 30, UiPanel.baseClipRegion.w);
        }

        /// <summary>
        /// 当点击关闭按钮
        /// </summary>
        public void OnCloseBtnClick()
        {
            gameObject.SetActive(false);
        }

        public void OpenMessageBox()
        {
            gameObject.SetActive(true);
        }

        public void HideMessageBox()
        {
            gameObject.SetActive(false);
        }
    }
}
