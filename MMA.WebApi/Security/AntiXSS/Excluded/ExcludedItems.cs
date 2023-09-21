using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace MMA.WebApi.Security.AntiXSS.Excluded
{
    public static class ExcludedItems
    {
        #region Flags
        public static bool ExcludeMethodsFlag { get; set; } = true;
        public static bool ExcludePathsFlag { get; set; } = true;
        #endregion

        #region WhatIsExcluded
        public static List<string> ExcludedMethods = new List<string> { HttpMethods.Get };
        public static List<string> ExcludedPaths = new List<string> { "/api/document/upload" };
        #endregion

        #region Methods
        //Be aware of these 2 methods calling hierarchy
        public static bool isPathExcluded(string path) =>
            ExcludedPaths.Contains(path) ? true : false;

        public static bool isMethodExcluded(string method) =>
            ExcludedMethods.Contains(method) ? true : false;
        #endregion
    }
}
