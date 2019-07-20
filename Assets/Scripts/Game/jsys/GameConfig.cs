namespace Assets.Scripts.Game.jsys
{
    public class GameConfig
    {
        /// <summary>
        /// ��Ϸ״̬����
        /// </summary>
        public enum GoldSharkState
        {
            Bet = 0,
            Marquee = 1,
            Finish = 2
        }
        /// <summary>
        /// ��Ϸ״̬����
        /// </summary>
        public int TurnTableState;
        /// <summary>
        /// ��Ϸ���ʱ�������
        /// </summary>
        public int[] Imultiplying= new int[12];

        /// <summary>
        /// �������˸���
        /// </summary>
        public float MarqueeInterval = 0.03f;

        /// <summary>
        ///��Ϸ���(������������) 
        /// </summary>
        public int TurnTableResult = 100;
        
        /// <summary>
        /// bonu ��
        /// </summary>
        public int BonuNumber = 0;

        public bool IsBetPanelOnShow = false;

        /// <summary>
        /// �Ƿ��н���
        /// </summary>
        public bool IsGoldShark = false;       
        /// <summary>
        /// �Ƿ�������
        /// </summary>
        public bool IsSliverShark = false;


    }
}
