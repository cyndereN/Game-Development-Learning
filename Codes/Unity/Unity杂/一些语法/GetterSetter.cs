
[SerializeField]
private string name;
public string Name
{
    get { return name; }
    set
    {
        Debug.Log("通过属性设置");
        name = value;
    }
}

 
// 通过inspector更改
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
 
[CustomEditor(typeof(Test))]
public class TestInspector : Editor
{
    Test model;
    public override void OnInspectorGUI()
    {
        model = target as Test;
        string name = EditorGUILayout.TextField("名字", model.name);
        if (model.Name != name)
        {
            model.Name = name;
        }
 
        base.DrawDefaultInspector();
    }
 
}
 
//------------------------------------------------------
// 是另一种简写, set前面的private声明是告诉编译器属性X是只读（read-only）的. 意思是对于外部类不能通过属性X给x赋值,而只能读取其值。  
public int X {get; private set;} 
 
// 等价于
private int x;
public int X
{
    get { return x; }
}