using System.Web.Mvc;

namespace Elixir.Views
{
    public static partial class ViewDataUtils
    {
        //TODO: change head type according to current user's role
        
        public static TType GetValue<TType>(this ViewDataDictionary viewData, ViewDataKeys key)
        {
            var keyString = key.ToString();
            return viewData.ContainsKey(keyString) ? (TType)viewData[keyString] : default(TType);
        }

        public static void AddOrUpdateValue<TType>(this ViewDataDictionary viewData, ViewDataKeys key, TType value)
        {
            var keyString = key.ToString();
            if (viewData.ContainsKey(keyString) == false)
                viewData.Add(keyString, value);
            else
                viewData[keyString] = value;
        }
    }
}