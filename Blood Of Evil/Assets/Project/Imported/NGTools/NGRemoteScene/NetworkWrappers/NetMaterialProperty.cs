using System;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	/// <summary>Describes information and value of a single shader property.</summary>
	public sealed class NetMaterialProperty
	{
		public readonly string	displayName;
		public readonly string	name;
		public readonly NGShader.ShaderPropertyType	type;
		public readonly bool	hidden;
		public readonly float	rangeMin;
		public readonly float	rangeMax;

		// Color value of the property.
		public Color	colorValue;
		// Float vaue of the property.
		public float	floatValue;
		//public MaterialProperty.TexDim textureDimension { get; }
		//public Vector4 textureScaleAndOffset { get; set; }
		// Texture value of the property.
		public UnityObject	textureValue;
		public Vector2	textureOffset;
		public Vector2	textureScale;
		// Vector value of the property.
		public Vector4	vectorValue;

		public static void	Serialize(Material mat, NGShaderProperty prop, ByteBuffer buffer)
		{
			buffer.AppendUnicodeString(prop.description);
			buffer.AppendUnicodeString(prop.name);
			buffer.Append((int)prop.type);
			buffer.Append(prop.hidden);
			buffer.Append(prop.rangeMin);
			buffer.Append(prop.rangeMax);

			Type		type = null;
			TypeHandler	typeHandler;

			if (prop.type == NGShader.ShaderPropertyType.Color)
				type = typeof(Color);
			else if (prop.type == NGShader.ShaderPropertyType.Float ||
					 prop.type == NGShader.ShaderPropertyType.Range)
				type = typeof(float);
			else if (prop.type == NGShader.ShaderPropertyType.TexEnv)
				type = typeof(Texture);
			else if (prop.type == NGShader.ShaderPropertyType.Vector)
				type = typeof(Vector4);

			typeHandler = TypeHandlersManager.GetTypeHandler(type);
			InternalNGDebug.Assert(typeHandler != null, "TypeHandler for " + prop.name + " is not supported.");

			if (prop.type == NGShader.ShaderPropertyType.Color)
				typeHandler.Serialize(buffer, type, mat.GetColor(prop.name));
			else if (prop.type == NGShader.ShaderPropertyType.Float ||
					 prop.type == NGShader.ShaderPropertyType.Range)
			{
				typeHandler.Serialize(buffer, type, mat.GetFloat(prop.name));
			}
			else if (prop.type == NGShader.ShaderPropertyType.TexEnv)
			{
				typeHandler.Serialize(buffer, type, mat.GetTexture(prop.name));

				TypeHandler	vector2Handler = TypeHandlersManager.GetTypeHandler<Vector2>();

				vector2Handler.Serialize(buffer, typeof(Vector2), mat.GetTextureOffset(prop.name));
				vector2Handler.Serialize(buffer, typeof(Vector2), mat.GetTextureScale(prop.name));
			}
			else if (prop.type == NGShader.ShaderPropertyType.Vector)
				typeHandler.Serialize(buffer, type, mat.GetVector(prop.name));

			//Debug.Log("NetMP.Ser " + prop.description + " " + prop.name + " " + prop.type + "	" + prop.hidden);
		}

		public static NetMaterialProperty	Deserialize(ByteBuffer buffer)
		{
			return new NetMaterialProperty(buffer);
		}

		private	NetMaterialProperty(ByteBuffer buffer)
		{
			this.displayName = buffer.ReadUnicodeString();
			this.name = buffer.ReadUnicodeString();
			this.type = (NGShader.ShaderPropertyType)buffer.ReadInt32();
			this.hidden = buffer.ReadBoolean();
			this.rangeMin = buffer.ReadSingle();
			this.rangeMax = buffer.ReadSingle();

			Type		type = null;
			TypeHandler	typeHandler;

			if (this.type == NGShader.ShaderPropertyType.Color)
				type = typeof(Color);
			else if (this.type == NGShader.ShaderPropertyType.Float ||
					 this.type == NGShader.ShaderPropertyType.Range)
			{
				type = typeof(float);
			}
			else if (this.type == NGShader.ShaderPropertyType.TexEnv)
				type = typeof(Texture);
			else if (this.type == NGShader.ShaderPropertyType.Vector)
				type = typeof(Vector4);

			typeHandler = TypeHandlersManager.GetTypeHandler(type);
			InternalNGDebug.Assert(typeHandler != null, "TypeHandler for " + this.name + " is not supported.");

			if (this.type == NGShader.ShaderPropertyType.Color)
				this.colorValue = (Color)typeHandler.Deserialize(buffer, type);
			else if (this.type == NGShader.ShaderPropertyType.Float ||
					 this.type == NGShader.ShaderPropertyType.Range)
			{
				this.floatValue = (float)typeHandler.Deserialize(buffer, type);
			}
			else if (this.type == NGShader.ShaderPropertyType.TexEnv)
			{
				this.textureValue = (UnityObject)typeHandler.Deserialize(buffer, type);

				TypeHandler	vector2Handler = TypeHandlersManager.GetTypeHandler<Vector2>();
				this.textureOffset = (Vector2)vector2Handler.Deserialize(buffer, typeof(Vector2));
				this.textureScale = (Vector2)vector2Handler.Deserialize(buffer, typeof(Vector2));
			}
			else if (this.type == NGShader.ShaderPropertyType.Vector)
				this.vectorValue = (Vector4)typeHandler.Deserialize(buffer, type);

			//Debug.Log("NGMP.Des " + this.displayName + " " + this.name + " " + this.type + "	" + this.hidden);
		}
	}
}