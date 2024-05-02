using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace AlePostProcessUPR
{
    /// <summary>
    /// 后处理容器 基类
    /// </summary>
    public abstract class CustomVolumeComponent : VolumeComponent
    {
        //开启状态
        [HideInInspector]
        public BoolParameter Enable = new BoolParameter(false, false);

        protected override void OnEnable()
        {
            base.OnEnable();

            //VolumeComponent.active 有Bug始终为true
            //使用Enable的overrideState作为Volume的激活状态
            for (int i = 0; i < parameters.Count; i++)
            {
                var param = parameters[i];

                if (param == Enable)
                {
                    param.overrideState = true;
                    break;
                }
            }
        }
    }
}