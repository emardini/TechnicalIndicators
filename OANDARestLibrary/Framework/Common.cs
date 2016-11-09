namespace OANDARestLibrary.Framework
{
    using System;
    using System.Reflection;

    public class Common
    {
        #region Public Methods and Operators

        public static object GetDefault(Type t)
        {
            return typeof(Common).GetTypeInfo().GetDeclaredMethod("GetDefaultGeneric").MakeGenericMethod(t).Invoke(null, null);
        }

        public static T GetDefaultGeneric<T>()
        {
            return default(T);
        }

        #endregion
    }
}