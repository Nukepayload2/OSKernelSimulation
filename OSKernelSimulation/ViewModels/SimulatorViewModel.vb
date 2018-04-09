Imports OSKernelSimulation.Portable

Public Class SimulatorViewModel
    Public Shared ReadOnly Property Current As SimulatorViewModel
    Sub New()
        _Current = Me
    End Sub

    Public Property DelayTime%
    Public Property Simulator As New CPUDispatcherSimulator
    Public Property SimulatorStartCommands As New CPUDispatcherCommandList
    Public Async Function SaveLogAsync() As Task
        Dim saver As New Windows.Storage.Pickers.FileSavePicker With {.CommitButtonText = "导出"}
        saver.FileTypeChoices.Add("json日志文件", {".json"})
        Dim file = Await saver.PickSaveFileAsync()
        Using strm = Await file.OpenStreamForWriteAsync
            Await AppCaching.SaveAsync(strm, Simulator.Core.Snapshots)
        End Using
    End Function
End Class