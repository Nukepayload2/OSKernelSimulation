Imports System.Text

Public Class DispatcherLogManager
    ''' <summary>
    ''' 标头
    ''' </summary>
    Public ReadOnly Property Headers As New ObservableCollection(Of String)
    ''' <summary>
    ''' 已经写入的日志
    ''' </summary>
    Public ReadOnly Property Logs As New ObservableCollection(Of DispatcherLogItem)
    ''' <summary>
    ''' 写进程快照
    ''' </summary>
    Public Sub Write(log As IEnumerable(Of ProcessSnapshot), queuesDescription As String, currentProcessTime As Integer)
        Logs.Add(New DispatcherLogItem(log, queuesDescription, currentProcessTime))
    End Sub
    ''' <summary>
    ''' 重置日志管理器
    ''' </summary>
    Public Sub Reset()
        Headers.Clear()
        Logs.Clear()
    End Sub
End Class