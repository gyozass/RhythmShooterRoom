public enum HitType { Perfect, Good, Okay, Miss }

public struct HitResult
{
    public float offset;
    public HitType type;
}