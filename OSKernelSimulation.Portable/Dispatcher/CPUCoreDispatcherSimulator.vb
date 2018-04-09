Imports System.Text
Imports OSKernelSimulation.Portable
''' <summary>
''' 模拟为一个CPU核心编写的采用多级队列反馈式调度策略调度。会随着<see cref="CPUDispatcherSimulator"/>一起实例化。
''' </summary>
Public Class CPUCoreDispatcherSimulator
    ''' <summary>
    ''' 采用多级队列反馈式调度策略的进程队列
    ''' </summary>
    Public ReadOnly Property Queues As New Dictionary(Of ThreadPriorityLevel, Dictionary(Of ThreadStateSilm, Queue(Of ProcessSimulator)))
    Shared states As ThreadStateSilm() = GetOrderedEnumValues(Of ThreadStateSilm)(),
           levels As ThreadPriorityLevel() = GetOrderedEnumValues(Of ThreadPriorityLevel)(),
           threadStateExceptTerminated As ThreadStateSilm() = states.Take(4).ToArray

    Sub New()
        InitializeQueues()
    End Sub

    Protected Sub InitializeQueues()
        For Each l In levels
            Dim stateDic As New Dictionary(Of ThreadStateSilm, Queue(Of ProcessSimulator))
            Queues.Add(l, stateDic)
            For Each state In states
                stateDic.Add(state, New Queue(Of ProcessSimulator))
            Next
        Next
    End Sub

    ''' <summary>
    ''' 重置模拟状态
    ''' </summary>
    Public Sub Reset()
        Queues.Clear()
        InitializeQueues()
        LastProcessTime = -1
        CurrentProcessTime = 0
        Snapshots.Reset()
    End Sub
    ''' <summary>
    ''' 是否未完成全部进程
    ''' </summary>
    Public ReadOnly Property IsUnfinished As Boolean
        Get
            Return Aggregate st In Queues.Values From lv In st Where lv.Key <> ThreadStateSilm.Terminated Where lv.Value.Any Into Any
        End Get
    End Property

    Dim LastProcessTime% = -1
    Dim CurrentProcessTime% = 0
    ''' <summary>
    ''' 开始执行之前，设置启动命令
    ''' </summary>
    ''' <returns></returns>
    Public Property CurrentCommandList As CPUDispatcherCommandList
    ''' <summary>
    ''' 执行状态快照，用于调试调度程序
    ''' </summary>
    Public Property Snapshots As New DispatcherLogManager
    ''' <summary> 
    ''' 单步执行。完成会返回 False
    ''' </summary>
    Friend Function RunSingleStep() As Boolean
        For Each proc In CurrentCommandList.Processes
            If proc.StartRound > LastProcessTime AndAlso proc.StartRound <= CurrentProcessTime Then
                Snapshots.Headers.Add(proc.Name)
                ProcessSimulator.Create(proc)
            End If
        Next
        Dim procLogs As New List(Of ProcessSnapshot)
        LastProcessTime = CurrentProcessTime
        Dim worked = False
        Dim waitYielded = False
        Dim fixedWorkTime As New Integer?
        Dim workedPid = 0UI
        '拍摄进程快照
        Dim queueDebugInfo As New StringBuilder
        For Each level In levels
            Dim stateDic = Queues(level)
            For Each state In states
                For Each proc In stateDic(state)
                    queueDebugInfo.AppendLine($"在优先级{level}, 进程状态{state}, 有进程Id为{proc.ControlBlock.Id}的进程")
                Next
            Next
        Next
        '处理需要启动和执行的进程
        EnumProcess(Sub(state, stateQueue, proc)
                        Select Case state
                            Case ThreadStateSilm.Initialized
                                proc.Start()
                                '抢占式执行前需要插队到就绪队列第一个
                                Dim readyQueue = Queues(proc.ControlBlock.BasePriority)(ThreadStateSilm.Ready)
                                For i = 0 To readyQueue.Count - 2
                                    readyQueue.Enqueue(readyQueue.Dequeue)
                                Next
                                WorkProcess(proc, procLogs, worked, fixedWorkTime, workedPid)
                                RunSingleStep = True
                            Case ThreadStateSilm.Ready
                                WorkProcess(proc, procLogs, worked, fixedWorkTime, workedPid)
                                RunSingleStep = True
                            Case ThreadStateSilm.Blocked
                                RunSingleStep = True
                            Case Else
                                Throw New SimulationException("提供了错误的线程状态")
                        End Select
                    End Sub)
        '处理在等待的进程
        EnumProcess(Sub(state, stateQueue, proc)
                        Select Case state
                            Case ThreadStateSilm.Initialized, ThreadStateSilm.Ready
                                RunSingleStep = True
                            Case ThreadStateSilm.Blocked
                                '每一个线程都经历等待IO操作，并且未完成等待的应该留下快照。
                                For Each blocked In stateQueue
                                    Dim ctlBlk = blocked.ControlBlock
                                    Dim jobs = ctlBlk.MainThread.Jobs
                                    If workedPid <> ctlBlk.Id AndAlso Not jobs.ShouldYield Then
                                        Dim threadSnapshot = blocked.DoWork(fixedWorkTime)
                                        If threadSnapshot.RunningState <> ThreadStateSilm.Blocked Then
                                            Throw New SimulationException("检测到阻塞队列中的非阻塞进程")
                                        End If
                                        procLogs.Add(New ProcessSnapshot(threadSnapshot, ctlBlk.Id, ctlBlk.Name))
                                        Debug.WriteLine($"Process={ctlBlk.Name}, Count={jobs.Jobs.Count}, Index={jobs.JobIndex}")
                                    End If
                                Next
                                '每次只能切换一个等待的进程
                                If Not waitYielded AndAlso workedPid <> proc.ControlBlock.Id Then
                                    Dim mainThread = proc.ControlBlock.MainThread
                                    Dim topJob = mainThread.Jobs
                                    If topJob.ShouldYield Then
                                        topJob.Yield() '如果等待完了，切换任务到下一个
                                        Debug.WriteLine($"切换了进程：{proc.ControlBlock.Name}")
                                        mainThread.Yield() '移出Blocked队列, 然后看看应该移动到哪里
                                        waitYielded = True
                                    End If
                                End If
                                RunSingleStep = True
                            Case Else
                                Throw New SimulationException("提供了错误的线程状态")
                        End Select
                    End Sub)
        '写日志以便显示
        If procLogs.Any Then Snapshots.Write(procLogs, queueDebugInfo.ToString, CurrentProcessTime)
        Return RunSingleStep
    End Function

    Private Sub WorkProcess(proc As ProcessSimulator, procLogs As List(Of ProcessSnapshot), ByRef worked As Boolean, ByRef fixedWorkTime As Integer?, ByRef workedPid As UInteger)
        If Not worked Then
            Dim threadSnapshot = proc.DoWork()
            fixedWorkTime = threadSnapshot.WorkTime
            CurrentProcessTime += threadSnapshot.WorkTime
            procLogs.Add(New ProcessSnapshot(threadSnapshot, proc.ControlBlock.Id, proc.ControlBlock.Name))
            Dim STAThread = proc.ControlBlock.MainThread
            Dim priority As ThreadPriorityLevel
            Select Case STAThread.Priority
                Case ThreadPriorityLevel.Highest
                    priority = ThreadPriorityLevel.AboveNormal
                Case ThreadPriorityLevel.AboveNormal
                    priority = ThreadPriorityLevel.Normal
                Case ThreadPriorityLevel.Normal
                    priority = ThreadPriorityLevel.BelowNormal
                Case ThreadPriorityLevel.BelowNormal, ThreadPriorityLevel.Lowest
                    priority = ThreadPriorityLevel.Lowest
                Case Else
                    Throw New SimulationException($"进程 {proc.ControlBlock.Name} [{proc.ControlBlock.Id}] 的主线程 (id为{STAThread.ControlBlock.Id}) 优先级数据损坏。")
            End Select
            Debug.WriteLine($"执行了进程：{proc.ControlBlock.Name}")
            STAThread.Yield(priority)
            worked = True
            workedPid = proc.ControlBlock.Id
        End If
    End Sub

    Private Sub EnumProcess(processCallback As ProcessDelegate)
        For Each l In levels
            Dim stateDic = Queues(l)
            For Each state In threadStateExceptTerminated
                Dim stateQueue = stateDic(state)
                If stateQueue.Any Then
                    Dim proc = stateQueue.Peek
                    If state <> proc.ControlBlock.MainThread.State Then
                        Throw New SimulationException("检测到漏掉移动队列的进程")
                    End If
                    processCallback(state, stateQueue, proc)
                End If
            Next
        Next
    End Sub

End Class

Delegate Sub ProcessDelegate(state As ThreadStateSilm, stateQueue As Queue(Of ProcessSimulator), proc As ProcessSimulator)