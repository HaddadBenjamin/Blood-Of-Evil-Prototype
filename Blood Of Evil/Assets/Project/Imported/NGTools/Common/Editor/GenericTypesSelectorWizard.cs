using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor
{
	public class GenericTypesSelectorWizard : ScriptableWizard
	{
		private const string	TextControlName = "filter";
		private static Color	MouseHoverBackgroundColor = new Color(.1F, .1F, .1F, .4F);

		private Action<Type>	OnCreate;
		private Type			type;
		private bool			closeOnCreate;
		private bool			enableCategories;

		private Type[]		types;

		private string		filter;
		private List<Type>	displayingTypes;

		private Dictionary<string, List<Type>>	categories;

		private Type	selected;
		private Vector2	scrollPosition;

		private	float	allNamespaceWidth = 0F;
		private float	namespaceWidth;

		private bool	once;

		/// <summary>
		/// <para>Creates a form containing a list of <paramref name="type"/>.</para>
		/// <para>It will call your callback <paramref name="OnCreate"/> when a type is selected.</para>
		/// </summary>
		/// <see cref="CategoryAttribute"/>
		/// <param name="title">The title of the wizard.</param>
		/// <param name="type">The base type the wizard must use to display all the inherited types.</param>
		/// <param name="OnCreate">A method with the selected type as argument.</param>
		/// <param name="allAssemblies">Get types from all assemblies.</param>
		/// <param name="closeOnCreate">Automatically close the wizard after calling the callback.</param>
		/// <param name="enableCategories">Enable it to display types sorted through categories using the attribute CategoryAttribute on types.</param>
		public static void	Start(string title, Type type, Action<Type> OnCreate, bool allAssemblies, bool closeOnCreate, bool enableCategories = false)
		{
			GenericTypesSelectorWizard.GetWindow<GenericTypesSelectorWizard>(true).Init(title, type, OnCreate, allAssemblies, closeOnCreate, enableCategories);
		}

		public void	Init(string title, Type type, Action<Type> OnCreate, bool allAssemblies, bool closeOnCreate, bool enableCategories = false)
		{
			this.SetTitle(title);
			this.type = type;
			this.OnCreate = OnCreate;
			this.closeOnCreate = closeOnCreate;
			this.enableCategories = enableCategories;
			if (allAssemblies == true)
				this.types = Utility.GetAllSubClassesOf(this.type).OrderBy((a) => a.Name).ToArray();
			else
				this.types = Utility.GetSubClassesOf(this.type).OrderBy((a) => a.Name).ToArray();
			this.filter = string.Empty;
			this.displayingTypes = new List<Type>();
			this.once = false;

			if (this.enableCategories == true)
			{
				this.categories = new Dictionary<string, List<Type>>();

				for (int i = 0; i < this.types.Length; i++)
					this.AddTypeToCategory(this.types[i]);
			}

			this.selected = null;
		}

		protected virtual void	OnGUI()
		{
			if (this.once == false)
			{
				GUI.FocusControl(GenericTypesSelectorWizard.TextControlName);
				this.ProcessMaxNamespaceWidth(this.types);
				this.allNamespaceWidth = this.namespaceWidth;
				this.once = true;
			}

			GUILayout.BeginHorizontal();
			{
				EditorGUI.BeginChangeCheck();
				GUI.SetNextControlName(GenericTypesSelectorWizard.TextControlName);
				this.filter = GUILayout.TextField(this.filter);
				if (EditorGUI.EndChangeCheck() == true)
				{
					if (this.enableCategories == true)
					{
						this.categories.Clear();

						this.ProcessMaxNamespaceWidth(this.types);

						for (int i = 0; i < this.types.Length; i++)
						{
							if (CultureInfo.InvariantCulture.CompareInfo.IndexOf(this.types[i].FullName, this.filter, CompareOptions.IgnoreCase) == -1)
								continue;

							this.AddTypeToCategory(this.types[i]);
						}
					}
					else if (string.IsNullOrEmpty(this.filter) == false)
					{
						this.displayingTypes.Clear();

						for (int i = 0; i < this.types.Length; i++)
						{
							if (CultureInfo.InvariantCulture.CompareInfo.IndexOf(this.types[i].FullName, this.filter, CompareOptions.IgnoreCase) != -1)
								this.displayingTypes.Add(this.types[i]);
						}

						this.ProcessMaxNamespaceWidth(this.displayingTypes);
					}
					else
						this.namespaceWidth = this.allNamespaceWidth;
				}

				GUI.enabled = this.selected != null;
				if (GUILayout.Button(LC.G("Create"), GUILayout.ExpandWidth(false)) == true)
					this.Create(this.selected);
				GUI.enabled = true;
			}
			GUILayout.EndHorizontal();

			this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition);
			{
				if (this.enableCategories == true)
				{
					foreach (var pair in this.categories.OrderBy(i => i.Key))
					{
						GUILayout.Label(pair.Key, GeneralStyles.BigCenterText);

						for (int i = 0; i < pair.Value.Count; i++)
							this.DrawType(pair.Value[i]);
					}
				}
				else
				{
					if (string.IsNullOrEmpty(this.filter) == true)
					{
						for (int i = 0; i < this.types.Length; i++)
							this.DrawType(this.types[i]);
					}
					else
					{
						for (int i = 0; i < this.displayingTypes.Count; i++)
							this.DrawType(this.displayingTypes[i]);
					}
				}
			}
			EditorGUILayout.EndScrollView();

			this.Repaint();
		}

		protected virtual void	Update()
		{
			if (EditorApplication.isCompiling == true)
				this.Close();
		}

		protected virtual void	OnLostFocus()
		{
			this.Close();
		}

		private void	AddTypeToCategory(Type type)
		{
			CategoryAttribute[]	attributes = type.GetCustomAttributes(typeof(CategoryAttribute), true) as CategoryAttribute[];
			List<Type>			cat;
			string				categoryName;

			if (attributes.Length > 0)
				categoryName = attributes[0].name;
			else
				categoryName = CategoryAttribute.DefaultCategory;

			if (this.categories.TryGetValue(categoryName, out cat) == true)
				cat.Add(type);
			else
			{
				cat = new List<Type>();
				cat.Add(type);
				this.categories.Add(categoryName, cat);
			}
		}

		private void	DrawType(Type type)
		{
			using (ColorContentRestorer.Get(type == this.selected ? Color.yellow : GUI.contentColor))
			{
				Rect	r = GUILayoutUtility.GetRect(new GUIContent(""), GUI.skin.button);

				r.width += this.namespaceWidth;

				if (Event.current.type == EventType.Repaint &&
					r.Contains(Event.current.mousePosition) == true)
				{
					EditorGUI.DrawRect(r, GenericTypesSelectorWizard.MouseHoverBackgroundColor);
				}

				if (GUI.Button(r, type.Namespace, GUI.skin.label) == true)
				{
					if (this.selected == type)
						this.Create(this.selected);
					else
						this.selected = type;
				}

				r.x += this.namespaceWidth;
				r.width -= this.namespaceWidth;

				GUI.Label(r, type.Name);
			}
		}

		private void	ProcessMaxNamespaceWidth(IEnumerable<Type> types)
		{
			this.namespaceWidth = 0F;

			foreach (Type type in types)
			{
				Utility.content.text = type.Namespace;
				float	width = GUI.skin.label.CalcSize(Utility.content).x;
				if (this.namespaceWidth < width)
					this.namespaceWidth = width;
			}
		}

		private void	Create(Type type)
		{
			this.OnCreate(type);

			if (this != null && this.closeOnCreate == true)
				this.Close();
		}
	}
}