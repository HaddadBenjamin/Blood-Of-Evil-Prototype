using UnityEngine;

namespace NGTools.NGRemoteScene
{
	public sealed class NetMaterial
	{
		public readonly int						instanceID;
		public readonly string					name;
		public string							shader;
		public readonly NetMaterialProperty[]	properties;

		public int	selectedShader = -1;

		public static void	Serialize(Material mat, NGShader shader, ByteBuffer buffer)
		{
			buffer.Append(mat.GetInstanceID());
			buffer.AppendUnicodeString(mat.name);

			if (mat.shader != null)
				buffer.AppendUnicodeString(mat.shader.name);
			else
				buffer.Append(0);

			buffer.Append(shader.properties.Length);

			for (int i = 0; i < shader.properties.Length; i++)
				NetMaterialProperty.Serialize(mat, shader.properties[i], buffer);
		}

		public static NetMaterial	Deserialize(ByteBuffer buffer)
		{
			return new NetMaterial(buffer);
		}

		private	NetMaterial(ByteBuffer buffer)
		{
			this.instanceID = buffer.ReadInt32();
			this.name = buffer.ReadUnicodeString();
			this.shader = buffer.ReadUnicodeString();
			this.properties = new NetMaterialProperty[buffer.ReadInt32()];

			for (int i = 0; i < this.properties.Length; i++)
				this.properties[i] = NetMaterialProperty.Deserialize(buffer);
		}
	}
}