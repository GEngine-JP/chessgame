using UnityEngine;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.FishGame.Language
{
    public class SpriteID_LanguageAutoSelect : MonoBehaviour {

        public string[] SpriteNames;

        // Use this for initialization
        private tk2dSprite mSpr;
        void OnEnable()
        {
            mSpr = GetComponent<tk2dSprite>();

            if (mSpr ==null ||SpriteNames == null || SpriteNames.Length == 0)
            {
                YxDebug.LogError("LocalPos_LanguageAutoSelect���������Աδ��ֵ����.");
                Destroy(this);
                return;
            }
            //GameMain.EvtLanguageChange += Handle_LanguageChanged;
            mSpr.spriteId = mSpr.GetSpriteIdByName(SpriteNames[(int)GameMain.Singleton.BSSetting.LaguageUsing.Val]);
        }

        void OnDisable()
        {
            //GameMain.EvtLanguageChange -= Handle_LanguageChanged;
        }
        void Handle_LanguageChanged(global::Assets.Scripts.Game.FishGame.Common.core.Language l)
        {
            //transform.localScale = SpriteNames[(int)GameMain.Singleton.BSSetting.LaguageUsing.Val];
            mSpr.spriteId = mSpr.GetSpriteIdByName(SpriteNames[(int)GameMain.Singleton.BSSetting.LaguageUsing.Val]);
        }
    }
}
