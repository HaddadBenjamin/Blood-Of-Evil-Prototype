using NGTools;
using System;
using System.Reflection;
using UnityEditor;

namespace NGToolsEditor
{
	using UnityEngine;

	[CustomPropertyDrawer(typeof(ShowIfAttribute))]
	internal sealed class ShowIfDrawer : PropertyDrawer
	{
		private ConditionalRenderer	renderer;

		public override float	GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (this.renderer == null)
				this.renderer = new ConditionalRenderer("ShowIf", this, base.GetPropertyHeight, true);

			return this.renderer.GetPropertyHeight(property, label);
		}

		public override void	OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			this.renderer.OnGUI(position, property, label);
		}
	}

	internal sealed class ConditionalRenderer
	{
		private string										name;
		private Func<SerializedProperty, GUIContent, float>	getPropertyHeight;
		private PropertyDrawer								drawer;
		private bool										normalBooleanValue;

		private string			errorAttribute = null;
		private FieldInfo		conditionField;
		private ShowIfAttribute	attr;

		private object		lastValue;
		private string		lastValueStringified;
		private string[]	targetValueStringified;
		private Decimal[]	targetValueDecimaled;

		private bool	display;

		private Func<SerializedProperty, GUIContent, float> PropertyHeight;

		public	ConditionalRenderer(string name, PropertyDrawer drawer, Func<SerializedProperty, GUIContent, float> getPropertyHeight, bool normalBooleanValue)
		{
			this.name = name;
			this.drawer = drawer;
			this.getPropertyHeight = getPropertyHeight;
			this.normalBooleanValue = normalBooleanValue;
		}

		public float	GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (this.attr == null)
				this.InitializeDrawer(property);

			if (this.errorAttribute != null)
				return EditorGUIUtility.singleLineHeight;
			if (this.conditionField == null)
				return this.getPropertyHeight(property, label);

			return this.PropertyHeight(property, label);
		}

		public void	OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (this.errorAttribute != null)
			{
				using (ColorContentRestorer.Get(Color.black))
				{
					EditorGUI.LabelField(position, label.text, this.errorAttribute);
				}
			}
			else if (this.conditionField == null || this.display == this.normalBooleanValue)
				EditorGUI.PropertyField(position, property, label);
		}

		private void	InitializeDrawer(SerializedProperty property)
		{
			this.attr = (this.drawer.attribute as ShowIfAttribute);

			this.conditionField = this.drawer.fieldInfo.DeclaringType.GetField(this.attr.fieldName);
			if (this.conditionField == null)
				this.errorAttribute = this.name + " is requiring field \"" + this.attr.fieldName + "\".";
			else if (this.attr.@operator != Ops.None)
			{
				if (this.attr.values[0] == null)
				{
					this.targetValueStringified = new string[] { string.Empty };
					this.PropertyHeight = this.GetHeightAllOpsString;

					if (this.attr.@operator != Ops.Equals &&
						this.attr.@operator != Ops.Diff)
					{
						this.errorAttribute = this.name + " is requiring a null value whereas its operator is \"" + this.attr.@operator + "\" which is impossible.";
					}
				}
				else if (this.attr.values[0] is Boolean)
				{
					this.targetValueStringified = new string[] { this.attr.values[0].ToString() };
					this.PropertyHeight = this.GetHeightAllOpsString;

					if (this.attr.@operator != Ops.Equals &&
						this.attr.@operator != Ops.Diff)
					{
						this.errorAttribute = this.name + " is requiring a boolean whereas its operator is \"" + this.attr.@operator + "\" which is impossible.";
					}
				}
				else if (this.attr.values[0] is Int32 ||
						 this.attr.values[0] is Single ||
						 this.attr.values[0] is Enum ||
						 this.attr.values[0] is Double ||
						 this.attr.values[0] is Decimal ||
						 this.attr.values[0] is Int16 ||
						 this.attr.values[0] is Int64 ||
						 this.attr.values[0] is UInt16 ||
						 this.attr.values[0] is UInt32 ||
						 this.attr.values[0] is UInt64 ||
						 this.attr.values[0] is Byte ||
						 this.attr.values[0] is SByte)
				{
					this.targetValueDecimaled = new Decimal[] { Convert.ToDecimal(this.attr.values[0]) };
					this.PropertyHeight = this.GetHeightAllOpsScalar;
				}
				else
				{
					this.targetValueStringified = new string[] { this.attr.values[0].ToString() };
					this.PropertyHeight = this.GetHeightAllOpsString;
				}
			}
			else if (this.attr.multiOperator != MultiOps.None)
			{
				if (this.CheckUseOfNonScalarValue() == true)
				{
					this.targetValueStringified = new string[this.attr.values.Length];
					for (int i = 0; i < this.attr.values.Length; i++)
					{
						if (this.attr.values[i] != null)
							this.targetValueStringified[i] = this.attr.values[i].ToString();
						else
							this.targetValueStringified[i] = string.Empty;
					}

					this.PropertyHeight = this.GetHeightMultiOpsString;
				}
				else
				{
					this.targetValueDecimaled = new Decimal[this.attr.values.Length];
					for (int i = 0; i < this.attr.values.Length; i++)
						this.targetValueDecimaled[i] = Convert.ToDecimal(this.attr.values[i]);

					this.PropertyHeight = this.GetHeightMultiOpsScalar;
				}
			}

			// Force the next update.
			object	newValue = this.conditionField.GetValue(property.serializedObject.targetObject);

			if (this.lastValue == newValue)
				this.lastValue = true;

		}

		private bool	CheckUseOfNonScalarValue()
		{
			for (int i = 0; i < this.attr.values.Length; i++)
			{
				if (this.attr.values[i] == null ||
					this.attr.values[i] is String ||
					this.attr.values[i] is Boolean)
				{
					return true;
				}
			}

			return false;
		}

		private float	GetHeightAllOpsString(SerializedProperty property, GUIContent label)
		{
			object	newValue = this.conditionField.GetValue(property.serializedObject.targetObject);

			if (this.lastValue != newValue)
			{
				this.lastValue = newValue;
				if (this.lastValue != null &&
					// Unity Object is not referenced as real null, it is fake. Don't trust them.
					(typeof(Object).IsAssignableFrom(this.lastValue.GetType()) == false ||
					 ((this.lastValue as Object).ToString() != "null")))
				{
					this.lastValueStringified = this.lastValue.ToString();
				}
				else
					this.lastValueStringified = string.Empty;

				this.display = !this.normalBooleanValue;

				if (this.attr.@operator == Ops.Equals)
				{
					if (this.lastValueStringified.Equals(this.targetValueStringified[0]) == true)
						this.display = this.normalBooleanValue;
				}
				else if (this.attr.@operator == Ops.Diff)
				{
					if (this.lastValueStringified.Equals(this.targetValueStringified[0]) == false)
						this.display = this.normalBooleanValue;
				}
				else if (this.attr.@operator == Ops.Sup)
				{
					if (this.lastValueStringified.CompareTo(this.targetValueStringified[0]) > 0)
						this.display = this.normalBooleanValue;
				}
				else if (this.attr.@operator == Ops.Inf)
				{
					if (this.lastValueStringified.CompareTo(this.targetValueStringified[0]) < 0)
						this.display = this.normalBooleanValue;
				}
				else if (this.attr.@operator == Ops.SupEquals)
				{
					if (this.lastValueStringified.CompareTo(this.targetValueStringified[0]) >= 0)
						this.display = this.normalBooleanValue;
				}
				else if (this.attr.@operator == Ops.InfEquals)
				{
					if (this.lastValueStringified.CompareTo(this.targetValueStringified[0]) <= 0)
						this.display = this.normalBooleanValue;
				}
			}

			if (this.display == this.normalBooleanValue)
				return this.getPropertyHeight(property, label);
			return 0F;
		}

		private float	GetHeightAllOpsScalar(SerializedProperty property, GUIContent label)
		{
			object	newValue = this.conditionField.GetValue(property.serializedObject.targetObject);

			if (this.lastValue != newValue)
			{
				this.lastValue = newValue;

				try
				{
					Decimal	value = Convert.ToDecimal(newValue);

					this.display = !this.normalBooleanValue;

					if (this.attr.@operator == Ops.Equals)
					{
						if (value == this.targetValueDecimaled[0])
							this.display = this.normalBooleanValue;
					}
					else if (this.attr.@operator == Ops.Diff)
					{
						if (value != this.targetValueDecimaled[0])
							this.display = this.normalBooleanValue;
					}
					else if (this.attr.@operator == Ops.Sup)
					{
						if (value > this.targetValueDecimaled[0])
							this.display = this.normalBooleanValue;
					}
					else if (this.attr.@operator == Ops.Inf)
					{
						if (value < this.targetValueDecimaled[0])
							this.display = this.normalBooleanValue;
					}
					else if (this.attr.@operator == Ops.SupEquals)
					{
						if (value >= this.targetValueDecimaled[0])
							this.display = this.normalBooleanValue;
					}
					else if (this.attr.@operator == Ops.InfEquals)
					{
						if (value <= this.targetValueDecimaled[0])
							this.display = this.normalBooleanValue;
					}
				}
				catch
				{
				}
			}

			if (this.display == this.normalBooleanValue)
				return this.getPropertyHeight(property, label);
			return 0F;
		}

		private float	GetHeightMultiOpsString(SerializedProperty property, GUIContent label)
		{
			object	newValue = this.conditionField.GetValue(property.serializedObject.targetObject);

			if (this.lastValue != newValue)
			{
				this.lastValue = newValue;
				if (this.lastValue != null &&
					// Unity Object is not referenced as real null, it is fake. Don't trust them.
					(typeof(Object).IsAssignableFrom(this.lastValue.GetType()) == false ||
					 ((this.lastValue as Object).ToString() != "null")))
				{
					this.lastValueStringified = this.lastValue.ToString();
				}
				else
					this.lastValueStringified = string.Empty;

				if (this.attr.multiOperator == MultiOps.Equals)
				{
					this.display = !this.normalBooleanValue;

					for (int i = 0; i < this.targetValueStringified.Length; i++)
					{
						if (this.lastValueStringified.Equals(this.targetValueStringified[i]) == true)
						{
							this.display = this.normalBooleanValue;
							break;
						}
					}
				}
				else if (this.attr.multiOperator == MultiOps.Diff)
				{
					int	i = 0;

					this.display = this.normalBooleanValue;

					for (; i < this.targetValueStringified.Length; i++)
					{
						if (this.lastValueStringified.Equals(this.targetValueStringified[i]) == true)
						{
							this.display = !this.normalBooleanValue;
							break;
						}
					}
				}
			}

			if (this.display == this.normalBooleanValue)
				return this.getPropertyHeight(property, label);
			return 0F;
		}

		private float	GetHeightMultiOpsScalar(SerializedProperty property, GUIContent label)
		{
			object	newValue = this.conditionField.GetValue(property.serializedObject.targetObject);

			if (this.lastValue != newValue)
			{
				this.lastValue = newValue;

				try
				{
					Decimal	value = Convert.ToDecimal(newValue);

					if (this.attr.multiOperator == MultiOps.Equals)
					{
						this.display = !this.normalBooleanValue;

						for (int i = 0; i < this.targetValueDecimaled.Length; i++)
						{
							if (value == this.targetValueDecimaled[i])
							{
								this.display = this.normalBooleanValue;
								break;
							}
						}
					}
					else if (this.attr.multiOperator == MultiOps.Diff)
					{
						int	i = 0;

						this.display = this.normalBooleanValue;

						for (; i < this.targetValueDecimaled.Length; i++)
						{
							if (value == this.targetValueDecimaled[i])
							{
								this.display = !this.normalBooleanValue;
								break;
							}
						}
					}
				}
				catch
				{
				}
			}

			if (this.display == this.normalBooleanValue)
				return this.getPropertyHeight(property, label);
			return 0F;
		}
	}
}