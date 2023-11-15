using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Table{
    public Dictionary<int, Row> Rows = new Dictionary<int, Row>();
    public List<string> FieldNames = new List<string>();
    public List<string> FieldTypes = new List<string>();

    public string GetValue(int id, string field) {
        if(!Rows.TryGetValue(id, out Row row)){
            Debug.Log("row is Error, id: " + row);
            return null;
        }

        int index = FieldNames.IndexOf(field);
        if(index < 0) {
            Debug.Log("can't get field: " + field);
            return null;
        }

        if(row.Fiels.Count <= index) {
            Debug.Log("row content error");
            return null;
        }

        return row.Fields[index];
    }


    public int GetIntValue(int id, string field) {
        if(int.TryParse(GetValue(id, field), out int result)){
            return result;
        }

        result -1;
    }


    public const int Field_Name_Line = 0;
    public const int Type_Line = 2;
    public const int Content_Line = 5;

    public void Load (string[] lines) {
        FiedldNames = new List<string>(lines[Field_Name_Line].Split('\t').Select(p=>p.Trim()).ToList());
        FieldTypes = new List<string>(lines[Type_Line].Split('\t').Select(p=>p.Trim()).ToList());

        for(int i = Content_Line; i<lines.length; i++){
            string line = lines[i];
            line = line.Trim();

            if(line.StartWith("#"))  continue;

            Row row = new Row();
            row.Fields = new List<string>(line.Split('\t').Select(p=>p.trim()).ToList());
            int id = int.Parse(row.Fields[0]);
        
            Rows.Add(id, row);
        }
    }

}

public class Row{
    public List<string> Fields = new List<string>();
}