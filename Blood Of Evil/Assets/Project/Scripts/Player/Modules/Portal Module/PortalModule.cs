using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

namespace BloodOfEvil.Player.Modules.Portails
{
    using Scene;
    using Helpers;
    using ObjectInScene;

    using Services.Keys;
    using Services.Audio;

    public class PortalModule : AInitializableComponent
    {
        #region Fields
        [SerializeField]
        private PortalDataNode startPortal;
        [SerializeField]
        private PortalDataNode endPortal;
        private Transform myTransform;

        private PortalNode startPortalNode;
        private PortalNode endPortalNode;

        private KeysService keysService;
        #endregion

        #region Unity Behaviour
        void Update()
        {
            if (this.keysService.IsDown(EPlayerInput.Portal))
                StartCoroutine(this.CreateTeleport());
        }
        #endregion

        #region Abstract Behaviour
        public override void Initialize()
        {
            this.myTransform = transform;

            this.startPortal.Position = this.GetPortalStartPosition();
            this.endPortal.Position = this.startPortal.Position + Vector3.right * -8.0f;

            this.keysService = PlayerServicesAndModulesContainer.Instance.InputService;
        }
        #endregion

        #region Intern Behaviour
        private IEnumerator CreateTeleport()
        {
            SceneServicesContainer.Instance.AudioReferencesArraysService.Play2DSound(EAudioCategory.SFX, "Portal");

            PlayerServicesAndModulesContainer.Instance.ObjectsPoolService.RemoveAllObjectInPool("Blue Portal Opening");
            PlayerServicesAndModulesContainer.Instance.ObjectsPoolService.RemoveAllObjectInPool("Blue Portal Stay Open");
            PlayerServicesAndModulesContainer.Instance.ObjectsPoolService.RemoveAllObjectInPool("Blue Portal Closing");

            Vector3 portalStartPosition = this.GetPortalStartPosition();

            PlayerServicesAndModulesContainer.Instance.ObjectsPoolService.AddObjectInPool("Blue Portal Opening", portalStartPosition, this.startPortal.EulerAngles);

            if (this.endPortal.SceneName == SceneManager.GetActiveScene().name)
                PlayerServicesAndModulesContainer.Instance.ObjectsPoolService.AddObjectInPool("Blue Portal Opening", this.endPortal.Position, this.endPortal.EulerAngles);

            yield return new WaitForSeconds(0.8f);
            PlayerServicesAndModulesContainer.Instance.ObjectsPoolService.RemoveAllObjectInPool("Blue Portal Opening");

            this.startPortalNode = PlayerServicesAndModulesContainer.Instance.ObjectsPoolService.
                AddObjectInPool("Blue Portal Stay Open", portalStartPosition, this.startPortal.EulerAngles).
                GetComponent<PortalNode>();
            this.startPortalNode.Initialize(this.startPortal, this.endPortal, EPortal.Source, EPortal.Destination);

            if (this.endPortal.SceneName == SceneManager.GetActiveScene().name)
            {
                this.endPortalNode = PlayerServicesAndModulesContainer.Instance.ObjectsPoolService.
                    AddObjectInPool("Blue Portal Stay Open", this.endPortal.Position, this.endPortal.EulerAngles).
                    GetComponent<PortalNode>();

                this.endPortalNode.Initialize(this.endPortal, this.startPortal, EPortal.Destination, EPortal.Source);
            }
            // Initialize.
        }

        private Vector3 GetPortalStartPosition()
        {
            return this.myTransform.position + this.myTransform.up * 1.0f + Vector3.forward * 2.0f;
        }
        #endregion

        #region Public Behaviour
        public IEnumerator Teleport(PortalNode portalNode)
        {
            SceneServicesContainer.Instance.AudioReferencesArraysService.Play2DSound(EAudioCategory.SFX, "Portal");

            PlayerServicesAndModulesContainer.Instance.ObjectsPoolService.AddObjectInPool("Blue Portal Closing", portalNode.DataSource.Position, portalNode.DataSource.EulerAngles);
            PlayerServicesAndModulesContainer.Instance.ObjectsPoolService.RemoveObjectInPool("Blue Portal Stay Open", this.startPortalNode.gameObject);

            yield return new WaitForSeconds(2f);

            if (portalNode.DataDestination.SceneName != portalNode.DataSource.SceneName)
                SceneManagerHelper.LoadScene(portalNode.DataDestination.SceneName);

            this.myTransform.position = portalNode.DataDestination.Position;
            this.myTransform.eulerAngles = portalNode.DataDestination.EulerAngles;

            PlayerServicesAndModulesContainer.Instance.ObjectsPoolService.AddObjectInPool("Blue Portal Closing", this.endPortal.Position, this.endPortal.EulerAngles);

            yield return new WaitForSeconds(1f);

            PlayerServicesAndModulesContainer.Instance.ObjectsPoolService.RemoveAllObjectInPool("Blue Portal Closing");
        }
        #endregion
    }
}