namespace Pact.Cli.Utils;

public static class DynamicUtil
{
    /// <summary>
    /// Safe cast from dynamic type object
    /// </summary>
    /// <param name="obj">cast source object</param>
    /// <typeparam name="T">Casted type</typeparam>
    /// <returns>Retrun casted value, if failed to cast then return default value.</returns>
    public static T? CastTo<T>(dynamic obj)
    {
        if (typeof(T).IsPrimitive)
        {
            if (obj is T val)
            {
                return val;
            }
        }
        else if (obj is T val)
        {
            return val;
        }

        return default(T);
    }

    /// <summary>
    /// Validate argument type
    /// </summary>
    /// <param name="obj">type validation target</param>
    /// <typeparam name="T">Validation type</typeparam>
    /// <returns>True is same type between argument type and type parameter, otherwise false.</returns>
    public static bool Is<T>(dynamic obj)
    {
        try
        {
            if (typeof(T).IsPrimitive)
            {
                if (obj is T)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (obj is T)
            {
                return true;
            }

            return false;
        }
        catch (InvalidCastException)
        {
            return false;
        }
    }
}
