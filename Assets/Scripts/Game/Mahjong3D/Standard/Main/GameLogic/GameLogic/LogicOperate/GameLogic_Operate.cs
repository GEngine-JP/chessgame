using System.Collections.Generic;
using YxFramwork.ConstDefine;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [S2CResponseLogic]
    public partial class GameLogic_Operate : AbsGameLogicBase
    {
        private int[] mOpMenu;
        private ContinueTaskContainer mAutoThrow;
        private List<KeyValuePair<int, bool>> mOpMenuCache = new List<KeyValuePair<int, bool>>();

        [S2CResponseHandler(NetworkProtocol.MJOpreateType)]
        public void OnOperate(ISFSObject data)
        {
            if (ParseOperate(data))
            {
                Dispatch();
            }
        }

        public override void OnInit()
        {
            mOpMenu = (int[])System.Enum.GetValues(typeof(OperateMenuType));
        }

        private void Dispatch()
        {
            if (mOpMenuCache.Count > 0)
            {
                GameCenter.EventHandle.Dispatch((int)UIEventProtocol.OperateMenuCtrl, new OpreateMenuArgs() { OpMenu = mOpMenuCache });
            }
        }

        private bool ParseOperate(ISFSObject data)
        {
            mOpMenuCache.Clear();
            if (data.ContainsKey(RequestKey.KeySeat))
            {
                int seat = data.GetInt(RequestKey.KeySeat);
                //不是自己的op
                if (DataCenter.OneselfData.Seat != seat)
                {
                    return false;
                }
            }
            if (data.ContainsKey(ProtocolKey.KeyOp))
            {
                int opMenu = data.GetInt(ProtocolKey.KeyOp);
                DataCenter.OperateMenu = opMenu;
                if (opMenu != 0)
                {
                    //解析按钮列表菜單
                    for (int i = 0; i < mOpMenu.Length; i++)
                    {
                        if (GameUtils.BinaryCheck(mOpMenu[i], opMenu))
                        {
                            mOpMenuCache.Add(new KeyValuePair<int, bool>(mOpMenu[i], true));
                        }
                    }
                }
            }
            else
            {
                DataCenter.OperateMenu = 0;
            }
            //上听自动打牌
            AutoThrowout();
            //听牌
            OnTingCard(data);
            return true;
        }

        private void OnTingCard(ISFSObject data)
        {
            var list = DataCenter.OneselfData.TingList;
            list.Clear();
            //有听菜单的 tinglist
            if (data.ContainsKey("tingout"))
            {
                DataCenter.OneselfData.SetTinglist(data.TryGetIntArray("tingout"));
            }
            //不带听玩法时候 tinglist
            if (data.ContainsKey("tingoutlist"))
            {
                DataCenter.OneselfData.SetTinglist(data.TryGetIntArray("tingoutlist"));
            }
            if (DataCenter.ConfigData.MahjongQuery && MahjongUtility.TingTipCtrl == 0)
            {
                GameCenter.Shortcuts.MahjongQuery.Do(p => p.ShowQueryTipOnOperate(list));
            }
        }

        private void AutoThrowout()
        {
            if (null == mAutoThrow)
            {
                mAutoThrow = ContinueTaskManager.NewTask().AppendFuncTask(() => AutoThrowoutTask());
            }
            if (DataCenter.Config.AiAgency && GameCenter.Shortcuts.CheckState(GameSwitchType.AiAgency))
            {
                if (GameCenter.Shortcuts.AiAgency.Holdup(DataCenter.OperateMenu))
                {
                    if (DataCenter.CurrOpChair == 0)
                    {
                        mAutoThrow.Start();
                    }
                    else
                    {
                        GameCenter.Network.C2S.Custom<C2SCustom>().OnGuo();
                    }
                }
            }
            else if (DataCenter.CurrOpChair == 0 && DataCenter.OneselfData.IsAuto && DataCenter.OperateMenu == 0)
            {
                mAutoThrow.Start();
            }
        }

        private IEnumerator<float> AutoThrowoutTask()
        {
            yield return Config.TimeTingPutCardWait;
            //如果没有补张，可以自动出牌
            if (!GameCenter.Shortcuts.SwitchCombination.IsOpen((int)GameSwitchType.HasBuzhang))
            {
                GameCenter.Network.C2S.ThrowoutCard(DataCenter.GetInMahjong);
            }
        }
    }
}