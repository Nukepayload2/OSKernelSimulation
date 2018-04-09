Public Class CPUDispatcherCommandList
    Public ReadOnly Property Processes As New ObservableCollection(Of ProcessStartInfo)
    Public Sub CreateProcess(start As ProcessStartInfo)
        Processes.Add(start)
    End Sub
    ''' <summary>
    ''' 重置Job状态以便保存
    ''' </summary>
    Public Sub Reset()
        For Each proc In Processes
            Dim timeline = proc.InitialTimeline
            timeline.JobIndex = 0
            timeline.ShouldYield = False
            For Each job In timeline.Jobs
                job.Progress = 0
            Next
        Next
    End Sub
End Class