using NGTools.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace NGToolsEditor.NGRemoteScene
{
	public sealed class DataDrawer
	{
		public sealed class LayerPath : IDisposable
		{
			private DataDrawer	data;

			public	LayerPath(DataDrawer data)
			{
				this.data = data;
				this.data.PushPath();
			}

			public void	Dispose()
			{
				this.data.PopPath();
			}
		}

		private static StringBuilder	valuePath = new StringBuilder(64);

		public string	name { get; private set; }
		public object	value { get; private set; }
		public NGRemoteInspectorWindow	inspector { get; private set; }

		public readonly IUnityData	unityData;

		public	DataDrawer(IUnityData unityData)
		{
			this.unityData = unityData;
		}

		public void	Init(string path, NGRemoteInspectorWindow inspector)
		{
			this.inspector = inspector;
			DataDrawer.valuePath.Length = 0;
			DataDrawer.valuePath.Append(path);
		}

		public DataDrawer	DrawChild(string name, object value)
		{
			if (this.layerPaths.Count > 0)
				DataDrawer.valuePath.Length = this.layerPaths.Peek();

			DataDrawer.valuePath.Append('.');
			DataDrawer.valuePath.Append(name);

			this.name = name;

			this.value = value;

			return this;
		}

		public string	GetPath()
		{
			return DataDrawer.valuePath.ToString();
		}

		public LayerPath	CreateLayerChildScope()
		{
			return new LayerPath(this);
		}

		private Stack<int>	layerPaths = new Stack<int>();

		public void	PushPath()
		{
			this.layerPaths.Push(DataDrawer.valuePath.Length);
		}

		public void	PopPath()
		{
			DataDrawer.valuePath.Length = this.layerPaths.Pop();
		}
	}
}