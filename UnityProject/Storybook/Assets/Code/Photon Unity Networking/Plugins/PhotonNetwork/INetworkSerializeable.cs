public interface INetworkSerializeable
{
    void OnSerialize(PhotonStream stream);
    void OnDeserialize(PhotonStream stream);
}