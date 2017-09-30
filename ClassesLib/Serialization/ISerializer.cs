namespace ClassesLib.Serialization
{
    public interface ISerializer<T>
    {
        byte[] SerializeToBytes(T obj);
        T DesirializeToObj(byte[] bytes);

        string SerializeToJson(T obj);
        T DesirializeToObj(string objAsJson);
    }
}
