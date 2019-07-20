using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.jsys
{
    public class AnimationManager : MonoBehaviour
    {
        public static AnimationManager Instance;
        //下注按钮的图片数组
        public Image[] BetAnimationImages;

        //显示倍数的图片
        public Image BeishuImage;

        //倍数的数组
        public Sprite[] BeishuSprites;

        public void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// 收到转动结果消息处理,显示获奖动画
        /// </summary>
        public void ShowAnimation()
        {
            //Debug.Log("收到转动结果消息处理,显示获奖动画");
            BetPanelManager.Instance.ShowiWiningText(App.GetGameData<GlobalData>().Winning);
            SetBeishuSprite(App.GetGameData<GlobalData>().Multiplying[App.GetGameData<GlobalData>().EndAnimal]);

            AudioPlay.Instance.PlaySounds("Animal" + App.GetGameData<GlobalData>().EndAnimal + "");
           
//            StartCoroutine(PlaySound(App.GetGameData<GlobalData>().EndAnimal));
            //显示此局游戏所出现的小动物按钮的闪动

            Invoke("ShowBetPanel", 1.0f);
        }
        IEnumerator PlaySound(int num)
        {
            if (num == 8 || num == 9)
            {
                yield return new WaitForSeconds(1f);
                AudioPlay.Instance.PlaySounds("Dajiang");
            }
        }
        public void ShowBetPanel()
        {
            if (App.GetGameData<GlobalData>().IsShark)
            {
                if (8 == App.GetGameData<GlobalData>().SharkPos)
                {
                    TurnGroupsManager.Instance.GameConfig.IsSliverShark = true;

                }
                if (9 == App.GetGameData<GlobalData>().SharkPos)
                {
                    TurnGroupsManager.Instance.GameConfig.IsGoldShark = true;
                }
                BetAnimationImages[App.GetGameData<GlobalData>().SharkPos].gameObject.SetActive(true);
            }
            else
            {
                if (0 <= App.GetGameData<GlobalData>().EndAnimal && App.GetGameData<GlobalData>().EndAnimal <= 3)
                {
                    BetAnimationImages[10].gameObject.SetActive(true);
                }
                if (3 < App.GetGameData<GlobalData>().EndAnimal && App.GetGameData<GlobalData>().EndAnimal < 8)
                {
                    BetAnimationImages[11].gameObject.SetActive(true);
                }
                if (TurnGroupsManager.Instance.GameConfig.IsGoldShark)
                {
                    BetAnimationImages[9].gameObject.SetActive(true);
                    TurnGroupsManager.Instance.GameConfig.IsGoldShark = false;
                }
                if (TurnGroupsManager.Instance.GameConfig.IsSliverShark)
                {
                    BetAnimationImages[8].gameObject.SetActive(true);
                    TurnGroupsManager.Instance.GameConfig.IsSliverShark = false;
                }
                BetAnimationImages[App.GetGameData<GlobalData>().EndAnimal].gameObject.SetActive(true);
            }
        }

        public void HideBetPanel()
        {
            foreach (Image image in BetAnimationImages)
            {
                if (image.gameObject.activeSelf)
                {
                    image.gameObject.SetActive(false);
                }
            }
        }

        //隐藏获奖动画
        public void HideAnimation()
        {
            BeishuImage.gameObject.SetActive(false);
        }
        //转动前显示金鲨银鲨动画
        public void ShowGoldSharkAnimation()
        {
            Invoke("HidGoldSharkAnimation", 4.0f);
        }
        //隐藏金鲨银鲨动画
        public void HidGoldSharkAnimation()
        {
            TurnGroupsManager.Instance.ChangeState();
            MusicManager.Instance.Stop();
            AudioPlay.Instance.PlaySounds("Paodeng");
        }
        //设置倍数动画
        public void SetBeishuSprite(int beishu)
        {
            switch (beishu)
            {
                case 3:
                    {
                        BeishuImage.sprite = BeishuSprites[0];
                    }
                    break;
                case 4:
                    {
                        BeishuImage.sprite = BeishuSprites[1];
                    }
                    break;
                case 5:
                    {
                        BeishuImage.sprite = BeishuSprites[1];
                    }
                    break;
                case 6:
                    {
                        BeishuImage.sprite = BeishuSprites[2];
                    }
                    break;
                case 8:
                    {
                        BeishuImage.sprite = BeishuSprites[3];
                    }
                    break;
                case 12:
                    {
                        BeishuImage.sprite = BeishuSprites[4];
                    }
                    break;
                case 24:
                    {
                        BeishuImage.sprite = BeishuSprites[5];
                    }
                    break;
            }
            BeishuImage.gameObject.SetActive(true);
        }
    }
}
