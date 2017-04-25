using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloodOfEvil.Utilities.Cameras
{
    /// <summary>
    /// C'est la caméra d'un jeu d'infiltration type FPS.
    /// Le curseur de la souris est la cible de la caméra mais le joueur doit toujours être visible par la caméra.
    /// On peut faire trembler la caméra avec une touche.
    /// </summary>
    public class InfiltrationCamera : MonoBehaviour
    {
        #region Fields
        #region Camera Destination
        [SerializeField, Tooltip("C'est la caméra du joueur"), Header("Move")]
        private Camera playerCamera;
        [SerializeField, Tooltip("C'est la cible de la caméra, le joueur.")]
        private Transform cameraTarget;
        [SerializeField, Tooltip("C'est la vitesse de déplacement de la caméra.")]
        private float cameraMoveSpeed = 10.0f;

        /// <summary>
        /// C'est la destination de la caméra, elle va se déplacer jusqu'à atteindre cette position.
        /// </summary>
        private Vector3 cameraDestination;
        #endregion

        #region Shake Configuration
        [SerializeField, Tooltip("C'est la durée d'un tremblement de la caméra."), Header("Shake")]
        private float shakeDuration = 0.5f;
        [SerializeField, Tooltip("C'est la puissance du tremblement de la caméra.")]
        private float shakeIntensity = 0.15f;
        [SerializeField, Tooltip("C'est la touche à appuyer pour faire trembler la caméra.")]
        private KeyCode shakeKeyCode = KeyCode.Mouse1;

        /// <summary>
        /// C'est le vecteur de déplacement de tremblement de la caméra.
        /// </summary>
        private Vector3 shakeVelocity;
        #endregion
        #endregion

        #region Unity Behaviour

        #region A DEGAGER
        // Na rien à faire là, mais c'est du temporaire.
        [SerializeField]
        private Texture2D FPSCursorTexture;
        private void Start()
        {
            Cursor.SetCursor(FPSCursorTexture, Vector2.zero, CursorMode.Auto);
        }
        #endregion

        private void Update()
        {
            if (Input.GetKeyDown(this.shakeKeyCode))
                StartCoroutine(this.ShakeCamera());
        }

        private void FixedUpdate()
        {
            this.UpdateCameraDestination();

            this.UpdateCameraPosition();
        }
        #endregion

        #region Intern Behaviour
        /// <summary>
        /// Met à jour la position de la caméra de façon fluide et lui applique le vecteur de tremblement.
        /// </summary>
        private void UpdateCameraPosition()
        {
            this.playerCamera.transform.position += this.shakeVelocity;

            this.playerCamera.transform.position = Vector3.Slerp(
                this.playerCamera.transform.position,
                this.cameraDestination,
                Time.deltaTime * this.cameraMoveSpeed);
        }

        /// <summary>
        /// Calcul la destination de la caméra.
        /// Le curseur de la souris est la cible de la caméra mais le joueur doit toujours être visible par la caméra.
        /// </summary>
        private void UpdateCameraDestination()
        {
            Plane plane = new Plane(Vector3.up, 0);
            float dist;
            Ray ray = this.playerCamera.ScreenPointToRay(Input.mousePosition);

            if (plane.Raycast(ray, out dist))
            {
                Vector3 planeCollisonPoint = ray.GetPoint(dist);
                Vector3 playerPositionToPoint = planeCollisonPoint - this.cameraTarget.position;

                this.cameraDestination =
                    new Vector3(
                    planeCollisonPoint.x - (playerPositionToPoint.x * 0.2f),
                    this.playerCamera.transform.position.y,
                    planeCollisonPoint.z - (playerPositionToPoint.z * 0.2f));
            }
        }

        // Devrait être dans le code de la caméra mais osef.
        private IEnumerator ShakeCamera()
        {
            float elapsed = 0.0f;

            while (elapsed < shakeDuration)
            {
                elapsed += Time.deltaTime;

                float percentComplete = elapsed / shakeDuration;
                float horizontalShake = this.shakeIntensity * Random.value * (Random.Range(0, 2) == 1 ? - 1.0f : 1.0f) * percentComplete;
                float verticalShake = this.shakeIntensity * Random.value * (Random.Range(0, 2) == 1 ? -1.0f : 1.0f) * percentComplete;

                this.shakeVelocity = new Vector3(horizontalShake, 0.0f, verticalShake);

                yield return null;
            }

            shakeVelocity = Vector3.zero;
        }
        #endregion
    }
}
