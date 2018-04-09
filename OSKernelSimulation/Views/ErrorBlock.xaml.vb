' The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

Public NotInheritable Class ErrorBlock
    Inherits UserControl
    Public Sub Show(errMessage$)
        TblErrorText.Text = errMessage
        Visibility = Visibility.Visible
    End Sub
    Public Sub Hide()
        Visibility = Visibility.Collapsed
    End Sub
    Private Sub BtnOk_Click(sender As Object, e As RoutedEventArgs)
        Hide()
    End Sub
End Class
