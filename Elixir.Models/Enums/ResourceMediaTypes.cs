using System;
using System.Collections.Generic;

namespace Elixir.Models.Enums
{
    //TODO: use it on GetResourcesByTopicAndType BL method
    public class ResourceMediaTypes
    {
        private List<ResourceMediaType> _filter { get; set; }

        public ResourceMediaTypes(List<ResourceMediaType> filter)
        {
            _filter = filter;
        }

        public string ToSqlFilter()
        {
            if (_filter == null || _filter.Count == 0)
                return null;
            string condition = "";
            for (int i = 0; i < _filter.Count; i++)
            {
                condition += $"{Enum.GetName(typeof(ResourceMediaType), _filter[i])} = TRUE";
                if (i < _filter.Count - 1)
                {
                    condition += " OR ";
                }    
            }
            
            return condition;
        }
    }

    public enum ResourceMediaType
    {
        IsBook,
        IsFilm
    }
}
