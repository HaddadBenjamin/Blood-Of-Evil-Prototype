using NGTools;
using NGTools.Network;
using NGTools.NGRemoteScene;
using System;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGRemoteScene
{
	public class MethodArgumentsWindow : EditorWindow
	{
		private static Type[]	types = null;

		private NGRemoteHierarchyWindow	hierarchy;
		private Client					client;
		private int						parentInstanceID;
		private int						instanceID;
		private ClientMethod			method;
		private ArgumentDrawer[]		drawers;
		private BgColorContentAnimator	invokeFeedbackAnim;

		public void	Init(NGRemoteHierarchyWindow hierarchy, Client client, int parentInstanceID, int instanceID, ClientMethod method)
		{
			this.hierarchy = hierarchy;
			this.client = client;
			this.parentInstanceID = parentInstanceID;
			this.instanceID = instanceID;
			this.method = method;

			this.drawers = new ArgumentDrawer[this.method.argumentNames.Length];

			if (MethodArgumentsWindow.types == null)
				MethodArgumentsWindow.types = Utility.GetSubClassesOf(typeof(ArgumentDrawer));

			for (int i = 0; i < this.method.argumentTypes.Length; i++)
			{
				for (int j = 0; j < MethodArgumentsWindow.types.Length; j++)
				{
					ArgumentDrawerFor[]	attribute = MethodArgumentsWindow.types[j].GetCustomAttributes(typeof(ArgumentDrawerFor), false) as ArgumentDrawerFor[];

					if (attribute.Length > 0)
					{
						if (attribute[0].type.IsAssignableFrom(this.method.argumentTypes[i]) == true)
						{
							this.drawers[i] = Activator.CreateInstance(MethodArgumentsWindow.types[j], this.method.argumentNames[i], this.method.argumentTypes[i]) as ArgumentDrawer;
							this.drawers[i].Load(this.method.name + "." + this.method.argumentNames[i]);
							break;
						}
					}
				}
			}

			this.invokeFeedbackAnim = new BgColorContentAnimator(this.Repaint, 1F, 0F);
		}

		protected virtual void	OnDisable()
		{
			for (int i = 0; i < this.drawers.Length; i++)
			{
				if (this.drawers[i] != null)
					this.drawers[i].Save(this.method.name + "." + this.drawers[i].name);
			}
		}

		protected virtual void	Update()
		{
			if (EditorApplication.isCompiling == true)
			{
				this.Close();
				return;
			}
		}

		protected virtual void	OnGUI()
		{
			if (string.IsNullOrEmpty(this.hierarchy.LastInvokeResult) == false)
				EditorGUILayout.HelpBox("Last Invoke Result (" + this.hierarchy.LastInvokeResultTime.ToString() + ")\n" + this.hierarchy.LastInvokeResult, MessageType.Info);

			GUILayout.Label(this.method.returnType.FullName + '	' + this.method.name);

			bool	missingDrawer = false;

			for (int i = 0; i < this.drawers.Length; i++)
			{
				if (this.drawers[i] != null)
					this.drawers[i].OnGUI();
				else
				{
					EditorGUILayout.LabelField(this.method.argumentNames[i], this.method.argumentTypes[i].Name + " is unsupported.");
					missingDrawer = true;
				}
			}

			using (this.invokeFeedbackAnim.Restorer(0F, .8F + this.invokeFeedbackAnim.Value, 0F, 1F))
			{
				GUI.enabled = !missingDrawer;

				if (GUILayout.Button("Invoke") == true)
				{
					ByteBuffer	buffer = Utility.GetBBuffer();

					try
					{
						for (int i = 0; i < this.drawers.Length; i++)
							this.drawers[i].Append(buffer);

						this.client.AddPacket(new ClientInvokeBehaviourMethodPacket(this.parentInstanceID, this.instanceID, this.method.GetSignature(), Utility.ReturnBBuffer(buffer)));

						this.invokeFeedbackAnim.Start();
					}
					catch (Exception ex)
					{
						InternalNGDebug.LogException("Failed to invoke method on the remote scene.", ex);
					}
				}
			}
		}
	}
}