using UnityEngine;
using System.Collections;
using System;

namespace BloodOfEvil.Player.Modules.Cursors
{
    using Scene;
    using Helpers;
    using Extensions;
    using Enemies;
    using ObjectInScene;

    public class CursorModule : AInitializableComponent
    {
        #region Fields
        private Texture2D cursorTexture;
        private CursorMode cursorMode = CursorMode.Auto;
        private Vector2 hotSpot = Vector2.zero;
        private ECursor cursor = ECursor.Attack;

        //private Transform myTransform;
        private Ray ray;
        private RaycastHit raycastHit;
        #endregion

        #region Properties
        public ECursor Cursor
        {
            get { return cursor; }

            set
            {
                if (cursor != value)
                {
                    cursor = value;

                    this.cursorTexture = SceneServicesContainer.Instance.TextureReferencesService.Get(
                            EnumerationHelper.EnumerationToString(this.cursor).ReplaceUppercaseBySpaceAndUppercase());

                    this.UpdateCursor();
                }
            }
        }
        #endregion

        #region Abstract Behaviour

        public override void Initialize()
        {
            //this.myTransform = transform;
        }

        #endregion

        #region Public Behaviour
        void FixedUpdate()
        {
            this.ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(this.ray, out this.raycastHit))
            {
                GameObject objectHit = this.raycastHit.collider.gameObject;

                this.SetCursor(LayerMask.LayerToName(objectHit.layer), objectHit);
            }
            else
                this.Cursor = ECursor.Move;
        }
        #endregion

        #region Intern Behaviour
        private void UpdateCursor()
        {
            UnityEngine.Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
        }

        private void SetCursor(string layerName, GameObject objectHit)
        {
            if ("Enemy" == layerName)
            {
                EEnemyCategory category = objectHit.transform.GetComponent<EnemyServicesAndModulesContainer>().Instance.AttributesModule.Category;

                this.Cursor = (EEnemyCategory.Boss == category ||
                                EEnemyCategory.WorldBoss == category ||
                                EEnemyCategory.Gobelin == category) ?
                                   ECursor.AttackBoss :
                                   ECursor.Attack;
            }
            //else if ("Portail" == layerName)
            //    this.Cursor = ECursor.Portail;
            else
                this.Cursor = ECursor.Move;
        }
        #endregion
    }
}