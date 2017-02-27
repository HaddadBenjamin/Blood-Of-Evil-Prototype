using System;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor
{
	public sealed class ErrorPopup : PopupWindowContent
	{
		public const float	Height = 40F;

		public Exception	exception = null;
		public float		boxHeight = 40F;
		public string		customMessage = string.Empty;

		private Exception	workingException = null;
		private Vector2		size;
		private string		fullError;

		private int	clearRetry = 0;
		private int	lastExceptionHash = 0;

		public	ErrorPopup(string customMessage = "", float height = 30F)
		{
			this.customMessage = customMessage;
			this.boxHeight = height;
		}

		public void	OpenError(Rect position)
		{
			if (this.workingException == null)
				this.workingException = this.exception;

			this.fullError = this.workingException.ToString();

			this.size = GUI.skin.textArea.CalcSize(new GUIContent(this.fullError));
			this.size.y += ErrorPopup.Height;

			PopupWindow.Show(position, this);
		}

		public override Vector2	GetWindowSize()
		{
			return this.size;
		}

		public override void	OnGUI(Rect r)
		{
			float	w = r.width;

			r.height = ErrorPopup.Height;
			EditorGUI.HelpBox(r, "Please contact the author and send the following error :", MessageType.Info);

			r.width = 120F;
			r.x = w - r.width - 5F;
			r.y += 5F;
			r.height = ErrorPopup.Height - 10F;
			if (GUI.Button(r, "Contact the author") == true)
			{
				// Force the wizard to be unique.
				ScriptableWizard.GetWindow<ContactFormWizard>().Close();
				ContactFormWizard	wizard = ScriptableWizard.DisplayWizard<ContactFormWizard>(string.Empty);
				wizard.complementaryInformation = "Exception raised :\n" + this.fullError;
			}

			r.x = 0F;
			r.y += r.height + 5F;
			r.height = this.size.y - ErrorPopup.Height;
			r.width = w;
			EditorGUI.TextField(r, this.fullError);
		}

		public void	OnGUIRect(Rect r)
		{
			// Use this trick to avoid layout error when the exception is raised.
			if (Event.current.type == EventType.Layout)
				this.workingException = this.exception;

			if (this.workingException == null)
				return;

			r.width -= 16F;

			EditorGUI.HelpBox(r, this.customMessage, MessageType.Error);

			this.HandleEvents(r);

			r.x += r.width;
			r.width = 16F;
			if (GUI.Button(r, "X") == true)
				this.ClearException();
		}

		public void	OnGUILayout()
		{
			// Use this trick to avoid layout error when the exception is raised.
			if (Event.current.type == EventType.Layout)
				this.workingException = this.exception;

			if (this.workingException == null)
				return;

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.HelpBox(this.customMessage, MessageType.Error);
				Rect	r = GUILayoutUtility.GetLastRect();

				this.HandleEvents(GUILayoutUtility.GetLastRect());

				r.x = r.x + r.width + 1F;
				r.width = GUILayoutUtility.GetRect(25F, r.height, GUI.skin.button, GUILayout.Width(25F)).width + 4F;
				if (GUI.Button(r, "X") == true)
					this.ClearException();
			}
			EditorGUILayout.EndHorizontal();
		}

		private void	HandleEvents(Rect r)
		{
			EditorGUIUtility.AddCursorRect(r, MouseCursor.Link);

			if (Event.current.type == EventType.Repaint &&
				r.Contains(Event.current.mousePosition) == true)
			{
				Utility.DrawUnfillRect(r, Color.yellow);
			}

			if (Event.current.type == EventType.MouseMove &&
				r.Contains(Event.current.mousePosition) == true)
			{
				EditorWindow.mouseOverWindow.Repaint();
			}

			if (Event.current.type == EventType.MouseDown &&
				r.Contains(Event.current.mousePosition) == true)
			{
				this.OpenError(r);
			}
		}

		private void	ClearException()
		{
			this.fullError = this.workingException.ToString();

			int	hash = this.fullError.GetHashCode();

			this.exception = null;
			this.workingException = null;

			if (this.lastExceptionHash == hash)
				++this.clearRetry;
			else
			{
				this.lastExceptionHash = hash;
				this.clearRetry = 1;
			}

			if (this.clearRetry == 3)
			{
				if (EditorUtility.DisplayDialog(Constants.PackageTitle, "It seems that the error is redundant.\nPlease contact the author to improve stability and your brain sanity. :)", "Contact the author", "OK") == true)
				{
					// Force the wizard to be unique.
					ScriptableWizard.GetWindow<ContactFormWizard>().Close();
					ContactFormWizard	wizard = ScriptableWizard.DisplayWizard<ContactFormWizard>(string.Empty);
					wizard.complementaryInformation = "Exception raised :\n" + this.fullError;
				}
			}
		}
	}
}