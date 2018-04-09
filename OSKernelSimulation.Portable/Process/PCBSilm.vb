'（1）	如何组织进程：
'确定PCB内容标识信息、状态和运行时间与存储地址等信息、现场信息、管理信息
'PCB组织方式：相同状态的进程PCB构成一个队列（即有空闲、就绪、运行、阻塞和完成5个队列）

''' <summary>
''' 简易的进程控制块
''' </summary>
Public Class PCBSilm
    ''' <summary>
    ''' 进程身份标识
    ''' </summary>
    Public Property Id As UInteger

    Dim _Name$
    ''' <summary>
    ''' 进程的名字
    ''' </summary>
    Public Property Name$
        Get
            Return _Name
        End Get
        Friend Set(value$)
            _Name = value
        End Set
    End Property

    ''' <summary>
    ''' 主线程
    ''' </summary>
    Public Property MainThread As ThreadSimulator
    ''' <summary>
    ''' 创建线程时使用的默认优先级
    ''' </summary>
    Public Property BasePriority As ThreadPriorityLevel

    Dim _TIdSeed As UInteger = 0
    ''' <summary>
    ''' 下一个可用的 Thread Id
    ''' </summary>
    Friend ReadOnly Property TIdSeed As UInteger
        Get
            _TIdSeed += 4UI
            Return _TIdSeed
        End Get
    End Property
End Class