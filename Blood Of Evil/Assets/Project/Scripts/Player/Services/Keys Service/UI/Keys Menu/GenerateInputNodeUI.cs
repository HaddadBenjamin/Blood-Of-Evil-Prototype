using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace BloodOfEvil.Player.Services.Keys.UI
{
    using Audio;
    using Serialization;
    using Helpers;

    using Scene;
    using Scene.Services.ObjectPool;

    public class GenerateInputNodeUI : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private EPlayerInput[] inputsThatCantBeModifiable;
        private Transform myTransform;
        private ObjectsPool pool; // Pas pertinent dans ce cas car le nombre reste toujours le même. mais OSEF.
        private InputDataConfiguration[] inputDataConfigurations;
        [SerializeField]
        private EPlayerInput[] inputThatCanBeModifiable;

        [SerializeField]
        private Button defaultButton;
        [SerializeField]
        private Button acceptButton;
        [SerializeField]
        private Button cancelButton;
        private InputDataConfigurationArraySerializable inputDataSerializable;
        #endregion

        #region Unity Behaviour
        void Start()
        {
            this.defaultButton.onClick.AddListener(delegate ()
            {
                this.Generate(PlayerServicesAndModulesContainer.Instance.InputService.GetCopyDefaultInputsDataConfiguration());

                SceneServicesContainer.Instance.AudioReferencesArraysService.Play2DSound(EAudioCategory.SFX, "Click Button");
            });

            this.acceptButton.onClick.AddListener(delegate ()
            {
                this.inputDataSerializable = new InputDataConfigurationArraySerializable();

                this.inputDataSerializable.inputDataConfigurations = this.inputDataConfigurations;

                SerializerHelper.JsonSerializeSave(this.inputDataSerializable, SceneServicesContainer.Instance.FileSystemConfiguration.KeysSettingsFilename);

                PlayerServicesAndModulesContainer.Instance.InputService.SetInputsDataConfiguration(this.inputDataSerializable.inputDataConfigurations);
                    // le service d'input doit récupérer ces informations.

                    SceneServicesContainer.Instance.AudioReferencesArraysService.Play2DSound(EAudioCategory.SFX, "Click Button");
            });

            this.cancelButton.onClick.AddListener(delegate ()
            {
                if (SerializerHelper.DoesCompletSavePathExists("Keys", ".json"))
                {
                    InputDataConfigurationArraySerializable inputDataSerializable = SerializerHelper.JsonDeserializeLoad<InputDataConfigurationArraySerializable>("Keys");

                    this.Generate(inputDataSerializable.inputDataConfigurations);
                }
                else
                    this.Generate(PlayerServicesAndModulesContainer.Instance.InputService.GetCopyDefaultInputsDataConfiguration());

                SceneServicesContainer.Instance.AudioReferencesArraysService.Play2DSound(EAudioCategory.SFX, "Click Button");
            });

            this.myTransform = transform;
            this.pool = PlayerServicesAndModulesContainer.Instance.ObjectsPoolService.GetPool("Input Node UI Line");

            // Devrait charger le fichier d'évênements si elle n'existe pas récupérer le défault.
            StartCoroutine(this.GenerateInputAtEndFrame());
        }
        #endregion

        #region Intern Behaviour
        private void Generate(InputDataConfiguration[] inputDataConfigurations)
        {
            if (this.pool.GetTheNumberOfObjectsInPool() > 0)
            {
                for (int poolIndex = this.pool.GetGameobjects().Length - 1; poolIndex >= 0; poolIndex--)
                {
                    GameObject obj = this.pool.GetGameobjects()[poolIndex];

                    if (null != obj && obj.activeSelf)
                    {
                        obj.GetComponent<InputNodeUI>().UnsubscribeButtons();

                        pool.RemoveObjectInPool(obj);
                    }
                }
            }

            this.inputDataConfigurations = inputDataConfigurations;

            for (int inputDataConfigurationIndex = 0; inputDataConfigurationIndex < inputDataConfigurations.Length; inputDataConfigurationIndex++)
                pool.AddObjectInPool(this.myTransform).GetComponent<InputNodeUI>().Initialize(
                    inputDataConfigurations,
                    inputDataConfigurationIndex,
                    Array.Exists(this.inputsThatCantBeModifiable, input => input == EnumerationHelper.IntegerToEnumeration<EPlayerInput>(inputDataConfigurationIndex)));
        }

        private IEnumerator GenerateInputAtEndFrame()
        {
            yield return new WaitForSeconds(0.1f);

            this.Generate(PlayerServicesAndModulesContainer.Instance.InputService.GetCopyInputsDataConfiguration());
        }
        #endregion
    }
}