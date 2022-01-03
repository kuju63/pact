namespace Pact.Cli.Utils;

public static class DynamicUtil
{
    public static bool Is<T>(dynamic obj, out T? castValue)
    {
        try
        {
            if (typeof(T).IsPrimitive)
            {
                if (obj is T)
                {
                    castValue = obj;
                    return true;
                }
                else
                {
                    castValue = default;
                    return false;
                }
            }
            else if (obj is T)
            {
                castValue = obj;
                return true;
            }

            castValue = default;
            return false;
        }
        catch (InvalidCastException)
        {
            castValue = default;
            return false;
        }
    }
}
