using System.Collections;
using UnityEngine;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public enum MahjongColor
    {
        Normal,
        Golden,
        Gray,
        Blue,
    }

    public class MahjongContainer : MonoBehaviour
    {
        protected Quaternion mRotaToAcross = Quaternion.Euler(0, 0, -90);
        protected bool mIsAcross;

        /// <summary>
        /// 麻将牌值
        /// </summary>
        public MahjongCard MahjongCard;
        /// <summary>
        /// 打骰子之后，确定牌再桌上的顺序
        /// </summary>
        public int TableSortIndex = -1;
        /// <summary>
        /// 排序用 先根据牌值 然后 根据index
        /// </summary>
        public int MahjongIndex = -1;

        public MahjongSign Sign { get; protected set; }
        public MouseRoll MouseRoll { get; protected set; }
        public BoxCollider BoxCollider { get; protected set; }
        public UserContorl UserContorl { get; protected set; }
        public MahjongAnimation Tweener { get; protected set; }

        private void Awake()
        {
            Sign = GetComponent<MahjongSign>();
            Tweener = GetComponent<MahjongAnimation>();
        }

        public void OnInitalization()
        {
            Vector3 mahjongSize = MiscUtility.MahjongSize;
        }

        public override string ToString() { return "值:" + Value + "；编号：" + MahjongIndex + " 排序号：" + TableSortIndex; }

        public void OnReset()
        {
            MahjongIndex = -1;
            TableSortIndex = -1;
            SetAllowOffsetStatus(true);
            ChangeToHardLayer(false);
            RemoveMahjongScript();
            ShowNormal();
            mIsAcross = false;
            Lock = false;

            mLaizi = false;
            mTingCard = false;
            mOther = false;
            mNumber = 1;
            Sign.OnReset();
        }

        public void RollUp()
        {
            if (MouseRoll != null)
            {
                MouseRoll.RollUp();
            }
        }

        public void RollDown()
        {
            if (MouseRoll != null)
            {
                MouseRoll.RollDown();
            }
        }

        public void ResetPos()
        {
            if (MouseRoll != null)
            {
                MouseRoll.ResetPos();
            }
        }

        public int Value
        {
            get { return MahjongCard.Value; }
            set
            {
                MahjongCard.Value = value;
                Laizi = GameCenter.DataCenter.IsLaizi(value);
            }
        }

        public virtual void SetMahjongScript()
        {
            if (BoxCollider == null)
            {
                if (gameObject.GetComponent<BoxCollider>())
                {
                    BoxCollider = gameObject.GetComponent<BoxCollider>();
                }
                else
                {
                    BoxCollider = gameObject.AddComponent<BoxCollider>();
                }
                BoxCollider.size = MiscUtility.MahjongSize;
            }

            if (UserContorl == null)
            {
                if (gameObject.GetComponent<UserContorl>())
                {
                    UserContorl = gameObject.GetComponent<UserContorl>();
                }
                else
                {
                    UserContorl = gameObject.AddComponent<UserContorl>();
                }
            }

            if (MouseRoll == null)
            {
                if (gameObject.GetComponent<MouseRoll>())
                {
                    MouseRoll = gameObject.GetComponent<MouseRoll>();
                }
                else
                {
                    MouseRoll = gameObject.AddComponent<MouseRoll>();
                }
                MouseRoll.Target = transform;
            }
        }

        public void RemoveMahjongScript()
        {
            if (MouseRoll != null)
            {
                DestroyImmediate(MouseRoll);
                MouseRoll = null;
            }
            if (UserContorl != null)
            {
                DestroyImmediate(UserContorl);
                UserContorl = null;
            }
            if (BoxCollider != null)
            {
                DestroyImmediate(BoxCollider);
                BoxCollider = null;
            }
        }

        /// <summary>
        /// 赖子
        /// </summary>
        protected bool mLaizi;
        public bool Laizi
        {
            get { return mLaizi; }
            set
            {
                mLaizi = value;
                if (value)
                {
                    Sign.LaiziSign(value);
                }
            }
        }

        /// <summary>
        /// 听牌标记
        /// </summary>
        protected bool mTingCard;
        public bool IsTingCard
        {
            get { return mTingCard; }
            set
            {
                mTingCard = value;
                if (value)
                {
                    Sign.TingSign(value);
                }
                else
                {
                    var item = Sign.GetSign(MahSignType.Ting);
                    if (item != null)
                    {
                        item.SetState(false);
                    }
                }
            }
        }

        /// <summary>
        /// 麻将记牌标记
        /// </summary>
        protected int mNumber = 1;
        public int Number
        {
            get { return mNumber; }
            set
            {
                mNumber = value;
                Sign.SetNumberSign(mNumber);
            }
        }

        /// <summary>
        /// 其他标记
        /// </summary>
        protected bool mOther;
        public void SetOtherSign(Anchor anchor, bool state)
        {
            mOther = state;
            Sign.OtherSign(anchor, state);
        }

        public bool GetOther()
        {
            return mOther;
        }

        public void ShowGray()
        {
            SetMahjongColor(MahjongColor.Gray);
        }

        public void ShowNormal()
        {
            SetMahjongColor(MahjongColor.Normal);
        }

        public void ShowBlue()
        {
            SetMahjongColor(MahjongColor.Blue);
        }

        public void ShowGolden()
        {
            SetMahjongColor(MahjongColor.Golden);
        }

        /// <summary>
        /// 设置麻将颜色
        /// </summary>
        public void SetMahjongColor(MahjongColor skin)
        {
            MahjongCard.SetMahjongColor(skin);
        }

        public MouseRoll Roll { get { return MouseRoll; } }

        public bool IsAcross
        {
            get { return mIsAcross; }
            set
            {
                mIsAcross = value;
                if (value)
                {
                    transform.localRotation = mRotaToAcross;
                }
            }
        }

        public void SetMjRota(float x, float y, float z)
        {
            transform.localRotation = Quaternion.Euler(x, y, z);
        }

        protected bool _lock;
        public bool Lock
        {
            get { return _lock; }
            set
            {
                _lock = value;
                if (_lock)
                {
                    ShowGray();
                }
                else
                {
                    ShowNormal();
                }
            }
        }

        public void SetSelectFlag(bool status)
        {
            if (UserContorl != null)
            {
                UserContorl.mNoSelectFlag = status;
            }
        }

        public void ChangeToHardLayer(bool isHard)
        {
            if (isHard && GameCenter.Instance.GameType == GameType.Normal)
            {
                MahjongUtility.ChangeLayer(transform, 9);
            }
            else
            {
                MahjongUtility.ChangeLayer(transform, 0);
            }
        }

        public void SetAllowOffsetStatus(bool status)
        {
            if (UserContorl != null)
            {
                UserContorl.AllowOffsetStatus = status;
            }
        }

        public void SetThowOutCall(Action<Transform> throwOutCall)
        {
            if (UserContorl != null)
            {
                UserContorl.OnThrowOut = throwOutCall;
            }
        }

        public void RotaTo(Vector3 to, float time, float delayTime = 0, Action callBack = null)
        {
            StartCoroutine(RotoTo(to, time, delayTime, callBack));
        }

        public void RotaTo(Vector3 from, Vector3 to, float time, float delayTime = 0, Action callBack = null)
        {
            transform.localRotation = Quaternion.Euler(from);
            StartCoroutine(RotoTo(to, time, 0.02f, callBack));
        }

        public IEnumerator RotoTo(Vector3 to, float time, float delayTime = 0, Action callBack = null)
        {
            float val = 0;
            float bTime = Time.time;
            Quaternion fqua = transform.localRotation;
            Quaternion tquat = Quaternion.Euler(to);
            while (val < time)
            {
                val = Time.time - bTime;
                float smoothval = val / time;
                transform.localRotation = Quaternion.Lerp(fqua, tquat, smoothval);
                yield return new WaitForFixedUpdate();
            }
            if (callBack != null)
            {
                callBack();
            }
        }
    }
}