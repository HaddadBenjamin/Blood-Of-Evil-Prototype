using NGTools;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor
{
	[CustomPropertyDrawer(typeof(GroupAttribute), true)]
	internal sealed class GroupDrawer : PropertyDrawer
	{
		public const float	Spacing = 2F;

		internal static bool	isMasterDrawing = false;
		internal static bool	heightChildren = true;

		private bool			init;
		private bool			mustDraw;
		private string			groupName;
		private List<string>	fields;
		private List<SerializedProperty>	props;

		private GUIStyle	richFoldout;

		public override float	GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (this.init == false)
				this.Initialize(property);

			if (this.mustDraw == false)
			{
				if (GroupDrawer.isMasterDrawing == true)
					return EditorGUI.GetPropertyHeight(property, label, GroupDrawer.heightChildren);
				return -GroupDrawer.Spacing;
			}

			float	height = EditorGUIUtility.singleLineHeight; // Group name

			if (EditorPrefs.GetBool(property.propertyPath, true) == true)
			{
				bool	restore = GroupDrawer.isMasterDrawing;
				GroupDrawer.isMasterDrawing = true;

				for (int i = 0; i < this.props.Count; i++)
					height += EditorGUI.GetPropertyHeight(this.props[i], null) + GroupDrawer.Spacing;

				GroupDrawer.isMasterDrawing = restore;
			}

			return height;
		}

		public override void	OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (this.init == false)
				this.Initialize(property);

			if (this.mustDraw == false)
			{
				if (GroupDrawer.isMasterDrawing == true)
					EditorGUI.PropertyField(position, property, label);
				return;
			}

			GroupDrawer.isMasterDrawing = true;

			if (this.richFoldout == null)
			{
				this.richFoldout = new GUIStyle(EditorStyles.foldout);
				this.richFoldout.richText = true;
			}

			position.height = EditorGUIUtility.singleLineHeight;
			bool	isExpanded = EditorPrefs.GetBool(property.propertyPath, true);
			EditorGUI.BeginChangeCheck();
			isExpanded = EditorGUI.Foldout(position, isExpanded, this.groupName, true, this.richFoldout);
			if (EditorGUI.EndChangeCheck() == true)
				EditorPrefs.SetBool(property.propertyPath, isExpanded);

			if (isExpanded == true)
			{
				position.y += position.height + GroupDrawer.Spacing;

				++EditorGUI.indentLevel;
				GroupDrawer.heightChildren = false;
				for (int i = 0; i < this.props.Count; i++)
				{
					if (this.props[i].hasVisibleChildren == true)
					{
						position.height = EditorGUI.GetPropertyHeight(this.props[i], null, false);
						EditorGUI.PropertyField(position, this.props[i], false);
						position.y += position.height + GroupDrawer.Spacing;

						if (this.props[i].isExpanded == true)
						{
							++EditorGUI.indentLevel;

							var		it = this.props[i].Copy();
							var		end = it.GetEndProperty();
							bool	enterChildren = true;

							while (it.NextVisible(enterChildren) == true && SerializedProperty.EqualContents(it, end) == false)
							{
								position.height = EditorGUI.GetPropertyHeight(it, null, true);
								EditorGUI.PropertyField(position, it, true);
								position.y += position.height + GroupDrawer.Spacing;

								enterChildren = false;
							}

							--EditorGUI.indentLevel;
						}
					}
					else
					{
						position.height = EditorGUI.GetPropertyHeight(this.props[i]);
						EditorGUI.PropertyField(position, this.props[i]);
						position.y += position.height + GroupDrawer.Spacing;
					}
				}
				GroupDrawer.heightChildren = true;
				--EditorGUI.indentLevel;
			}

			GroupDrawer.isMasterDrawing = false;
		}

		private void	Initialize(SerializedProperty property)
		{
			this.init = true;

			FieldInfo[]			fields = this.fieldInfo.DeclaringType.GetFields();
			GroupAttribute[]	originAttributes = this.fieldInfo.GetCustomAttributes(typeof(GroupAttribute), true) as GroupAttribute[];
			bool				firstPassed = false;

			this.fields = new List<string>();
			this.mustDraw = false;
			this.groupName = originAttributes[0].group;

			for (int i = 0; i < fields.Length; i++)
			{
				GroupAttribute[]	group = fields[i].GetCustomAttributes(typeof(GroupAttribute), true) as GroupAttribute[];

				if (group.Length > 0)
				{
					if (group[0].group == this.groupName)
					{
						if (firstPassed == false && fields[i] == this.fieldInfo)
							this.mustDraw = true;

						if (group[0].hide == false)
							this.fields.Add(fields[i].Name);

						firstPassed = true;
					}
				}
				else
				{
					InGroupAttribute[]	inGroup = fields[i].GetCustomAttributes(typeof(InGroupAttribute), true) as InGroupAttribute[];

					if (inGroup.Length > 0 &&
						inGroup[0].group == this.groupName)
					{
						this.fields.Add(fields[i].Name);
					}
				}
			}

			if (this.mustDraw == true)
			{
				SerializedProperty	p = property.Copy();
				p.Reset();
				p.Next(true);

				this.props = new List<SerializedProperty>();

				do
				{
					if (this.fields.Contains(p.name))
						this.props.Add(p.Copy());
				}
				while (p.NextVisible(false));
			}
		}
	}
}