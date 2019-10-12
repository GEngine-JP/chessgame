using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Common.core
{
    public class Swimmer : MonoBehaviour
    { 
        public int SwimDepth;//��������λ��
        public enum State
        {
            Stop, Swim
        }

        //�¼�
        public delegate void Event_RotateStart(float angle);
        public Event_RotateStart EvtRotateStart;//��ʼ��ת

        public Event_Generic EvtSwimOutLiveArea;//�ο�����������
        //����
        public float BoundCircleRadius = 1F;//��԰�뾶
        private Rect mLiveDimension;//����������,�ڳ�ʼ��ʱ����

        public float Speed = 1F;
        public float RotateSpd = 180F;//�Ƕ�/�� 

        public State CurrentState
        {
            get { return mState; }
            set { mState = value; }
        }

        private Transform mTs;
        private State mState;
        private static readonly float CheckLiveInterval = 0.2F;//����Ƿ񳬳����淶Χʱ��
        private float mCheckLiveRemainTime = 0F;

        private bool mStateRotating = false;//�Ƿ�����ת״̬
        struct RotateData
        {
            public float rotateDir;
            public float angleAbs;
            public float stopRotateRadian;
            public float rotatedAngle;
            public float rotateDelta;
        }
        private RotateData mCurRotateData;
        public void CopyDataTo(Swimmer tar)
        {
            tar.BoundCircleRadius = BoundCircleRadius;
            tar.Speed = Speed;
            tar.RotateSpd = RotateSpd;
            tar.mLiveDimension = mLiveDimension;
            //Prefab����û�м���mLiveDimenesion
            //tar.SetLiveDimension(BoundCircleRadius);
        } 

        void Awake()
        {
            mTs = transform;
            mCheckLiveRemainTime = CheckLiveInterval;
            SetLiveDimension(Defines.ClearFishRadius);
        }

    

        /// <summary>
        /// �Ƿ�����������
        /// </summary>
        /// <returns></returns>
        public bool IsInLiveArea()
        {
            return mLiveDimension.Contains(mTs.position);
        }

        /// <summary>
        /// ©һ����Ϊ��������
        /// </summary>
        /// <returns></returns>
        public bool IsInWorld()
        {
            return GameMain.Singleton.WorldDimension.Contains(mTs.position);
        }

        public void SetLiveDimension(float radiusMulti)
        {
            mLiveDimension.x = GameMain.Singleton.WorldDimension.x - BoundCircleRadius * radiusMulti;
            mLiveDimension.y = GameMain.Singleton.WorldDimension.y - BoundCircleRadius * radiusMulti;
            mLiveDimension.width = GameMain.Singleton.WorldDimension.width + 2F * BoundCircleRadius * radiusMulti;
            mLiveDimension.height = GameMain.Singleton.WorldDimension.height + 2F * BoundCircleRadius * radiusMulti;
        }


        public void Go()
        {
            mState = State.Swim; 
        }

        public void StopImm()
        {
            mState = State.Stop;
        }

        public void Rotate(float angle)
        {
            if (mTs == null)
                mTs = transform;
            StartRotate(angle); 
        } 
      
        void StartRotate(float angle)
        {
            mStateRotating = true;

            if (EvtRotateStart != null)
                EvtRotateStart(angle);

            mCurRotateData.rotateDir = Mathf.Sign(angle);
            mCurRotateData.angleAbs = Mathf.Abs(angle) /**Mathf.Deg2Rad*/;
            mCurRotateData.stopRotateRadian = mCurRotateData.angleAbs * 0.95F;
            mCurRotateData.rotatedAngle = 0F;
            mCurRotateData.rotateDelta = 0F; 
        }

        void Update()
        {
            if (mState == State.Swim)
            {
                mTs.position += Speed * Time.deltaTime * mTs.right;

                if (mStateRotating)
                {
                    mCurRotateData.rotateDelta = RotateSpd * Time.deltaTime * (1F - mCurRotateData.rotatedAngle / mCurRotateData.angleAbs);
                    mTs.rotation *= Quaternion.Euler(0F, 0F, mCurRotateData.rotateDir * mCurRotateData.rotateDelta);
                    mCurRotateData.rotatedAngle += mCurRotateData.rotateDelta;
                    if (mCurRotateData.rotatedAngle > mCurRotateData.stopRotateRadian)
                        mStateRotating = false;
                }
            }

            if (mCheckLiveRemainTime < 0F)
            {
                if (!IsInLiveArea() && EvtSwimOutLiveArea != null)
                    EvtSwimOutLiveArea();

                mCheckLiveRemainTime = CheckLiveInterval;
            }
            mCheckLiveRemainTime -= Time.deltaTime;
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, BoundCircleRadius);
        }
#endif
     
    }
}
