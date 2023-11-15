public class EditorTableManager 
{
    private static EditorTableManager s_instance;

    public static EditorTableManager Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = new EditorTableManager();
                s_instance.Load();
            }

            return s_instance;
        }
    }

    public Table t;

    public void Load()
    {
        string[] tContent = Resources.Load<TextAsset>("Tables/Table").text.Split('\n');
        t.Load(tContent);
    }
}

// Usage 
// Get value
EditorTableManager.Instance.t.GetValue(Id, field);
// Has key
EditorTableManager.Instance.t.Rows.ContainsKey(Id);
// Traverse
foreach (var kv in EditorTableManager.Instance.t.Rows) {}
