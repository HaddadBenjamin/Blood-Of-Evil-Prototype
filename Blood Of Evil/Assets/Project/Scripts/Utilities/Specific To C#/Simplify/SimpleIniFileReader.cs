using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using StringDictionary = System.Collections.Generic.Dictionary<string, string>;

namespace BloodOfEvil.Utilities
{
    public class SimpleIniFileReader
    {
        #region Attributes
        private StringDictionary keysValues;
        private string path;
        #endregion

        #region Properties
        public StringDictionary KeysValues { get { return keysValues; } private set { keysValues = value; } }
        public string Path { get { return path; } private set { path = value; } }
        #endregion

        #region builder
        public SimpleIniFileReader(string iniFilePath)
        {
            this.keysValues = new StringDictionary();
            this.path = iniFilePath;

            try
            {
                using (TextReader file = new StreamReader(this.path))
                {
                    for (string line = file.ReadLine(); null != line; line = file.ReadLine())
                    {
                        string[] keyValue = line.Split(new char[] { '=' }, 2);

                        this.keysValues.Add(keyValue[0], keyValue[1]);
                    }
                }
            }
            catch (Exception exception) { Debug.Log(exception.Message); }
        }
        #endregion
        #region functions
        public void SaveFile(StringDictionary fileRepresentation = null)
        {
            try
            {
                using (TextWriter file = new StreamWriter(this.path))
                {
                    if (null != fileRepresentation)
                        this.keysValues = new StringDictionary(fileRepresentation);

                    List<KeyValuePair<string, string>> keysValuesList = this.keysValues.ToList();
                    string fileContent = "";

                    keysValuesList.ForEach(node => fileContent += node.Key + "=" + node.Value + "\r\n");
                    file.Write(fileContent);
                }
            }
            catch (Exception exception) { Debug.Log(exception.Message); }
        }

        public void AddLineAndSave(string key, string value)
        {
            this.keysValues.Add(key, value);
            this.SaveFile();
        }

        public void RemoveLineAndSave(string key)
        {
            if (this.keysValues.ContainsKey(key))
            {
                this.keysValues.Remove(key);
                this.SaveFile();
            }
        }
        #endregion
    }
}