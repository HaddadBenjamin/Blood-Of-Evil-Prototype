using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Entities.Modules.Footsteps
{
    using Scene;

    public class FootstepsModule : MonoBehaviour
    {
        #region Fields
        [SerializeField, Tooltip("Foot transforms.")]
        private Transform leftFootTransform,
                          rightFootTransform;
        /// <summary>
        /// Those audioclip will be modified when I will integrate WWise package.
        /// </summary>
        [SerializeField, Tooltip("Foot audio sources.")]
        private AudioSource leftFootAudioSource,
                            rightFootAudioSource;
        #endregion

        #region Public Behaviour
        /// <summary>
        /// Call the footstep service with the left footsteps parameters.
        /// </summary>
        public void LeftFootStepAction()
        {
            SceneServicesContainer.Instance.FootstepService.PlayFootstep(this.leftFootTransform, this.leftFootAudioSource);
        }

        /// <summary>
        /// Call the footstep service with the right footsteps parameters.
        /// </summary>
        public void RightFootStepAction()
        {
            SceneServicesContainer.Instance.FootstepService.PlayFootstep(this.rightFootTransform, this.rightFootAudioSource);
        }
        #endregion
    }
}