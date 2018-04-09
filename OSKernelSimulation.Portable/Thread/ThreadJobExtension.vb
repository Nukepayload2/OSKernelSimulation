Module ThreadJobExtension
    ''' <summary>
    ''' 优先级越高，每次执行的时间片越少
    ''' </summary>
    <Extension>
    Friend Function GetWorkTime(priority As ThreadPriorityLevel) As Integer
        Select Case priority
            Case ThreadPriorityLevel.Lowest
                Return 5
            Case ThreadPriorityLevel.BelowNormal
                Return 4
            Case ThreadPriorityLevel.Normal
                Return 3
            Case ThreadPriorityLevel.AboveNormal
                Return 2
            Case Else
                Return 1
        End Select
    End Function
End Module