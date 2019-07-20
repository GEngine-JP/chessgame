using UnityEngine;
using System.Collections;
using DG.Tweening;
using YxFramwork.Manager;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.brnn3d
{
    public class Pai : MonoBehaviour
    {
        public static Pai Instance;
        private int _area;
        private int _paiIndex;
        private int _paiLun;

        private Animator _paiAni;
        protected void Awake()
        {
            Instance = this;
            Transform tf = transform.Find("default");
            if (tf == null)
                YxDebug.LogError("No Such Object");
            _paiAni = tf.GetComponent<Animator>();
            if (_paiAni == null)
                YxDebug.LogError("No Such Component");
        }
        //显示翻牌后的阶段
        public void Show(int index, int iArea, int _paiLun)
        {
            _area = iArea;
            this._paiLun = _paiLun;
            _paiIndex = index;
            StartCoroutine("WaitToShow", index * 0.58f);
        }

        IEnumerator WaitToShow(float s)
        {
            yield return new WaitForSeconds(s);
            CardMachine.Instance.CardMachinPlay();
            yield return new WaitForSeconds(0.1f);
            Vector3 vor = transform.position;
            transform.position = PaiMode.Instance.PaiFirstTf.position;
            transform.localScale = new Vector3(2.3f, 0.5f, 2.6f);
            Tweener te = transform.DOMove(PaiMode.Instance.PaiSecondTf.position, 0.1f);
            te.OnComplete(delegate()
           {
               transform.position = PaiMode.Instance.PaiSecondTf.position;

               MusicManager.Instance.Play("sendcard");
               Invoke("StopMusic", 5f);
               Tweener tw = transform.DOMove(vor, 0.5f);
               tw.OnComplete(delegate()
               {
                   if (_paiIndex > 23)
                       PaiMode.Instance.FanPaiFun();
               });
           });
        }
        //音乐停止播放
        private void StopMusic()
        {
            MusicManager.Instance.Stop();
        }
        //播放翻牌动画
        public void PlayFanPaiAni()
        {
            _paiAni.Play("fp");
        }

    }
}

