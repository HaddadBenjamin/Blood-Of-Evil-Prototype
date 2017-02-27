using NGTools;
using NGTools.NGRemoteScene;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGRemoteScene
{
	/// <summary>
	/// </summary>
	/// <remarks>
	/// File format:
	/// 
	/// Designation:
	/// [] = Array
	/// * = Zero or more
	/// + = One or more
	/// 
	/// Length	Type	Comment
	/// 4	Char[]	Magic number
	/// 2	UInt16	Version
	/// 
	/// 2	UInt16	Parameter count
	/// [N	String	Parameter name
	///  N	String	Parameter value]+
	/// 
	/// [1	byte	Module ID
	///  4	Int32	Module length
	///  N	Byte[]	Data]+
	/// </remarks>
	public sealed class Replay
	{
		public static readonly char[]	MagicNumber = new char[] { 'a', 'b', 'c', 'd' };
		public static readonly UInt16	Version = 1;
		public static readonly string	LastOpenReplayKey = "LastOpenReplay";

		public bool	canSave = false;

		public string	name;

		public int		width;
		public int		height;
		public float	maxTime;

		public bool		playing = false;
		public float	cursorTime = 0F;
		public float	speed = 1F;
		public bool		repeat = false;

		public List<ReplayDataModule>	modules = new List<ReplayDataModule>();

		public float	realTimeOffset { get; private set; }

		private bool	endReached;

		private double	lastFrame;

		public	Replay(string name)
		{
			this.name = name;

			// Initialize modules.
			this.modules = new List<ReplayDataModule>();

			foreach (Type type in Utility.EachSubClassesOf(typeof(ReplayDataModule)))
				this.modules.Add(Activator.CreateInstance(type) as ReplayDataModule);

			this.modules.Sort((a, b) => b.priority - a.priority);
		}

		public	Replay(IReplaySettings settings)
		{
			this.name = DateTime.Now.ToLongTimeString();
			this.canSave = true;
			this.width = settings.TextureWidth;
			this.height = settings.TextureHeight;

			// Initialize modules.
			this.modules = new List<ReplayDataModule>();

			for (int i = 0; i < settings.Modules.Count; i++)
			{
				ReplayDataModule	module = settings.Modules[i].ConvertToReplay(settings);

				if (module != null && module.data.Count > 0)
					this.modules.Add(module);
			}

			this.Init();
		}

		public void	Play()
		{
			this.playing = true;
			this.lastFrame = EditorApplication.timeSinceStartup;

			if (this.cursorTime >= this.maxTime)
				this.Set(0F);
		}

		public void	Pause()
		{
			this.playing = false;
		}

		public void	Stop()
		{
			this.playing = false;
			this.Set(0F);
		}

		public void	Set(float t, bool force = false)
		{
			if (force == false)
			{
				if (t >= this.maxTime)
				{
					// Last frame reached, now we can loop.
					if (this.endReached == true)
					{
						this.endReached = false;
						t = 0F;
					}
					else if (this.repeat == true)
					{
						// Do not skip the last frame.
						this.endReached = true;
						t = this.maxTime;
					}
					else
					{
						t = this.maxTime;
						this.playing = false;
					}
				}
				else if (t < 0)
				{
					// Last frame reached, now we can loop.
					if (this.endReached == true)
					{
						this.endReached = false;
						t = this.maxTime;
					}
					else if (this.repeat == true)
					{
						// Do not skip the last frame.
						this.endReached = true;
						t = 0F;
					}
					else
					{
						t = 0F;
						this.playing = false;
					}
				}
			}
			else
				t = Mathf.Clamp(t, 0F, this.maxTime);

			t += this.realTimeOffset;

			for (int i = 0; i < this.modules.Count; i++)
				this.modules[i].SetTime(t);

			t -= this.realTimeOffset;

			this.cursorTime = t;
		}

		public void	Update()
		{
			if (this.playing == true)
			{
				double	time = EditorApplication.timeSinceStartup;
				this.Set(this.cursorTime + (float)(time - this.lastFrame) * this.speed);
				this.lastFrame = time;
			}
		}

		public bool	Load(string filepath)
		{
			try
			{
				using (StreamReader reader = new StreamReader(filepath))
				{
					BinaryReader	br = new BinaryReader(reader.BaseStream);

					char[]	magicNumber = br.ReadChars(4);

					if (magicNumber[0] != Replay.MagicNumber[0] ||
						magicNumber[1] != Replay.MagicNumber[1] ||
						magicNumber[2] != Replay.MagicNumber[2] ||
						magicNumber[3] != Replay.MagicNumber[3])
					{
						return false;
					}

					ushort	version = br.ReadUInt16();

					// Create multi version manager
					if (version != 1)
						return false;

					UInt16	parameterCount = br.ReadUInt16();

					for (int i = 0; i < parameterCount; i++)
					{
						string	parameterName = br.ReadString();
						string	parameterValue = br.ReadString();

						if (parameterName == "width")
							this.width = int.Parse(parameterValue);
						else if (parameterName == "height")
							this.height = int.Parse(parameterValue);
					}

					ByteBuffer	buffer = Utility.GetBBuffer();

					while (br.PeekChar() != -1)
					{
						byte	moduleID  = br.ReadByte();
						Int32	length = br.ReadInt32();
						int		i = 0;

						for (; i < this.modules.Count; i++)
						{
							if (this.modules[i].moduleID == moduleID)
							{
								if (buffer.Capacity < length)
									buffer.Resize(length);

								buffer.Length = br.Read(buffer.GetRawBuffer(), 0, length);

								this.modules[i].Import(this, buffer);
								buffer.Clear();
								break;
							}
						}

						if (i >= this.modules.Count)
							br.BaseStream.Position += length;
					}

					// Remove unused module.
					for (int i = 0; i < this.modules.Count; i++)
					{
						if (this.modules[i].data.Count == 0)
							this.modules.RemoveAt(i--);
					}

					Utility.RestoreBBuffer(buffer);
				}

				this.Init();

				return true;
			}
			catch (OverflowException ex)
			{
				InternalNGDebug.LogException("Replay at \"" + filepath + "\" is corrupted.", ex);
			}
			catch (Exception ex)
			{
				InternalNGDebug.LogException(ex);
			}

			return false;
		}

		public bool	Save(string filepath)
		{
			try
			{
				using (StreamWriter swriter = new StreamWriter(filepath))
				{
					BinaryWriter	bwriter = new BinaryWriter(swriter.BaseStream);

					// Header
					bwriter.Write(Replay.MagicNumber);
					bwriter.Write(Replay.Version);

					// Number of parameters
					bwriter.Write((UInt16)2);

					// Parameters
					bwriter.Write("width");
					bwriter.Write(this.width.ToString());
					bwriter.Write("height");
					bwriter.Write(this.height.ToString());

					ByteBuffer	buffer = Utility.GetBBuffer();

					for (int i = 0; i < this.modules.Count; i++)
					{
						bwriter.Write(this.modules[i].moduleID);

						this.modules[i].Export(buffer);

						byte[]	data = buffer.GetBuffer();

						bwriter.Write(data.Length);
						bwriter.Write(data);

						buffer.Clear();
					}

					Utility.RestoreBBuffer(buffer);
				}

				return true;
			}
			catch (Exception ex)
			{
				InternalNGDebug.LogException(ex);
			}

			return false;
		}

		private void	Init()
		{
			for (int i = 0; i < this.modules.Count; i++)
			{
				if (this.maxTime < this.modules[i].data[this.modules[i].data.Count - 1].time)
					this.maxTime = this.modules[i].data[this.modules[i].data.Count - 1].time;

				if (this.modules[i].moduleID == ScreenshotModule.ModuleID)
					this.realTimeOffset = this.modules[i].data[0].time;
			}

			this.maxTime -= this.realTimeOffset;

			this.Set(0F);
		}
	}
}