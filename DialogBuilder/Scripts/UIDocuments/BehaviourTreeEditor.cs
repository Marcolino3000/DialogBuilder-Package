using DialogBuilder.Scripts.Tree;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogBuilder.Scripts.UIDocuments
{
    public class BehaviourTreeEditor : EditorWindow
    {
        // [SerializeField] private VisualTreeAsset m_VisualTreeAsset = default;

        private BehaviourTreeView _treeView;
        private InspectorView _inspectorView;
        private IMGUIContainer _blackboardView;

        private SerializedObject _treeObject;
        private SerializedProperty _blackboardProperty;
    
        private Vector3 _savedViewPosition;
        private Vector3 _savedViewScale = new(0.5f, 0.5f, 1);

        [MenuItem("DialogTreeEditor/Editor...")]
        public static void OpenWindow()
        {
            BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
            wnd.titleContent = new GUIContent("DialogTreeEditor");
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            if (Selection.activeObject is BehaviourTree)
            {
                OpenWindow();
                return true;
            }

            return false;
        }
    
        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;
        
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/DialogBuilder/Scripts/UIDocuments/BehaviourTreeEditor.uxml");
            visualTree.CloneTree(root);
        
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/DialogBuilder/Scripts/UIDocuments/BehaviourTreeEditor.uss");
            root.styleSheets.Add(styleSheet);

            _inspectorView = root.Q<InspectorView>();
        
            _treeView = root.Q<BehaviourTreeView>();
            _treeView.OnNodeSelected = OnNodeSelectionChanged;
            _treeView.focusable = true;
        
            _blackboardView = root.Q<IMGUIContainer>();
            _blackboardView.onGUIHandler = () =>
            {
                if (_blackboardProperty != null)
                {
                    _treeObject.Update(); 
                    EditorGUILayout.PropertyField(_blackboardProperty);
                    _treeObject.ApplyModifiedProperties();
                }
            };
        
            _treeView.UpdateViewTransform(_savedViewPosition, _savedViewScale);
        
            _treeView.viewTransformChanged += (g) =>
            {
                _savedViewPosition = g.viewTransform.position;
                _savedViewScale = g.viewTransform.scale;
            };
        
            OnSelectionChange();
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange stateChange)
        {
            switch (stateChange)
            {
                case PlayModeStateChange.EnteredEditMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
            }
        }

        private void OnSelectionChange()
        {
            BehaviourTree tree = Selection.activeObject as BehaviourTree;
        
            if(!tree)
            {
                if (Selection.activeObject)
                {
                    if (Selection.activeObject is not GameObject gameObject)
                        return;
                
                    DialogTreeRunner runner = gameObject.GetComponent<DialogTreeRunner>();
                    if (runner)
                    {
                        tree = runner.Tree;
                    }
                }
            }

            if (Application.isPlaying)
            {
                if(_treeView == null)
                {
                    return;
                }
            
                if (tree)
                {
                    _treeView.PopulateView(tree);
                }
            }

            else
            {
                if(_treeView == null)
                {
                    return;
                }
            
                if (tree&& AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
                {
                    _treeView.PopulateView(tree);
                }
            }

            if (tree != null)
            {
                _treeObject = new SerializedObject(tree);
                _blackboardProperty = _treeObject.FindProperty("Blackboard");
            }
        }

        private void OnNodeSelectionChanged(NodeView nodeView)
        {
            _inspectorView.UpdateSelection(nodeView);
        }

        private void OnInspectorUpdate()
        {
            // _treeView.UpdateNodeStates();
        }

        private void OnValidate()
        {
            if (_treeView == null)
                return;
        
            _treeView.UpdateNodeStates();
        }
    }
}
