using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace UnityEditor.AleToonURP.ShaderGUI
{
    /// <summary>
    /// GUI扩展
    /// </summary>
    public static class ShaderGUIExtension
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="colorDefault"></param>
        public static void Init(Color colorDefault)
        {
            ColorDefault = colorDefault;
        }

        /// <summary>
        /// GUI默认色
        /// </summary>
        public static Color ColorDefault { get; set; }

        /// <summary>
        /// GUI 开启色
        /// </summary>
        public static Color ColorON { get; } = Color.green;

        /// <summary>
        /// GUI 关闭色
        /// </summary>
        public static Color ColorOFF { get; } = Color.gray;

        #region 属性值输入框
        /// <summary>
        /// Int输入框
        /// </summary>
        /// <param name="matProp"></param>
        /// <param name="content"></param>
        public static void IntField(MaterialProperty matProp, GUIContent content)
        {
            var stencilNoNew = EditorGUILayout.IntField(content, (int)matProp.floatValue);
            if (matProp.floatValue != stencilNoNew)
                matProp.floatValue = stencilNoNew;
        }
        #endregion

        #region 切换按钮
        public static GUILayoutOption[] LayoutBtnSmall = new GUILayoutOption[] { GUILayout.Width(50) }; //小尺寸
        public static GUILayoutOption[] LayoutBtnMiddle = new GUILayoutOption[] { GUILayout.Width(120) }; //中尺寸

        /// <summary>
        /// 按钮 开关切换
        /// </summary>
        /// <param name="material"></param>
        /// <param name="matPropName">材质球属性名称</param>
        /// <param name="txtON">自定义文本 ON</param>
        /// <param name="txtOFF">自定义文本 OFF</param>
        public static void BtnToggleLabel(string label, MaterialProperty matProp)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PrefixLabel(label);
            OnBtnToggle(matProp);

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 按钮 开关切换
        /// </summary>
        /// <param name="material"></param>
        /// <param name="matPropName">材质球属性名称</param>
        /// <param name="txtON">自定义文本 ON</param>
        /// <param name="txtOFF">自定义文本 OFF</param>
        public static void BtnToggleLabel(GUIContent label, MaterialProperty matProp)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PrefixLabel(label);
            OnBtnToggle(matProp);

            EditorGUILayout.EndHorizontal();
        }

        private static void OnBtnToggle(MaterialProperty matProp)
        {
            if (matProp.floatValue == 0)
            {
                GUI.color = ColorOFF;
                if (GUILayout.Button("关", LayoutBtnSmall))
                    matProp.floatValue = 1f;
                GUI.color = ColorDefault;
            }
            else
            {
                GUI.color = ColorON;
                if (GUILayout.Button("开", LayoutBtnSmall))
                    matProp.floatValue = 0f;
                GUI.color = ColorDefault;
            }
        }

        /// <summary>
        /// 按钮 开关切换 Pass是否开启
        /// </summary>
        /// <param name="material"></param>
        /// <param name="shaderPassName">shader Pass名称</param>
        public static void BtnToggleLabelPass(GUIContent label, Material material, string shaderPassName)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PrefixLabel(label);

            if (material.GetShaderPassEnabled(shaderPassName))
            {
                GUI.color = ColorON;
                if (GUILayout.Button("开", LayoutBtnSmall))
                    material.SetShaderPassEnabled(shaderPassName, false);
                GUI.color = ColorDefault;
            }
            else
            {
                GUI.color = ColorOFF;
                if (GUILayout.Button("关", LayoutBtnSmall))
                    material.SetShaderPassEnabled(shaderPassName, true);
                GUI.color = ColorDefault;
            }

            EditorGUILayout.EndHorizontal();
        }
        #endregion

        #region 下拉弹窗
        /// <summary>
        /// 下拉弹窗
        /// </summary>
        /// <param name="label"></param>
        /// <param name="property"></param>
        /// <param name="type"></param>
        /// <param name="materialEditor"></param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static void DropdownEnum(GUIContent label, MaterialProperty property, System.Type type, MaterialEditor materialEditor)
        {
            if (property == null)
            {
                LabelTextColor("下拉弹窗错误！ >> 无效的材质球属性", Color.red);
                return;
            }

            EditorGUI.showMixedValue = property.hasMixedValue;

            //下拉弹窗
            var enumNames = System.Enum.GetNames(type);
            var mode = property.floatValue;
            EditorGUI.BeginChangeCheck();
            mode = EditorGUILayout.Popup(label, (int)mode, enumNames);
            if (EditorGUI.EndChangeCheck())
            {
                //更新材质球数值
                materialEditor.RegisterPropertyChangeUndo(label.text);
                property.floatValue = mode;
            }

            EditorGUI.showMixedValue = false;
        }
        #endregion

        #region 折叠面板
        /// <summary>
        /// 折叠面板 UI风格
        /// </summary>
        public enum EFoldoutStyleType
        {
            /// <summary>
            /// 主面板
            /// </summary>
            Main,
            /// <summary>
            /// 子面板
            /// </summary>
            Sub
        }

        //折叠面板的打开状态
        private static Dictionary<string, bool> m_DicFoldoutIsOpen = new Dictionary<string, bool>();

        /// <summary>
        /// 折叠面板
        /// </summary>
        /// <param name="title">作为记录打开状态的Key</param>
        /// <param name="panelGUI">折叠面板</param>
        /// <param name="styleType"></param>
        public static void FoldoutPanel(string title, System.Action panelGUI, EFoldoutStyleType styleType = EFoldoutStyleType.Main)
        {
            var style = new GUIStyle("ShurikenModuleTitle");
            style.font = new GUIStyle(EditorStyles.boldLabel).font;
            style.border = new RectOffset(7, 7, 4, 4);
            style.fixedHeight = 22;
            float toggleRectXoffset = 0f;

            //折叠面板类型
            switch (styleType)
            {
                case EFoldoutStyleType.Main:
                    style.contentOffset = new Vector2(20f, -2f);
                    toggleRectXoffset = 4f;
                    break;
                case EFoldoutStyleType.Sub:
                    style.contentOffset = new Vector2(32f, -2f);
                    toggleRectXoffset = 16f;
                    style.padding = new RectOffset(5, 7, 4, 4);
                    break;
            }
            var rect = GUILayoutUtility.GetRect(16f, 22f, style);
            GUI.Box(rect, title, style);

            //当前面板的打开状态
            bool isOpen = false;
            if (!m_DicFoldoutIsOpen.TryGetValue(title, out isOpen))
                m_DicFoldoutIsOpen.Add(title, isOpen);

            //当前事件处理
            var e = Event.current;
            if (e.type == EventType.Repaint)
            {
                var toggleRect = new Rect(rect.x + toggleRectXoffset, rect.y + 2f, 13f, 13f);
                EditorStyles.foldout.Draw(toggleRect, false, false, isOpen, false);
            }
            if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
            {
                m_DicFoldoutIsOpen[title] = !isOpen;
                e.Use();
            }

            //是否打开面板
            if (isOpen)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.Space();
                panelGUI?.Invoke();
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.Space();
        }
        #endregion

        #region 彩色文本
        /// <summary>
        /// 标签 彩色
        /// </summary>
        /// <param name="text"></param>
        /// <param name="color"></param>
        /// <param name="style"></param>
        public static void LabelTextColor(string text, Color color, GUIStyle style = null)
        {
            if (style == null)
                style = EditorStyles.label;

            GUI.color = color;
            GUILayout.Label(text, EditorStyles.boldLabel);
            GUI.color = ColorDefault;
        }
        #endregion
    }
}