using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Adapters;
using YxFramwork.Controller;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Common.UI
{
    /// <summary>
    /// ָ��ͼƬ������
    /// </summary>
    public class AppointTextureLoaderView : YxView
    {
        public const string TextureResPath = "res/textures";
        /// <summary>
        /// ͼƬ����
        /// </summary>
        public string TextureName;

        public ELoadType LoadType;
        /// <summary>
        /// ͼƬ
        /// </summary>
        public YxBaseTextureAdapter TextureAdapter;

        protected override void OnStart()
        {
            base.OnStart();
            //����ͼƬ
            LoadTexture(LoadType);
        }

        public void LoadTexture(ELoadType loadType)
        {
            switch (loadType)
            {
                case ELoadType.Res:
                    var path = TextureResPath.CombinePath(TextureName.Trim());
                    var texture = ImageController.LoadLocalImage(path);//Resources.Load<Texture2D>(TextureName);
                    FinishedLoadTexture(texture, 0);
                    break;
                case ELoadType.Url:
                    Debug.LogError(TextureName);
//                    ImageController.LoadImageFromUrl(TextureName, FinishedLoadTexture);
                    AsyncImage.Instance.GetAsyncImage(TextureName, FinishedLoadTexture);
                    break;
                case ELoadType.Request:
                    ImageController.LoadImageFromServerConfig(TextureName, FinishedLoadTexture);
                    break;
            }
        }

        protected void FinishedLoadTexture(Texture texture,int code)
        {
            if (texture == null) return;
            TextureAdapter.SetTexture(texture);
        }

        public enum ELoadType
        {
            /// <summary>
            /// ��Դ
            /// </summary>
            Res,
            /// <summary>
            /// url
            /// </summary>
            Url,
            /// <summary>
            /// �����ȡurl
            /// </summary>
            Request
        }
    }
}
