''' <summary>
''' 表示一个32位的进程工作
''' </summary>
Public Class JobTimelineItem
    Implements INotifyPropertyChanged

    Dim _Length As Integer
    ''' <summary>
    ''' 需要多少个时间片
    ''' </summary> 
    Public Property Length As Integer
        Get
            Return _Length
        End Get
        Set
            _Length = Value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Length)))
        End Set
    End Property
    ''' <summary>
    ''' 已经进行了多少个时间片了
    ''' </summary>
    Public Property Progress%

    Dim _IsIOOperation As Boolean

    ''' <summary>
    ''' 这是不是一个IO操作。这将决定执行工作时状态是<see cref="ThreadStateSilm.Running"/>还是<see cref="ThreadStateSilm.Blocked"/>
    ''' </summary>
    Public Property IsIOOperation As Boolean
        Get
            Return _IsIOOperation
        End Get
        Set
            _IsIOOperation = Value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(IsIOOperation)))
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Function Clone() As JobTimelineItem
        Return DirectCast(MemberwiseClone(), JobTimelineItem)
    End Function
End Class