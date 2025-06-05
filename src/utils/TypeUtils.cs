using System;

namespace EvaluationUtils;

public class TypeChecker
{
    public static bool Equals(Type one, Type two)
    {
        return one == two;
    }

    public static bool Matches(Type one, Type[] targets)
    {
        foreach (Type t in targets)
            if (one == t)
                return true;

        return false;
    }
}
