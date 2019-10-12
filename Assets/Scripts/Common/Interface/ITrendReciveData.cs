using System.Collections.Generic;

namespace Assets.Scripts.Common.Interface
{
    public interface ITrendReciveData
    {
        /// <summary>
        /// ���ý���λ��
        /// </summary>
        /// <returns></returns>
        ITrendReciveData SetResultArea();
        /// <summary>
        /// ��ý���λ��
        /// </summary>
        /// <returns></returns>
        List<string> GetResultArea();
        /// <summary>
        /// ��ý���������
        /// </summary>
        /// <returns></returns>
        int GetResultType();
        /// <summary>
        /// ���ͳ�ƵĴ�������
        /// </summary>
        /// <returns></returns>
//        List<int> GetTotalNum();
    }
}

