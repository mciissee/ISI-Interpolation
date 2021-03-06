/*************************************************************************************************************************************
* Developed by Mamadou Cisse                                                                                                        *
* Mail => mciissee@gmail.com                                                                                                        *
* Twitter => http://www.twitter.com/IncMce                                                                                          *
* Unity Asset Store catalog: http://u3d.as/riS	                                                                                    *
*************************************************************************************************************************************/

namespace InfinityEditor
{

    using System.Collections.Generic;
    using InfinityEngine.Extensions;
    using InfinityEngine.Attributes;
    using UnityEditor;
    using System;
    using UnityEngine;
    using InfinityEngine.Utils;
    using InfinityEngine;
    using System.IO;
    using System.Xml;
    using System.Linq;
    using UnityEngine.SceneManagement;


    /// <summary>
    ///   Utils class
    /// </summary>
    public static class EditorUtils
    {

        #region Fields

        private static GUIStyle HelpBoxStyle;
        private static int go_count, components_count, error_count;

        private static GUIStyle separatorStyle;

        /// <summary>
        /// Sepator GUI Style
        /// </summary>
        public static GUIStyle SeparatorStyle
        {
            get
            {

                if (separatorStyle == null)
                {
                    separatorStyle = new GUIStyle();
                    separatorStyle.normal.background = EditorGUIUtility.FindTexture("AvatarBlendBackground");
                }
                return separatorStyle;
            }
        }

        /// <summary>
        /// The path where resources are generated by <c>InfinityEngine</c> plugins
        /// </summary>
        public const string GenResFolder = "Assets/InfinityEngine/Gen/Resources/";

        /// <summary>
        /// The path where scripts are generated by <c>InfinityEngine</c> plugins
        /// </summary>
        public const string GenScriptFolder = "Assets/InfinityEngine/Gen/Scripts/";

        /// <summary>
        /// The path where xml resources are generated by <c>ISILocalization</c> plugin
        /// </summary>
        public const string GenXmlFolder = "Assets/InfinityEngine/Gen/Xml/";

        #endregion Fields

        #region GUI Functions

        private static bool DrawHeader(bool enableHelp, bool showHelp, string docUrl)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUI.indentLevel++;

            if (GUILayout.Button(AssetReferences.Logo, GUI.skin.label, GUILayout.Width(64), GUILayout.Height(32)))
            {
                Application.OpenURL("http://u3d.as/riS");
            }

            EditorGUI.indentLevel--;

            if (enableHelp)
            {
                GUILayout.Space(10);
                if (GUILayout.Button(showHelp ? AssetReferences.HelpIconEnable : AssetReferences.HelpIconDisable, GUI.skin.label, GUILayout.Width(64), GUILayout.Height(32)))
                {
                    showHelp = !showHelp;
                }
            }
            GUILayout.Space(10);
            if (!string.IsNullOrEmpty(docUrl))
            {
                if (GUILayout.Button(AssetReferences.DocIcon, GUI.skin.label, GUILayout.Width(64), GUILayout.Height(32)))
                {
                    Application.OpenURL(docUrl);
                }
            }

            EditorGUILayout.EndHorizontal();
            return showHelp;
        }


        /// <summary>
        /// Displays header content in unity inspector.
        /// </summary>
        public static void DrawHeader()
        {
            DrawHeader(false, false, null);
        }

        /// <summary>
        /// Displays header content in unity inspector with a link to a documentation.
        /// </summary>
        /// <param name="docUrl">The link to the doc</param>
        public static void DrawHeader(string docUrl)
        {
            DrawHeader(false, false, docUrl);
        }

        /// <summary>
        /// Displays header content in unity inspector.
        /// </summary>
        /// <param name="showHelp">Is enable help or not ?</param>
        /// <returns>
        /// The reversed value of the parameter <c>showHelp</c> if the button help is clicked, 
        /// the value of the parameter otherwise.
        /// </returns>
        public static bool DrawHeader(bool showHelp)
        {
            return DrawHeader(true, showHelp, null);
        }

        /// <summary>
        /// Displays header content in unity inspector with a link to a doc.
        /// </summary>
        /// <param name="showHelp">Is enable help or not ?</param>
        /// <param name="docUrl">The link to the doc</param>
        /// <returns>
        /// The reversed value of the parameter <c>showHelp</c> if the button help is clicked, 
        /// the value of the parameter otherwise.
        /// </returns>
        public static bool DrawHeader(bool showHelp, string docUrl)
        {
            return DrawHeader(true, showHelp, docUrl);
        }

        /// <summary>
        /// Draws a drag and drop area and return <c>true</c> if there is an dragged object.
        /// </summary>
        /// <param name="width">The max witdh of the drag and drop area</param>
        /// <param name="height">The height of the drag and drop area</param>
        /// <param name="message">The message to display in the drag and drop area</param>
        /// <param name="onDragColor">Color of the drag and drop area when there is a dragged object</param>
        /// <returns><c>true</c> if there is an dragged object <c>false</c> otherwise.</returns>
        public static bool Drop(float width, float height, string message, Color onDragColor)
        {
            var dragArea = GUILayoutUtility.GetRect(0, 0, GUILayout.MaxWidth(width), GUILayout.Height(height));
            var evt = Event.current;

            var lastColor = GUI.color;
            GUI.color = dragArea.Contains(evt.mousePosition) ? onDragColor : lastColor;
            GUI.Box(dragArea, message);
            GUI.color = lastColor;

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dragArea.Contains(evt.mousePosition))
                        break;
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        return true;
                    }
                    break;
            }
            return false;
        }

        /// <summary>
        /// Draws a drag and drop area and return <c>true</c> if there is an dragged object.
        /// </summary>
        /// <param name="width">The witdh of the drag and drop area</param>
        /// <param name="height">The height of the drag and drop area</param>
        /// <param name="message">The message to display in the drag and drop area</param>
        /// <returns><c>true</c> if there is an dragged object <c>false</c> otherwise.</returns>
        public static bool Drop(float width, float height, string message)
        {
            return Drop(width, height, message, new Color(0, 0, 0, .1f));
        }

        /// <summary>
        /// Shows help message.
        /// </summary>
        /// <param name="message">The lessage to show.</param>
        /// <param name="type">The type of the message.</param>
        /// <param name="condition">The message will be displayed only if the value of this is set to <c>true</c>.</param>
        public static void ShowMessage(string message, MessageType type, bool condition)
        {
            if (condition)
            {
                var color = GUI.color;
                GUI.color = Color.white;
                EditorGUILayout.HelpBox(message, type);
                GUI.color = color;
            }
        }

        /// <summary>
        /// Shows help message.
        /// </summary>
        /// <param name="rect">The position of the message</param>
        /// <param name="message">The lessage to show.</param>
        /// <param name="type">The type of the message.</param>
        /// <param name="condition">The message will be displayed only if the value of this is set to <c>true</c>.</param>
        public static void ShowMessage(Rect rect, string message, MessageType type, bool condition)
        {
            if (condition)
            {
                var color = GUI.color;
                GUI.color = Color.white;
                EditorGUI.HelpBox(rect, message, type);
                GUI.color = color;
            }
        }

        /// <summary>
        /// Shows help message. (with rich text support)
        /// </summary>
        /// <param name="message">The lessage to show.</param>
        /// <param name="condition">The message will be displayed only if the value of this is set to <c>true</c>.</param>
        /// <param name="anchor">Message anchor position</param>
        public static void ShowMessage(string message, bool condition, TextAnchor anchor = TextAnchor.MiddleCenter)
        {
            if (condition)
            {
                if (HelpBoxStyle == null)
                {
                    HelpBoxStyle = GUI.skin.GetStyle("HelpBox");
                    HelpBoxStyle.alignment = anchor;
                    HelpBoxStyle.richText = true;
                }
                var color = GUI.color;
                GUI.color = Color.white;
                EditorGUILayout.LabelField(message, HelpBoxStyle);
                GUI.color = color;
            }
        }

        /// <summary>
        /// Shows help message.
        /// </summary>
        /// <param name="rect">The position of the message</param>
        /// <param name="message">The message to show.</param>
        /// <param name="condition">The message will be displayed only if the value of this is set to <c>true</c>.</param>
        public static void ShowMessage(Rect rect, string message, bool condition)
        {
            if (condition)
            {
                if (HelpBoxStyle == null)
                {
                    HelpBoxStyle = GUI.skin.GetStyle("HelpBox");
                    HelpBoxStyle.richText = true;
                }
                var color = GUI.color;
                GUI.color = Color.white;
                EditorGUI.LabelField(rect, message, HelpBoxStyle);
                GUI.color = color;
            }
        }

        public static Rect GetCenteredRect(int width, int height)
        {
            var rect = GUILayoutUtility.GetRect(0, 0, GUILayout.Width(width), GUILayout.Height(height));
            rect.center = new Vector2(EditorGUIUtility.currentViewWidth / 2, rect.center.y);
            return rect;
        }

        #endregion GUI Functions

        #region Scene Explorer

        public static void FindAllMissingComponents()
        {
            go_count = 0;
            components_count = 0;
            error_count = 0;
            var roots = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var root in roots)
            {
                FindMissing(root);
            }
            Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} missing", go_count, components_count, error_count));
        }

        private static void FindMissing(GameObject g)
        {
            FindMissingComponentInGameObject(g, typeof(Component), component => component == null, (name, type, index) =>

                string.Concat(name, " has an empty script attached in position: ", index)
            );
        }

        private static void FindMissingComponentInGameObject(GameObject gameObject, Type type, Func<Component, bool> predicate, Func<string, string, int, string> message)
        {
            go_count++;
            var components = gameObject.GetComponents(type);
            var len = components.Length;
            for (var i = 0; i < len; i++)
            {
                components_count++;
                if (predicate.Invoke(components[i]))
                {
                    error_count++;
                    var name = gameObject.name;
                    var transform = gameObject.transform;
                    while (transform.parent != null)
                    {
                        name = transform.parent.name + "/" + name;
                        transform = transform.parent;
                    }
                    Debug.Log(message.Invoke(name, type.Name, i), gameObject);
                }
            }
            var t = gameObject.transform;
            foreach (Transform child in t)
            {
                FindMissingComponentInGameObject(child.gameObject, type, predicate, message);
            }
        }

        private static void ValidateGameObject(GameObject gameObject)
        {
            go_count++;
            var components = gameObject.GetComponents<MonoBehaviour>();
            var len = components.Length;
            MonoBehaviour behaviour;

            for (var i = 0; i < len; i++)
            {
                behaviour = components[i];
                if (behaviour == null)
                {
                    continue;
                }
                components_count++;
                ValidateDontDrawAttributesOfComponent(behaviour);
                ValidateMessageAttributesOfComponent(behaviour);
                ValidateVisibleIfAttributesOfComponent(behaviour);
            }
            Transform t = gameObject.transform;
            foreach (Transform child in t)
            {
                ValidateGameObject(child.gameObject);
            }

        }

        private static void ValidateDontDrawAttributesOfComponent(MonoBehaviour behaviour)
        {

            var type = behaviour.GetType();
            DontDrawInspectorIfAttribute invalidAttribute;
            var message = string.Empty;
            if (DontDrawInspectorIfAttribute.TryFindInvalidAttribute(behaviour, out invalidAttribute))
            {
                error_count++;
                if (invalidAttribute.IsMissingFunction)
                {
                    message = string.Format("The attribute 'DontDrawInspectorIfAttribute'of the component {0}  refers to a missing method -> '{1}'", type.Name, invalidAttribute.MethodName);
                    Debug.LogError(message, behaviour);
                }
                else
                {
                    message = string.Format("The attribute 'DontDrawInspectorIfAttribute' of the component {0} has the error -> '{1}'", type.Name, invalidAttribute.Message);
                    Debug.LogError(message, behaviour);
                }
            }
        }

        private static void ValidateMessageAttributesOfComponent(MonoBehaviour behaviour)
        {
            var type = behaviour.GetType();
            MessageAttribute invalidAttribute;
            var message = string.Empty;
            var fields = ReflectionUtils.GetCachedFields(type);
            foreach (var field in fields)
            {
                if (MessageAttribute.TryFindInvalidAttribute(behaviour, field, out invalidAttribute))
                {
                    message = string.Format("The attribute 'MessageAttribute' of the field {0} of the component {1} has the error -> '{2}'", field, type.Name, invalidAttribute.Message);
                    Debug.LogError(message, behaviour);
                    error_count++;
                }
            }
        }

        private static void ValidateVisibleIfAttributesOfComponent(MonoBehaviour behaviour)
        {
            var type = behaviour.GetType();
            VisibleIfAttribute invalidAttribute;
            var message = string.Empty;
            var fields = ReflectionUtils.GetCachedFields(type);
            foreach (var field in fields)
            {
                if (VisibleIfAttribute.TryFindInvalidAttribute(behaviour, field, out invalidAttribute))
                {
                    message = string.Format("The attribute 'VisibleIf' of the field {0} of the component {1}  refers to a missing member -> '{2}'", field, type.Name, invalidAttribute.MemberName);
                    Debug.LogError(message, behaviour);
                    error_count++;
                }
            }
        }

        public static void ValidateSceneObjects()
        {
            go_count = 0;
            components_count = 0;
            error_count = 0;
            var roots = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var root in roots)
            {
                ValidateGameObject(root);
            }
            Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} ", go_count, components_count, error_count));
        }

        /// <summary>
        /// Find all components of the given types in the current scene.
        /// </summary>
        /// <param name="args">The type of the components to find</param>
        public static void FindAllComponentsOfType(params Type[] args)
        {
            go_count = 0;
            components_count = 0;
            error_count = 0;
            var roots = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var root in roots)
            {
                foreach (var type in args)
                {
                    FindMissingComponentInGameObject(root, type, component => true, (name, typeName, index) => string.Concat(name, " has an component of type ", typeName, " in position ", index));
                }
            }
            Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} ", go_count, components_count, error_count));
        }

        #endregion Scene Explorer

        #region Assets Explorer

        /// <summary>
        /// Checks InfinityEngine Gen folder an create it if it is not created
        /// </summary>
        public static void CheckGenFolder()
        {
            if (!Directory.Exists(GenResFolder))
                Directory.CreateDirectory(GenResFolder);

            if (!Directory.Exists(GenScriptFolder))
                Directory.CreateDirectory(GenScriptFolder);

            if (!Directory.Exists(GenXmlFolder))
                Directory.CreateDirectory(GenXmlFolder);
        }

        /// <summary>
        /// Iterates the nodes of the xml document.
        /// </summary>
        /// <param name="xml">The xml</param>
        /// <param name="action">The action do for each xmlnode</param>
        public static void IterateXML(string xml, Action<XmlNode> action)
        {
            IterateXML(xml, "resources", action);
        }

        /// <summary>
        /// Iterates the nodes of the xml document.
        /// </summary>
        /// <param name="xml">The xml</param>
        /// <param name="root">The name of the root node</param>
        /// <param name="action">The action do for each xmlnode</param>
        public static void IterateXML(string xml, string root, Action<XmlNode> action)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            var resources = doc[root];
            var children = resources.ChildNodes.Cast<XmlNode>().Where(n => n.NodeType != XmlNodeType.Comment);
            foreach (var node in children)
            {
                action.Invoke(node);
            }
        }

        /// <summary>
        /// Finds all assets of the type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type of the assets to find</typeparam>
        /// <returns>All assets of the type T</returns>
        public static List<T> FindAssetsByType<T>() where T : UnityEngine.Object
        {
            var assets = new List<T>();
            var typeName = typeof(T).ToString().Replace("UnityEngine.", "");
            var paths = AssetDatabase.FindAssets(string.Format("t:{0}", typeName))
                        .Select(AssetDatabase.GUIDToAssetPath)
                        .Distinct()
                        .ToArray();

            for (int i = 0; i < paths.Length; i++)
            {
                var asset = AssetDatabase.LoadAssetAtPath<T>(paths[i]);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }
            return assets;
        }

        /// <summary>
        /// Finds all asset paths of the given type
        /// </summary>
        /// <param name="type">The type of the assets to find</param>
        /// <returns>all asset paths of the given type</returns>
        public static string[] FindAssetPaths(Type type)
        {
            return AssetDatabase.FindAssets(string.Format("t:{0}", type.Name))
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Distinct()
                .ToArray();
        }

        #endregion Assets Explorer

        #region Editor Extensions

        /// <summary>
        /// Creates a texture with just a single color
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        public static Texture2D ColorToTex(Color col)
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(1, 1, col);
            tex.Apply();
            return tex;
        }
        public static Texture2D ColorToTex(Color col, int width, int height)
        {
            Texture2D tex = new Texture2D(width, height);
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    tex.SetPixel(x, y, col);
                }
            }
            tex.Apply();
            return tex;
        }

        public static Texture2D ColorToTex(string hexcol)
        {
            return ColorToTex(Infinity.HexToColor(hexcol));
        }
        public static Texture2D ColorToTex(string hexcol, int width, int height)
        {
            return ColorToTex(Infinity.HexToColor(hexcol), width, height);
        }

        /// <summary>
        /// Creates an array foldout like in inspectors for SerializedProperty of array type.
        /// Counterpart for standard EditorGUILayout.PropertyField which doesn't support SerializedProperty of array type.
        /// </summary>
        public static void ArrayField(SerializedProperty property)
        {
            bool wasEnabled = GUI.enabled;
            int prevIdentLevel = EditorGUI.indentLevel;

            // Iterate over all child properties of array
            bool childrenAreExpanded = true;
            int propertyStartingDepth = property.depth;
            while (property.NextVisible(childrenAreExpanded) && propertyStartingDepth < property.depth)
            {
                childrenAreExpanded = EditorGUILayout.PropertyField(property);
            }

            EditorGUI.indentLevel = prevIdentLevel;
            GUI.enabled = wasEnabled;
        }

        /// <summary>
        /// Creates a filepath textfield with a browse button. Opens the open file panel.
        /// </summary>
        public static string FileLabel(string name, float labelWidth, string path, string extension)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(name, GUILayout.MaxWidth(labelWidth));
            string filepath = EditorGUILayout.TextField(path);
            if (GUILayout.Button("Browse"))
            {
                filepath = EditorUtility.OpenFilePanel(name, path, extension);
            }
            EditorGUILayout.EndHorizontal();
            return filepath;
        }

        /// <summary>
        /// Creates a folder path textfield with a browse button. Opens the save folder panel.
        /// </summary>
        public static string FolderLabel(string name, float labelWidth, string path)
        {
            EditorGUILayout.BeginHorizontal();
            string filepath = EditorGUILayout.TextField(name, path);
            if (GUILayout.Button("Browse", GUILayout.MaxWidth(labelWidth)))
            {
                filepath = EditorUtility.SaveFolderPanel(name, path, "Folder");
            }
            EditorGUILayout.EndHorizontal();
            return filepath;
        }

        /// <summary>
        /// Creates an array foldout like in inspectors. Hand editable ftw!
        /// </summary>
        public static string[] ArrayFoldout(string label, string[] array, ref bool foldout)
        {
            EditorGUILayout.BeginVertical();
            foldout = EditorGUILayout.Foldout(foldout, label);
            string[] newArray = array;
            if (foldout)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical();
                int arraySize = EditorGUILayout.IntField("Size", array.Length);
                if (arraySize != array.Length)
                    newArray = new string[arraySize];

                var entry = string.Empty;
                for (int i = 0; i < arraySize; i++)
                {
                    entry = string.Empty;
                    if (i < array.Length)
                        entry = array[i];
                    newArray[i] = EditorGUILayout.TextField("Element " + i, entry);
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            return newArray;
        }

        /// <summary>
        /// Creates a toolbar that is filled in from an Enum. Useful for setting tool modes.
        /// </summary>
        public static Enum EnumToolbar(Enum selected)
        {
            string[] toolbar = System.Enum.GetNames(selected.GetType());
            Array values = System.Enum.GetValues(selected.GetType());

            for (int i = 0; i < toolbar.Length; i++)
            {
                string toolname = toolbar[i];
                toolname = toolname.Replace("_", " ");
                toolbar[i] = toolname;
            }

            int selected_index = 0;
            while (selected_index < values.Length)
            {
                if (selected.ToString() == values.GetValue(selected_index).ToString())
                {
                    break;
                }
                selected_index++;
            }
            selected_index = GUILayout.Toolbar(selected_index, toolbar);
            return (Enum)values.GetValue(selected_index);
        }

        /// <summary>
        /// Creates a button that can be toggled. Looks nice than GUI.toggle
        /// </summary>
        /// <returns>
        /// Toggle state
        /// </returns>
        /// <param name='state'>
        /// If set to <c>true</c> state.
        /// </param>
        /// <param name='label'>
        /// If set to <c>true</c> label.
        /// </param>
        public static bool ToggleButton(bool state, string label)
        {
            BuildStyle();

            bool out_bool = false;

            if (state)
                out_bool = GUILayout.Button(label, toggled_style);
            else
                out_bool = GUILayout.Button(label);

            if (out_bool)
                return !state;
            else
                return state;
        }

        static GUIStyle toggled_style;
        public static GUIStyle StyleButtonToggled
        {
            get
            {
                BuildStyle();
                return toggled_style;
            }
        }


        private static void BuildStyle()
        {
            if (toggled_style == null)
            {
                toggled_style = new GUIStyle(GUI.skin.button);
                toggled_style.normal.background = toggled_style.onActive.background;
                toggled_style.normal.textColor = toggled_style.onActive.textColor;
            }


        }
        #endregion Editor Extensions

    }

}