using System.Collections;
using Assets.Scripts.Game.lswc.Core;
using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Game.lswc.Control.Scene.Manager
{
    /// <summary>
    /// 控制烟花是否播放
    /// </summary>
    public class LSFireWorksControl:InstanceControl
    {
        private static LSFireWorksControl _instance;

        public static LSFireWorksControl Instance
        {
           get {return _instance; }
        }

        public bool BeginState=false;

        private List<GameObject> fireWorks=new List<GameObject>();

        private void Awake()
        {
            _instance = this;
        }

        void Start () 
        {
            for (int i = 0; i <transform.childCount; i++)
            {
                var obj = transform.GetChild(i).gameObject;
                obj.SetActive(BeginState);
                fireWorks.Add(obj);
            }   
        }

        public void Show()
        {
            StartCoroutine(PlayParticalSys());
        }

        private IEnumerator PlayParticalSys()
        {
            foreach (var fireWork in fireWorks)
            {
                fireWork.SetActive(true);
                yield return new WaitForEndOfFrame();
            }
        }

        public void Hide()
        {
            foreach (var fireWork in fireWorks)
            {
                fireWork.SetActive(false);
            }
        }

        public override void OnExit()
        {
           
        }
    }
}
