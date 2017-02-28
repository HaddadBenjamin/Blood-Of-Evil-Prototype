using UnityEngine;
using System.Collections;

// 10 Attributs :
//Damage Percentage
//Attack Speed Percentage
//Critical Chance Percentage
//Critical Damage Percentage
//Dexterity Percentage
//Minimal Damage
//Maximal Damage
//Dexterity
//Attack Range
//Attack Speed
namespace BloodOfEvil.Player.Modules.Attributes
{
    using Entities.Modules.Attributes;

    using Services.Keys;
    using Animations;
    using Configuration;

    public sealed class PlayerAttackCategoryAttributes : EntityAttackCategoryAttributes
    {
        #region Fields
        private KeysService keysService;
        //private CanvasesManagerService canvasService;
        private GameObject pathfindingParticuleGameObject;
        private PlayerAnimationModule animationModule;
        #endregion

        #region Constructor
        public PlayerAttackCategoryAttributes(PlayerAttributesModule attributeModule) : base(attributeModule)
        {
        }
        #endregion

        #region Virtual Behaviour
        public override void Update()
        {
            base.IsAttacking =
                this.keysService.IsContiniouslyDown(EPlayerInput.AttackWithoutMove) &&
                !PlayerServicesAndModulesContainer.Instance.CanvasesService.ACanvasIsOpen; // Cette ligne est sale mais il y  a un problème de dépendance.


            if (base.IsAttacking)
            {
                this.pathfindingParticuleGameObject.SetActive(false);
                this.animationModule.SetIsAttackingParameter(base.IsAttacking);
            }
        }
        public override void InitialzeAttributes()
        {
            this.keysService = PlayerServicesAndModulesContainer.Instance.InputService;
            //this.canvasService = PlayerServicesAndModulesContainer.Instance.CanvasesService;
            this.pathfindingParticuleGameObject = PlayerServicesAndModulesContainer.Instance.GameObjectInSceneReferencesService.Get("Pathfinding Particle");
            this.animationModule = PlayerServicesAndModulesContainer.Instance.AnimationModule;
            base.InitialzeAttributes();

            base.GetAttribute(EEntityCategoriesAttributes.Attack, "Attack Speed Percentage").AtStart.Value = 150.0f;
            base.GetAttribute(EEntityCategoriesAttributes.Attack, "Attack Range").Initialize("Attack Range", 1.0f);
        }

        public override void CreateCallbacksAttributes()
        {
            base.CreateCallbacksAttributes();

            //base.attributeModule.GetComponent<CanvasesManagerService>().SubscribeToACanvasIsOpenModifiedCallback(delegate (bool input)
            //{
            //    this.canAttack = !input;
            //});
            PlayerCharacteristicCategoryAttributeConfiguration configuration = PlayerServicesAndModulesContainer.Instance.ConfigurationService.CharacteristicAttributes;

            base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Chance").Current.ValueListener(delegate (float input)
            {
                base.GetAttribute(EEntityCategoriesAttributes.Attack, "Critical Chance Percentage").SetOtherAttribute("Characteristic", input * configuration.CriticalChancePerChance);
            });

            base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Dexterity").Current.ValueListener(delegate (float input)
            {
                base.GetAttribute(EEntityCategoriesAttributes.Attack, "Dexterity").SetOtherAttribute("Characteristic", input * configuration.AccuracyPerDexterity);
                base.GetAttribute(EEntityCategoriesAttributes.Attack, "Dexterity Percentage").SetOtherAttribute("Characteristic", input * configuration.AccuracyPercentagePerDexterity);
            });

            base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Strength").Current.ValueListener(delegate (float input)
            {
                base.GetAttribute(EEntityCategoriesAttributes.Attack, "Minimal Damage").SetOtherAttribute("Characteristic", input * configuration.MinimalDamagePerStrength);
                base.GetAttribute(EEntityCategoriesAttributes.Attack, "Maximal Damage").SetOtherAttribute("Characteristic", input * configuration.MaximalDamagePerStrength);
                base.GetAttribute(EEntityCategoriesAttributes.Attack, "Damage Percentage").SetOtherAttribute("Characteristic", input * configuration.DamagePercentagePerStrength);
            });
        }
        #endregion
    }
}