using IPTE_Base_Project.DataSources;

namespace IPTE_Base_Project.Devices
{
    interface IMultiplexer
    {
        MultiplexerType Type { get; set; }
        string Topology { get; set; }
    }
}
