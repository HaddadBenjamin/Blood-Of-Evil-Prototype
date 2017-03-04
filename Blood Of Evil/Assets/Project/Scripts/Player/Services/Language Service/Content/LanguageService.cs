using System;
using UnityEngine;

namespace BloodOfEvil.Player.Services.Language
{
    using Scene;
    using Helpers;
    using ObjectInScene;

    using Utilities.Serialization;
    using Serialization;

    public class LanguageService : ISerializable, IDataInitializable
    {
        #region Fields
        private ELanguage currentLanguage = ELanguage.English;

        public string[][] currentLanguageTexts;
        public string[][] defaultLanguageTexts;
        private int[][] hashIds;

        // Si l'on change de langage, cette action est lancé, ce qui permettra au texte de se remettre à jour.
        public Action NewLanguageHaveBeenLoaded;
        #endregion

        #region Properties
        public ELanguage CurrentLanguage
        {
            get { return currentLanguage; }
            set
            {
                currentLanguage = value;

                // Ne doit pas tester si le language courant à été modifié car le bouton reload de l'interface ne marchera pas.
                // Ne doit pas tester si le language modifié est le courant car pareil l'interface doit se modifier.
                this.LoadLanguage();
            }
        }
        #endregion

        #region Interface Implementation
        // Charge l'énumération du langage choisi. Ne peut pas contenir le contenu de la méthode LoadLanguage qui charge le contenu d'un fichier de langage car 
        // la modification de CurrentLanguage = value provoquerai une boucle infini.
        void ISerializable.Load()
        {
            SerializerHelper.Load<ELanguageSerializable>(
                filename: this.GetLanguageChooseFileName(),
                isReplicatedNextTheBuild: true,
                isEncrypted: false,
                onLoadSuccess: (ELanguageSerializable data) =>
                {
                    this.CurrentLanguage = data.ELanguage;
                });
        }

        // Sauvegarde l'énumération du langage choisi. Ne peut contenir le contenu de la méthode SaveLanguage car le fait de sauvegarder le nom du langage choisi et son contenu son 2 actions différentes.
        void ISerializable.Save()
        {
            SerializerHelper.Save<ELanguageSerializable>(
                filename: this.GetLanguageChooseFileName(),
                dataToSave : new ELanguageSerializable(this.currentLanguage),
                isReplicatedNextTheBuild: true,
                isEncrypted: false);
        }

        void IDataInitializable.Initialize()
        {
            this.currentLanguageTexts = new string[EnumerationHelper.Count<ELanguageCategory>()][];
            this.defaultLanguageTexts = new string[EnumerationHelper.Count<ELanguageCategory>()][];
            this.hashIds = new int[EnumerationHelper.Count<ELanguageCategory>()][];

            this.GenerateDefaultLanguage();
            this.InitializeHashIds();
        }
        #endregion

        #region Public Behaviour
        public void InitializeAtEndFrame()
        {
            ((ISerializable)this).Load();
        }

        public string[] GetCurrentLanguageCategoryTexts(ELanguageCategory languageCategory)
        {
            int categoryIndex = EnumerationHelper.GetIndex(languageCategory);

            if (categoryIndex >= this.currentLanguageTexts.Length)
                Array.Resize(ref this.currentLanguageTexts, this.defaultLanguageTexts.Length);

            return this.currentLanguageTexts[categoryIndex];
        }

        public string[] GetDefaultLanguageCategoryTexts(ELanguageCategory languageCategory)
        {
            return this.defaultLanguageTexts[EnumerationHelper.GetIndex(languageCategory)];
        }

        public int[] GetHashIdsCategory(ELanguageCategory languageCategory)
        {
            return this.hashIds[EnumerationHelper.GetIndex(languageCategory)];
        }

        // Pourrait évidement être plus simple en utilisant 2 Dictionary<string, string> mais je souhaité utiliser les hash ID et les tableaux de données de sorte à utiliser des conteneurs optimisés.
        // De plus j'évite les potentielles erreurs grâce à mes nombreux tests et debug.
        public string GetText(ELanguageCategory category, string text)
        {
            //Debug.LogFormat("The key : {0}, of category : {1}", text, category);
            int categoryIndex = EnumerationHelper.GetIndex<ELanguageCategory>(category);
            int textIndex = ObjectContainerHelper.GetHashCodeIndex(text, this.hashIds[categoryIndex]);
            bool currentLanguageIsNotDefaultLanguage = !this.DoesCurrentLangageIsDefaultLanguage();

            // Si ce n'est pas le langage par défault on tente de renvoyer le langage courant.
            if (currentLanguageIsNotDefaultLanguage &&
                categoryIndex < this.currentLanguageTexts.Length)
            {
                string[] currentLanguagesTextCategory = this.currentLanguageTexts[categoryIndex];

                if (null != currentLanguagesTextCategory)
                {
                    //Debug.LogFormat("category : {0}, index : {1}, text : {2}", category, textIndex, text);
                    if (textIndex < currentLanguagesTextCategory.Length)
                        return currentLanguagesTextCategory[textIndex];
                }
            }

            // Si le langage courant est langage par défault ou que nous avons pas réussi à récupérer le texte du langage courant alors nous renvoyons le texte du langage par défault.
            string[] defaultLanguageCategory = this.defaultLanguageTexts[categoryIndex];

            if (textIndex < defaultLanguageCategory.Length)
                return currentLanguageIsNotDefaultLanguage ?
                        "<color=red>[EMPTY]</color>" :
                        defaultLanguageCategory[textIndex];
            else
                Debug.LogErrorFormat("The key : {0}, of category : {1} is not present in the default language", text, category);

            return null;
        }

        // N'est pas appelé si le langage courant est le langage par défault.
        public void SaveLanguage()
        {
            if (!this.DoesCurrentLangageIsDefaultLanguage())
            {
                SerializerHelper.Save< SerializableStringArrayArray>(
                    filename: this.GetFileName(),
                    dataToSave: new SerializableStringArrayArray(this.currentLanguageTexts),
                    isReplicatedNextTheBuild: true,
                    isEncrypted: false);

                PlayerServicesAndModulesContainer.Instance.TextInformationService.AddTextInformation(
                    string.Format("{0} {1} {2}.",
                        this.GetText(ELanguageCategory.TextInformation, "The language"),
                        this.GetText(ELanguageCategory.Language, EnumerationHelper.EnumerationToString<ELanguage>(this.CurrentLanguage)),
                        this.GetText(ELanguageCategory.TextInformation, "have been saved")));

                // Permet de mettre à jour les textes des interfaces.
                this.NewLanguageHaveBeenLoaded();
            }
        }

        public bool DoesCurrentLangageIsDefaultLanguage()
        {
            return ELanguage.English == this.currentLanguage;
        }

        public void ModifyText(ELanguageCategory category, int textIndex, string text)
        {
            int categoryIndex = EnumerationHelper.GetIndex(category);

            if (null == this.currentLanguageTexts[categoryIndex])
                this.currentLanguageTexts[categoryIndex] = new string[this.defaultLanguageTexts.Length];

            if (textIndex >= this.currentLanguageTexts[categoryIndex].Length)
                Array.Resize(ref this.currentLanguageTexts[categoryIndex], this.defaultLanguageTexts[categoryIndex].Length);

            //Debug.Log(this.currentLanguageTexts[categoryIndex].Length + " " + textIndex);
            this.currentLanguageTexts[categoryIndex][textIndex] = text;
        }
        #endregion

        #region Intern Behaviour
        private string GetFileName()
        {
            return FileSystemHelper.CombinePath(
                SceneServicesContainer.Instance.FileSystemConfiguration.LanguageSettingsDirectoryName,
                EnumerationHelper.EnumerationToString(this.CurrentLanguage));
        }

        private string GetLanguageChooseFileName()
        {
            return SceneServicesContainer.Instance.FileSystemConfiguration.LanguageSettingLanguageChooseFileName;
        }

        private void InitializeHashIds()
        {
            for (int hashIdLanguageCategoryIndex = 0; hashIdLanguageCategoryIndex < EnumerationHelper.Count<ELanguageCategory>(); hashIdLanguageCategoryIndex++)
            {
                if (null != this.defaultLanguageTexts[hashIdLanguageCategoryIndex])
                {
                    ObjectContainerHelper.InitializeHashIds
                        (this.defaultLanguageTexts[hashIdLanguageCategoryIndex],
                        ref this.hashIds[hashIdLanguageCategoryIndex]);

                    if (EnumerationHelper.EnumerationIntegerIndexToString<ELanguage>(hashIdLanguageCategoryIndex) == "Language")
                    {
                        var hashs = this.hashIds[hashIdLanguageCategoryIndex];

                        for (int i = 0; i < this.defaultLanguageTexts[hashIdLanguageCategoryIndex].Length; i++)
                            Debug.LogFormat("Text : {0}, Hash : {1}", this.defaultLanguageTexts[hashIdLanguageCategoryIndex][i], hashs[i]);
                    }
                }
            }
        }

        // Méthode en private car elle est automatiquement appelé par la méthode de changement de langage.
        // Cette méthode n'est pas appelé si le langage courant est le langage par défault
        private void LoadLanguage()
        {
            // Nettoyage du contenu de notre conteneur de langue courante.
            for (int categoryIndex = 0; categoryIndex < this.currentLanguageTexts.Length; categoryIndex++)
            {
                if (null != this.currentLanguageTexts[categoryIndex])
                    Array.Clear(this.currentLanguageTexts[categoryIndex], 0, this.currentLanguageTexts[categoryIndex].Length);
            }

            SerializerHelper.Load< SerializableStringArrayArray>(
                filename: this.GetFileName(),
                isReplicatedNextTheBuild: true,
                isEncrypted: false,
                onLoadSuccess: (SerializableStringArrayArray data) =>
                {
                    data.StringArrayArrayToSerializableStringArrayArray(ref this.currentLanguageTexts);

                    PlayerServicesAndModulesContainer.Instance.TextInformationService.AddTextInformation(
                        string.Format("{0} {1} {2}.",
                            this.GetText(ELanguageCategory.TextInformation, "The language"),
                            this.GetText(ELanguageCategory.Language, EnumerationHelper.EnumerationToString<ELanguage>(this.CurrentLanguage)),
                            this.GetText(ELanguageCategory.TextInformation, "have been loaded")));

                    if (null != this.NewLanguageHaveBeenLoaded)
                        NewLanguageHaveBeenLoaded();
                });
        }
        #endregion

        #region Generate
        private void GenerateDefaultLanguage()
        {
            #region Attributes Categories
            // Beaucoup d'autre textes sont à venir.
            this.defaultLanguageTexts[EnumerationHelper.GetIndex(ELanguageCategory.AttributesCategory)] = new string[]
            {
            "Life",
            "Mana",
            "Experience",
            "Attack",
            "Movement",
            "Resistances",
            "Defence",
            "Loot",
            "Skill",
            "Characteristics",
            };
            #endregion

            #region Attributes Sub Categories
            this.defaultLanguageTexts[EnumerationHelper.GetIndex(ELanguageCategory.SubAttributesCategory)] = new string[]
            {
            "At Start",
            "Current",
            "Percent",
            "Characteristic",
            "Success",
            "All Resistances",
            "Shrine",
            "Items",
            "Bonus",
            "Level Up",
            };
            #endregion

            #region Menu Attributes Editor
            this.defaultLanguageTexts[EnumerationHelper.GetIndex(ELanguageCategory.AttributesEditorMenu)] = new string[]
            {
            "Attributes Editor",
            "Categories",
            "Apply",
            "Modified Value :",
            };
            #endregion

            #region Menu Language Editor
            this.defaultLanguageTexts[EnumerationHelper.GetIndex(ELanguageCategory.LanguageEditorMenu)] = new string[]
            {
            "Language choose :",
            "Categories",
            "Apply",
            "Modify :",
            "Content :",
            "Reload",
            "Save",
            "Text ID :",
            "Language Editor"
            };
            #endregion

            #region Language
            this.defaultLanguageTexts[EnumerationHelper.GetIndex(ELanguageCategory.Language)] = new string[]
            {
            "French",
            "English",
            };
            #endregion

            #region Language Category
            this.defaultLanguageTexts[EnumerationHelper.GetIndex(ELanguageCategory.LanguageCategory)] = new string[]
            {
            // StringHelper EachCapitalize Doit etrep récédé d'un espace.
            "Language",
            "Attributes Category",
            "Sub Attributes Category",
            "Life Attributes",
            "Mana Attributes",
            "Experience Attributes",
            "Attack Attributes",
            "Movement Attributes",
            "Resistances Attributes",
            "Defence Attributes",
            "Loot Attributes",
            "Skill Attributes",
            "Characteristics Attributes",
            "Attributes Editor Menu",
            "Language Editor Menu",
            "Language Category",
            "Main Menu And Sub Menus",
            "Keys Menu",
            "Life Mana Experience Menu",
            "Tooltips",
            "Text Information",
            "Map Areas",
            "Monster Names",
            "Level Up Menu"
            };
            #endregion

            #region Attributes Life
            this.defaultLanguageTexts[EnumerationHelper.GetIndex(ELanguageCategory.LifeAttributes)] = new string[]
            {
            "Maximum Life",
            "Life",
            "Percentage Of Life",
            "Percentage Of Life Regenerated Per Second",
            "Life Regenerated Per Second",
            };
            #endregion

            #region Attributes Characteristics
            this.defaultLanguageTexts[EnumerationHelper.GetIndex(ELanguageCategory.CharacteristicsAttributes)] = new string[]
            {
            "Percentage Of All Characteristics",
            "All Characteristics",
            "Strength",
            "Power",
            "Dexterity",
            "Endurance",
            "Chance",
            "Faith",
            "Resistance",
            "Wisdom",
            "Spirit",
            "Constitution",
            "Remain points",
            };
            #endregion

            #region Attributes Defence
            this.defaultLanguageTexts[EnumerationHelper.GetIndex(ELanguageCategory.DefenceAttributes)] = new string[]
            {
            "Percentage Of Defence",
            "Defence"
            };
            #endregion

            #region Attributes Experience
            this.defaultLanguageTexts[EnumerationHelper.GetIndex(ELanguageCategory.ExperienceAttributes)] = new string[]
            {
            "Percentage Of Experience",
            "Experience",
            "Experience Additionnal Per Kill",
            "Add Experience",
            "Total Experience",
            "Maximum Experience",
            "Level"
            };
            #endregion

            #region Attributes Loot
            this.defaultLanguageTexts[EnumerationHelper.GetIndex(ELanguageCategory.LootAttributes)] = new string[]
            {
            "Percentage Of Gold Find",
            "Percentage Of Item Find",
            "Percentage Of Magic Find",
            "Gold"
            };
            #endregion

            #region Attributes Movement
            this.defaultLanguageTexts[EnumerationHelper.GetIndex(ELanguageCategory.MovementAttributes)] = new string[]
            {
            "Percentage Of Movement Speed",
            "Number Of Steps",
            };
            #endregion

            #region Attributes Resistances
            this.defaultLanguageTexts[EnumerationHelper.GetIndex(ELanguageCategory.ResistancesAttributes)] = new string[]
            {
            "Percentage Of Fire Resistance",
            "Percentage Of Cold Resistance",
            "Percentage Of Lighting Resistance",
            "Percentage Of Faith Resistance",
            "Percentage Of Wind Resistance",
            "Percentage Of Earth Resistance",
            "Percentage Of Poison Resistance",
            "Fire Resistance",
            "Cold Resistance",
            "Lighting Resistance",
            "Faith Resistance",
            "Wind Resistance",
            "Earth Resistance",
            "Poison Resistance",
            "Fire Resistance Limitation",
            "Cold Resistance Limitation",
            "Lighting Resistance Limitation",
            "Faith Resistance Limitation",
            "Wind Resistance Limitation",
            "Earth Resistance Limitation",
            "Poison Resistance Limitation",
            "All Resistances",
            "All Resistances Percentage",
            };
            #endregion

            #region Attributes Skill
            this.defaultLanguageTexts[EnumerationHelper.GetIndex(ELanguageCategory.SkillAttributes)] = new string[]
            {
            "Percentage Of Skill Effect",
            "Percentage Of Heal Effect",
            "Percentage Of Minion Life",
            "Percentage Of Minion Damage",
            "Minimal Heal",
            "Maximal Heal",
            };
            #endregion

            #region Attributes Mana
            this.defaultLanguageTexts[EnumerationHelper.GetIndex(ELanguageCategory.ManaAttributes)] = new string[]
            {
            "Maximum Mana",
            "Mana",
            "Percentage Of Mana",
            "Percentage Of Mana Regenerated Per Second",
            "Mana Regenerated Per Second",
            };
            #endregion

            #region Attributes Attack
            this.defaultLanguageTexts[EnumerationHelper.GetIndex(ELanguageCategory.AttackAttributes)] = new string[]
            {
            "Percentage Of Damage",
            "Percentage Of Attack Speed",
            "Percentage Of Critical Chance",
            "Percentage Of Critical Damage",
            "Percentage Of Dexterity",
            "Minimal Damage",
            "Maximal Damage",
            "Dexterity",
            "Attack Range",
            "Attack Speed",
            "Critical !",
            "Miss !"
            };
            #endregion

            #region Main Menu & Sub Menus
            this.defaultLanguageTexts[EnumerationHelper.GetIndex(ELanguageCategory.MainMenuAndSubMenus)] = new string[]
            {
            "Main Menu",
            "Settings Menu",
            "Audio Menu",
            "Language Menu",
            "Video Menu",
            "Keys Menu",
            "Resume",
            "Settings",
            "Quit the game",
            "Go to previous menu",
            "Language :",
            "Overall",
            "Dialog",
            "Music",
            "Full screen",
            "Resolution :",
            "Quality :",
            "Cameras Effects",
            "Anti-aliasing",
            "Camera blur",
            "Light intensity",
            "Settings",
            "Audio",
            "Language",
            "Video",
            "Keys",
            "Fastest",
            "Fast",
            "Simple",
            "Good",
            "Beautiful",
            "Fantastic",
            "Sun Shafts",
            "Bloom",
            "Color Correction",
            "Vignette",
            "Global Fog",
            "Game Menu",
            "Game",
            "Auto-save",
            "Auto-save every X minutes",
            "Save",
            "Load",
            "Reload",
            };
            #endregion

            #region Keys Menu
            this.defaultLanguageTexts[EnumerationHelper.GetIndex(ELanguageCategory.KeysMenu)] = new string[]
            {
            "[NONE]",
            "Function",
            "Key button 1",
            "Key button 2",
            "Cancel",
            "Reload",
            "Accept",
            "Default",
            "Move",
            "Default",
            "Close All Menus",
            "Attributes Editor Menu",
            "Language Editor Menu",
            "Main Menu",
            "Settings Menu",
            "Language Menu",
            "Sound Configuration Menu",
            "Video Configuration Menu",
            "Keys Configuration Menu",
            "[CHOOSE KEY]",
            "Life Mana Experience Menu",
            "Enable Or Disable Health Bars",
            "Attack Without Move",
            "Portal",
            "Game Menu",
            "Select",
            "Stop To Move",

            "Move Forward",
            "Move Left",
            "Move Right",
            "Move Backward",

            "Move Forward2",
            "Move Left2",
            "Move Right2",
            "Move Backward2",
            "Level Up Menu",
            };
            #endregion

            #region Tooltips
            this.defaultLanguageTexts[EnumerationHelper.GetIndex(ELanguageCategory.Tooltips)] = new string[]
            {
            "Close menu",
            "Modify life text format",
            "Modify mana text format",
            "Modify experience text format",
            "Add a new key",
            "Remove this key",
            "Save configuration in a file and reload your inputs",
            "Reload the defaut inputs configuration in menu but don't apply them",
            "Try to load your inputs file if it exits else load your default configuration in menu only",
            "Reload your current configuration languagage file",
            "Save your current language file and apply the modification",
            "Apply the text modification in menu only",
            "Apply the attribute value modifications in menu only",
            "Reload your current attribute data",
            "Save your current attribute data",
            "Reload the default attribute data",

            "Save the current scene",
            "Load the current scene",
            "Reload the default state of current scene."
            };
            #endregion

            #region Text Information
            this.defaultLanguageTexts[EnumerationHelper.GetIndex(ELanguageCategory.TextInformation)] = new string[]
            {
            "Menu opening",
            "Menu closing",
            "Key",
            "have been deleted",
            "Choose a key.",
            "The key",
            "have been added",
            "The language",
            "have been saved",
            "have been loaded",
            "The new attribute value is",
            };
            #endregion

            #region Map Area
            this.defaultLanguageTexts[EnumerationHelper.GetIndex(ELanguageCategory.MapAreas)] = new string[]
           {
            "Hell Plain",
            "Don't Destroy Scene"
           };
            #endregion

            #region Monsters Name
            this.defaultLanguageTexts[EnumerationHelper.GetIndex(ELanguageCategory.MonsterNames)] = new string[]
            {
            "Mutant",
            };

            this.defaultLanguageTexts[EnumerationHelper.GetIndex(ELanguageCategory.LevelUpMenu)] = new string[]
            {
                "Level Up Menu"
            };
            #endregion

            for (int i = 0; i < EnumerationHelper.Count<ELanguageCategory>(); i++)
            {
                if (null == this.defaultLanguageTexts[i])
                    this.defaultLanguageTexts[i] = new string[] { };
            }
        }
        #endregion
    }
}