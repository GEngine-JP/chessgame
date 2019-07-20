﻿using System.Collections.Generic;
using Assets.Scripts.Game.lswc.Control.Scene.Item;
using Assets.Scripts.Game.lswc.Core;
using UnityEngine;

namespace Assets.Scripts.Game.lswc.Control.Scene.Manager
{
    /// <summary>
    /// 晶体管理类，控制所有晶体的状态
    /// </summary>
    public class LSCrystalControl : InstanceControl
    {
        private static LSCrystalControl _instance;

        public static LSCrystalControl Instance
        {
            get { return _instance; }
        }

        private float _durTime = 0;

        [HideInInspector]
        public List<LSCrystalItemControl> _crystalList;

        private bool _isChange = false;

        private float _changeColorTime = 0.5f;

        private float _resetColorTime = 1.0f;

        private void Awake()
        {
            _instance = this;
            _crystalList = new List<LSCrystalItemControl>();
        }

        public void Show(bool change)
        {
            _isChange = change;
            if(!_isChange)
            {
                ChangeCrystalItem(Type_Crystal.NorMal);
            }
        }

        public void AddCrystal(LSCrystalItemControl item)
        {
            if (_crystalList.Contains(item))
            {
                Debug.Log("Item exist,item name is" + item.name);
                return;
            }
            _crystalList.Add(item);
        }

        private void Update()
        {
            if (_isChange)
            {
                _durTime += Time.deltaTime;

                if (_durTime > _resetColorTime)
                {
                    ChangeCrystalItem(Type_Crystal.NorMal);
                    _durTime = 0;
                }
                else if (_durTime > _changeColorTime)
                {
                    ChangeCrystalItem(Type_Crystal.Yellow);
                }

            }

        }
        private void ChangeCrystalItem(Type_Crystal type)
        {
            foreach (LSCrystalItemControl item in _crystalList)
            {
                item.ChangeCrystal(type);
            }
        }

        public void DelCrystal(LSCrystalItemControl item)
        {
            if (!_crystalList.Contains(item))
            {
                Debug.Log("Item exist,item name is" + item.name);
                return;
            }
            _crystalList.Remove(item);
        }

        public override void OnExit()
        {
            _instance = null;
        }
    }
    public enum Type_Crystal
    {
        NorMal = 0,
        Red,
        Green,
        Yellow,
    }
}
