using System;
using Assets.Scripts.Common.UI;
using Assets.Scripts.Game.jh.EventII;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.jh.ui
{
    public class JhSettingWindow : SettingWindow
    {

        public GameObject Effect;

        public GameObject Sound;


        protected void SpiteVal(GameObject obj,bool value)
        {
            var sprite = obj.GetComponent<UISprite>();
            if (value)
            {
                sprite.spriteName = "ID_182";
            }
            else
            {
                sprite.spriteName = "ID_164";
            }
        }

        protected EventObject EventObj 
        {
            get
            {
                return (EventObject) Data;
            }
        }

        protected override void OnStart()
        {
            base.OnStart();
            var musicMgr = Facade.Instance<MusicManager>();
            SpiteVal(Effect, musicMgr.EffectVolume > 0);
            SpiteVal(Sound, musicMgr.MusicVolume > 0);
        }

        public void OnEffectClick()
        {
            var musicMgr = Facade.Instance<MusicManager>();
            if (musicMgr.EffectVolume > 0)
            {
                musicMgr.EffectVolume = 0;
            }
            else
            {
                musicMgr.EffectVolume = 1;
            }
            SpiteVal(Effect, musicMgr.EffectVolume > 0);
        }

        public void OnSoundClick()
        {
            var musicMgr = Facade.Instance<MusicManager>();
            if (musicMgr.MusicVolume > 0)
            {
                musicMgr.MusicVolume = 0;
            }
            else
            {
                musicMgr.MusicVolume = 1;
            }
            SpiteVal(Sound, musicMgr.MusicVolume > 0);
        }

        public void OnExit()
        {
            EventObj.SendEvent("ServerEvent", "HupReq", 2);
            Hide();
        }

    }
}
