namespace OANDARestLibrary.TradeLibrary.DataTypes.Communications.Requests
{
    using System.Reflection;
    using System.Text;

    // Functionally very similar to System.Nullable, could possibly just replace this

    public abstract class Request
    {
        #region Public Properties

        public abstract string EndPoint { get; }

        #endregion

        #region Public Methods and Operators

        public string GetRequestString()
        {
            var result = new StringBuilder();
            result.Append(this.EndPoint);
            var firstJoin = true;
            foreach (var declaredField in this.GetType().GetTypeInfo().DeclaredFields)
            {
                var prop = declaredField.GetValue(this);
                var smartProp = prop as ISmartProperty;
                if (smartProp != null && smartProp.HasValue)
                {
                    if (firstJoin)
                    {
                        result.Append("?");
                        firstJoin = false;
                    }
                    else
                    {
                        result.Append("&");
                    }

                    result.Append(declaredField.Name + "=" + prop);
                }
            }
            return result.ToString();
        }

        public abstract EServer GetServer();

        #endregion
    }
}