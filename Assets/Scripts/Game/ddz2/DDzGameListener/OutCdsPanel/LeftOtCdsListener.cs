using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Assets.Scripts.Game.ddz2.InheritCommon;
using Assets.Scripts.Game.ddz2.PokerRule;
using Sfs2X.Entities.Data;
using YxFramwork.Common;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.ddz2.DDzGameListener.OutCdsPanel
{
    /// <summary>
    /// 自己手牌区域
    /// </summary>
    public class LeftOtCdsListener : OutCdsListener {

        protected override void OnGetLasOutData(int currp,ISFSObject lasOutData)
        {
            var leftSeat = App.GetGameData<GlobalData>().GetLeftPlayerSeat;
            if (leftSeat != lasOutData.GetInt(RequestKey.KeySeat) || leftSeat==currp) return;


            var outCds = lasOutData.GetIntArray(RequestKey.KeyCards);
            AllocateCds(outCds);
            PlayPartical(lasOutData);
        }
        /// <summary>
        /// 如果是自己出牌则出牌
        /// </summary>
        protected override void OnTypeOutCard(object sender ,DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            var seat = data.GetInt(RequestKey.KeySeat);

            var leftSeat = App.GetGameData<GlobalData>().GetLeftPlayerSeat;
            if (seat == leftSeat)
            {
                AllocateCds(data.GetIntArray(RequestKey.KeyCards), LandSeat == leftSeat);
                PlayPartical(data);
            }
            else if (seat == App.GetGameData<GlobalData>().GetRightPlayerSeat)
            {
                ClearAllOutCds();
            }
            
        }

        /// <summary>
        /// 如果是自己pass则情况自己之前出的牌
        /// </summary>
        protected override void OnTypePass(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            var seat = data.GetInt(RequestKey.KeySeat);
            if (seat == App.GetGameData<GlobalData>().GetLeftPlayerSeat ||
                seat == App.GetGameData<GlobalData>().GetRightPlayerSeat)
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
            //ClearAllOutCds();

            ShowHandCds(App.GetGameData<GlobalData>().GetLeftPlayerSeat, args.IsfObjData.GetSFSArray("users"));

        }
    }
}
