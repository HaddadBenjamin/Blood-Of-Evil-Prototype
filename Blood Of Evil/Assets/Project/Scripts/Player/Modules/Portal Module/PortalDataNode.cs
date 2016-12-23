using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace BloodOfEvil.Player.Modules.Portails
{
    [System.Serializable]
    public class PortalDataNode
    {
        #region Fields
        private Vector3 position = Vector3.zero;
        private Vector3 eulerAngles = Vector3.zero;
        [SerializeField]
        private string sceneName = "";// SceneManager.GetActiveScene().name;
        #endregion

        #region Properties
        public Vector3 Position
        {
            get
            {
                return position;
            }

            set
            {
                position = value;
            }
        }

        public Vector3 EulerAngles
        {
            get
            {
                return eulerAngles;
            }

            set
            {
                eulerAngles = value;
            }
        }

        public string SceneName
        {
            get
            {
                return sceneName;
            }

            set
            {
                sceneName = value;
            }
        }
        #endregion
    }
}