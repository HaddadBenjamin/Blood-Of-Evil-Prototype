namespace BloodOfEvil.Player.Services.Game.Serialization
{
    using Utilities;
    using Keys;

    [System.Serializable]
    public class GameServiceSerializable
    {
        #region Fields
        public bool AutoSave;
        public float AutoSaveEveryXSeconds;
        #endregion

        #region Constructor
        public GameServiceSerializable() { }
        #endregion

        #region Save & Load Behaviour.
        /// <summary>
        /// Save.
        /// </summary>
        /// <param name="attribute"></param>
        public GameServiceSerializable(Player.Services.Game.GameService gameService)
        {
            this.AutoSave = gameService.AutoSave;
            this.AutoSaveEveryXSeconds = gameService.AutoSaveEveryXSeconds;
        }

        public void Load(Player.Services.Game.GameService gameService)
        {
            gameService.AutoSave = this.AutoSave;
            gameService.AutoSaveEveryXSeconds = this.AutoSaveEveryXSeconds;
            gameService.AutoSaveTimer = new Timer(gameService.AutoSaveEveryXSeconds, false);
        }
        #endregion
    }
}