using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Common.core
{
    public static class Defines
    {

        public static int NumPlayer = 6;//�����Ŀ 

        public static int MaxNumPlayer = 20;//��������Ŀ
        public static float TimeBackGroundJumpSelect = 0.5F;//��̨�������� ������� ��� //todo �� ɾ
        public static int NumScoreUpMax = int.MaxValue;//����Ϸ���1�� 

        public static float ClearFishRadius = 1.1F;//���뿪��������ﵽ KillFishRadius*�Լ��İ뾶 �����
        public static float FireSpeed = 0.32f;//����ٶ�
     
        public static Rect WorldDimensionUnit = new Rect(-683F, -384F, 1366F, 768F);//��λ��Ļ����(Unity���絥λ)

        public static int OriginWidthUnit = 1366;
        public static int OriginHeightUnit = 768;

        public static int ScreenWidthUnit = 854;
        public static int ScreenHeightUnit = 480;

        public static int SoundVolumeLevelNum = 16;//�����ֶ��ټ�
        public static int PswLength = 6;//���볤��  todo ��ɾ
        public static float FreezeBombTime = 10F;//����ը������ʱ�� ��λ:��

        public static float OffsetAdv_FishGlobleDepth = 0.05F;//�����ƫ���� todo ��ɾ 

        public static float GlobleDepth_PrepareInBG = 90F;//׼������̨ todo ��ɾ 
        public static float GlobleDepth_GameDataViewer = 100F;//���ݿ���ʾ todo ��ɾ 


        public static float GlobleDepth_ChangeNewScene = 200F;//������ʱ���³��� todo ��ɾ 

        public static float GlobleDepth_DieFishPopDigit = 110F;//�������������� todo ��ɾ 
        public static float GlobleDepth_BombParticle = 300F;//ը������ todo ��ɾ 
        public static float GlobleDepth_LiziKa = 400F;//���ӿ� todo ��ɾ 

        public static float GlobleDepth_DieFish = 420F;//�����㶯�� todo ��ɾ 

        public static float GlobleDepth_PlayerTargeter = 540F;//�����׼��� todo ��ɾ 
        public static float GlobleDepth_Web = 560F;//���� todo ��ɾ 

        public static float GlobleDepth_WaterWave = 580F;//һֱ�ڳ�������Ĳ���Ч�� todo ��ɾ 
        public static float GlobleDepth_SceneSweeper = 590F;//�����л�ʱɨ���Ĳ��� todo ��ɾ 
        public static float GlobleDepth_FishBase = 801F;//�������� todo ��ɾ 

        public static float GlobleDepth_SceneBubblePar = 700;//��������(����) todo ��ɾ 
        public static float GlobleDepth_TempSceneShake = 940F;//��ʱ�𶯴������������ todo ��ɾ 


        public static float GMDepth_Fish = 0;//850F;//����Ҫ�������  todo ��ɾ  
    
    }
    public static class PubFunc
    {
        public static Quaternion QuatRotateZ90 = new Quaternion(0F, 0F, 0.7F, 0.7F);//��Z����ת90��

        /// <summary>
        /// �����תright������ָ����������Ԫ��
        /// </summary> 
        /// <param name="rightTarget"></param>
        /// <returns></returns>
        public static Quaternion RightToRotation(Vector3 rightTarget)
        {
            return Quaternion.LookRotation(Vector3.forward, rightTarget) * QuatRotateZ90;
        }
    }

    public enum Language
    {
        Cn,
        En
    }

    public enum HittableType
    {
        Normal,         //��ͨ��typeIndex:0~49
        AreaBomb,       //��Χը��typeIndex 100~149
        SameTypeBomb,   //ͬ��ը��typeIndex:��Normal��һ��
        FreezeBomb,      //����ը��
        SameTypeBombEx,  //��������ͬ��ը����ը�� 
        Other
    }

    public struct FishOddsData
    {
        public FishOddsData(uint id, int odds) { ID = id; Odds = odds; }
        public uint ID;
        /// <summary>
        /// �������
        /// </summary>
        public int Odds; 
    }

    public delegate void Event_Generic();
    public enum HpyInputKey
    {
        Up, Down, Left, Right, Fire, Lock, Quicken  
    }

    [System.Serializable]
    public class WebScoreScaleRatio
    {
        public WebScoreScaleRatio()
        {
            Scale = PositionScale = /*ScaleCollider =*/ BubbleScale = 1F;
        
        }
        public int StartScore;//��ʼ��ֵ,���ɳ�����һ��ʼ��ֵ
        //public float Size;//����Сunity�е�λ
        public float Scale;//��׼����ֵ-������ֵ
        public float PositionScale;//λ������ֵ
        //public float ScaleCollider;//��ײ����ֵ
        public float BubbleScale;//��������
        public string NameSprite;//��sprite id
        public GameObject PrefabWeb;//��sprite prefab
        public GameObject PrefabWebBoom;//��Ч��,���prefab
    }

    /// <summary>
    /// �ɱ�����
    /// </summary>
    public enum FlyingCoinType
    {
        Sliver,Glod
    }

    /// <summary>
    /// ���ܷ���,��ͨ,����
    /// </summary>
    public enum GunPowerType
    {
        Normal,
        Lizi
    }

    /// <summary>
    /// ��Ϸ�Ѷ�
    /// </summary>
    public enum GameDifficult
    {
        VeryEasy,
        Easy,
        Hard,
        VeryHard,
        DieHard
    }
    /// <summary>
    /// ����������
    /// </summary>
    public enum OutBountyType
    {
        OutCoinButtom,
        OutTicketButtom,
        OutCoinImmediate,
        OutTicketImmediate
    }

    /// <summary>
    /// ��������
    /// </summary>
    public enum ArenaType
    {
        Small,Medium,Larger
    }


    /// <summary>
    ///  ��̨��������(ע��˳��,�еط�ʹ�ø�˳���ж���Ļ����)
    /// </summary>
    public enum GunLayoutType //todo ��ɾ 
    {
        //W10,W8,W6,W4,L4,L3,
        L1, L2, L3, L4, W4, W6, W8, W10,                                                        //����
        S_L6S, S_L6D, S_L8S, S_L8D, S_W8S, S_W8D, S_W12S, S_W16S,S_W20S,                //˫��
        L_L2S, L_L2D, L_L4S, L_L4D, L_L6S, L_L6D, L_L8S, L_L8D, L_W8S, L_W8D, L_W10S, L_W10D, L_W12S, L_W14S, L_W16S//����
    }
}