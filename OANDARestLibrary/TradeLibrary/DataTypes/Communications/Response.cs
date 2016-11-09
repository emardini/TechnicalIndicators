namespace OANDARestLibrary.TradeLibrary.DataTypes.Communications
{
    using System.Reflection;
    using System.Text;

    using OANDARestLibrary.Framework;

    public class Response
    {
        #region Public Methods and Operators

        public override string ToString()
        {
            // use reflection to display all the properties that have non default values
            var result = new StringBuilder();
            var props = this.GetType().GetTypeInfo().DeclaredProperties;
            result.AppendLine("{");
            foreach (var prop in props)
            {
                if (prop.Name != "Content" && prop.Name != "Subtitle" && prop.Name != "Title" && prop.Name != "UniqueId")
                {
                    var value = prop.GetValue(this);
                    var valueIsNull = value == null;
                    var defaultValue = Common.GetDefault(prop.PropertyType);
                    var defaultValueIsNull = defaultValue == null;
                    if ((valueIsNull != defaultValueIsNull) // one is null when the other isn't
                        || (!valueIsNull && (value.ToString() != defaultValue.ToString()))) // both aren't null, so compare as strings
                    {
                        result.AppendLine(prop.Name + " : " + prop.GetValue(this));
                    }
                }
            }
            result.AppendLine("}");
            return result.ToString();
        }

        #endregion
    }
}