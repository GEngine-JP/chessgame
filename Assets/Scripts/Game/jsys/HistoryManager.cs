using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;

namespace Assets.Scripts.Game.jsys
{
    public class HistoryManager : MonoBehaviour
    {

        public static HistoryManager Instance;

        /// <summary>
        /// 图片Image属性
        /// </summary>
        public Image[] ImgSprite;

        /// <summary>
        /// 图片sprite信息
        /// </summary>

        public Sprite[] Sprites;
        private int[] histroyNums = new int[10];
        public void Awake()
        {
            Instance = this;
        }
        protected void Start()
        {
            for (int i = 0; i < 10; i++)
            {
                histroyNums[i] = -1;
                if (histroyNums[i] == -1)
                {
                    ImgSprite[i].gameObject.SetActive(false);
                }
            }
        }

        //初始化的时候显示历史纪录
        public void ShowHistory(int[] history)
        {
            for (int i = 0; i < histroyNums.Length; i++)
            {
                if (history[i] == -1)
                {
                    history[i] = 9;
                    ImgSprite[i].gameObject.SetActive(false);
                }
                else
                {
                    ImgSprite[i].gameObject.SetActive(true);
                }

                ImgSprite[i].sprite = Sprites[history[i]];
            }
        }
        private int _index;
        //正常游戏时历史记录的变化
        public void ShowNewHistory(int pos)
        {
            App.GetGameData<GlobalData>().History[_index] = pos;
            _index++;
            _index %= 10;
            for (int i = 0; i < histroyNums.Length; i++)
            {
                ImgSprite[i].sprite = Sprites[App.GetGameData<GlobalData>().History[(i + _index) % 10]];
            }
        }
    }
}

