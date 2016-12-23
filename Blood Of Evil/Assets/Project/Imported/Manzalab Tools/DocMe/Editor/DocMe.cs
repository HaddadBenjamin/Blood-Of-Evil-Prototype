using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.IO;


//This is the DocMe Editor documented with DocMe itself :P
public class DocMe : EditorWindow{

	//Holds info about a type's parent and childs used along with the dictionary
	public class TypeLinks{

		public Type parent;
		public List<Type> childs = new List<Type>();
	}

	//Holds info about a member documentation
	public class DocInfo{

		public string text;
		public int line;
		public string link;

		public DocInfo(string text, int line){
			this.text = text;
			this.line = line;
		}

		public DocInfo(string text, int line, string link){
			this.text = text;
			this.line = line;
			this.link = link;
		}
	}


	//Main

	private MonoScript targetScript                       = null;
	private Type targetType                               = null;
	
	private  Dictionary<string, List<Type>> assemblyTypes = new Dictionary<string, List<Type>>();
	
	private Dictionary<Type, DocInfo> docClasses          = new Dictionary<Type, DocInfo>();
	private Dictionary<FieldInfo, DocInfo> docFields      = new Dictionary<FieldInfo, DocInfo>();
	private Dictionary<PropertyInfo, DocInfo> docProps    = new Dictionary<PropertyInfo, DocInfo>();
	private Dictionary<MethodInfo, DocInfo> docMethods    = new Dictionary<MethodInfo, DocInfo>();
	
	private List<Type> typePath                           = new List<Type>();
	private Dictionary<Type, string[]> cachedSources      = new Dictionary<Type, string[]>();
	private Dictionary<MemberInfo, DocInfo> cachedDocs    = new Dictionary<MemberInfo, DocInfo>();
	private int lastParsedLine                            = 0;
	//
	
	//Project related

	private Dictionary<Type, TypeLinks> typeNodes = new Dictionary<Type, TypeLinks>();
	private List<Type> projectTypes               = new List<Type>();
	private List<Type> projectBaseTypes           = new List<Type>();
	//

	//GUI Specific
	
	private Rect scriptsListRect;
	private Rect mainRect;
	private float rightMargin               = 18;
	private GUIStyle projectListStyle       = new GUIStyle();
	private GUIStyle rich                   = new GUIStyle();
	private Type hierarchySelected          = null;
	
	private bool showFields                 = true;
	private bool showProperties             = true;
	private bool showMethods                = true;
	private string filterString             = string.Empty;
	private string filterProjectSciptString = string.Empty;
	private bool filterShowAttributes       = false;
	private bool filterShowDocumentation    = true;
	private bool filterOnlyPublic           = true;
	private bool filterOnlyDocumented       = false;
	private bool filterOnlyProject          = false;
	private bool filterOnlyDeclared         = true;
	private bool _showProjectView           = false;
	
	private int fieldViewCount;
	private int propViewCount;
	private int methodViewCount;
	private Texture2D tex;
	private Vector2 scrollPos;
	private Vector2 scrollPosProject;
	private List<Type> backQueu             = new List<Type>();
	private string lastEntryName;
	private int netPendings;
	private string typeSearchString;
	private List<Type> typeSearchResults    = new List<Type>();
	private DocMeRoutiner _coroutiner;
	//

	/////////
	/////////

	//the routiner helping monobehaviour for coroutines
	private DocMeRoutiner coroutiner{
		get
		{
			if (!_coroutiner){
				_coroutiner = new GameObject("_DocMeRoutiner").AddComponent(typeof(DocMeRoutiner)) as DocMeRoutiner;
				_coroutiner.gameObject.hideFlags = HideFlags.HideInInspector;
			}
			return _coroutiner;
		}
	}

	private bool showProjectView{
		get
		{
			return _showProjectView;
		}
		set
		{
			_showProjectView = value;
			rightMargin = value? 200 : 18;
		}
	}

	private bool darkTheme{
		get {return EditorGUIUtility.isProSkin;}
	}

	private string hexOrange{
		get {return darkTheme? "e9b64d" : "000000";}
	}

	private string hexBlue{
		get {return darkTheme? "9090fd" : "3333cc";}
	}

	private string hexDark{
		get {return darkTheme? "f2f2f2" : "000000";}
	}
	

	/////////
	/////////

	//cleaning
	public void OnDestroy(){
		if (_coroutiner)
			DestroyImmediate(_coroutiner.gameObject);
	}

	//SetDirty the routiner monobehaviour so that it runs coroutines for this window
	public void Update(){
		if (_coroutiner)
			EditorUtility.SetDirty(_coroutiner);
	}

	//initialize some stuff
	public void OnEnable(){

		rich.normal.textColor = darkTheme? new Color(0.8f, 0.8f, 0.8f) : new Color(0.2f,0.2f,0.2f);
		rich.fontSize = 12;
		rich.richText = true;
		rich.alignment = TextAnchor.MiddleLeft;
		rich.padding = new RectOffset(5, 0, 2, 1);

		projectListStyle.normal.textColor = darkTheme? Color.white : Color.black;
		projectListStyle.alignment = TextAnchor.MiddleLeft;
		projectListStyle.border = new RectOffset(2,2,2,2);
		projectListStyle.margin = new RectOffset(2,8,7,6);
		projectListStyle.overflow = new RectOffset(2,2,2,2);
		projectListStyle.stretchWidth = true;
		projectListStyle.clipping = TextClipping.Clip;

		CollectAssemblies();
		MakeProjectHierarhy();

		//setting the target type first for the backqueu trick
		targetType = typeof(MonoBehaviour);
		NewView(typeof(MonoBehaviour));
		Repaint();
	}

	//Set a new view when a script is selected in Unity's project tab
	public void OnSelectionChange(){
		
		System.Object[] selection = Selection.GetFiltered(typeof(MonoScript), SelectionMode.Assets);
		if (selection.Length == 0)
			return;

		MonoScript monoScript = selection[0] as MonoScript;
		if (monoScript.GetClass() != null && monoScript.GetClass() != targetType)
			NewView(monoScript.GetClass());
	}

	//collect all types but nested available into a dict
	public void CollectAssemblies(){

		assemblyTypes.Clear();
		Assembly[] assemblies= AppDomain.CurrentDomain.GetAssemblies();
		foreach (Assembly assembly in assemblies){
			
			string simpleName= assembly.GetName().Name;
			if (!assemblyTypes.ContainsKey(simpleName))
				assemblyTypes[simpleName] = new List<Type>();

			foreach (Type type in assembly.GetTypes()){

				if (type.IsNested)
					continue;

				assemblyTypes[simpleName].Add(type);
			}
		}
	}

	//Is the type a project type
	public bool IsProject(Type type){
		return projectTypes.Contains(type);
	}

	// Gather information about project types into nodes
	public void MakeProjectHierarhy(){

		projectTypes.Clear();
		typeNodes.Clear();
		projectBaseTypes.Clear();
		
		foreach (MonoScript script in GetAllScriptsOfType(typeof(System.Object))){
			projectTypes.Add(script.GetClass());
			foreach (Type nested in script.GetClass().GetNestedTypes(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly))
				projectTypes.Add(nested);
		}

		foreach (Type typeA in projectTypes){
		
			if (!typeNodes.ContainsKey(typeA))
				typeNodes[typeA] = new TypeLinks();

			foreach (Type typeB in projectTypes){
		
				if (typeB.BaseType == typeA)
					typeNodes[typeA].childs.Add(typeB);
			}

			typeNodes[typeA].parent = IsProject(typeA.BaseType)? typeA.BaseType : null;

			if (!IsProject(typeA.BaseType) && !typeA.IsNested)
				projectBaseTypes.Add(typeA);
		}
	}

	private void GotoPrevious(){

		Type previous= backQueu[backQueu.Count -1];
		backQueu.Remove(previous);
		targetType = previous;
		NewView(previous);
	}

	// Set a new view for a type. Document it, it's members and all backward inheritance
	public void NewView(Type newType){

		if (newType != targetType)
			backQueu.Add(targetType);

		targetType = newType;

		docClasses.Clear();
		docFields.Clear();
		docProps.Clear();
		docMethods.Clear();
		typePath.Clear();
		scrollPos = Vector2.zero;

		Type typeToDoc= targetType;
		while(typeToDoc != null){

			SetDocSource(typeToDoc);
			typePath.Add(typeToDoc);

			docClasses[typeToDoc] = GetDoc(typeToDoc);

			foreach (FieldInfo field in typeToDoc.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly))
				docFields[field] = GetDoc(field);

			foreach (PropertyInfo prop in typeToDoc.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly))
				docProps[prop] = GetDoc(prop);

			foreach (MethodInfo method in typeToDoc.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly)){

				if (method.IsSpecialName)
					continue;

				docMethods[method] = GetDoc(method);
			}

			//extension methods
			foreach (MethodInfo exMethod in GetExtensionMethods(typeToDoc)){
				SetDocSource(exMethod.DeclaringType);
				docMethods[exMethod] = GetDoc(exMethod);
			}

			typeToDoc = typeToDoc.BaseType;
		}

		targetScript = GetScriptForType(targetType);
		if (typePath.Count != 0)
			typePath.RemoveAt(0);

		Repaint();
	}

	//Get a list of methods that extend the provided type
	public List<MethodInfo> GetExtensionMethods(Type type){

		List<MethodInfo> exMethods = new List<MethodInfo>();
		foreach (KeyValuePair<string, List<Type> > assembliedType in assemblyTypes){

			foreach (Type t in assembliedType.Value){
				
				if (!t.IsSealed || t.IsGenericType || !t.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false))
					continue;

				foreach (MethodInfo m in t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)){
					
					if (!m.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false))
						continue;
					
					if (m.GetParameters()[0].ParameterType == type)
						exMethods.Add(m);
				}
			}
		}

		return exMethods;
	}

	//////////////////////////
	///////DOCUMENTING////////

	// Set the documentation source text from where the members will read based on the type
	public void SetDocSource(Type type){

		//basicly to avoid spamming web domains
		if (cachedSources.ContainsKey(type))
			return;


		lastParsedLine = 0;

		if (IsProject(type)){
			
			//whenever seting a source reset the last parsed line for when GetCustomDoc gets called
			cachedSources[type] = File.ReadAllLines( AssetDatabase.GetAssetPath(GetScriptForType(type)) );

		} else
		if (type.Namespace == "UnityEngine" || type.Namespace == "UnityEditor"){

			string resultingURL= "http://docs.unity3d.com/Documentation/ScriptReference/" + type.Name + ".html";

			Action<string> Callback = delegate(string sourceText){
				string tempFilePath= Application.dataPath + "/temp.txt";
				File.WriteAllText(tempFilePath, sourceText);
				cachedSources[type] = File.ReadAllLines(tempFilePath);
				File.Delete(tempFilePath);				
			};

			coroutiner.StartCoroutine( GetURLText(resultingURL, Callback) );
		}
	}

	//get the text from a url
	private System.Collections.IEnumerator GetURLText(string url, Action<string> callback){

		WWW www= new WWW(url);

		netPendings ++;
		while(!www.isDone)
			yield return null;
		netPendings --;

		if (string.IsNullOrEmpty(www.error)){

			callback(www.text);
			
			//when the last url is done set new view 
			if (netPendings == 0){
				yield return null;
				NewView(targetType);
			}

		} else {

			callback(null);
		}
	}

	//Returns the DocInfo for a member, null for no documentation
	public DocInfo GetDoc(MemberInfo member){

		Type type= (member.MemberType == MemberTypes.TypeInfo)? (member as Type) : member.DeclaringType;

		if (cachedDocs.ContainsKey(member))
			return cachedDocs[member];

		if (!cachedSources.ContainsKey(type))
			return null;

		if (IsProject(type)){
			cachedDocs[member] = GetCustomDoc(member);
			return cachedDocs[member];

		} else
		if (type.Namespace == "UnityEngine" || type.Namespace == "UnityEditor"){
			cachedDocs[member] = GetUnityDoc(member);
			return cachedDocs[member];
		}

		return null;
	}

	// Parsing unity web docs for documentation
	private DocInfo GetUnityDoc(MemberInfo member){

		bool isType = member.MemberType == MemberTypes.TypeInfo;
		Type type= isType? (member as Type) : member.DeclaringType;
		string[] lines= cachedSources[type];
		string seperationSymbol= (member.MemberType == MemberTypes.Method)? "." : "-";
		string doc = null;

		for (int i= lastParsedLine; i < lines.Length; i++){

			if (isType){
				break;
/*
				if (lines[i].Contains("<div class=\"script-section-softheading\">Description</div>")){
					doc = lines[i + 1];
					break;
				}
*/			
			} else if (!isType){

				string check = string.Format("{0}{1}{2}", member.DeclaringType.Name, seperationSymbol, member.Name);
				if (lines[i].Contains(check)){
					doc = lines[i];
					doc = doc.Replace("<tr><td class=\"lbl\"><a href=\"" + check + ".html\">" + member.Name + "</a></td><td class=\"desc\">", "");
					doc = doc.Replace("</td></tr>", "");
					break;
				}
			}
		}

		if (string.IsNullOrEmpty(doc))
			return null;

		var link = string.Empty;
		if (!isType){
			link = "http://docs.unity3d.com/Documentation/ScriptReference/" + type.Name + seperationSymbol + member.Name + ".html";
		} else {
			link = "http://docs.unity3d.com/Documentation/ScriptReference/" + member.Name + ".html";
		}

		return new DocInfo(doc, 0, link);
	}

	
	//Will parse a text based on the type and return a DocInfo
	private DocInfo GetCustomDoc(MemberInfo member){

		Type type= (member.MemberType == MemberTypes.TypeInfo)? (member as Type) : member.DeclaringType;

		//get the line of a member and set the last parsed line to it for the next member that will gets parsed
		int lineNumber= FindMemberLine(member, lastParsedLine);
		lastParsedLine = lineNumber;

		if (lineNumber <= 0)
			return null;

		string[] lines= cachedSources[type];
		bool commentFound = false;
		string doc = null;

		for (int i= lineNumber -2; true; i--){

			string currentLine= lines[i];

			if (i <= 0)
				break;

			if (currentLine.Trim().StartsWith("[") || currentLine.Trim().StartsWith("@"))
				continue;

			if (currentLine.Trim().StartsWith("//")){

				commentFound = true;
				currentLine = currentLine.Replace("/// ", "");
				currentLine = currentLine.Replace("<summary>", "");
				currentLine = currentLine.Replace("</summary>", "");
				currentLine = currentLine.Replace("//", "");
				currentLine = currentLine.Trim();

				if (currentLine.Trim() != string.Empty)
					doc = currentLine + (!string.IsNullOrEmpty(doc)? "\n" + doc : "");

				continue;
			}

			if (currentLine.Trim() != string.Empty)
				break;
			
			if (commentFound && currentLine.Trim() == string.Empty)
				break;
		}

		if (string.IsNullOrEmpty(doc))
			return null;

		return new DocInfo(doc, lineNumber);
	}



	//Finds the line at which a member is decalred, optionaly providing a line after which to search for it
	public int FindMemberLine(MemberInfo m){
		return FindMemberLine(m, 0);
	}

	public int FindMemberLine(MemberInfo m, int searchAfter){

		Type type= (m.MemberType == MemberTypes.TypeInfo)? (m as Type) : m.DeclaringType;
		string[] lines= cachedSources[type];
		
		//start at minus bracketing unless its a type
		int bracketing= (m.MemberType == MemberTypes.TypeInfo)? 0 : -1;
		
		//if a namespace is defined we need another bracket
		if (!string.IsNullOrEmpty(type.Namespace))
			bracketing --;

		//if the type of the member is nested we need one more bracket
		if (type.IsNested)
			bracketing --;

		for (int i= 0; i < lines.Length; i++){

			string line= lines[i];

			if (line.Length < 0)
				continue;

			if (line.Trim().StartsWith("//") || line.Trim().StartsWith("/*") || line.Trim().StartsWith("*/") || line.Trim().StartsWith("*"))
				continue;

			//when we are not within a bracketing, we are after the requested line (lastParsedLine),
			//have striped the junk string and the line contains the member name,
			//its like 99% correct find
			if (bracketing == 0 && i >= searchAfter){
				
				line = StripJunk(line);
				if (line.Contains(" " + m.Name))
					return i + 1;
			}

			//increase/decrease current bracketing
			if (line.Contains("{"))
				bracketing ++;
			if (line.Contains("}"))
				bracketing --;

		}

		// Debug.Log(m.Name);
		return 0;
	}

	//removes junk string from a line within enclosing brackets
	private string StripJunk(string text){

		//first check for enclosing brackets if exist in same line
		string s1= "{";
		string s2= "}";
		
		if (!text.Contains(s1) || !text.Contains(s2)){

			//if not check for parenthesys
			s1 = "(";
			s2 = ")";
		}

		//if not return the string
		if (!text.Contains(s1) || !text.Contains(s2))
			return text;

		//get the first and last indexes
		int index1= text.IndexOf(s1);
		int index2= text.LastIndexOf(s2);

		//if existant and indeed closing symbol is after the opening symbol, remove the stuff inbetween
		if (index1 >= 0 && index2 >= 0 && index2 > index1)
			text = text.Replace(text.Substring(index1, index2 - index1 + 1), "");

		return text;
	}

	//opens a script file at the declaration of the member provided
	private void OpenFileAt(MemberInfo m){
		Type type= (m.MemberType == MemberTypes.TypeInfo)? (m as Type) : m.DeclaringType;
		MonoScript script = GetScriptForType(type);
		if (!script){
			Debug.LogWarning(m.Name + " is not a project member");
			return;
		}
		AssetDatabase.OpenAsset(script, FindMemberLine(m));
	}

	///////DOCUMENTING////////
	//////////////////////////



	void OnGUI(){

		///
		GUI.skin.label.richText = true;
		projectListStyle.normal.background = GUI.skin.box.normal.background;
		///

		//should never happen!
		if (targetType == null){
			Debug.LogWarning("OOooops...Somehow the viewing type became null. Fallback to MonoBehaviour");
			NewView(typeof(MonoBehaviour));
			return;
		}

		scriptsListRect = new Rect(Screen.width - rightMargin, 0, rightMargin, Screen.height - 24);
		mainRect = new Rect(0, 0, Screen.width - rightMargin, Screen.height - 24);

		EditorGUILayout.Space();
		GUILayout.BeginArea(mainRect);

		if (!ShowTypeSearch()){

			ShowControls();
			ShowLogs();
			ShowClass();

			BoldSeparator();

			scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

			ShowFieldsList();
			ShowPropertiesList();
			ShowMethodsList();

			EditorGUILayout.EndScrollView();
		}

		GUILayout.EndArea();

		ShowProjectScripts();
	}

	//the search bar for types at the very top of the window
	private bool ShowTypeSearch(){

		EditorGUILayout.BeginHorizontal("Box");

		typeSearchString = EditorGUILayout.TextField(typeSearchString);

		if (GUILayout.Button("Search Type", GUILayout.Width(100)) && typeSearchString != string.Empty){
			typeSearchResults.Clear();
			foreach (KeyValuePair<string, List<Type> > a in assemblyTypes){
				foreach (Type type in a.Value){
					if (type.Name.ToLower().StartsWith(typeSearchString.ToLower()))
						typeSearchResults.Add(type);
				}
			}
		}

		EditorGUILayout.EndHorizontal();

		if (typeSearchResults.Count == 0)
			return false;

		GUI.backgroundColor = new Color(1.0f, 0.7f, 0.7f);
		if (GUILayout.Button("Clear Results")){
			typeSearchResults.Clear();
			return false;
		}
		GUI.backgroundColor = Color.white;

		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

		foreach (Type t in typeSearchResults.ToArray()){
			
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button(TypeName(t), projectListStyle)){
				typeSearchResults.Clear();
				NewView(t);
			}


			GUI.color = darkTheme? new Color(1f, 1f, 1f, 0.2f) : Color.white;
			GUILayout.Label(t.Assembly.GetName().Name, projectListStyle, GUILayout.Width(200));
			GUI.color = Color.white;
			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.EndScrollView();
		return true;
	}


	//the controls at the top, basicaly setting of filters
	private void ShowControls(){

		EditorGUILayout.BeginHorizontal();

		EditorGUILayout.BeginHorizontal("box");
		filterOnlyProject = EditorGUILayout.Toggle("Project Only", filterOnlyProject);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal("box");
		filterOnlyDeclared = EditorGUILayout.Toggle("Declared Only", filterOnlyDeclared);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal("box");
		filterOnlyPublic = EditorGUILayout.Toggle("Public Only", filterOnlyPublic);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal("box");
		filterOnlyDocumented = EditorGUILayout.Toggle("Documented Only", filterOnlyDocumented);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal("box");
		filterShowDocumentation = EditorGUILayout.Toggle("Show Documentation", filterShowDocumentation);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal("box");
		filterShowAttributes = EditorGUILayout.Toggle("Show Attributes", filterShowAttributes);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.EndHorizontal();
	}

	//some logs if there is need that shown at the top
	private void ShowLogs(){

		var color = darkTheme? "ea2828" : "000000";

		if (filterOnlyProject && !IsProject(targetType) )
			GUILayout.Label("<size=20><color=#" + color + "> Project Only Filer is On. This is not a Project Type </color></size>");

		if (netPendings != 0)
			GUILayout.Label("<size=15><color=#" + color +"> Fetching Documentation. Please Wait... </color></size>");
	}


	//show the class info at the top
	private void ShowClass(){

		//the inheritance path
		EditorGUILayout.BeginHorizontal("textfield");
			GUI.backgroundColor = new Color(1f, 1f, 1f, 0.3f);

			float alpha= 1;
			foreach (Type type in typePath.ToArray()){
				
				if (filterOnlyProject && !IsProject(type))
					continue;

				GUI.color = new Color(1f, 1f, 1f, alpha);
				if (GUILayout.Button("> " + TypeName(type), GUILayout.Height(15)))
					NewView(type);
				alpha -= 0.2f;
			}

			GUI.color = new Color(1f, 1f, 1f, 1f);
			GUI.backgroundColor = new Color(1f, 1f, 1f, 1f);
		EditorGUILayout.EndHorizontal();
		//

		EditorGUILayout.BeginHorizontal();

			//the back button
			if (backQueu.Count != 0 && GUILayout.Button("<", GUILayout.MaxHeight(95), GUILayout.Width(20)))
				GotoPrevious();

		EditorGUILayout.BeginVertical("Box");

			//show the class info
			GUILayout.Label("<size=20>" + TypePrefix(targetType) + "<b><color=#" + hexOrange + ">" + TypeName(targetType) + "</color></b><color=#" + hexBlue + ">" +
				(targetType.BaseType != null? Return(targetType.BaseType):"") + "</color></size>", GUILayout.Height(25));
			
			//show interfaces
			EditorGUILayout.BeginHorizontal();
				var interfaces = targetType.GetInterfaces();
				for (int i= 0; i < interfaces.Length; i++)
					LinkButton ((i != 0? ", " : "") + "<color=#" + hexBlue + ">" + TypeName(interfaces[i]) + "</color>", interfaces[i]);

				GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();

			//show attributes
			EditorGUILayout.BeginHorizontal();
			if (filterShowAttributes)
				ShowAttributesInfo(targetType);
			EditorGUILayout.EndHorizontal();

			//show assembly, NS and path if project type
			EditorGUILayout.BeginHorizontal();
				GUILayout.Label(targetType.Assembly.GetName().Name);
				GUILayout.FlexibleSpace();
				GUI.backgroundColor = new Color(1f, 1f, 1f, 0.3f);
				if (targetScript && GUILayout.Button(UnityEditor.AssetDatabase.GetAssetPath(targetScript)))
					Selection.activeObject = targetScript;
				GUI.backgroundColor = new Color(1f, 1f, 1f, 1f);
			EditorGUILayout.EndHorizontal();

			if (!string.IsNullOrEmpty(targetType.Namespace) )
				GUILayout.Label(targetType.Namespace);

			//show doc if abailable
			ShowDoc(docClasses[targetType]);

		EditorGUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();

		//search bar for members
		GUI.backgroundColor = filterString == string.Empty || !darkTheme? Color.white : Color.black;
		EditorGUILayout.BeginHorizontal("box");
		filterString = EditorGUILayout.TextField("Search Members", filterString);
		EditorGUILayout.EndHorizontal();
		GUI.backgroundColor = Color.white;
		//
	}


	//show the list of fields
	private void ShowFieldsList(){

		EditorGUILayout.BeginVertical("Box");
		GUI.color = showFields? new Color(1f,1f,1f,1f) : new Color(1f,1f,1f,0.5f);
		EditorGUILayout.BeginVertical("button", GUILayout.Height(30));
			if (GUILayout.Button(string.Format("<color=#" + hexDark + "><size=20> {0} FIELDS </size></color> {1}/{2}", showFields? "▼": "►", fieldViewCount, docFields.Keys.Count), new GUIStyle("label") ))
				showFields = !showFields;
			GUI.color = Color.white;
		EditorGUILayout.EndVertical();

		fieldViewCount = 0;
		if (showFields){

			bool endOfDeclared = false;
			//cache the dict for enumeratior
			foreach (KeyValuePair<FieldInfo, DocInfo> field in new Dictionary<FieldInfo, DocInfo>(docFields)){

				if (IsFiltered(field.Key))
					continue;

				if (!endOfDeclared && field.Key.DeclaringType != targetType){
					BoldSeparator();
					endOfDeclared = true;
					GUILayout.Label("<size=16><b>Inherited</b></size>");
				}

				ShowMember(field.Key, field.Value);
				fieldViewCount++;
				GUILayout.Space(5);
			}
		}

		EditorGUILayout.EndVertical();
	}

	//show the list of properties
	private void ShowPropertiesList(){

		EditorGUILayout.BeginVertical("Box");
		GUI.color = showProperties? Color.white : new Color(1f,1f,1f,0.5f);
		EditorGUILayout.BeginVertical("button", GUILayout.Height(30));
			if (GUILayout.Button(string.Format("<color=#" + hexDark + "><size=20> {0} PROPERTIES </size></color> {1}/{2}", showProperties? "▼": "►", propViewCount, docProps.Keys.Count), new GUIStyle("label") ))
			showProperties = !showProperties;
		GUI.color = Color.white;
		EditorGUILayout.EndVertical();

		propViewCount = 0;
		if (showProperties){

			bool endOfDeclared = false;
			//cache the dict for enumeratior
			foreach (KeyValuePair<PropertyInfo, DocInfo> prop in new Dictionary<PropertyInfo, DocInfo>(docProps)){

				if (IsFiltered(prop.Key))
					continue;

				if (!endOfDeclared && prop.Key.DeclaringType != targetType){
					BoldSeparator();
					endOfDeclared = true;
					GUILayout.Label("<size=16><b>Inherited</b></size>");
				}
				
				ShowMember(prop.Key, prop.Value);
				propViewCount++;
				GUILayout.Space(5);
			}
		}

		EditorGUILayout.EndVertical();
	}

	//show the list of methods
	private void ShowMethodsList(){

		EditorGUILayout.BeginVertical("Box");
		GUI.color = showMethods? Color.white : new Color(1f,1f,1f,0.5f);
		EditorGUILayout.BeginHorizontal("button", GUILayout.Height(30));
			if (GUILayout.Button(string.Format("<color=#" + hexDark + "><size=20> {0} METHODS </size></color> {1}/{2}", showMethods? "▼": "►", methodViewCount, docMethods.Keys.Count), new GUIStyle("label") ))
			showMethods = !showMethods;
		GUI.color = Color.white;
		EditorGUILayout.EndHorizontal();

		methodViewCount = 0;
		if (showMethods){

			bool endOfDeclared = false;
			//cache the dict for enumeratior
			foreach (KeyValuePair<MethodInfo, DocInfo> method in new Dictionary<MethodInfo, DocInfo>(docMethods)){

				if (IsFiltered(method.Key))
					continue;

				if (!endOfDeclared && method.Key.DeclaringType != targetType){
					BoldSeparator();
					endOfDeclared = true;
					GUILayout.Label("<size=16><b>Inherited</b></size>");
				}

				ShowMember(method.Key, method.Value);
				methodViewCount++;
				GUILayout.Space(5);
			}
		}

		EditorGUILayout.EndVertical();		
	}


	//determine if a member is filtered based on the selected options
	private bool IsFiltered(MemberInfo m){

		if (filterOnlyDeclared && m.DeclaringType != targetType)
			return true;

		if (filterOnlyProject && !IsProject(m.DeclaringType))
			return true;

		if (filterString != string.Empty && !m.Name.ToLower().StartsWith(filterString.ToLower()))
			return true;

		if (filterOnlyDocumented){

			if (m.MemberType == MemberTypes.Field && docFields[m as FieldInfo] == null)
				return true;
			
			if (m.MemberType == MemberTypes.Property && docProps[m as PropertyInfo] == null)
				return true;
			
			if (m.MemberType == MemberTypes.Method && docMethods[m as MethodInfo] == null)
				return true;
		}

		if (filterOnlyPublic){

			if (m.MemberType == MemberTypes.Field && !(m as FieldInfo).IsPublic)
				return true;

			if (m.MemberType == MemberTypes.Method && !(m as MethodInfo).IsPublic)
				return true;

			if (m.MemberType == MemberTypes.Property && ( (m as PropertyInfo).GetGetMethod() == null && (m as PropertyInfo).GetSetMethod() == null) )
				return true;
		}

		return false;
	}

	//shows the member entry
	private void ShowMember(MemberInfo m, DocInfo doc){

		bool isOverload= (m.Name == lastEntryName && (fieldViewCount != 0 || propViewCount != 0 || methodViewCount != 0));
		string implementedFrom = null;
		string overrideFrom = null;
		string extensionFrom = null;
		

		if (m.MemberType == MemberTypes.Method){
			
			MethodInfo baseDef= (m as MethodInfo).GetBaseDefinition();
			if (baseDef.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false))
				extensionFrom = TypeName(baseDef.DeclaringType);

			if (baseDef != m as MethodInfo)
				overrideFrom = TypeName(baseDef.DeclaringType);
		}

		foreach (Type t in targetType.GetInterfaces()){
			if ( m.Name.Contains(t.Name) ){
				implementedFrom = TypeName(t);
				break;
			}
		}

		if (isOverload)
			GUI.backgroundColor = new Color(1f,1f,1f,0.3f);

		EditorGUILayout.BeginVertical("Box");

			if (extensionFrom != null)
				GUILayout.Label("<b>Extension</b> " + extensionFrom);

			if (implementedFrom != null)
				GUILayout.Label("<b>Implemeted</b> " + implementedFrom);

			if (overrideFrom != null)
				GUILayout.Label("<b>Overrides</b> "  + overrideFrom);

			if (filterShowAttributes)
				ShowAttributesInfo(m);

			EditorGUILayout.BeginHorizontal();

				if (isOverload){
					GUILayout.Label("^", GUILayout.Width(18));
				} else
				if ( (doc == null || string.IsNullOrEmpty(doc.link) ) && !IsProject(m.DeclaringType)){
					GUILayout.Label(">", GUILayout.Width(18));
				}

				if (!isOverload){

					GUI.backgroundColor = new Color(1f,1f,1f,0.4f);
					if (IsProject(m.DeclaringType) && GUILayout.Button("S", GUILayout.Width(18)))
						OpenFileAt(m);
					if (doc != null && !string.IsNullOrEmpty(doc.link) && GUILayout.Button("?", GUILayout.Height(15), GUILayout.Width(18)) )
						Application.OpenURL(doc.link);					
					GUI.backgroundColor = Color.white;
				}

				if (m.MemberType == MemberTypes.Field){
					FieldInfo field= m as FieldInfo;
					ShowEntry( field.Name, field.FieldType, Scope(field));
				} else

				if (m.MemberType == MemberTypes.Property){
					PropertyInfo prop= m as PropertyInfo;
					ShowEntry( prop.Name, prop.PropertyType, Scope(prop));
				} else

				if (m.MemberType == MemberTypes.Method){
					MethodInfo method= m as MethodInfo;
					ShowEntry( method.Name, method.GetParameters().ToList(), method.ReturnType, Scope(method));
				}

				GUILayout.FlexibleSpace();

				ShowBaseTypeInfo(m);

			EditorGUILayout.EndHorizontal();
			GUILayout.Space(3);

			if (!isOverload && filterShowDocumentation)
				ShowDoc(doc);

		EditorGUILayout.EndVertical();

		lastEntryName = m.Name;
		GUI.backgroundColor = Color.white;
	}

	/////////////
	/////////////
	//show the declaring type of a member
	private void ShowBaseTypeInfo(MemberInfo m){

		if (m.DeclaringType != targetType){
			GUI.color = new Color(1f,1f,1f,0.3f);
			GUILayout.Label(TypeName(m.DeclaringType), new GUIStyle("textfield"), GUILayout.Width(100));
			GUI.color = Color.white;
		}
	}

	//show the attributes the member has
	private void ShowAttributesInfo(MemberInfo m){

		GUI.color = new Color(0.8f, 0.8f, 1f);

		Attribute[] attributes = m.GetCustomAttributes(true) as Attribute[];
		EditorGUILayout.BeginHorizontal();
		foreach (Attribute att in attributes){
			LinkButton("[" + att.GetType().Name + "]", att.GetType());
			GUILayout.Space(5);
		}
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUI.color = Color.white;
	}

	//show the doc if not null, meaning text and line link
	private void ShowDoc(DocInfo doc){

		if (doc != null){

			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal("textfield");
			GUI.color = darkTheme? Color.white : new Color(1,1,1, 0.7f);
			GUILayout.Label(doc.text);
			GUI.color = Color.white;
			GUILayout.FlexibleSpace();
			GUI.backgroundColor = new Color(1f,1f,1f,0.1f);

			if (doc.line != 0 && GUILayout.Button("line." + doc.line))
				AssetDatabase.OpenAsset(targetScript, doc.line);

			GUI.backgroundColor = Color.white;
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
		}
	}
	/////////////
	/////////////

	//for methods. Set clicked type instead of calling NewView!
	private void ShowEntry(string name, List<ParameterInfo> parameters, Type type, string scope){

		ParameterInfo param;
		float height= rich.CalcHeight(new GUIContent(name), Mathf.Infinity);

		EditorGUILayout.BeginHorizontal();

			//name
			GUILayout.Label(Name(name), rich, GUILayout.Height(height));

			//parameters
			for (int i= 0; i < parameters.Count; i++){
				param = parameters[i];

				LinkButton( (i == 0? " ( " : "") + Param(param.ParameterType), param.ParameterType);
				GUILayout.Label( param.Name + (i == parameters.Count -1? " ) " : ", "), rich, GUILayout.Height(height));
			}

			if (parameters.Count == 0)
				GUILayout.Label(" () ", rich, GUILayout.Height(height));				
			
			//return
			LinkButton(Return(type), type);

			//scope
			GUILayout.Label(Scope(scope), rich, GUILayout.Height(height));

		EditorGUILayout.EndHorizontal();

	}

	//for fields and properties. Set clicked type instead of calling NewView!
	private void ShowEntry(string name, Type type, string scope){

		float height= rich.CalcHeight(new GUIContent(name), Mathf.Infinity);

		EditorGUILayout.BeginHorizontal();

			//name
			GUILayout.Label(Name(name), rich, GUILayout.Height(height));

			//return
			LinkButton(Return(type), type);

			//scope
			GUILayout.Label(Scope(scope), rich, GUILayout.Height(height));

		EditorGUILayout.EndHorizontal();
	}

	//a button to use for showing a cursor when the fact that it is a button is not clear by graphics
	private bool LinkButton(string text, Type linkType){

		if (GUILayout.Button(text, rich)){
			NewView(linkType);
			return true;
		}
		EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
		return false;
	}
	
	//////
	//helper function to show stuff with correct text and colors
	private string Name(string s){
		return "<size=15><b><color=#" + hexOrange + ">" + Strip(s, ".") + "</color></b></size>";
	}

	private string Param(Type t){
		return "<color=#" + hexBlue + "> " + (t.IsByRef? "<i>ref</i> " : "") + TypeName(t) + "</color>";
	}

	private string Return(Type t){
		return "<color=#" + hexBlue + "> <b>:</b> " + TypeName(t) + "</color>";
	}

	//get the right text for a type name
	private string TypeName(Type t){

		string s= t.Name;
		
		if (t.IsGenericParameter)
			s = "T";

		if (t == typeof(bool))
			s = "Boolean";
		if (t == typeof(int))
			s = "Integer";
		if (t == typeof(float))
			s = "Float";

		if (t.IsGenericType){
			
			Type[] args= t.GetGenericArguments();
			
			if (args.Length != 0){
			
				s = s.Replace("`" + args.Length.ToString(), "");

				s += " <";
				for (int i= 0; i < args.Length; i++)
					s += (i == 0? "":", ") + TypeName(args[i]);
				s += "> ";
			}
		}

		//for Uscript 
		if (t.Assembly.GetName().Name.Contains("UnityScript") && t.BaseType == typeof(MulticastDelegate))
			s = "Function";

		return s;
	}

	//get correct text for prefix of a type
	private string TypePrefix(Type t){

		string s = string.Empty;
		if (t.IsAbstract)
			s += "Abstract ";
		if (t.IsAbstract && t.IsSealed)
			s = "Static ";
		if (t.IsNested)
			s += "Nested(" + TypeName(t.DeclaringType) + ") ";
		if (t.IsClass)
			s += "Class ";
		if (t.IsEnum)
			s += "Enum ";
		if (t.IsInterface)
			s += "Interface ";
		if (t.IsValueType)
			s += "Struct ";

		return s;
	}

	private string Scope(string s){
		return " | <color=#cb5d45>" + s + "</color> ";
	}

	//get the right text for a member's scope
	private string Scope(MemberInfo m){

		string finalString = string.Empty;
		
		if (m.MemberType == MemberTypes.Field){
			
			FieldInfo field= m as FieldInfo;

			if (field.IsPrivate)
				finalString += "Private ";
			if (field.IsPublic)
				finalString += "Public ";
			if (field.IsStatic)
				finalString += "Static ";
			
			finalString = string.IsNullOrEmpty(finalString)? "Protected" : finalString;

		} else
		if (m.MemberType == MemberTypes.Property){

			PropertyInfo prop= m as PropertyInfo;
			finalString = (prop.CanRead? "Get " : "") + (prop.CanWrite? "/ Set" : "");

		} else
		if (m.MemberType == MemberTypes.Method){

			MethodInfo method= m as MethodInfo;
			finalString = method.IsVirtual? "Virtual " : "Final ";

			if (method.IsPrivate)
				finalString += "Private ";
			if (method.IsPublic)
				finalString += "Public ";
			if (method.IsAbstract)
				finalString += "Abstract ";
			if (method.IsFamily)
				finalString += "Protected ";
			if (method.IsAssembly)
				finalString += "Internal ";
			if (method.IsStatic)
				finalString += "Static ";
		}

		return finalString;		
	}

	//strip anything before the last specified character
	private string Strip(string text, string before){

		int index= text.LastIndexOf(before);
		if (index >= 0)
			return text.Substring(index + 1);
		return text;
	}

	//////
	//////

	//the view on the right of the window
	private void ShowProjectScripts(){

		GUILayout.BeginArea(scriptsListRect);

		if (!showProjectView){

			GUI.backgroundColor = new Color(0.7f, 0.7f, 1f);
			if (GUILayout.Button("<", GUILayout.Width(18), GUILayout.Height(Screen.height)))
				showProjectView = true;
			GUI.backgroundColor = Color.white;
			GUILayout.EndArea();
			return;
		}

		TextAnchor lastAlign= GUI.skin.label.alignment;
		GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		GUI.Box(scriptsListRect, "", new GUIStyle("textfield") );
		
		EditorGUILayout.BeginVertical("textfield");

		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.BeginVertical();
			GUILayout.Label("<size=20><b><color=#" + hexOrange + ">Doc</color>Me</b></size>", GUILayout.Height(25));
			GUILayout.Label("<size=10>Gavalakis Vaggelis</size>");
			EditorGUILayout.EndVertical();

			if (GUILayout.Button(">", GUILayout.Width(18), GUILayout.Height(45)))
				showProjectView = false;
		EditorGUILayout.EndHorizontal();

		BoldSeparator();

		EditorGUILayout.Space();
		EditorGUILayout.BeginHorizontal("box");
		GUILayout.Label("Project View");
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space();
		filterProjectSciptString = EditorGUILayout.TextField(filterProjectSciptString, GUILayout.Width(155));
		EditorGUILayout.Space();

		GUI.skin.label.alignment = lastAlign;
		scrollPosProject = GUILayout.BeginScrollView(scrollPosProject);

		var typesToList = new List<Type>();
		if (filterProjectSciptString != string.Empty){

			foreach (Type type in projectTypes){
				
				if (!type.Name.ToLower().StartsWith(filterProjectSciptString.ToLower()))
					continue;

				typesToList.Add(type);
			}

		} else {

			if (hierarchySelected != null){
				GUI.backgroundColor = new Color(0.7f, 0.7f, 1f, 0.8f);
				if (GUILayout.Button( "^", GUILayout.Height(13)))
					hierarchySelected = typeNodes[hierarchySelected].parent;
				GUI.backgroundColor = Color.white;
				EditorGUILayout.Space();
			}

			if (hierarchySelected == null){

				GUILayout.Label("Project Base Classes");
				foreach (Type type in projectBaseTypes)
					typesToList.Add(type);
			
			} else {

				GUI.backgroundColor = new Color(0.5f, 1f, 0.5f);
				GUI.color = (hierarchySelected == targetType)? Color.white : new Color(1f,1f,1f,0.8f);
				if (GUILayout.Button(TypeName(hierarchySelected), projectListStyle, GUILayout.MaxWidth(rightMargin - 16)))
					NewView(hierarchySelected);

				GUI.backgroundColor = Color.white;
				EditorGUILayout.Space();

				foreach (Type child in typeNodes[hierarchySelected].childs)
					typesToList.Add(child);
			}
		}

		foreach (Type shownType in typesToList){

			EditorGUILayout.BeginHorizontal();
				GUI.color = (shownType == targetType)? Color.white : new Color(1f,1f,1f,0.5f);

				if (GUILayout.Button(TypeName(shownType), projectListStyle, GUILayout.MaxWidth(rightMargin - 40), GUILayout.ExpandWidth(true)))
					NewView(shownType);

				if (typeNodes[shownType].childs.Count != 0 && GUILayout.Button(typeNodes[shownType].childs.Count.ToString(), projectListStyle, GUILayout.Width(14))){
					filterProjectSciptString = string.Empty;
					hierarchySelected = shownType;
					NewView(shownType);
				}

				GUI.color = Color.white;
			EditorGUILayout.EndHorizontal();
		}

		GUI.color = Color.white;
		EditorGUILayout.EndVertical();
		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}

	//a bold separator for gui
	private void BoldSeparator(){

		if (tex == null)
			tex = new Texture2D(1,1);

		Rect lastRect= GUILayoutUtility.GetLastRect();

		GUILayout.Space(14);
		GUI.color = new Color(0f, 0f, 0f, 0.25f);
		GUI.DrawTexture(new Rect(0, lastRect.yMax + 6, Screen.width, 4), tex);
		GUI.DrawTexture(new Rect(0, lastRect.yMax + 6, Screen.width, 1), tex);
		GUI.DrawTexture(new Rect(0, lastRect.yMax + 9, Screen.width, 1), tex);
		GUI.color = Color.white;
	}


	// Gets all MonoScripts in project of the base type provided
	List<MonoScript> GetAllScriptsOfType(Type type){
		
		List<System.Object> allAssets = new List<System.Object>();
		List<MonoScript> projectScripts = new List<MonoScript>();

		foreach (string path in AssetDatabase.GetAllAssetPaths()){

			if (path.EndsWith(".js") || path.EndsWith(".cs") || path.EndsWith(".boo"))
				allAssets.Add(AssetDatabase.LoadAssetAtPath(path, typeof(MonoScript)));
		}

		foreach (MonoScript monoScript in allAssets){

			if (monoScript.GetClass() == null)
				continue;

			if (type.IsAssignableFrom(monoScript.GetClass()))
				projectScripts.Add(monoScript);
		}

		return projectScripts;
	}

	// Get the monoscript coresponding to a type either the main type implemented in the monoscript
	//or one of it's nested types
	MonoScript GetScriptForType(Type type){

		foreach (MonoScript monoScript in GetAllScriptsOfType(typeof(System.Object))){

			Type foundType= monoScript.GetClass();
			
			if (foundType == null)
				continue;
			
			if (foundType == type)
				return monoScript;

			foreach (Type nested in foundType.GetNestedTypes(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly) ){
				if (nested == type)
					return monoScript;
			}
		}

		return null;
	}



	[MenuItem("Window/DocMe")]
	static void ShowWindow(){

		EditorWindow window = EditorWindow.GetWindow(typeof(DocMe));
		window.Show();
	}
}
