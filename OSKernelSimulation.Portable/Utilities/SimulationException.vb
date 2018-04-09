Public Class SimulationException
    Inherits Exception
    Sub New()
        MyBase.New
    End Sub
    Sub New(Message$)
        MyBase.New(Message)
    End Sub
    Sub New(Message$, Inner As Exception)
        MyBase.New(Message, Inner)
    End Sub
End Class
