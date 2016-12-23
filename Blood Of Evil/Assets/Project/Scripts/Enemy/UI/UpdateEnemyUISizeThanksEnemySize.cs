using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Enemies
{
    using Modules.Attributes;

    public class UpdateEnemyUISizeThanksEnemySize : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private EnemyAttributesModule enemyAttributes;

        const float DAMAGE_UI_DEFAULT_LOCAL_SIZE = 0.005f;
        #endregion

        #region Unity Behaviour
        void Start()
        {
            this.enemyAttributes.EnemySizeListener += delegate (float enemySize)
            {
                transform.localScale = Vector3.one * DAMAGE_UI_DEFAULT_LOCAL_SIZE / enemySize;
            };
        }
        #endregion
    }
}