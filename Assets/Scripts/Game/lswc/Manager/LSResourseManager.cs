using System;
using System.Collections;
using Assets.Scripts.Game.lswc.Control.System;
using Assets.Scripts.Game.lswc.Core;
using Assets.Scripts.Game.lswc.Data;
using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Manager;
using com.yxixia.utile.YxDebug;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Game.lswc.Manager
{
    public class LSResourseManager : InstanceControl
    {

        private static LSResourseManager _instance;

        public static LSResourseManager Instance
        {
            get
            {
                return _instance;
            }
            set { _instance = value; }
        }

        private Dictionary<string, Sprite> _spriteCache;

        private Dictionary<string, Material> _materialCache;

        private LoadState _state = LoadState.BeginLoad;

        public Material[] Materials;

        public Sprite[] Sprites;

        public GameObject[] Prefab;

        public delegate void ResourseLoadState();

        [HideInInspector]
        public ResourseLoadState OnLoadBegin;

        [HideInInspector]
        public ResourseLoadState OnLoad;

        [HideInInspector]
        public ResourseLoadState OnLoadFinished;

        private float beginTime;

        private void Awake()
        {
            _instance = this;
            InitReousrse();
        }

        public void InitReousrse()
        {
            LoadBegin();
            //MusicManager.Instance.LoadAudioBundle(LSConstant.GameKey, "AudioSource");
            //MusicManager.Instance.MusicVolume = 0.1f;
            //MusicManager.Instance.EffectVolume = 1;
            LoadResourse();
        }

        private void LoadBegin()
        {
            beginTime = Time.realtimeSinceStartup;
            if (OnLoadBegin != null)
            {
                OnLoadBegin();
            }
        }

        private void LoadResourse()
        {
            _state = LoadState.OnLoad;
            if (OnLoad != null)
            {
                OnLoad();
            }
            _spriteCache = new Dictionary<string, Sprite>();
            _materialCache = new Dictionary<string, Material>();
            StartCoroutine(LoadSpriteResourse());
            StartCoroutine(LoadMaterialResourse());
            _state = LoadState.FinishLoad;
        }
        #region 本来统一用Object存所有的引用对象，但是考虑到装箱影响效率，又写回来了

        private IEnumerator LoadSpriteResourse()
        {
            foreach (var sprite in Sprites)
            {
                if (_spriteCache.ContainsKey(sprite.name))
                {
                    continue;
                }
                _spriteCache.Add(sprite.name, sprite);
            }
            yield return _spriteCache;
        }

        private IEnumerator LoadMaterialResourse()
        {
            foreach (var material in Materials)
            {
                if (_materialCache.ContainsKey(material.name))
                {
                    continue;
                }
                _materialCache.Add(material.name, material);
            }
            yield return _materialCache;
        }
        #endregion

        private void Update()
        {
            if (_state == LoadState.FinishLoad)
            {
                //Debug.LogWarning("加载总时间是: " + (Time.realtimeSinceStartup - beginTime));
                _state = LoadState.Null;
                //Debug.LogWarning("图片文件数量是: " + _spriteCache.Count);
                //Debug.LogWarning("材质文件数量是: " + _materialCache.Count);
                //Debug.LogWarning("音频文件数量是: " + _audioCache.Count);
                if (OnLoadFinished != null)
                {
                    OnLoadFinished();
                }
            }
        }

        public Material GetMaterial(string name)
        {
            if(_materialCache.ContainsKey(name))
            {
                return _materialCache[name];
            }
            else
            {
                YxDebug.LogError("Material " + name + " is not exist");
                return null;
            }
        }

        public void  PlayBackgroundMusic(string name)
        {
//            MusicManager.Instance.PlayBacksound(name);
        }

        public void PlayVoice(string name)
        {

            MusicManager.Instance.Play(name);
        }

        public Sprite GetSprite(string name)
        {
            if (_spriteCache.ContainsKey(name))
            {
                return _spriteCache[name];
            }
            else
            {
                YxDebug.LogError("Sprite " + name + " is not exist");
                return null;
            }
        }

        public override void OnExit()
        {
            _instance = null;
        }
    }

    public enum ResourseType
    {
        Audio = 0,
        Sprite,
        Material,
    }

    public enum LoadState
    {
        BeginLoad,
        OnLoad,
        FinishLoad,
        Null,
    }

}
