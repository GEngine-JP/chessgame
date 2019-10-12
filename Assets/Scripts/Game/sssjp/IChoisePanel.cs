﻿using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.sssjp
{
    public interface IChoisePanel
    {
        void Init();
        void ShowChoiseView(ISFSObject cardData);
        void OnClickCard(PokerCard card);
        void OnDragOverCard(PokerCard card);
        void Reset();
        void HideChoiseView();
    }
}