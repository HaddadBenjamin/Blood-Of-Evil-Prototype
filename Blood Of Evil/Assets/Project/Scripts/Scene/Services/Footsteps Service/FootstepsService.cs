using UnityEngine;
using System.Collections;
using System; // Array.Find
using System.Linq; // IEnumerable.Where

namespace BloodOfEvil.Scene.Services.Footsteps
{
    using Helpers;
    using Extensions;
    using Player;

    public class FootstepsService : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private FootstepCategory[] footstepCategories;
        private RaycastHit hit;
        private Ray ray;
        private Terrain terrain;
        #endregion

        #region Public Behaviour
        /// <summary>
        /// Play a footstep if it found a material in the foostepCategory.
        /// AudioSource -> Will change when I will integrate WWise.
        /// </summary>
        public void PlayFootstep(Transform footTransform, AudioSource audioSource)
        {
            // 1) Raycast bottom. : V.
            // 1.1) Collide with the ground : X. WHY ?
            // 2) Get all materials : V.
            // 3) Search it in my footstep categories : V.
            // 4) if found play a random sound of this category and return : V.

            // Ray from footstep position to the ground.
            this.ray = new Ray(footTransform.position, -Vector3.up * 10.0f);

            // If the foot raycast with something.
            if (Physics.Raycast(this.ray, out this.hit))
            {
                Renderer renderer = hit.collider.GetComponent<Renderer>();

                // Show the footsteps raycast in the inspector.
                Debug.DrawRay(ray.origin, ray.direction, Color.green, 2.0f);

                if (null != renderer)
                {
                    // Retrieve the ground materials.
                    Material[] materials = renderer.materials;

                    // If one is found in the footsteps categories, jusst play the sound.
                    foreach (Material material in materials)
                    {
                        foreach (FootstepCategory footstepCategory in this.footstepCategories)
                        {
                            // Test the texture from an object at footstep position, if don't exists try on the terrain.
                            if (footstepCategory.PlayIfTextureIsFound(material.mainTexture, audioSource))
                                return;

                            // Why the terrain is null here ? it looks to be correctly initialized at UpdateTerrain.
                            Texture terraintexture = this.terrain.GetTextureAtPosition(hit.transform.position);

                            if (null != terraintexture)
                                // Display the terrain texture name to understand why I don't find it.
                                Debug.LogFormat("Terrain texture : {0}", terraintexture.name);

                            if (footstepCategory.PlayIfTextureIsFound(terraintexture, audioSource) &&
                                !string.IsNullOrEmpty(terraintexture.name))
                                return;
                        }
                    }
                }
            }
        }

        public void UpdateTerrain(Terrain terrain)
        {
            this.terrain = terrain;
        }
        #endregion
    }

    [System.Serializable]
    public class FootstepCategory
    {
        #region Fields
        /// <summary>
        /// Sand, wood, rock, etc..
        /// </summary>
        [SerializeField]
        private string categoryName;
        /// <summary>
        /// All materials of this category.
        /// </summary>
        [SerializeField]
        private Texture[] textures;
        /// <summary>
        /// Later with wwise integration it will be just a string "soundEventName".
        /// When one of the materials is found, we can play one random sound in this array.
        /// </summary>
        [SerializeField]
        private AudioClip[] clips;
        #endregion

        #region Public Behaviour
        /// <summary>
        /// Play a random footstep sound if the material is found in this footstep category.
        /// </summary>
        public bool PlayIfTextureIsFound(Texture texture, AudioSource audioSource)
        {
            // Double search -> ugly, to fix. 
            Texture textureFound = Array.Find(this.textures, textureElement => texture.name.Contains(textureElement.name));

            bool textureIsFound = null != textureFound;

            if (textureIsFound)
                this.PlayARandomFootstepSound(audioSource);

            return textureIsFound;
        }
        #endregion

        static int i;
        #region Intern Behaviour
        /// <summary>
        /// Play a random footstep clip.
        /// </summary>
        private void PlayARandomFootstepSound(AudioSource audioSource)
        {
            // Play the footstep if the footstep category contains at least 1 audioClip.
            if (null != audioSource &&
                this.clips.Length > 0)
            {
                AudioClip footstepClip = this.clips[MathHelper.GenerateRandomBeetweenTwoInts(0, this.clips.Length - 1)];

                //Debug.LogFormat("Footstep sound : {0}", i++);

                audioSource.volume = PlayerServicesAndModulesContainer.Instance.AudioService.GeVolumeOfAnAudioCategory(Player.Services.Audio.EAudioCategory.SFX);

                audioSource.PlayOneShot(footstepClip);
            }
        }
        #endregion
    }
}