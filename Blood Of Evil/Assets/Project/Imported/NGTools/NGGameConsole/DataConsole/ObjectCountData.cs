using System;
using System.Text;

namespace NGTools.NGGameConsole
{
	using UnityEngine;

	public class ObjectCountData : DataConsole
	{
		[Serializable]
		public class ObjectType
		{
			[Header("Depending on what numbers you are looking for.")]
			public Sources		source;
			public ObjectTypes	type;
		}

		public enum Sources
		{
			Object,
			Resources,
		}

		public enum ObjectTypes
		{
			Object,
			GameObject,
			Material,
			Mesh,
			Shader,
		}

		private static Type[]	types = { typeof(Object), typeof(GameObject), typeof(Material), typeof(Mesh), typeof(Shader) };

		public ObjectType[]	objectTypes;
		[Header("Interval in second.")]
		public float		refreshInterval = 5F;

		private float			nextRefresh = 0F;
		private int				objectCount = 0;

		public override void	FullGUI()
		{
			if (this.nextRefresh < Time.time)
			{
				this.nextRefresh = Time.time + this.refreshInterval;

				StringBuilder	buffer = Utility.sharedBuffer;

				buffer.Length = 0;

				for (int i = 0; i < this.objectTypes.Length; i++)
				{
					if (this.objectTypes[i].source == Sources.Object)
						this.objectCount = Object.FindObjectsOfType(ObjectCountData.types[(int)this.objectTypes[i].type]).Length;
					else if (this.objectTypes[i].source == Sources.Resources)
						this.objectCount = Resources.FindObjectsOfTypeAll(ObjectCountData.types[(int)this.objectTypes[i].type]).Length;
					buffer.AppendLine(ObjectCountData.types[(int)this.objectTypes[i].type].Name + " Count: " + this.objectCount);
				}

				if (buffer.Length >= Environment.NewLine.Length)
					 buffer.Length -= Environment.NewLine.Length;
				this.label.text = buffer.ToString();
			}

			GUILayout.TextArea(this.label.text, this.fullStyle);
		}

		public override string	Copy()
		{
			return this.label.text;
		}
	}
}