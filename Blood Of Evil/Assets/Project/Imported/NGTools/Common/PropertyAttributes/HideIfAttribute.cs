using UnityEngine;

namespace NGTools
{
	public class HideIfAttribute : PropertyAttribute
	{
		public readonly string		fieldName;
		public readonly Ops			@operator;
		public readonly MultiOps	multiOperator;
		public readonly object[]	values;

		public	HideIfAttribute(string fieldName, Ops @operator, object value)
		{
			this.fieldName = fieldName;
			this.@operator = @operator;
			this.multiOperator = MultiOps.None;
			this.values = new object[] { value };
		}

		public	HideIfAttribute(string fieldName, MultiOps multiOperator, params object[] values)
		{
			this.fieldName = fieldName;
			this.@operator = Ops.None;
			this.multiOperator = multiOperator;
			this.values = values;
		}
	}
}