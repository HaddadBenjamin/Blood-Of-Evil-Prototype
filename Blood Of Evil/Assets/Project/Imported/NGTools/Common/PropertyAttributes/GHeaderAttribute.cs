using UnityEngine;

namespace NGTools
{
	public class GHeaderAttribute : PropertyAttribute
	{
		public readonly bool	first;
		public readonly string	header;

		/// <summary>
		/// </summary>
		/// <param name="header">String defining the header.</param>
		/// <param name="first">Use when GHeader is the first element of a Group.</param>
		public	GHeaderAttribute(string header, bool first = false)
		{
			this.header = header;
			this.first = first;
		}
	}
}