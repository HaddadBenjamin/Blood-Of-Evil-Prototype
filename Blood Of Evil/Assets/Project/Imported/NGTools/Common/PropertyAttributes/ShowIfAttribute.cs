using UnityEngine;

namespace NGTools
{
	public enum MultiOps
	{
		None = -1,
		/// <summary>Checks if the field's value equals one of the requirements.</summary>
		Equals,
		/// <summary>Checks if the field's value differs from all the requirements.</summary>
		Diff,
	}

	public enum Ops
	{
		None = -1,
		/// <summary>Checks if the field's value is equal to the requirement.</summary>
		Equals,
		/// <summary>Checks if the field's value is different from the requirement.</summary>
		Diff,
		/// <summary>Checks if the field's value is superior than the requirement.</summary>
		Sup,
		/// <summary>Checks if the field's value is less than the requirement.</summary>
		Inf,
		/// <summary>Checks if the field's value is greater or equal to the requirement.</summary>
		SupEquals,
		/// <summary>Checks if the field's value is less than or equal to the requirement.</summary>
		InfEquals,
	}

	public class ShowIfAttribute : PropertyAttribute
	{
		public readonly string		fieldName;
		public readonly Ops			@operator;
		public readonly MultiOps	multiOperator;
		public readonly object[]	values;

		public	ShowIfAttribute(string fieldName, Ops @operator, object value)
		{
			this.fieldName = fieldName;
			this.@operator = @operator;
			this.multiOperator = MultiOps.None;
			this.values = new object[] { value };
		}

		public	ShowIfAttribute(string fieldName, MultiOps multiOperator, params object[] values)
		{
			this.fieldName = fieldName;
			this.@operator = Ops.None;
			this.multiOperator = multiOperator;
			this.values = values;
		}
	}
}