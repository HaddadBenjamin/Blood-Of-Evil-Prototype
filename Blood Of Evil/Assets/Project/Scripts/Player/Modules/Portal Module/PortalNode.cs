using UnityEngine;
using System.Collections;
using System;

namespace BloodOfEvil.Player.Modules.Portails
{
    using Utilities.UI;

    public class PortalNode : MonoBehaviour
    {
        #region Fields
        private PortalDataNode dataSource;
        private PortalDataNode dataDestination;
        private EPortal portalSource;
        private EPortal portalDestination;
        #endregion

        #region Properties
        public EPortal PortalSource
        {
            get
            {
                return portalSource;
            }

            private set
            {
                portalSource = value;
            }
        }

        public EPortal PortalDestination
        {
            get
            {
                return portalDestination;
            }

            private set
            {
                portalDestination = value;
            }
        }

        public PortalDataNode DataSource
        {
            get
            {
                return dataSource;
            }

            private set
            {
                dataSource = value;
            }
        }

        public PortalDataNode DataDestination
        {
            get
            {
                return dataDestination;
            }

            private set
            {
                dataDestination = value;
            }
        }
        #endregion

        #region Public Behaviour
        public void Initialize(PortalDataNode dataSoure, PortalDataNode dataDestination, EPortal portalSource, EPortal portalDestination)
        {
            this.DataDestination = dataDestination;
            this.DataSource = dataSoure;
            this.PortalSource = portalSource;
            this.PortalDestination = portalDestination;

            GetComponent<Tooltip3DNode>().UpdateLanguageTextAndAddASuit.UpdateDefaultText(DataDestination.SceneName);
        }

        void OnTriggerEnter(Collider other)
        {
            if ("Player" == other.tag)
                StartCoroutine(PlayerServicesAndModulesContainer.Instance.PortalModule.Teleport(this));
        }
        #endregion
    }
}