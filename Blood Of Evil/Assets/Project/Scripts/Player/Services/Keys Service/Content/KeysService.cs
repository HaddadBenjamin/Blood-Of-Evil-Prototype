using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace BloodOfEvil.Player.Services.Keys
{
    using Scene;
    using Helpers;
    using ObjectInScene;

    using Canvases;

    using Serialization;

    /// <summary>
    /// Ce code contient une copie présente 4 fois. Bonjour le mailaise -> beurk.
    /// </summary>
    [System.Serializable]
    public sealed class KeysService : IDataInitializable
    {
        #region Fields
        [SerializeField, ExecuteInEditMode, Header("Editable for serialization only.")]
        private InputDataConfiguration[] inputDataConfigurations;
        private InputDataConfiguration[] defaultInputDataConfigurations;
        #endregion

        #region Intern Behaviour
        private InputDataConfiguration GetInputDataConfiguration(EPlayerInput input)
        {
            return this.inputDataConfigurations[EnumerationHelper.GetIndex<EPlayerInput>(input)];
        }

        private void InputsConfiguration()
        {
            this.inputDataConfigurations[EnumerationHelper.GetIndex<EPlayerInput>(EPlayerInput.Move)].KeyCodes.Add(KeyCode.Mouse0);
            this.inputDataConfigurations[EnumerationHelper.GetIndex<EPlayerInput>(EPlayerInput.CloseAllMenus)].KeyCodes.Add(KeyCode.Escape);
            this.inputDataConfigurations[EnumerationHelper.GetIndex<EPlayerInput>(EPlayerInput.MainMenu)].KeyCodes.Add(KeyCode.Escape);
            this.inputDataConfigurations[EnumerationHelper.GetIndex<EPlayerInput>(EPlayerInput.EnableOrDisableHealthBars)].KeyCodes.Add(KeyCode.Tab);
            this.inputDataConfigurations[EnumerationHelper.GetIndex<EPlayerInput>(EPlayerInput.AttackWithoutMove)].KeyCodes.Add(KeyCode.LeftShift);
            this.inputDataConfigurations[EnumerationHelper.GetIndex<EPlayerInput>(EPlayerInput.AttackWithoutMove)].KeyCodes.Add(KeyCode.Mouse0);
            this.inputDataConfigurations[EnumerationHelper.GetIndex<EPlayerInput>(EPlayerInput.Portal)].KeyCodes.Add(KeyCode.T);
            this.inputDataConfigurations[EnumerationHelper.GetIndex<EPlayerInput>(EPlayerInput.Select)].KeyCodes.Add(KeyCode.Mouse0);
            this.inputDataConfigurations[EnumerationHelper.GetIndex<EPlayerInput>(EPlayerInput.StopToMove)].KeyCodes.Add(KeyCode.LeftShift);
            this.inputDataConfigurations[EnumerationHelper.GetIndex<EPlayerInput>(EPlayerInput.StopToMove)].KeyCodes.Add(KeyCode.LeftShift);

            this.inputDataConfigurations[EnumerationHelper.GetIndex<EPlayerInput>(EPlayerInput.MoveForward)].KeyCodes.Add(KeyCode.Z);
            this.inputDataConfigurations[EnumerationHelper.GetIndex<EPlayerInput>(EPlayerInput.MoveLeft)].KeyCodes.Add(KeyCode.Q);
            this.inputDataConfigurations[EnumerationHelper.GetIndex<EPlayerInput>(EPlayerInput.MoveBackward)].KeyCodes.Add(KeyCode.S);
            this.inputDataConfigurations[EnumerationHelper.GetIndex<EPlayerInput>(EPlayerInput.MoveRight)].KeyCodes.Add(KeyCode.D);

            this.inputDataConfigurations[EnumerationHelper.GetIndex<EPlayerInput>(EPlayerInput.MoveForward2)].KeyCodes.Add(KeyCode.UpArrow);
            this.inputDataConfigurations[EnumerationHelper.GetIndex<EPlayerInput>(EPlayerInput.MoveLeft2)].KeyCodes.Add(KeyCode.LeftArrow);
            this.inputDataConfigurations[EnumerationHelper.GetIndex<EPlayerInput>(EPlayerInput.MoveBackward2)].KeyCodes.Add(KeyCode.DownArrow);
            this.inputDataConfigurations[EnumerationHelper.GetIndex<EPlayerInput>(EPlayerInput.MoveRight2)].KeyCodes.Add(KeyCode.RightArrow);
            this.UpdateDefaultInputDataConfiguration();
        }
        #endregion

        #region Interfaces Behaviour
        void IDataInitializable.Initialize()
        {
            this.inputDataConfigurations = new InputDataConfiguration[EnumerationHelper.Count<EPlayerInput>()];

            for (int inputDataConfigurationIndex = 0; inputDataConfigurationIndex < this.inputDataConfigurations.Length; inputDataConfigurationIndex++)
                this.inputDataConfigurations[inputDataConfigurationIndex] = new InputDataConfiguration();

            this.InputsConfiguration();
        }
        #endregion

        #region Public Behaviour
        public bool IsDown(EPlayerInput input)
        {
            InputDataConfiguration inputDataConfiguration = this.GetInputDataConfiguration(input);

            inputDataConfiguration.DoesIsDownAtPreviousFrame = inputDataConfiguration.DoesIsDown;
            inputDataConfiguration.DoesIsDown = false;

            if (inputDataConfiguration.KeyCodes.Count == 0)
                return false;

            // Si il y a plusieurs touches on testes la dernière touche en down et les autres en contiouslyDown, cela permet d'avoir un comportement qui arrive plus simplement.
            if (inputDataConfiguration.KeyCodes.Count > 0)
            {
                for (int inputIndex = 0; inputIndex < inputDataConfiguration.KeyCodes.Count - 1; inputIndex++)
                {
                    if (!(Input.GetKey(inputDataConfiguration.KeyCodes[inputIndex])))
                        return false;
                }

                if (!(Input.GetKeyDown(inputDataConfiguration.KeyCodes[inputDataConfiguration.KeyCodes.Count - 1])))
                    return false;
            }
            else
            {
                foreach (KeyCode keyCode in inputDataConfiguration.KeyCodes)
                {
                    if (!(Input.GetKeyDown(keyCode)))
                        return false;
                }
            }

            inputDataConfiguration.DoesIsDown = true;

            return true;
        }

        public bool IsContiniouslyDown(EPlayerInput input)
        {
            InputDataConfiguration inputDataConfiguration = this.GetInputDataConfiguration(input);

            inputDataConfiguration.DoesIsContinouslyDownAtPreviousFrame = inputDataConfiguration.DoesIsContinouslyDown;
            inputDataConfiguration.DoesIsContinouslyDown = false;

            if (inputDataConfiguration.KeyCodes.Count == 0)
                return false;

            foreach (KeyCode keyCode in inputDataConfiguration.KeyCodes)
            {
                if (!(Input.GetKey(keyCode)))
                    return false;
            }

            inputDataConfiguration.DoesIsContinouslyDown = true;

            return true;
        }

        public bool IsUp(EPlayerInput input)
        {
            InputDataConfiguration inputDataConfiguration = this.GetInputDataConfiguration(input);

            inputDataConfiguration.DoesIsUpAtPreviousFrame = inputDataConfiguration.DoesIsUp;
            inputDataConfiguration.DoesIsUp = false;

            if (inputDataConfiguration.KeyCodes.Count == 0)
                return false;

            foreach (KeyCode keyCode in inputDataConfiguration.KeyCodes)
            {
                if (!(Input.GetKeyUp(keyCode)))
                    return false;
            }

            inputDataConfiguration.DoesIsUp = true;

            return true;
        }

        public bool DoesInputHaveBeenPressedAtPreviousFrame(EPlayerInput input)
        {
            return this.GetInputDataConfiguration(input).DoesIsContinouslyDownAtPreviousFrame;
        }

        public void AddCanvasConfigurationInput(CanvasInputAndIDConfiguration canvasConfiguration)
        {
            InputDataConfiguration inputDataConfiguration =
                this.inputDataConfigurations[EnumerationHelper.GetIndex<EPlayerInput>(canvasConfiguration.Input)] =
                this.defaultInputDataConfigurations[EnumerationHelper.GetIndex<EPlayerInput>(canvasConfiguration.Input)];

            inputDataConfiguration.KeyCodes = canvasConfiguration.InputsToOpenOrCloseCanvas.KeyCodes;

            this.LoadConfigFile();
            //this.UpdateDefaultInputDataConfiguration();
        }

        // Code sale et redondant
        public InputDataConfiguration[] GetCopyDefaultInputsDataConfiguration()
        {
            InputDataConfiguration[] copyDefault = new InputDataConfiguration[this.defaultInputDataConfigurations.Length];

            for (int sourceIndex = 0; sourceIndex < this.defaultInputDataConfigurations.Length; sourceIndex++)
            {
                InputDataConfiguration destinationConfiguration = copyDefault[sourceIndex] = new InputDataConfiguration();

                List<KeyCode> keyCodes = this.defaultInputDataConfigurations[sourceIndex].KeyCodes;

                for (int keyCodeIndex = 0; keyCodeIndex < keyCodes.Count; keyCodeIndex++)
                    destinationConfiguration.KeyCodes.Add(keyCodes[keyCodeIndex]);
            }

            return copyDefault;
        }

        // Code sale et redondant
        public InputDataConfiguration[] GetCopyInputsDataConfiguration()
        {
            InputDataConfiguration[] copy = new InputDataConfiguration[this.inputDataConfigurations.Length];

            for (int sourceIndex = 0; sourceIndex < this.inputDataConfigurations.Length; sourceIndex++)
            {
                InputDataConfiguration destinationConfiguration = copy[sourceIndex] = new InputDataConfiguration();

                List<KeyCode> keyCodes = this.inputDataConfigurations[sourceIndex].KeyCodes;

                for (int keyCodeIndex = 0; keyCodeIndex < keyCodes.Count; keyCodeIndex++)
                    destinationConfiguration.KeyCodes.Add(keyCodes[keyCodeIndex]);
            }

            return copy;
        }

        // Code sale et redondant
        public void SetInputsDataConfiguration(InputDataConfiguration[] dataInput)
        {
            //this.CopyInputDataConfiguration(inputDataConfigurations, ref this.inputDataConfigurations);

            for (int sourceIndex = 0; sourceIndex < dataInput.Length; sourceIndex++)
            {
                InputDataConfiguration destinationConfiguration = this.inputDataConfigurations[sourceIndex] = new InputDataConfiguration();

                List<KeyCode> keyCodes = dataInput[sourceIndex].KeyCodes;

                for (int keyCodeIndex = 0; keyCodeIndex < keyCodes.Count; keyCodeIndex++)
                    destinationConfiguration.KeyCodes.Add(keyCodes[keyCodeIndex]);
            }
        }

        public bool DoesAMoveKeyHaveBeenPressed()
        {
            return (this.IsContiniouslyDown(EPlayerInput.MoveLeft) ||
                    this.IsContiniouslyDown(EPlayerInput.MoveLeft2) ||
                    this.IsContiniouslyDown(EPlayerInput.MoveRight) ||
                    this.IsContiniouslyDown(EPlayerInput.MoveRight2) ||
                    this.IsContiniouslyDown(EPlayerInput.MoveForward) ||
                    this.IsContiniouslyDown(EPlayerInput.MoveForward2) ||
                    this.IsContiniouslyDown(EPlayerInput.MoveBackward) ||
                    this.IsContiniouslyDown(EPlayerInput.MoveBackward2) ||
                    this.IsContiniouslyDown(EPlayerInput.Move));
        }

        public Vector3 GetMoveKeysDirection()
        {
            if (this.IsContiniouslyDown(EPlayerInput.MoveLeft) ||
                this.IsContiniouslyDown(EPlayerInput.MoveLeft2))
                return Vector3.left;
            if (this.IsContiniouslyDown(EPlayerInput.MoveRight) ||
                this.IsContiniouslyDown(EPlayerInput.MoveRight2))
                return Vector3.right;
            if (this.IsContiniouslyDown(EPlayerInput.MoveForward) ||
                this.IsContiniouslyDown(EPlayerInput.MoveForward2))
                return Vector3.up;
            if (this.IsContiniouslyDown(EPlayerInput.MoveBackward) ||
                this.IsContiniouslyDown(EPlayerInput.MoveBackward2))
                return Vector3.down;

            return Vector3.zero;
        }
        #endregion

        #region Intern Behaviour
        private string GetFileName()
        {
            return SceneServicesContainer.Instance.FileSystemConfiguration.KeysSettingsFilename;
        }

        private void LoadConfigFile()
        {
            SerializerHelper.Load< InputDataConfigurationArraySerializable>(
                filename: this.GetFileName(),
                isReplicatedNextTheBuild: false,
                isEncrypted: false,
                onLoadSuccess: (InputDataConfigurationArraySerializable data) =>
                {
                    this.SetInputsDataConfiguration(data.inputDataConfigurations);
                });
        }

        // Code sale et redondant
        private void UpdateDefaultInputDataConfiguration()
        {
            this.defaultInputDataConfigurations = new InputDataConfiguration[this.inputDataConfigurations.Length];

            for (int sourceIndex = 0; sourceIndex < this.inputDataConfigurations.Length; sourceIndex++)
            {
                InputDataConfiguration destinationConfiguration = this.defaultInputDataConfigurations[sourceIndex] = new InputDataConfiguration();

                List<KeyCode> keyCodes = this.inputDataConfigurations[sourceIndex].KeyCodes;

                for (int keyCodeIndex = 0; keyCodeIndex < keyCodes.Count; keyCodeIndex++)
                    destinationConfiguration.KeyCodes.Add(keyCodes[keyCodeIndex]);
            }

            this.LoadConfigFile();
        }
        #endregion
    }
}