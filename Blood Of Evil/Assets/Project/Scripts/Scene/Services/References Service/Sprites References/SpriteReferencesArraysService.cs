using System;
using UnityEngine;

namespace BloodOfEvil.Scene.Services.References
{
    using Helpers;
    using ObjectInScene;

    public sealed class SpriteReferencesArraysService : AInitializableComponent
    {
        #region Fields
        private SpriteReferences[] allSprites;

        [SerializeField]
        private SpriteReferences languageFlags;
        [SerializeField]
        private SpriteReferences mainMenu;
        #endregion

        #region Abstract Initializer
        public override void Initialize()
        {
            this.allSprites = new SpriteReferences[EnumerationHelper.Count<ESpriteCategory>()];

            this.allSprites[EnumerationHelper.GetIndex<ESpriteCategory>(ESpriteCategory.LanguageFlag)] = this.languageFlags;
            this.allSprites[EnumerationHelper.GetIndex<ESpriteCategory>(ESpriteCategory.MainMenu)] = this.mainMenu;

            Array.ForEach(this.allSprites, stuffsCategory => stuffsCategory.Initialize());
        }
        #endregion

        #region Behaviour Methods
        public Sprite Get(ESpriteCategory spriteCategory, string spriteName)
        {
            return this.allSprites[EnumerationHelper.GetIndex<ESpriteCategory>(spriteCategory)].Get(spriteName);
        }
        #endregion
    }
}