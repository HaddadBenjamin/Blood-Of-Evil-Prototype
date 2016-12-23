using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Scene.Services.Time
{
    public sealed class TimeService
    {
        public float TimeScale;

        public float DeltaTime()
        {
            return UnityEngine.Time.deltaTime * this.TimeScale;
        }
    }
}