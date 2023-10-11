using System;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ProjectVS
{
    [CreateAssetMenu(fileName = "Input Handler Code Generator", menuName = "Code Generator/Input Handler")]
    public class InputHandlerCodeGenerator : ScriptableObject
    {
        [SerializeField] InputActionAsset asset;

        public void Generate()
        {
            try
            {
                string sourceCode ="";
                string inputActionMapProperty = "";
                string inputActionMapField = "";
                string inputActionMapFieldInstance = "";
                string inputActionMapAddInputLayer = "";
                string inputActionMapRemoveInputLayer = "";
                string inputActionMapClass = "";
                foreach (var actionMap in asset.actionMaps)
                {
                    inputActionMapProperty += string.Format(inputActionMapPropertyFormat, actionMap.name);
                    inputActionMapField += string.Format(inputActionMapFieldFormat, actionMap.name);
                    inputActionMapFieldInstance += string.Format(inputActionMapFieldInstanceFormat, actionMap.name);
                    inputActionMapAddInputLayer += string.Format(addInputLayerFormat, actionMap.name);
                    inputActionMapRemoveInputLayer += string.Format(removeInputLayerFormat, actionMap.name);
                    
                    string inputActionField = "";
                    string inputActionFieldInstance = "";
                    string inputActionAddInputLayer = "";
                    string inputActionRemoveInputLayer = "";
                    foreach (var action in actionMap.actions)
                    {
                        inputActionField += string.Format(inputActionFieldFormat, action.name);
                        inputActionFieldInstance += string.Format(inputActionFieldInstanceFormat, action.name);
                        inputActionAddInputLayer += string.Format(addInputLayerFormat, action.name);
                        inputActionRemoveInputLayer += string.Format(removeInputLayerFormat, action.name);
                    }

                    inputActionMapClass += string.Format(inputActionMapClassFormat,
                        actionMap.name,
                        inputActionField,
                        inputActionFieldInstance,
                        inputActionAddInputLayer,
                        inputActionRemoveInputLayer);
                }

                sourceCode = string.Format(sourceFormat,
                    inputActionMapProperty,
                    inputActionMapField,
                    inputActionMapFieldInstance,
                    inputActionMapAddInputLayer,
                    inputActionMapRemoveInputLayer,
                    inputActionMapClass);

                using (StreamWriter sw = new StreamWriter("Assets/ProjectVS/Scripts/Input/InputHandler.cs"))
                {
                    sw.Write(sourceCode);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        const string inputActionMapPropertyFormat =
@"public InputActionMap_{0} {0} => inputActionMaps.{0};" + "\r\n";

        const string inputActionMapFieldFormat =
@"public InputActionMap_{0} {0};" + "\r\n";

        const string inputActionMapFieldInstanceFormat =
@"{0} = new InputActionMap_{0}(asset.FindActionMap(""{0}"", throwIfNotFound: true));" + "\r\n";

        const string addInputLayerFormat =
@"{0}.AddInputLayer();" + "\r\n";

        const string removeInputLayerFormat =
@"{0}.RemoveInputLayer();" + "\r\n";

        const string inputActionFieldFormat =
@"public InputAction {0};" + "\r\n";

        const string inputActionFieldInstanceFormat =
@"{0} = new InputAction(actionMap.FindAction(""{0}"", throwIfNotFound: true));" + "\r\n";

        /// <summary>
        /// 0: 액션맵 이름 <br/>
        /// 1: inputActionFieldFormat <br/>
        /// 2: inputActionFieldInstanceFormat <br/>
        /// 3: addInputLayerFormat <br/>
        /// 4: removeInputLayerFormat
        /// </summary>
        const string inputActionMapClassFormat =
@"public class InputActionMap_{0}
        {{
            {1}

            public InputActionMap_{0}(InputActionMap actionMap)
            {{
                {2}
            }}

            public void AddInputLayer()
            {{
                {3}
            }}

            public void RemoveInputLayer()
            {{
                {4}
            }}
        }}";


        /// <summary>
        /// 0: inputActionMapPropertyFormat <br/>
        /// 1: inputActionMapFieldFormat <br/>
        /// 2: inputActionMapFieldInstanceFormat <br/>
        /// 3: addInputLayerFormat <br/>
        /// 4: removeInputLayerFormat <br/>
        /// 5: inputActionMapClassFormat
        /// </summary>
        const string sourceFormat =
@"using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace ProjectVS
{{
    using CallbackContext = InputAction.CallbackContext;

    // Input Handler Code Generator에 의해 생성됨
    public class InputHandler
    {{
        static InputHandler instance = null;
        public static InputHandler Instance
        {{
            get
            {{
                if (null == instance)
                {{
                    instance = new InputHandler();
                    instance.Init();
                }}
                return instance;
            }}
        }}

        static InputActionAsset asset;

        InputActionMaps inputActionMaps;

        {0}

        InputHandler() {{ }}

        void Init()
        {{
            asset = ResourceManager.Load<InputActionAsset>(""Input/InputActions"");
            asset.Enable();
            instance.inputActionMaps = new InputActionMaps(asset);
        }}

        public void AddInputLayer()
        {{
            inputActionMaps.AddInputLayer();
        }}

        public void RemoveInputLayer()
        {{
            inputActionMaps.RemoveInputLayer();
        }}

        public class InputActionMaps
        {{
            {1}

            public InputActionMaps(InputActionAsset asset)
            {{
                {2}
            }}

            public void AddInputLayer()
            {{
                {3}
            }}

            public void RemoveInputLayer()
            {{
                {4}
            }}
        }}

        {5}

        public class InputAction
        {{
            UnityEngine.InputSystem.InputAction action;

            InputLayers inputLayers;

            public Action onEnabled;
            public Action onDisabled;
            public Action onAddInputLayer;
            public Action onRemoveInputLayer;

            public bool enabled => action.enabled;

            public InputAction(UnityEngine.InputSystem.InputAction action)
            {{
                this.action = action;
                inputLayers = new InputLayers();
                onEnabled = null;
                onDisabled = null;
                onAddInputLayer = null;
                onRemoveInputLayer = null;
            }}

            public event Action<CallbackContext> started
            {{
                add
                {{
                    inputLayers.Peek()._started.Add(value);
                    action.started += value;
                }}
                remove
                {{
                    inputLayers.Peek()._started.Remove(value);
                    action.started -= value;
                }}
            }}

            public event Action<CallbackContext> performed
            {{
                add
                {{
                    inputLayers.Peek()._performed.Add(value);
                    action.performed += value;
                }}
                remove
                {{
                    inputLayers.Peek()._performed.Remove(value);
                    action.performed -= value;
                }}
            }}

            public event Action<CallbackContext> canceled
            {{
                add
                {{
                    inputLayers.Peek()._canceled.Add(value);
                    action.canceled += value;
                }}
                remove
                {{
                    inputLayers.Peek()._canceled.Remove(value);
                    action.canceled -= value;
                }}
            }}

            void Restore()
            {{
                foreach (var callback in inputLayers.Peek()._started)
                {{
                    action.started += callback;
                }}

                foreach (var callback in inputLayers.Peek()._performed)
                {{
                    action.performed += callback;
                }}

                foreach (var callback in inputLayers.Peek()._canceled)
                {{
                    action.canceled += callback;
                }}
            }}

            void Clear()
            {{
                foreach (var callback in inputLayers.Peek()._started)
                {{
                    action.started -= callback;
                }}

                foreach (var callback in inputLayers.Peek()._performed)
                {{
                    action.performed -= callback;
                }}

                foreach (var callback in inputLayers.Peek()._canceled)
                {{
                    action.canceled -= callback;
                }}
            }}

            public void AddInputLayer()
            {{
                Clear();
                inputLayers.AddLayer();
                onAddInputLayer?.Invoke();
            }}

            public void RemoveInputLayer()
            {{
                Clear();
                inputLayers.RemoveLayer();
                onRemoveInputLayer?.Invoke();
                Restore();
            }}

            public void Enable()
            {{
                onEnabled?.Invoke();
                action.Enable();
            }}

            public void Disable()
            {{
                onDisabled?.Invoke();
                action.Disable();
            }}
        }}

        public class InputLayers
        {{
            List<InputLayer> layers;
            int cur;

            public InputLayer Peek() => layers[cur];
            public InputLayer AddLayer()
            {{
                ++cur;
                if (layers.Count == cur)
                {{
                    layers.Add(new InputLayer());
                }}
                return layers[cur];
            }}

            public InputLayer RemoveLayer()
            {{
                layers[cur--].Clear();
                return layers[cur];
            }}

            public InputLayers()
            {{
                layers = new List<InputLayer> {{ new InputLayer() }};
                cur = 0;
            }}
        }}

        public class InputLayer
        {{
            public List<Action<CallbackContext>> _started = new List<Action<CallbackContext>>();
            public List<Action<CallbackContext>> _performed = new List<Action<CallbackContext>>();
            public List<Action<CallbackContext>> _canceled = new List<Action<CallbackContext>>();

            public void Clear()
            {{
                _started.Clear();
                _performed.Clear();
                _canceled.Clear();
            }}
        }}
    }}
}}";
    }
}
