using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEditor;
using UnityEditorInternal;
using AlePostProcessUPR;

namespace AlePostProcessUPR.Editor
{
    /// <summary>
    /// A custom property drawer for the settings for custom post-processing feature
    /// </summary>
    [CustomPropertyDrawer(typeof(CustomPostProcess.CustomPostProcessSettings), true)]
    internal class CustomPostProcessSettingsEditor : PropertyDrawer 
    {
        /// <summary>
        /// This will contain a list of all available renderers for each injection point.
        /// </summary>
        private Dictionary<CustomPostProcessInsertPoint, List<Type>> _availableRenderers;

        /// <summary>
        /// Contains 3 Reorderable list for each settings property.
        /// </summary>
        private struct DrawerState 
        {
            public ReorderableList listAfterOpaqueAndSky, listBeforePostProcess, listAfterPostProcess;
        }

        /// <summary>
        /// Since the drawer is shared for multiple properties, we need to store the reorderable lists for each property by path.
        /// </summary>
        private Dictionary<string, DrawerState> propertyStates = new Dictionary<string, DrawerState>();

        /// <summary>
        /// Get the renderer name from the attached custom post-process attribute.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetName(Type type)
        {
            return CustomPostProcessAttribute.GetAttribute(type)?.Name ?? type?.Name;
        }

        // This code is mostly copied from Unity's HDRP repository
        /// <summary>
        /// Intialize a reoderable list
        /// </summary>
        void InitList(ref ReorderableList reorderableList, List<string> elements, string headerName, CustomPostProcessInsertPoint injectionPoint, CustomPostProcess feature)
        {
            reorderableList = new ReorderableList(elements, typeof(string), true, true, true, true);

            reorderableList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, headerName, EditorStyles.boldLabel);

            reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                rect.height = EditorGUIUtility.singleLineHeight;
                var elemType = Type.GetType(elements[index]);
                EditorGUI.LabelField(rect, GetName(elemType), EditorStyles.boldLabel);
            };

            reorderableList.onAddCallback = (list) =>
            {
                var menu = new GenericMenu();

                foreach (var type in _availableRenderers[injectionPoint])
                {
                    if (!elements.Contains(type.AssemblyQualifiedName))
                        menu.AddItem(new GUIContent(GetName(type)), false, () => {
                            Undo.RegisterCompleteObjectUndo(feature, $"Added {type.ToString()} Custom Post Process");
                            elements.Add(type.AssemblyQualifiedName);
                            forceRecreate(feature); // This is done since OnValidate doesn't get called.
                        });
                }

                if (menu.GetItemCount() == 0)
                    menu.AddDisabledItem(new GUIContent("No Custom Post Process Availble"));

                menu.ShowAsContext();
                EditorUtility.SetDirty(feature);
            };
            reorderableList.onRemoveCallback = (list) =>
            {
                Undo.RegisterCompleteObjectUndo(feature, $"Removed {list.list[list.index].ToString()} Custom Post Process");
                elements.RemoveAt(list.index);
                EditorUtility.SetDirty(feature);
                forceRecreate(feature); // This is done since OnValidate doesn't get called.
            };
            reorderableList.elementHeightCallback = _ => EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            reorderableList.onReorderCallback = (list) => { 
                EditorUtility.SetDirty(feature); 
                forceRecreate(feature); // This is done since OnValidate doesn't get called.
            };
        }

        /// <summary>
        /// Initialize a drawer state for the giver property if none already exists.
        /// </summary>
        /// <param name="property">The property that will be edited/displayed</param>
        private void Init(SerializedProperty property)
        {
            var path = property.propertyPath;
            if(!propertyStates.ContainsKey(path)){
                var state = new DrawerState();
                var feature = property.serializedObject.targetObject as CustomPostProcess;
                InitList(ref state.listAfterOpaqueAndSky, feature.m_Settings.m_RenderersAfterOpaqueAndSky, "After Opaque and Sky", CustomPostProcessInsertPoint.AfterOpaqueAndSky, feature);
                InitList(ref state.listBeforePostProcess, feature.m_Settings.m_RenderersBeforePostProcess, "Before Post Process", CustomPostProcessInsertPoint.BeforePostProcess, feature);
                InitList(ref state.listAfterPostProcess, feature.m_Settings.m_RenderersAfterPostProcess, "After Post Process", CustomPostProcessInsertPoint.AfterPostProcess, feature);
                propertyStates.Add(path, state);
            }   
        }

        /// <summary>
        /// Present the property on the Editor GUI
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            populateRenderers();
            EditorGUI.BeginProperty(position, label, property);
            Init(property);
            DrawerState state = propertyStates[property.propertyPath];
            EditorGUI.BeginChangeCheck();
            state.listAfterOpaqueAndSky.DoLayoutList();
            EditorGUILayout.Space();
            state.listBeforePostProcess.DoLayoutList();
            EditorGUILayout.Space();
            state.listAfterPostProcess.DoLayoutList();
            if (EditorGUI.EndChangeCheck()){
				property.serializedObject.ApplyModifiedProperties();
            }
            EditorGUI.EndProperty();
            EditorUtility.SetDirty(property.serializedObject.targetObject);
        }

        /// <summary>
        /// Force recreating the render feature
        /// </summary>
        /// <param name="feature">The render feature to recreate</param>
        private void forceRecreate(CustomPostProcess feature)
        {
            feature.Create();
        }

        /// <summary>
        /// Finds all the custom post-processing renderer classes and categorizes them by injection point
        /// </summary>
        private void populateRenderers()
        {
            if(_availableRenderers != null) return;
            _availableRenderers = new Dictionary<CustomPostProcessInsertPoint, List<Type>>()
            {
                { CustomPostProcessInsertPoint.AfterOpaqueAndSky, new List<Type>() },
                { CustomPostProcessInsertPoint.BeforePostProcess, new List<Type>() },
                { CustomPostProcessInsertPoint.AfterPostProcess , new List<Type>() }
            };
            foreach(var type in TypeCache.GetTypesDerivedFrom<CustomPostProcessRenderer>()){
                if(type.IsAbstract) continue;
                var attributes = type.GetCustomAttributes(typeof(CustomPostProcessAttribute), false);
                if(attributes.Length != 1) continue;
                CustomPostProcessAttribute attribute = attributes[0] as CustomPostProcessAttribute;
                if(attribute.InsertPoint.HasFlag(CustomPostProcessInsertPoint.AfterOpaqueAndSky))
                    _availableRenderers[CustomPostProcessInsertPoint.AfterOpaqueAndSky].Add(type);
                if(attribute.InsertPoint.HasFlag(CustomPostProcessInsertPoint.BeforePostProcess))
                    _availableRenderers[CustomPostProcessInsertPoint.BeforePostProcess].Add(type);
                if(attribute.InsertPoint.HasFlag(CustomPostProcessInsertPoint.AfterPostProcess))
                    _availableRenderers[CustomPostProcessInsertPoint.AfterPostProcess].Add(type);
            }
        }
    }
}