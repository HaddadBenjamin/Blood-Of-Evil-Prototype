using UnityEngine;
using System.Collections;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BloodOfEvil.Player.Services.Canvases
{
    using Helpers;
    using Extensions;

    using Scene;
    using Scene.Services.References;

    using Keys;
    using Audio;
    using Video;
    using Language;

    public class CanvasesManagerService : AReferenceContainer<GameObject>
    {
        #region Fields
        private CanvasInputAndIDConfiguration[] canvasesConfigurations;
        private FadeInFadeOutCanvas[] fadeInFadeOutCanvases;
        private Canvas[] canvases;
        private CanvasScaler[] canvasesScaler;
        [SerializeField]
        private CanvasScaler[] canvasesScalerSimpleUI;

        [SerializeField]
        private Vector2 shadowEffectDistance;
        [SerializeField]
        private Color shadowColor;
        private bool aCanvasIsOpen = false;
        public Action<bool> ACanvasIsOpenListener;

        [SerializeField]
        private Vector2 developmentResolution = new Vector2(1536.0f, 864.0f);
        [SerializeField]
        private Vector2 buildResolution = new Vector2(1680.0f, 800.0f);

        private KeysService keysService;
        #endregion

        #region Properties
        public bool ACanvasIsOpen
        {
            get { return aCanvasIsOpen; }
            private set
            {
                aCanvasIsOpen = value;

                // Permet dans mon gameplay d'empêcher le joueur de se déplacer ou d'attaquer si une fenêtre est ouverte.
                ACanvasIsOpenListener.SafeCall(aCanvasIsOpen);
            }
        }
        #endregion

        #region Virtual Behaviour
        public override void Initialize()
        {
            base.Initialize();

            this.keysService = PlayerServicesAndModulesContainer.Instance.InputService;

            this.canvasesConfigurations = Array.ConvertAll(base.references, obj => obj.GetComponent<CanvasInputAndIDConfiguration>());
            this.fadeInFadeOutCanvases = Array.ConvertAll(base.references, obj => obj.GetComponent<FadeInFadeOutCanvas>());
            this.canvases = Array.ConvertAll(base.references, obj => obj.GetComponent<Canvas>());
            this.canvasesScaler = Array.ConvertAll(base.references, obj => obj.GetComponent<CanvasScaler>());

            // Permet de lier chaque touches de la fenêtre à mon manageur d'action utilisateur et donc permettre de l'ouvrir et de la fermer avec une combinaisons de touches.
            Array.ForEach(this.canvasesConfigurations, canvasConfiguration =>
                this.keysService.AddCanvasConfigurationInput(canvasConfiguration));

            // Empêcher que les fenêtres se détruisent lors d'un changement de scène. Chaque fenêtre ont un effet d'ombre et sont aussi draggable.
            Array.ForEach(base.references, obj => DontDestroyOnLoad(obj));
            Array.ForEach(base.references, obj =>
            {
                UnityEngine.UI.Shadow shadow = obj.transform.Find("Panel").gameObject.AddComponent<UnityEngine.UI.Shadow>();

                shadow.effectDistance = this.shadowEffectDistance;
                shadow.effectColor = this.shadowColor;
                shadow.useGraphicAlpha = true;
            });
            Array.ForEach(base.references, obj => obj.transform.Find("Panel").gameObject.AddComponent<PanelDraggable>());
            Array.ForEach(this.fadeInFadeOutCanvases, fadeInFadeOutCanvas => fadeInFadeOutCanvas.Initalize());
            Array.ForEach(this.canvasesScaler, canvasScaler => canvasScaler.referenceResolution = this.GetCanvasResolutionReference());
            Array.ForEach(this.canvasesScalerSimpleUI, canvasScaler => canvasScaler.referenceResolution = this.GetCanvasResolutionReference());
            Array.ForEach(this.canvasesScalerSimpleUI, obj => DontDestroyOnLoad(obj.gameObject));

            this.UpdateIsACanvasIsOpen();
        }
        #endregion

        #region Unity Behaviour
        void Update()
        {
            this.InverseActiveCanvasesOnInputDown();
            this.UpdateIsACanvasIsOpen();

            if (this.keysService.IsDown(EPlayerInput.CloseAllMenus))
            {
                if (this.ACanvasIsOpen)
                    this.CloseAllCanvases();
                else
                    this.Open("[MENU] Main");

            }

            if (this.keysService.IsDown(EPlayerInput.EnableOrDisableHealthBars))
            {
                GameObject healthBarCanvas = SceneServicesContainer.Instance.GameObjectInSceneReferencesService.Get("[UI] Enemies Health Bars");

                healthBarCanvas.SetActive(!healthBarCanvas.activeSelf);
            }

            this.UpdateIsACanvasIsOpen();
        }
        #endregion

        #region Public Behaviour
        public void Open(string canvasName)
        {
            if (this.fadeInFadeOutCanvases[base.GetHashId(canvasName)].FadeOut())
            {
                PlayerServicesAndModulesContainer.Instance.MovementModule.StopToMove();
                SceneServicesContainer.Instance.AudioReferencesArraysService.Play2DSound(EAudioCategory.SFX, "Open Menu");

                PlayerServicesAndModulesContainer.Instance.
                    TextInformationService.AddTextInformation(
                    string.Format("{0} : {1}.",
                        PlayerServicesAndModulesContainer.Instance.LanguageService.GetText(ELanguageCategory.TextInformation, "Menu opening"),
                        PlayerServicesAndModulesContainer.Instance.LanguageService.GetText(ELanguageCategory.KeysMenu,
                                EnumerationHelper.EnumerationToString(canvasesConfigurations[base.GetHashId(canvasName)].Input).
                                ReplaceUppercaseBySpaceAndUppercase())));

            }
        }

        public void Close(string canvasName)
        {
            if (this.fadeInFadeOutCanvases[base.GetHashId(canvasName)].FadeIn())
                SceneServicesContainer.Instance.AudioReferencesArraysService.Play2DSound(EAudioCategory.SFX, "Close Menu");
        }

        public void InverseActive(string canvasName)
        {
            FadeInFadeOutCanvas fadeInFadeOutCanvas = this.fadeInFadeOutCanvases[base.GetHashId(canvasName)];

            if (fadeInFadeOutCanvas.IsOpen())
                this.Close(canvasName);
            else
                this.Open(canvasName);
        }

        public void CloseAllCanvases()
        {
            Array.ForEach(this.fadeInFadeOutCanvases, fadeInFadeOutCanvas =>
            {
                if (fadeInFadeOutCanvas.IsOpen())
                    this.Close(fadeInFadeOutCanvas.name);
            });
        }

        public void SetCanvasTransformAtTop(Canvas topCanvas)
        {
            Array.ForEach(this.canvases, canvas =>
            {
                canvas.sortingOrder -= 1;
            });

            topCanvas.sortingOrder = 1;
        }

        public void SubscribeToACanvasIsOpenModifiedCallback(Action<bool> callback)
        {
            this.ACanvasIsOpenListener += callback;
        }

        public Vector2 GetCanvasResolutionRatio()
        {
            return EResolutionType.Build == PlayerServicesAndModulesContainer.Instance.ResolutionType ?
                    new Vector2(this.buildResolution.x / this.developmentResolution.x,
                                this.buildResolution.y / this.developmentResolution.y) :
                    Vector2.one;
        }
        #endregion

        #region Intern Behaviour
        private void UpdateIsACanvasIsOpen()
        {
            bool isOpen = false;

            Array.ForEach(this.fadeInFadeOutCanvases, fadeInFadeOutCanvas =>
            {
                if (fadeInFadeOutCanvas.IsOpen())
                    isOpen = true;
            });

            this.ACanvasIsOpen = isOpen;
        }

        private void InverseActiveCanvasesOnInputDown()
        {
            if (null != this.canvasesConfigurations)
            {
                Array.ForEach(this.canvasesConfigurations, canvasConfiguration =>
                {
                                // Si les touches de la fenêtres sont appuyés et que l'utilisateur n'a pas le focus sur un champ de texte, il peut inverse l'état de la fenêtre (ouvrir / fermer).
                                if (PlayerServicesAndModulesContainer.Instance.InputService.IsDown(canvasConfiguration.Input) &&
                            null == EventSystem.current.currentSelectedGameObject)
                    {
                        if (canvasConfiguration.ShowOnlyOnDebugMode &&
                            PlayerServicesAndModulesContainer.Instance.DebugMode ||
                            !canvasConfiguration.ShowOnlyOnDebugMode)
                            this.InverseActive(canvasConfiguration.name);
                    }
                });
            }
        }

        private Vector2 GetCanvasResolutionReference()
        {
            return EResolutionType.Build == PlayerServicesAndModulesContainer.Instance.ResolutionType ?
                this.buildResolution :
                this.developmentResolution;
        }
        #endregion
    }
}