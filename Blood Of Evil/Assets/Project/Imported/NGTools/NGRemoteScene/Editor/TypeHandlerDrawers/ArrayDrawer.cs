using NGTools.NGRemoteScene;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGRemoteScene
{
	internal sealed class ArrayDrawer : TypeHandlerDrawer
	{
		private List<TypeHandlerDrawer>	subDrawers;
		private bool					fold;
		private Type					subType;
		private TypeHandler				subHandler;

		public	ArrayDrawer(TypeHandler typeHandler, Type type, object value) : base(typeHandler)
		{
			this.subType = Utility.GetArraySubType(type);
			this.subHandler = TypeHandlersManager.GetTypeHandler(this.subType);
			this.subDrawers = new List<TypeHandlerDrawer>();
		}

		public override float	GetHeight(object value)
		{
			if (this.fold == false)
				return EditorGUIUtility.singleLineHeight;

			ArrayData	array = value as ArrayData;

			if (array.array == null)
				return EditorGUIUtility.singleLineHeight + EditorGUIUtility.singleLineHeight;

			float	height = 32F; // First line + Size
			int		i = 0;

			foreach (var item in array.array)
			{
				if (item != null)
				{
					// Add new drawer for new element.
					if (this.subDrawers.Count <= i)
						this.subDrawers.Add(TypeHandlerDrawersManager.CreateTypeHandlerDrawer(this.subHandler, this.subType, item));

					height += this.subDrawers[i].GetHeight(item);
				}
				else
					height += EditorGUIUtility.singleLineHeight;

				++i;
			}

			return height;
		}

		public override void	Draw(Rect r, DataDrawer data)
		{
			ArrayData	array = data.value as ArrayData;

			if (array.array == null)
			{
				--EditorGUI.indentLevel;
				this.fold = EditorGUI.Foldout(r, this.fold, Utility.NicifyVariableName(data.name) + " Null", true);
				++EditorGUI.indentLevel;

				if (this.fold == false)
					return;

				r.y += r.height;

				GUI.Label(r, "Array was not loaded because it has more elements than " + ArrayHandler.BigArrayThreshold + ".");
				r.y += r.height;

				if (GUI.Button(r, "Load") == true)
					data.inspector.Hierarchy.LoadBigArray(data.GetPath());

				return;
			}

			r.height = EditorGUIUtility.singleLineHeight;

			--EditorGUI.indentLevel;
			this.fold = EditorGUI.Foldout(r, this.fold, Utility.NicifyVariableName(data.name) + " " + array.array.Length.ToString(), true);
			++EditorGUI.indentLevel;

			if (this.fold == false)
				return;

			r.y += r.height;

			++EditorGUI.indentLevel;
			EditorGUI.BeginChangeCheck();
			int	newSize = EditorGUI.IntField(r, "Size", array.array.Length);
			if (EditorGUI.EndChangeCheck() == true)
				this.AsyncUpdateCommand(data.unityData, data.GetPath(), newSize, typeof(int), TypeHandlersManager.GetTypeHandler(typeof(int)));

			r.y += r.height;

			if (array.array != null)
			{
				using (data.CreateLayerChildScope())
				{
					int	i = 0;
					foreach (var item in array.array)
					{
						// Add new drawer for new element.
						if (this.subDrawers.Count <= i)
							this.subDrawers.Add(TypeHandlerDrawersManager.CreateTypeHandlerDrawer(this.subHandler, this.subType, item));

						float	height = this.subDrawers[i].GetHeight(item);

						if (r.y + height <= data.inspector.scrollPosition.y)
						{
							r.y += height;
							continue;
						}

						if (item != null)
							this.subDrawers[i].Draw(r, data.DrawChild(i.ToString(), item));
						else
							EditorGUI.LabelField(r, i.ToString(), "Null");

						r.y += height;
						if (r.y - data.inspector.scrollPosition.y > data.inspector.bodyRect.height)
						{
							// Override i to prevent removing unwanted subDrawers.
							i = int.MaxValue;
							break;
						}

						++i;
					}

					// Drawer are linked to their item, therefore they must be removed as their item is removed.
					if (i < this.subDrawers.Count)
						this.subDrawers.RemoveRange(i, this.subDrawers.Count - i);
				}
			}
			--EditorGUI.indentLevel;
		}
	}
}