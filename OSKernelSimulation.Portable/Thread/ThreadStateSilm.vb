Public Enum ThreadStateSilm
    ''' <summary>
    ''' 还没开始运行，但是已经完成初始化了
    ''' </summary>
    Initialized
    ''' <summary>
    ''' 准备好执行后面的指令
    ''' </summary>
    Ready
    ''' <summary>
    ''' 运行指令中
    ''' </summary>
    Running
    ''' <summary>
    ''' 在等待IO
    ''' </summary>
    Blocked
    ''' <summary>
    ''' 结束了
    ''' </summary>
    Terminated
End Enum