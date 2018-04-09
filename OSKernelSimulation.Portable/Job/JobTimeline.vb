Imports System.Threading

Public Class JobTimeline
    Sub New()

    End Sub

    ''' <summary>
    ''' 现在任务执行到哪个下标了。如果超出<see cref="Jobs"/>下标，则说明任务完成了。
    ''' </summary>
    Public Property JobIndex%
    ''' <summary>
    ''' 所执行的任务。分计算和IO两大类。
    ''' </summary>
    Public Property Jobs As New ObservableCollection(Of JobTimelineItem)
    Public ReadOnly Property CurrentJob As JobTimelineItem
        Get
            Return If(Jobs.Any, Jobs(JobIndex), Nothing)
        End Get
    End Property

    ''' <summary>
    ''' 任务是否已经完成了
    ''' </summary>
    Public ReadOnly Property HasFinished As Boolean
        Get
            Return Jobs.Count <= JobIndex
        End Get
    End Property
    ''' <summary>
    ''' 表示<see cref="AutoResetEvent"/>是否发生
    ''' </summary>
    Public Property ShouldYield As Boolean

    ''' <summary>
    ''' 进行计算。完成了或者是进行到不同的任务则会返回剩余的执行时间，否则返回0。
    ''' </summary>
    Public Function DoWork(workTime%) As Integer
        If workTime = 0 Then Throw New ArgumentException(NameOf(workTime))
        With CurrentJob
            .Progress += workTime
            Dim remainTime = .Progress - .Length
            If remainTime > 0 Then
                .Progress = .Length
                Dim oldJob = CurrentJob
                Yield()
                If Not HasFinished AndAlso CurrentJob.IsIOOperation = oldJob.IsIOOperation Then
                    Return DoWork(remainTime) '状态没切换，继续
                Else
                    Return remainTime '切换了等待状态 
                End If
            ElseIf remainTime = 0 Then
                Yield()
            End If
            Return 0
        End With
    End Function
    ''' <summary>
    ''' 等待IO操作并设置 <see cref="ShouldYield"/> 表示这个IO操作是否已经结束了。如果结束了，由上一级在合适的时候调用 <see cref="Yield"/>
    ''' </summary>
    Public Function WaitForAutoResetEvent(waitTime As Integer) As Integer
        If waitTime = 0 Then Throw New ArgumentException(NameOf(waitTime))
        With CurrentJob
            .Progress += waitTime
            Dim remainTime = .Progress - .Length
            If remainTime > 0 Then
                .Progress = .Length
                ShouldYield = True
                Return remainTime
            ElseIf remainTime = 0 Then
                ShouldYield = True
            Else
                ShouldYield = False
            End If
            Return 0
        End With
    End Function
    ''' <summary>
    ''' 准备好切换到下一个任务。
    ''' </summary>
    Public Sub Yield()
        If HasFinished Then
            Throw New SimulationException("已经完成全部任务，无法继续切换")
        ElseIf CurrentJob.Progress < CurrentJob.Length Then
            Throw New SimulationException("检测到冗余的切换操作")
        End If
        JobIndex += 1
        ShouldYield = False
    End Sub
End Class