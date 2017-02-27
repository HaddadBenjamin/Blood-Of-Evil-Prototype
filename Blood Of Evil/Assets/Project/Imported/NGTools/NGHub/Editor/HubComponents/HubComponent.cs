using System;
using UnityEngine;

namespace NGToolsEditor.NGHub
{
	[Serializable, Exportable(ExportableAttribute.ArrayOptions.Overwrite)]
	public abstract class HubComponent
	{
		public const string	StaticVerifierMethodName = "CanDrop";

		[NonSerialized]
		public NGHubWindow	hub;
		public readonly string		name;
		public readonly bool		hasEditorGUI;
		public readonly bool		closeOnLostFocus;

		protected	HubComponent(string name, bool hasEditorGUI = false, bool closeOnLostFocus = false)
		{
			this.name = name;
			this.hasEditorGUI = hasEditorGUI;
			this.closeOnLostFocus = closeOnLostFocus;
		}

		public virtual void	Init(NGHubWindow hub)
		{
			this.hub = hub;
		}

		public virtual void	Uninit()
		{
		}

		public virtual void		OnPreviewGUI(Rect r)
		{
			GUI.Label(r, this.name);
		}

		public virtual void		OnEditionGUI()
		{
		}

		public virtual void		OnGUI()
		{
			GUILayout.Label("NO OnGUI IMPLEMENTED");
		}

		public virtual void		InitDrop(NGHubWindow hub)
		{
			this.hub = hub;
		}
	}
}