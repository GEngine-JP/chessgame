using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahjongObject : ObjectBase
    {
        public int Value;
        public MeshRenderer Mesh;
        public SpriteRenderer LaiziFlag;

        public void FlagState(bool statue)
        {
            LaiziFlag.gameObject.SetActive(statue);
            if (statue)
            {
                if (null == LaiziFlag.sprite)
                {
                    LaiziFlag.sprite = GameCenter.Assets.GetSprite("Laizi");
                    //SpriteRenderer sprite = MahjongUtility.GetScriptableAssets<MahjongMiscAssets>().GetAssetComponent<SpriteRenderer>("Laizi");
                    //if (null != sprite)
                    //{
                    //    LaiziFlag.sprite = sprite.sprite;
                    //}
                }
            }
        }

        public void SginFlagState(string sginName)
        {
            if (null != LaiziFlag)
            {
                //SpriteRenderer sprite = MahjongUtility.GetScriptableAssets<MahjongMiscAssets>().GetAssetComponent<SpriteRenderer>(sginName);
                //if (null != sprite)
                //{
                //    LaiziFlag.sprite = sprite.sprite;
                //}
                LaiziFlag.sprite = GameCenter.Assets.GetSprite(sginName);
                LaiziFlag.gameObject.SetActive(true);
            }
        }
    }
}