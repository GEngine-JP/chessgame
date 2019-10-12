using Assets.Scripts.Game.FishGame.Common.core;
using Assets.Scripts.Game.FishGame.Common.Utils;
using Assets.Scripts.Game.FishGame.Fishs;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.FishGame.FishGenereate
{
    /// <summary>
    /// ��������Ĵ�����Χʱ,����ָ��Fish
    /// </summary>
    /// <remarks>
    /// �¼�:
    ///    1.Msg_FishGenerateWhenEnterWorld_Generated(Fish f) ������
    /// ע��:
    ///    1.���ڿ�ͷ����
    ///    2.�������ͬʱɾ������
    /// 
    /// </remarks>
    public class FishGenerateWhenEnterWorld : MonoBehaviour 
    {
        private Fish _prefabFish;
        public int FishIndex;
        public bool IsClearAI = true;
        public float BornDimScaleE = 3F;//todo ��bug,����С��2,�������ɾͻ�ɾ��,��������ƸĽ�

        public delegate void Evt_FishGenerated(Fish f);
        public Evt_FishGenerated EvtFishGenerated;

        private Rect m_BornDim;
        private Transform mTs;
        void Start()
        {
            mTs = transform;
            var main = GameMain.Singleton;
            if (main == null) return;
            var fishGenerator = main.FishGenerator;
            if (fishGenerator == null) return;
            _prefabFish = fishGenerator.GetFishPrefab(FishIndex);
            if (_prefabFish == null) return;
            m_BornDim.x = GameMain.Singleton.WorldDimension.x - _prefabFish.swimmer.BoundCircleRadius * BornDimScaleE;
            m_BornDim.y = GameMain.Singleton.WorldDimension.y - _prefabFish.swimmer.BoundCircleRadius * BornDimScaleE;
            m_BornDim.width = GameMain.Singleton.WorldDimension.width + 2F * _prefabFish.swimmer.BoundCircleRadius * BornDimScaleE;
            m_BornDim.height = GameMain.Singleton.WorldDimension.height + 2F * _prefabFish.swimmer.BoundCircleRadius * BornDimScaleE;
        }

   

        void Update ()
        {
            if (_prefabFish == null) Destroy(gameObject);
            if(m_BornDim.Contains(mTs .position))//������������
            {
                var f = Instantiate(_prefabFish); 
                var swimmer = f.swimmer;
                var fTs = f.transform;
                if (IsClearAI)
                    f.ClearAi();
                swimmer.SetLiveDimension(10000);
                f.AniSprite.playAutomatically = false;
                f.AniSprite.PlayFrom(f.AniSprite.DefaultClip,Time.time);

                f.gameObject.AddComponent<FishDimenSetWhenEnterWorld>();

                fTs.parent = mTs.parent;
                var fishGenrator = GameMain.Singleton.FishGenerator;
                var depth = App.GetGameData<FishGameData>().ApplyFishDepth(swimmer.SwimDepth);
                fTs.localPosition = new Vector3(mTs.localPosition.x, mTs.localPosition.y, depth);
                fTs.localRotation = mTs.localRotation;
                fTs.localScale = mTs.localScale;
                SendMessage("Msg_FishGenerateWhenEnterWorld_Generated",f, SendMessageOptions.DontRequireReceiver);
                if (EvtFishGenerated != null)
                    EvtFishGenerated(f);

                Destroy(gameObject);

            }
        }

        void OnDrawGizmos()
        {
            //Gizmos.DrawIcon(transform.position, "Light Gizmo.tiff", true);
            Gizmos.DrawWireSphere(transform.position, 10);
        }
    }
}
