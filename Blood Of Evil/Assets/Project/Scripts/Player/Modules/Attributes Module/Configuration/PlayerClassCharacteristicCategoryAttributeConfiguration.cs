using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Player.Modules.Attributes.Configuration
{
    [System.Serializable]
    public class PlayerClassCharacteristicCategoryAttributeConfiguration
    {
        #region Fields
        [SerializeField, Header("At Start")]
        private float strengthAtStart;
        [SerializeField]
        private float powerAtStart;
        [SerializeField]
        private float dexterityAtStart;
        [SerializeField]
        private float enduranceAtStart;
        [SerializeField]
        private float chanceAtStart;
        [SerializeField]
        private float faithAtStart;
        [SerializeField]
        private float resistanceAtStart;
        [SerializeField]
        private float wisdomAtStart;
        [SerializeField]
        private float spiritAtStart;
        [SerializeField]
        private float constitutionAtStart;

        [SerializeField, Header("At Level Up")]
        private float strengthAtLevelUp;
        [SerializeField]
        private float powerAtLevelUp;
        [SerializeField]
        private float dexterityAtLevelUp;
        [SerializeField]
        private float enduranceAtLevelUp;
        [SerializeField]
        private float chanceAtLevelUp;
        [SerializeField]
        private float faithAtLevelUp;
        [SerializeField]
        private float resistanceAtLevelUp;
        [SerializeField]
        private float wisdomAtLevelUp;
        [SerializeField]
        private float spiritAtLevelUp;
        [SerializeField]
        private float constitutionAtLevelUp;
        //base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Strength").Initialize("Strength", 6.0f);
        //    base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Power").Initialize("Power", 2.0f);
        //    base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Dexterity").Initialize("Dexterity", 3.0f);
        //    base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Endurance").Initialize("Endurance", 6.0f);
        //    base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Chance").Initialize("Chance", 3.0f);
        //    base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Faith").Initialize("Faith", 2.0f);
        //    base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Resistance").Initialize("Resistance", 4.0f);
        //    base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Wisdom").Initialize("Wisdom", 2.0f);
        //    base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Spirit").Initialize("Spirit", 2.0f);
        //    base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Constitution").Initialize("Constitution", 1.0f);
        #endregion

        #region Properties
        public float StrengthAtStart
        {
            get { return strengthAtStart; }
            private set { strengthAtStart = value; }
        }
        public float PowerAtStart
        {
            get { return powerAtStart; }
            private set { powerAtStart = value; }
        }
        public float DexterityAtStart
        {
            get { return dexterityAtStart; }
            private set { dexterityAtStart = value; }
        }
        public float EnduranceAtStart
        {
            get { return enduranceAtStart; }
            private set { enduranceAtStart = value; }
        }
        public float ChanceAtStart
        {
            get { return strengthAtStart; }
            private set { strengthAtStart = value; }
        }
        public float FaithAtStart
        {
            get { return faithAtStart; }
            private set { faithAtStart = value; }
        }
        public float ResistanceAtStart
        {
            get { return resistanceAtStart; }
            private set { resistanceAtStart = value; }
        }
        public float WisdomAtStart
        {
            get { return wisdomAtStart; }
            private set { wisdomAtStart = value; }
        }
        public float SpiritAtStart
        {
            get { return spiritAtStart; }
            private set { spiritAtStart = value; }
        }
        public float ConstitutionAtStart
        {
            get { return constitutionAtStart; }
            private set { constitutionAtStart = value; }
        }

        public float StrengthAtLevelUp
        {
            get { return strengthAtLevelUp; }
            private set { strengthAtLevelUp = value; }
        }
        public float PowerAtLevelUp
        {
            get { return powerAtLevelUp; }
            private set { powerAtLevelUp = value; }
        }
        public float DexterityAtLevelUp
        {
            get { return dexterityAtLevelUp; }
            private set { dexterityAtLevelUp = value; }
        }
        public float EnduranceAtLevelUp
        {
            get { return enduranceAtLevelUp; }
            private set { enduranceAtLevelUp = value; }
        }
        public float ChanceAtLevelUp
        {
            get { return strengthAtLevelUp; }
            private set { strengthAtLevelUp = value; }
        }
        public float FaithAtLevelUp
        {
            get { return faithAtLevelUp; }
            private set { faithAtLevelUp = value; }
        }
        public float ResistanceAtLevelUp
        {
            get { return resistanceAtLevelUp; }
            private set { resistanceAtLevelUp = value; }
        }
        public float WisdomAtLevelUp
        {
            get { return wisdomAtLevelUp; }
            private set { wisdomAtLevelUp = value; }
        }
        public float SpiritAtLevelUp
        {
            get { return spiritAtLevelUp; }
            private set { spiritAtLevelUp = value; }
        }
        public float ConstitutionAtLevelUp
        {
            get { return constitutionAtLevelUp; }
            private set { constitutionAtLevelUp = value; }
        }
        #endregion
    }
}