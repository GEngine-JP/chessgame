﻿using DG.Tweening;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using YxFramwork.Common;

namespace Assets.Scripts.Game.jsys
{
    public class ModelManager : MonoBehaviour
    {

        public static ModelManager Instance;
        //鱼路径
        public Transform[] PointsTransforms;
        //飞禽路径
        public Transform[] AnimalPointsTransforms;
        //所有鱼父类
        public Transform[] AllFishObjs;
        //所有走兽父类
        public Transform[] AllAnimalObjs;
        //所有飞禽父类
        public Transform[] AllBirdObjs;
        //所有鱼
        public Transform[] AllFishs;
        //所有走兽
        public Transform[] AllAnimals;
        //所有飞禽
        public Transform[] AllBirds;
        //开奖路径
        public Transform[] KaijiangPoints;
        //动物开奖路径
        public Transform[] AnimalkaijiangPoints;
        //动物消失左边点
        public Transform LeftPoint;
        //动物消失右边点
        public Transform RightPoint;
        //所有开奖动物
        public Transform[] RewardAnimals;
        public Transform AnimalWiner;
        public Transform[] AnimalLose;
        public Transform AnimalLookAt;
        public Transform CameraLookAt;
        public Camera AnimalCamera;
        public Camera BirdCamera;
        public Transform CameraTransform;
        public Transform CameraTransformBird;
        public Transform BirdWiner;
        public Transform CameraLookAtBird;
        public GameObject LightGameObject;
        //特效
        public GameObject EffectGameObject;
        public GameObject BirdEffect;
        public GameObject LionEffect;
        public GameObject PandaEffect;
        //动物摄像头路径
        public Transform[] AnimalPath;
        private Vector3[] animalPaths = new Vector3[2];
        //龙卷风
        public Transform WindEffect;
        public Transform HitWindEffect;

        //开奖号码
        private int _luckyNumber;

        public void Awake()
        {
            Instance = this;

        }

        //鲨鱼走到开奖位置
        public void GotoKaiJiang()
        {
            //Debug.Log("显示开奖的位置");

            if (App.GetGameData<GlobalData>().Judge)
            {
                _luckyNumber = App.GetGameData<GlobalData>().EndAnimal;

            }
            else if (App.GetGameData<GlobalData>().Judge == false && (App.GetGameData<GlobalData>().SharkPos == 8 || App.GetGameData<GlobalData>().SharkPos == 9))
            {
                _luckyNumber = App.GetGameData<GlobalData>().SharkPos;
                App.GetGameData<GlobalData>().Judge = true;
            }
            //Pan=!Pan;
            iTween.Stop(RewardAnimals[_luckyNumber].gameObject);
            Hashtable args = new Hashtable();
            //设置类型为线性，线性效果会好一些。
            args.Add("easeType", iTween.EaseType.linear);
            //设置寻路的速度
            // args.Add("speed", 30f);
            args.Add("time", 2f);
            //是否先从原始位置走到路径中第一个点的位置
            args.Add("movetopath", true);
            //是否让模型始终面朝当面目标的方向，拐弯的地方会自动旋转模型
            //如果你发现你的模型在寻路的时候始终都是一个方向那么一定要打开这个
            args.Add("orienttopath", true);


            //设置背景隐藏模型	          
            if (_luckyNumber >= 0 && _luckyNumber < 4)
            {
                //显示陆地,隐藏鱼和鸟模型
                ChangeToForest();
                HaidiManager.Instance.Banjiangtai.SetActive(true);
                //设置开奖动物属性isWiner为true
                GameObject winAnimal = RewardAnimals[_luckyNumber].gameObject;
                winAnimal.GetComponent<Pathfinding>().IsWiner = true;
                //开奖动物到开奖点,非开奖动物到开奖点附近
                AnimalGotoPosition();
                KaiJiang();
            }
            else if (_luckyNumber >= 4 && _luckyNumber < 8)
            {
                args.Add("path", AnimalkaijiangPoints);
                //显示天空,隐藏鱼和动物模型
                ChangeToSky();
                ShowBirdEffect();
                ShowOrHideAnimals(AllBirds, false);
                GameObject winBird = RewardAnimals[_luckyNumber].gameObject;
                winBird.transform.FindChild("body").gameObject.SetActive(true);
                winBird.transform.position = BirdWiner.position;
                winBird.transform.LookAt(BirdCamera.transform);
                winBird.GetComponent<BirdManager>().IsOpen = true;
                LookatBird();
                // SmallfishsBack(allBirdObjs);
                KaiJiang();
            }
            else
            {
                args.Add("path", KaijiangPoints);
                //显示海洋,隐藏动物和鸟模型
                ChangeToHaidi();
                ShowOrHideAnimals(AllFishs, false);
                RewardAnimals[_luckyNumber].FindChild("body").gameObject.SetActive(true);
                RewardAnimals[_luckyNumber].GetComponent<SharkManager>().IsOpen = true;
                iTween.MoveTo(RewardAnimals[_luckyNumber].gameObject, args);
                TurnGroupsManager.Instance.showWinning();
                StartCoroutine(OpenStart());
            }
        }


        //显示或隐藏某种动物
        public void ShowOrHideAnimals(Transform[] tran, bool isShow)
        {
            for (int i = 0; i < tran.Length; i++)
            {
                if (tran[i].gameObject.activeSelf != isShow)
                {
                    tran[i].gameObject.SetActive(isShow);
                }
            }
        }

        private IEnumerator OpenStart()
        {
            yield return new WaitForSeconds(2f);
            WindEffect.gameObject.SetActive(true);
            KaiJiang();
        }

        //播放开奖动画
        public void KaiJiang()
        {
            if (_luckyNumber == 8 || _luckyNumber == 9)
            {
                StartCoroutine(HitWind());
            }
            Animation an = RewardAnimals[_luckyNumber].gameObject.GetComponentInChildren<Animation>();
            if (an)
            {
                an.Play("roar");
            }

            StartCoroutine(Recover());
        }
        //开奖完恢复原来的样子
        public IEnumerator Recover()
        {
            yield return new WaitForSeconds(5);
            //龙卷风隐藏
            WindEffect.gameObject.SetActive(false);
            HitWindEffect.gameObject.SetActive(false);
            if (_luckyNumber >= 4)
            {
                Animation an = RewardAnimals[_luckyNumber].gameObject.GetComponentInChildren<Animation>();
                if (an)
                {
                    an.Play("move");
                }
            }

            for (int i = 0; i < AllFishObjs.Length; i++)
            {

                SharkManager manger = AllFishObjs[i].GetComponent<SharkManager>();
                manger.IsOpen = false;
                manger.CanMove = true;
            }
            ResumeAnimal();
            for (int i = 0; i < AllBirdObjs.Length; i++)
            {

                AllBirdObjs[i].GetComponent<BirdManager>().IsOpen = false;
                AllBirdObjs[i].GetComponent<BirdManager>().CanMove = true;
            }
        }
        private IEnumerator HitWind()
        {
       
            yield return new WaitForSeconds(3.5f);
            HitWindEffect.gameObject.SetActive(true);
            TurnGroupsManager.Instance.HideWinning();

        }
        //切换海底场景
        public void ChangeToHaidi()
        {
            if (GoldSharkGameUIManager.Instance.CanChangeBg)
            {
                GoldSharkGameUIManager.Instance.SetBgSprite(1);
                Instance.LightGameObject.SetActive(true);
                HaidiManager.Instance.SetBgSprite(2);
                ShowOrHideAnimals(AllFishs, true);
                ShowOrHideAnimals(AllAnimals, false);
                ShowOrHideAnimals(AllBirds, false);
            }
        }
        //切换到天空场景
        public void ChangeToSky()
        {
            if (GoldSharkGameUIManager.Instance.CanChangeBg)
            {
                GoldSharkGameUIManager.Instance.SetBgSprite(0);
                Instance.LightGameObject.SetActive(false);
                HaidiManager.Instance.SetBgSprite(1);
                ShowOrHideAnimals(AllFishs, false);
                ShowOrHideAnimals(AllAnimals, false);
                ShowOrHideAnimals(AllBirds, true);
            }
        }
        //切换到森林场景
        public void ChangeToForest()
        {
            if (GoldSharkGameUIManager.Instance.CanChangeBg)
            {
                GoldSharkGameUIManager.Instance.SetBgSprite(0);
                Instance.LightGameObject.SetActive(false);
                HaidiManager.Instance.SetBgSprite(0);
                ShowOrHideAnimals(AllFishs, false);
                ShowOrHideAnimals(AllAnimals, true);
                ShowOrHideAnimals(AllBirds, false);
            }
        }
        //动物开奖
        public void AnimalGotoPosition()
        {
            StopPath();
            Transform[] loseAnimal = new Transform[3];
            int i = 0;
            foreach (Transform animal in AllAnimalObjs)
            {
                if (animal.GetComponent<Pathfinding>().IsWiner)
                {
                    int animalKind = animal.GetComponent<Pathfinding>().AnimalKind;
                    ShowEffect(animalKind);
                    animal.GetComponent<Pathfinding>().ScaleToBig();
                    animal.position = AnimalWiner.position;
                    animal.LookAt(AnimalLookAt);
                }
                else
                {
                    loseAnimal[i] = animal;
                    i++;
                }
            }
            for (int j = 0; j < 3; j++)
            {
                loseAnimal[j].GetComponent<Pathfinding>().ScaleToSmall();
                loseAnimal[j].position = AnimalLose[j].position;
                loseAnimal[j].LookAt(AnimalLookAt);
                Animation an = loseAnimal[j].GetComponentInChildren<Animation>();
                an.Play("rest");
            }
            LookatAnimal();
        }

        private void StopPath()
        {
            foreach (Transform animal in AllAnimalObjs)
            {
                animal.GetComponent<NavMeshAgent>().enabled = false;
                animal.GetComponent<Pathfinding>().ChoosePath = false;
            }
        }

        //动物摄像头看动物
        public void LookatAnimal()
        {
            AnimalCamera.transform.DOPath(animalPaths, 3f);
        }

        private void back()
        {
            LookBack();
        }

        //动物摄像头回来
        public void LookBack()
        {
            Vector3 rotate = new Vector3(15.2f, 0, 0);
            AnimalCamera.transform.DOMove(CameraTransform.position, 2);
            AnimalCamera.transform.DORotate(rotate, 1);

        }

        //鸟摄像头看鸟
        public Vector3 LookatBird()
        {
            Vector3 vec = AnimalCamera.transform.position;
            BirdCamera.transform.DOMove(CameraLookAtBird.position, 2);
            return vec;
        }

        //鸟摄像头回来
        public void LookBirdBack()
        {
            BirdCamera.transform.DOMove(CameraTransformBird.position, 2);
        }

        public void ShowBirdEffect()
        {
            BirdEffect.SetActive(true);
        }

        public void ShowEffect(int num)
        {
            EffectGameObject.SetActive(true);
            if (num == 1)
            {
                LionEffect.SetActive(true);
            }
            if (num == 2)
            {
                PandaEffect.SetActive(true);
            }
        }
        public void HideEffect()
        {
            EffectGameObject.SetActive(false);
            LionEffect.SetActive(false);
            PandaEffect.SetActive(false);
            BirdEffect.SetActive(false);


        }
        //开奖后森林相关恢复
        public void ResumeAnimal()
        {
            LookBack();
            LookBirdBack();
            StartCoroutine(Resumed());
        }

        public IEnumerator Resumed()
        {
            yield return new WaitForSeconds(3);
            HideEffect();
            HaidiManager.Instance.Banjiangtai.SetActive(false);
            for (int i = 0; i < AllAnimalObjs.Length; i++)
            {
                AllAnimalObjs[i].GetComponent<Pathfinding>().ScaleToNomal();
                Animation ani = AllAnimalObjs[i].GetComponentInChildren<Animation>();
                if (ani)
                {
                    ani.Play("move");
                }
                if (AllAnimalObjs[i].GetComponent<Pathfinding>().IsWiner == true)
                {
                    AllAnimalObjs[i].GetComponent<Pathfinding>().IsWiner = false;
                }
                AllAnimalObjs[i].GetComponent<NavMeshAgent>().enabled = true;
                AllAnimalObjs[i].GetComponent<Pathfinding>().ChoosePath = true;
            }
        }

        private void GetAnimalPath()
        {
            for (int i = 0; i < AnimalPath.Length; i++)
            {
                animalPaths[i] = AnimalPath[i].position;
            }
        }

        protected void Start()
        {
            GetAnimalPath();
        }
    }
}
