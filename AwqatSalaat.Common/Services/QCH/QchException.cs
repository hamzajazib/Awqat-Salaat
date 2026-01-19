using System;

namespace AwqatSalaat.Services.QCH
{
    public class QchException : Exception
    {
        public QchException() : base() { }
        public QchException(string message) : base(message) { }
    }
}
