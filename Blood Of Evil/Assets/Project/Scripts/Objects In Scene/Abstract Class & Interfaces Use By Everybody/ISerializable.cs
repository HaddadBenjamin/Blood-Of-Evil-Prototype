using UnityEngine;
using System.Collections;

namespace BloodOfEvil.ObjectInScene
{
    public interface ISerializable
    {
        void Load();
        void Save();
    }
}