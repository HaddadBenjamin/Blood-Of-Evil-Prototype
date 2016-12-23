using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Scene.Services.Footsteps
{
    [RequireComponent(typeof(Terrain))]
    public class UpdateFootstepTerrain : MonoBehaviour
    {
        #region Unity Behaviour
        private void Awake()
        {
            Scene.SceneServicesContainer.Instance.FootstepService.UpdateTerrain(GetComponent<Terrain>());
        }
        #endregion
    }
}