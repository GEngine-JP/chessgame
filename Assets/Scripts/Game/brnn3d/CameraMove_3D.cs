using System.Collections;
using UnityEngine;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.brnn3d
{
    public class CameraMove_3D : MonoBehaviour
    {
        public static CameraMove_3D Instance;
        protected void Awake()
        {
            Instance = this;
        }

        public CameraPathBezierAnimator[] pathAnimator;

        private CameraPathBezierAnimator playAni;

        //主要包含的就是结算阶段的相关UI
        public void Move()
        {
            StartCoroutine("Wait");
        }
        IEnumerator Wait()
        {
            yield return new WaitForSeconds(8f);

            DownUIController.Instance.DownUILeftUIOn_OffClicked(false);//显示历史纪录的面板
            PaiMode.Instance.History();//游戏历史记录的显示
            ZhongJiangMode.Instance.HideZhongJiangEffect();//隐藏中奖特效
            NiuNumberUI.Instance.HideNiuNumberUI();//隐藏牛数界面
            PaiMode.Instance.DeletPaiList(); //清空牌的列表

            Instance.CameraMoveByPath(1);//3D摄像机移动照大厅
            SettleMentUI.Instance.SetSettleMentUI();//游戏结束计算面板的显示

            UserInfoUI.Instance.SetUserInfoUI();//刷新玩家信息

            SettleMentUI.Instance.HideSettleMentUI();//游戏结算面板的隐藏
            BetMode.Instance.DeletCoinList();  //删除下注金币
            BankerListUI.Instance.DeleteBankerListUI();//更新前先删除

            BankerInfoUI.Instance.SetBankerInfoUIData();//设置庄家的信息
        }

        //相机移动的路径
        public void CameraMoveByPath(int pathID)
        {
            if (playAni != null)
                playAni.Stop();
            playAni = pathAnimator[pathID];
            if (playAni != null)
            {
                playAni.Play();
            }
        }
    }
}

