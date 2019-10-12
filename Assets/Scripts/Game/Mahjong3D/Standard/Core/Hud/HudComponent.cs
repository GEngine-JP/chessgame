using System.Collections.Generic;
using System;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    /// <summary>
    /// 对 AssetBundleInfo 扩展， 添加必要信息
    /// </summary>
    //public class PanelAssetBundleInfo : AssetBundleInfo
    //{
    //    public Type PanelType;
    //}

    public class HudComponent : BaseComponent, ISceneInitCycle, IGameInfoICycle, IGameReadyCycle, IReconnectedCycle, IGameStartCycle, IGameEndCycle, IContinueGameCycle
    {
        /// <summary>
        /// Panel对象缓存
        /// </summary>
        private Dictionary<Type, UIPanelBase> mPanelsManager = new Dictionary<Type, UIPanelBase>();
        /// <summary>
        /// Panel信息缓存
        /// </summary>
        private Dictionary<Type, UIPanelDataAttribute> mPanelTypeMap = new Dictionary<Type, UIPanelDataAttribute>();

        public UIPanelController UIPanelController;

        /// <summary>
        /// 初始化游戏UI
        /// </summary>
        public override void OnInitalization()
        {
            GameCenter.RegisterCycle(this);
            UIPanelController.OnInit();
            UIPanelTypesCollection();
        }

        private void UIPanelTypesCollection()
        {
            var types = GameCenter.Assets.TypeBinder.AllPanelTypes();
            for (int i = 0; i < types.Count; i++)
            {
                var type = types[i];
                var atts = type.GetCustomAttributes(typeof(UIPanelDataAttribute), false);
                if (atts == null || atts.Length == 0) return;
                UIPanelDataAttribute uiDataAtt = atts[0] as UIPanelDataAttribute;
                if (!mPanelTypeMap.ContainsKey(type))
                {
                    mPanelTypeMap.Add(type, uiDataAtt);
                }
            }
        }

        public void OnSceneInitCycle()
        {
            if (GameCenter.Instance.GameType == GameType.Normal)
            {
                var panels = GameCenter.Assets.TypeBinder.DefaultShowPanelTypes();
                AsyncCreatePanel(panels);
            }
            else
            {
                var panels = GameCenter.Assets.TypeBinder.PlaybackShowPanelTypes();
                AsyncCreatePanel(panels);
            }
        }

        private void AsyncCreatePanel(List<Type> panels)
        {
            for (int i = 0; i < panels.Count; i++)
            {
                GetPanel(panels[i]);
            }

            //UIPanelDataAttribute data;
            //var bundleInfos = new List<AssetBundleInfo>();
            //for (int i = 0; i < panels.Count; i++)
            //{
            //    if (mPanelTypeMap.TryGetValue(panels[i], out data))
            //    {
            //        var info = new PanelAssetBundleInfo();
            //        info.Attach = YxFramwork.Common.App.GameKey;
            //        info.Name = "UIPanel" + "/" + data.AssetsBundleName;
            //        info.AssetName = data.AssetsBundleName;
            //        info.PanelType = panels[i];
            //        bundleInfos.Add(info);
            //    }
            //}
            //ResourceManager.LoadAssetListAsync(bundleInfos, (list) =>
            //{
            //    UIPanelDataAttribute panelAtt;
            //    for (int i = 0; i < list.Count; i++)
            //    {
            //        var info = list[i] as PanelAssetBundleInfo;
            //        var obj = Instantiate(info.Asset) as GameObject;
            //        var panel = obj.GetComponent<UIPanelBase>();
            //        if (mPanelTypeMap.TryGetValue(info.PanelType, out panelAtt))
            //        {
            //            panel.OnInit();
            //            SetPanelParent(panel, panelAtt.Hierarchy);
            //            mPanelsManager.Add(info.PanelType, panel);
            //        }
            //    }
            //    OnGameInfoICycle();
            //    if (GameCenter.DataCenter.IsReconect)
            //    {
            //        OnGameReadyCycle();
            //    }
            //    else
            //    {
            //        OnReconnectedCycle();
            //    }
            //});
        }

        public T SetPanelParent<T>(T panel, UIPanelhierarchy hierarchy) where T : UIPanelBase
        {
            if (null == panel) return null;
            RectTransform transform = UIPanelController.UIHierarchy[(int)hierarchy] as RectTransform;
            panel.transform.ExSetParent(transform);
            panel.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
            panel.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            return panel;
        }

        public bool TryGetPanel<Panel>(out Panel panel) where Panel : UIPanelBase
        {
            panel = null;
            Type type = typeof(Panel);
            if (mPanelsManager.ContainsKey(type))
            {
                panel = mPanelsManager[type] as Panel;
                return true;
            }
            return false;
        }

        public UIPanelBase GetPanel(Type type)
        {
            if (mPanelsManager.ContainsKey(type))
            {
                return mPanelsManager[type];
            }
            UIPanelBase panel = null;
            UIPanelDataAttribute info;
            if (mPanelTypeMap.TryGetValue(type, out info))
            {
                panel = CreatePanel<UIPanelBase>(info.AssetsBundleName);
                if (!panel.ExIsNullOjbect())
                {
                    UIPanelController.SetPanel(panel, info.Hierarchy);
                    mPanelsManager.Add(type, panel);
                }
            }
            return panel;
        }

        public Panel GetPanel<Panel>() where Panel : UIPanelBase
        {
            Type type = typeof(Panel);
            return GetPanel(type) as Panel;
            //Type type = typeof(Panel);
            //if (mPanelsManager.ContainsKey(type))
            //{
            //    return mPanelsManager[type] as Panel;
            //}
            //Panel panel = null;
            //UIPanelDataAttribute info;
            //if (mPanelTypeMap.TryGetValue(type, out info))
            //{
            //    panel = CreatePanel<Panel>(info.AssetsBundleName);
            //    if (!panel.ExIsNullOjbect())
            //    {
            //        UIPanelController.SetPanel(panel, info.Hierarchy);
            //        mPanelsManager.Add(type, panel);
            //    }
            //}
            //return panel;
        }

        public void DestoryPanel<Panel>() where Panel : UIPanelBase
        {
            UIPanelBase panel;
            Type type = typeof(Panel);
            if (mPanelsManager.TryGetValue(type, out panel))
            {
                mPanelsManager.Remove(type);
                DestroyImmediate(panel);
            }
        }

        public T CreatePanel<T>(string assetsBundleName) where T : UIPanelBase
        {
            UIPanelBase obj = null;
            obj = GameUtils.CreateUIPanelAsset<T>(assetsBundleName);
            if (obj != null)
            {
                obj.OnInit();
                return obj as T;
            }
            return null;
        }

        public void OpenPanel<Panel>() where Panel : UIPanelBase
        {
            Panel panel = GetPanel<Panel>();
            if (panel != null) { panel.Open(); }
        }

        public void OpenPanel<Panel, T>(T t) where Panel : UIPanelBase
        {
            Panel panel = GetPanel<Panel>();
            IUIPanelControl<T> control = panel as IUIPanelControl<T>;
            if (control != null) { control.Open(t); }
        }

        public void ClosePanel<Panel>() where Panel : UIPanelBase
        {
            Type type = typeof(Panel);
            if (mPanelsManager.ContainsKey(type)) { mPanelsManager[type].Close(); }
        }

        public void OnGameInfoICycle()
        {
            mPanelsManager.ExIterationAction((panel) => { panel.Value.OnGetInfoUpdate(); });
        }

        public void OnGameReadyCycle()
        {
            mPanelsManager.ExIterationAction((panel) => { panel.Value.OnReadyUpdate(); });
        }

        public void OnReconnectedCycle()
        {
            mPanelsManager.ExIterationAction((panel) => { panel.Value.OnReconnectedUpdate(); });
            UIPanelController.RefreshOtherPanelOnReconnected();
        }

        public void OnGameStartCycle()
        {
            mPanelsManager.ExIterationAction((panel) => { panel.Value.OnStartGameUpdate(); });
        }

        public void OnGameEndCycle()
        {
            mPanelsManager.ExIterationAction((panel) => { panel.Value.OnEndGameUpdate(); });
        }

        public void OnContinueGameCycle()
        {
            mPanelsManager.ExIterationAction((panel) => { panel.Value.OnContinueGameUpdate(); });
        }
    }
}