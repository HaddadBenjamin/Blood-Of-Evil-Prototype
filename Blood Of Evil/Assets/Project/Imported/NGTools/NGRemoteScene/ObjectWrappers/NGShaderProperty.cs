namespace NGTools.NGRemoteScene
{
	/// <summary>Describes information and value of a single shader property.</summary>
	public sealed class NGShaderProperty
	{
		public readonly string						description;
		public readonly string						name;
		public readonly NGShader.ShaderPropertyType	type;
		public readonly bool						hidden;
		public readonly float						rangeMin;
		public readonly float						rangeMax;

		public	NGShaderProperty(ByteBuffer buffer)
		{
			this.description = buffer.ReadUnicodeString();
			this.name = buffer.ReadUnicodeString();
			this.type = (NGShader.ShaderPropertyType)buffer.ReadInt32();
			this.hidden = buffer.ReadBoolean();
			this.rangeMin = buffer.ReadSingle();
			this.rangeMax = buffer.ReadSingle();

			//Debug.Log(this.description + " " + this.name + " " + this.type + "	" + this.hidden);
		}
	}
}