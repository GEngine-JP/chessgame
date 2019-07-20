using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using YxFramwork.Common;
using YxFramwork.Manager;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.bjl3d
{
    public class GameScene : MonoBehaviour
    {
        public Transform[] ZhuMoveDemos;
        public Transform[] ZhuMaNoMoveDemos;

        /// <summary>
        /// 白名单
        /// </summary>
        public Transform PaiList;
        public Transform CoinList;
        /// <summary>
        /// 下注音效
        /// </summary>
        public AudioClip[] Clips;
        //下注区点击响应
        /// <summary>
        /// 下注区点击响应
        /// </summary>
        public Transform[] BetEffects;
        //下注区圈圈特效
        /// <summary>
        /// 下注去圈圈特效
        /// </summary>
        public Transform[] QqEffs;
        /// <summary>
        /// GameScene脚本
        /// </summary>
        public static GameScene Instance;

        private Animator _girlAnimator;

        /// <summary>
        /// 赢得区域的Effs
        /// </summary>
        public Transform[] WinAreaEffs;
        /// <summary>
        /// 其他玩家筹码
        /// </summary>
        public Transform[] ChouMaWeiBegings;


        private Dictionary<int, int> betEffDic = new Dictionary<int, int>();


        protected void Awake()
        {
            Instance = this;

            Transform tf = transform.Find("Scene/painv_fapai3");
            if (tf == null)
                YxDebug.LogError("No Such Object");

            if (tf != null) _girlAnimator = tf.GetComponent<Animator>();
            if (_girlAnimator == null)
                YxDebug.LogError("No Such Component");
        }

        private bool _isBetAudio = true;
        private int iOtherCoinType = -1;
        /// <summary>
        /// 用户数据
        /// </summary>
        public void UserNoteDataFun()
        {

            if (App.GetGameData<GlobalData>().CurrentUser.Seat == App.GetGameData<GlobalData>().UserSeat)
            {
                CoinMove(App.GetGameData<GlobalData>().P);
                ShowBetEffect(App.GetGameData<GlobalData>().P);
            }
            else
            {
                CoinMove(App.GetGameData<GlobalData>().P, App.GetGameData<GlobalData>().GoldOne, false);
                MusicManager.Instance.Play("AddChip");
                // AudioClip clip = ResourcesLoader.instance.LoadAudio("AddChip");
                //AudioManager.Instance.Play(clip, false, .8f);
            }
        }
        int GetCoinType(long money)
        {
            int index = 0;
            switch (money)
            {
                case 100:
                    index = 0;
                    break;
                case 1000:
                    index = 1;
                    break;
                case 10000:
                    index = 2;
                    break;
                case 100000:
                    index = 3;
                    break;
                case 1000000:
                    index = 4;
                    break;
                case 5000000:
                    index = 5;
                    break;
                case 10000000:
                    index = 6;
                    break;
            }
            return index;
        }

        IEnumerator TimeToBetAudio(float s)
        {
            yield return new WaitForSeconds(s);
            _isBetAudio = true;
        }
        /// <summary>
        /// 选择影响特效
        /// </summary>
        /// <param name="areaId"></param>
        void ShowBetEffect(int areaId)
        {

            if (!betEffDic.ContainsKey(areaId))
            {
                if (!BetEffects[areaId].GetComponent<MeshRenderer>().enabled)
                {
                    BetEffects[areaId].gameObject.SetActive(false);
                    BetEffects[areaId].gameObject.SetActive(true);
                }
                if (!QqEffs[areaId].gameObject.activeSelf)
                    QqEffs[areaId].gameObject.SetActive(true);
                betEffDic.Add(areaId, 1);
            }
            else
            {
                if (!BetEffects[areaId].GetComponent<MeshRenderer>().enabled)
                {
                    BetEffects[areaId].gameObject.SetActive(false);
                    BetEffects[areaId].gameObject.SetActive(true);
                }
            }
        }
        /// <summary>
        /// 硬币移动
        /// </summary>
        /// <param name="pathindex"></param>
        /// <param name="iMoney"></param>
        /// <param name="isSelf"></param>
        void CoinMove(int pathindex, long iMoney = -1, bool isSelf = true)
        {
            Transform tf = transform.FindChild("Plan/Plane" + pathindex);//iAreaID
            if (tf == null)
                return;
            Transform tf2 = tf.FindChild("Bet_" + (UserInfoUI.Instance.GameConfig.CoinType + 1));
            if (tf2 == null)
                return;

            Vector3[] path = iTweenPath.GetPath("CoinPath_" + pathindex);
            if (!isSelf)
            {
                if (iMoney == -1)
                    return;
                GameObject goOther = Instantiate(ZhuMaNoMoveDemos[GetCoinType(iMoney)].gameObject);
                if (goOther != null)
                    goOther.gameObject.SetActive(true);
                if (goOther != null)
                {
                    goOther.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                    goOther.transform.localEulerAngles = new Vector3(0, 0, 0);
                    goOther.transform.position = ChouMaWeiBegings[Random.Range(0, 7)].position;
                    Tweener tw = goOther.transform.DOMove(tf2.position, 0.5f);
                    tw.OnComplete(delegate()
                        {
                            Destroy(goOther);
                        });
                }
                return;
            }

            GameObject go = Instantiate(ZhuMoveDemos[UserInfoUI.Instance.GameConfig.CoinType].gameObject);
            Coin coin = go.GetComponent<Coin>();
            if (coin == null)
                return;
            coin.Init(pathindex);

            Animator ani = go.transform.GetComponent<Animator>();
            if (ani == null)
                YxDebug.LogError("No Such Animator");
            if (ani != null)
            {
                ani.enabled = true;
                ani.Play("coinEff");
            }
            go.SetActive(true);
            go.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            path[0] = ZhuMoveDemos[UserInfoUI.Instance.GameConfig.CoinType].position;
            go.transform.position = path[0];

            Transform tff = PlanScene.Instance.Planes[pathindex];
            if (tff == null)
                return;
            Plan plan = tff.GetComponent<Plan>();

            if (plan.CoinDic.ContainsKey(UserInfoUI.Instance.GameConfig.CoinType + 1))
                path[2] = tf2.position + new Vector3(0f, plan.CoinDic[UserInfoUI.Instance.GameConfig.CoinType + 1] * 0.02f, 0f);
            else
                path[2] = tf2.position;

            Hashtable args = new Hashtable();
            args.Add("path", path);
            //设置类型为线性，线性效果会好一些。
            args.Add("easeType", iTween.EaseType.linear);

            args.Add("speed", 15);
            //是否先从原始位置走到路径中第一个点的位置
            args.Add("movetopath", true);
            //是否让模型始终面朝当面目标的方向，拐弯的地方会自动旋转模型
            //如果你发现你的模型在寻路的时候始终都是一个方向那么一定要打开这个
            args.Add("orienttopath", true);
            args.Add("oncomplete", "ItweenAnimation");
            iTween.MoveTo(go.gameObject, args);
        }
        int GetPahtIndex(string tag)
        {
            if (tag == "BetArea0")
                return 0;
            if (tag == "BetArea1")
                return 1;
            if (tag == "BetArea2")
                return 2;
            if (tag == "BetArea3")
                return 3;
            if (tag == "BetArea4")
                return 4;
            if (tag == "BetArea5")
                return 5;
            if (tag == "BetArea6")
                return 6;
            if (tag == "BetArea7")
                return 7;
            return -1;
        }

        float _tmpTime;
        // Update is called once per frame
        //        protected void Update()
        //        {
        //            if (Input.GetMouseButton(0))
        //            {
        //                if (Time.timeSinceLevelLoad - _tmpTime > 0.2f)
        //                {
        //                    if (UserInfoUI.Instance.GetSelfName == " ")
        //                    {
        //                        return;
        //                    }
        //#if UNITY_ANDROID || UNITY_IPHONE
        //                    foreach ( var touch in Input.touches )
        //                    {
        //                        Ray ray = Camera.main.ScreenPointToRay(touch.position);
        //                        RaycastHit hitInfo;
        //                        if ( Physics.Raycast(ray, out hitInfo) && GameConfig.Instance.iCoinType >= 0 )
        //                        {

        //                            GameObject gameObj = hitInfo.collider.gameObject;
        //                            if ( GameConfig.Instance.IsXiaZhuTime && gameObj.tag.Contains("BetArea") )
        //                            {
        //                                if ( UserInfoUI.Instance.GetSelfMoney < chouma[GameConfig.Instance.iCoinType] )
        //                                {
        //                                    GameUI.Instance.NoteText_Show("金币不足！！！");
        //                                    return;
        //                                }
        //                                if ( GameConfig.Instance.isSelf_Z )
        //                                {
        //                                    GameUI.Instance.NoteText_Show("自己是庄家，不能下注！！！");
        //                                }
        //                                else
        //                                {
        //                                    if ( isCanBet(gameObj.tag) )
        //                                    {
        //                                        if ( isBetAudio )
        //                                        {
        //                                            isBetAudio = false;
        //                                            AudioClip clip = ResourcesLoader.instance.LoadAudio("Bet");
        //                                            AudioManager.Instance.Play(clip, false, .8f);
        //                                            StartCoroutine("TimeToBetAudio", 2.0f);
        //                                        }
        //                                        iAreaID = GetPahtIndex(gameObj.tag);
        //                                        C_S_UserNote node = default(C_S_UserNote);
        //                                        node.byDeskStation = (byte) PersonalData.Instance.UserInfo.LogicSeat;
        //                                        node.iChouMaType = GameConfig.Instance.iCoinType;
        //                                        node.iNoteArea = iAreaID;
        //                                        GameLogicTable.Instance.PlayerBet(node);
        //                                    }
        //                                    else
        //                                    {
        //                                        GameUI.Instance.NoteText_Show("该区域不能下注了！！！");
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //#endif

        //#if UNITY_STANDALONE_WIN
        //                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //                    RaycastHit hitInfo;
        //                    if (Physics.Raycast(ray, out hitInfo) && UserInfoUI.Instance.gameConfig.CoinType >= 0)
        //                    {
        //                        GameObject gameObj = hitInfo.collider.gameObject;
        //                        if (Datastore.Instance.MyxiazhuangTime && gameObj.tag.Contains("BetArea"))
        //                        {
        //                            if (Datastore._Instance.Mymoney < chouma[UserInfoUI.Instance.gameConfig.CoinType])
        //                            {
        //                                GameUI.Instance.NoteText_Show("金币不足！！！");
        //                                return;
        //                            }
        //                            if (UserInfoUI.Instance.gameConfig.IsSelfZ)
        //                            {
        //                                GameUI.Instance.NoteText_Show("自己是庄家，不能下注！！！");
        //                            }
        //                            else
        //                            {
        //                                if (isCanBet(gameObj.tag))
        //                                {
        //                                    if (true)
        //                                    {
        //                                        if (_isBetAudio)
        //                                        {
        //                                            _isBetAudio = false;
        //                                            //                                        AudioClip clip = ResourcesLoader.instance.LoadAudio("Bet");
        //                                            //                                        AudioManager.Instance.Play(clip, false, .8f);
        //                                            StartCoroutine("TimeToBetAudio", 2.0f);
        //                                        }
        //                                        _iAreaId = GetPahtIndex(gameObj.tag);
        //                                        //                                  node.iNoteArea = iAreaID;

        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    GameUI.Instance.NoteText_Show("该区域不能下注了！！！");
        //                                }
        //                            }
        //                        }
        //                    }
        //#endif
        //                    _tmpTime = Time.timeSinceLevelLoad;
        //                }
        //            }

        //        }

        private long[] chouma = { 100, 1000, 10000, 100000, 1000000, 5000000, 10000000 };

        private bool isCanBet(string tag)
        {
            return chouma[UserInfoUI.Instance.GameConfig.CoinType] <= UserInfoUI.Instance.GameConfig.LAreaMaxZhu[GetPahtIndex(tag)];

        }
        /// <summary>
        /// 女孩发送卡片
        /// </summary>
        /// <param name="rOl"></param>
        public void GirlSendCard(int rOl)
        {
            if (rOl == -1)
                return;
            if (_girlAnimator != null)
            {
                if (rOl == 0)
                {
                    _girlAnimator.SetInteger("SendCardType", 2);
                    _girlAnimator.Play("SendCardRight");
                }
                else
                {
                    _girlAnimator.SetInteger("SendCardType", 1);
                    _girlAnimator.Play("SendCardLeft");
                }
                _girlAnimator.SetInteger("SendCardType", 0);
            }
        }

        /// <summary>
        /// 清理扑克牌
        /// </summary>
        public void ClearPai()
        {
            foreach (Transform tf in PaiList)
            {
                if (tf.name.Contains("Pai_"))
                    Destroy(tf.gameObject);
            }
           
            ClearWinAreaEffect();
        }
        /// <summary>
        /// 明确选择的影响
        /// </summary>
        public void ClearBetEffect()
        {
            betEffDic.Clear();

            for (var i = 0; i < 9; i++)
            {
                if (BetEffects[i].gameObject.activeSelf)
                    BetEffects[i].gameObject.SetActive(false);
            }
            for (var i = 0; i < 8; i++)
            {
                if (QqEffs[i].gameObject.activeSelf)
                    QqEffs[i].gameObject.SetActive(false);
            }
        }
        /// <summary>
        /// 明确赢得区域
        /// </summary>
        public void ClearWinAreaEffect()
        {
            for (var i = 0; i < WinAreaEffs.Length; i++)
            {
                if (WinAreaEffs[i].gameObject.activeSelf)
                    WinAreaEffs[i].gameObject.SetActive(false);
            }
        }

    }
}

