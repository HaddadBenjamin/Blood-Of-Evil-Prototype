using NGTools;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEditor;

namespace NGToolsEditor.NGComponentReplacer
{
	using UnityEngine;

	public abstract class TypeConverter
	{
		public readonly bool	hasGUI;

		protected	TypeConverter(bool hasGUI)
		{
			this.hasGUI = hasGUI;
		}

		public abstract bool	CanConvert(Type from, Type to);
		public abstract object	Convert(Type from, Type to, object value);

		public virtual float	GetHeight()
		{
			return 0F;
		}

		public virtual void	OnGUI(Rect position)
		{
		}

		public virtual void	Save(ByteBuffer buffer)
		{
		}

		public virtual void	Load(ByteBuffer buffer)
		{
		}
	}
	
	internal sealed class NestedFieldConverter : TypeConverter
	{
		public List<string>	targetFields = new List<string>();

		public	NestedFieldConverter() : base(true)
		{
		}

		public override bool	CanConvert(Type from, Type to)
		{
			return (from.IsPrimitive == true || from == typeof(Decimal) || from == typeof(String)) && typeof(Object).IsAssignableFrom(to) == false && (to.IsClass == true || to.IsStruct() == true);
		}

		public override object	Convert(Type from, Type to, object value)
		{
			if (value == null)
			{
				if (to == typeof(string))
					return string.Empty;
				return Activator.CreateInstance(to);
			}

			return string.Empty;
		}

		public override float	GetHeight()
		{
			return 0F;
		}

		public override void	OnGUI(Rect position)
		{
		}
	}

	internal sealed class PrimitivesAndStringConverter : TypeConverter
	{
		public	PrimitivesAndStringConverter() : base(false)
		{
		}

		public override bool	CanConvert(Type from, Type to)
		{
			return (to.IsPrimitive == true || to == typeof(string)) && (from.IsPrimitive == true || from == typeof(string));
		}

		public override object	Convert(Type from, Type to, object value)
		{
			if (value == null)
			{
				if (to == typeof(string))
					return string.Empty;
				return Activator.CreateInstance(to);
			}

			try
			{
				if (to == typeof(string))
					return (value as IConvertible).ToString(CultureInfo.CurrentCulture);

				if (to == typeof(Char))
					return (value as IConvertible).ToChar(CultureInfo.CurrentCulture);

				if (to == typeof(Boolean))
					return (value as IConvertible).ToBoolean(CultureInfo.CurrentCulture);

				if (to == typeof(Byte))
					return (value as IConvertible).ToByte(CultureInfo.CurrentCulture);
				if (to == typeof(SByte))
					return (value as IConvertible).ToSByte(CultureInfo.CurrentCulture);

				if (to == typeof(Int16))
					return (value as IConvertible).ToInt16(CultureInfo.CurrentCulture);
				if (to == typeof(Int32))
					return (value as IConvertible).ToInt32(CultureInfo.CurrentCulture);
				if (to == typeof(Int64))
					return (value as IConvertible).ToInt64(CultureInfo.CurrentCulture);

				if (to == typeof(UInt16))
					return (value as IConvertible).ToUInt16(CultureInfo.CurrentCulture);
				if (to == typeof(UInt32))
					return (value as IConvertible).ToUInt32(CultureInfo.CurrentCulture);
				if (to == typeof(UInt64))
					return (value as IConvertible).ToUInt64(CultureInfo.CurrentCulture);
			
				if (to == typeof(Single))
					return (value as IConvertible).ToSingle(CultureInfo.CurrentCulture);
				if (to == typeof(Double))
					return (value as IConvertible).ToDouble(CultureInfo.CurrentCulture);
				if (to == typeof(Decimal))
					return (value as IConvertible).ToDecimal(CultureInfo.CurrentCulture);
			}
			catch (Exception ex)
			{
				Debug.LogException(new Exception("Type convertion failed from " + from.Name + " to " + to.Name + " with " + value + ".", ex));
			}

			if (to == typeof(string))
				return string.Empty;
			return Activator.CreateInstance(to);
		}
	}

	[InitializeOnLoad]
	public class NGComponentReplacerWindow : EditorWindow, IHasCustomMenu
	{
		private sealed class PopupExample : PopupWindowContent
		{
			private const float	ConvertionHeaderHeight = 20F;

			private NGComponentReplacerWindow	parent;
			private List<ConvertionSetup>		convertions;

			public	PopupExample(NGComponentReplacerWindow parent, List<ConvertionSetup> convertions)
			{
				this.parent = parent;
				this.convertions = convertions;
			}

			public override Vector2	GetWindowSize()
			{
				float	height = this.convertions.Count * PopupExample.ConvertionHeaderHeight;

				// Delete all button.
				if (this.convertions.Count > 1)
					height += 24F;

				for (int i = 0; i < this.convertions.Count; i++)
				{
					height += this.convertions[i].converters.Count * PopupExample.ConvertionHeaderHeight;
					for (int j = 0; j < this.convertions[i].converters.Count; j++)
					{
						if (this.convertions[i].converters[j].hasGUI == true)
							height += this.convertions[i].converters[j].GetHeight();
					}
				}

				return new Vector2(400F, height);
			}

			public override void	OnGUI(Rect rect)
			{
				if (this.convertions.Count > 1)
				{
					if (GUILayout.Button("Delete All") == true && ((Event.current.modifiers & Constants.ByPassPromptModifier) != 0 || EditorUtility.DisplayDialog(NGComponentReplacerWindow.Title, "Please confirm deleting all convertions?", "Yes", "No") == true))
					{
						for (int i = 0; i < this.convertions.Count; i++)
							this.parent.links.RemoveAt(this.parent.links.IndexOf(this.convertions[i]));
						this.parent.Focus();
						return;
					}
				}

				Rect	r = GUILayoutUtility.GetRect(this.editorWindow.position.width, PopupExample.ConvertionHeaderHeight, GUI.skin.label);
				float	x = r.x;
				float	w = r.width;

				for (int i = 0; i < this.convertions.Count; i++)
				{
					r.x = x;
					r.width = w - 20F;
					GUI.Label(r, this.parent.targetFields[this.convertions[i].target].Name + " > " + this.parent.replaceFields[this.convertions[i].replace].Name);

					r.x += r.width;
					r.width = 20F;
					if (GUI.Button(r, "X") == true)
					{
						this.convertions.RemoveAt(i);
						this.parent.Repaint();
						return;
					}

					r.y += r.height;

					for (int j = 0; j < this.convertions[i].converters.Count; j++)
					{
						r.x = x;
						r.width = w - 20F;
						GUI.Label(r, this.convertions[i].converters[j].GetType().Name);

						r.x += r.width;
						r.width = 20F;
						if (GUI.Button(r, "X") == true)
						{
							this.convertions[i].converters.RemoveAt(j);
							this.parent.Repaint();

							if (this.convertions[i].converters.Count == 0)
							{
								this.parent.links.RemoveAt(this.parent.links.IndexOf(this.convertions[i]));
								this.convertions.RemoveAt(i);

								if (this.convertions.Count == 0)
									this.parent.Focus();
							}
							return;
						}

						r.x = x;
						r.y += r.height;
						r.width = w;
						r.height = this.convertions[i].converters[j].GetHeight();

						if (this.convertions[i].converters[j].hasGUI == true)
							this.convertions[i].converters[j].OnGUI(r);

						r.y += r.height;
					}
				}
			}

			public override void	OnOpen()
			{
			}

			public override void	OnClose()
			{
			}
		}

		private class SelectionState
		{
			public Side						side = Side.None;
			public List<CompatibilityType>	previewLinks = new List<CompatibilityType>();
			public int						lastHoveringField = -1;
			public int						hoveringField = -1;
			public int						selectedField = -1;
		}

		private class ObjectMatch
		{
			public Transform			transform;
			public List<Component>		components = new List<Component>();
			public List<ObjectMatch>	children = new List<ObjectMatch>();

			public void	OnGUI()
			{
				if (this.transform == null)
				{
					using (ColorContentRestorer.Get(Color.black))
					{
						EditorGUILayout.LabelField("GameObject destroyed");

						++EditorGUI.indentLevel;
						for (int i = 0; i < this.components.Count; i++)
							EditorGUILayout.LabelField("Component destroyed");
					}
				}
				else
				{
					EditorGUILayout.LabelField(this.transform.gameObject.name);

					++EditorGUI.indentLevel;
					for (int i = 0; i < this.components.Count; i++)
						EditorGUILayout.LabelField(this.components[i].ToString());
				}

				for (int i = 0; i < this.children.Count; i++)
					this.children[i].OnGUI();
				--EditorGUI.indentLevel;
			}

			public void	Replace(NGComponentReplacerWindow data)
			{
				for (int i = 0; i < this.components.Count; i++)
				{
					Undo.RecordObject(this.transform.gameObject, "Replacing " + data.target.Name);

					Component	newComponent = Undo.AddComponent(this.transform.gameObject, data.replace);

					if (newComponent == null)
						continue;

					// TODO Add converter data in links
					for (int j = 0; j < data.links.Count; j++)
					{
						IFieldModifier	from = data.targetFields[data.links[j].target];
						IFieldModifier	to = data.replaceFields[data.links[j].replace];

						if (to.Type.IsAssignableFrom(from.Type) == true)
							to.SetValue(newComponent, from.GetValue(this.components[i]));
						else
						{
							for (int k = 0; k < data.converters.Length; k++)
							{
								if (data.converters[k].CanConvert(from.Type, to.Type) == true)
								{
									Debug.Log("Converting " + from.Type + " to " + to.Type);
									to.SetValue(newComponent, data.converters[k].Convert(from.Type, to.Type, from.GetValue(this.components[i])));
								}
							}
						}
					}

					Undo.DestroyObjectImmediate(this.components[i]);
				}
			}
		}

		private class ConvertionSetup
		{
			public int	this[int index]
			{
				get
				{
					if (index == 0)
						return this.target;
					return this.replace;
				}
				set
				{
					if (index == 0)
						this.target = value;
					else
						this.replace = value;
				}
			}
			public int					target;
			public int					replace;
			public List<TypeConverter>	converters = new List<TypeConverter>();
		}

		private enum Side
		{
			None = -1,
			Target,
			Replace
		}

		private enum CompatibilityType
		{
			Impossible,
			Easy,
			ImplicitConvertion,
			ExplicitConvertion
		}

		public const string	Title = "ƝƓ Ҁomponent Ʀeplacer";
		public const string	LastTargetTypePrefKey = "NGComponentReplacer_lastTargetType";
		public const string	LastReplaceTypePrefKey = "NGComponentReplacer_lastReplaceType";
		public const string	DualTypesLinksPrefKeyPrefix = "NGComponentReplacer_links_";
		public const float	FieldBottomPadding = 1F;
		public const float	FieldHeight = 20F;
		public const float	DeleteAreaWidth = 10F;
		public const float	BottomMessageHeight = 40F;

		public static Color	ImpossibleConvertionColor = Color.red;
		public static Color	ExplicitConvertionColor = Color.magenta;
		public static Color	ImplicitConvertionColor = Color.yellow;
		public static Color	SimpleColor = Color.green;

		private int		selectedTab;

		private Type	target;
		private Type	replace;

		private TypeConverter[]	converters;

		private List<ConvertionSetup>	links = new List<ConvertionSetup>();

		private SelectionState	selectionState = new SelectionState();

		private GUIStyle	labelCentered;
		private GUIStyle	labelCenteredOnRight;

		private VerticalScrollbar	targetScrollbar;
		private VerticalScrollbar	replaceScrollbar;

		private List<IFieldModifier>	targetFields = new List<IFieldModifier>();
		private List<IFieldModifier>	replaceFields = new List<IFieldModifier>();

		private float	targetMaxWidth = -1;
		private float	replaceMaxWidth = -1;

		private MessageType	bottomMessageType;
		private string		bottomMessage;

		private List<ObjectMatch>	rootMatches = new List<ObjectMatch>();
		private List<ObjectMatch>	matches = new List<ObjectMatch>();

		private ErrorPopup	errorPopup = new ErrorPopup("An error occured, try to reopen " + NGComponentReplacerWindow.Title + ".");

		//private void	TestScalarConverter(Type to, Type from, object value)
		//{
		//	object	v = new ScalarConverter().Convert(to, from, value);
		//	Debug.Log(from.Name + " \"" + (value ?? "NULL") + "\" => " + v.GetType().Name + " " + v);
		//}

		static	NGComponentReplacerWindow()
		{
			Utility.AddMenuItemPicker(Constants.MenuItemPath + NGComponentReplacerWindow.Title);
		}

#if !NGTOOLS_FREE
		[MenuItem(Constants.MenuItemPath + NGComponentReplacerWindow.Title + "	[BETA]", priority = Constants.MenuItemPriority + 350)]
#endif
		public static void	Open()
		{
			EditorWindow.GetWindow<NGComponentReplacerWindow>(NGComponentReplacerWindow.Title);
		}

		protected virtual void	OnEnable()
		{
			//try
			//{
			//	this.TestScalarConverter(typeof(Char), typeof(string), "3");
			//	this.TestScalarConverter(typeof(Char), typeof(int), 47);
			//	this.TestScalarConverter(typeof(Char), typeof(int), null);
			//	this.TestScalarConverter(typeof(Char), typeof(string), null);
			//	this.TestScalarConverter(typeof(Boolean), typeof(int), 47);
			//	this.TestScalarConverter(typeof(Boolean), typeof(int), 0);
			//	this.TestScalarConverter(typeof(Boolean), typeof(int), -1);
			//	this.TestScalarConverter(typeof(Boolean), typeof(float), 0F);
			//	this.TestScalarConverter(typeof(Boolean), typeof(float), -1F);
			//	this.TestScalarConverter(typeof(Boolean), typeof(string), "true");
			//	this.TestScalarConverter(typeof(Boolean), typeof(string), "false");
			//	this.TestScalarConverter(typeof(Boolean), typeof(string), "");
			//	this.TestScalarConverter(typeof(Boolean), typeof(string), "0");
			//	this.TestScalarConverter(typeof(Boolean), typeof(string), null);
			//	this.TestScalarConverter(typeof(Boolean), typeof(Char), '0');
			//	this.TestScalarConverter(typeof(Boolean), typeof(Char), '\0');
			//	this.TestScalarConverter(typeof(Boolean), typeof(Char), null);
			//	this.TestScalarConverter(typeof(Boolean), typeof(Char), ' ');
			//	this.TestScalarConverter(typeof(Int16), typeof(Char), ' ');
			//	this.TestScalarConverter(typeof(Int16), typeof(Char), null);
			//	this.TestScalarConverter(typeof(Int16), typeof(string), null);
			//	this.TestScalarConverter(typeof(Int16), typeof(string), "");
			//	this.TestScalarConverter(typeof(Int16), typeof(string), "123");
			//	this.TestScalarConverter(typeof(Int16), typeof(string), "-123");
			//	this.TestScalarConverter(typeof(Int16), typeof(string), "123.5");
			//	this.TestScalarConverter(typeof(Int16), typeof(string), "-123.5");
			//	this.TestScalarConverter(typeof(Int16), typeof(string), "112300");
			//	this.TestScalarConverter(typeof(Int16), typeof(string), "-112300");
			//	this.TestScalarConverter(typeof(Int16), typeof(string), "ABC");
			//	this.TestScalarConverter(typeof(Int16), typeof(string), "123ABC");
			//	this.TestScalarConverter(typeof(Int16), typeof(string), "-123ABC");
			//	this.TestScalarConverter(typeof(string), typeof(Boolean), true);
			//	this.TestScalarConverter(typeof(string), typeof(Boolean), false);
			//	this.TestScalarConverter(typeof(string), typeof(Boolean), null);
			//	this.TestScalarConverter(typeof(string), typeof(Single), 0);
			//	this.TestScalarConverter(typeof(string), typeof(Single), 123.4F);
			//	this.TestScalarConverter(typeof(string), typeof(Single), -123.4F);
			//	this.TestScalarConverter(typeof(string), typeof(Single), 1.23456789F);
			//	this.TestScalarConverter(typeof(string), typeof(Single), -1.23456789F);
			//	this.TestScalarConverter(typeof(string), typeof(Single), 12.3456789F);
			//	this.TestScalarConverter(typeof(string), typeof(Single), -12.3456789F);
			//}
			//catch (Exception ex)
			//{
			//	Debug.LogException(ex);
			//}

			this.converters = Utility.CreateInstancesOf<TypeConverter>();

			string	k = NGEditorPrefs.GetString(NGComponentReplacerWindow.LastTargetTypePrefKey);

			if (string.IsNullOrEmpty(k) == false)
				this.SetTarget(Type.GetType(k));

			k = NGEditorPrefs.GetString(NGComponentReplacerWindow.LastReplaceTypePrefKey);

			if (string.IsNullOrEmpty(k) == false)
				this.SetReplace(Type.GetType(k));

			this.RestoreLinks();

			this.targetScrollbar = new VerticalScrollbar(0F, 0F, this.position.height);
			this.targetScrollbar.interceiptEvent = false;
			this.targetScrollbar.hasCustomArea = true;
			this.replaceScrollbar = new VerticalScrollbar(0F, 0F, this.position.height);
			this.replaceScrollbar.interceiptEvent = false;
			this.replaceScrollbar.hasCustomArea = true;

			this.wantsMouseMove = true;
		}

		protected virtual void	OnDisable()
		{
			if (this.target != null)
				NGEditorPrefs.SetString(NGComponentReplacerWindow.LastTargetTypePrefKey, this.target.GetShortAssemblyType());
			else
				NGEditorPrefs.DeleteKey(NGComponentReplacerWindow.LastTargetTypePrefKey);

			if (this.replace != null)
				NGEditorPrefs.SetString(NGComponentReplacerWindow.LastReplaceTypePrefKey, this.replace.GetShortAssemblyType());
			else
				NGEditorPrefs.DeleteKey(NGComponentReplacerWindow.LastReplaceTypePrefKey);

			this.SaveLinks();
		}

		protected virtual void	OnGUI()
		{
			if (this.labelCentered == null)
			{
				this.labelCentered = new GUIStyle(GUI.skin.label);
				this.labelCentered.alignment = TextAnchor.MiddleLeft;
				this.labelCenteredOnRight = new GUIStyle(GUI.skin.label);
				this.labelCenteredOnRight.alignment = TextAnchor.MiddleRight;
			}

			Rect	r = new Rect(0F, 0F, 100F, 20F);
			bool	pressed = false;

			this.errorPopup.OnGUILayout();

			GUILayout.Space(r.height);

			if (GUI.Button(r, "Target Type") == true)
				pressed = true;

			r.x += r.width;
			r.width = this.position.width * .5F - r.width;

			if (this.target != null)
			{
				if (GUI.Button(r, this.target.FullName, GeneralStyles.VerticalCenterLabel) == true)
					pressed = true;
			}
			else if (GUI.Button(r, "None", GeneralStyles.VerticalCenterLabel) == true)
				pressed = true;

			if (pressed == true)
			{
				GenericTypesSelectorWizard	wizard = GenericTypesSelectorWizard.Start("Pick Target", typeof(Component), this.SetTarget, true, true);
				wizard.EnableNullValue = true;
				wizard.SelectedType = this.target;
				wizard.position = new Rect(this.position.x, this.position.y + 50F, wizard.position.width, wizard.position.height);
			}

			pressed = false;

			r.x += r.width;
			r.width = 100F;
			if (GUI.Button(r, "Replace Type") == true)
				pressed = true;

			r.x += r.width;
			r.width = this.position.width * .5F - r.width;

			if (this.replace != null)
			{
				if (GUI.Button(r, this.replace.FullName, GeneralStyles.VerticalCenterLabel) == true)
					pressed = true;
			}
			else if (GUI.Button(r, "None", GeneralStyles.VerticalCenterLabel) == true)
				pressed = true;

			if (pressed == true)
			{
				GenericTypesSelectorWizard	wizard = GenericTypesSelectorWizard.Start("Pick Replace", typeof(Component), this.SetReplace, true, true);
				wizard.EnableNullValue = true;
				wizard.SelectedType = this.replace;
				wizard.position = new Rect(this.position.x + this.position.width * .5F, this.position.y + 50F, wizard.position.width, wizard.position.height);
			}

			if (this.target == this.replace && this.target != null)
				EditorGUILayout.HelpBox("Target and Replace can not point to the same Type.", MessageType.Error);

			EditorGUILayout.BeginHorizontal();
			{
				if (GUILayout.Toggle(this.selectedTab == 0, "Model", GeneralStyles.ToolbarToggle) == true)
					this.selectedTab = 0;

				if (GUILayout.Toggle(this.selectedTab == 1, "Result", GeneralStyles.ToolbarToggle) == true)
					this.selectedTab = 1;
			}
			EditorGUILayout.EndHorizontal();

			if (this.selectedTab == 0)
				this.OnGUIModel();
			else if (this.selectedTab == 1)
				this.OnGUIResult();
		}

		private void	OnGUIResult()
		{
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUI.BeginDisabledGroup(this.rootMatches.Count == 0);
				{
					using (BgColorContentRestorer.Get(GeneralStyles.HighlightActionButton))
					{
						if (GUILayout.Button("Replace All Component") == true &&
							((Event.current.modifiers & Constants.ByPassPromptModifier) != 0 || EditorUtility.DisplayDialog(NGComponentReplacerWindow.Title, "ATTENTION! Replacing a Component will destroy all references to it. You will have to reassign references.", "Yes", "No") == true))
						{
							for (int i = 0; i < this.matches.Count; i++)
								this.matches[i].Replace(this);
						}
					}
				}
				EditorGUI.EndDisabledGroup();
			}
			EditorGUILayout.EndHorizontal();

			Rect	r = new Rect(0F, 62F, this.position.width, 1F);

			EditorGUI.DrawRect(r, Color.gray);

			if (this.rootMatches.Count == 0)
			{
				r = GUILayoutUtility.GetLastRect();
				r.y += r.height;
				r.height = this.position.height - r.y;
				GUI.Label(r, "No result", GeneralStyles.CenterText);
			}
			else
			{

				for (int i = 0; i < this.rootMatches.Count; i++)
					this.rootMatches[i].OnGUI();
			}
		}

		private void	OnGUIModel()
		{
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUI.BeginDisabledGroup(this.target == null);
				{
					using (BgColorContentRestorer.Get(GeneralStyles.HighlightActionButton))
					{
						if (GUILayout.Button("Search Component In Scene") == true)
						{
							this.selectedTab = 1;

							Object[]	objects = Resources.FindObjectsOfTypeAll(this.target);

							this.matches.Clear();
							this.rootMatches.Clear();

							for (int i = 0; i < objects.Length; i++)
							{
								if ((objects[i].hideFlags & HideFlags.DontSave) == 0)
								{
									Transform	t = (objects[i] as Component).transform;
									int			j = 0;

									for (; j < this.matches.Count; j++)
									{
										if (this.matches[j].transform == t)
										{
											matches[j].components.Add(objects[i] as Component);
											break;
										}
									}

									if (j >= this.matches.Count)
									{
										ObjectMatch	match = new ObjectMatch() { transform = t };
										match.components.Add(objects[i] as Component);
										this.matches.Add(match);

										t = t.parent;

										if (t == null)
											this.rootMatches.Add(match);
										else
										{
											while (t != null)
											{
												j = 0;

												for (; j < this.matches.Count; j++)
												{
													if (this.matches[j].transform == t)
													{
														this.matches[j].children.Add(match);
														break;
													}
												}

												if (j >= this.matches.Count)
												{
													ObjectMatch	parentMatch = new ObjectMatch() { transform = t };

													parentMatch.children.Add(match);
													this.matches.Add(parentMatch);

													if (t.parent == null)
														this.rootMatches.Add(parentMatch);
												}

												t = t.parent;
											}
										}
									}
								}
							}
						}
					}
				}
				EditorGUI.EndDisabledGroup();
			}
			EditorGUILayout.EndHorizontal();

			Rect	r = GUILayoutUtility.GetLastRect();

			r = new Rect(0F, r.y + r.height, this.position.width, 1F);

			EditorGUI.DrawRect(r, Color.gray);

			r.y += 3F;
			r.height = this.position.height - r.y; // - Top bar.

			if (this.target == null)
			{
				r.x = 0F;
				r.width = this.position.width * .5F;
				GUI.Label(r, "Pick a Component in Target", GeneralStyles.CenterText);
			}

			if (this.replace == null)
			{
				r.width = this.position.width * .5F;
				r.x = r.width;
				GUI.Label(r, "Pick a Component in Replace", GeneralStyles.CenterText);

				if (this.target == null)
					return;
			}

			r.x = 0F;
			r.width = this.position.width;
			r.height -= NGComponentReplacerWindow.BottomMessageHeight; // - Bottom bar.

			if (this.targetMaxWidth == -1F)
				this.targetMaxWidth = this.GetMaxWidth(this.targetFields);
			if (this.replaceMaxWidth == -1F)
				this.replaceMaxWidth = this.GetMaxWidth(this.replaceFields);

			GUI.BeginGroup(r);
			{
				this.targetScrollbar.realHeight = this.targetFields.Count * (NGComponentReplacerWindow.FieldHeight);
				this.targetScrollbar.SetSize(r.height);
				if (Event.current.type == EventType.Repaint)
				{
					// Remove position due to offset from GUI.BeginGroup.
					r.x = 15F;
					r.y = 0F;
					r.width = this.targetMaxWidth;
					this.targetScrollbar.allowedMouseArea = r;
				}
				this.targetScrollbar.OnGUI();

				this.replaceScrollbar.realHeight = this.replaceFields.Count * (NGComponentReplacerWindow.FieldHeight);
				this.replaceScrollbar.SetPosition(this.position.width - 15F, 0F);
				this.replaceScrollbar.SetSize(r.height);
				if (Event.current.type == EventType.Repaint)
				{
					// Remove position due to offset from GUI.BeginGroup.
					r.width = this.replaceMaxWidth;
					r.x = this.position.width - 15F - r.width;
					r.y = 0F;
					this.replaceScrollbar.allowedMouseArea = r;
				}
				this.replaceScrollbar.OnGUI();

				r.x = 15F;
				r.y = -this.targetScrollbar.offsetY;
				r.width = this.targetMaxWidth;
				r.height = NGComponentReplacerWindow.FieldHeight;

				if (Event.current.type == EventType.MouseMove)
				{
					this.selectionState.lastHoveringField = this.selectionState.hoveringField;
					this.selectionState.hoveringField = -1;
				}

				for (int i = 0; i < this.targetFields.Count; i++)
				{
					if (r.y + r.height <= 0)
					{
						r.y += r.height;
						continue;
					}

					this.HandleFieldEvent(Side.Target, Side.Replace, this.targetFields, this.replaceFields, this.targetScrollbar, this.replaceScrollbar, r, i);

					Utility.content.text = "[" + this.targetFields[i].Type.Name + "] " + this.targetFields[i].Name;
					GUI.Label(r, Utility.content, this.labelCentered);

					r.y += r.height;

					if (r.y > this.targetScrollbar.maxHeight)
						break;
				}

				r.x = this.position.width - this.replaceMaxWidth - 15F;
				r.y = -this.replaceScrollbar.offsetY;
				r.width = this.replaceMaxWidth;

				for (int i = 0; i < this.replaceFields.Count; i++)
				{
					if (r.y + r.height <= 0)
					{
						r.y += r.height;
						continue;
					}

					this.HandleFieldEvent(Side.Replace, Side.Target, this.replaceFields, this.targetFields, this.replaceScrollbar, this.targetScrollbar, r, i);

					Utility.content.text = this.replaceFields[i].Name + " [" + this.replaceFields[i].Type.Name + "]";
					GUI.Label(r, Utility.content, this.labelCenteredOnRight);

					r.y += r.height;

					if (r.y > this.replaceScrollbar.maxHeight)
						break;
				}

				for (int i = 0; i < this.links.Count; i++)
				{
					Vector2	a = new Vector2(this.targetMaxWidth + 15F + NGComponentReplacerWindow.DeleteAreaWidth,
											NGComponentReplacerWindow.FieldHeight * .5F - this.targetScrollbar.offsetY + this.links[i].target * NGComponentReplacerWindow.FieldHeight);
					Vector2	b = new Vector2(this.position.width - this.replaceMaxWidth - 15F - NGComponentReplacerWindow.DeleteAreaWidth,
											NGComponentReplacerWindow.FieldHeight * .5F - this.replaceScrollbar.offsetY + this.links[i].replace * NGComponentReplacerWindow.FieldHeight);

					CompatibilityType	c = this.GetTypesCompatibility(this.targetFields[this.links[i].target].Type, this.replaceFields[this.links[i].replace].Type);

					Utility.DrawLine(a, b, this.GetTypeCompatibilityColor(c));

					r.x = a.x - NGComponentReplacerWindow.DeleteAreaWidth;
					r.y = a.y - NGComponentReplacerWindow.FieldHeight * .5F;
					r.width = NGComponentReplacerWindow.DeleteAreaWidth;
					r.height = NGComponentReplacerWindow.FieldHeight - NGComponentReplacerWindow.FieldBottomPadding;

					EditorGUI.DrawRect(r, Color.white);
					if (Event.current.type == EventType.MouseDown)
					{
						if (r.Contains(Event.current.mousePosition) == true)
						{
							List<ConvertionSetup>	convertions = new List<ConvertionSetup>();
							int						index = this.links[i].target;

							for (int j = 0; j < this.links.Count; j++)
							{
								if (this.links[j].target == index)
									convertions.Add(this.links[j]);
							}

							PopupWindow.Show(r, new PopupExample(this, convertions));

							Event.current.Use();
						}
					}

					r.x = b.x;
					r.y = b.y - NGComponentReplacerWindow.FieldHeight * .5F;

					EditorGUI.DrawRect(r, Color.white);
					if (Event.current.type == EventType.MouseDown)
					{
						if (r.Contains(Event.current.mousePosition) == true)
						{
							List<ConvertionSetup>	convertions = new List<ConvertionSetup>();
							int						index = this.links[i].replace;

							for (int j = 0; j < this.links.Count; j++)
							{
								if (this.links[j].replace == index)
									convertions.Add(this.links[j]);
							}

							PopupWindow.Show(r, new PopupExample(this, convertions));

							Event.current.Use();
						}
					}
				}
			}
			GUI.EndGroup();

			if (this.selectionState.selectedField == -1)
			{
				this.bottomMessageType = MessageType.Info;
				this.bottomMessage = "Select a field or property.";
			}
			else
			{
				this.bottomMessageType = MessageType.Info;
				this.bottomMessage = "Select a field or property on the other side to link them.";

				if (this.selectionState.hoveringField != -1)
				{
					if (this.selectionState.previewLinks[this.selectionState.hoveringField] == CompatibilityType.Impossible)
					{
						this.bottomMessageType = MessageType.Error;
						this.bottomMessage = "Impossible.";
					}
					else if (this.selectionState.previewLinks[this.selectionState.hoveringField] == CompatibilityType.Easy)
					{
						this.bottomMessageType = MessageType.Info;
						this.bottomMessage = "Possible.";
					}
					else if (this.selectionState.previewLinks[this.selectionState.hoveringField] == CompatibilityType.ImplicitConvertion)
					{
						this.bottomMessageType = MessageType.Warning;
						this.bottomMessage = "Possible, but the converted value might be unexpected. (Size overflow, boolean True or False only, string to integer without digits, etc...)";
					}
					else if (this.selectionState.previewLinks[this.selectionState.hoveringField] == CompatibilityType.ExplicitConvertion)
					{
						this.bottomMessageType = MessageType.Warning;
						this.bottomMessage = "Possible, but you need to manually setup the convertion before proceeding.";
					}
				}
			}

			EditorGUI.HelpBox(new Rect(0F, this.position.height - NGComponentReplacerWindow.BottomMessageHeight, this.position.width, NGComponentReplacerWindow.BottomMessageHeight), this.bottomMessage, this.bottomMessageType);

			if (this.selectionState.selectedField == -1 && this.selectionState.hoveringField == -1 && this.selectionState.lastHoveringField != -1)
			{
				this.selectionState.previewLinks.Clear();
				this.targetScrollbar.ClearInterests();
				this.replaceScrollbar.ClearInterests();
				this.Repaint();
			}

			if (Event.current.type == EventType.MouseDown)
			{
				this.selectionState.side = Side.None;
				this.selectionState.hoveringField = -1;
				this.selectionState.selectedField = -1;
				this.selectionState.previewLinks.Clear();
				this.Repaint();
			}
		}

		private void	HandleFieldEvent(Side side, Side otherSide, List<IFieldModifier> fields, List<IFieldModifier> otherFields, VerticalScrollbar scrollbar, VerticalScrollbar otherScrollbar, Rect r, int i)
		{
			if (Event.current.type == EventType.Repaint)
				this.DrawBackground(r, i, side);
			else if (this.target == this.replace)
				return;
			else if (Event.current.type == EventType.MouseMove)
			{
				if (this.selectionState.selectedField == -1)
				{
					if (r.Contains(Event.current.mousePosition) == true)
					{
						this.selectionState.side = side;
						this.selectionState.hoveringField = i;
						if (this.selectionState.lastHoveringField != i)
						{
							this.UpdatePreviewLink(fields[i].Type, otherFields, side == Side.Replace, otherScrollbar);
							this.Repaint();
						}
					}
				}
				else if (this.selectionState.side == otherSide && r.Contains(Event.current.mousePosition) == true)
				{
					this.selectionState.hoveringField = i;
					this.Repaint();
				}
			}
			else if (Event.current.type == EventType.MouseDown)
			{
				if (r.Contains(Event.current.mousePosition) == true)
				{
					if (this.selectionState.side == otherSide &&
						this.selectionState.selectedField != -1)
					{
						if (this.selectionState.previewLinks[i] != CompatibilityType.Impossible)
						{
							int	j = 0;

							for (; j < this.links.Count; j++)
							{
								if (this.links[j][(int)side] == i && this.links[j][(int)otherSide] == this.selectionState.selectedField)
									break;
							}

							if (j >= this.links.Count)
							{
								ConvertionSetup	pair = new ConvertionSetup();
								pair[(int)side] = i;
								pair[(int)otherSide] = this.selectionState.selectedField;
								pair.converters.Add(this.AllocateTypeConverter(this.targetFields[pair.target].Type, this.replaceFields[pair.replace].Type));
								this.links.Add(pair);
							}

							this.selectionState.side = side;
							this.selectionState.hoveringField = i;
							this.selectionState.selectedField = -1;
							scrollbar.ClearInterests();
							this.UpdatePreviewLink(fields[i].Type, otherFields, side == Side.Replace, otherScrollbar);
						}
					}
					else
					{
						if (this.selectionState.selectedField == i)
						{
							this.selectionState.selectedField = -1;
							this.selectionState.hoveringField = i;
							scrollbar.ClearInterests();
						}
						else
						{
							this.selectionState.side = side;
							this.selectionState.hoveringField = -1;
							this.selectionState.selectedField = i;
							scrollbar.ClearInterests();
							scrollbar.AddInterest(NGComponentReplacerWindow.FieldHeight * .5F + i * (NGComponentReplacerWindow.FieldHeight + FieldBottomPadding), NGComponentReplacerWindow.SimpleColor);
							this.UpdatePreviewLink(fields[i].Type, otherFields, side == Side.Replace, otherScrollbar);
						}
					}

					Event.current.Use();
				}
			}
		}

		private void	DrawBackground(Rect r, int i, Side side)
		{
			r.height -= NGComponentReplacerWindow.FieldBottomPadding;

			if (this.selectionState.side != side)
			{
				if (this.selectionState.previewLinks.Count > 0)
					EditorGUI.DrawRect(r, this.GetTypeCompatibilityColor(this.selectionState.previewLinks[i]));
				else
					EditorGUI.DrawRect(r, Color.cyan);
			}
			else
			{
				if (this.selectionState.selectedField == i)
					EditorGUI.DrawRect(r, Color.green);
				else if (this.selectionState.selectedField == -1 && this.selectionState.side == side && this.selectionState.hoveringField == i)
					EditorGUI.DrawRect(r, Color.white);
				else
					EditorGUI.DrawRect(r, Color.cyan);
			}
		}

		private void	SetTarget(Type type)
		{
			if (type == typeof(Transform)
#if UNITY_5
				|| type == typeof(RectTransform)
#endif
				)
			{
				EditorUtility.DisplayDialog(NGComponentReplacerWindow.Title, "Transform " +
#if UNITY_5
					"and RectTransform " +
#endif
					"can not be choosen.", "OK");
				return;
			}

			this.SaveLinks();

			this.target = type;

			this.targetFields.Clear();
			if (this.target != null)
				this.GetFieldsProperties(this.target, this.targetFields, false);

			this.targetMaxWidth = -1;

			this.selectionState.side = Side.None;
			this.selectionState.hoveringField = -1;
			this.selectionState.selectedField = -1;
			this.selectionState.previewLinks.Clear();

			this.RestoreLinks();

			this.Repaint();
		}

		private void	SetReplace(Type type)
		{
			if (type == typeof(Transform)
#if UNITY_5
				|| type == typeof(RectTransform)
#endif
				)
			{
				EditorUtility.DisplayDialog(NGComponentReplacerWindow.Title, "Transform " +
#if UNITY_5
					"and RectTransform " +
#endif
					"can not be choosen.", "OK");
				return;
			}

			this.SaveLinks();

			this.replace = type;

			this.replaceFields.Clear();
			if (this.replace != null)
				this.GetFieldsProperties(this.replace, this.replaceFields, true);

			this.replaceMaxWidth = -1;

			this.selectionState.side = Side.None;
			this.selectionState.hoveringField = -1;
			this.selectionState.selectedField = -1;
			this.selectionState.previewLinks.Clear();

			this.RestoreLinks();

			this.Repaint();
		}

		private void	GetFieldsProperties(Type type, List<IFieldModifier> list, bool receiver)
		{
			FieldInfo[]	fields = type.GetFields();

			for (int i = 0; i < fields.Length; i++)
			{
				if (fields[i].IsLiteral == true || fields[i].IsStatic == true)
					continue;

				list.Add(new FieldModifier(fields[i]));
			}

			PropertyInfo[]	properties = type.GetProperties();

			for (int i = 0; i < properties.Length; i++)
			{
				if (properties[i].GetIndexParameters().Length != 0)
					continue;

				if (receiver == true)
				{
					if (properties[i].GetSetMethod() == null)
						continue;
				}
				else if (properties[i].GetGetMethod() == null)
					continue;

				list.Add(new PropertyModifier(properties[i]));
			}
		}

		private float	GetMaxWidth(List<IFieldModifier> list)
		{
			float	maxWidth = 0F;

			for (int i = 0; i < list.Count; i++)
			{
				Utility.content.text = "[" + list[i].Type.Name + "] " + list[i].Name;

				float	w = GUI.skin.label.CalcSize(Utility.content).x;
				if (maxWidth < w)
					maxWidth = w;
			}

			return maxWidth;
		}

		private void	UpdatePreviewLink(Type type, List<IFieldModifier> fields, bool receiver, VerticalScrollbar scrollbar)
		{
			this.selectionState.previewLinks.Clear();
			scrollbar.ClearInterests();

			Type	from = type;
			Type	to = type;

			for (int i = 0; i < fields.Count; i++)
			{
				if (receiver == false)
					to = fields[i].Type;
				else
					from = fields[i].Type;

				CompatibilityType	c = this.GetTypesCompatibility(from, to);
				// TODO Gérer les compatibilités ascendantes, lister les Converters et choisir.
				this.selectionState.previewLinks.Add(c);

				scrollbar.AddInterest(NGComponentReplacerWindow.FieldHeight * .5F + i * NGComponentReplacerWindow.FieldHeight, this.GetTypeCompatibilityColor(c));
			}
		}

		private TypeConverter	AllocateTypeConverter(Type from, Type to)
		{
			for (int i = 0; i < this.converters.Length; i++)
			{
				if (this.converters[i].CanConvert(from, to) == true)
					return Activator.CreateInstance(this.converters[i].GetType()) as TypeConverter;
			}

			return null;
		}

		private Color	GetTypeCompatibilityColor(CompatibilityType c)
		{
			if (c == CompatibilityType.Impossible)
				return NGComponentReplacerWindow.ImpossibleConvertionColor;
			else if (c == CompatibilityType.Easy)
				return NGComponentReplacerWindow.SimpleColor;
			else if (c == CompatibilityType.ExplicitConvertion)
				return NGComponentReplacerWindow.ExplicitConvertionColor;
			else if (c == CompatibilityType.ImplicitConvertion)
				return NGComponentReplacerWindow.ImplicitConvertionColor;

			throw new Exception("CompatibilityType \"" + c + "\" does not exist.");
		}

		private CompatibilityType	GetTypesCompatibility(Type from, Type to)
		{
			if (to.IsAssignableFrom(from) == true)
				return CompatibilityType.Easy;

			for (int j = 0; j < this.converters.Length; j++)
			{
				if (this.converters[j].CanConvert(from, to) == true)
				{
					if (this.converters[j].hasGUI == true)
						return CompatibilityType.ExplicitConvertion;
					else
						return CompatibilityType.ImplicitConvertion;
				}
			}

			return CompatibilityType.Impossible;
		}

		private void	RestoreLinks()
		{
			this.links.Clear();

			if (this.target == null || this.replace == null || this.target == this.replace)
				return;

			string	rawLinks = NGEditorPrefs.GetString(NGComponentReplacerWindow.DualTypesLinksPrefKeyPrefix + this.target.FullName + "." + this.replace.FullName);

			if (string.IsNullOrEmpty(rawLinks) == true)
			{
				// Automatically create links for fields sharing their name and compatible type.
				for (int i = 0; i < this.targetFields.Count; i++)
				{
					for (int j = 0; j < this.replaceFields.Count; j++)
					{
						if (this.targetFields[i].Name == this.replaceFields[j].Name)
						{
							CompatibilityType	c = this.GetTypesCompatibility(this.targetFields[i].Type, this.replaceFields[j].Type);

							if (c == CompatibilityType.Easy)
								this.links.Add(new ConvertionSetup() { target = i, replace = j });
							else if (c == CompatibilityType.ImplicitConvertion)
							{
								ConvertionSetup	convertion = new ConvertionSetup() { target = i, replace = j };
								convertion.converters.Add(this.AllocateTypeConverter(this.targetFields[i].Type, this.replaceFields[j].Type));
								this.links.Add(convertion);
							}
						}
					}
				}
				return;
			}

			ByteBuffer	buffer = Utility.GetBBuffer(Convert.FromBase64String(rawLinks));

			if (buffer.Length < 4)
				return;

			int	linksCount = buffer.ReadInt32();

			for (int i = 0; i < linksCount; ++i)
			{
				int	convertionLength = buffer.ReadInt32();
				int	lastBufferPosition = buffer.Position;

				try
				{
					string	targetFieldName = buffer.ReadUnicodeString();
					string	replaceFieldName = buffer.ReadUnicodeString();

					int	targetIndex = -1;
					int	replaceIndex = -1;

					for (int j = 0; j + 1 < this.targetFields.Count; j++)
					{
						if (this.targetFields[j].Name == targetFieldName)
						{
							targetIndex = j;
							break;
						}
					}

					if (targetIndex == -1)
						continue;

					for (int j = 0; j < this.replaceFields.Count; j++)
					{
						if (this.replaceFields[j].Name == replaceFieldName)
						{
							replaceIndex = j;
							break;
						}
					}

					if (targetIndex != -1 && replaceIndex != -1)
					{
						ConvertionSetup	convertion = new ConvertionSetup() { target = targetIndex, replace = replaceIndex };
						int				convertersCount = buffer.ReadInt32();

						for (int j = 0; j < convertersCount; j++)
						{
							Type	type = Type.GetType(buffer.ReadUnicodeString());

							for (int k = 0; k < this.converters.Length; k++)
							{
								if (this.converters[k].GetType() == type)
								{
									TypeConverter	converter = Activator.CreateInstance(type) as TypeConverter;
									int				converterLength = buffer.ReadInt32();
									int				lastBufferPosition2 = buffer.Position;

									try
									{
										converter.Load(buffer);
										convertion.converters.Add(converter);
									}
									finally
									{
										buffer.Position = lastBufferPosition2 + converterLength;
									}

									break;
								}
							}
						}

						this.links.Add(convertion);
					}
				}
				finally
				{
					buffer.Position = lastBufferPosition + convertionLength;
				}
			}
		}

		private void	SaveLinks()
		{
			if (this.target != null && this.replace != null)
			{
				if (this.links.Count > 0)
				{
					ByteBuffer	buffer1 = Utility.GetBBuffer();

					buffer1.Append(this.links.Count);
					for (int i = 0; i < this.links.Count; i++)
					{
						ByteBuffer	buffer2 = Utility.GetBBuffer();

						buffer2.AppendUnicodeString(this.targetFields[this.links[i][0]].Name);
						buffer2.AppendUnicodeString(this.replaceFields[this.links[i][1]].Name);
						buffer2.Append(this.links[i].converters.Count);

						for (int j = 0; j < this.links[i].converters.Count; j++)
						{
							buffer2.AppendUnicodeString(this.links[i].converters[j].GetType().GetShortAssemblyType());

							ByteBuffer	buffer3 = Utility.GetBBuffer();
							this.links[i].converters[j].Save(buffer3);
							buffer2.Append(buffer3.Length);
							buffer2.Append(buffer3);
							Utility.RestoreBBuffer(buffer3);
						}

						buffer1.Append(buffer2.Length);
						buffer1.Append(buffer2);
						Utility.RestoreBBuffer(buffer2);
					}

					NGEditorPrefs.SetString(NGComponentReplacerWindow.DualTypesLinksPrefKeyPrefix + this.target.FullName + "." + this.replace.FullName, Convert.ToBase64String(Utility.ReturnBBuffer(buffer1)));
				}
				else
					NGEditorPrefs.DeleteKey(NGComponentReplacerWindow.DualTypesLinksPrefKeyPrefix + this.target.FullName + "." + this.replace.FullName);
			}
		}

		void	IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
		{
			Utility.AddNGMenuItems(menu, this, NGComponentReplacerWindow.Title, Constants.WikiBaseURL + "#markdown-header-116-ng-component-replacer");
		}
	}
}