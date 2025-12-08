using Unity.Entities;

public struct PlayerExpData :IComponentData
{
    public int Current;
    public int Required;
    public int Level;
}
