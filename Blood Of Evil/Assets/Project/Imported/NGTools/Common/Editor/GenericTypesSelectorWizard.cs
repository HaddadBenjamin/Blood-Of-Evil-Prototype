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
		private static Color	SelectedTypeBackgroundColor = Color.green * .5F;

		/// <summary>Enable it to display types sorted through categories using the attribute CategoryAttribute on types.</summary>
		private bool			enableCategories;
		public bool				EnableCategories
		{
			get
			{
				return this.enableCategories;
			}
			set
			{
				this.enableCategories = value;

				if (value == true && this.categories == null)
				{
					this.categories = new Dictionary<string, List<Type>>();

					for (int i = 0; i < this.types.Length; i++)
						this.AddTypeToCategory(this.types[i]);
				}
			}
		}

		/// <summary>Enable it to display a null value as first line.</summary>
		public bool	EnableNullValue { get; set; }
		public Type	SelectedType { get; set; }

		private Action<Type>	OnCreate;
		private Type			type;
		private bool			closeOnCreate;

		private Type[]		types;

		private string		filter;
		private List<Type>	displayingTypes = new List<Type>();
		private List<Type>	temporaryFilterTypes = new List<Type>();

		private Dictionary<string, List<Type>>	categories;

		private VerticalScrollbar	scrollbar;

		private float	allNamespaceWidth = 0F;
		private float	allNameWidth = 0F;
		private float	namespaceWidth;
		private float	nameWidth;

		private bool	GUIOnce;
		private bool	scrollWitnessOnce;

		/// <summary>
		/// <para>Creates a form containing a list of <paramref name="type"/>.</para>
		/// <para>It will call your callback <paramref name="OnCreate"/> when a type is selected.</para>
		/// </summary>
		/// <see cref="CategoryAttribute"/>
		/// <param name="title">The title of the wizard.</param>
		/// <param name="type">The base type the wizard must use to display all the inherited types.</param>
		/// <param name="OnCreate">A method with the selected type as argument.</param>
		/// <param name="allAssemblies">Get types from all assemblies if true, or only in editor assembly.</param>
		/// <param name="closeOnCreate">Automatically close the wizard after calling the callback.</param>
		public static GenericTypesSelectorWizard	Start(string title, Type type, Action<Type> OnCreate, bool allAssemblies, bool closeOnCreate)
		{
			return GenericTypesSelectorWizard.GetWindow<GenericTypesSelectorWizard>(true).Init(title, type, OnCreate, allAssemblies, closeOnCreate);
		}

		public GenericTypesSelectorWizard	Init(string title, Type type, Action<Type> OnCreate, bool allAssemblies, bool closeOnCreate)
		{
			this.SetTitle(title);
			this.type = type;
			this.OnCreate = OnCreate;
			this.closeOnCreate = closeOnCreate;
			this.EnableCategories = false;
			this.EnableNullValue = false;
			if (allAssemblies == true)
				this.types = Utility.GetAllSubClassesOf(this.type).OrderBy((a) => a.Name).ToArray();
			else
				this.types = Utility.GetSubClassesOf(this.type).OrderBy((a) => a.Name).ToArray();
			this.filter = string.Empty;
			this.GUIOnce = false;
			this.scrollWitnessOnce = false;

			if (this.EnableCategories == true)
			{
				this.categories = new Dictionary<string, List<Type>>();

				for (int i = 0; i < this.types.Length; i++)
					this.AddTypeToCategory(this.types[i]);
			}

			this.SelectedType = null;

			this.scrollbar = new VerticalScrollbar(0F, 0F, this.position.height);
			this.scrollbar.interceiptEvent = true;

			this.wantsMouseMove = true;

			return this;
		}

		protected virtual void	OnGUI()
		{
			if (this.GUIOnce == false)
			{
				this.GUIOnce = true;
				GUI.FocusControl(GenericTypesSelectorWizard.TextControlName);
				this.ProcessMaxNamespaceWidth(this.types);
				this.allNamespaceWidth = this.namespaceWidth;
				this.allNameWidth = this.nameWidth;
			}

			GUILayout.BeginHorizontal(GeneralStyles.Toolbar);
			{
				EditorGUI.BeginChangeCheck();
				GUI.SetNextControlName(GenericTypesSelectorWizard.TextControlName);

				using (BgColorContentRestorer.Get(this.temporaryFilterTypes.Count == 0 && string.IsNullOrEmpty(this.filter) == false, Color.red))
				{
					this.filter = GUILayout.TextField(this.filter, GeneralStyles.ToolbarSearchTextField);
					if (GUILayout.Button(GUIContent.none, GeneralStyles.ToolbarSearchCancelButton) == true)
						this.filter = string.Empty;
				}
				if (EditorGUI.EndChangeCheck() == true)
				{
					this.scrollWitnessOnce = false;
					this.scrollbar.ClearInterests();

					if (this.EnableCategories == true)
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
						this.temporaryFilterTypes.Clear();

						for (int i = 0; i < this.types.Length; i++)
						{
							if (CultureInfo.InvariantCulture.CompareInfo.IndexOf(this.types[i].FullName, this.filter, CompareOptions.IgnoreCase) != -1)
								this.temporaryFilterTypes.Add(this.types[i]);
						}

						if (this.temporaryFilterTypes.Count > 0)
						{
							List<Type>	tmp = this.displayingTypes;
							this.displayingTypes = this.temporaryFilterTypes;
							this.temporaryFilterTypes = tmp;
							// Add a fake value to avoid the input filter turning red.
							this.temporaryFilterTypes.Add(null);
							this.ProcessMaxNamespaceWidth(this.displayingTypes);
						}
						else
						{
							if (this.displayingTypes.Count == 0)
								this.displayingTypes.AddRange(this.types);

							this.ProcessMaxNamespaceWidth(this.displayingTypes);
						}
					}
					else
					{
						this.displayingTypes.Clear();
						this.namespaceWidth = this.allNamespaceWidth;
						this.nameWidth = this.allNameWidth;
						this.ResizeWindow();
					}
				}

				EditorGUI.BeginDisabledGroup(this.SelectedType == null && this.EnableNullValue == false);
				{
					if (GUILayout.Button(LC.G("Select"), GeneralStyles.ToolbarButton, GUILayout.ExpandWidth(false)) == true)
						this.Create(this.SelectedType);
				}
				EditorGUI.EndDisabledGroup();
			}
			GUILayout.EndHorizontal();

			Rect	r = GUILayoutUtility.GetRect(0F, EditorGUIUtility.singleLineHeight);
			float	rowHeight = r.height;

			if (Event.current.type == EventType.Repaint)
			{
				float	totalHeight = 0F;

				if (this.EnableNullValue == true)
					totalHeight = rowHeight;

				if (this.EnableCategories == true)
				{
					float	categoryHeight = GeneralStyles.BigCenterText.lineHeight + GeneralStyles.BigCenterText.padding.vertical;

					foreach (var pair in this.categories.OrderBy(i => i.Key))
						totalHeight += categoryHeight + (pair.Value.Count * rowHeight);
				}
				else if (string.IsNullOrEmpty(this.filter) == true)
					totalHeight += this.types.Length * rowHeight;
				else
					totalHeight += this.displayingTypes.Count * rowHeight;

				this.scrollbar.realHeight = totalHeight;
				this.scrollbar.SetPosition(this.position.width - 15F, r.y);
				this.scrollbar.SetSize(this.position.height - r.y);
			}

			this.scrollbar.OnGUI();

			Rect	bodyRect = this.position;
			bodyRect.x = 0F;
			bodyRect.y = r.y;
			bodyRect.height -= r.y;

			GUI.BeginGroup(bodyRect);
			{
				r.y = -this.scrollbar.offsetY;
				r.width -= this.scrollbar.maxWidth;

				if (this.EnableNullValue == true)
				{
					this.DrawType(r, null, string.Empty, "Null");
					r.y += r.height;
				}

				if (this.EnableCategories == true)
				{
					float	categoryHeight = GeneralStyles.BigCenterText.lineHeight + GeneralStyles.BigCenterText.padding.vertical;

					foreach (var pair in this.categories.OrderBy(i => i.Key))
					{
						r.height = categoryHeight;
						GUI.Label(r, pair.Key, GeneralStyles.BigCenterText);
						r.y += r.height;

						r.height = rowHeight;

						for (int i = 0; i < pair.Value.Count; i++)
						{
							if (this.scrollWitnessOnce == true && r.y + r.height <= 0)
							{
								r.y += r.height;
								continue;
							}

							this.DrawType(r, pair.Value[i], pair.Value[i].Namespace, pair.Value[i].Name);

							r.y += r.height;

							if (this.scrollWitnessOnce == true && r.y > this.scrollbar.maxHeight)
								break;
						}
					}
				}
				else if (string.IsNullOrEmpty(this.filter) == true)
				{
					for (int i = 0; i < this.types.Length; i++)
					{
						if (this.scrollWitnessOnce == true && r.y + r.height <= 0)
						{
							r.y += r.height;
							continue;
						}

						this.DrawType(r, this.types[i], this.types[i].Namespace, this.types[i].Name);

						r.y += r.height;

						if (this.scrollWitnessOnce == true && r.y > this.scrollbar.maxHeight)
							break;
					}
				}
				else
				{
					for (int i = 0; i < this.displayingTypes.Count; i++)
					{
						if (this.scrollWitnessOnce == true && r.y + r.height <= 0)
						{
							r.y += r.height;
							continue;
						}

						this.DrawType(r, this.displayingTypes[i], this.displayingTypes[i].Namespace, this.displayingTypes[i].Name);

						r.y += r.height;

						if (this.scrollWitnessOnce == true && r.y > this.scrollbar.maxHeight)
							break;
					}
				}
			}
			GUI.EndGroup();
		}

		protected virtual void	Update()
		{
			if (EditorApplication.isCompiling == true)
				this.Close();
		}

		protected virtual void	OnLostFocus()
		{
#if UNITY_4_5 || UNITY_4_6 || UNITY_4_7
			// HACK: Workaround to avoid crash in Unity 4.
			EditorApplication.delayCall += this.Close;
#else
			this.Close();
#endif
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

		private void	DrawType(Rect r, Type type, string @namespace, string name)
		{
			if (this.scrollWitnessOnce == false &&
				Event.current.type == EventType.Repaint &&
				this.SelectedType == type)
			{
				this.scrollWitnessOnce = true;
				this.scrollbar.ClearInterests();
				this.scrollbar.AddInterest(r.y + (r.height * .5F) + this.scrollbar.offsetY, GenericTypesSelectorWizard.SelectedTypeBackgroundColor / GenericTypesSelectorWizard.SelectedTypeBackgroundColor.a);
				this.Repaint();
			}

			if (Event.current.type == EventType.Repaint)
			{
				if (this.SelectedType == type)
					EditorGUI.DrawRect(r, GenericTypesSelectorWizard.SelectedTypeBackgroundColor);
				if (r.Contains(Event.current.mousePosition) == true)
					EditorGUI.DrawRect(r, GenericTypesSelectorWizard.MouseHoverBackgroundColor);
			}

			if (GUI.Button(r, @namespace, GUI.skin.label) == true)
			{
				if (this.SelectedType == type)
					this.Create(this.SelectedType);
				else
					this.SelectedType = type;

				this.scrollbar.ClearInterests();
				this.scrollbar.AddInterest(r.y + (r.height * .5F) + this.scrollbar.offsetY, GenericTypesSelectorWizard.SelectedTypeBackgroundColor / GenericTypesSelectorWizard.SelectedTypeBackgroundColor.a);
			}

			r.x += this.namespaceWidth;
			r.width -= this.namespaceWidth;

			GUI.Label(r, name);
		}

		private void	ProcessMaxNamespaceWidth(IEnumerable<Type> types)
		{
			this.namespaceWidth = 0F;
			this.nameWidth = 0F;

			foreach (Type type in types)
			{
				Utility.content.text = type.Namespace;
				float	width = GUI.skin.label.CalcSize(Utility.content).x;
				if (this.namespaceWidth < width)
					this.namespaceWidth = width;
			}

			foreach (Type type in types)
			{
				Utility.content.text = type.Name;
				float	width = GUI.skin.label.CalcSize(Utility.content).x;
				if (this.nameWidth < width)
					this.nameWidth = width;
			}

			this.ResizeWindow();
		}

		private void	ResizeWindow()
		{
			// Resize the window if too small.
			if (this.namespaceWidth > 0F && this.position.width != this.namespaceWidth + this.nameWidth + 15F) // Namespace + Name + Scrollbar
				this.position = new Rect(this.position.x, this.position.y, this.namespaceWidth + this.nameWidth + 15F, this.position.height);
		}

		private void	Create(Type type)
		{
			this.OnCreate(type);

			if (this != null && this.closeOnCreate == true)
				this.Close();
		}
	}
}