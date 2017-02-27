using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace NGToolsEditor.NGConsole
{
	internal static class FrameBuilder
	{
		public static string		returnType;
		public static string		namespaceName;
		public static string		classType;
		public static List<string>	genericTypes = new List<string>();
		public static bool			isStaticMethod;
		public static string		methodName;
		public static List<string>	parameterTypes = new List<string>();
		public static List<string>	parameterNames = new List<string>();
		public static bool			fileExist;
		public static string		fileName;
		public static int			line;

		public static void	Clear()
		{
			FrameBuilder.returnType = string.Empty;
			FrameBuilder.namespaceName = string.Empty;
			FrameBuilder.classType = string.Empty;
			FrameBuilder.genericTypes.Clear();
			FrameBuilder.isStaticMethod = false;
			FrameBuilder.methodName = string.Empty;
			FrameBuilder.parameterTypes.Clear();
			FrameBuilder.parameterNames.Clear();
			FrameBuilder.fileExist = false;
			FrameBuilder.fileName = string.Empty;
			FrameBuilder.line = 0;
		}

		public static string	ToRawString()
		{
			return FrameBuilder.returnType
				+ " " + FrameBuilder.namespaceName
				+ " " + FrameBuilder.classType
				+ "<" + string.Join(", ", FrameBuilder.genericTypes.ToArray())
				+ "> " + FrameBuilder.methodName
				+ "(" + string.Join(", ", FrameBuilder.parameterTypes.ToArray())
				+ " ; " + string.Join(", ", FrameBuilder.parameterNames.ToArray())
				+ ") " + FrameBuilder.fileExist
				+ " " + FrameBuilder.fileName
				+ ":" + FrameBuilder.line;
		}

		public static string	ToString(string prefix, NGSettings.StackTraceSettings settings)
		{
			// Append prefix.
			StringBuilder	buffer = Utility.GetBuffer();

			buffer.Append(prefix);

			// Append return type.
			if (settings.displayReturnValue == true)
			{
				buffer.Append(FrameBuilder.returnType, settings.returnValueColor);
				if (settings.indentAfterReturnType == true)
				{
					if (FrameBuilder.returnType.Length < 8)
						buffer.Append('	');
					else
						buffer.Append(' ');
				}
				else
					buffer.Append(' ');
			}

			// Append class type and method name.
			if (settings.displayReflectedType == NGSettings.StackTraceSettings.DisplayReflectedType.None ||
				string.IsNullOrEmpty(FrameBuilder.classType) == true)
			{
				buffer.Append(FrameBuilder.methodName, settings.methodNameColor);
			}
			else
			{
				if (settings.displayReflectedType == NGSettings.StackTraceSettings.DisplayReflectedType.NamespaceAndClass)
				{
					buffer.AppendStartColor(settings.reflectedTypeColor);
					buffer.Append(FrameBuilder.namespaceName);
					buffer.Append('.');
					buffer.Append(FrameBuilder.classType);
				}
				else
				{
					buffer.AppendStartColor(settings.reflectedTypeColor);
					buffer.Append(FrameBuilder.classType);
				}

				if (FrameBuilder.genericTypes.Count > 0)
				{
					buffer.Append('<');

					for (int i = 0; i < FrameBuilder.genericTypes.Count; i++)
					{
						buffer.Append(FrameBuilder.genericTypes[i], settings.argumentTypeColor);
						buffer.Append(',');
					}

					buffer.Length -= 1;
					buffer.Append('>');
				}

				if (FrameBuilder.isStaticMethod == true)
					buffer.Append(':');
				else
					buffer.Append('.');
				buffer.AppendEndColor();
				buffer.Append(methodName, settings.methodNameColor);
			}

			buffer.Append("(");

			// Append parameters.
			for (int i = 0; i < parameterTypes.Count; i++)
			{
				if (settings.displayArgumentType == true)
				{
					buffer.Append(parameterTypes[i], settings.argumentTypeColor);
					if (settings.displayArgumentName == true && i < parameterNames.Count)
					{
						buffer.Append(' ');
						buffer.Append(parameterNames[i], settings.argumentNameColor);
					}
				}
				else if (settings.displayArgumentName == true && i < parameterNames.Count)
					buffer.Append(parameterNames[i], settings.argumentNameColor);
				buffer.Append(", ");
			}
			if (parameterTypes.Count > 0)
				buffer.Remove(buffer.Length - 2, 2);

			buffer.Append(")");

			if (settings.indentAfterArgument == true)
				buffer.Append('	');
			else
				buffer.Append(' ');

			// Append frame file and line.
			if (string.IsNullOrEmpty(FrameBuilder.fileName) == false &&
				Preferences.Settings.stackTrace.displayFilepath != NGSettings.PathDisplay.Hidden)
			{
				if (Preferences.Settings.stackTrace.displayFilepath == NGSettings.PathDisplay.Visible ||
					(Preferences.Settings.stackTrace.displayFilepath == NGSettings.PathDisplay.OnlyIfExist &&
					 FrameBuilder.fileExist == true))
				{
					// Change filename
					if (settings.displayRelativeToAssets == true &&
						FrameBuilder.fileExist == true &&
						FrameBuilder.fileName.StartsWith("Assets") == false)
					{
						FrameBuilder.fileName = FrameBuilder.fileName.Substring(Application.dataPath.Length + 1);
					}

					buffer.Append(FrameBuilder.fileName, settings.filepathColor);
					buffer.Append(' ');
					buffer.Append(FrameBuilder.line.ToString(), settings.lineColor);
				}
			}

			return Utility.ReturnBuffer(buffer);
		}
	}
}