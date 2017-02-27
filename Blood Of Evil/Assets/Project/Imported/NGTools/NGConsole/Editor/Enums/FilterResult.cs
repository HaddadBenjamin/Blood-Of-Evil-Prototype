namespace NGToolsEditor.NGConsole
{
	public enum FilterResult
	{
		/// <summary>
		/// The log is not handled by a filter.
		/// </summary>
		None,
		/// <summary>
		/// The log is accepted by a filter.
		/// </summary>
		Accepted,
		/// <summary>
		/// The log is refused by a filter.
		/// </summary>
		Refused
	}
}