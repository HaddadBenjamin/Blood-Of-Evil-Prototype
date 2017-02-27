using System;
using System.Text;

namespace NGTools
{
	/// <summary>
	/// <para>Simple and straight resizable buffer. It easily appends anything to the buffer.</para>
	/// <para>You should reuse the same ByteBuffer as much as possible to be optimal.</para>
	/// </summary>
	public class ByteBuffer
	{
		public enum ResizeMode
		{
			Strict,
			Double,
		}

		public ResizeMode		resizeMode;
		/// <summary>Blocks Clear, Flush and Append methods.</summary>
		public readonly bool	writable;
		private int				length;
		public int				Capacity { get { return this.buffer.Length; } }
		public int				Length
		{
			get
			{
				return this.length;
			}
			set
			{
				if (this.writable == false)
					throw new InvalidOperationException("Buffer is unwritable.");
				this.length = value;
			}
		}
		public int				Position { get; set; }

		private byte[]	buffer;

		public	ByteBuffer(int capacity)
		{
			this.resizeMode = ResizeMode.Double;
			this.buffer = new byte[capacity];
			this.writable = true;
		}

		public	ByteBuffer(int capacity, ResizeMode mode)
		{
			this.resizeMode = mode;
			this.buffer = new byte[capacity];
			this.writable = true;
		}

		public	ByteBuffer(int capacity, bool writable)
		{
			this.resizeMode = ResizeMode.Double;
			this.buffer = new byte[capacity];
			this.writable = writable;
		}

		public	ByteBuffer(int capacity, ResizeMode mode, bool writable)
		{
			this.resizeMode = mode;
			this.buffer = new byte[capacity];
			this.writable = writable;
		}

		public	ByteBuffer(byte[] buffer)
		{
			this.buffer = (byte[])buffer.Clone();
			this.length = this.buffer.Length;
			this.writable = false;
		}

		public	ByteBuffer(byte[] buffer, bool writable)
		{
			this.buffer = (byte[])buffer.Clone();
			this.length = this.buffer.Length;
			this.writable = writable;
		}

		public void	Resize(int newSize)
		{
			this.Resize(newSize, false);
		}

		private void	Resize(int newSize, bool force)
		{
			if (this.writable == false && force == false)
				return;

			switch (this.resizeMode)
			{
				case ResizeMode.Strict:
					while (newSize > this.Length)
					{
						byte[]	newBuffer = new byte[newSize];
						if (this.Length > 0)
							Buffer.BlockCopy(this.buffer, 0, newBuffer, 0, this.Length);
						this.buffer = newBuffer;
					}
					break;
				case ResizeMode.Double:
					int	nextSize = this.buffer.Length << 1;

					while (newSize > nextSize)
						nextSize <<= 1;

					byte[]	newBuffer2 = new byte[nextSize];
					if (this.Length > 0)
						Buffer.BlockCopy(this.buffer, 0, newBuffer2, 0, this.Length);
					this.buffer = newBuffer2;
					break;
			}
		}

		public void	AppendUnicodeString(string content)
		{
			if (this.writable == false)
				return;

			if (string.IsNullOrEmpty(content) == true)
				this.Append(0);
			else
			{
				byte[]	unicode = Encoding.UTF8.GetBytes(content);

				this.Append(unicode.Length);
				this.Append(unicode);
			}
		}

		public void	Append(ByteBuffer src)
		{
			if (this.writable == false)
				return;
			if (this.Length + src.Length > this.buffer.Length)
				this.Resize(this.Length + src.Length);

			Buffer.BlockCopy(src.buffer, src.Position, this.buffer, this.Length, src.Length);
			this.Length += src.Length;
		}

		public void	Append(byte[] src, int position, int length)
		{
			if (this.writable == false)
				return;
			if (this.Length + length > this.buffer.Length)
				this.Resize(this.Length + length);

			Buffer.BlockCopy(src, position, this.buffer, this.Length, length);
			this.Length += length;
		}

		public void	Append(Array src)
		{
			if (this.writable == false)
				return;
			if (this.Length + src.Length > this.buffer.Length)
				this.Resize(this.Length + src.Length);

			Buffer.BlockCopy(src, 0, this.buffer, this.Length, src.Length);
			this.Length += src.Length;
		}

		public void	Append(Boolean value)
		{
			byte[]	bytes = BitConverter.GetBytes(value);
			this.Append(bytes);
		}

		public void	Append(Byte value)
		{
			if (this.writable == false)
				return;
			if (this.Length + sizeof(Byte) > this.buffer.Length)
				this.Resize(this.Length + sizeof(Byte));

			this.buffer[this.Length] = value;
			this.Length += sizeof(Byte);
		}

		public void	Append(SByte value)
		{
			byte[]	bytes = BitConverter.GetBytes(value);
			this.Append(bytes);
		}

		public void	Append(Char value)
		{
			byte[]	bytes = BitConverter.GetBytes(value);
			this.Append(bytes);
		}

		public void	Append(Single value)
		{
			byte[]	bytes = BitConverter.GetBytes(value);
			this.Append(bytes);
		}

		public void	Append(Double value)
		{
			byte[]	bytes = BitConverter.GetBytes(value);
			this.Append(bytes);
		}

		public void	Append(Int16 value)
		{
			byte[]	bytes = BitConverter.GetBytes(value);
			this.Append(bytes);
		}

		public void	Append(Int32 value)
		{
			byte[]	bytes = BitConverter.GetBytes(value);
			this.Append(bytes);
		}

		public void	Append(Int64 value)
		{
			byte[]	bytes = BitConverter.GetBytes(value);
			this.Append(bytes);
		}

		public void	Append(UInt16 value)
		{
			byte[]	bytes = BitConverter.GetBytes(value);
			this.Append(bytes);
		}

		public void	Append(UInt32 value)
		{
			byte[]	bytes = BitConverter.GetBytes(value);
			this.Append(bytes);
		}

		public void	Append(UInt64 value)
		{
			byte[]	bytes = BitConverter.GetBytes(value);
			this.Append(bytes);
		}

		public void	Append(String src)
		{
			byte[]	bytes = Encoding.UTF8.GetBytes(src);
			this.Append(bytes);
		}

		public Int16	ReadInt16()
		{
			if (this.Position + sizeof(Int16) > this.Length)
				throw new OverflowException("Unsufficient bytes (" + sizeof(Int16) + " bytes) in buffer of " + this.Length + " at " + this.Position + ".");
			var	v = BitConverter.ToInt16(this.buffer, this.Position);
			this.Position += sizeof(Int16);
			return v;
		}

		public Int32	ReadInt32()
		{
			if (this.Position + sizeof(Int32) > this.Length)
				throw new OverflowException("Unsufficient bytes (" + sizeof(Int32) + " bytes) in buffer of " + this.Length + " at " + this.Position + ".");
			var	v = BitConverter.ToInt32(this.buffer, this.Position);
			this.Position += sizeof(Int32);
			return v;
		}

		public Int64	ReadInt64()
		{
			if (this.Position + sizeof(Int64) > this.Length)
				throw new OverflowException("Unsufficient bytes (" + sizeof(Int64) + " bytes) in buffer of " + this.Length + " at " + this.Position + ".");
			var	v = BitConverter.ToInt64(this.buffer, this.Position);
			this.Position += sizeof(Int64);
			return v;
		}

		public UInt16	ReadUInt16()
		{
			if (this.Position + sizeof(UInt16) > this.Length)
				throw new OverflowException("Unsufficient bytes (" + sizeof(UInt16) + " bytes) in buffer of " + this.Length + " at " + this.Position + ".");
			var	v = BitConverter.ToUInt16(this.buffer, this.Position);
			this.Position += sizeof(UInt16);
			return v;
		}

		public UInt32	ReadUInt32()
		{
			if (this.Position + sizeof(UInt32) > this.Length)
				throw new OverflowException("Unsufficient bytes (" + sizeof(UInt32) + " bytes) in buffer of " + this.Length + " at " + this.Position + ".");
			var	v = BitConverter.ToUInt32(this.buffer, this.Position);
			this.Position += sizeof(UInt32);
			return v;
		}

		public UInt64	ReadUInt64()
		{
			if (this.Position + sizeof(UInt64) > this.Length)
				throw new OverflowException("Unsufficient bytes (" + sizeof(UInt64) + " bytes) in buffer of " + this.Length + " at " + this.Position + ".");
			var	v = BitConverter.ToUInt64(this.buffer, this.Position);
			this.Position += sizeof(UInt64);
			return v;
		}

		public Single	ReadSingle()
		{
			if (this.Position + sizeof(Single) > this.Length)
				throw new OverflowException("Unsufficient bytes (" + sizeof(Char) + " bytes) in buffer of " + this.Length + " at " + this.Position + ".");
			var	v = BitConverter.ToSingle(this.buffer, this.Position);
			this.Position += sizeof(Single);
			return v;
		}

		public Double	ReadDouble()
		{
			if (this.Position + sizeof(Double) > this.Length)
				throw new OverflowException("Unsufficient bytes (" + sizeof(Double) + " bytes) in buffer of " + this.Length + " at " + this.Position + ".");
			var	v = BitConverter.ToDouble(this.buffer, this.Position);
			this.Position += sizeof(Double);
			return v;
		}

		public Byte		ReadByte()
		{
			if (this.Position + sizeof(Byte) > this.Length)
				throw new OverflowException("Unsufficient bytes (" + sizeof(Byte) + " bytes) in buffer of " + this.Length + " at " + this.Position + ".");
			this.Position += sizeof(Byte);
			return this.buffer[this.Position - sizeof(Byte)];
		}

		public SByte	ReadSByte()
		{
			if (this.Position + sizeof(SByte) > this.Length)
				throw new OverflowException("Unsufficient bytes (" + sizeof(SByte) + " bytes) in buffer of " + this.Length + " at " + this.Position + ".");
			this.Position += sizeof(SByte);
			return (sbyte)this.buffer[this.Position - sizeof(SByte)];
		}

		public Boolean	ReadBoolean()
		{
			if (this.Position + sizeof(Boolean) > this.Length)
				throw new OverflowException("Unsufficient bytes (" + sizeof(Boolean) + " bytes) in buffer of " + this.Length + " at " + this.Position + ".");
			var	v = BitConverter.ToBoolean(this.buffer, this.Position);
			this.Position += sizeof(Boolean);
			return v;
		}

		public Char		ReadChar()
		{
			if (this.Position + sizeof(Char) > this.Length)
				throw new OverflowException("Unsufficient bytes (" + sizeof(Char) + " bytes) in buffer of " + this.Length + " at " + this.Position + ".");
			var	v = BitConverter.ToChar(this.buffer, this.Position);
			this.Position += sizeof(Char);
			return v;
		}

		public String	ReadString(int length)
		{
			if (this.Position + length > this.Length)
				throw new OverflowException("Unsufficient bytes (" + length + " bytes) in buffer of " + this.Length + " at " + this.Position + ".");
			var	v = Encoding.UTF8.GetString(this.buffer, this.Position, length);
			this.Position += length;
			return v;
		}

		public String	ReadUnicodeString()
		{
			int	length = this.ReadInt32();

			if (length > 0)
				return Encoding.UTF8.GetString(this.ReadBytes(length));
			return string.Empty;
		}

		public Byte[]	ReadBytes(int length)
		{
			if (this.Position + length > this.Length)
				throw new OverflowException("Unsufficient bytes (" + length + " bytes) in buffer of " + this.Length + " at " + this.Position + ".");
			byte[]	v = new byte[length];
			Buffer.BlockCopy(this.buffer, this.Position, v, 0, length);
			this.Position += length;
			return v;
		}

		public void	Clear()
		{
			this.Length = 0;
			this.Position = 0;
		}

		/// <summary>
		/// Returns a copy of the buffer and clears it.
		/// </summary>
		/// <returns></returns>
		public byte[]	Flush()
		{
			byte[]	copy = this.GetBuffer();
			this.Clear();
			return copy;
		}

		public byte[]	GetRawBuffer()
		{
			return this.buffer;
		}

		public byte[]	GetBuffer()
		{
			byte[]	copy = new byte[this.Length];
			Buffer.BlockCopy(this.buffer, 0, copy, 0, this.Length);
			return copy;
		}

		/// <summary>
		/// Writes a <paramref name="destination"/> into this buffer. Even when unwritable.
		/// </summary>
		/// <param name="destination"></param>
		/// <param name="length"></param>
		public void		CopyBuffer(ByteBuffer destination, int length)
		{
			//if (destination.writable == false)
			//	throw new OverflowException("Destination buffer too small and is not writable.");
			if (this.Position + length > this.Length)
				throw new OverflowException("Unsufficient bytes in buffer of " + this.Length + " at " + this.Position + ".");
			if (destination.buffer.Length < length)
				destination.Resize(length, true);

			Buffer.BlockCopy(this.buffer, this.Position, destination.buffer, 0, length);
			destination.length = length;
			destination.Position = 0;
		}

		/// <summary>
		/// Writes a <paramref name="destination"/> into this buffer. Even when unwritable.
		/// </summary>
		/// <param name="destination"></param>
		/// <param name="length"></param>
		public void		CopyBuffer(ByteBuffer destination, int position, int length)
		{
			//if (destination.writable == false)
			//	throw new OverflowException("Destination buffer too small and is not writable.");
			if (position + length > this.Length)
				throw new OverflowException("Unsufficient bytes in buffer of " + this.Length + " at " + this.Position + ".");
			if (destination.buffer.Length < length)
				destination.Resize(length, true);

			Buffer.BlockCopy(this.buffer, position, destination.buffer, 0, length);
			destination.length = length;
			destination.Position = 0;
		}
	}
}