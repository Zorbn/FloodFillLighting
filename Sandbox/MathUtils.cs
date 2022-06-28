namespace Sandbox;

public class MathUtils
{
    public static byte Lerp(byte start, byte end, float delta)
    {
        return (byte)(start + (end - start) * delta);
    }
}