using Assets.Scripts.Game.lswc.Data;
using Assets.Scripts.Game.lswc.Manager;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.lswc.Item
{
    public class LSColorItem : LSItemBase
    {

        public  LSColorType CurColor;

        private MeshRenderer _curRender;

        private void Start()
        {
            _curRender =transform.GetChild(0).GetComponent<MeshRenderer>();
        }

        public void SetColorType(LSColorType type)
        {
            CurColor = type;
            _curRender.material =App.GetGameData<GlobalData>().GetColorMaterial(CurColor);
        }

        public bool IsRightColor(LSColorType type)
        {
            return type == CurColor;
        }

        public override int GetItemsNumber()
        {
            return LSConstant.Num_ColorItemNumber;
        }

       
    }
}
