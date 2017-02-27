using NGTools;
using NGTools.Network;
using NGTools.NGRemoteScene;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using InnerUtility = NGTools.Utility;

namespace NGToolsEditor.NGRemoteScene
{
	using UnityEngine;

	public sealed class ClientComponent
	{
		public const float	Spacing = 3F;
		public const float	ComponentHeaderHeight = 16F;
		public static Color	BackgroundColorBar = new Color(102F / 255F, 102F / 255F, 102F / 255F);

		private static List<ClientField>	cachedFields = new List<ClientField>(16);

		public readonly ClientGameObject	parent;
		public readonly Type				type;
		public readonly int					instanceID;
		public readonly bool				togglable;
		public readonly bool				deletable;
		public readonly string				name;
		public readonly ClientField[]		fields;
		public readonly ClientMethod[]		methods;
		public readonly TypeHandler			booleanHandler;
		public readonly int					enabledFieldIndex;

		private readonly string[]	methodNames;

		#region Editor
		private readonly Texture2D	icon = null;
		private readonly IUnityData	unityData;
		private bool				fold = true;

		private int					selectedMethod;
		#endregion Editor

		public	ClientComponent(ClientGameObject parent, NetComponent component, IUnityData unityData)
		{
			this.parent = parent;
			this.type = component.type;
			this.unityData = unityData;
			this.instanceID = component.instanceID;
			this.togglable = component.togglable;
			this.deletable = component.deletable;
			this.name = component.name;

			ClientComponent.cachedFields.Clear();

			this.enabledFieldIndex = -1;

			for (int i = 0; i < component.fields.Length; i++)
			{
				if (component.fields[i].name.Equals("enabled") == true)
					this.enabledFieldIndex = i;

				ClientComponent.cachedFields.Add(new ClientField(this, component.fields[i], this.unityData));
			}

			this.fields = ClientComponent.cachedFields.ToArray();

			this.methods = new ClientMethod[component.methods.Length];
			this.methodNames = new string[this.methods.Length];

			for (int i = 0; i < this.methods.Length; i++)
			{
				this.methods[i] = new ClientMethod(this, component.methods[i]);

				StringBuilder	buffer = Utility.GetBuffer();

				buffer.Append(component.methods[i].returnType.Name);
				buffer.Append('	');
				buffer.Append(component.methods[i].name);
				buffer.Append('(');

				string	comma = string.Empty;

				for (int j = 0; j < component.methods[i].argumentTypes.Length; j++)
				{
					buffer.Append(comma);
					buffer.Append(component.methods[i].argumentTypes[j].Name);

					comma = ", ";
				}

				buffer.Append(')');

				this.methodNames[i] = Utility.ReturnBuffer(buffer);
			}

			this.booleanHandler = TypeHandlersManager.GetTypeHandler<bool>();

			if (this.type != null)
				this.icon = AssetPreview.GetMiniTypeThumbnail(this.type);
			if (this.icon == null)
				this.icon = UnityEditorInternal.InternalEditorUtility.GetIconForFile(".cs");
		}

		public ClientField	GetField(string name)
		{
			for (int i = 0; i < this.fields.Length; i++)
			{
				if (this.fields[i].name == name)
					return this.fields[i];
			}

			return null;
		}

		public float	GetHeight(NGRemoteInspectorWindow inspector)
		{
			float	height = ClientComponent.ComponentHeaderHeight + ClientComponent.Spacing; // Component bar

			if (this.fold == true)
			{
				for (int i = 0; i < this.fields.Length; i++)
				{
					if (this.fields[i].isPublic == true && this.enabledFieldIndex != i)
						height += this.fields[i].GetHeight(inspector);
				}
			}

			return height;
		}

		public void		OnGUI(Rect r, NGRemoteInspectorWindow inspector)
		{
			r = this.DrawHeader(r, inspector);

			if (this.fold == false)
				return;

			++EditorGUI.indentLevel;
			for (int i = 0; i < this.fields.Length; i++)
			{
				if (this.fields[i].isPublic == true && this.enabledFieldIndex != i)
				{
					float	height = this.fields[i].GetHeight(inspector);

					if (r.y + height <= inspector.scrollPosition.y)
					{
						r.y += height;
						continue;
					}

					r.height = height;
					this.fields[i].Draw(r, inspector);

					r.y += height;
					if (r.y - inspector.scrollPosition.y > inspector.bodyRect.height)
						break;
				}
			}
			--EditorGUI.indentLevel;
		}

		private void	RemoveComponent()
		{
			this.unityData.AddPacket(new ClientDeleteComponentsPacket(this.parent.instanceID, this.instanceID));
		}

		private void	CopyComponent()
		{
			try
			{
				GameObject	go = new GameObject();

				try
				{
					Component	c;

					if (this.type == typeof(Transform))
						c = go.GetComponent<Transform>();
					else
						c = go.AddComponent(this.type);

					for (int i = 0; i < this.fields.Length; i++)
						this.SetValue(c, this.fields[i].name, this.fields[i].value);

					if (ComponentUtility.CopyComponent(c) == false)
						Debug.LogError("Copy component failed.");
				}
				finally
				{
					GameObject.DestroyImmediate(go);
				}
			}
			catch (Exception ex)
			{
				InternalNGDebug.LogException("Copy component failed.", ex);
			}
		}

		private object	ConvertValue(Type type, object value)
		{
			GenericClass	o = value as GenericClass;

			if (o != null)
			{
				object	result = Activator.CreateInstance(type);

				for (int j = 0; j < o.names.Length; j++)
				{
					FieldInfo	subField = type.GetField(o.names[j]);
					InternalNGDebug.Assert(subField != null, "Field \"" + o.names[j] + "\" was not found in type \"" + type + "\".");

					if (subField.IsLiteral == true)
						continue;

					result = this.SetValue(result, o.names[j], o.values[j]);
				}

				return result;
			}
			else
			{
				ArrayData	a = value as ArrayData;

				if (a != null)
				{
					if (a.array != null)
					{
						if (type.IsArray == true)
						{
							Type	subType = Utility.GetArraySubType(type);
							Array	result = Array.CreateInstance(type, a.array.Length);

							for (int j = 0; j < a.array.Length; j++)
								result.SetValue(this.ConvertValue(subType, a.array.GetValue(j)), j);

							return result;
						}
						else if (typeof(IList).IsAssignableFrom(type) == true)
						{
							IList	result = Activator.CreateInstance(type) as IList;
							Type	subType = Utility.GetArraySubType(type);

							for (int j = 0; j < a.array.Length; j++)
								result.Add(this.ConvertValue(subType, a.array.GetValue(j)));

							return result;
						}
						else
							throw new InvalidCastException("Type \"" + type + "\" is not supported as an array.");
					}

					return null;
				}
				else
					return value;
			}
		}

		private object	SetValue(object instance, string name, object value)
		{
			if (value is UnityObject)
				return instance;

			GenericClass	o = value as GenericClass;
			IFieldModifier	field = InnerUtility.GetFieldInfo(instance.GetType(), name);
			InternalNGDebug.Assert(field != null, "Field \"" + name + "\" was not found in type \"" + instance.GetType() + "\".");
			object			fieldValue = field.GetValue(instance);

			if (o != null)
			{
				if (fieldValue == null)
					fieldValue = Activator.CreateInstance(field.Type);

				for (int j = 0; j < o.names.Length; j++)
				{
					FieldInfo	subField = field.Type.GetField(o.names[j]);
					InternalNGDebug.Assert(subField != null, "Field \"" + o.names[j] + "\" was not found in type \"" + field.Type + "\".");

					if (subField.IsLiteral == true)
						continue;

					fieldValue = this.SetValue(fieldValue, o.names[j], o.values[j]);
				}

				field.SetValue(instance, fieldValue);

				return instance;
			}
			else
			{
				ArrayData	a = value as ArrayData;

				if (a != null)
				{
					if (a.array != null)
					{
						Type	subType = Utility.GetArraySubType(field.Type);
						if (fieldValue == null)
							fieldValue = Array.CreateInstance(subType, a.array.Length);

						if (field.Type.IsArray == true)
						{
							Array	fieldArray = fieldValue as Array;

							for (int j = 0; j < a.array.Length; j++)
								fieldArray.SetValue(this.ConvertValue(subType, a.array.GetValue(j)), j);
						}
						else if (typeof(IList).IsAssignableFrom(field.Type) == true)
						{
							IList	fieldArray = fieldValue as IList;

							for (int j = 0; j < a.array.Length; j++)
								fieldArray[j] = this.ConvertValue(subType, a.array.GetValue(j));
						}
						else
							throw new InvalidCastException("Type \"" + field.Type + "\" is not supported as an array.");

						field.SetValue(instance, fieldValue);
					}
				}
				else if (value is EnumInstance)
					field.SetValue(instance, (value as EnumInstance).value);
				else
					field.SetValue(instance, value);
			}

			return instance;
		}

		private Rect	DrawHeader(Rect r, NGRemoteInspectorWindow inspector)
		{
			if (Event.current.type == EventType.Repaint)
			{
				Rect	bar = r;

				bar.y += 1F;
				bar.x = 0F;
				bar.width = inspector.position.width;
				bar.height = 1F;

				EditorGUI.DrawRect(bar, ClientComponent.BackgroundColorBar);
			}

			r.y += ClientComponent.Spacing;

			float	x = r.x;
			float	width = r.width;
			float	height = r.height;

			r.height = ClientComponent.ComponentHeaderHeight;

			if (this.type != null &&
				Event.current.type == EventType.MouseDown &&
				Event.current.button == 1 &&
				r.Contains(Event.current.mousePosition) == true)
			{
				GenericMenu	menu = new GenericMenu();

				if (this.deletable == true)
					menu.AddItem(new GUIContent("Remove Component"), false, this.RemoveComponent);
				menu.AddItem(new GUIContent("Copy Component"), false, this.CopyComponent);
				menu.ShowAsContext();

				Event.current.Use();
			}

			if (this.togglable == true)
			{
				Rect	r2 = r;

				r2.width = 16F;
				r2.x += 34F;

				EditorGUI.BeginChangeCheck();
				bool	enable = EditorGUI.Toggle(r2, (bool)this.fields[this.enabledFieldIndex].value);
				if (EditorGUI.EndChangeCheck() == true)
					this.unityData.AddPacket(new ClientUpdateFieldValuePacket(this.parent.instanceID.ToString() + "." + this.instanceID.ToString() + ".enabled", this.booleanHandler.Serialize(enable), this.booleanHandler));
			}

			r.width -= 170F;

			this.fold = EditorGUI.Foldout(r, this.fold, GUIContent.none, true);

			Rect	r3 = r;
			r3.x += 16F;
			r3.width = 16F;
			GUI.DrawTexture(r3, this.icon);

			r.x += 48F;
			r.width -= 48F;
			EditorGUI.LabelField(r, new GUIContent(Utility.NicifyVariableName(this.name)), GeneralStyles.ComponentName);

			r.x += r.width;
			r.width = 170F - 20F;

			this.selectedMethod = EditorGUI.Popup(r, this.selectedMethod, this.methodNames);

			r.x += r.width;
			r.width = 20F;

			if (GUI.Button(r, "@") == true)
			{
				MethodArgumentsWindow	window = EditorWindow.GetWindow<MethodArgumentsWindow>("Method Invoker Form");
				window.Init(inspector.Hierarchy, this.unityData.Client, this.parent.instanceID, this.instanceID, this.methods[this.selectedMethod]);
			}

			r.x = x;
			r.width = width;
			r.height = height;

			r.y += EditorGUIUtility.singleLineHeight;
			r.height -= EditorGUIUtility.singleLineHeight;

			return r;
		}
	}
}