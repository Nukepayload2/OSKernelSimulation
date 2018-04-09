''' <summary>
''' 简易的线程控制块
''' </summary>
Public Class TCBSilm
    ''' <summary>
    ''' 线程身份标识
    ''' </summary>
    Public Property Id As UInteger
    ''' <summary>
    ''' 线程状态
    ''' </summary>
    Public Property State As ThreadStateSilm
    ''' <summary>
    ''' 线程的优先级
    ''' </summary>
    Public Property Priority As ThreadPriorityLevel

End Class