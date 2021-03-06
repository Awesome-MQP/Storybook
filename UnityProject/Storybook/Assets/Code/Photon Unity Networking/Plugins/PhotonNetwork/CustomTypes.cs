// ----------------------------------------------------------------------------
// <copyright file="CustomTypes.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2011 Exit Games GmbH
// </copyright>
// <summary>
//
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------

#pragma warning disable 1587
/// \file
/// <summary>Sets up support for Unity-specific types. Can be a blueprint how to register your own Custom Types for sending.</summary>
#pragma warning restore 1587


using System.IO;
using System.Runtime.InteropServices;
using ExitGames.Client.Photon;
using System;
using Photon;
using UnityEngine;



/// <summary>
/// Internally used class, containing de/serialization methods for various Unity-specific classes.
/// Adding those to the Photon serialization protocol allows you to send them in events, etc.
/// </summary>
internal static class CustomTypes
{
    /// <summary>Register</summary>
    internal static void Register()
    {
        PhotonPeer.RegisterType(typeof (PhotonStream), (byte) 'S', SerializePhotonStream, DeserializePhotonStream);
        PhotonPeer.RegisterType(typeof (PhotonView), (byte) 'B', SerializePhotonView, DeserializePhotonView);
        PhotonPeer.RegisterType(typeof(Vector2), (byte)'W', SerializeVector2, DeserializeVector2);
        PhotonPeer.RegisterType(typeof(Vector3), (byte)'V', SerializeVector3, DeserializeVector3);
        PhotonPeer.RegisterType(typeof(Quaternion), (byte)'Q', SerializeQuaternion, DeserializeQuaternion);
        PhotonPeer.RegisterType(typeof(PhotonPlayer), (byte)'P', SerializePhotonPlayer, DeserializePhotonPlayer);
        PhotonPeer.RegisterType(typeof (uint), (byte) 'U', SerializeUint, DeserializeUint);
    }


    #region Custom De/Serializer Methods


    public static readonly byte[] memVector3 = new byte[3 * 4];
    private static short SerializeVector3(MemoryStream outStream, object customobject)
    {
        Vector3 vo = (Vector3)customobject;

        int index = 0;
        lock (memVector3)
        {
            byte[] bytes = memVector3;
            Protocol.Serialize(vo.x, bytes, ref index);
            Protocol.Serialize(vo.y, bytes, ref index);
            Protocol.Serialize(vo.z, bytes, ref index);
            outStream.Write(bytes, 0, 3 * 4);
        }

        return 3 * 4;
    }

    private static object DeserializeVector3(MemoryStream inStream, short length)
    {
        Vector3 vo = new Vector3();
        lock (memVector3)
        {
            inStream.Read(memVector3, 0, 3 * 4);
            int index = 0;
            Protocol.Deserialize(out vo.x, memVector3, ref index);
            Protocol.Deserialize(out vo.y, memVector3, ref index);
            Protocol.Deserialize(out vo.z, memVector3, ref index);
        }

        return vo;
    }


    public static readonly byte[] memVector2 = new byte[2 * 4];
    private static short SerializeVector2(MemoryStream outStream, object customobject)
    {
        Vector2 vo = (Vector2)customobject;
        lock (memVector2)
        {
            byte[] bytes = memVector2;
            int index = 0;
            Protocol.Serialize(vo.x, bytes, ref index);
            Protocol.Serialize(vo.y, bytes, ref index);
            outStream.Write(bytes, 0, 2 * 4);
        }

        return 2 * 4;
    }

    private static object DeserializeVector2(MemoryStream inStream, short length)
    {
        Vector2 vo = new Vector2();
        lock (memVector2)
        {
            inStream.Read(memVector2, 0, 2 * 4);
            int index = 0;
            Protocol.Deserialize(out vo.x, memVector2, ref index);
            Protocol.Deserialize(out vo.y, memVector2, ref index);
        }

        return vo;
    }


    public static readonly byte[] memQuarternion = new byte[4 * 4];
    private static short SerializeQuaternion(MemoryStream outStream, object customobject)
    {
        Quaternion o = (Quaternion)customobject;

        lock (memQuarternion)
        {
            byte[] bytes = memQuarternion;
            int index = 0;
            Protocol.Serialize(o.w, bytes, ref index);
            Protocol.Serialize(o.x, bytes, ref index);
            Protocol.Serialize(o.y, bytes, ref index);
            Protocol.Serialize(o.z, bytes, ref index);
            outStream.Write(bytes, 0, 4 * 4);
        }

        return 4 * 4;
    }

    private static object DeserializeQuaternion(MemoryStream inStream, short length)
    {
        Quaternion o = new Quaternion();

        lock (memQuarternion)
        {
            inStream.Read(memQuarternion, 0, 4 * 4);
            int index = 0;
            Protocol.Deserialize(out o.w, memQuarternion, ref index);
            Protocol.Deserialize(out o.x, memQuarternion, ref index);
            Protocol.Deserialize(out o.y, memQuarternion, ref index);
            Protocol.Deserialize(out o.z, memQuarternion, ref index);
        }

        return o;
    }

    public static readonly byte[] memPlayer = new byte[4];
    private static short SerializePhotonPlayer(MemoryStream outStream, object customobject)
    {
        int ID = ((PhotonPlayer)customobject).ID;

        lock (memPlayer)
        {
            byte[] bytes = memPlayer;
            int off = 0;
            Protocol.Serialize(ID, bytes, ref off);
            outStream.Write(bytes, 0, 4);
            return 4;
        }
    }

    private static object DeserializePhotonPlayer(MemoryStream inStream, short length)
    {
        int ID;
        lock (memPlayer)
        {
            inStream.Read(memPlayer, 0, length);
            int off = 0;
            Protocol.Deserialize(out ID, memPlayer, ref off);
        }

        if (PhotonNetwork.networkingPeer.mActors.ContainsKey(ID))
        {
            return PhotonNetwork.networkingPeer.mActors[ID];
        }
        else
        {
            return null;
        }
    }

    public static readonly byte[] memUint = new byte[4];
    private static short SerializeUint(MemoryStream outStream, object customObject)
    {
        lock (memUint)
        {
            byte[] bytes = memUint;
            int off = 0;
            uint val = (uint) customObject;
            Protocol.Serialize((int)val, bytes, ref off);
            outStream.Write(bytes, 0, 4);
            return 4;
        }
    }

    private static object DeserializeUint(MemoryStream inStream, short length)
    {
        int intVal;
        lock (memUint)
        {
            inStream.Read(memUint, 0, length);
            int off = 0;
            Protocol.Deserialize(out intVal, memUint, ref off);
        }

        return (uint) intVal;
    }

    public static readonly byte[] memPhotonView = new byte[4];
    private static short SerializePhotonView(MemoryStream outStream, object customObject)
    {
        lock (memPhotonView)
        {
            byte[] bytes = memPhotonView;
            int off = 0;

            PhotonView photonView = customObject as PhotonView;

            Protocol.Serialize(photonView == null ? 0 : photonView.viewID, bytes, ref off);

            outStream.Write(bytes, 0, 4);

            return 4;
        }
    }

    private static object DeserializePhotonView(MemoryStream inStream, short length)
    {
        int viewId;

        lock (memPhotonView)
        {
            inStream.Read(memPhotonView, 0, 4);

            int off = 0;

            Protocol.Deserialize(out viewId, memPhotonView, ref off);
        }

        if (viewId == 0)
            return null;

        PhotonView view = PhotonView.Find(viewId);
        return view;
    }

    private static short SerializePhotonStream(MemoryStream outStream, object customObject)
    {
        PhotonStream stream = (PhotonStream)customObject;
        object[] data = stream.data.ToArray();

        outStream.WriteByte((byte) data.Length);

        short byteCount = 1;
        foreach (object o in data)
        {
            byte[] serializedData = Protocol.Serialize(o);
            outStream.WriteByte((byte)serializedData.Length);
            outStream.Write(serializedData, 0, serializedData.Length);

            byteCount += (short)(serializedData.Length + 1);
        }

        return byteCount;
    }

    private static object DeserializePhotonStream(MemoryStream inStream, short length)
    {
        int objectCount = inStream.ReadByte();
        object[] data = new object[objectCount];

        for (int i = 0; i < objectCount; i++)
        {
            int objectByteCount = inStream.ReadByte();
            byte[] serializedObject = new byte[objectByteCount];

            inStream.Read(serializedObject, 0, objectByteCount);

            data[i] = Protocol.Deserialize(serializedObject);
        }

        return new PhotonStream(false, data);
    }

    #endregion
}
