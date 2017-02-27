using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGConsole
{
	[Serializable]
	public sealed class GroupFilters : ISerializationCallbackReceiver
	{
		public event Action	FilterAltered;

		[Exportable(ExportableAttribute.ArrayOptions.Immutable)]
		public List<ILogFilter>	filters;

		[SerializeField]
		private byte[]	serializedFilters;

		public	GroupFilters()
		{
			this.filters = new List<ILogFilter>();
		}

		/// <summary>
		/// <para>Checks whether the given <paramref name="Row"/> is accepted or refused by filters.</para>
		/// <para>If no filter enabled, it returns true.</para>
		/// <para>To accept a log, at least one filter must accept it by returning Accepted.</para>
		/// <para>But if one filter returns Refused, this overwhelms the final result and the log if rejected.</para>
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		public bool	Filter(Row row)
		{
			// By default, row is accepted if there is no filter activated.
			bool	isAccepted = true;

			for (int i = 0; i < this.filters.Count; i++)
			{
				// But is rejected if at least one filter is activated.
				if (this.filters[i].Enabled == true)
					isAccepted = false;
			}

			for (int i = 0; i < this.filters.Count; i++)
			{
				if (this.filters[i].Enabled == true)
				{
					FilterResult	result = this.filters[i].CanDisplay(row);

					if (result == FilterResult.Refused)
						return false;

					if (result == FilterResult.Accepted)
						isAccepted = true;
				}
			}

			return isAccepted;
		}

		public void	OnGUI()
		{
			for (int i = 0; i < this.filters.Count; i++)
			{
				EditorGUI.BeginChangeCheck();
				this.filters[i].Enabled = GUILayout.Toggle(this.filters[i].Enabled, LC.G(this.filters[i].GetType().Name), Preferences.Settings.general.menuButtonStyle);
				if (EditorGUI.EndChangeCheck() == true)
				{
					// Delete on middle click.
					if (Event.current.button == 2)
					{
						this.filters.RemoveAt(i);
						this.OnFilterAltered();
					}
					// Show context menu on right click.
					else if (Event.current.button == 1)
					{
						// Cancel toggle.
						this.filters[i].Enabled = !this.filters[i].Enabled;

						GenericMenu	menu = new GenericMenu();
						menu.AddItem(new GUIContent("Delete"), false, this.DeleteStream, this.filters[i]);
						menu.DropDown(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0, 0));
					}
				}
			}

			if (GUILayout.Button("+", Preferences.Settings.general.menuButtonStyle) == true)
			{
				if (FreeConstants.CheckMaxFilters(this.filters.Count) == true)
				{
					GenericMenu	menu = new GenericMenu();

					for (int i = 0; i < NGConsoleWindow.logFilterTypes.Length; ++i)
						menu.AddItem(new GUIContent(Utility.NicifyVariableName(LC.G(NGConsoleWindow.logFilterTypes[i].Name))), false, this.AddFilter, i);

					menu.DropDown(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0, 0));
				}
			}

			GUILayout.FlexibleSpace();
		}

		private void	AddFilter(object data)
		{
			ILogFilter	filter = Activator.CreateInstance(NGConsoleWindow.logFilterTypes[(int)data]) as ILogFilter;

			this.filters.Add(filter);

			filter.ToggleEnable += this.OnFilterAltered;
			filter.Enabled = true;
		}

		private void	DeleteStream(object data)
		{
			((ILogFilter)data).ToggleEnable -= this.OnFilterAltered;
			this.filters.Remove((ILogFilter)data);

			this.OnFilterAltered();
		}

		private void	OnFilterAltered()
		{
			if (this.FilterAltered != null)
				this.FilterAltered();
		}

		public void	OnAfterDeserialize()
		{
			if (this.serializedFilters != null)
			{
				try
				{
					this.filters = Utility.DeserializeField<List<ILogFilter>>(this.serializedFilters);
				}
				catch
				{
				}
			}
		}

		public void	OnBeforeSerialize()
		{
			try
			{
				this.serializedFilters = Utility.SerializeField(this.filters);
			}
			catch
			{
			}
		}
	}
}