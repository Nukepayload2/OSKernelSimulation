Public Enum ThreadPriorityLevel
    ''' <summary>
    ''' 对于线程上的任务来说, 这是位于时间片队列最末的一组
    ''' </summary>
    Lowest = -2
    ''' <summary>
    ''' 低于普通
    ''' </summary>
    BelowNormal = -1
    ''' <summary>
    ''' 普通
    ''' </summary>
    Normal = 0
    ''' <summary>
    ''' 高于普通
    ''' </summary>
    AboveNormal = 1
    ''' <summary>
    ''' 时间片最前的一组
    ''' </summary>
    Highest = 2
End Enum