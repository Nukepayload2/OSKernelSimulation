''' <summary>
''' 模拟一个进程。请使用<see cref="CPUCoreDispatcherSimulator"/>进行初始化。
''' </summary>
Public Class ProcessSimulator
    ''' <summary>
    ''' 包含这个进程的上下文信息。如果修改不当，则会出现未定义的行为。
    ''' </summary>
    Public Property ControlBlock As PCBSilm

    ''' <summary>
    ''' 仅在反序列化时调用
    ''' </summary>
    Public Sub New()
    End Sub
    ''' <summary>
    ''' 创建一个进程，并加入<see cref="ThreadStateSilm.Initialized"/>队列中。
    ''' </summary>
    Public Shared Function Create(startInfo As ProcessStartInfo) As ProcessSimulator
        Return Create(startInfo.Name, startInfo.BasePriority, startInfo.InitialTimeline)
    End Function
    ''' <summary>
    ''' 创建一个进程，并加入<see cref="ThreadStateSilm.Initialized"/>队列中。
    ''' </summary>
    ''' <param name="basePriority">进程默认的优先级</param>
    Public Shared Function Create(name$, basePriority As ThreadPriorityLevel, jobStartInfo As JobTimeline) As ProcessSimulator
        Dim proc As New ProcessSimulator
        proc.ControlBlock = New PCBSilm With {
            .Name = name,
            .BasePriority = basePriority,
            .Id = CPUDispatcherSimulator.Current.PIdSeed
        }
        Dim thr As New ThreadSimulator With {
            .Jobs = jobStartInfo,
            .ControlBlock = New TCBSilm With {
                .Priority = basePriority,
                .Id = proc.ControlBlock.TIdSeed
            }
        }
        proc.ControlBlock.MainThread = thr
        CPUDispatcherSimulator.Current.Core.Queues(thr.ControlBlock.Priority)(ThreadStateSilm.Initialized).Enqueue(proc)
        Return proc
    End Function
    ''' <summary>
    ''' 将此进程启动, 移除<see cref="ThreadStateSilm.Initialized"/>并添加到<see cref="ThreadStateSilm.Ready"/>队列中。
    ''' </summary>
    Public Sub Start()
        If ControlBlock.MainThread.ControlBlock.State = ThreadStateSilm.Initialized Then
            Dim jobs = ControlBlock.MainThread.Jobs.Jobs
            If jobs.Count > 0 Then
                ControlBlock.MainThread.State = If(jobs.First.IsIOOperation, ThreadStateSilm.Blocked, ThreadStateSilm.Ready)
            Else
                ControlBlock.MainThread.State = ThreadStateSilm.Terminated
            End If
        Else
            Throw New SimulationException($"PId为{ControlBlock.Id}的进程{ControlBlock.Name}已经启动了。不要进行冗余的启动。")
        End If
    End Sub
    ''' <summary>
    ''' 执行任务。
    ''' </summary>
    Public Function DoWork(Optional workTime As Integer? = Nothing) As ThreadSnapshot
        Return ControlBlock.MainThread.DoWork(workTime)
    End Function
End Class