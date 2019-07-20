using Assets.Scripts.Game.ddz2.DDz2Common;
using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Assets.Scripts.Game.ddz2.InheritCommon;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.ddz2.DDzGameListener.OutCdsPanel
{
    /// <summary>
    /// 自己手牌区域
    /// </summary>
    public class SelfOtCdsListener : OutCdsListener
    {
        /// <summary>
        /// 春天例子特效在某个时刻
        /// </summary>
        [SerializeField] protected GameObject ParticalChunTian;
        
        protected override void ClearParticalGob()
        {
            base.ClearParticalGob();
            DestroyImmediate(ParticalChunTian);
        }

        protected override void OnGetLasOutData(int currp,ISFSObject lasOutData)
        {
            var selfSeat = App.GetGameData<GlobalData>().GetSelfSeat;
            if (selfSeat != lasOutData.GetInt(RequestKey.KeySeat) || selfSeat == currp) return;

             AllocateCds(lasOutData.GetIntArray(RequestKey.KeyCards));
             PlayPartical(lasOutData);
        }

        /// <summary>
        /// 如果是自己出牌则出牌
        /// </summary>
        protected override void OnTypeOutCard(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            var seat = data.GetInt(RequestKey.KeySeat);

            var selfSeat = App.GetGameData<GlobalData>().GetSelfSeat;
            if (seat == selfSeat)
            {
                AllocateCds(data.GetIntArray(RequestKey.KeyCards),LandSeat==selfSeat);
                PlayPartical(data);
            }
            else if (seat == App.GetGameData<GlobalData>().GetLeftPlayerSeat)
            {
                ClearAllOutCds();
            }

        }

        /// <summary>
        /// 如果是自己pass则清除自己之前出的牌
        /// </summary>
        protected override void OnTypePass(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            var seat = data.GetInt(RequestKey.KeySeat);
            if (seat == App.GetGameData<GlobalData>().GetSelfSeat ||
                seat == App.GetGameData<GlobalData>().GetLeftPlayerSeat)
            {
                ClearAllOutCds();
            }

        }

        /// <summary>
        /// 当游戏结算时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void OnTypeGameOver(object sender, DdzbaseEventArgs args)
        {
            ShowHandCds(App.GetGameData<GlobalData>().GetSelfSeat,args.IsfObjData.GetSFSArray("users"));

            //判断是否显示春天特效
            if(args.IsfObjData.GetInt(NewRequestKey.KeySpring)<1)return;
            ParticalChunTian.SetActive(false);
            ParticalChunTian.SetActive(true);
            ParticalChunTian.GetComponent<ParticleSystem>().Stop();
            ParticalChunTian.GetComponent<ParticleSystem>().Clear();
            ParticalChunTian.GetComponent<ParticleSystem>().Play();
        }


        protected override void OnUserReady(object sender, DdzbaseEventArgs args)
        {
            base.OnUserReady(sender, args);
            //清理春天特效残留
            ParticalChunTian.SetActive(false);
        }
    }
}
