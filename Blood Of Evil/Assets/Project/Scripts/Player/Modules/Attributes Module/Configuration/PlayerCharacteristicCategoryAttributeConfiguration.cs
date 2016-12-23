using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Player.Modules.Attributes.Configuration
{
    //Class at start
    // class on level up
    [System.Serializable]
    public class PlayerCharacteristicCategoryAttributeConfiguration
    {
        #region Fields
        [SerializeField, Header("Strength")]
        private float minimalDamagePerStrength = 2.0f;
        [SerializeField]
        private float maximalDamagePerStrength = 3.0f;
        [SerializeField]
        private float damagePercentagePerStrength = 2.0f;

        [SerializeField, Header("Wisdom")]
        private float energyPerWisdom = 3.0f;

        [SerializeField, Header("Faith")]
        private float minimalHealPerFaith = 1.0f;
        [SerializeField]
        private float maximalHealPerFaith = 2.0f;
        [SerializeField]
        private float healEffectPercentageFaith = 3.0f;

        [SerializeField, Header("Resistance")]
        private float defencePerResistance = 10.0f;
        [SerializeField]
        private float defencePercentagePerResistance = 2.0f;

        [SerializeField, Header("Chance")]
        private float criticalChancePerChance = 0.5f;
        [SerializeField]
        private float chanceToFindRareItemPerChance = 3.0f;

        [SerializeField, Header("Endurance")]
        private float lifePerEndurance = 25.0f;
        [SerializeField]
        private float lifePercentagePerEndurance = 2.0f;

        [SerializeField, Header("Power")]
        private float energyPerPower = 15.0f;
        [SerializeField]
        private float skillEffectPerPower = 2.0f;

        [SerializeField, Header("Dexterity")]
        private float accuracyPerDexterity = 15.0f;
        [SerializeField]
        private float accuracyPercentagePerDexterity = 2.0f;

        [SerializeField, Header("Spririt")]
        private float allResistancePerSpirit = 0.5f;
        [SerializeField]
        private float manaPercentagePerSpirit = 1.5f;

        [SerializeField, Header("Constitution")]
        private float petDamagePercentagePerConstitution = 3.0f;
        [SerializeField]
        private float petLifePercentagePerConstitution = 3.0f;
        #endregion

        #region Properties
        public float MinimalDamagePerStrength
        {
            get { return minimalDamagePerStrength; }
            private set { minimalDamagePerStrength = value; }
        }
        public float MaximalDamagePerStrength
        {
            get { return maximalDamagePerStrength; }
            private set { maximalDamagePerStrength = value; }
        }
        public float DamagePercentagePerStrength
        {
            get { return damagePercentagePerStrength; }
            private set { damagePercentagePerStrength = value; }
        }

        public float EnergyPerWisdom
        {
            get { return energyPerWisdom; }
            private set { energyPerWisdom = value; }
        }

        public float MinimalHealPerFaith
        {
            get { return minimalHealPerFaith; }
            private set { minimalHealPerFaith = value; }
        }
        public float MaximalHealPerFaith
        {
            get { return maximalHealPerFaith; }
            private set { maximalHealPerFaith = value; }
        }
        public float HealEffectPercentageFaith
        {
            get { return healEffectPercentageFaith; }
            private set { healEffectPercentageFaith = value; }
        }

        public float DefencePerResistance
        {
            get { return defencePerResistance; }
            private set { defencePerResistance = value; }
        }
        public float DefencePercentagePerResistance
        {
            get { return defencePercentagePerResistance; }
            private set { defencePercentagePerResistance = value; }
        }

        public float CriticalChancePerChance
        {
            get { return criticalChancePerChance; }
            private set { criticalChancePerChance = value; }
        }
        public float ChanceToFindRareItemPerChance
        {
            get { return chanceToFindRareItemPerChance; }
            private set { chanceToFindRareItemPerChance = value; }
        }

        public float LifePerEndurance
        {
            get { return lifePerEndurance; }
            private set { lifePerEndurance = value; }
        }
        public float LifePercentagePerEndurance
        {
            get { return lifePercentagePerEndurance; }
            private set { lifePercentagePerEndurance = value; }
        }

        public float EnergyPerPower
        {
            get { return energyPerPower; }
            private set { energyPerPower = value; }
        }
        public float SkillEffectPerPower
        {
            get { return skillEffectPerPower; }
            private set { skillEffectPerPower = value; }
        }

        public float AccuracyPerDexterity
        {
            get { return accuracyPerDexterity; }
            private set { accuracyPerDexterity = value; }
        }
        public float AccuracyPercentagePerDexterity
        {
            get { return accuracyPercentagePerDexterity; }
            private set { accuracyPercentagePerDexterity = value; }
        }

        public float AllResistancePerSpirit
        {
            get { return allResistancePerSpirit; }
            private set { allResistancePerSpirit = value; }
        }
        public float ManaPercentagePerSpirit
        {
            get { return manaPercentagePerSpirit; }
            private set { manaPercentagePerSpirit = value; }
        }

        public float PetDamagePercentagePerConstitution
        {
            get { return petDamagePercentagePerConstitution; }
            private set { petDamagePercentagePerConstitution = value; }
        }
        public float PetLifePercentagePerConstitution
        {
            get { return petLifePercentagePerConstitution; }
            private set { petLifePercentagePerConstitution = value; }
        }
        #endregion
    }
}