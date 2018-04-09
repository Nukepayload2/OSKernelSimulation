Public Class StartRoundConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, language As String) As Object Implements IValueConverter.Convert
        Return value.ToString
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, language As String) As Object Implements IValueConverter.ConvertBack
        Dim num = 0
        If Integer.TryParse(value.ToString, num) Then
            Return num
        Else
            Return -1
        End If
    End Function
End Class