using System;
using System.Runtime.Serialization;

namespace GeoLabAPI.Exceptions
{
    [Serializable]
    public class NotFoundException : Exception
    {
        public NotFoundException() : base("The case does not exist...") { }
    }

    [Serializable]
    public class DuplicateException : Exception
    {
        public DuplicateException() : base("The case entered is duplicate...") { }
    }
}