using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Core.Serialization;

public class SerializationTest : MonoBehaviour
{
    void Start()
    {
        if(!File.Exists("test.xml"))
            TestWriting();
    }

    void Update()
    {
        if (read)
        {
            read = false;

            TestReading();
        }

        if (write)
        {
            write = false;
            TestWriting();
        }
    }

    private void TestWriting()
    {
        SerializedObject serializedObject = Serializer.Serialize(testSaveObject);
        serializedObject.WriteTo("test.xml");
    }

    private void TestReading()
    {
        SerializedObject serializedObject = new SerializedObject("test.xml");
        testSaveObject = Serializer.Deserialize<TestSaveObject>(serializedObject);
    }

    [SerializeField]
    private TestSaveObject testSaveObject = new TestSaveObject();

    [SerializeField]
    private bool read;
    [SerializeField]
    private bool write;
}

[Serializable]
[Version(1)]
public class TestSaveObject
{
    [SerializeField]
    public int d = 3;
    [SerializeField]
    public float b = 4;
    [SerializeField]
    public string words = "A test";
    [SerializeField]
    public Vector2 vector;

    private static void _UpgradeToVersion1(SerializedObjectNode field)
    {
        if(field.Name == "a")
            field.Rename("d");
    }
}
