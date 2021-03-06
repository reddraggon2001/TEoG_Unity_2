﻿namespace EssenceMenu
{
    public class GrowVagina : GrowOrgan
    {
        private Essence Femi => player.Essence.Femi;
        private Vagina vagina;

        public void Setup(PlayerMain player, Vagina vagina)
        {
            this.vagina = vagina;
            BaseSetup(player);
        }

        protected override void DisplayCost() => btnText.text = $"{Settings.MorInch(vagina.Size)} {vagina.Cost}Femi";

        protected override void Grow()
        {
            if (Femi.Amount >= vagina.Cost)
            {
                vagina.Grow();
                DisplayCost();
            }
        }
    }
}