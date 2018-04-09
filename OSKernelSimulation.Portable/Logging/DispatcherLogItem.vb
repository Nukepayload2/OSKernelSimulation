Public Class DispatcherLogItem
    Sub New(snapshots As IEnumerable(Of ProcessSnapshot), queuesDescription As String, currentProcessTime As Integer)
        Me.CurrentProcessTime = currentProcessTime
        Me.Snapshots = snapshots
        Me.QueuesDescription = queuesDescription
    End Sub

    Public Property CurrentProcessTime As Integer
    Public Property Snapshots As IEnumerable(Of ProcessSnapshot)
    Public Property QueuesDescription As String
End Class