using System.Collections.Generic;

namespace NGToolsEditor.NGRemoteScene
{
	public interface IReplaySettings
	{
		float	RecordLastSeconds { get; }
		int		TextureWidth { get; }
		int		TextureHeight { get; }

		List<CameraDataModuleEditor>	Modules { get; }
	}
}