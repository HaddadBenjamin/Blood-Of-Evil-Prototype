using NGTools;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NGToolsEditor.NGConsole
{
	[Serializable]
	public sealed class LogConditionParser : ILogContentGetter
	{
		public static readonly Frame[]					nullFrame = {};
		public static readonly List<Frame>				arrayFrames = new List<Frame>(8);
		public static readonly Dictionary<int, Frame>	cachedFrames = new Dictionary<int, Frame>();
		public static readonly Dictionary<int, Frame[]>	cachedFramesArrays = new Dictionary<int, Frame[]>();

		private string	headMessage;
		public string	HeadMessage
		{
			get
			{
				if (this.isParsed == false)
					this.ParseLog();
				return this.headMessage;
			}
		}

		private string	fullMessage;
		public string	FullMessage
		{
			get
			{
				if (this.isParsed == false)
					this.ParseLog();
				return this.fullMessage;
			}
		}

		private string	stackTrace;
		public string	StackTrace
		{
			get
			{
				if (this.isParsed == false)
					this.ParseLog();
				return this.stackTrace;
			}
		}

		public bool	HasStackTrace
		{
			get
			{
				return (this.frames != null && this.frames != LogConditionParser.nullFrame) ||
					   string.IsNullOrEmpty(this.StackTrace) == false;
			}
		}

		private Frame[]	frames;
		/// <summary>
		/// <para>An array of Frames giving parsed data.</para>
		/// <para>Is generated once on demand.</para>
		/// </summary>
		public Frame[]	Frames
		{
			get
			{
				if (this.isParsed == false)
					this.ParseLog();
				if (this.frames == null)
					this.ParseStackTrace();
				return this.frames;
			}
		}

		private string	category;
		public string	Category
		{
			get
			{
				if (this.isParsed == false)
					this.ParseLog();
				if (this.frames == null)
					this.ParseStackTrace();
				return this.category;
			}
		}

		/// <summary>Defines if the Row is ready to be used. Do never use a main value (Head message, full message, stack trace) from non-ready Row! IsParsed is used to delay or to skip non-essential Row from computation when receiving massive logs.</summary>
		public bool		isParsed;

		//public float	maxWidthFrame;
		//public float	maxWidthHeadLine;

		private LogEntry	log;

		public	LogConditionParser(LogEntry logEntry)
		{
			this.log = logEntry;
		}

		public void	Uninit()
		{
			this.frames = null;
		}

		/// <summary>
		/// Prepares the row by parsing its log.
		/// </summary>
		public void	ParseLog()
		{
			InternalNGDebug.AssertFile(this.isParsed == false, "Parsed Row is being parsed again.");

			this.isParsed = true;
			this.stackTrace = null;
			this.frames = null;
			//this.maxWidthFrame = float.MinValue;

			this.ParseCondition();

			var	n = this.FullMessage.IndexOf('\n');
			if (n > 0)
				this.headMessage = this.FullMessage.Substring(0, n);
			else
				this.headMessage = this.FullMessage;

			//if (Preferences.Settings.general.horizontalScrollbar == true)
			//{
			//	Utility.content.text = this.HeadMessage;
			//	Vector2	size = Preferences.Settings.log.style.CalcSize(Utility.content);
			//	if (this.maxWidthHeadLine < size.x)
			//		this.maxWidthHeadLine = size.x;
			//}
		}

		public void	ParseStackTrace()
		{
			if (this.ParseCompileLog() == true)
				return;

			//Debug.Log(this.log.mode +" " + (int)this.log.mode + " " + (int)Mode.DontExtractStacktrace);
			if (string.IsNullOrEmpty(this.stackTrace) == true)
			{
				this.frames = LogConditionParser.nullFrame;
				return;
			}
			//Debug.Log(this.log);
			//Debug.Log(this.headMessage);
			//Debug.Log(this.fullMessage);
			//Debug.Log(this.stackTrace);
			int		stackTraceHash = this.stackTrace.GetHashCode();
			bool	overwrite = false;

			if (LogConditionParser.cachedFramesArrays.TryGetValue(stackTraceHash, out this.frames) == true)
			{
				overwrite = true;

				for (int i = 0; i < this.frames.Length; i++)
				{
					if (string.IsNullOrEmpty(this.frames[i].category) == false)
					{
						this.category = this.frames[i].category;
						break;
					}
				}
			}
			else
			{
				string[]	frames = this.stackTrace.Split('\n');
				int			n = 0;

				LogConditionParser.arrayFrames.Clear();
				for (int i = 0; i < frames.Length; i++)
				{
					if (string.IsNullOrEmpty(frames[i]) == true)
						continue;

					Frame	f;
					int		hash = frames[i].GetHashCode();

					if (LogConditionParser.cachedFrames.TryGetValue(hash, out f) == false)
					{
						f = this.ParseStackFrame(frames[i], n);
						LogConditionParser.cachedFrames.Add(hash, f);
					}

					if (f == null)
						continue;

					if (string.IsNullOrEmpty(this.category) == true && string.IsNullOrEmpty(f.category) == false)
						this.category = f.category;

					//Debug.Log("Frame[" + n + "]=" + frame + "..." + frame.Length);
					if (Preferences.Settings.general.filterUselessStackFrame == true)
					{
						for (int j = 0; j < NGConsoleWindow.stackFrameFilters.Length; j++)
						{
							if (NGConsoleWindow.stackFrameFilters[j].Filter(frames[i]) == true)
							{
								//Debug.Log("Skipped");
								// Overwrite log's first stack frame when skipped.
								if (n == 0)
									overwrite = true;
								goto skipFrame;
							}
						}
					}

					if (f != null)
					{
						++n;
						LogConditionParser.arrayFrames.Add(f);
					}

					skipFrame:
					continue;
				}

				if (frames.Length > 0 && LogConditionParser.arrayFrames.Count == 0)
					this.frames = new Frame[1] { new Frame() { frameString = "No frame can be reached." } };
				else
					this.frames = LogConditionParser.arrayFrames.ToArray();

				LogConditionParser.cachedFramesArrays.Add(stackTraceHash, this.frames);
			}

			if (overwrite == true)
			{
				for (int i = 0; i < this.frames.Length; i++)
				{
					// Overwrite with the first available frame.
					if (string.IsNullOrEmpty(this.frames[i].fileName) == false)
					{
						this.log.file = this.frames[i].fileName;
						this.log.line = this.frames[i].line;
						break;
					}
				}
			}
		}

		/// <summary>
		/// Parses condition if the log is a compile output.
		/// </summary>
		/// <returns></returns>
		public bool	ParseCompileLog()
		{
			// Compile error/warning case.
			if ((this.log.mode & Mode.ScriptCompileError) != 0 ||
				(this.log.mode & Mode.ScriptCompileWarning) != 0)
			{
				this.frames = new Frame[1] {
					new Frame() {
						frameString = Utility.Color(this.log.file, Preferences.Settings.stackTrace.filepathColor) + " " + Utility.Color(this.log.line.ToString(), Preferences.Settings.stackTrace.lineColor),
						fileName = this.log.file,
						line = this.log.line,
						fileExist = true,
					}
				};
				this.fullMessage = this.log.condition;
				return true;
			}

			return false;
		}

		/// <summary>
		/// Extracts message and stack trace from the condition.
		/// </summary>
		private void	ParseCondition()
		{
			if (this.ParseCompileLog() == true)
				return;

			int	i;

			// Compile errors/warnings may not contain stack trace, but the file and line is included in condition.
			// Hence the position of the following condition.

			// No stack trace.
			if ((this.log.mode & (Mode.DontExtractStacktrace)) != 0)
			{
				this.frames = LogConditionParser.nullFrame;
				this.fullMessage = this.log.condition;
				return;
			}

			// Exception/Error/Assert log, with ScriptingWarning.
			if ((this.log.mode & (Mode.ScriptingWarning | // Strange, but it works well.
								  Mode.ScriptingException |
								  Mode.ScriptingError |
								  Mode.Error |
								  Mode.Fatal |
								  Mode.Assert |
								  Mode.GraphCompileError |
								  Mode.ScriptingAssertion)) != 0)
			{
				// Find first stack frame.
				i = this.log.condition.LastIndexOf(this.log.file + ":" + this.log.line);
				if (i != -1)
				{
					// Find beginning of frame.
					i = this.log.condition.LastIndexOf('\n', i);
					System.Diagnostics.Debug.Assert(i != -1, "EOL not found before first stack frame.");
					if (i != -1)
					{
						var	j = this.log.condition.LastIndexOf("UnityEngine.Debug:", i);
						if (j != -1)
						{
							// Extract the stack trace and remove the last new line.
							this.stackTrace = this.log.condition.Substring(j, this.log.condition.Length - j - 1);

							// Skip Debug.Log frame.
							i = this.log.condition.LastIndexOf('\n', j);

							if (i != -1)
								this.fullMessage = this.log.condition.Substring(0, i);
							else
								this.fullMessage = this.log.condition;
						}
						else
						{
							this.fullMessage = this.log.condition.Substring(0, i);
							this.stackTrace = this.log.condition.Substring(i + 1);
						}
						return;
					}
				}

				int	start = 0;

				while (start != -1)
				{
					// Skip a line
					start = this.log.condition.IndexOf('\n', start + 1);
					if (start != -1)
					{
						// The end of the line if there is.
						int	end = this.log.condition.IndexOf('\n', start + 1);

						// If this is the last line already.
						if (end == -1)
							end = this.log.condition.Length;

						// Verify if the line is a frame with a file:line.
						int	k = this.log.condition.IndexOf(") (at ", start, end - start);
						if (k != -1)
						{
							if (this.log.condition.IndexOf(":", start, k - start) > 0 && // A colon should be present before the method name.
								this.log.condition.IndexOf(":", k, end - k) > k) // The line should be present after the file and the method.
							{
								this.fullMessage = this.log.condition.Substring(0, start);
								this.stackTrace = this.log.condition.Substring(start + 1);
								return;
							}
						}
						else
						{
							k = this.log.condition.IndexOf("(", start, end - start);
							if (k != -1)
							{
								// Or verify if the line is just a method.
								if (this.log.condition.IndexOf(":", start, k - start) > 0 && // A colon should be present before the method name.
									this.log.condition.IndexOf(")", k, end - k) > k) // The end parenthese should be present at the very end.
								{
									this.fullMessage = this.log.condition.Substring(0, start);
									this.stackTrace = this.log.condition.Substring(start + 1);
									return;
								}
							}
						}
					}
				}

				this.fullMessage = this.log.condition;

				this.frames = LogConditionParser.nullFrame;
				return;
			}

			// Find the exact first stack frame of Debug.Log*().
			i = this.log.condition.LastIndexOf("UnityEngine.Debug:");
			if (i != -1)
			{
				// Extract the stack trace and remove the last new line.
				this.stackTrace = this.log.condition.Substring(i, this.log.condition.Length - i - 1);

				i = this.log.condition.LastIndexOf('\n', i);

				if (i != -1)
					this.fullMessage = this.log.condition.Substring(0, i);
				else
					this.fullMessage = this.log.condition;
			}
			else
			{
				this.fullMessage = this.log.condition;
				this.frames = LogConditionParser.nullFrame;
			}
		}

		private Frame	ParseStackFrame(string raw, int n)
		{
			Frame	frame = new Frame();
			frame.raw = raw;

			FrameBuilder.Clear();

			StringBuilder	buffer = Utility.GetBuffer();

			try
			{

				// Exception/Error log.
				if ((this.log.mode & (Mode.ScriptingException |
									  Mode.ScriptingError |
									  Mode.ScriptingError |
									  Mode.Error |
									  Mode.Fatal |
									  Mode.GraphCompileError)) != 0)
				{
					buffer.Append(raw);

					// For convenience, just convert it to a normal log, using the StringBuilder, also for convenience.
					int	lastDot = 0;
					for (int i = 0; i < buffer.Length; ++i)
					{
						// Detect next symbole, skip constructors and static constructors. (".ctor" and ".cctor")
						if (buffer[i] == '.' &&
							i > 0 &&
							buffer[i - 1] != '.' &&
							buffer[i - 1] != ':')
						{
							lastDot = i;
						}
						else if (buffer[i] == ':')
							lastDot = i;
						// Delete any spaces before parameters.
						else if (buffer[i] == ' ')
						{
							buffer.Remove(i, 1);
							--i;
						}

						// Find start of method and avoid special prefix starting with a parenthese.
						if (buffer[i] == '(' && i > 0)
						{
							buffer[lastDot] = ':';
							break;
						}
					}

					raw = buffer.ToString();
					buffer.Length = 0;
				}

				// Handle "Rethrow as Exception:"
				if (raw.StartsWith("Rethrow") == true)
					return null;

				// Too much stacks get truncated possibly anywhere.
				if (raw.Contains("<messagetruncated>") == true)
					return null;

				//InternalNGDebug.LogFile("raw="+raw);

				bool	unreachableFrame = false;

				int		endClass = raw.IndexOf(':');
				FrameBuilder.classType = raw.Substring(0, endClass);

				int		endMethod = raw.IndexOf('(', endClass);
				FrameBuilder.methodName = raw.Substring(endClass + 1, endMethod - endClass - 1);

				int		endParameters = raw.IndexOf(')', endMethod);
				string	allParameters = raw.Substring(endMethod + 1, endParameters - endMethod - 1);

				int		startFilepath = raw.IndexOf("at ", endMethod);

				// Handle yield context, i.e. "Namespace.Class.<Method>c__Iterator23:MoveNext".
				if (FrameBuilder.classType.Contains("<") == true)
				{
					unreachableFrame = true;
					buffer.Append(FrameBuilder.classType);

					int	startYieldMethod = FrameBuilder.classType.IndexOf('<');
					int	endYieldMethod = FrameBuilder.classType.IndexOf('>');

					buffer.Remove(endYieldMethod, buffer.Length - endYieldMethod);
					buffer.Remove(0, startYieldMethod + 1);

					if (Preferences.Settings.stackTrace.displayReturnValue == true)
						FrameBuilder.returnType = "YieldInstruction";

					FrameBuilder.classType = "";
					FrameBuilder.methodName = buffer.ToString();

					buffer.Length = 0;
				}
				// Handle special prefix, i.e. "(wrapper remoting-invoke-with-check) Namespace.Class".
				else if (FrameBuilder.classType.Contains("(") == true)
				{
					buffer.Append(FrameBuilder.classType);
					buffer.Remove(0, FrameBuilder.classType.IndexOf(')') + 1);
					FrameBuilder.classType = buffer.ToString();

					buffer.Length = 0;
				}
				// Handle generic method, i.e. "Namespace.Class.Method[Type]".
				else if (FrameBuilder.methodName.Contains("[") == true)
				{
					buffer.Append(FrameBuilder.methodName);

					int	startGenericType = FrameBuilder.methodName.IndexOf('[');
					int	endGenericType = FrameBuilder.methodName.LastIndexOf(']');

					buffer.Remove(startGenericType, endGenericType - startGenericType + 1);

					FrameBuilder.methodName = buffer.ToString();

					buffer.Length = 0;
				}

				// Split namespace from class.
				n = FrameBuilder.classType.LastIndexOf('.');
				if (n != -1)
				{
					FrameBuilder.namespaceName = FrameBuilder.classType.Substring(0, n);
					FrameBuilder.classType = FrameBuilder.classType.Substring(n + 1);
				}

				//InternalNGDebug.LogFile("class=" + FrameBuilder.classType);
				//InternalNGDebug.LogFile("method=" + FrameBuilder.methodName);
				//InternalNGDebug.LogFile("param=" + allParameters);

				if (startFilepath != -1)
				{
					int		endFilepath = raw.LastIndexOf(':', raw.Length - 1);
					string	filepath = raw.Substring(startFilepath + 3, endFilepath - startFilepath - 3);

					int		endLine = raw.IndexOf(')', endFilepath);
					string	line = raw.Substring(endFilepath + 1, endLine - endFilepath - 1);

					// Skip non-editable frame.
					frame.fileExist = Utility.files.FileExist(filepath);
					if (frame.fileExist == false &&
						Preferences.Settings.stackTrace.skipUnreachableFrame == true)
					{
						return null;
					}

					frame.fileName = filepath;
					frame.line = int.Parse(line);
					FrameBuilder.fileExist = frame.fileExist;
					//InternalNGDebug.LogFile("filepath="+filepath);
					//InternalNGDebug.LogFile("line=" + int.Parse(line));
				}
				// It is not guaranteed that the fileName is the first frame. Use it in the only case of total failure. i.e. Debug.Log*()
				else if (string.IsNullOrEmpty(this.log.file) == true && n == 0)
				{
					frame.fileName = this.log.file;
					frame.line = this.log.line;
				}
				else
				{
					frame.fileName = string.Empty;
				}

				FrameBuilder.fileName = frame.fileName;
				FrameBuilder.line = frame.line;

				if (unreachableFrame == false &&
					(Preferences.Settings.stackTrace.displayReturnValue == true ||
					 Preferences.Settings.stackTrace.displayArgumentName == true))
				{
					FastClassCache.HashClass	classType = Utility.classes.GetType(FrameBuilder.namespaceName, FrameBuilder.classType);

					if (classType != null)
					{
						if (FrameBuilder.methodName == ".cctor" || // Static constructor.
							FrameBuilder.methodName == ".ctor") // Normal constructor.
						{
							string[]	parameters = allParameters.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

							// Many parameters includes ambiguity.
							if (parameters.Length > 0 && classType.type != null)
							{
								ConstructorInfo	ci = this.GetConstructorFromParameters(classType.type, parameters);

								// Append data from the found constructor.
								if (ci != null)
								{
									FrameBuilder.namespaceName = ci.ReflectedType.Namespace;
									FrameBuilder.classType = ci.ReflectedType.Name;

									if (Preferences.Settings.stackTrace.displayArgumentType == true ||
										Preferences.Settings.stackTrace.displayArgumentName == true)
									{
										this.AppendParameters(ci.GetParameters());
									}
								}
								// Append every data as we can from the condition.
								else
									this.AddConditionParameters(parameters);
							}
						}
						else if (classType.methods != null)
						{
							MethodInfo	methodInfo = classType.methods.GetMethod(FrameBuilder.methodName, allParameters);

							// Append data from the found method.
							if (methodInfo != null)
							{
								FrameBuilder.isStaticMethod = methodInfo.IsStatic;

								NGLoggerAttribute[]	attributes = methodInfo.GetCustomAttributes(typeof(NGLoggerAttribute), false) as NGLoggerAttribute[];
								if (attributes.Length > 0)
									frame.category = attributes[0].tag;

								if (Preferences.Settings.stackTrace.displayReturnValue == true)
									FrameBuilder.returnType = methodInfo.ReturnType.Name;

								if (methodInfo.ReflectedType.IsGenericTypeDefinition == true)
								{
									n = FrameBuilder.classType.IndexOf("[");

									if (n != -1)
									{
										string[]	genericTypes = FrameBuilder.classType.Substring(n + 1, FrameBuilder.classType.Length - n - 2).Split(','); // Removed the 2 brackets.

										for (int i = 0; i < genericTypes.Length; i++)
										{
											Type	type = Utility.GetType(genericTypes[i]);

											if (type != null)
												FrameBuilder.genericTypes.Add(type.Name);
											else
												FrameBuilder.genericTypes.Add(genericTypes[i]);
										}
									}
									else
									{
										foreach (var type in methodInfo.ReflectedType.GetGenericArguments())
											FrameBuilder.genericTypes.Add(type.Name);
									}
								}

								FrameBuilder.namespaceName = methodInfo.ReflectedType.Namespace;
								FrameBuilder.classType = methodInfo.ReflectedType.Name;
								FrameBuilder.methodName = methodInfo.Name;

								if (Preferences.Settings.stackTrace.displayArgumentType == true ||
									Preferences.Settings.stackTrace.displayArgumentName == true)
								{
									this.AppendParameters(methodInfo.GetParameters());
								}
							}
							// Append every data as we can from the condition.
							else
							{
								string[]	parameters = allParameters.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

								this.AddConditionParameters(parameters);
							}
						}
					}
				}

				// Get custom Category from NGSettings, it does not have priority over attribute!
				if (frame.category == null)
				{
					string	m = FrameBuilder.namespaceName + '.' + FrameBuilder.classType + ':' + FrameBuilder.methodName;

					for (int i = 0; i < Preferences.Settings.stackTrace.categories.Count; i++)
					{
						if (Preferences.Settings.stackTrace.categories[i].method == m)
						{
							frame.category = Preferences.Settings.stackTrace.categories[i].category;
							break;
						}
					}
				}

				if (n == 0)
					frame.frameString = FrameBuilder.ToString("→ ", Preferences.Settings.stackTrace);
				else
					frame.frameString = FrameBuilder.ToString("↑ ", Preferences.Settings.stackTrace);

				//if (Preferences.Settings.general.horizontalScrollbar == true)
				//{
				//	// Calcul max width
				//	Utility.content.text = frame.frameString;
				//	Vector2	size = Preferences.Settings.stackTrace.style.CalcSize(Utility.content);
				//	if (this.maxWidthFrame < size.x)
				//		this.maxWidthFrame = size.x;
				//}
			}
			catch (Exception ex)
			{
				InternalNGDebug.LogFileException("Raw " + n + "=" + raw + Environment.NewLine + FrameBuilder.ToRawString(), ex);
				InternalNGDebug.LogFile(frame.frameString);
				return null;
			}
			finally
			{
				Utility.RestoreBuffer(buffer);
			}

			return frame;
		}

		/// <summary>
		/// Adds parameters from the condition when Method/ConstructorInfo is not found.
		/// </summary>
		/// <param name="parameters"></param>
		private void	AddConditionParameters(string[] parameters)
		{
			for (int i = 0; i < parameters.Length; i++)
			{
				string	paramTrim = parameters[i].Trim();
				string[] paramData = paramTrim.Split(' ');

				if (paramData.Length == 2)
				{
					FrameBuilder.parameterTypes.Add(paramData[0]);
					FrameBuilder.parameterNames.Add(paramData[1]);
				}
				else
					FrameBuilder.parameterTypes.Add(paramData[0]);
			}
		}

		private ConstructorInfo	GetConstructorFromParameters(Type classType, string[] parameters)
		{
			List<ConstructorInfo>	matchingParamContructors = new List<ConstructorInfo>();

			// Get constructors matching the exact number of parameters.
			foreach (var c in classType.GetConstructors(~BindingFlags.Default))
			{
				if (c.GetParameters().Length == parameters.Length)
					matchingParamContructors.Add(c);
			}

			if (matchingParamContructors.Count == 1)
				return matchingParamContructors[0];
			else
			{
				// In case of many matching methods, due to lack of data (e.g. System.Object and UnityEngine.Object both displaying as Object...).
				// Take the the first method that match the parameters.
				foreach (var c in matchingParamContructors)
				{
					if (this.MatchParameters(c.GetParameters(), parameters) == true)
						return c;
				}
			}
			return null;
		}

		private bool	MatchParameters(ParameterInfo[] parameters, string[] models)
		{
			for (int i = 0; i < parameters.Length; i++)
			{
				string		paramTrim = models[i].Trim();
				string[]	paramData = paramTrim.Split(' ');

				//Debug.Log(parameters[i].ParameterType.FullName + "(" + paramData[0] + ")");
				// Check type.
				if (parameters[i].ParameterType.FullName.Contains(paramData[0]) == false)
				{
					//Debug.Log("NoMatch");
					return false;
				}

				// Check name.
				if (paramData.Length == 2)
				{
					//Debug.Log(parameters[i].Name + "(" + paramData[1] + ")");
					if (parameters[i].Name.Contains(paramData[1]) == false)
					{
						//Debug.Log("NoMatch");
						return false;
					}
				}
			}
			return true;
		}

		/// <summary>
		/// Appends parameters' type and name. It only handles ref, out, default and nullable parameter.
		/// </summary>
		/// <param name="parameters"></param>
		private void	AppendParameters(ParameterInfo[] parameters)
		{
			for (int i = 0; i < parameters.Length; i++)
			{
				if (Preferences.Settings.stackTrace.displayArgumentType == true)
				{
					StringBuilder	buffer = Utility.GetBuffer();

					if (parameters[i].IsOptional == true)
						buffer.Append('[');

					// Handle type constraints.
					if (parameters[i].IsOut == true)
					{
						buffer.Append("out ");

						// Handle nullable type.
						if (parameters[i].ParameterType.GetElementType().UnderlyingSystemType.IsGenericType == true &&
							parameters[i].ParameterType.GetElementType().UnderlyingSystemType.GetGenericTypeDefinition() == typeof(Nullable<>))
						{
							buffer.Append(Nullable.GetUnderlyingType(parameters[i].ParameterType.GetElementType().UnderlyingSystemType).Name);
							buffer.Append('?');
						}
						else
							buffer.Append(parameters[i].ParameterType.Name);

					}
					else if (parameters[i].ParameterType.IsByRef == true)
					{
						buffer.Append("ref ");

						// Handle nullable type.
						if (parameters[i].ParameterType.GetElementType().UnderlyingSystemType.IsGenericType == true &&
							parameters[i].ParameterType.GetElementType().UnderlyingSystemType.GetGenericTypeDefinition() == typeof(Nullable<>))
						{
							buffer.Append(Nullable.GetUnderlyingType(parameters[i].ParameterType.GetElementType().UnderlyingSystemType).Name);
							buffer.Append('?');
						}
						else
							buffer.Append(parameters[i].ParameterType.Name);
					}
					else
					{
						// Handle nullable type.
						if (parameters[i].ParameterType.IsGenericType == true &&
							parameters[i].ParameterType.GetGenericTypeDefinition() == typeof(Nullable<>))
						{
							buffer.Append(Nullable.GetUnderlyingType(parameters[i].ParameterType).Name);
							buffer.Append('?');
						}
						else
							buffer.Append(parameters[i].ParameterType.Name);
					}

					if (!(parameters[i].RawDefaultValue is DBNull))
					{
						buffer.Append(" = ");
						if (parameters[i].RawDefaultValue == null)
							buffer.Append("null");
						else
							buffer.Append(parameters[i].RawDefaultValue);
					}

					if (parameters[i].IsOptional == true)
						buffer.Append(']');

					FrameBuilder.parameterTypes.Add(Utility.ReturnBuffer(buffer));

					if (Preferences.Settings.stackTrace.displayArgumentName == true)
						FrameBuilder.parameterNames.Add(parameters[i].Name);
				}
				else if (Preferences.Settings.stackTrace.displayArgumentName == true)
						FrameBuilder.parameterNames.Add(parameters[i].Name);
			}
		}
	}
}