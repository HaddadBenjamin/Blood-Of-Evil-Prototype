using NGTools;
using NGTools.NGRemoteScene;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGRemoteScene
{
	internal sealed class ClassDrawer : TypeHandlerDrawer
	{
		private readonly GenericClass	genericClass;
		private TypeHandlerDrawer[]		fieldDrawers;
		private bool					fold;

		public	ClassDrawer(TypeHandler typeHandler, object genericClass) : base(typeHandler)
		{
			this.genericClass = genericClass as GenericClass;
			InternalNGDebug.Assert(genericClass != null, "Argument genericClass is null in ClassDrawer's constructor.");
		}

		public override float	GetHeight(object value)
		{
			if (this.genericClass.isNull == true ||
				this.fold == false)
			{
				return EditorGUIUtility.singleLineHeight;
			}

			float	height = 16F;

			if (this.fieldDrawers == null)
			{
				this.fieldDrawers = new TypeHandlerDrawer[this.genericClass.names.Length];

				for (int i = 0; i < this.genericClass.names.Length; i++)
				{
					TypeHandler	subHandler = TypeHandlersManager.GetTypeHandler(this.genericClass.types[i]);

					this.fieldDrawers[i] = TypeHandlerDrawersManager.CreateTypeHandlerDrawer(subHandler, this.genericClass.types[i], this.genericClass.values[i]);
				}
			}

			for (int i = 0; i < this.genericClass.names.Length; i++)
				height += this.fieldDrawers[i].GetHeight(this.genericClass.values[i]);

			return height;
		}

		public override void	Draw(Rect r, DataDrawer data)
		{
			r.height = EditorGUIUtility.singleLineHeight;

			if (this.genericClass.isNull == true)
			{
				EditorGUI.LabelField(r, Utility.NicifyVariableName(data.name), "Null");
				return;
			}

			//Rect	r = EditorGUILayout.GetControlRect();

			--EditorGUI.indentLevel;
			this.fold = EditorGUI.Foldout(r, this.fold, Utility.NicifyVariableName(data.name), true);
			++EditorGUI.indentLevel;

			if (this.fold == false)
				return;

			r.y += r.height;

			if (this.fieldDrawers == null)
			{
				this.fieldDrawers = new TypeHandlerDrawer[this.genericClass.names.Length];

				for (int i = 0; i < this.genericClass.names.Length; i++)
				{
					TypeHandler	subHandler = TypeHandlersManager.GetTypeHandler(this.genericClass.types[i]);

					this.fieldDrawers[i] = TypeHandlerDrawersManager.CreateTypeHandlerDrawer(subHandler, this.genericClass.types[i], this.genericClass.values[i]);
				}
			}

			using (data.CreateLayerChildScope())
			{
				++EditorGUI.indentLevel;
				for (int i = 0; i < this.genericClass.names.Length; i++)
				{
					r.height = this.fieldDrawers[i].GetHeight(this.genericClass.values[i]);

					if (r.y + r.height <= data.inspector.scrollPosition.y)
					{
						r.y += r.height;
						continue;
					}

					this.fieldDrawers[i].Draw(r, data.DrawChild(this.genericClass.names[i], this.genericClass.values[i]));

					r.y += r.height;
					if (r.y - data.inspector.scrollPosition.y > data.inspector.bodyRect.height)
						break;
				}
				--EditorGUI.indentLevel;
			}
		}
	}
}