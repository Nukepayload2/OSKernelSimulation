''' <summary>
''' 模拟一个线程。在<see cref="ProcessSimulator"/>初始化时会作为主线程随着它初始化。
''' </summary>
Public Class ThreadSimulator
    Implements INotifyPropertyChanged

    ''' <summary>
    ''' 包含这个线程的上下文信息。如果修改不当，则会出现未定义的行为。
    ''' </summary>
    Public Property ControlBlock As TCBSilm
    ''' <summary>
    ''' 线程需要进行的工作
    ''' </summary>
    Public Property Jobs As JobTimeline

    ''' <summary>
    ''' 仅在反序列化时调用
    ''' </summary>
    Sub New()
    End Sub

    Public ReadOnly Property IsMainThread As Boolean = True

    ''' <summary>
    ''' 获取或切换线程状态。主线程切换状态会改变调度器中的队列。使用限制：调节状态的对象必须位于队列顶端。
    ''' </summary>
    Public Property State As ThreadStateSilm
        Get
            Return ControlBlock.State
        End Get
        Set(value As ThreadStateSilm)
            If IsMainThread Then
                Dim curQueues = CPUDispatcherSimulator.Current.Core.Queues(ControlBlock.Priority)
                Dim proc = curQueues(State).Dequeue()
                QueueTopBugCheck(proc)
                ControlBlock.State = value
                curQueues(value).Enqueue(proc)
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(State)))
            End If
        End Set
    End Property

    Public Sub UpdateStateAndPriority(state As ThreadStateSilm, priority As ThreadPriorityLevel?)
        If IsMainThread Then
            If priority.HasValue Then
                Dim queues = CPUDispatcherSimulator.Current.Core.Queues
                Dim proc = queues(ControlBlock.Priority)(ControlBlock.State).Dequeue()
                QueueTopBugCheck(proc)
                ControlBlock.State = state
                ControlBlock.Priority = priority.Value
                queues(priority.Value)(state).Enqueue(proc)
            Else
                Me.State = state
            End If
        End If
    End Sub

    Private Sub QueueTopBugCheck(proc As ProcessSimulator)
        If proc.ControlBlock.MainThread IsNot Me Then
            Throw New InvalidOperationException("调节状态的对象必须位于队列顶端。")
        End If
    End Sub

    ''' <summary>
    ''' 获取或切换线程优先级。主线程切换优先级会改变调度器中的队列。
    ''' </summary>
    Public Property Priority As ThreadPriorityLevel
        Get
            Return ControlBlock.Priority
        End Get
        Set(value As ThreadPriorityLevel)
            If IsMainThread Then
                Dim curQueues = CPUDispatcherSimulator.Current.Core.Queues
                Dim proc = curQueues(Priority)(State).Dequeue
                QueueTopBugCheck(proc)
                ControlBlock.Priority = value
                curQueues(value)(State).Enqueue(proc)
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Priority)))
            End If
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    ''' <summary>
    ''' 执行任务，返回实际执行的快照
    ''' </summary>
    Friend Function DoWork(workTime As Integer?) As ThreadSnapshot
        Dim snapshot As New ThreadSnapshot
        workTime = If(workTime, ControlBlock.Priority.GetWorkTime)
        Dim remainTime = 0
        If Jobs Is Nothing Then
            Throw New SimulationException($"未能正确初始化属性{NameOf(Jobs)}")
        Else
            If Jobs.HasFinished Then
                Throw New SimulationException("任务已经完成了，不要试图执行它")
            Else
                Dim preState = ControlBlock.State
                Select Case preState
                    Case ThreadStateSilm.Ready
                        State = ThreadStateSilm.Running
                        remainTime = Jobs.DoWork(workTime.Value)
                        snapshot.RunningState = State
                    Case ThreadStateSilm.Blocked
                        remainTime = Jobs.WaitForAutoResetEvent(workTime.Value)
                        snapshot.RunningState = State
                    Case Else
                        Throw New SimulationException("刚刚创建或者已经终结的进程不能执行任何代码")
                End Select
                snapshot.Priority = Priority
            End If
        End If
        snapshot.WorkTime = workTime.Value - remainTime
        snapshot.PieceAt = Aggregate j In Jobs.Jobs Where Not j.IsIOOperation Take While j.Progress > 0 Select j.Progress Into Sum
        Return snapshot
    End Function

    ''' <summary>
    ''' 导致调用线程执行准备好在当前处理器上运行的另一个线程。
    ''' </summary>
    Public Sub Yield(Optional priority As ThreadPriorityLevel? = Nothing)
        If Jobs.HasFinished Then
            Debug.WriteLine("线程已退出")
            UpdateStateAndPriority(ThreadStateSilm.Terminated, priority)
        Else
            Dim curJob = Jobs.CurrentJob
            If curJob.IsIOOperation Then
                UpdateStateAndPriority(ThreadStateSilm.Blocked, priority)
            Else
                UpdateStateAndPriority(ThreadStateSilm.Ready, priority)
            End If
        End If
    End Sub
End Class