using System;
using UnityEngine;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using System.Text;

[Serializable]
public class Certificate : ScriptableObject
{
    public X509Certificate2 X509Certificate2
    {
        get
        {
            if (m_x509Certificate2 == null)
            {
                byte[] rawData = Encoding.ASCII.GetBytes(m_privateKey);
                return m_x509Certificate2 = new X509Certificate2(rawData);
            }

            return m_x509Certificate2;
        }
    }

    public Certificate(string privateKey)
    {
        m_privateKey = privateKey;
    }

    [SerializeField, Multiline]
    private string m_privateKey;

    private X509Certificate2 m_x509Certificate2;
}
