using System.Collections.Generic;

namespace BloodOfEvil.Utilities.Serialization
{
    [System.Serializable]
    public class SerializableStringArray
    {
        #region Fields
        public List<string> StringArray;
        #endregion

        #region Constructor
        public SerializableStringArray() { }
        public SerializableStringArray(string[] stringArray)
        {
            this.StringArray = new List<string>();

            for (int i = 0; i < stringArray.Length; i++)
            {
                if (null != stringArray[i])
                    this.StringArray.Add(stringArray[i]);
            }
        }
        #endregion
    }
}