//#if UNITY_EDITOR1//�༭ģʽ�´�д���ݿ�
//#define ENABLE_WRITE_DB//��д�����ݿ�(ֻд��FARM,���Ч��)
//#endif

namespace Assets.Scripts.Game.FishGame.Common.external.NemoFileIO
{
    /// <summary>
    /// �־����ݷ�װ
    /// </summary>
    /// <remarks>��Ҫ�����ǵ�һ�ζ�ȡʱ��ȡӲ��,֮��Ͷ�ȡ�ڴ�,������ÿ�ζ���ȡӲ��</remarks>
    /// <typeparam name="TValType">���ֵ����</typeparam>
    /// <typeparam name="TStoreType">�洢������</typeparam>
    /// <remark>
    /// ע��:
    ///     1.��֤�ļ�����������һ���߳��н���,
    /// </remark>
    public class PersistentData<TValType, TStoreType>  
//where ValType : StoreType ,IEnumerable ,IEnumerator
    {
        private bool _mHaveReaded;//�Ѿ���Ӳ�̶�ȡ��
        private TValType _mVal; 
        public PersistentData(string name)
        {  
            _mVal = Mask(Val);
        }
        public TValType Mask(TValType val)
        {
            if (typeof(TStoreType) == typeof(int))
            {
                return (TValType)(System.Object)((int)(System.Object)(val) ^ 0x5129A9AD);
            }
            return val;
        }

    
        public TValType Val
        {
            get
            {
                if (!_mHaveReaded)//��һ����Ҫ���ļ�
                {
                    _mVal = Mask(default(TValType));
                    _mHaveReaded = true;
                } 
                return Mask(_mVal);//UnMask
            }
            set
            { 
                _mVal = Mask(value); 
            }
        } 


        /// <summary>
        /// ������ֵ������д��Ӳ��
        /// </summary>
        /// <param name="val"></param>
        public void SetImmdiately(TValType val)
        { 
            if ((System.Object)_mVal != (System.Object)Mask(val))
            {
                _mVal = Mask(val);  
            }
        }
    }
}