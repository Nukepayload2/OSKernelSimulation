''' <summary>
''' 记录线程执行某次计算的状态
''' </summary>
Public Class ThreadSnapshot
    Public Property WorkTime As Integer
    Public Property PieceAt As Integer
    Public Property RunningState As ThreadStateSilm
    Public Property Priority As ThreadPriorityLevel
End Class