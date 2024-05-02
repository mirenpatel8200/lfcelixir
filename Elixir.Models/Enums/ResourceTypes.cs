using System;
using System.Collections.Generic;
using System.Linq;
using Elixir.Utils;

namespace Elixir.Models.Enums
{
    [Flags]
    public enum ResourceTypes
    {

        Unknown = 1, // 99
        Organisation = 2, // 1
        Person = 4, // 2
        Creation = 8, // 3
        Other = 16//,//  9
        //All = 31
    }

    public static class ResourceTypesUtils
    {
        private static Dictionary<ResourceTypes, int> _lookup = new Dictionary<ResourceTypes, int>()
        {
            { ResourceTypes.Unknown, 99},
            { ResourceTypes.Organisation, 1},
            { ResourceTypes.Person, 2},
            { ResourceTypes.Creation, 3},
            { ResourceTypes.Other, 9}//,
            //{ ResourceTypes.All, 11}
        };

        /// <summary>
        /// Returns array of integers which represent each resource type value in database.
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public static int[] ToDatabaseValues(this ResourceTypes types)
        {
            //return types == ResourceTypes.All
            //    ? new int[] { 11 }
            //    : types.GetFlags().Cast<ResourceTypes>().Select(x => _lookup[x]).ToArray();
            return types.GetFlags().Cast<ResourceTypes>().Select(x => _lookup[x]).ToArray(); 
        }
    }
}
