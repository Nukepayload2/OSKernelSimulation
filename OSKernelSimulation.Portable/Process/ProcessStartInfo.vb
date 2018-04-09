Public Class ProcessStartInfo
    Implements INotifyPropertyChanged

    Dim _Name As String
    Public Property Name As String
        Get
            Return _Name
        End Get
        Set
            _Name = Value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Name)))
        End Set
    End Property

    Dim _BasePriority As ThreadPriorityLevel
    Public Property BasePriority As ThreadPriorityLevel
        Get
            Return _BasePriority
        End Get
        Set
            _BasePriority = Value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(BasePriority)))
        End Set
    End Property

    Dim _InitialTimeline As New JobTimeline
    Public Property InitialTimeline As JobTimeline
        Get
            Return _InitialTimeline
        End Get
        Set
            _InitialTimeline = Value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(InitialTimeline)))
        End Set
    End Property

    Dim _StartRound As Integer
    Public Property StartRound As Integer
        Get
            Return _StartRound
        End Get
        Set
            _StartRound = Value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(StartRound)))
        End Set
    End Property

    Public Function CloneWithoutTimeline() As ProcessStartInfo
        Return New ProcessStartInfo With {.Name = Name, .BasePriority = BasePriority, .StartRound = StartRound}
    End Function

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

End Class