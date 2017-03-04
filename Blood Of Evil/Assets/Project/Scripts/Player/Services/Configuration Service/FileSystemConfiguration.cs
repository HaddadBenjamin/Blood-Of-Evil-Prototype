using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Player.Services.Configuration
{
    using Helpers;

    [System.Serializable]
    public class FileSystemConfiguration
    {
        #region Fields
        [SerializeField, Header("Player Settings")]
        private string videoSettingsFilename;
        [SerializeField]
        private string audioSettingsFilename;
        [SerializeField]
        private string keysSettingsFilename;
        [SerializeField]
        private string gameSettingsFilename;
        [SerializeField]
        private string languageSettingsDirectoryName;
        [SerializeField]
        private string languageSettingLanguageChooseFileName;

        [SerializeField, Header("Player Gameplay")]
        private string attributesFilename;
        [SerializeField]
        private string characteristicsPointsAddedButNotApplyedFilename;
        [SerializeField]
        private string sceneNameFilename;

        [SerializeField, Header("Scene")]
        private string scenesDirectoryName;

        [SerializeField, Header("Enemies")]
        private string enemiesDirectoryName;
        #endregion

        #region Properties
        public string CharacteristicsPointsAddedButNotApplyedFilename
        {
            get
            {
                return characteristicsPointsAddedButNotApplyedFilename;
            }

            private set
            {
                characteristicsPointsAddedButNotApplyedFilename = value;
            }
        }

        public string VideoSettingsFilename
        {
            get
            {
                return videoSettingsFilename;
            }

            private set
            {
                videoSettingsFilename = value;
            }
        }

        public string AudioSettingsFilename
        {
            get
            {
                return audioSettingsFilename;
            }

            private set
            {
                audioSettingsFilename = value;
            }
        }

        public string KeysSettingsFilename
        {
            get
            {
                return keysSettingsFilename;
            }

            private set
            {
                keysSettingsFilename = value;
            }
        }

        public string LanguageSettingsDirectoryName
        {
            get
            {
                return languageSettingsDirectoryName;
            }
        }

        public string LanguageSettingLanguageChooseFileName
        {
            get
            {
                return languageSettingLanguageChooseFileName;
            }

            private set
            {
                languageSettingLanguageChooseFileName = value;
            }
        }

        public string AttributesFilename
        {
            get
            {
                return attributesFilename;
            }

            private set
            {
                attributesFilename = value;
            }
        }

        public string GameSettingsFilename
        {
            get
            {
                return gameSettingsFilename;
            }

            set
            {
                gameSettingsFilename = value;
            }
        }

        public string SceneNameFilename
        {
            get
            {
                return sceneNameFilename;
            }

            set
            {
                sceneNameFilename = value;
            }
        }

        public string ScenesDirectoryName
        {
            get
            {
                return scenesDirectoryName;
            }

            set
            {
                scenesDirectoryName = value;
            }
        }

        public string EnemiesDirectoryName
        {
            get
            {
                return enemiesDirectoryName;
            }

            set
            {
                enemiesDirectoryName = value;
            }
        }

        public string PositionAndRotationFilename
        {
            get
            {
                return string.Format("{0}/{1}/{2}",
                    this.GetCurrentSceneDirectory(),
                    "Player",
                    "Position And Euler Angles");
            }
        }


        public string GetCurrentSceneDirectory()
        {
            return string.Format("{0}/{1}",
                ScenesDirectoryName,
                SceneManagerHelper.GetCurrentSceneName());
        }

        //public string EnemyDeathFilename(int enemySaveIndex)
        //{
        //    return string.Format("{0}/{1}",
        //        this.GetEnemyDirectory(enemySaveIndex),
        //        "Death State");
        //}

        public string EnemyAttributesFilename(int enemySaveIndex)
        {
            return string.Format("{0}/{1}",
                this.GetEnemyDirectory(enemySaveIndex),
                "Attributes");
        }

        public string EnemyIAInitialPositionFilename(int enemySaveIndex)
        {
            return string.Format("{0}/{1}",
                this.GetEnemyDirectory(enemySaveIndex),
                "IA Initial Position And Euler Angles");
        }

        public string EnemyPositionAndRotationFilename(int enemySaveIndex)
        {
            return string.Format("{0}/{1}",
                this.GetEnemyDirectory(enemySaveIndex),
                "Position And Euler Angles");
        }

        public string EnemyCategoryFilename(int enemySaveIndex)
        {
            return string.Format("{0}/{1}",
                this.GetEnemyDirectory(enemySaveIndex),
                "Category");
        }

        public string GetEnemyDirectory(int enemySaveIndex)
        {
            return string.Format("{0}/{1}/{2}",
                this.GetCurrentSceneDirectory(),
                EnemiesDirectoryName,
                enemySaveIndex);
        }

        public string GetEnemyDirectoryName()
        {
            return string.Format("{0}/{1}",
                this.GetCurrentSceneDirectory(),
                EnemiesDirectoryName);
        }
        #endregion
    }
}