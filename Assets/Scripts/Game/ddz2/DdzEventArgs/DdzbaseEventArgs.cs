using System;
using System.Collections.Generic;
using Assets.Scripts.Game.ddz2.InheritCommon;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.ddz2.DdzEventArgs
{
    public class DdzbaseEventArgs : EventArgs
    {
        public readonly ISFSObject IsfObjData;

        public DdzbaseEventArgs(ISFSObject isfData)
        {
            IsfObjData = isfData;
        }


    }
}
