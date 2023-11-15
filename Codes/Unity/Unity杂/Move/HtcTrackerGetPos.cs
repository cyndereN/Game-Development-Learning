using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GetPos : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject R_Tracker;
    public GameObject H_Tracker;
    string filePath;
    Transform R_transform;
    Transform H_transform;

    void Start()
    {
        var dateTime = System.DateTime.Now;
        string fileName = string.Format("{0:D2}_{1:D2}_{2:D4}", dateTime.Hour, dateTime.Minute, dateTime.Second);
        filePath = "C:\\Users\\METAHCI\\Desktop\\AIR\\Htc Tracker\\Assets\\Data\\" + fileName+ ".csv";
        StreamWriter writer = new StreamWriter(filePath);
        writer.WriteLine("Time Stamp, Device_1, Position_x, Position_y, Position_z, Rotation_x, Rotation_y, Rotation_z, Device_2, Position_x, Position_y, Position_z, Rotation_x, Rotation_y, Rotation_z");
        writer.Flush();
        writer.Close();

        StartCoroutine(myRecordPos());
    }

    // Update is called once per frame
    void Update()
    {
  
    }

    IEnumerator myRecordPos()
    {
         while (true)
        {

            R_transform = R_Tracker.transform;
            H_transform = H_Tracker.transform;
            Debug.Log("H" + H_transform.position);
            Debug.Log("R" + R_transform.position);
            FileStream fappend = File.Open(filePath, FileMode.Append);
            StreamWriter writer = new StreamWriter(fappend);


            writer.WriteLine(Time.time+ ","+ "R" + "," + R_transform.position.x + "," + R_transform.position.y + "," + R_transform.position.z + "," + R_transform.eulerAngles.x + "," + R_transform.eulerAngles.y + "," + R_transform.eulerAngles.z + ","+ 
               "H" + "," + H_transform.position.x + "," + H_transform.position.y + "," + H_transform.position.z + "," + H_transform.eulerAngles.x + "," + H_transform.eulerAngles.y + "," + H_transform.eulerAngles.z);
            writer.WriteLine();
            writer.Flush();
            writer.Close();
            yield return new WaitForSeconds(0.1f);
        }
    }
}
