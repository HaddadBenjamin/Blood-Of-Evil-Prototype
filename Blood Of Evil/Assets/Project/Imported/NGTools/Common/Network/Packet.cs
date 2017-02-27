using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace NGTools.Network
{
	public abstract partial class Packet
	{
		private static Dictionary<Type, int>			cachedPacketId = new Dictionary<Type, int>();
		private static Dictionary<Type, FieldInfo[]>	cachedPacketFields = new Dictionary<Type, FieldInfo[]>();

		public readonly int		packetId;
		public readonly bool	isBatchable;

		protected	Packet()
		{
			if (Packet.cachedPacketId.TryGetValue(this.GetType(), out this.packetId) == false)
			{
				PacketLinkToAttribute[]	attributes = this.GetType().GetCustomAttributes(typeof(PacketLinkToAttribute), true) as PacketLinkToAttribute[];

				if (attributes.Length != 1)
					throw new MissingComponentException("Missing attribute PacketLinkToAttribute on " + this.ToString());

				this.packetId = attributes[0].packetId;
				this.isBatchable = attributes[0].isBatchable;
				Packet.cachedPacketId.Add(this.GetType(), this.packetId);
			}
		}

		protected	Packet(ByteBuffer buffer) : this()
		{
			try
			{
				this.In(buffer);
			}
			catch (Exception ex)
			{
				InternalNGDebug.LogFileException(this.GetType().ToString(), ex);
				throw;
			}
		}

		public virtual void	Out(ByteBuffer buffer)
		{
			FieldInfo[]	fis = this.GetFields(this.GetType());

			for (int i = 0; i < fis.Length; i++)
			{
				if (fis[i].FieldType == typeof(String))
					buffer.AppendUnicodeString((String)fis[i].GetValue(this));
				else if (fis[i].FieldType.IsEnum == true)
					buffer.Append((Int32)fis[i].GetValue(this));
				else if (fis[i].FieldType == typeof(Boolean))
					buffer.Append((Boolean)fis[i].GetValue(this));
				else if (fis[i].FieldType == typeof(Byte))
					buffer.Append((Byte)fis[i].GetValue(this));
				else if (fis[i].FieldType == typeof(SByte))
					buffer.Append((SByte)fis[i].GetValue(this));
				else if (fis[i].FieldType == typeof(Char))
					buffer.Append((Char)fis[i].GetValue(this));
				else if (fis[i].FieldType == typeof(Single))
					buffer.Append((Single)fis[i].GetValue(this));
				else if (fis[i].FieldType == typeof(Double))
					buffer.Append((Double)fis[i].GetValue(this));
				else if (fis[i].FieldType == typeof(Int16))
					buffer.Append((Int16)fis[i].GetValue(this));
				else if (fis[i].FieldType == typeof(Int32))
					buffer.Append((Int32)fis[i].GetValue(this));
				else if (fis[i].FieldType == typeof(Int64))
					buffer.Append((Int64)fis[i].GetValue(this));
				else if (fis[i].FieldType == typeof(UInt16))
					buffer.Append((UInt16)fis[i].GetValue(this));
				else if (fis[i].FieldType == typeof(UInt32))
					buffer.Append((UInt32)fis[i].GetValue(this));
				else if (fis[i].FieldType == typeof(UInt64))
					buffer.Append((UInt64)fis[i].GetValue(this));
				else if (fis[i].FieldType == typeof(Vector2))
				{
					Vector2	v = (Vector2)fis[i].GetValue(this);
					buffer.Append(v.x);
					buffer.Append(v.y);
				}
				else if (fis[i].FieldType == typeof(Vector3))
				{
					Vector3	v = (Vector3)fis[i].GetValue(this);
					buffer.Append(v.x);
					buffer.Append(v.y);
					buffer.Append(v.z);
				}
				else if (fis[i].FieldType == typeof(Vector4))
				{
					Vector4	v = (Vector4)fis[i].GetValue(this);
					buffer.Append(v.x);
					buffer.Append(v.y);
					buffer.Append(v.z);
					buffer.Append(v.w);
				}
				else if (fis[i].FieldType == typeof(Rect))
				{
					Rect	r = (Rect)fis[i].GetValue(this);
					buffer.Append(r.x);
					buffer.Append(r.y);
					buffer.Append(r.width);
					buffer.Append(r.height);
				}
				else if (fis[i].FieldType == typeof(Quaternion))
				{
					Quaternion	q = (Quaternion)fis[i].GetValue(this);
					buffer.Append(q.x);
					buffer.Append(q.y);
					buffer.Append(q.z);
					buffer.Append(q.w);
				}
				else if (fis[i].FieldType == typeof(Type))
					buffer.AppendUnicodeString(((Type)fis[i].GetValue(this)).GetShortAssemblyType());
				else if (fis[i].FieldType == typeof(Byte[]))
				{
					Byte[]	a = (Byte[])fis[i].GetValue(this);
					buffer.Append(a.Length);
					buffer.Append(a);
				}
				else if (fis[i].FieldType.IsUnityArray() == true)
				{
					object	rawArray = fis[i].GetValue(this);

					ICollectionModifier	collectionModifier = NGTools.Utility.GetCollectionModifier(rawArray);

					buffer.Append(collectionModifier.Size);

					this.AppendArrayToBuffer(collectionModifier, Utility.GetArraySubType(fis[i].FieldType), buffer);
				}
				else
					throw new NotSupportedException("Type \"" + fis[i].FieldType + "\" is not supported.");
			}
		}

		public virtual void	In(ByteBuffer buffer)
		{
			FieldInfo[]	fis = this.GetFields(this.GetType());

			for (int i = 0; i < fis.Length; i++)
			{
				if (fis[i].FieldType == typeof(Int32))
					fis[i].SetValue(this, buffer.ReadInt32());
				else if (fis[i].FieldType == typeof(String))
					fis[i].SetValue(this, buffer.ReadUnicodeString());
				else if (fis[i].FieldType == typeof(Single))
					fis[i].SetValue(this, buffer.ReadSingle());
				else if (fis[i].FieldType.IsEnum == true)
					fis[i].SetValue(this, buffer.ReadInt32());
				else if (fis[i].FieldType == typeof(Boolean))
					fis[i].SetValue(this, buffer.ReadBoolean());
				else if (fis[i].FieldType == typeof(Byte))
					fis[i].SetValue(this, buffer.ReadByte());
				else if (fis[i].FieldType == typeof(SByte))
					fis[i].SetValue(this, buffer.ReadSByte());
				else if (fis[i].FieldType == typeof(Char))
					fis[i].SetValue(this, buffer.ReadChar());
				else if (fis[i].FieldType == typeof(Double))
					fis[i].SetValue(this, buffer.ReadDouble());
				else if (fis[i].FieldType == typeof(Int16))
					fis[i].SetValue(this, buffer.ReadInt16());
				else if (fis[i].FieldType == typeof(Int64))
					fis[i].SetValue(this, buffer.ReadInt64());
				else if (fis[i].FieldType == typeof(UInt16))
					fis[i].SetValue(this, buffer.ReadUInt16());
				else if (fis[i].FieldType == typeof(UInt32))
					fis[i].SetValue(this, buffer.ReadUInt32());
				else if (fis[i].FieldType == typeof(UInt64))
					fis[i].SetValue(this, buffer.ReadUInt64());
				else if (fis[i].FieldType == typeof(Vector2))
					fis[i].SetValue(this, new Vector2(buffer.ReadSingle(), buffer.ReadSingle()));
				else if (fis[i].FieldType == typeof(Vector3))
					fis[i].SetValue(this, new Vector3(buffer.ReadSingle(), buffer.ReadSingle(), buffer.ReadSingle()));
				else if (fis[i].FieldType == typeof(Vector4))
					fis[i].SetValue(this, new Vector4(buffer.ReadSingle(), buffer.ReadSingle(), buffer.ReadSingle(), buffer.ReadSingle()));
				else if (fis[i].FieldType == typeof(Rect))
					fis[i].SetValue(this, new Rect(buffer.ReadSingle(), buffer.ReadSingle(), buffer.ReadSingle(), buffer.ReadSingle()));
				else if (fis[i].FieldType == typeof(Quaternion))
					fis[i].SetValue(this, new Quaternion(buffer.ReadSingle(), buffer.ReadSingle(), buffer.ReadSingle(), buffer.ReadSingle()));
				else if (fis[i].FieldType == typeof(Type))
					fis[i].SetValue(this, Type.GetType(buffer.ReadUnicodeString()));
				else if (fis[i].FieldType == typeof(Byte[]))
					fis[i].SetValue(this, buffer.ReadBytes(buffer.ReadInt32()));
				else if (fis[i].FieldType.IsUnityArray() == true)
				{
					object	array;

					if (fis[i].FieldType.IsArray == true)
						array = Array.CreateInstance(fis[i].FieldType.GetElementType(), buffer.ReadInt32());
					else
					{
						int		count = buffer.ReadInt32();
						array = Activator.CreateInstance(fis[i].FieldType, count);
						IList	list = (IList)array;
						object	defaultValue;

						if (Utility.GetArraySubType(fis[i].FieldType).IsValueType == true)
							defaultValue = Activator.CreateInstance(Utility.GetArraySubType(fis[i].FieldType));
						else
							defaultValue = null;

						for (int j = 0; j < count; j++)
							list.Add(defaultValue);
					}

					ICollectionModifier	collectionModifier = NGTools.Utility.GetCollectionModifier(array);

					this.ReadArrayFromBuffer(collectionModifier, Utility.GetArraySubType(fis[i].FieldType), buffer);

					fis[i].SetValue(this, array);
				}
				else
					throw new NotSupportedException("Type \"" + fis[i].FieldType + "\" is not supported.");
			}
		}

		/// <summary>
		/// Must draw a description of the packet in a single line of 16F.
		/// </summary>
		/// <param name="unityData"></param>
		public virtual void	OnGUI(IUnityData unityData)
		{
			GUILayout.Label(this.GetType().Name);
		}

		/// <summary>Checks if an existing Packet can be aggregated, therefore merging data into it and then be discarded.</summary>
		/// <param name="x">A pending Packet.</param>
		/// <returns>True when aggregated, otherwise false.</returns>
		public virtual bool	AggregateInto(Packet x)
		{
			return false;
		}

		private void	AppendArrayToBuffer(ICollectionModifier array, Type type, ByteBuffer buffer)
		{
			if (type == typeof(String))
			{
				for (int i = 0; i < array.Size; i++)
					buffer.AppendUnicodeString((string)array.Get(i));
			}
			else if (type.IsEnum == true)
			{
				for (int i = 0; i < array.Size; i++)
					buffer.Append((Int32)array.Get(i));
			}
			else if (type == typeof(Boolean))
			{
				for (int i = 0; i < array.Size; i++)
					buffer.Append((Boolean)array.Get(i));
			}
			else if (type == typeof(Byte))
			{
				for (int i = 0; i < array.Size; i++)
					buffer.Append((Byte)array.Get(i));
			}
			else if (type == typeof(SByte))
			{
				for (int i = 0; i < array.Size; i++)
					buffer.Append((SByte)array.Get(i));
			}
			else if (type == typeof(Char))
			{
				for (int i = 0; i < array.Size; i++)
					buffer.Append((Char)array.Get(i));
			}
			else if (type == typeof(Single))
			{
				for (int i = 0; i < array.Size; i++)
					buffer.Append((Single)array.Get(i));
			}
			else if (type == typeof(Double))
			{
				for (int i = 0; i < array.Size; i++)
					buffer.Append((Double)array.Get(i));
			}
			else if (type == typeof(Int16))
			{
				for (int i = 0; i < array.Size; i++)
					buffer.Append((Int16)array.Get(i));
			}
			else if (type == typeof(Int32))
			{
				for (int i = 0; i < array.Size; i++)
					buffer.Append((Int32)array.Get(i));
			}
			else if (type == typeof(Int64))
			{
				for (int i = 0; i < array.Size; i++)
					buffer.Append((Int64)array.Get(i));
			}
			else if (type == typeof(UInt16))
			{
				for (int i = 0; i < array.Size; i++)
					buffer.Append((UInt16)array.Get(i));
			}
			else if (type == typeof(UInt32))
			{
				for (int i = 0; i < array.Size; i++)
					buffer.Append((UInt32)array.Get(i));
			}
			else if (type == typeof(UInt64))
			{
				for (int i = 0; i < array.Size; i++)
					buffer.Append((UInt64)array.Get(i));
			}
		}

		private void	ReadArrayFromBuffer(ICollectionModifier array, Type type, ByteBuffer buffer)
		{
			if (type == typeof(String))
			{
				for (int i = 0; i < array.Size; i++)
					array.Set(i, buffer.ReadUnicodeString());
			}
			else if (type.IsEnum == true)
			{
				for (int i = 0; i < array.Size; i++)
					array.Set(i, buffer.ReadInt32());
			}
			else if (type == typeof(Boolean))
			{
				for (int i = 0; i < array.Size; i++)
					array.Set(i, buffer.ReadBoolean());
			}
			else if (type == typeof(Byte))
			{
				for (int i = 0; i < array.Size; i++)
					array.Set(i, buffer.ReadByte());
			}
			else if (type == typeof(SByte))
			{
				for (int i = 0; i < array.Size; i++)
					array.Set(i, buffer.ReadSByte());
			}
			else if (type == typeof(Char))
			{
				for (int i = 0; i < array.Size; i++)
					array.Set(i, buffer.ReadChar());
			}
			else if (type == typeof(Single))
			{
				for (int i = 0; i < array.Size; i++)
					array.Set(i, buffer.ReadSingle());
			}
			else if (type == typeof(Double))
			{
				for (int i = 0; i < array.Size; i++)
					array.Set(i, buffer.ReadDouble());
			}
			else if (type == typeof(Int16))
			{
				for (int i = 0; i < array.Size; i++)
					array.Set(i, buffer.ReadInt16());
			}
			else if (type == typeof(Int32))
			{
				for (int i = 0; i < array.Size; i++)
					array.Set(i, buffer.ReadInt32());
			}
			else if (type == typeof(Int64))
			{
				for (int i = 0; i < array.Size; i++)
					array.Set(i, buffer.ReadInt64());
			}
			else if (type == typeof(UInt16))
			{
				for (int i = 0; i < array.Size; i++)
					array.Set(i, buffer.ReadUInt16());
			}
			else if (type == typeof(UInt32))
			{
				for (int i = 0; i < array.Size; i++)
					array.Set(i, buffer.ReadUInt32());
			}
			else if (type == typeof(UInt64))
			{
				for (int i = 0; i < array.Size; i++)
					array.Set(i, buffer.ReadUInt64());
			}
		}

		private FieldInfo[]	GetFields(Type type)
		{
			FieldInfo[]	fis;

			if (Packet.cachedPacketFields.TryGetValue(type, out fis) == false)
			{
				List<FieldInfo>	fields = new List<FieldInfo>(type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance));

				for (int i = 0; i < fields.Count; i++)
				{
					if (fields[i].IsDefined(typeof(StripFromNetworkAttribute), false) == true)
					{
						fields.RemoveAt(i);
						--i;
					}
				}

				fis = fields.ToArray();
				Packet.cachedPacketFields.Add(type, fis);
			}

			return fis;
		}
	}
}