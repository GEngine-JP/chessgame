﻿using UnityEngine;
using System.Linq;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahjongGroupsManager : SceneManagerBase
    {
        public Transform[] EffectposGroup { get; set; }
        public MahjongGroup[] MahjongOther { get; set; }
        public MahjongThrow[] MahjongThrow { get; set; }
        public MahjongHand[] MahjongHandWall { get; set; }
        public MahjongCpgGroup[] MahjongCpgs { get; set; }

        public MahjongWall[] MahjongWall;
        public ObjContainer CpgsContainer;
        public ObjContainer OtherContainer;
        public ObjContainer ThrowContainer;
        public ObjContainer HandWallContainer;
        public ObjContainer EffectposContainer;
        public MahjongSwitchGorup SwitchGorup;

        private int mCurrMahjongWallIndex;
        private int mSendCardBeginIndex;

        public override void OnInitalization()
        {
            MahjongHandWall = HandWallContainer.GetComponent<MahjongHand>(4);
        }

        /// <summary>
        /// 根据玩家人数，设置数据
        /// </summary>      
        public void GetContainer(int count)
        {
            SwitchGorup.Oninit(count);
            MahjongOther = OtherContainer.GetComponent<MahjongGroup>(count);
            MahjongThrow = ThrowContainer.GetComponent<MahjongThrow>(count);
            MahjongCpgs = CpgsContainer.GetComponent<MahjongCpgGroup>(count);
            EffectposGroup = EffectposContainer.GetComponent<Transform>(count);
            MahjongHandWall = HandWallContainer.GetComponent<MahjongHand>(count);
            for (int i = 0; i < MahjongThrow.Length; i++)
            {
                MahjongThrow[i].OnInitalization();
            }
        }

        /// <summary>
        /// 开始发牌位置
        /// </summary>
        public int SendCardBeginIndex
        {
            get { return mSendCardBeginIndex; }
            set { mSendCardBeginIndex = value; }
        }

        /// <summary>
        /// 麻将牌墙的索引
        /// </summary>
        public int CurrMahjongWallIndex
        {
            get { return mCurrMahjongWallIndex; }
            set { mCurrMahjongWallIndex = value; }
        }

        public bool PlayerToken
        {
            set { PlayerHand.HasToken = value; }
            get { return PlayerHand.HasToken; }
        }

        public MahjongPlayerHand PlayerHand
        {
            get
            {
                return (MahjongPlayerHand)MahjongHandWall[0];
            }
        }

        /// <summary>
        /// 当前抓麻将的牌墙
        /// </summary>
        public MahjongWall CurrGetMahjongWall
        {
            get
            {
                int count = MahjongWall[CurrMahjongWallIndex].MahjongList.Count;
                bool flag = MahjongWall[CurrMahjongWallIndex].StartIndex >= count;
                //当是空的时候 是正好完事开始在下一长城拿麻将
                if (flag)
                {
                    int length = MahjongWall.Length;
                    MahjongWall[CurrMahjongWallIndex].SeStartIndext(0);
                    //当是空的时候 是正好完事开始在下一长城拿麻将
                    CurrMahjongWallIndex = (CurrMahjongWallIndex + length - 1) % length;
                    return MahjongWall[CurrMahjongWallIndex];
                }
                else
                {
                    return MahjongWall[CurrMahjongWallIndex];
                }
            }
        }

        /// <summary>
        /// 从当前牌墙中移除一定数量的牌
        /// </summary>   
        public void PopMahFromCurrWall(int count)
        {
            if (count == 0) return;
            MahjongWall curr = CurrGetMahjongWall;
            for (int i = 0; i < count; i++)
            {
                curr.PopMahjong();
                if (curr.StartIndex >= curr.MahjongList.Count)
                {
                    PopMahFromCurrWall(count - (i + 1));
                    break;
                }
            }
        }

        /// <summary>
        /// 从当前牌墙中移除一张牌
        /// </summary>      
        public MahjongContainer PopMahFromCurrWall()
        {
            return CurrGetMahjongWall.PopMahjong();
        }

        /// <summary>
        /// 设置牌墙中牌的个数
        /// </summary>
        public void SetMahjongWallCapacity(int mahjongCount)
        {
            int mjCnt = mahjongCount / MahjongWall.Length;
            //麻将摆的类型 0 每个墙是一样的 1 上下两家多出一摞
            int mjTpye;
            if (mjCnt % 2 == 0)
            {
                mjTpye = 0;
            }
            else
            {
                mjTpye = 1;
                mjCnt -= 1;
            }
            for (int i = 0; i < MahjongWall.Length; i++)
            {
                if (mjTpye == 0)
                {
                    MahjongWall[i].SetRowCnt(mjCnt / 2);
                }
                else if (mjTpye == 1)
                {
                    MahjongWall[i].SetRowCnt(i % 2 == 0 ? (mjCnt / 2) + 1 : mjCnt / 2);
                }
            }
        }

        /// <summary>
        /// 设置发牌位置
        /// </summary>       
        public void SetSendCardSPos(int bankerSeat, int playerSeat)
        {
            var db = GameCenter.DataCenter;
            int chair = (bankerSeat - playerSeat + db.MaxPlayerCount) % db.MaxPlayerCount;
            if (db.MaxPlayerCount == 2)
            {
                chair = chair == 1 ? 2 : chair;
            }
            else if (db.MaxPlayerCount == 3)
            {
                chair = chair == 2 ? 3 : chair;
            }
            CurrMahjongWallIndex = (chair + 2) % 4;
            SendCardBeginIndex = CurrMahjongWallIndex;
        }

        /// <summary>
        /// 设置抓牌开始的位子
        /// </summary>
        /// <param name="saiziPoint"></param>
        public void SetCatchCardStartPos(int[] saiziPoint)
        {
            int point = saiziPoint.Sum() * 2;
            int count = (GameCenter.DataCenter.Room.MahjongCount / 2) / 2;
            if (count > 0)
            {
                while (point >= count)
                {
                    point -= count;
                }
            }
            MahjongWall[CurrMahjongWallIndex].SeStartIndext(point);
            SetMahjongTableIndex();
        }

        /// <summary>
        /// 设置麻将在牌桌中索引
        /// 发牌点前一张牌 index = 0
        /// </summary>
        public void SetMahjongTableIndex()
        {
            MahjongContainer container;
            var startWall = MahjongWall[SendCardBeginIndex];
            int tableIndex = startWall.StartIndex;
            int wallIndex = SendCardBeginIndex;
            wallIndex = wallIndex + 1;
            wallIndex = wallIndex < 4 ? wallIndex : 0;
            OnSetMahjongTableIndex(wallIndex, ref tableIndex);
            tableIndex = startWall.StartIndex - 1;
            for (int i = 0; i < startWall.StartIndex; i++)
            {
                container = startWall[i];
                if (tableIndex % 2 == 0)
                {
                    container.TableSortIndex = tableIndex + 1;
                }
                else
                {
                    container.TableSortIndex = tableIndex - 1;
                }
                --tableIndex;
            }
        }

        public void OnSetMahjongTableIndex(int wallIndex, ref int currTableIndex)
        {
            MahjongContainer container;
            var wall = MahjongWall[wallIndex];
            for (int i = wall.MahjongList.Count - 1; i >= 0; i--)
            {
                container = wall[i];
                if (currTableIndex % 2 == 0)
                {
                    container.TableSortIndex = currTableIndex + 1;
                }
                else
                {
                    container.TableSortIndex = currTableIndex - 1;
                }
                currTableIndex++;
            }
            if (wallIndex != SendCardBeginIndex)
            {
                //下一牌墙
                wallIndex = wallIndex + 1;
                wallIndex = wallIndex < 4 ? wallIndex : 0;
                OnSetMahjongTableIndex(wallIndex, ref currTableIndex);
            }
        }

        /// <summary>
        /// 标记打出的单张牌
        /// </summary>     
        public void OnFlagMahjong(int card)
        {
            for (int i = 0; i < MahjongThrow.Length; i++)
            {
                MahjongThrow[i].SignItemByValueGreen(card);
            }
            MahjongUtility.PlayEnvironmentSound("get");
        }

        /// <summary>
        /// 清除标记牌
        /// </summary>  
        public void OnClearFlagMahjong()
        {
            for (int i = 0; i < MahjongThrow.Length; i++)
            {
                MahjongThrow[i].ReplyItem();
            }
        }

        public override void OnReset()
        {           
            ResetMahjongGroup();
            //设置麻将牌墙
            for (int i = 0; i < MahjongWall.Length; i++)
            {
                MahjongWall[i].AddMahjongToWall();
            }

            SendCardBeginIndex = 0;
            CurrMahjongWallIndex = 0;
        }

        public void ResetMahjongGroup()
        {
            for (int i = 0; i < MahjongWall.Length; i++)
            {
                MahjongWall[i].OnReset();
            }
            for (int i = 0; i < MahjongHandWall.Length; i++)
            {
                MahjongHandWall[i].OnReset();
            }
            for (int i = 0; i < MahjongThrow.Length; i++)
            {
                MahjongThrow[i].OnReset();
            }
            for (int i = 0; i < MahjongCpgs.Length; i++)
            {
                MahjongCpgs[i].OnReset();
            }
            for (int i = 0; i < MahjongOther.Length; i++)
            {
                MahjongOther[i].OnReset();
            }
            for (int i = 0; i < SwitchGorup.SwitchNodes.Length; i++)
            {
                SwitchGorup.SwitchNodes[i].OnReset();
            }
        }

        /// <summary>
        /// 翻宝时移除牌      
        /// </summary>        
        public void OnFanbaoRmoveMahjong(int baoIndex)
        {
            RmoveFanbaoMahjong(baoIndex, SendCardBeginIndex);
        }

        private void RmoveFanbaoMahjong(int baoIndex, int wallIndex)
        {
            MahjongContainer container;
            var wall = MahjongWall[wallIndex];
            for (int i = 0; i < wall.MahjongList.Count; i++)
            {
                container = wall[i];
                if (container.TableSortIndex != MiscUtility.DefValue)
                {
                    if (baoIndex == container.TableSortIndex)
                    {
                        wall.PopFanbaoMahjong(container);
                        return;
                    }
                }
            }
            wallIndex = wallIndex + 1;
            wallIndex = wallIndex < 4 ? wallIndex : 0;
            if (wallIndex != SendCardBeginIndex)
            {
                RmoveFanbaoMahjong(baoIndex, wallIndex);
            }
        }
    }
}