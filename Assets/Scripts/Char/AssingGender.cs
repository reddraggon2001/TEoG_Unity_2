﻿using System;
using System.Collections.Generic;

namespace EnemyCreatorStuff
{
    public static class AssingGender
    {
        private static readonly Genders[] allGenders = (Genders[])Enum.GetValues(typeof(Genders));
        private static List<Genders> startGenders;

        private static Genders StartGender
        {
            get
            {
                if (startGenders == null)
                {
                    startGenders = new List<Genders>(allGenders);
                    // add some extra of all expect doll, so doll gets very small chance and extra male and female so they are more common that others
                    startGenders.AddRange(new List<Genders>() { Genders.Cuntboy, Genders.Dickgirl, Genders.Female, Genders.Female, Genders.Herm, Genders.Male, Genders.Male });
                }
                return startGenders[rnd.Next(startGenders.Count)];
            }
        }

        private static Random rnd = new Random();

        private static void GenderSwitch(Genders genders, float amount, BasicChar who)
        {
            Essence Masc = who.Essence.Masc, Femi = who.Essence.Femi;
            switch (genders)
            {
                case Genders.Male:
                    Masc.Gain(amount);
                    break;

                case Genders.Female:
                    Femi.Gain(amount);
                    break;

                case Genders.Herm:
                    Masc.Gain(amount / 2);
                    Femi.Gain(amount / 2);
                    break;

                case Genders.Dickgirl:
                    who.SexualOrgans.SetGenderPrefActive = true;
                    who.SexualOrgans.SetGenderPref = Genders.Dickgirl;
                    who.SexualOrgans.Dicks.AddDick();
                    who.SexualOrgans.Boobs.AddBoobs();
                    Masc.Gain(amount / 2);
                    Femi.Gain(amount / 2);
                    break;

                case Genders.Cuntboy:
                    who.SexualOrgans.SetGenderPrefActive = true;
                    who.SexualOrgans.SetGenderPref = Genders.Cuntboy;
                    who.SexualOrgans.Vaginas.AddVag();
                    Femi.Gain(amount);
                    break;

                case Genders.Doll:
                    who.Essence.StableEssence.BaseValue += (int)amount / 2 + 10;
                    Masc.Gain(amount / 2);
                    Femi.Gain(amount / 2);
                    break;

                default:
                    // TODO add stable essence so I can give them essence without is transforming them
                    break;
            }
        }

        /// <summary> Assing a random gender  </summary>
        /// <param name="amount">Total amount of essence to assing</param>
        public static void GetEssense(this BasicChar who, float amount) => GenderSwitch(StartGender, amount, who);

        public static void GetEssense(this BasicChar who, float amount, Genders genderLock) => GenderSwitch(genderLock, amount, who);

        /// <summary> </summary>
        /// <param name="favourGenderType"></param>
        public static void GetEssense(this BasicChar who, float amount, GenderTypes favourGenderType)
        {
            List<Genders> weightedList = new List<Genders>(allGenders);
            weightedList.AddRange(favourGenderType == GenderTypes.Feminine
                ? new List<Genders>() { Genders.Dickgirl, Genders.Female, Genders.Herm, Genders.Female }
                : new List<Genders>() { Genders.Cuntboy, Genders.Male, Genders.Male });
            Genders gender = weightedList[rnd.Next(weightedList.Count)];
            GenderSwitch(gender, amount, who);
        }
    }
}