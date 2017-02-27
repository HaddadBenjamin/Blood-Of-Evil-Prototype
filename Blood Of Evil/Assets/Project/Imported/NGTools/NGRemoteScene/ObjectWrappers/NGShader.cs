namespace NGTools.NGRemoteScene
{
	public sealed class NGShader
	{
		public enum ShaderPropertyType
		{
			Color = 0,
			Vector = 1,
			Float = 2,
			Range = 3,
			TexEnv = 4,
		}

		public readonly string				name;
		public readonly NGShaderProperty[]	properties;

		public	NGShader(ByteBuffer buffer)
		{
			this.name = buffer.ReadUnicodeString();
			this.properties = new NGShaderProperty[buffer.ReadInt32()];

			for (int i = 0; i < this.properties.Length; i++)
				this.properties[i] = new NGShaderProperty(buffer);
		}

		public NGShaderProperty	GetProperty(string name)
		{
			for (int i = 0; i < this.properties.Length; i++)
			{
				if (this.properties[i].name == name)
					return this.properties[i];
			}

			return null;
		}
	}
}