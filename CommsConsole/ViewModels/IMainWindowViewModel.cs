using ReactiveUI;
using System.Collections.Generic;
using System.Reactive;

namespace CommsConsole.ViewModels
{
    public interface IMainWindowViewModel
    {
        string AppName { get; set; }
        ReactiveCommand<Unit, Unit>? ClosePortCommand { get; }
        ReactiveCommand<Unit, Unit>? OpenPortCommand { get; }
        List<string> PortList { get; set; }
        bool PortOpen { get; set; }
        ReactiveCommand<Unit, Unit>? RefreshPortsCommand { get; }
        string RxPacket { get; set; }
        string SelectedPort { get; set; }
        ReactiveCommand<Unit, Unit>? SendPacketCommand { get; }
        int TargetNode { get; set; }
        int ThisNode { get; set; }
        string Title { get; set; }
        string TxPacket { get; set; }
    }
}