''' <summary>
''' 模拟CPU调度。此类是单实例的。
''' </summary>
Public Class CPUDispatcherSimulator
    ''' <summary>
    ''' 获取缓存的<see cref="CPUDispatcherSimulator"/>的实例
    ''' </summary>
    Public Shared ReadOnly Property Current As CPUDispatcherSimulator
    Sub New()
        _Current = Me
    End Sub
    ' 0 保留为系统中断
    Dim _PIdSeed As UInteger = 0
    ''' <summary>
    ''' 下一个可用的 Process Id
    ''' </summary>
    Friend ReadOnly Property PIdSeed As UInteger
        Get
            _PIdSeed += 4UI
            Return _PIdSeed
        End Get
    End Property
    ''' <summary>
    ''' 需要调度的核心只有一个
    ''' </summary>
    Public Property Core As New CPUCoreDispatcherSimulator

    ''' <summary>
    ''' 在另一个线程上连续执行模拟，并启用延时。
    ''' </summary>
    ''' <param name="delayMillsec">延时的毫秒数</param>
    Public Async Function RunAsync(startCommand As CPUDispatcherCommandList, delayMillsec As Integer) As Task
        Core.Reset()
        startCommand.Reset()
        Core.CurrentCommandList = startCommand
        Do While Core.RunSingleStep
            Await Task.Delay(delayMillsec)
        Loop
    End Function
End Class