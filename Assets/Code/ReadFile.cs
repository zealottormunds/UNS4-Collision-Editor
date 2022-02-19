using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System;
using System.Globalization;
using System.Linq;

public class ReadFile : MonoBehaviour
{
    public bool EmptySpaces = false;
    public string FilePath = "";
    public List<MeshRenderer> rend = new List<MeshRenderer>();
    public List<MeshFilter> m = new List<MeshFilter>();

    public int SectionCount = 0;
    public int TotalVertex = 0;
    public List<int> SectionVertexCount = new List<int>();
    public List<List<Vector3>> SectionVertex = new List<List<Vector3>>();
    public List<byte[]> SectionFoot = new List<byte[]>();

    void Start()
    {
        try
        {
            OpenFile();
        }
        catch(Exception e)
        {
            MessageBox.Show(e.Message);
        }
    }

    void OpenFile()
    {
        MessageBox.Show("Select your collision file...");

        OpenFileDialog o = new OpenFileDialog();
        o.ShowDialog();
        if(!(o.FileName != "" && File.Exists(o.FileName))) return;

        FilePath = o.FileName;

        DialogResult d = MessageBox.Show("Does this collision have empty space in header?", "", MessageBoxButtons.YesNo);

        if (d == DialogResult.Yes) EmptySpaces = true;

        byte[] fileBytes = File.ReadAllBytes(FilePath);

        int f = 0;
        int totalLen = GetInt(fileBytes, f);
        f += 4;

        while(f < fileBytes.Length)
        {
            int actualSectionCount = GetInt(fileBytes, f);
            //MessageBox.Show(actualSectionCount.ToString("X2"));
            f += 4;

            //MessageBox.Show("Foot: " + fileBytes[f + 1].ToString("X2"));
            byte[] footEff = new byte[] { fileBytes[f], fileBytes[f + 1], fileBytes[f + 2], fileBytes[f + 3] };
            f += 4;

            if (EmptySpaces) f += 8;

            SectionVertex.Add(new List<Vector3>());
            SectionVertexCount.Add(actualSectionCount * 0x3);
            SectionFoot.Add(footEff);

            for(int x = 0; x < actualSectionCount * 0x3; x++)
            {
                //Debug.Log(f.ToString("X2"));
                float X = BitConverter.ToSingle(new byte[] { fileBytes[f + 3], fileBytes[f + 2], fileBytes[f + 1], fileBytes[f + 0] }, 0);
                f += 4;

                float Z = BitConverter.ToSingle(new byte[] { fileBytes[f + 3], fileBytes[f + 2], fileBytes[f + 1], fileBytes[f + 0] }, 0);
                f += 4;

                float Y = BitConverter.ToSingle(new byte[] { fileBytes[f + 3], fileBytes[f + 2], fileBytes[f + 1], fileBytes[f + 0] }, 0);
                f += 4;

                SectionVertex[SectionCount].Add(new Vector3(X / 20, Y / 20, Z / 20));
            }

            SectionCount++;
        }

        for(int x = 0; x < SectionCount; x++)
        {
            GameObject mesh = new GameObject();
            mesh.transform.position = Vector3.zero;
            rend.Add(mesh.AddComponent<MeshRenderer>());
            rend[rend.Count - 1].material = this.GetComponent<Renderer>().material;
            m.Add(mesh.AddComponent<MeshFilter>());
            m[m.Count - 1].mesh = new Mesh();

            List<Vector3> vertices = SectionVertex[x];
            List<int> triangles = new List<int>();

            for(int a = 0; a < vertices.Count / 3; a++)
            {
                triangles.Add((3 * a) + 2);
                triangles.Add((3 * a) + 1);
                triangles.Add((3 * a) + 0);
            }

            /*for (int a = 0; a < vertices.Count / 3; a++)
            {
                triangles.Add((3 * a) + 2);
                triangles.Add((3 * a) + 1);
                triangles.Add((3 * a) + 0);
            }*/

            m[m.Count - 1].mesh.SetVertices(vertices);
            m[m.Count - 1].mesh.SetTriangles(triangles, 0);
            //m.mesh.RecalculateNormals();

            m[m.Count - 1].GetComponent<Renderer>().material.color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

            m[m.Count - 1].gameObject.AddComponent<StaticWireframeRenderer>();
            m[m.Count - 1].GetComponent<StaticWireframeRenderer>().WireMaterial = gameObject.GetComponent<StaticWireframeRenderer>().WireMaterial;
            m[m.Count - 1].gameObject.name = (m.Count - 1).ToString();
        }
    }

    public void ExportObj()
    {
        SaveFileDialog s = new SaveFileDialog();
        s.ShowDialog();

        if (s.FileName == "") return;

        List<string> file = new List<string>();

        file.Add("# File exported with UNS4 Collision Editor");
        file.Add("# Tool by Zealot Tormunds");
        file.Add("");

        int prevVert = 0;

        for(int a = 0; a < SectionVertex.Count; a++)
        {
            file.Add("g Mesh" + a.ToString());

            for(int x = 0; x < SectionVertex[a].Count; x++)
            {
                file.Add("v " + 
                    SectionVertex[a][x].x.ToString("0.000").Replace(',','.') + " " + 
                    SectionVertex[a][x].y.ToString("0.000").Replace(',', '.') + " " + 
                    (-SectionVertex[a][x].z).ToString("0.000").Replace(',', '.')
                );
            }

            file.Add("");

            for (int x = 0; x < SectionVertex[a].Count; x++)
            {
                file.Add("vt 0 0");
            }

            file.Add("");

            for (int x = 0; x < SectionVertex[a].Count; x++)
            {
                file.Add("vn 0 0 0");
            }

            file.Add("");

            for (int x = 0; x < SectionVertex[a].Count / 3; x++)
            {
                string v1 = ((x * 3) + 1 + prevVert).ToString();
                string v2 = ((x * 3) + 2 + prevVert).ToString();
                string v3 = ((x * 3) + 3 + prevVert).ToString();

                string line = v3 + "/" + v3 + "/1 ";
                line = line + v2 + "/" + v2 + "/1 ";
                line = line + v1 + "/" + v1 + "/1 ";

                file.Add("f " + line);
            }

            file.Add("");

            prevVert = prevVert + SectionVertex[a].Count;
        }

        File.WriteAllLines(s.FileName, file.ToArray());
    }

    public void ImportObj()
    {
        OpenFileDialog o = new OpenFileDialog();
        o.ShowDialog();

        if (o.FileName == "" || File.Exists(o.FileName) == false) return;

        string[] file = File.ReadAllLines(o.FileName);
        int a = 0;

        int actualGroup = -1;

        List<Vector3> vertList = new List<Vector3>();
        while (a < file.Length)
        {
            string identifier = "#";
            if (file[a].Length > 2) identifier = file[a].Substring(0, 2);

            switch(identifier)
            {
                case "v ":
                    string[] param = file[a].Split(' ');
                    vertList.Add(new Vector3(
                        float.Parse(param[1], System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture), 
                        float.Parse(param[2], System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture),
                        float.Parse(param[3], System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture)
                        ));
                    //Debug.Log(float.Parse(param[1], System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture));
                    break;
            }

            a++;
        }

        a = 0;

        List<int> triList = new List<int>();
        int prevTri = 0;
        int tricount = 0;

        while (a < file.Length)
        {
            string identifier = "#";
            if (file[a].Length > 2) identifier = file[a].Substring(0, 2);
            
            switch (identifier)
            {
                case "g ":
                    if (actualGroup == -1)
                    {
                        actualGroup++;
                        SectionVertex[actualGroup].Clear();
                        tricount = 0;
                        Debug.Log("Cleared list");
                    }
                    else
                    {
                        SectionVertexCount[actualGroup] = SectionVertex[actualGroup].Count;
                        Importer_AddToGroup(actualGroup, SectionVertex[actualGroup], triList);
                        Debug.Log(SectionVertex[actualGroup][0]);
                        triList.Clear();

                        actualGroup++;
                        SectionVertex[actualGroup].Clear();
                        tricount = 0;
                    }
                    break;
                case "f ":
                    string[] param2 = file[a].Split(' ');
                    List<string[]> param3 = new List<string[]>();
                    param3.Add(param2[1].Split('/'));
                    param3.Add(param2[2].Split('/'));
                    param3.Add(param2[3].Split('/'));

                    int t1 = int.Parse(param3[2][0]) - 1;
                    int t2 = int.Parse(param3[1][0]) - 1;
                    int t3 = int.Parse(param3[0][0]) - 1;

                    if (triList.Count == 0) prevTri = t1;

                    //triList.Add(t1 - prevTri);
                    //triList.Add(t2 - prevTri);
                    //triList.Add(t3 - prevTri);
                    triList.Add(tricount);
                    triList.Add(tricount + 1);
                    triList.Add(tricount + 2);
                    tricount = tricount + 3;

                    //Debug.Log(vertList[t3]);

                    if (SectionVertex[actualGroup].Count == 0) Debug.Log(vertList[t1]);

                    SectionVertex[actualGroup].Add(vertList[t1]);
                    SectionVertex[actualGroup].Add(vertList[t2]);
                    SectionVertex[actualGroup].Add(vertList[t3]);

                    int temp = triList[triList.Count - 3];
                    triList[triList.Count - 3] = triList[triList.Count - 1];
                    triList[triList.Count - 1] = temp;
                    break;
            }

            a++;
        }

        SectionVertexCount[actualGroup] = SectionVertex[actualGroup].Count;
        Importer_AddToGroup(actualGroup, SectionVertex[actualGroup], triList);

        Debug.Log(SectionVertex[actualGroup][0]);

        triList.Clear();
    }

    public void Importer_AddToGroup(int actualGroup, List<Vector3> thisVertList, List<int> triList)
    {
        GameObject.Destroy(m[actualGroup].gameObject);
        GameObject newmesh = new GameObject();
        newmesh.transform.position = Vector3.zero;

        rend[actualGroup] = newmesh.AddComponent<MeshRenderer>();
        rend[actualGroup].material = this.GetComponent<Renderer>().material;
        m[actualGroup] = newmesh.AddComponent<MeshFilter>();
        m[actualGroup].mesh = new Mesh();
        m[actualGroup].mesh.SetVertices(thisVertList);
        m[actualGroup].mesh.SetTriangles(triList.ToArray(), 0);
        m[actualGroup].mesh.RecalculateNormals();

        m[actualGroup].gameObject.AddComponent<VertexList>().vertexList.Clear();
        m[actualGroup].gameObject.GetComponent<VertexList>().vertexList.AddRange(thisVertList.ToArray());

        //Debug.Log(thisVertList[0]);

        m[actualGroup].GetComponent<Renderer>().material.color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        m[actualGroup].gameObject.AddComponent<StaticWireframeRenderer>();
        m[actualGroup].GetComponent<StaticWireframeRenderer>().WireMaterial = gameObject.GetComponent<StaticWireframeRenderer>().WireMaterial;
        m[actualGroup].gameObject.name = actualGroup.ToString();
    }

    public void ExportHit()
    {
        SaveFileDialog s = new SaveFileDialog();
        s.ShowDialog();

        if (s.FileName == "") return;

        List<byte> fileBytes = new List<byte>();

        int maincount = 0;
        for (int a = 0; a < SectionVertex.Count; a++) maincount = maincount + SectionVertex[a].Count;
        byte[] mainSectCount = BitConverter.GetBytes(maincount);

        for (int x = 0; x < 0x4; x++) fileBytes.Add(mainSectCount[3 - x]);
        //int mainheaderlength = 0x4;
        //for (int x = 0; x < mainheaderlength; x++) fileBytes.Add(0x0);

        int headerlength = 0x8;
        if (EmptySpaces) headerlength = headerlength + 0x8;

        for (int a = 0; a < SectionVertex.Count; a++)
        {
            // Add section header
            byte[] vertCountSect = BitConverter.GetBytes(SectionVertex[a].Count / 3);
            for (int x = 0; x < 0x4; x++) fileBytes.Add(vertCountSect[3 - x]);
            for (int x = 0; x < 0x4; x++) fileBytes.Add(SectionFoot[a][x]);
            for (int x = 0; x < headerlength - 0x8; x++) fileBytes.Add(0x0);

            for (int x = 0; x < SectionVertex[a].Count; x++)
            {
                byte[] f1 = BitConverter.GetBytes(SectionVertex[a][x].x * 20);
                byte[] f2 = BitConverter.GetBytes(SectionVertex[a][x].z * 20);
                byte[] f3 = BitConverter.GetBytes(SectionVertex[a][x].y * 20);

                for (int b = 0; b < 4; b++) fileBytes.Add(f1[3 - b]);
                for (int b = 0; b < 4; b++) fileBytes.Add(f2[3 - b]);
                for (int b = 0; b < 4; b++) fileBytes.Add(f3[3 - b]);
            }
        }

        File.WriteAllBytes(s.FileName, fileBytes.ToArray());
    }

    public int GetInt(byte[] fileBytes, int Index, bool reverse = true)
    {
        int a = fileBytes[Index] + (fileBytes[Index + 1] * 0x100) + (fileBytes[Index + 2] * 0x10000) + (fileBytes[Index + 3] * 0x1000000);

        if (reverse) a = fileBytes[Index + 3] + (fileBytes[Index + 2] * 0x100) + (fileBytes[Index + 1] * 0x10000) + (fileBytes[Index + 0] * 0x1000000);

        return a;
    }
}
