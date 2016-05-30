using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendanceRRHH.BLL
{
    public interface ILogger
    {
        void Debug(string message);
        void Trace(string message);
        void Info(string message);
        void Warning(string message);
        void Error(string message);
        void Error(string message, Exception exception);
        void Fatal(string message);
        void Fatal(string message, Exception exception);
    }
}