using System.Reflection;
using UnityEngine;

namespace NGTools
{
	public class AudioSourceExposer : ComponentExposer
	{
		public	AudioSourceExposer() : base(typeof(AudioSource))
		{
		}

		public override PropertyInfo[]	GetPropertyInfos()
		{
			PropertyInfo[]	fields = new PropertyInfo[] {
				this.type.GetProperty("bypassEffects"),
				this.type.GetProperty("bypassListenerEffects"),
				this.type.GetProperty("bypassReverbZones"),
				this.type.GetProperty("clip"),
				this.type.GetProperty("dopplerLevel"),
				this.type.GetProperty("ignoreListenerPause"),
				this.type.GetProperty("ignoreListenerVolume"),
				this.type.GetProperty("loop"),
				this.type.GetProperty("maxDistance"),
				this.type.GetProperty("minDistance"),
				this.type.GetProperty("mute"),
				this.type.GetProperty("outputAudioMixerGroup"),
#if UNITY_4_5 || UNITY_4_6 || UNITY_4_7
				this.type.GetProperty("pan"),
				this.type.GetProperty("panLevel"),
#else
				this.type.GetProperty("panStereo"),
				this.type.GetProperty("spatialBlend"),
#endif
				this.type.GetProperty("pitch"),
				this.type.GetProperty("playOnAwake"),
				this.type.GetProperty("priority"),
				this.type.GetProperty("reverbZoneMix"),
				this.type.GetProperty("rolloffMode"),
#if !UNITY_4_5 && !UNITY_4_6 && !UNITY_4_7 && !UNITY_5_0 && !UNITY_5_1
				this.type.GetProperty("spatialize"),
#endif
				this.type.GetProperty("spread"),
				this.type.GetProperty("time"),
				this.type.GetProperty("timeSamples"),
				this.type.GetProperty("velocityUpdateMode"),
				this.type.GetProperty("volume")
			};

			return fields;
		}
	}
}