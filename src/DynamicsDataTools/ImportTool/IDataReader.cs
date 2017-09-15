using Microsoft.Xrm.Sdk;
using System.Collections.Generic;

namespace DynamicsDataTools.ImportTool
{
    public interface IDataReader
    {
        string Extension { get; }
        IList<Entity> Read(string fileName);
    }
}