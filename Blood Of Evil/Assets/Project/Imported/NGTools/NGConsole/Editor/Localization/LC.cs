namespace NGToolsEditor
{
	/// <summary>
	/// Alias of Localization.
	/// </summary>
	public static class LC
	{
		/// <summary>
		/// <para>Fetches the content associated to the given <paramref name="key"/>.</para>
		/// <para>Returns the content from the default language when not found.</para>
		/// <para>If still missing from the default language, returns a debug string.</para>
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static string	G(string key)
		{
			return Localization.Get(key);
		}
	}
}