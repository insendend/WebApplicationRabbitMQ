namespace ClassesLib.Serialization
{
    public interface ISerializer<T>
    {
        byte[] Serialize(T obj);

        T Desirialize(byte[] bytes);
    }
}
