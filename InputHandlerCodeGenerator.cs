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
                string sourceCode = "";
                string property = "";
                string inputlayer = "";

                string actionMapField = "";
                string actionMapInstance = "";

                string excuteRestoreMethodInInputLayer = "";
                string excuteClearMethodInInputLayer = "";

                string inputActionMap = "";

                foreach (var actionMap in asset.actionMaps)
                {
                    actionMapField += string.Format(inputActionMapFieldFormat, actionMap.name);
                    actionMapInstance += string.Format(newActionMapInstanceFormat, actionMap.name);

                    excuteRestoreMethodInInputLayer += string.Format(excuteRestoreMethodFormat, actionMap.name);
                    excuteClearMethodInInputLayer += string.Format(excuteClearMethodFormat, actionMap.name);

                    string inputActionField = "";
                    string newActionInstance = "";
                    string excuteRestoreMethodInInputActionMap = "";
                    string excuteClearMethodInInputActionMap = "";
                    foreach (var action in actionMap.actions)
                    {
                        inputActionField += string.Format(inputActionFieldFormat, action.name);
                        newActionInstance += string.Format(newActionInstanceFormat, action.name);
                        excuteRestoreMethodInInputActionMap += string.Format(excuteRestoreMethodFormat, action.name);
                        excuteClearMethodInInputActionMap += string.Format(excuteClearMethodFormat, action.name);

                        property += string.Format(inputActionPropertyFormat, action.name, actionMap.name);
                    }
                    string restoreMethod = string.Format(methodFormat, restoreMethodName, excuteRestoreMethodInInputActionMap);
                    string clearMethod = string.Format(methodFormat, clearMethodName, excuteClearMethodInInputActionMap);

                    inputActionMap += string.Format(inputActionMapFormat,
                        actionMap.name,
                        inputActionField,
                        newActionInstance,
                        restoreMethod,
                        clearMethod);
                }

                excuteRestoreMethodInInputLayer = string.Format(methodFormat, restoreMethodName, excuteRestoreMethodInInputLayer);
                excuteClearMethodInInputLayer = string.Format(methodFormat, clearMethodName, excuteClearMethodInInputLayer);

                inputlayer = string.Format(inputLayerFormat,
                    actionMapField,
                    actionMapInstance,
                    excuteRestoreMethodInInputLayer,
                    excuteClearMethodInInputLayer);

                sourceCode = string.Format(sourceFormat,
                    property,
                    inputlayer,
                    inputActionMap);

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

        static string restoreMethodName = "Restore";
        static string clearMethodName = "Clear";
        static string excuteMethodName = "ExecutePrivateMethod";

        /// <summary>
        /// {0} 메서드 이름 <br/>
        /// {1} 내용 <br/>
        /// </summary>
        static string methodFormat =
@"public void {0}()
{{
    {1}
}}
";
        /// <summary>
        /// {0} object
        /// </summary>
        static string excuteRestoreMethodFormat =
$@"{excuteMethodName}(""{restoreMethodName}"", BindingFlags.Instance | BindingFlags.NonPublic, {{0}}, null);
";

        /// <summary>
        /// {0} object
        /// </summary>
        static string excuteClearMethodFormat =
$@"{excuteMethodName}(""{clearMethodName}"", BindingFlags.Instance | BindingFlags.NonPublic, {{0}}, null);
";


        /// <summary>
        /// {0} inputActionPropertyFormat <br/>
        /// {1} inputLayerFormat <br/>
        /// {2} inputActionMapFormat <br/>
        /// </summary>
        static string sourceFormat =
$@"using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.InputSystem;

namespace ProjectVS
{{{{
    using CallbackContext = InputAction.CallbackContext;

    // Input Handler Code Generator에 의해 생성됨 Resources/ScriptableObject
    public class InputHandler
    {{{{
        static bool init = false;
        static InputActionAsset asset;

        static Stack<InputLayer> inputActionMaps;

        InputLayer input => inputActionMaps.Peek();
        
        {{0}}

        public InputHandler()
        {{{{
            if (init is false) Init();
        }}}}

        void Init()
        {{{{
            init = true;
            asset = ResourceManager.Load<InputActionAsset>(""Input/InputActions"");
            asset.Enable();
            inputActionMaps = new Stack<InputLayer>();
            inputActionMaps.Push(new InputLayer(asset));
        }}}}

        public void AddInputLayer()
        {{{{
            input.{clearMethodName}();
            inputActionMaps.Push(new InputLayer(asset));
        }}}}

        public void RemoveInputLayer()
        {{{{
            input.{clearMethodName}();
            if (0 == inputActionMaps.Count) return;
            inputActionMaps.Pop();
            input.{restoreMethodName}();
        }}}}

        {{1}}

        {{2}}

        public struct InputAction
        {{{{
            UnityEngine.InputSystem.InputAction action;

            List<Action<CallbackContext>> _started;
            List<Action<CallbackContext>> _performed;
            List<Action<CallbackContext>> _canceled;

            public bool enabled => action.enabled;

            public InputAction(UnityEngine.InputSystem.InputAction action)
            {{{{
                this.action = action;
                _started = new List<Action<CallbackContext>>();
                _performed = new List<Action<CallbackContext>>();
                _canceled = new List<Action<CallbackContext>>();
            }}}}

            public event Action<CallbackContext> started
            {{{{
                add
                {{{{
                    _started.Add(value);
                    action.started += value;
                }}}}
                remove
                {{{{
                    _started.Remove(value);
                    action.started -= value;
                }}}}
            }}}}

            public event Action<CallbackContext> performed
            {{{{
                add
                {{{{
                    _performed.Add(value);
                    action.performed += value;
                }}}}
                remove
                {{{{
                    _performed.Remove(value);
                    action.performed -= value;
                }}}}
            }}}}

            public event Action<CallbackContext> canceled
            {{{{
                add
                {{{{
                    _canceled.Add(value);
                    action.canceled += value;
                }}}}
                remove
                {{{{
                    _canceled.Remove(value);
                    action.canceled -= value;
                }}}}
            }}}}

            void {restoreMethodName}()
            {{{{
                foreach (var callback in _started)
                {{{{
                    action.started += callback;
                }}}}

                foreach (var callback in _performed)
                {{{{
                    action.performed += callback;
                }}}}

                foreach (var callback in _canceled)
                {{{{
                    action.canceled += callback;
                }}}}
            }}}}

            void {clearMethodName}()
            {{{{
                foreach (var callback in _started)
                {{{{
                    action.started -= callback;
                }}}}

                foreach (var callback in _performed)
                {{{{
                    action.performed -= callback;
                }}}}

                foreach (var callback in _canceled)
                {{{{
                    action.canceled -= callback;
                }}}}
            }}}}

            public void Enable() => action.Enable();
            public void Disable() => action.Disable();
        }}}}

        static bool {excuteMethodName}(string name, BindingFlags flags, object obj, object[] parameters)
        {{{{
            try
            {{{{
                Type type = obj.GetType();
                MethodInfo info = type.GetMethod(name, flags);
                info.Invoke(obj, parameters);
            }}}}
            catch (Exception)
            {{{{
                return false;
            }}}}

            return true;
        }}}}
    }}}}
}}}}";

        /// <summary>
        /// {0} Action 이름 <br/>
        /// {1} ActionMap 이름 <br/>
        /// </summary>
        static string inputActionPropertyFormat =
@"public InputAction {0} => input.{1}.{0};
";

        /// <summary>
        /// {0} inputActionMapFieldFormat <br/>
        /// {1} newActionMapInstance <br/>
        /// {2} excuteMethodFormat Restore <br/>
        /// {3} excuteMethodFormat Clear <br/>
        /// </summary>
        static string inputLayerFormat =
@"struct InputLayer
{{
    {0}

    public InputLayer(InputActionAsset asset)
    {{
        {1}
    }}

    {2}

    {3}
}}
";

        /// <summary>
        /// {0} ActionMap 이름
        /// </summary>
        static string inputActionMapFieldFormat =
@"public InputActionMap_{0} {0};
";

        /// <summary>
        /// {0} ActionMap 이름
        /// </summary>
        static string newActionMapInstanceFormat =
@"{0} = new InputActionMap_{0}(asset.FindActionMap(""{0}"", throwIfNotFound: true));
";

        /// <summary>
        /// {0} ActionMap 이름 <br/>
        /// {1} inputActionFieldFormat <br/>
        /// {2} newActionInstanceFormat <br/>
        /// {3} excuteMethodFormat Restore <br/>
        /// {4} excuteMethodFormat Clear <br/>
        /// </summary>
        static string inputActionMapFormat =
@"struct InputActionMap_{0}
{{
    {1}

    public InputActionMap_{0}(InputActionMap actionMap)
    {{
        {2}
    }}

    {3}

    {4}
}}
";

        /// <summary>
        /// {0} Action 이름
        /// </summary>
        static string inputActionFieldFormat =
@"public InputAction {0};
";

        /// <summary>
        /// {0} Action 이름
        /// </summary>
        static string newActionInstanceFormat =
@"{0} = new InputAction(actionMap.FindAction(""{0}"", throwIfNotFound: true));
";
    }
}
