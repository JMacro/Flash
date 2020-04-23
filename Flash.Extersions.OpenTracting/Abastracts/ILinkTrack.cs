using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.Extersions.OpenTracting
{
    public interface ILinkTrack : IDisposable
    {
        
        void LogRequest(dynamic value);
        void LogResponse(dynamic value);
        void LogException(Exception ex);        
        void Log(string key, dynamic value);
        void SetTag(string key, dynamic value);
        void SetError();
    }
}
