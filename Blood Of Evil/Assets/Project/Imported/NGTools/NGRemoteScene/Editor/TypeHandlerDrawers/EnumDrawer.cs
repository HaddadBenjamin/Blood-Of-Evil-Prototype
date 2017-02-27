using NGTools.NGRemoteScene;
using System;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGRemoteScene
{
	[TypeHandlerDrawerFor(typeof(EnumHandler))]
	internal sealed class EnumDrawer : TypeHandlerDrawer
	{
		private BgColorContentAnimator	anim;

		public	EnumDrawer(TypeHandler typeHandler) : base(typeHandler)
		{
		}

		public override void Draw(Rect r, DataDrawer data)
		{
			if (this.anim == null)
				this.anim = new BgColorContentAnimator(data.inspector.Repaint, 1F, 0F);

			if (data.inspector.Hierarchy.GetUpdateNotification(data.GetPath()) == true)
				this.anim.Start();

			EnumInstance	enumInstance = data.value as EnumInstance;

			using (this.anim.Restorer(0F, .8F + this.anim.Value, 0F, 1F))
			{
				EnumData	enumData = data.inspector.Hierarchy.GetEnumData(enumInstance.type);

				if (enumData != null)
				{
					if (enumInstance.GetFlag() == EnumInstance.IsFlag.Unset)
					{
						if (enumData.hasFlagAttribute == true)
							enumInstance.SetFlag(EnumInstance.IsFlag.Flag);
						else
							enumInstance.SetFlag(EnumInstance.IsFlag.Value);
					}

					float	width = r.width;

					r.width = 16F;
					if (GUI.Button(r, "F", enumInstance.GetFlag() == EnumInstance.IsFlag.Flag ? GUI.skin.button : GUI.skin.textField) == true)
					{
						if (enumInstance.GetFlag() == EnumInstance.IsFlag.Value)
							enumInstance.SetFlag(EnumInstance.IsFlag.Flag);
						else
							enumInstance.SetFlag(EnumInstance.IsFlag.Value);
					}

					r.width = width;

					EditorGUI.BeginChangeCheck();

					int	newValue;

					if (enumInstance.GetFlag() == EnumInstance.IsFlag.Value)
						newValue = EditorGUI.IntPopup(r, Utility.NicifyVariableName(data.name), enumInstance.value, enumData.names, enumData.values);
					else
					{
						newValue = EditorGUI.MaskField(r, Utility.NicifyVariableName(data.name), enumInstance.value, enumData.names);
					}

					if (EditorGUI.EndChangeCheck() == true)
						this.AsyncUpdateCommand(data.unityData, data.GetPath(), newValue, typeof(Enum));
				}
				else
					EditorGUI.LabelField(r, Utility.NicifyVariableName(data.name), "Null");
			}
		}
	}
}