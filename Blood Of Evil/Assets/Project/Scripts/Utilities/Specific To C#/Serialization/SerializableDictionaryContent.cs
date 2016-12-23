using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace BloodOfEvil.Utilities.Serialization
{
    // J'ai tellement le seum, je ne peux pas le sérializer car ma classe est template. fuck this.
    [System.Serializable]
    public sealed class SerializableDictionaryContent<TKey, TValue>
    {
        #region Fields
        public SerializableKeyValueContent<TKey, TValue>[] Content;
        #endregion

        #region Constructor
        public SerializableDictionaryContent() { }

        public SerializableDictionaryContent(TKey[] keys, TValue value)
        {
            this.Content = new SerializableKeyValueContent<TKey, TValue>[keys.Length];

            int index = 0;
            Array.ForEach(keys, key =>
            {
                    //Debug.LogFormat("index : {0}, keys.Length : {1}", index, keys.Length);
                    this.Content[index] = new SerializableKeyValueContent<TKey, TValue>();
                this.Content[index].Initialize(key, value);

                index++;
            });
        }
        #endregion

        #region Public Behaviour
        public void DictionaryToSerializableDictionaryContent(Dictionary<TKey, TValue> dictionary)
        {
            this.Content = new SerializableKeyValueContent<TKey, TValue>[dictionary.Count];

            int index = 0;
            foreach (KeyValuePair<TKey, TValue> entry in dictionary)
            {
                this.Content[index].Initialize(entry.Key, entry.Value);

                index++;
            }
        }

        public void SerializableDictionaryContentToDictionary(Dictionary<TKey, TValue> dictionary)
        {
            Array.ForEach(this.Content, entry => dictionary.Add(entry.Key, entry.Value));
        }

        public void Clear()
        {
            Array.Clear(this.Content, 0, this.Content.Length);
        }
        #endregion
    }
}