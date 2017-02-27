using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace BloodOfEvil.Player.Services.Keys.UI
{
    using Scene;
    using Extensions;
    using Helpers;
    using Utilities.UI;

    using Audio;
    using Language;
    using Language.UI;

    /// <summary>
    /// Voici probablement le code le plus sal que j'ai fais jusqu'à présent. Désole ça risque de vous donner la nausée.
    /// </summary>
    public class InputNodeUI : MonoBehaviour
    {
        #region Fields
        //private InputDataConfiguration[] inputDataConfigurations;
        private InputDataConfiguration inputDataConfiguration;
        private Transform myTransform;
        private Text functionText;
        private int inputDataConfigurationIndex;

        // Tous les attributs ci-dessous devrait se retrouver dans une classe à part.
        private Text keyButton1Text;
        private Text keyButton2Text;
        private Button addRemoveButton1;
        private Button addRemoveButton2;
        private Text addRemoveTextButton1;
        private Text addRemoveTextButton2;
        private Image backgroundText1;
        private Image backgroundText2;
        // Doit changer le bacground dans sa property.
        private bool canRetrieveEvent1;
        private bool canRetrieveEvent2;
        private bool isAModifiableInput;
        #endregion

        #region Properties
        public bool CanRetrieveEvent1
        {
            get { return canRetrieveEvent1; }
            set
            {
                canRetrieveEvent1 = value;

                this.backgroundText1.GetComponent<Image>().color =
                    canRetrieveEvent1 ? Color.yellow :
                    Color.white;
            }
        }

        public bool CanRetrieveEvent2
        {
            get { return canRetrieveEvent2; }
            set
            {
                canRetrieveEvent2 = value;

                this.backgroundText2.GetComponent<Image>().color =
                    canRetrieveEvent2 ? Color.yellow :
                    Color.white;
            }
        }
        #endregion`

        #region Unity Behaviour
        void Update()
        {
            if (this.canRetrieveEvent1 || this.canRetrieveEvent2)
            {
                foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(kcode))
                    {
                        // Permet de désasigner les autres évênements utilisant cette touche en première key code.
                        //InputDataConfiguration[] dataConfigurationToClear =
                        //    Array.FindAll(this.inputDataConfigurations, dataConfiguration =>
                        //    dataConfiguration.KeyCodes.Count > 0 && dataConfiguration.KeyCodes[0] == kcode);

                        //for (int dataConfigurationIndexToClearIndex = 0; dataConfigurationIndexToClearIndex < dataConfigurationToClear.Length; dataConfigurationIndexToClearIndex++)
                        //{
                        //    dataConfigurationToClear[dataConfigurationIndexToClearIndex].KeyCodes.Clear();
                        //    //Reupdate interface.
                        //}

                        if (this.CanRetrieveEvent1)
                        {
                            this.CanRetrieveEvent1 = false;
                            this.inputDataConfiguration.KeyCodes[0] = kcode;
                        }
                        else if (CanRetrieveEvent2)
                        {
                            this.CanRetrieveEvent2 = false;
                            this.inputDataConfiguration.KeyCodes[1] = kcode;
                        }

                        PlayerServicesAndModulesContainer.Instance.TextInformationService.AddTextInformation(
                            string.Format("{0} [{1}] {2}.",
                                PlayerServicesAndModulesContainer.Instance.LanguageService.GetText(ELanguageCategory.TextInformation, "The key"),
                                kcode,
                                PlayerServicesAndModulesContainer.Instance.LanguageService.GetText(ELanguageCategory.TextInformation, "have been added")));

                        this.UpdateTextAndButtonsContentAndBehaviour(this.inputDataConfigurationIndex);
                    }
                }
            }
        }
        #endregion

        #region Public Behaviour
        public void Initialize(InputDataConfiguration[] inputDataConfigurations, int inputDataConfigurationIndex, bool isAModifiableInput)
        {
            //this.inputDataConfigurations = inputDataConfigurations;
            this.isAModifiableInput = isAModifiableInput;
            this.inputDataConfigurationIndex = inputDataConfigurationIndex;
            this.inputDataConfiguration = inputDataConfigurations[this.inputDataConfigurationIndex];

            if (null == this.myTransform)
                this.UpdateComponentReferences();

            this.UpdateTextAndButtonsContentAndBehaviour(inputDataConfigurationIndex);
        }

        public void UnsubscribeButtons()
        {
            this.addRemoveButton1.onClick.RemoveAllListeners();
            this.addRemoveButton2.onClick.RemoveAllListeners();
        }
        #endregion

        #region Intern Behaviour
        private void UpdateComponentReferences()
        {
            this.myTransform = transform;

            this.functionText = this.myTransform.FindChild("Function Text").GetComponent<Text>();

            this.keyButton1Text = this.myTransform.FindChild("Key Button 1 Text").GetComponent<Text>();
            this.keyButton2Text = this.myTransform.FindChild("Key Button 2 Text").GetComponent<Text>();

            this.addRemoveButton1 = this.myTransform.FindChild("Add And Remove Key 1 Button").GetComponent<Button>();
            this.addRemoveButton2 = this.myTransform.FindChild("Add And Remove Key 2 Button").GetComponent<Button>();

            this.addRemoveTextButton1 = this.addRemoveButton1.transform.FindChild("Text").GetComponent<Text>();
            this.addRemoveTextButton2 = this.addRemoveButton2.transform.FindChild("Text").GetComponent<Text>();

            this.backgroundText1 = this.myTransform.FindChild("Key Background 1 Select").GetComponent<Image>();
            this.backgroundText2 = this.myTransform.FindChild("Key Background 2 Select").GetComponent<Image>();

            //List<KeyCode> kyCodes = this.inputDataConfiguration.KeyCodes;

            this.addRemoveButton1.gameObject.SetActive(this.isAModifiableInput);
            this.addRemoveButton2.gameObject.SetActive(this.isAModifiableInput && this.inputDataConfiguration.KeyCodes.Count > 0);

            PlayerServicesAndModulesContainer.Instance.LanguageService.NewLanguageHaveBeenLoaded += delegate ()
            {
                List<KeyCode> keyCodes = this.inputDataConfiguration.KeyCodes;

                if (keyCodes.Count < 1)
                    this.keyButton1Text.text = PlayerServicesAndModulesContainer.Instance.LanguageService.GetText(ELanguageCategory.KeysMenu, "[NONE]");
                if (keyCodes.Count < 2)
                    this.keyButton2Text.text = PlayerServicesAndModulesContainer.Instance.LanguageService.GetText(ELanguageCategory.KeysMenu, "[NONE]");
            };
        }

        /// <summary>
        /// Ce code est vraiment dégueulasse.
        /// </summary>
        /// <param name="inputDataConfigurationIndex"></param>
        private void UpdateTextAndButtonsContentAndBehaviour(int inputDataConfigurationIndex)
        {
            this.UnsubscribeButtons();

            this.functionText.GetComponent<UpdateLanguageText>().UpdateDefaultText(
                    EnumerationHelper.EnumerationIntegerIndexToString<EPlayerInput>(inputDataConfigurationIndex).ReplaceUppercaseBySpaceAndUppercase());
            this.functionText.GetComponent<UpdateLanguageText>().UpdateText();

            List<KeyCode> keyCodes = this.inputDataConfiguration.KeyCodes;

            this.addRemoveButton2.gameObject.SetActive(this.isAModifiableInput && this.inputDataConfiguration.KeyCodes.Count > 0);

            // Code très sale est redondant.
            if (keyCodes.Count > 0)
            {
                this.keyButton1Text.text = string.Format("[{0}]", EnumerationHelper.EnumerationToString(keyCodes[0]));

                if (this.isAModifiableInput)
                {
                    this.addRemoveTextButton1.color = Color.red;
                    this.addRemoveTextButton1.text = "X";
                    this.addRemoveTextButton1.fontSize = 16;

                    this.addRemoveButton1.GetComponent<GenericTooltip>().Reinitialize("Remove this key", Color.red);
                }

                this.addRemoveButton1.onClick.AddListener(delegate ()
                {
                    PlayerServicesAndModulesContainer.Instance.TextInformationService.AddTextInformation(
                        string.Format("{0} [{1}] {2}.",
                            PlayerServicesAndModulesContainer.Instance.LanguageService.GetText(ELanguageCategory.TextInformation, "Key"),
                            this.inputDataConfiguration.KeyCodes[0],
                            PlayerServicesAndModulesContainer.Instance.LanguageService.GetText(ELanguageCategory.TextInformation, "have been deleted")));

                    this.inputDataConfiguration.KeyCodes.Clear();
                    this.addRemoveButton1.onClick.RemoveAllListeners();

                    CanRetrieveEvent1 = false;

                    this.UpdateTextAndButtonsContentAndBehaviour(inputDataConfigurationIndex);

                    SceneServicesContainer.Instance.AudioReferencesArraysService.Play2DSound(EAudioCategory.SFX, "Remove Button");


                });
            }
            else
            {
                this.keyButton1Text.text = PlayerServicesAndModulesContainer.Instance.LanguageService.GetText(ELanguageCategory.KeysMenu, "[NONE]");

                if (this.isAModifiableInput)
                {
                    this.addRemoveTextButton1.color = Color.green;
                    this.addRemoveTextButton1.text = "+";
                    this.addRemoveTextButton1.fontSize = 21;

                    this.addRemoveButton1.GetComponent<GenericTooltip>().Reinitialize("Add a new key", Color.green);
                }

                this.addRemoveButton1.onClick.AddListener(delegate ()
                {
                    this.inputDataConfiguration.KeyCodes.Add(KeyCode.A);
                    this.CanRetrieveEvent1 = true;
                    this.addRemoveButton1.onClick.RemoveAllListeners();

                    this.UpdateTextAndButtonsContentAndBehaviour(inputDataConfigurationIndex);

                    this.keyButton1Text.text = PlayerServicesAndModulesContainer.Instance.LanguageService.GetText(ELanguageCategory.KeysMenu, "[CHOOSE KEY]");

                    SceneServicesContainer.Instance.AudioReferencesArraysService.Play2DSound(EAudioCategory.SFX, "Add Button");

                    PlayerServicesAndModulesContainer.Instance.TextInformationService.AddTextInformation(
                        PlayerServicesAndModulesContainer.Instance.LanguageService.GetText(ELanguageCategory.TextInformation, "Choose a key."));
                });
            }


            if (keyCodes.Count > 1)
            {
                this.keyButton2Text.text = string.Format("[{0}]", EnumerationHelper.EnumerationToString(keyCodes[1]));

                if (this.isAModifiableInput)
                {
                    this.addRemoveTextButton2.color = Color.red;
                    this.addRemoveTextButton2.text = "X";
                    this.addRemoveTextButton2.fontSize = 16;

                    this.addRemoveButton2.GetComponent<GenericTooltip>().Reinitialize("Remove this key", Color.red);
                }

                this.addRemoveButton2.onClick.AddListener(delegate ()
                {
                    PlayerServicesAndModulesContainer.Instance.TextInformationService.AddTextInformation(
                        string.Format("{0} [{1}] {2}.",
                            PlayerServicesAndModulesContainer.Instance.LanguageService.GetText(ELanguageCategory.TextInformation, "Key"),
                            this.inputDataConfiguration.KeyCodes[1],
                            PlayerServicesAndModulesContainer.Instance.LanguageService.GetText(ELanguageCategory.TextInformation, "have been deleted")));

                    this.inputDataConfiguration.KeyCodes.RemoveAt(1);
                    this.addRemoveButton2.onClick.RemoveAllListeners();

                    CanRetrieveEvent2 = false;

                    this.UpdateTextAndButtonsContentAndBehaviour(inputDataConfigurationIndex);

                    SceneServicesContainer.Instance.AudioReferencesArraysService.Play2DSound(EAudioCategory.SFX, "Remove Button");
                });
            }
            else if (keyCodes.Count > 0)
            {
                this.keyButton2Text.text = PlayerServicesAndModulesContainer.Instance.LanguageService.GetText(ELanguageCategory.KeysMenu, "[NONE]");

                if (this.isAModifiableInput)
                {
                    this.addRemoveTextButton2.color = Color.green;
                    this.addRemoveTextButton2.text = "+";
                    this.addRemoveTextButton2.fontSize = 21;

                    this.addRemoveButton2.GetComponent<GenericTooltip>().Reinitialize("Add a new key", Color.green);
                }

                this.addRemoveButton2.onClick.AddListener(delegate ()
                {
                    this.inputDataConfiguration.KeyCodes.Add(KeyCode.A);
                    this.CanRetrieveEvent2 = true;
                    this.addRemoveButton2.onClick.RemoveAllListeners();

                    this.UpdateTextAndButtonsContentAndBehaviour(inputDataConfigurationIndex);

                    this.keyButton2Text.text = PlayerServicesAndModulesContainer.Instance.LanguageService.GetText(ELanguageCategory.KeysMenu, "[CHOOSE KEY]");

                    SceneServicesContainer.Instance.AudioReferencesArraysService.Play2DSound(EAudioCategory.SFX, "Add Button");

                    PlayerServicesAndModulesContainer.Instance.TextInformationService.AddTextInformation(
                        PlayerServicesAndModulesContainer.Instance.LanguageService.GetText(ELanguageCategory.TextInformation, "Choose a key."));
                });
            }
        }
        #endregion
    }
}