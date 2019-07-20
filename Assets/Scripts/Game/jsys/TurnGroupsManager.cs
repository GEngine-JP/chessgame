using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Common.Utils;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.jsys
{
    public class TurnGroupsManager : MonoBehaviour
    {
        public static TurnGroupsManager Instance;
        //跑马灯的数组
        public Transform[] paoma;
        //当前图片的位置
        public int CurretImg;
        //当前时间
        private float _curretTimer;
        private int _nImg;

        public GameConfig GameConfig;
        //彩金的text
        public Text CaiJin;
        //彩金的面板
        public Image HandselUI;

        //动物的类型
        private int[] AnimalType =
            {
                8, 3, 3, 3, 9, 7, 7, 7, 8,//9个
                6,6,9,5,5,//5个
                8,4,4,4,9,1,1,1,8,//9个
                2,2,9,0,0//5个
            };

        public void Awake()
        {
            Instance = this;
            GameConfig = new GameConfig();
        }

        /// <summary>
        /// 显示彩金
        /// </summary>
        public void showWinning()
        {
            if (App.GetGameData<GlobalData>().Winning == 0)
            {
                HandselUI.transform.gameObject.SetActive(false);
                return;
            }
            HandselUI.transform.gameObject.SetActive(true);
            CaiJin.text = App.GetGameData<GlobalData>().Winning + "";
        }

        /// <summary>
        /// 隐藏彩金
        /// </summary>
        public void HideWinning()
        {
            HandselUI.transform.gameObject.SetActive(false);
        }
        public bool isWait = true;
        /// <summary>
        /// 游戏开始
        /// </summary>
        /// <param name="data"></param>
        public void PlayGame()
        {
            App.GetGameData<GlobalData>().Judge = true;
            _addTime = 0f;
            // Debug.Log("@@@@@@@@@@@@@开始转圈!!!!!!!!!!!!!!!!!!!!");
            //判断最后位置是编号几的动物
            App.GetGameData<GlobalData>().EndAnimal = AnimalType[App.GetGameData<GlobalData>().EndPos];
            //Debug.Log("最后一个动物的数字" + App.GetGameData<GlobalData>().EndAnimal);
            GameConfig.MarqueeInterval = 0.01f;
            CurretImg = App.GetGameData<GlobalData>().StarPos;
            _nImg = CurretImg;
            if (App.GetGameData<GlobalData>().IsShark)
            {
                GameConfig.TurnTableResult = 28 * 3 + App.GetGameData<GlobalData>().FishIdx;
            }
            if (App.GetGameData<GlobalData>().IsShark == false)
            {
                GameConfig.TurnTableResult = 28 * 3 + App.GetGameData<GlobalData>().EndPos;
            }
            App.GetGameData<GlobalData>().SharkPos = AnimalType[App.GetGameData<GlobalData>().FishIdx];

            if (App.GetGameData<GlobalData>().IsShark && (App.GetGameData<GlobalData>().SharkPos == 8 || App.GetGameData<GlobalData>().SharkPos == 9))
            {
                Debug.Log("");
                App.GetGameData<GlobalData>().EndAnimal = App.GetGameData<GlobalData>().SharkPos;
                AudioPlay.Instance.PlaySounds("Dajiang");
                App.GetGameData<GlobalData>().Judge = false;
                ChangeState();
                paoma[CurretImg].gameObject.SetActive(false);
                isWait = false;
            }
            if (isWait)
            {
                StartCoroutine("Wait");
                paoma[CurretImg].gameObject.SetActive(false);
            }
            isWait = true;
            MusicManager.Instance.Stop();

            AudioPlay.Instance.PlaySounds("Paodeng");

            if (GameConfig.IsBetPanelOnShow)
                BetPanelManager.Instance.HideUI();
        }
        public void PlayS()
        {
            AudioPlay.Instance.PlaySounds("Animal" + App.GetGameData<GlobalData>().SharkPos + "");
        }
        IEnumerator Wait()
        {
            yield return new WaitForSeconds(2f);
            ChangeState();
        }

        /// <summary>
        /// 改变游戏状态
        /// </summary>
        public void ChangeState()
        {
            GameConfig.TurnTableState = (int)GameConfig.GoldSharkState.Marquee;
        }

        private void HidePaoma(int curretImg)
        {
            if (curretImg >= 1)
            {
                paoma[curretImg - 1].gameObject.SetActive(false);
            }
            else if (curretImg == 0)
            {
                paoma[paoma.Length - 1].gameObject.SetActive(false);
            }
        }

        void Paoma(int curretImg)
        {
            paoma[curretImg].gameObject.SetActive(true);
            StartCoroutine("HidePaoma", curretImg);
        }

        /// <summary>
        /// 被调用的方法
        /// </summary>
        public void DiaoYong()
        {
            App.GameData.GStatus = GameStatus.Normal;
            ResultUIManager.Instance.GameFinish();
        }
        private float _addTime;

        private void Update()
        {
            _curretTimer += Time.deltaTime;
            if (_curretTimer > GameConfig.MarqueeInterval && GameConfig.TurnTableState == (int)GameConfig.GoldSharkState.Marquee)
            {
                CurretImg = _nImg % 28;
                if (_nImg == GameConfig.TurnTableResult)
                {
                    Paoma(CurretImg);
                    GameConfig.MarqueeInterval = 0.01f;
                    GameConfig.TurnTableResult = 0;
                    _nImg = 0;
                    _addTime = 0f;
                    GameConfig.TurnTableState = (int)GameConfig.GoldSharkState.Finish;
                    AnimationManager.Instance.ShowAnimation();
                    ModelManager.Instance.GotoKaiJiang();
                    Invoke("DiaoYong", 3f);
                    paoma[0].gameObject.SetActive(false);
                }
                else if (_nImg < GameConfig.TurnTableResult)
                {
                    if (GameConfig.TurnTableResult - _nImg < 28)
                    {
                        GameConfig.MarqueeInterval += _addTime;
                        _addTime += 0.001f;
                    }
                    Paoma(CurretImg);
                    _nImg++;
                    _curretTimer = 0f;
                }
            }
        }

    }

}

