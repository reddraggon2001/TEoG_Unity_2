﻿using UnityEngine;

namespace EssenceMenu
{
    public class EssenceBoobsButtons : EssenceOrganButtons
    {
        [SerializeField] private AddBoobs addBoobsPrefab = null;
        [SerializeField] private GrowBoobs growBoobsPrefab = null;

        protected override void UpdateButtons()
        {
            transform.KillChildren();
            Instantiate(addBoobsPrefab, transform).Setup(player);
            foreach (Boobs b in player.SexualOrgans.Boobs)
            {
                Instantiate(growBoobsPrefab, transform).Setup(player, b);
            }
        }
    }
}