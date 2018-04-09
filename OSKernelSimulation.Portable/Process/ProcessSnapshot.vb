Public Class ProcessSnapshot
    Sub New(mainThreadSnapshot As ThreadSnapshot, processId As UInteger, processName As String)
        Me.MainThreadSnapshot = mainThreadSnapshot
        Me.ProcessId = processId
        Me.ProcessName = processName
    End Sub

    Public Property MainThreadSnapshot As ThreadSnapshot
    Public Property ProcessId As UInteger
    Public Property ProcessName$
End Class
