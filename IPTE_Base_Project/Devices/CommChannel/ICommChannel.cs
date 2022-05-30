using System;

namespace IPTE_Base_Project.Devices.CommChannel
{

    public interface ICommChannel
    {
        event EventHandler CommChannelError;

        bool IsInitialized();
        
        void WriteCommand(string command);
        void WriteCommand(string commands, char separator);
        string ReadResponse();
        double ReadResponseDouble();
        double QueryDouble(string command);
        string Query(string command);
        //string Query(string commands, char separator);
        bool Initialize();
        void Terminate();
        bool Reset();

        void SessionReset();
    }
    
}

