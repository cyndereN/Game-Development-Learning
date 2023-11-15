using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ConfigTool
{
    [CreateAssetMenu(menuName = "Config Tool Data")]
    public class ToolEnvConfig : ScriptableObject
    {
        private string _toolPath = GetToolPath();
        private string _templatePath = GetTemplatePath();
        
        [Header("配置输入输出路径")]
        
        public string confRoot;
        
        public string outputCodeDir;
        
        public string outDataDir;

        private static string GetToolPath()
        {
            var absolutePath = Path.GetFullPath(ToolEditorDefine.RESOURCES_PATH);
            return absolutePath + "/Tool/net6.0/Luban.ClientServer.dll";
        }

        private static string GetTemplatePath()
        {
            var absolutePath = Path.GetFullPath(ToolEditorDefine.RESOURCES_PATH);
            return absolutePath + "/CodeTemplates";
        }
        
        public void GenCode()
        {
            string shell;
            string command;
#if UNITY_EDITOR_WIN
            _toolPath = _toolPath.Replace("/", "\\");
            
            _templatePath = _templatePath.Replace("/", "\\");

            shell = "powershell";
            
            command = String.Format(@"   dotnet {0} --template_search_path {1} -j cfg --^ 
                                         -d {2}\Defines\__root__.xml ^ 
                                         --input_data_dir {2} \Datas ^
                                         --output_data_dir {3} ^ 
                                         --output_code_dir {4} ^ 
                                         --gen_types code_cs_unity_bin_idx,cs_custom_data_bidx,after_custom_data_bidx ^ 
                                         -s all                                                               
                                         pause", 
                            _toolPath, _templatePath, confRoot, outDataDir, outputCodeDir);
#elif UNITY_EDITOR_OSX
            shell = "/bin/bash";
            command = String.Format(@"  dotnet {0} --template_search_path {1} -j cfg -- \
                                        -d {2}/Defines/__root__.xml \
                                        --input_data_dir {2} /Datas \
                                        --output_data_dir {3} \
                                        --output_code_dir {4} \
                                        --gen_types code_cs_unity_bin_idx,cs_custom_data_bidx,after_custom_data_bidx \
                                        -s all
                                         ", _toolPath, _templatePath, confRoot, outDataDir, outputCodeDir);
#else
            Debug.Log("Not a valid platform");
            return;
#endif
            RunCommand(command, shell);
        }

        private static void RunCommand(string command, string shell)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = shell,
                        Arguments = "-c \"" + command + "\"",
                        UseShellExecute = false,
                        CreateNoWindow = false,
                        ErrorDialog = true,
                        RedirectStandardOutput = true
                    }
                };

                process.Start();
                string result = process.StandardOutput.ReadToEnd();
                PostCommand(result);
                process.WaitForExit();
            }
            catch (Exception e)
            {
                Debug.LogError("Generation failed. Please check File Paths.");
            }

        }

        private static void PostCommand(string result)
        {
            string[] resultArray = result.Split('\n');
            var length = resultArray.Length;
            if (resultArray[length - 2].Contains("fail"))
            {
                Debug.LogError(result);
                EditorUtility.DisplayDialog("Fail!" , "Please check console for details.", "OK");
            }
            else  // Succeeded
            {
                Debug.Log(result);
                EditorUtility.DisplayDialog("Success!" , "", "OK");
            }
        }
    }
}