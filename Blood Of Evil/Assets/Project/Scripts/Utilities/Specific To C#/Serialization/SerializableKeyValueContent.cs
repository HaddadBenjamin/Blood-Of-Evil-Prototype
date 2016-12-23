namespace BloodOfEvil.Utilities.Serialization
{
    [System.Serializable]
    // J'ai tellement le seum, je ne peux pas le sérializer car ma classe est template. fuck this.
    public sealed class SerializableKeyValueContent<TKey, TValue>
    {
        #region Fields
        public TKey Key;
        public TValue Value;
        #endregion

        #region Public Behaviour
        public void Initialize(TKey key, TValue value)
        {
            this.Key = key;
            this.Value = value;
        }
        #endregion
    }
}