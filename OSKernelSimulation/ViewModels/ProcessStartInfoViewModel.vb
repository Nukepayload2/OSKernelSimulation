Imports OSKernelSimulation.Portable

Public Class ProcessStartInfoViewModel
    Public Shared ReadOnly Property Current As ProcessStartInfoViewModel
    Sub New()
        _Current = Me
    End Sub
    Public Property ProcessStartInfo As New ProcessStartInfo
    Public Property JobStartInfo As New JobTimelineItem
End Class
