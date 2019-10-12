using Assets.Scripts.Common.UI;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Common.Windows.GpsViews
{
    /// <summary>
    /// 连线版gps
    /// </summary>
    public class GpsLineWindow : YxNguiWindow
    {
        /// <summary>
        /// 线的样式
        /// </summary>
        public NguiLine LinePrefab;

        public Transform LineContainerPrefab;
        public string DisMetreFormat = "相距约{0}米";
        public string DisKmFormat = "相距约{0}千米";

        [Tooltip("线的前缀")]
        public string LinePrefix = "line_";
        [Tooltip("线距离背景的前缀")]
        public string LineTitlePrefix = "linetitle_";
        [Tooltip("正常样式名称")]
        public string LineNormalState = "normal";
        [Tooltip("无数据样式名称")]
        public string LineNoneState = "none";
        [Tooltip("警告样式名称")]
        public string LineWarringState = "warring";
        /// <summary>
        ///  
        /// </summary>
        public GpsItemView[] ItemViews;
        /// <summary>
        /// 线
        /// </summary>
        private NguiLine[] _lines;
        private Transform _lineContainer;

        protected override void OnAwake()
        {
            base.OnAwake();
            _lines = new NguiLine[ItemViews.Length];
            CheckIsStart = true;
        }

        protected override void OnFreshView()
        {
            if (_lineContainer == null)
            {
                YxWindowUtils.CreateItemParent(LineContainerPrefab, ref _lineContainer);
            }
            var gdata = App.GameData;
            if (gdata == null) return;
            var dataCount = ItemViews.Length;
            var lineIndex = 0;

            var lineNormal = string.Format("{0}{1}", LinePrefix, LineNormalState);
            var lineTitleNormal = string.Format("{0}{1}", LineTitlePrefix, LineNormalState);
            var lineNone = string.Format("{0}{1}", LinePrefix, LineNoneState);
            var lineTitleNone = string.Format("{0}{1}", LineTitlePrefix, LineNoneState);
            var lineWarring = string.Format("{0}{1}", LinePrefix, LineWarringState);
            var lineTitleWarring = string.Format("{0}{1}", LineTitlePrefix, LineWarringState);
            var gpsMgr = Facade.GetInterimManager<YxGPSManager>();
            if (gpsMgr != null)
            {
                var warnDistance = gpsMgr.WarnDistance;
                for (var i = 0; i < dataCount; i++)
                {
                    var p1Item = ItemViews[i];
                    if (p1Item == null) continue;
                    var p1Seat = i + 1;
                    p1Item.gameObject.SetActive(true);
                    var p1 = gdata.GetPlayer(p1Seat);
                    var info = p1 == null ? null : p1.Info;
                    p1Item.UpdateView(info);
                    for (var j = i + 1; j < dataCount; j++)//画线
                    {
                        var p2Seat = j + 1;
                        var distance = gpsMgr.GetDistance(p1Seat, p2Seat);
                        var p2Item = ItemViews[j];
                        if (p2Item == null)
                        {
                            continue;
                        }
                        var line = GetLine(lineIndex++);//YxWindowUtils.CreateItem(LinePrefab, _lineContainer);
                        var p1Pos = p1Item.transform.localPosition;
                        var p2Pos = p2Item.transform.localPosition;
                        if (p1Pos.x < p2Pos.x) line.Set(p1Pos, p2Pos, true);
                        else line.Set(p2Pos, p1Pos, true);
                        if (distance<0)
                        {
                            line.SetDistanceLabel("无法获取");
                            line.SetLineSkin(lineNone);
                            line.SetTitleSkin(lineTitleNone);
                            continue;//该座位暂时没人
                        }
                        // Vector2.Distance(p1.Gps, p2.Gps);
                        string disInfo;
                        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                        if (distance <= warnDistance)
                        {
                            disInfo = "距离过近"; //GetDistanceFormat(warnDistance, "距离小于{0}米", "距离小于{0}千米");
                                              //                        line.SetDistanceLabelColor(Color.red);
                            line.SetLineSkin(lineWarring);
                            line.SetTitleSkin(lineTitleWarring);
                        }
                        else
                        {
                            disInfo = GetDistanceFormat(distance, DisMetreFormat, DisKmFormat);
                            //                        line.SetDistanceLabelColor(Color.white);
                            line.SetLineSkin(lineNormal);
                            line.SetTitleSkin(lineTitleNormal);
                        }
                        line.SetDistanceLabel(disInfo);
                    }
                }
            }
        }

        protected string GetDistanceFormat(double distance, string metreFormat, string kmFormat)
        {
            return distance < 1000
                ? string.Format(metreFormat, distance.ToString("0.##"))
                : string.Format(kmFormat, (distance / 1000d).ToString("0.##"));
        }


        /// <summary>
        /// 获取线
        /// </summary>
        /// <returns></returns>
        public NguiLine GetLine(int index)
        {
            var count = _lines.Length;
            if (index >= count) return null;
            var line = _lines[index];
            if (line != null) return line;
            line = YxWindowUtils.CreateItem(LinePrefab, _lineContainer);
            _lines[index] = line;
            return line;
        }
    }
}
