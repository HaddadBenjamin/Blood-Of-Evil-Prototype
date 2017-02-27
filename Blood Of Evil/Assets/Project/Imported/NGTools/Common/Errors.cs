namespace NGTools
{
	public partial class Errors
	{
		// Server errors 1XXX
		public const int	Server_InternalServerError = 1000;
		public const int	Server_Exception = 1001;
	}

	public partial class Errors
	{
		// Scene server errors 3XXX
		public const int	Server_GameObjectNotFound = 3000;
		public const int	Server_ComponentNotFound = 3001;
		public const int	Server_PathNotResolved = 3002;
		public const int	Server_MethodNotFound = 3003;
		public const int	Server_InvalidArgument = 3004;
		public const int	Server_InvocationFailed = 3008;
		public const int	Server_MaterialNotFound = 3005;
		public const int	Server_ShaderNotFound = 3006;
		public const int	Server_ShaderPropertyNotFound = 3007;

		// Scene errors. 35XX
		public const int	Scene_Exception = 3500;
		public const int	Scene_GameObjectNotFound = 3501;
		public const int	Scene_ComponentNotFound = 3502;
		public const int	Scene_PathNotResolved = 3503;

		// Game Console errors. 4XXX
		public const int	GameConsole_NullDataConsole = 4000;

		// CLI errors. 45XX
		public const int	CLI_MethodDoesNotReturnString = 4500;
		public const int	CLI_RootCommandEmptyAlias = 4501;
		public const int	CLI_RootCommandNullBehaviour = 4502;
		public const int	CLI_EmptyRootCommand = 4503;
		public const int	CLI_ForbiddenCommandOnField = 4504;
		public const int	CLI_UnsupportedPropertyType = 4505;
		public const int	CLI_ForbiddenCharInName = 4506;

		// Camera errors. 5XXX
		public const int	Camera_RenderTextureFormatNotSupported = 5000;
		public const int	Camera_ModuleNotFound = 5001;

		// Fav errors. 7XXX
		public const int	Fav_ResolverNotStatic = 7000;
		public const int	Fav_ResolverIsAmbiguous = 7001;
		public const int	Fav_ResolverThrownException = 7002;
	}
}