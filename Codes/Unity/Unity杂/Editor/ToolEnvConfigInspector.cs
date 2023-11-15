using UnityEngine;
using UnityEditor;

namespace ConfigTool
{
    [CustomEditor(typeof(ToolEnvConfig))]
    public class ToolEnvConfigInspector : Editor
    {
        private string _confRoot;
        private string _outputCodeDir;
        private string _outDataDir;

        private void OnEnable()
        {
            ToolEnvConfig toolEnvConfig = target as ToolEnvConfig;
            _confRoot = toolEnvConfig.confRoot;
            _outputCodeDir = toolEnvConfig.outputCodeDir;
            _outDataDir = toolEnvConfig.outDataDir;
        }

        public override void OnInspectorGUI()
        {
            // base.OnInspectorGUI();

            ToolEnvConfig toolEnvConfig = target as ToolEnvConfig;

            //水平布局
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("配置表根路径(包含 Define 和 Datas)",  GUILayout.Width(240f));
                _confRoot = GUILayout.TextField(_confRoot);
                if (GUILayout.Button("浏览", GUILayout.Width(50f)))
                {
                    var confRoot = EditorUtility.OpenFolderPanel("配置表根路径", Application.dataPath, "");
                    if (!string.IsNullOrEmpty(confRoot))
                    {
                        _confRoot = confRoot;
                    }
                }
                if (string.IsNullOrEmpty(_confRoot))
                {
                    _confRoot = "配置表根路径(包含 Define 和 Datas)";
                }
            }
            GUILayout.EndHorizontal();
            
            
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("生成代码输出路径", GUILayout.Width(240f));
                _outputCodeDir = GUILayout.TextField(_outputCodeDir);
                if (GUILayout.Button("浏览", GUILayout.Width(50f)))
                {
                    var outputCodeDir = EditorUtility.OpenFolderPanel("生成代码输出路径", Application.dataPath, "");
                    if (!string.IsNullOrEmpty(outputCodeDir))
                    {
                        _outputCodeDir = outputCodeDir;
                    }
                }
                
                if (string.IsNullOrEmpty(_outputCodeDir))
                {
                    _outputCodeDir = "生成代码输出路径";
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("生成数据输出路径", GUILayout.Width(240f));
                _outDataDir = GUILayout.TextField(_outDataDir);
                if (GUILayout.Button("浏览", GUILayout.Width(50f)))
                {
                    var outDataDir = EditorUtility.OpenFolderPanel("生成数据输出路径", Application.dataPath, "");
                    if (!string.IsNullOrEmpty(outDataDir))
                    {
                        _outDataDir = outDataDir;
                    }
                }

                if (string.IsNullOrEmpty(_outDataDir))
                {
                    _outDataDir = "生成数据输出路径";
                }
            }
            GUILayout.EndHorizontal();
            
            toolEnvConfig.confRoot = _confRoot;
            toolEnvConfig.outDataDir = _outDataDir;
            toolEnvConfig.outputCodeDir = _outputCodeDir;
            if (GUILayout.Button("生成"))
                toolEnvConfig.GenCode();
            
        }
    }

}