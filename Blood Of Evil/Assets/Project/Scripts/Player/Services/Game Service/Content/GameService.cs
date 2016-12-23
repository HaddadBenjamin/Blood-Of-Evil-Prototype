using UnityEngine;
using System.Collections;
using System;

namespace BloodOfEvil.Player.Services.Game
{
    using Scene;
    using Helpers;
    using Utilities;
    using ObjectInScene;

    using Serialization;

    public class GameService : ISerializable, IDataInitializable, IDataUpdatable
    {
        #region Fields
        private bool autoSave;
        private float autoSaveEveryXSeconds;
        private Timer autoSaveTimer = new Timer(60.0f, false);
        #endregion

        #region Properties
        public bool AutoSave
        {
            get
            {
                return autoSave;
            }

            set
            {
                autoSave = value;
            }
        }

        public float AutoSaveEveryXSeconds
        {
            get
            {
                return autoSaveEveryXSeconds;
            }

            set
            {
                autoSaveEveryXSeconds = value;
            }
        }

        public Timer AutoSaveTimer
        {
            get
            {
                return autoSaveTimer;
            }

            set
            {
                autoSaveTimer = value;
            }
        }
        #endregion

        #region IAbstract Behaviour
        #endregion

        #region Interface Behaviour
        void IDataInitializable.Initialize()
        {
            this.AutoSave = true;
            this.AutoSaveEveryXSeconds = 60.0f;

            ((ISerializable)this).Load();
        }

        void ISerializable.Load()
        {
            string gameFileName = this.GetFileName();

            if (SerializerHelper.DoesCompletSavePathExists(gameFileName, ".json"))
                SerializerHelper.JsonDeserializeLoad<GameServiceSerializable>(gameFileName).Load(this);
        }

        void ISerializable.Save()
        {
            SerializerHelper.JsonSerializeSave(new GameServiceSerializable(this), this.GetFileName());
        }

        void IDataUpdatable.Update()
        {
            if (this.AutoSave &&
                this.AutoSaveTimer.IsRingingUpdated())
                SceneManagerHelper.SaveCurrentScene();
        }
        #endregion

        private string GetFileName()
        {
            return SceneServicesContainer.Instance.FileSystemConfiguration.GameSettingsFilename;
        }

        #region Override Behaviour
        #endregion
    }
}