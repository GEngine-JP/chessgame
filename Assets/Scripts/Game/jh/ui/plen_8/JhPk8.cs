using System;
using System.Collections;
using Assets.Scripts.Game.jh.Public;
using UnityEngine;

namespace Assets.Scripts.Game.jh.ui.plen_8
{
    public class JhPk8 : JhPk
    {

        public JhPkHeadAnimation PkHead;
//        void Update()
//        {
//            if (Input.GetKeyDown(KeyCode.A))
//            {
//                PkHead.Start();
//            }
//
//            if (Input.GetKeyDown(KeyCode.S))
//            {
//                PkHead.Reset();
//            }
//        }

        public override void OnPk(GameObject pk1Obj, GameObject pk2Obj, bool isWin, EventDelegate delDelegate)
        {
            Pos1 = pk1Obj.transform.position;
            Pos2 = pk2Obj.transform.position;
            Pk1Obj = pk1Obj;
            Pk2Obj = pk2Obj;

            JhPlayer player1 = pk1Obj.transform.parent.GetComponent<JhPlayer>();
            if (player1 == null) return;
            JhPlayer player2 = pk2Obj.transform.parent.GetComponent<JhPlayer>();
            if (player2 == null) return;

            JhPkHeadInfo info1 = Pk1.GetComponent<JhPkHeadInfo>();
            if (info1 != null)
            {
                info1.SetInfo(player1.HeadPortrait.GetTexture(),player1.NameLabel.Value,player1.CoinLabel.Value);
            }

            JhPkHeadInfo info2 = Pk2.GetComponent<JhPkHeadInfo>();
            if (info2 != null)
            {
                info2.SetInfo(player2.HeadPortrait.GetTexture(), player2.NameLabel.Value, player2.CoinLabel.Value);
            }

            PkAnm = StartCoroutine(PkAniamtion(isWin, delDelegate));
        }

        public void ShowPk()
        {
            Animation.SetActive(true);
        }

        protected override IEnumerator MoveTo()
        {
            PkHead.Start();
            yield return new WaitForSeconds(0.8f);
        }

        protected override IEnumerator ShowResult(bool isWin)
        {
            if (isWin)
            {
                PkHead.StartWinLost(Pk1.transform.localPosition, Pk2.transform.localPosition);
            }
            else
            {
                PkHead.StartWinLost(Pk2.transform.localPosition, Pk1.transform.localPosition);
            }
            yield return new WaitForSeconds(1.5f);
        }


        public override void ResetAnimation()
        {
            PkHead.Reset();
            PkHead.ResetWinLost();
            Pk1.SetActive(false);
            Pk2.SetActive(false);
            Animation.SetActive(false);
            Gzyz.SetActive(false);
        }

        public override void Reset()
        {
            base.Reset();
            PkHead.Reset();
        }
    }
}
