namespace NGToolsEditor
{
	public interface IGUIInitializer
	{
		/// <summary>
		/// Initializes settings for the very first time. Be careful to not override existing values like ScriptableObject, GUIStyle, etc...
		/// </summary>
		void	InitGUI();
	}
}