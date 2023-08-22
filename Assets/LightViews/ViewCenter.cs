using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets
{
    public class ViewCenter : ScriptableObject
    {
        private const bool _doesLogging = false;

        private static ViewCenter _instance;

        private static Dictionary<string, ViewModel> _viewModels;

        //
        //  SUBSYSTEM INITIALIZATION
        //

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            _instance = ScriptableObject.CreateInstance<ViewCenter>();

            SceneManager.sceneLoaded += OnSceneLoadedSearchForViews;
            Application.quitting += OnQuit;

#if UNITY_EDITOR
            var isEnterPlayModeOptionsEnabled = UnityEditor.EditorSettings.enterPlayModeOptionsEnabled;

            if(isEnterPlayModeOptionsEnabled) { 
                var isReloadSceneDisabled = UnityEditor.EditorSettings.enterPlayModeOptions.HasFlag(EnterPlayModeOptions.DisableSceneReload);

                if(isReloadSceneDisabled) SearchForViews();
            }
#endif
        }

        private static void OnQuit()
        {
            Log("Deattached searching!");

            SceneManager.sceneLoaded -= OnSceneLoadedSearchForViews;
            Application.quitting -= OnQuit;
        }

        //
        //  OBJECT INITIALIZATION
        //

        private static void OnSceneLoadedSearchForViews(Scene scene, LoadSceneMode mode) => SearchForViews();

        private static void SearchForViews()
        {
            Log("Searching for views!");

            _viewModels = new Dictionary<string, ViewModel>();

            var views = UnityEngine.Object.FindObjectsOfType<View>(true);
            Log("\tFounded: " + views.Count());

            foreach(var view in views)
            {
                var type = view.GetType();
                var typeName = Erase(type.Name);
                var viewViewModel = ((PropertyInfo)type.GetMember("ViewModel").FirstOrDefault()).GetValue(view);
                Log("\t\tView: " + typeName + "; ViewModel: " + viewViewModel.GetType().Name);

                if (viewViewModel != null && viewViewModel is ViewModel viewModel) {
                    _viewModels.Add(typeName, viewModel);
                    BindViewViewModel(view, viewModel);
                }
            }
        }

        private static void BindViewViewModel(View view, ViewModel viewModel)
        {
            var viewModelType = viewModel.GetType();
            var viewModelBindingMethod = viewModelType.GetMethod("BindView");

            viewModelBindingMethod.Invoke(viewModel, new[] { view });

            Log("\t\t\tBinding " + viewModelType.Name + " to " + view.GetType().Name);
        }

        private static void Log(string message, bool withSave = true)
        {
#pragma warning disable CS0162 // Unreachable code detected
            if (_doesLogging) Debug.Log(message);
#pragma warning restore CS0162 // Unreachable code detected
        }

        //
        //  ViewModels interactions
        //

        public static ViewModel GetViewModel(string name)
        {
            var result = _viewModels.TryGetValue(name, out var viewModel);

            if (result == false) throw new Exception("View not found: " + name);

            return viewModel;
        }

        public static T1 GetViewModel<T1>()
            where T1 : ViewModel
        {
            var name = Erase(typeof(T1).Name);

            var result = _viewModels.TryGetValue(name, out var viewModel);

            if (result == false) throw new Exception("View not found: " + name);
            if (viewModel is not T1) throw new Exception("Failured to convert view to specified type: " + name);

            return viewModel as T1;
        }

        //
        //  View and ViewModel auto name erasers
        //

        private static string Erase(string typeName)
        {
            if (typeName.EndsWith("View")) return typeName.Substring(0, typeName.Length - "View".Length);
            else if (typeName.EndsWith("ViewModel")) return typeName.Substring(0, typeName.Length - "ViewModel".Length);
            else if (typeName.EndsWith("Controller")) return typeName.Substring(0, typeName.Length - "Controller".Length);

            return typeName;
        }
    }
}