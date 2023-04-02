using System;
using System.Runtime.InteropServices;

namespace HazelcastRtd
{
    //
    // We provide definition of the RTD interfaces used by Excel
    // directly in this C# file. This way our assembly will not
    // have a dependency on Excel's type library.
    //
    
    
    
    [Guid("A43788C1-D91B-11D3-8F39-00C04F3651B8")]  // DO NOT CHANGE: declared by excel
    public interface IRtdUpdateEvent
    {
        void UpdateNotify ();
        int HeartbeatInterval { get; set; }
        void Disconnect ();
    }

    [Guid("EC0E6191-DB51-11D3-8F3E-00C04F3651B8")]  // DO NOT CHANGE: declared by excel
    public interface IRtdServer
    {
        public int ServerStart (IRtdUpdateEvent callback);

        public object ConnectData (int topicId,
                           [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)]
                           ref Array strings,
                           ref bool newValues);

        [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)] 
        public Array RefreshData (ref int topicCount);

        public void DisconnectData (int topicId);

        public int Heartbeat ();

        public void ServerTerminate ();
    }
}
