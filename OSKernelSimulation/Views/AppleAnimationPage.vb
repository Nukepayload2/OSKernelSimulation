#If WINDOWS_UWP Or WINDOWS_PHONE_APP Then
Imports Windows.UI.Xaml.Media.Animation
''' <summary>
''' 移植苹果的应用时使用。这种页面自带苹果导航动画和手势。
''' </summary>
Public MustInherit Class AppleAnimationPage
    Inherits Page
    Dim PaneAnim As New PaneThemeTransition With {.Edge = EdgeTransitionLocation.Right}
    Sub New()
        MyBase.New
        Transitions = New TransitionCollection
        Transitions.Add(PaneAnim)
        ManipulationMode = ManipulationModes.TranslateX
    End Sub
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        PaneAnim.Edge = If(e.NavigationMode = NavigationMode.Back, EdgeTransitionLocation.Left, EdgeTransitionLocation.Right)
        MyBase.OnNavigatedTo(e)
    End Sub
    Protected Overrides Sub OnNavigatingFrom(e As NavigatingCancelEventArgs)
        PaneAnim.Edge = If(e.NavigationMode <> NavigationMode.Back, EdgeTransitionLocation.Left, EdgeTransitionLocation.Right)
        MyBase.OnNavigatingFrom(e)
    End Sub
    Private Sub AppleAnimationPage_ManipulationCompleted(sender As Object, e As ManipulationCompletedRoutedEventArgs) Handles Me.ManipulationCompleted
        Dim trans = e.Cumulative.Translation
        Dim DeltaX As Double = Math.Abs(trans.X)
        If Math.Abs(trans.Y) * 3 < DeltaX AndAlso DeltaX > ActualWidth / 2 Then
            If trans.X > 0 Then
                If Frame.CanGoBack Then Frame.GoBack()
            Else
                If Frame.CanGoForward Then Frame.GoForward()
            End If
        End If
    End Sub
End Class
#End If