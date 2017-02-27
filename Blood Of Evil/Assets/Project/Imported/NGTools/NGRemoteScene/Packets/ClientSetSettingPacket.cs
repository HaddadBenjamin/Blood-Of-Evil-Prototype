using NGTools.Network;
using System;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	internal enum Setting
	{
		TargetRefresh,
		Wireframe,

		CameraClearFlags,
		CameraBackground,
		CameraCullingMask,
		CameraProjection,
		CameraFieldOfView,
		CameraSize,
		CameraClippingPlanesFar,
		CameraClippingPlanesNear,
		CameraViewportRect,
		CameraDepth,
		CameraRenderingPath,
		CameraOcclusionCulling,
		CameraHDR,
#if UNITY_5
		CameraTargetDisplay,
#endif
	}

	internal enum SettingType
	{
		Integer,
		Single,
		Boolean,
		String,
		Color,
		Rect,
		Vector2
	}

	[PacketLinkTo(PacketId.Camera_ClientSetSetting)]
	internal sealed class ClientSetSettingPacket : Packet
	{

		public Setting		setting;
		public SettingType	type;
		public object		value;

		private	ClientSetSettingPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ClientSetSettingPacket(Setting setting, SettingType type, object value)
		{
			this.setting = setting;
			this.type = type;
			this.value = value;
		}

		public override void	In(ByteBuffer buffer)
		{
			this.setting = (Setting)buffer.ReadInt32();
			this.type = (SettingType)buffer.ReadInt32();
			
			if (this.type == SettingType.Integer)
				this.value = buffer.ReadInt32();
			if (this.type == SettingType.Single)
				this.value = buffer.ReadSingle();
			else if (this.type == SettingType.Boolean)
				this.value = buffer.ReadBoolean();
			else if (this.type == SettingType.String)
				this.value = buffer.ReadUnicodeString();
			else if (this.type == SettingType.Color)
				this.value = new Color(buffer.ReadSingle(), buffer.ReadSingle(), buffer.ReadSingle(), buffer.ReadSingle());
			else if (this.type == SettingType.Rect)
				this.value = new Rect(buffer.ReadSingle(), buffer.ReadSingle(), buffer.ReadSingle(), buffer.ReadSingle());
			else if (this.type == SettingType.Vector2)
				this.value = new Vector2(buffer.ReadSingle(), buffer.ReadSingle());
		}

		public override void	Out(ByteBuffer buffer)
		{
			buffer.Append((Int32)this.setting);
			buffer.Append((Int32)this.type);

			if (this.type == SettingType.Integer)
				buffer.Append((Int32)this.value);
			else if (this.type == SettingType.Single)
				buffer.Append((Single)this.value);
			else if (this.type == SettingType.Boolean)
				buffer.Append((Boolean)this.value);
			else if (this.type == SettingType.String)
				buffer.AppendUnicodeString((String)this.value);
			else if (this.type == SettingType.Color)
			{
				Color	c = (Color)this.value;
				buffer.Append(c.r);
				buffer.Append(c.g);
				buffer.Append(c.b);
				buffer.Append(c.a);
			}
			else if (this.type == SettingType.Rect)
			{
				Rect	r = (Rect)this.value;
				buffer.Append(r.x);
				buffer.Append(r.y);
				buffer.Append(r.width);
				buffer.Append(r.height);
			}
			else if (this.type == SettingType.Vector2)
			{
				Vector2	r = (Vector2)this.value;
				buffer.Append(r.x);
				buffer.Append(r.y);
			}
		}

		public override bool	AggregateInto(Packet pendingPacket)
		{
			ClientSetSettingPacket	packet = pendingPacket as ClientSetSettingPacket;

			if (packet != null && packet.setting == this.setting && packet.value != this.value)
			{
				packet.value = this.value;
				return true;
			}

			return false;
		}

		public override void	OnGUI(IUnityData unityData)
		{
			GUILayout.Label("Updating setting " + this.setting + " (" + this.type + " " + this.value + ").");
		}
	}
}