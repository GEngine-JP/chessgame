using System.Collections.Generic;
using Assets.Scripts.Game.ddz2.PokerRule;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.ddz2.PokerCdCtrl
{
    public class OutLogTest : MonoBehaviour
    {
        private static OutLogTest _instance;

        public UILabel LogText;

        void Awake()
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            //在这里做一个Log的监听
            Application.logMessageReceived += HandleLog;


        }

        /*        private void Start()
                {
        /*            var data =
                        PokerRuleUtil.SortCds(new int[] { 72, 75, 97, 54, 29, 42, 28, 76, 22, 60, 58, 79, 38, 43, 39, 51, 41, 77, 45, 27 });
                    string str = "";
                    foreach (var i in data)
                    {
            
                        str += PokerRuleUtil.GetValue(i) + "  ";

                    }
                    YxDebug.LogError(str);#1#

                  //  YxDebug.LogError(PokerRuleUtil.GetCdsType(new int[] { 68, 52, 53, 57, 70, 21, 38, 43, 79, 45, 20, 54, 29, 47, 44, 97, 76, 61, 67, 37 }));
                }*/

        private List<string> _logtxtList = new List<string>();
        void HandleLog(string logString, string stackTrace, LogType type)
        {

            if (type != LogType.Error && type != LogType.Exception) return;

            _logtxtList.Add(type.ToString() + ":" + logString + "  trace: " + stackTrace);
            if (_logtxtList.Count > 3)
            {
                _logtxtList.RemoveAt(0);
            }
            LogText.text = "";
            foreach (var str in _logtxtList)
            {
                LogText.text += str + "\n\n";
            }


        }



    }
}
