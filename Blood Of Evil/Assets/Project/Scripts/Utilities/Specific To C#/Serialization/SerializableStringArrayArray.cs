using System.Collections.Generic;

namespace BloodOfEvil.Utilities.Serialization
{
    [System.Serializable]
    public class SerializableStringArrayArray
    {
        #region Fields
        public List<SerializableStringArray> StringArrayArray;
        #endregion

        #region Constructor
        public SerializableStringArrayArray() { }
        public SerializableStringArrayArray(string[][] stringArrayArrayNotSerializable)
        {
            this.StringArrayArray = new List<SerializableStringArray>();

            for (int stringArrayArrayIndex = 0; stringArrayArrayIndex < stringArrayArrayNotSerializable.Length; stringArrayArrayIndex++)
            {
                //Debug.Log(this.StringArrayArray);
                if (null != stringArrayArrayNotSerializable[stringArrayArrayIndex])
                    this.StringArrayArray.Add(new SerializableStringArray(stringArrayArrayNotSerializable[stringArrayArrayIndex]));
            }
        }
        #endregion

        #region Public Behaviour
        public void StringArrayArrayToSerializableStringArrayArray(ref string[][] stringArrayArrayNotSerializable)
        {
            stringArrayArrayNotSerializable = new string[this.StringArrayArray.Count][];

            for (int stringArrayArraIndex = 0; stringArrayArraIndex < StringArrayArray.Count; stringArrayArraIndex++)
            {
                stringArrayArrayNotSerializable[stringArrayArraIndex] = this.StringArrayArray[stringArrayArraIndex].StringArray.ToArray();
            }


        }
        #endregion
    }
}