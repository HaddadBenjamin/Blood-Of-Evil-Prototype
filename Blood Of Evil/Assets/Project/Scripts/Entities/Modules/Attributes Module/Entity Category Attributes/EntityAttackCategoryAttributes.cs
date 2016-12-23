using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace BloodOfEvil.Entities.Modules.Attributes
{
    using Scene;
    using Player;
    using Helpers;
    using Extensions;
    using Player.Services.Language;
    using Player.Modules.Attributes;

    public class EntityAttackCategoryAttributes : AEntityCategoryAttribute
    {
        #region Fields
        protected Transform myTransform;
        protected Animator myAnimator;
        protected bool isAttacking = false;

        private const float MAXIMUM_CHANCE_TO_EVADE = 66.6666f;

        private EntityAttributes attackRange;
        #endregion

        #region Properties
        public bool IsAttacking
        {
            get
            {
                return isAttacking;
            }

            protected set
            {
                isAttacking = value;
            }
        }

        public EntityAttributes AttackRange
        {
            get
            {
                return attackRange;
            }

            set
            {
                attackRange = value;
            }
        }
        #endregion

        #region Constructor
        public EntityAttackCategoryAttributes(AEntityAttributesModule attributeModule) : base(attributeModule)
        {
        }
        #endregion

        #region Virtual Behaviour
        public override void InitialzeAttributes()
        {
            base.GetAttribute(EEntityCategoriesAttributes.Attack, "Damage Percentage").InitializeDefaultPercentage("Percentage Of Damage");
            base.GetAttribute(EEntityCategoriesAttributes.Attack, "Attack Speed Percentage").InitializeDefaultPercentage("Percentage Of Attack Speed");
            base.GetAttribute(EEntityCategoriesAttributes.Attack, "Critical Chance Percentage").Initialize("Percentage Of Critical Chance", 5.0f);
            base.GetAttribute(EEntityCategoriesAttributes.Attack, "Critical Damage Percentage").Initialize("Percentage Of Critical Damage", 100.0f);
            base.GetAttribute(EEntityCategoriesAttributes.Attack, "Dexterity Percentage").InitializeDefaultPercentage("Percentage Of Dexterity");

            base.GetAttribute(EEntityCategoriesAttributes.Attack, "Minimal Damage").Initialize("Minimal Damage", 0.0f, base.GetAttribute(EEntityCategoriesAttributes.Attack, "Damage Percentage"));
            base.GetAttribute(EEntityCategoriesAttributes.Attack, "Maximal Damage").Initialize("Maximal Damage", 0.0f, base.GetAttribute(EEntityCategoriesAttributes.Attack, "Damage Percentage"));
            base.GetAttribute(EEntityCategoriesAttributes.Attack, "Dexterity").Initialize("Dexterity", 15.0f, base.GetAttribute(EEntityCategoriesAttributes.Attack, "Dexterity Percentage"));

            base.GetAttribute(EEntityCategoriesAttributes.Attack, "Attack Range").Initialize("Attack Range", 1.6f);
            base.GetAttribute(EEntityCategoriesAttributes.Attack, "Attack Speed").Initialize("Attack Speed", 1.0f, null, false);

            this.AttackRange.Initialize("Attack Range", 1.85f, null, false);
        }

        public override void CreateCallbacksAttributes()
        {
            this.myTransform = base.attributeModule.transform;
            this.myAnimator = this.myTransform.GetComponent<Animator>();
            this.AttackRange = base.GetAttribute(EEntityCategoriesAttributes.Attack, "Attack Range");

            base.GetAttribute(EEntityCategoriesAttributes.Attack, "Attack Speed Percentage").Current.ValueListener(delegate (float input)
            {
                this.myAnimator.SetFloat(Animator.StringToHash("Attack Speed Percentage"), input * PERCENTAGE_TO_UNIT);
            });
        }

        public virtual bool CanAttackTarget(Transform target)
        {
            return this.IsEnoughtNearToAttackTarget(target) &&
                    this.myTransform.IsFrontToTarget(target);
        }
        #endregion

        #region Public Behaviour
        public bool IsEnoughtNearToAttackTarget(Transform target)
        {
            return Vector3.Distance(target.position, this.myTransform.position) <
                   base.GetAttribute(EEntityCategoriesAttributes.Attack, "Attack Range").Current.Value;
        }

        public float GetDefaultDamage()
        {
            float damage = MathHelper.GenerateRandomBeetweenTwoFloats(
                base.GetAttribute(EEntityCategoriesAttributes.Attack, "Minimal Damage").Current.Value,
                base.GetAttribute(EEntityCategoriesAttributes.Attack, "Maximal Damage").Current.Value);

            return damage;
        }

        public void SetDamagesMinimalAndMaximalThenAttack(Vector2 damage)
        {
            this.SetMinimalAndMaximumDamage(damage);

            this.AttackNearestTargetWithMelee();
        }

        public void SetMinimalAndMaximumDamage(Vector2 damage)
        {
            base.GetAttribute(EEntityCategoriesAttributes.Attack, "Minimal Damage").AtStart.Value = damage.x;
            base.GetAttribute(EEntityCategoriesAttributes.Attack, "Maximal Damage").AtStart.Value = damage.y;
        }

        // Devrait prendre en considération la range du joueur pour savoir si il peut attaquer.
        public void AttackMelee(Transform opponentTransform, bool checkIfCanAttackTarget = true)
        {
            if (null != opponentTransform &&
                checkIfCanAttackTarget ? // permet juste de dire si l'on souhaite tester que l'on peut attaquer la cible (suffisament proche) par exemple.
                    this.CanAttackTarget(opponentTransform) :
                    true)
            {
                AEntityAttributesModule oponentAttributes = SceneServicesContainer.Instance.SceneStateModule.GetTargetAttributesModules(opponentTransform, base.attributeModule.TargetType);
                if (null == oponentAttributes)
                    return;

                float damage = this.GetDefaultDamage();
                bool isCriticalHit = this.IsCriticalHit();
                bool isMissHit = this.IsMissHit(oponentAttributes);

                if (isCriticalHit)
                    damage = this.GetCriticalDamage(damage);

                damage = this.ReduceDamageWithDefenceAndResistances(damage, oponentAttributes);

                if (!oponentAttributes.LifeCategoryAttributes.IsDeath)
                {
                    this.CreateDamageText(opponentTransform, damage, isCriticalHit, isMissHit);

                    if (!isMissHit &&
                        oponentAttributes.LifeCategoryAttributes.EndureDamage(damage))
                    {
                        if (EEntity.Player == base.attributeModule.EntityType &&
                            EEntity.Enemy == base.attributeModule.TargetType)
                        {
                            base.attributeModule.GetAttribute(EEntityCategoriesAttributes.Experience, "Add Experience").AtStart.Value =
                                oponentAttributes.GetAttribute(EEntityCategoriesAttributes.Experience, "Experience").Current.Value;
                        }
                    }
                }
            }
        }

        public void AttackNearestTargetWithMelee(bool checkIfCanAttackTarget = true)
        {
            Transform opponentTransform = SceneServicesContainer.Instance.SceneStateModule.GetNearestTarget(base.attributeModule.TargetType);

            this.AttackMelee(opponentTransform, checkIfCanAttackTarget);
        }

        private void CreateDamageText(Transform oponentTransform, float damage, bool isCritical, bool isMissHit)
        {
            GameObject damageTextPrefab = SceneServicesContainer.Instance.PrefabReferencesService.Get("Damage Text");

            Text damageText = SceneServicesContainer.Instance.PrefabReferencesService.Instantiate(
                "Damage Text",
                oponentTransform.Find("[UI] Damage, Heal, Critical"),
                4.0f).GetComponent<Text>();

            damageText.color = base.attributeModule.TargetType == EEntity.Player ?
                                Color.red :
                                Color.white;

            damageText.text = damage <= 0.0f ?
                "0" :
                    isMissHit ?
                    string.Format("<color=#f1c40f>{0}</color>", PlayerServicesAndModulesContainer.Instance.LanguageService.GetText(ELanguageCategory.AttackAttributes, "Miss !")) :
                        string.Format("{0} {1}",
                            ((int)damage).ToString(),
                            isCritical && base.attributeModule.EntityType == EEntity.Player ?
                            string.Format("<color=red>{0}</color>", PlayerServicesAndModulesContainer.Instance.LanguageService.GetText(ELanguageCategory.AttackAttributes, "Critical !")) :
                                "");

            damageText.GetComponent<Animator>().SetTrigger(
                Animator.StringToHash(
                    isMissHit ?
                    "Miss Hit" :
                        isCritical ?
                        "Critical Hit" :
                        "Normal Hit"
                ));

            damageText.transform.localScale = damageTextPrefab.transform.localScale;
        }

        public float GetCriticalDamage(float damage)
        {
            return damage * (base.GetAttribute(EEntityCategoriesAttributes.Attack, "Critical Damage Percentage").Current.Value * PERCENTAGE_TO_UNIT + 1);
        }

        private bool IsMissHit(AEntityAttributesModule opponentAttributes)
        {
            // 100 - 50 / 50
            float missChance = 100.0f -
                                ((base.GetAttribute(EEntityCategoriesAttributes.Attack, "Dexterity").Current.Value /
                                opponentAttributes.GetAttribute(EEntityCategoriesAttributes.Defence, "Defence").Current.Value) *
                                100.0f);

            return MathHelper.PercentageOfProbabilityToBoolean(missChance > MAXIMUM_CHANCE_TO_EVADE ?
                                                                 MAXIMUM_CHANCE_TO_EVADE :
                                                                 missChance);
        }

        private bool IsCriticalHit()
        {
            return MathHelper.PercentageOfProbabilityToBoolean(base.GetAttribute(EEntityCategoriesAttributes.Attack, "Critical Chance Percentage").Current.Value);
        }

        private float ReduceDamageWithDefenceAndResistances(float damage, AEntityAttributesModule opponentAttributes)
        {
            return damage - opponentAttributes.GetAttribute(EEntityCategoriesAttributes.Defence, "Defence").Current.Value * 0.1f;
        }
        #endregion
    }
}