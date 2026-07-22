using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace AleToonURP.ShaderGUI
{
    /// <summary>
    /// 材质球编辑器界面
    /// </summary>
    public class ShaderGUIBase : UnityEditor.ShaderGUI
    {
        protected MaterialEditor m_MaterialEditor; //当前的材质球编辑器
        protected Material m_Material; //当前的材质球（首个，用于读取与显示）
        protected Material[] m_Materials; //全部选中材质（多选编辑时，关键字/Pass/Tag/队列需对每个都写入）

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            //GUI扩展 初始化
            ShaderGUIExtension.Init(GUI.color);
            EditorGUIUtility.fieldWidth = 0;

            //获取并设置材质球属性
            m_MaterialEditor = materialEditor;
            m_Material = materialEditor.target as Material;
            m_Materials = System.Array.ConvertAll(materialEditor.targets, o => (Material)o);
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

        #region 多选材质写入（对全部选中材质生效；先比较后写，避免每帧重复写把材质反复标脏）
        /// <summary>
        /// 启用关键字（作用于全部选中材质）
        /// </summary>
        protected void EnableKeyword(string keyword)
        {
            foreach (var m in m_Materials)
                if (m != null && !m.IsKeywordEnabled(keyword)) m.EnableKeyword(keyword);
        }

        /// <summary>
        /// 禁用关键字（作用于全部选中材质）
        /// </summary>
        protected void DisableKeyword(string keyword)
        {
            foreach (var m in m_Materials)
                if (m != null && m.IsKeywordEnabled(keyword)) m.DisableKeyword(keyword);
        }

        /// <summary>
        /// 设置覆盖标签（作用于全部选中材质；值不同才写）
        /// </summary>
        protected void SetOverrideTag(string tag, string value)
        {
            foreach (var m in m_Materials)
                if (m != null && m.GetTag(tag, false, string.Empty) != value) m.SetOverrideTag(tag, value);
        }

        /// <summary>
        /// 设置渲染队列（作用于全部选中材质；值不同才写）
        /// </summary>
        protected void SetRenderQueue(int queue)
        {
            foreach (var m in m_Materials)
                if (m != null && m.renderQueue != queue) m.renderQueue = queue;
        }
        #endregion

        #region 材质球属性
        private Dictionary<string, MaterialProperty> m_DicMaterialProperty = new Dictionary<string, MaterialProperty>(); //字典 属性名称:材质球属性
        private MaterialProperty[] m_MaterialPropertyArray; //材质球属性数组
        private HashSet<string> m_MissingPropLogged = new HashSet<string>(); //已报过错的缺失属性名（同一名称只报一次，避免每帧重绘刷屏）

        /// <summary>
        /// 设置 材质球属性
        /// 每次 OnGUI 都用最新的 props 重建，避免属性对象在 Undo/重选材质/外部修改后过期
        /// </summary>
        /// <param name="props"></param>
        private void InitMatProperty(MaterialProperty[] props)
        {
            m_MaterialPropertyArray = props;
            if (m_MaterialPropertyArray == null) return;

            //记录材质球属性字典
            m_DicMaterialProperty.Clear();
            for (int i = 0; i < m_MaterialPropertyArray.Length; i++)
            {
                var matProp = m_MaterialPropertyArray[i];
                m_DicMaterialProperty[matProp.name] = matProp;
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
                {
                    if (m_MissingPropLogged.Add(propName)) //同一缺失名只报一次
                        Debug.LogError($"AleToonURPShaderGUI.GetMaterialProperty() Error!! >> 无效的属性名称-{propName}");
                }
                else
                    m_DicMaterialProperty.Add(propName, matProp);
            }

            return matProp;
        }
        #endregion
    }
}