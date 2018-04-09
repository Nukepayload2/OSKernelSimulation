Imports Windows.UI

Public Class IsIOOperationColorConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, language As String) As Object Implements IValueConverter.Convert
        Return If(DirectCast(value, Boolean), New SolidColorBrush(Colors.ForestGreen), New SolidColorBrush(Colors.DodgerBlue))
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, language As String) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function
End Class
