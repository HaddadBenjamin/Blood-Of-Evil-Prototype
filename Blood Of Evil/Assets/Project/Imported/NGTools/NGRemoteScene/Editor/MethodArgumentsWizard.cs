using NGTools;
using System;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor
{
	public class MethodArgumentsWizard : ScriptableWizard
	{
		private static Type[]	types = null;

		private Client					client;
		private int						parentInstanceID;
		private int						instanceID;
		private ClientMethod			method;
		private ArgumentDrawer[]		drawers;
		private BgColorContentAnimator	invokeFeedbackAnim;

		public void	Init(Client client, int parentInstanceID, int instanceID, ClientMethod method)
		{
			this.client = client;
			this.parentInstanceID = parentInstanceID;
			this.instanceID = instanceID;
			this.method = method;

			this.drawers = new ArgumentDrawer[this.method.argumentNames.Length];

			if (MethodArgumentsWizard.types == null)
				MethodArgumentsWizard.types = Utility.GetSubClassesOf(typeof(ArgumentDrawer));

			for (int i = 0; i < this.method.argumentTypes.Length; i++)
			{
				for (int j = 0; j < MethodArgumentsWizard.types.Length; j++)
				{
					ArgumentDrawerFor[]	attribute = MethodArgumentsWizard.types[j].GetCustomAttributes(typeof(ArgumentDrawerFor), false) as ArgumentDrawerFor[];

					if (attribute.Length > 0)
					{
						if (attribute[0].type == this.method.argumentTypes[i])
						{
							this.drawers[i] = Activator.CreateInstance(MethodArgumentsWizard.types[j], this.method.argumentNames[i]) as ArgumentDrawer;
							this.drawers[i].Deserialize(this.method.name + "." + this.method.argumentNames[i]);
							break;
						}
					}
				}
			}

			this.invokeFeedbackAnim = new BgColorContentAnimator(this.Repaint, 1F, 0F);
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
			GUILayout.Label(this.method.name);

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
						{
							this.drawers[i].Append(buffer);
						}

						this.client.AddPacket(new ClientInvokeBehaviourMethodPacket(this.parentInstanceID, this.instanceID, this.method.name, Utility.ReturnBBuffer(buffer)));

						this.invokeFeedbackAnim.Start();
					}
					catch (Exception ex)
					{
						InternalNGDebug.LogException("Failed to invoke method on the remote scene.", ex);
					}
				}
			}
		}

		protected virtual void	OnLostFocus()
		{
			for (int i = 0; i < this.drawers.Length; i++)
			{
				if (this.drawers[i] != null)
					this.drawers[i].Serialize(this.method.name + "." + this.drawers[i].name);
			}

			this.Close();
		}
	}
}