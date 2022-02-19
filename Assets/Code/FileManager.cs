using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FileManager : MonoBehaviour
{
    public static bool b_ReadBool(byte[] actual, int index)
    {
        bool a = (actual[index] != 0);
        return a;
    }

    public static byte[] b_ReadByteArray(byte[] actual, int index, int count)
    {
        List<byte> a = new List<byte>();
        for (int x = 0; x < count; x++)
        {
            a.Add(actual[index + x]);
        }
        return a.ToArray();
    }

    public static int b_byteArrayToInt(byte[] actual)
    {
        int a = 0;
        return actual[0] + actual[1] * 256 + actual[2] * 65536 + actual[3] * 16777216;
    }

    public static int b_byteArrayToIntRev(byte[] actual)
    {
        int a = 0;
        return actual[3] + actual[2] * 256 + actual[1] * 65536 + actual[0] * 16777216;
    }

    public static int b_ReadInt(byte[] fileBytes, int index)
    {
        return b_byteArrayToInt(b_ReadByteArray(fileBytes, index, 4));
    }

    public static int b_ReadIntRev(byte[] fileBytes, int index)
    {
        return b_byteArrayToIntRev(b_ReadByteArray(fileBytes, index, 4));
    }

    public static Int16 b_ReadShort(byte[] fileBytes, int index)
    {
        return BitConverter.ToInt16(fileBytes, index);
    }

    public static float b_ReadFloat(byte[] actual, int index)
    {
        float a = -1f;
        return BitConverter.ToSingle(actual, index);
    }

    public static Vector3 b_ReadVector3(byte[] actual, int index)
    {
        Vector3 a = new Vector3();
        a.x = BitConverter.ToSingle(actual, index);
        a.y = BitConverter.ToSingle(actual, index + 4);
        a.z = BitConverter.ToSingle(actual, index + 8);
        return a;
    }

    public static Quaternion b_ReadQuaternion(byte[] actual, int index)
    {
        Quaternion a = new Quaternion();
        a.x = BitConverter.ToSingle(actual, index);
        a.y = BitConverter.ToSingle(actual, index + 4);
        a.z = BitConverter.ToSingle(actual, index + 8);
        a.w = BitConverter.ToSingle(actual, index + 12);
        return a;
    }

    public static string b_ReadString(byte[] actual, int index, int count = -1)
    {
        string a = "";
        if (count == -1)
        {
            for (int x2 = index; x2 < actual.Length; x2++)
            {
                if (actual[x2] != 0)
                {
                    string str = a;
                    char c = (char)actual[x2];
                    a = str + c;
                }
                else
                {
                    x2 = actual.Length;
                }
            }
        }
        else
        {
            for (int x = index; x < index + count; x++)
            {
                string str2 = a;
                char c = (char)actual[x];
                a = str2 + c;
            }
        }
        return a;
    }

    public static byte[] b_ReplaceBytes(byte[] actual, byte[] bytesToReplace, int Index, int Invert = 0)
    {
        if (Invert == 0)
        {
            for (int x2 = 0; x2 < bytesToReplace.Length; x2++)
            {
                actual[Index + x2] = bytesToReplace[x2];
            }
        }
        else
        {
            for (int x = 0; x < bytesToReplace.Length; x++)
            {
                actual[Index + x] = bytesToReplace[bytesToReplace.Length - 1 - x];
            }
        }
        return actual;
    }

    public static byte[] b_ReplaceInt(byte[] actual, int value, int Index, int Invert = 0)
    {
        byte[] b = BitConverter.GetBytes(value);

        for(int x = 0; x < 4; x++)
        {
            if (Invert == 0) actual[Index + x] = b[x];
            else actual[Index + x] = b[3 - x];
        }

        return actual;
    }

    public static byte[] b_ReplaceString(byte[] actual, string str, int Index, int Count = -1)
    {
        if (Count == -1)
        {
            for (int x2 = 0; x2 < str.Length; x2++)
            {
                actual[Index + x2] = (byte)str[x2];
            }
        }
        else
        {
            for (int x = 0; x < Count; x++)
            {
                if (str.Length > x)
                {
                    actual[Index + x] = (byte)str[x];
                }
                else
                {
                    actual[Index + x] = 0;
                }
            }
        }
        return actual;
    }

    public static byte[] b_AddBool(byte[] actual, bool boolean)
    {
        List<byte> a = actual.ToList();

        if (boolean) a.Add(1);
        else a.Add(0);

        return a.ToArray();
    }

    public static byte[] b_AddBytes(byte[] actual, byte[] bytesToAdd, int Reverse = 0, int index = 0, int count = -1)
    {
        List<byte> a = actual.ToList();
        if (Reverse == 0)
        {
            if (count == -1) count = bytesToAdd.Length;
            for (int x = index; x < index + count; x++)
            {
                a.Add(bytesToAdd[x]);
            }
        }
        else
        {
            if (count == -1) count = bytesToAdd.Length;
            for (int x = index; x < index + count; x++)
            {
                a.Add(bytesToAdd[bytesToAdd.Length - 1 - x]);
            }
        }
        return a.ToArray();
    }

    public static byte[] b_AddInt(byte[] actual, int _num)
    {
        List<byte> a = actual.ToList();
        byte[] b = BitConverter.GetBytes(_num);
        for (int x = 0; x < 4; x++)
        {
            a.Add(b[x]);
        }
        return a.ToArray();
    }

    public static byte[] b_AddShort(byte[] actual, Int16 _num)
    {
        List<byte> a = actual.ToList();
        byte[] b = BitConverter.GetBytes(_num);
        for (int x = 0; x < b.Length; x++)
        {
            a.Add(b[x]);
        }
        return a.ToArray();
    }

    public static byte[] b_AddString(byte[] actual, string _str, int count = -1)
    {
        List<byte> a = actual.ToList();
        for (int x2 = 0; x2 < _str.Length; x2++)
        {
            a.Add((byte)_str[x2]);
        }
        for (int x = _str.Length; x < count; x++)
        {
            a.Add(0);
        }
        return a.ToArray();
    }

    public static byte[] b_AddFloat(byte[] actual, float f)
    {
        List<byte> a = actual.ToList();
        byte[] floatBytes = BitConverter.GetBytes(f);
        for (int x = 0; x < 4; x++) a.Add(floatBytes[x]);
        return a.ToArray();
    }

    public static byte[] b_AddVector3(byte[] actual, Vector3 v)
    {
        List<byte> a = actual.ToList();

        byte[] floatBytes = BitConverter.GetBytes(v.x);
        for (int x = 0; x < 4; x++) a.Add(floatBytes[x]);

        floatBytes = BitConverter.GetBytes(v.y);
        for (int x = 0; x < 4; x++) a.Add(floatBytes[x]);

        floatBytes = BitConverter.GetBytes(v.z);
        for (int x = 0; x < 4; x++) a.Add(floatBytes[x]);

        return a.ToArray();
    }

    public static byte[] b_AddQuaternion(byte[] actual, Quaternion q)
    {
        List<byte> a = actual.ToList();

        byte[] floatBytes = BitConverter.GetBytes(q.x);
        for (int x = 0; x < 4; x++) a.Add(floatBytes[x]);

        floatBytes = BitConverter.GetBytes(q.y);
        for (int x = 0; x < 4; x++) a.Add(floatBytes[x]);

        floatBytes = BitConverter.GetBytes(q.z);
        for (int x = 0; x < 4; x++) a.Add(floatBytes[x]);

        floatBytes = BitConverter.GetBytes(q.w);
        for (int x = 0; x < 4; x++) a.Add(floatBytes[x]);

        return a.ToArray();
    }

    public static int b_FindBytes(byte[] actual, byte[] bytes, int index = 0)
    {
        int actualIndex = index;
        byte[] actualBytes = new byte[bytes.Length];
        bool found = false;
        bool f = false;

        int foundIndex = -1;

        for (int a = actualIndex; a < (actual.Length - bytes.Length); a++)
        {
            f = true;

            for (int x = 0; x < bytes.Length; x++)
            {
                actualBytes[x] = actual[a + x];

                if (actualBytes[x] != bytes[x])
                {
                    x = bytes.Length;
                    f = false;
                }
            }

            if (f)
            {
                found = true;
                foundIndex = a;
                a = actual.Length;
            }
        }

        return foundIndex;
    }

    public static List<int> b_FindBytesList(byte[] actual, byte[] bytes, int index = 0)
    {
        int actualIndex = index;
        List<int> indexes = new List<int>();

        int lastFound = 0;
        while (lastFound != -1)
        {
            lastFound = b_FindBytes(actual, bytes, actualIndex);
            if (lastFound != -1)
            {
                actualIndex = lastFound + 1;
                indexes.Add(lastFound);
            }
        }

        return indexes;
    }
}
