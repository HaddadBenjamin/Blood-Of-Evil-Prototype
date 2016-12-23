using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace BloodOfEvil.Enemies
{
    using Modules.Attributes;

    using Player;
    using Player.Services.Language;
    using Player.Services.Language.UI;

    public class UpdateEnemyNameText : MonoBehaviour
    {
        [SerializeField]
        private EnemyAttributesModule enemyAttributes;

        private Text text;

        void Awake()
        {
            this.text = GetComponent<Text>();

            this.enemyAttributes.EnemyCategoryListener += delegate (EEnemyCategory category)
            {
                this.UpdateText(category, this.enemyAttributes.name);
            };
        }

        private void UpdateText(EEnemyCategory category, string objectName)
        {
            GetComponent<UpdateLanguageText>().UpdateDefaultText(objectName);

            this.text.color = GetColorCategory(category);
        }

        private static Color GetColorCategory(EEnemyCategory category)
        {
            switch (category)
            {
                case EEnemyCategory.WorldBoss:
                    return Color.red;
                case EEnemyCategory.Boss:
                    return Color.yellow;
                case EEnemyCategory.Gobelin:
                    return Color.grey;
                case EEnemyCategory.Gozu:
                    return Color.cyan;
                case EEnemyCategory.Champion:
                    return Color.blue;

                default:
                    return Color.white;
            }
        }
    }
}