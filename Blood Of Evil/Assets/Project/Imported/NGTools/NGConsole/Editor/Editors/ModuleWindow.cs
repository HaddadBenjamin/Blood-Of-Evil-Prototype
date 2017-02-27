using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGConsole
{
	public class ModuleWindow : EditorWindow, IHasCustomMenu
	{
		private const int	ForceRepaintRefreshTick = 100;

		private Module		module;
		private NGConsoleWindow	console;
		[SerializeField]
		private int			moduleID;

		private Rect	r;

		public void	Init(NGConsoleWindow console, Module module)
		{
			this.console = console;
			this.module = module;
			this.moduleID = this.module.Id;
			this.SetTitle(module.name);
		}

		protected virtual void	OnEnable()
		{
			PerWindowVars.InitWindow(this, "Module");

			if (this.console == null)
			{
				NGConsoleWindow[]	consoles = Resources.FindObjectsOfTypeAll<NGConsoleWindow>();

				if (consoles.Length > 0)
					this.console = consoles[0];
				else
					return;
			}

			if (this.console.IsReady == false)
			{
				EditorApplication.delayCall += this.OnEnable;
				return;
			}

			this.module = this.console.GetModule(this.moduleID);
			if (this.module == null)
				EditorApplication.delayCall += this.OnEnable;
			else
			{
				this.r = new Rect(0F, 0F, this.position.width, this.position.height);
				this.wantsMouseMove = true;
				Utility.RegisterIntervalCallback(this.Repaint, ModuleWindow.ForceRepaintRefreshTick);
			}
		}

		protected virtual void	OnDisable()
		{
			Utility.UnregisterIntervalCallback(this.Repaint);
		}

		protected virtual void	OnGUI()
		{
			if (this.module == null)
				return;

			this.r.width = this.position.width;
			this.r.height = this.position.height;
			Utility.drawingWindow = this;
			this.module.OnGUI(this.r);
		}

		void	IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
		{
			menu.AddItem(new GUIContent("Help"), false, () => Application.OpenURL(Constants.WikiBaseURL + "#markdown-header-111-module"));
		}
	}
}