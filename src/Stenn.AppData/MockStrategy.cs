using System;

namespace Stenn.AppData
{
    /// <summary>
    /// 
    /// </summary>
    public enum MockStrategy
    {
        /// <summary>
        /// Real app data service will be used
        /// </summary>
        None = 0,
        
        /// <summary>
        /// Mock that always returns empty queriable will be used as an app data service 
        /// </summary>
        Empty,
        
        /// <summary>
        /// Mock that always throws <see cref="NotImplementedException"/> will be used as an app data service 
        /// </summary>
        NotImplemented,
        
        /// <summary>
        /// Custom mock implemented by producer will be used as an app data service 
        /// </summary>
        Custom,
    }
}