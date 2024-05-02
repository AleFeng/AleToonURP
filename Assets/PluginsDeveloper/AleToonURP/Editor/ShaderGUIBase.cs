using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace UnityEditor.AleToonURP.ShaderGUI
{
    /// <summary>
    /// 材质球编辑器界面
    /// </summary>
    public class ShaderGUIBase : UnityEditor.ShaderGUI
    {
        protected MaterialEditor m_MaterialEditor; //当前的材质球编辑器
        protected Material m_Material; //当前的材质球

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            //GUI扩展 初始化
            ShaderGUIExtension.Init(GUI.color);
            EditorGUIUtility.fieldWidth = 0;

            //获取并设置材质球属性
            m_MaterialEditor = materialEditor;
            m_Material = materialEditor.target as Material;
            InitMatProperty(props);

            //信息改动检查
            EditorGUI.BeginChangeCheck();

            OnGUIAleToon();

            //更新材质球编辑器 参数
            if (EditorGUI.EndChangeCheck())
                m_MaterialEditor.PropertiesChanged();
        }

        /// <summary>
        /// GUI渲染
        /// 子类继承进行GUI面板设置
        /// </summary>
        protected virtual void OnGUIAleToon()
        {

        }

        #region 材质球属性
        private Dictionary<string, MaterialProperty> m_DicMaterialProperty = new Dictionary<string, MaterialProperty>(); //字典 属性名称:材质球属性
        private MaterialProperty[] m_MaterialPropertyArray; //材质球属性数组
        private bool m_IsInitMaterialProperty = false; //是否初始化 材质球属性

        /// <summary>
        /// 设置 材质球属性
        /// </summary>
        /// <param name="props"></param>
        private void InitMatProperty(MaterialProperty[] props)
        {
            if (m_IsInitMaterialProperty == true) return;
            m_IsInitMaterialProperty = true;

            m_MaterialPropertyArray = props;
            if (m_MaterialPropertyArray == null) return;

            //记录材质球属性字典
            m_DicMaterialProperty.Clear();
            for (int i = 0; i < m_MaterialPropertyArray.Length; i++)
            {
                var matProp = m_MaterialPropertyArray[i];
                m_DicMaterialProperty.Add(matProp.name, matProp);
            }
        }

        /// <summary>
        /// 获取 材质球属性
        /// </summary>
        /// <param name="propName"></param>
        /// <returns></returns>
        protected MaterialProperty GetMaterialProperty(string propName)
        {
            MaterialProperty matProp;
            if (!m_DicMaterialProperty.TryGetValue(propName, out matProp))
            {
                matProp = FindProperty(propName, m_MaterialPropertyArray);
                if (matProp == null)
                    Debug.LogError($"AleToonURPShaderGUI.GetMaterialProperty() Error!! >> 无效的属性名称-{propName}");
                else
                    m_DicMaterialProperty.Add(propName, matProp);
            }

            return matProp;
        }
        #endregion
    }
}