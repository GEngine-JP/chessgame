﻿/** 
 *文件名称:     TingOutCards.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-04-27 
 *描述:         听牌时，打出哪些牌后进入听状态
 *历史记录: 
*/

using System;
using System.Collections.Generic;
using Assets.Scripts.Game.lyzz2d.Game.Item;
using Assets.Scripts.Game.lyzz2d.Utils;
using Assets.Scripts.Game.lyzz2d.Utils.Single;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.lyzz2d.Game.GameCtrl
{
    public class ShowTingOutInfo : MonoSingleton<ShowTingOutInfo>
    {
        [SerializeField] private UIGrid _grid;

        [SerializeField] private UISprite _paiBg;

        [SerializeField] private GameObject _showParent;

        [SerializeField] private GameObject _totalBg;

        public Action<int> OnClickItemEvent;

        public Action OnClickResetEvevt;

        public override void Awake()
        {
            base.Awake();
            if (_totalBg != null)
            {
                UIEventListener.Get(_totalBg).onClick = OnClickReset;
            }
        }

        public void OnClickReset(GameObject obj)
        {
            if (OnClickResetEvevt != null)
            {
                OnClickResetEvevt();
            }
            OnClose();
        }

        public void Show(bool state)
        {
            _showParent.SetActive(state);
        }

        public void OnClose()
        {
            Show(false);
        }

        public void ShowInfo(List<int> data)
        {
            Show(true);
            data = GameTools.SortCardWithOutLaiZi(data, App.GetGameManager<Lyzz2DGameManager>().LaiZiNum);
            var ChildLenth = _grid.transform.childCount;
            while (ChildLenth > 0)
            {
                DestroyImmediate(_grid.transform.GetChild(0).gameObject);
                ChildLenth--;
            }
            for (int i = 0, lenth = data.Count; i < lenth; i++)
            {
                var item = GameTools.CreateMahjong(data[i], false).GetComponent<MahjongItem>();
                GameTools.DestroyDragObject(item);
                item.SelfData.Action = EnumMahJongAction.StandWith;
                item.SelfData.Direction = EnumMahJongDirection.Vertical;
                item.SelfData.ShowDirection = EnumShowDirection.Self;
                item.SelfData.MahjongLayer = ConstantData.ShowItemLayler;
                UIEventListener.Get(item.gameObject).onClick = OnClickItem;
                GameTools.AddChild(_grid.transform, item.transform);
                var box = item.gameObject.AddComponent<BoxCollider>();
                box.size = new Vector3(item.BGSprite.width, item.BGSprite.height);
            }
            _grid.repositionNow = true;
            _paiBg.width = (int) (_grid.GetChildList().Count*_grid.cellWidth);
        }

        public void OnClickItem(GameObject obj)
        {
            var item = obj.GetComponent<MahjongItem>();
            if (OnClickItemEvent != null)
            {
                OnClickItemEvent(item.Value);
            }
            OnClose();
        }
    }
}