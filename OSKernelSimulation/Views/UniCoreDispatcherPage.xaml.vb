' “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

Imports OSKernelSimulation.Portable
Imports Windows.UI
''' <summary>
''' 可用于自身或导航至 Frame 内部的空白页。
''' </summary>
Public NotInheritable Class UniCoreDispatcherPage
    Private Sub BtnCreateProcess_Click(sender As Object, e As RoutedEventArgs)
        ProcessOperation(TblProcTitle.Text = "修改进程信息")
    End Sub

    Private Sub ProcessOperation(isEdit As Boolean)
        Dim proc = ProcessStartInfoViewModel.Current.ProcessStartInfo
        Dim chs = Path.GetInvalidFileNameChars
        If String.IsNullOrEmpty(proc.Name) Then
            TxtName.BorderBrush = New SolidColorBrush(Colors.Red)
        Else
            Dim invalid = From ch In proc.Name Where chs.Contains(ch)
            If invalid.Any OrElse proc.Name.Length = 0 Then
                TxtName.BorderBrush = New SolidColorBrush(Colors.Red)
                ErrBlock.Show("进程名无效")
            ElseIf proc.StartRound < 0 Then
                ErrBlock.Show("启动时间无效, 请确保输入了范围合理的正整数")
            Else
                ErrBlock.Hide()
                TxtName.BorderBrush = New SolidColorBrush(Color.FromArgb(255, 122, 122, 122))
                If isEdit Then
                    If LstProcesses.SelectedIndex >= 0 Then
                        Dim np = proc.CloneWithoutTimeline
                        np.InitialTimeline = SimulatorViewModel.Current.SimulatorStartCommands.Processes(LstProcesses.SelectedIndex).InitialTimeline
                        SimulatorViewModel.Current.SimulatorStartCommands.Processes(LstProcesses.SelectedIndex) = np
                    End If
                Else
                    SimulatorViewModel.Current.SimulatorStartCommands.CreateProcess(proc.CloneWithoutTimeline)
                End If
            End If
        End If
    End Sub

    Private Sub BtnAddTask_Click(sender As Object, e As RoutedEventArgs)
        TaskOperation(TblTaskHeader.Text = "修改现有任务")
    End Sub

    Private Sub TaskOperation(isEdit As Boolean)
        Dim proc = TryCast(LstProcesses.SelectedItem, ProcessStartInfo)
        If proc IsNot Nothing Then
            ErrBlock.Hide()
            Dim job = ProcessStartInfoViewModel.Current.JobStartInfo
            If isEdit Then
                If LstTasks.SelectedIndex >= 0 Then
                    Dim curJob = proc.InitialTimeline.Jobs(LstTasks.SelectedIndex)
                    curJob.IsIOOperation = job.IsIOOperation
                    curJob.Length = job.Length
                End If
            Else
                proc.InitialTimeline.Jobs.Add(job.Clone)
            End If
        Else
            ErrBlock.Show("尚未选择任何进程，无法添加任务")
        End If
    End Sub

    Private Sub LstProcesses_RightTapped(sender As Object, e As RightTappedRoutedEventArgs)
        Dim att = FlyoutBase.GetAttachedFlyout(LstProcesses)
        If att IsNot Nothing AndAlso LstProcesses.SelectedItem IsNot Nothing Then
            att.ShowAt(LstProcesses)
        End If
    End Sub

    Private Async Sub BtnSaveCommandList_Click(sender As Object, e As RoutedEventArgs)
        Try
            PrgIO.IsActive = True
            IsHitTestVisible = False
            ErrBlock.Hide()
            Dim local = Windows.Storage.ApplicationData.Current.LocalFolder
            Dim createinf = Await local.CreateFileAsync("createinf.json", Windows.Storage.CreationCollisionOption.OpenIfExists)
            Using strm = Await createinf.OpenStreamForWriteAsync
                Await AppCaching.SaveAsync(strm, ProcessStartInfoViewModel.Current)
            End Using
            Dim cmdlst = Await local.CreateFileAsync("cmdlist.json", Windows.Storage.CreationCollisionOption.OpenIfExists)
            Using strm = Await cmdlst.OpenStreamForWriteAsync
                Dim simulatorStartCommands = SimulatorViewModel.Current.SimulatorStartCommands
                simulatorStartCommands.Reset()
                Await AppCaching.SaveAsync(strm, simulatorStartCommands)
            End Using
        Catch ex As Exception
            ErrBlock.Show(ex.Message)
        Finally
            IsHitTestVisible = True
            PrgIO.IsActive = False
        End Try
    End Sub

    Private Async Sub BtnLoadCommandList_Click(sender As Object, e As RoutedEventArgs)
        Try
            PrgIO.IsActive = True
            ErrBlock.Hide()
            Dim local = Windows.Storage.ApplicationData.Current.LocalFolder
            Dim createinf = Await local.CreateFileAsync("createinf.json", Windows.Storage.CreationCollisionOption.OpenIfExists)
            Using strm = Await createinf.OpenStreamForReadAsync
                Await AppCaching.LoadAsync(strm, ProcessStartInfoViewModel.Current)
            End Using
            Dim cmdlst = Await local.CreateFileAsync("cmdlist.json", Windows.Storage.CreationCollisionOption.OpenIfExists)
            SimulatorViewModel.Current.SimulatorStartCommands.Processes.Clear()
            Using strm = Await cmdlst.OpenStreamForReadAsync
                Await AppCaching.LoadAsync(strm, SimulatorViewModel.Current.SimulatorStartCommands)
            End Using
        Catch ex As FileNotFoundException
            ErrBlock.Show("你还没有存过档")
        Catch ex As Exception
            ErrBlock.Show(ex.Message)
        Finally
            PrgIO.IsActive = False
        End Try
    End Sub

    Private Sub BtnDelete_Click(sender As Object, e As RoutedEventArgs)
        If LstProcesses.SelectedIndex >= 0 Then
            SimulatorViewModel.Current.SimulatorStartCommands.Processes.RemoveAt(LstProcesses.SelectedIndex)
        End If
    End Sub

    Private Sub BtnModify_Click(sender As Object, e As RoutedEventArgs)
        If LstProcesses.SelectedIndex >= 0 Then
            TblProcTitle.Text = "修改进程信息"
            Dim proc = ProcessStartInfoViewModel.Current.ProcessStartInfo
            Dim pro = SimulatorViewModel.Current.SimulatorStartCommands.Processes(LstProcesses.SelectedIndex)
            proc.Name = pro.Name
            proc.InitialTimeline = pro.InitialTimeline
            proc.StartRound = pro.StartRound
            BtnAddProcess.Flyout.ShowAt(BtnAddProcess)
        Else
            ErrBlock.Show("未选择要修改的进程")
        End If
    End Sub

    Private Sub BtnAddProcess_Click(sender As Object, e As RoutedEventArgs) Handles BtnAddProcess.Click
        TblProcTitle.Text = "添加新进程"
    End Sub

    Private Sub BtnDeleteTsk_Click(sender As Object, e As RoutedEventArgs)
        If LstTasks.SelectedIndex >= 0 Then
            SimulatorViewModel.Current.SimulatorStartCommands.Processes(LstProcesses.SelectedIndex).InitialTimeline.Jobs.RemoveAt(LstTasks.SelectedIndex)
        End If
    End Sub

    Private Sub BtnModifyTsk_Click(sender As Object, e As RoutedEventArgs)
        If LstTasks.SelectedIndex >= 0 Then
            TblTaskHeader.Text = "修改现有任务"
            Dim curJob = SimulatorViewModel.Current.SimulatorStartCommands.Processes(LstProcesses.SelectedIndex).InitialTimeline.Jobs(LstTasks.SelectedIndex)
            Dim oldJob = ProcessStartInfoViewModel.Current.JobStartInfo
            oldJob.IsIOOperation = curJob.IsIOOperation
            oldJob.Length = curJob.Length
            BtnAddNewTask.Flyout.ShowAt(BtnAddNewTask)
        Else
            ErrBlock.Show("未选择要修改的任务")
        End If
    End Sub

    Private Sub BtnAddNewTask_Click(sender As Object, e As RoutedEventArgs)
        TblTaskHeader.Text = "添加新任务"
    End Sub

    Private Sub LstTasks_RightTapped(sender As Object, e As RightTappedRoutedEventArgs) Handles LstTasks.RightTapped
        Dim att = FlyoutBase.GetAttachedFlyout(LstTasks)
        If att IsNot Nothing AndAlso LstTasks.SelectedItem IsNot Nothing Then
            att.ShowAt(LstTasks)
        End If
    End Sub

    Private Async Sub btnRun_Click(sender As Object, e As RoutedEventArgs)
        Dim simu = SimulatorViewModel.Current
        ErrBlock.Hide()
        Dim btnRun = DirectCast(sender, Button)
        btnRun.IsEnabled = False
        Try
            Await simu.Simulator.RunAsync(simu.SimulatorStartCommands, simu.DelayTime)
        Catch ex As Exception
            ErrBlock.Show(ex.Message)
        Finally
            btnRun.IsEnabled = True
        End Try
    End Sub

    Private Async Sub btnExportLog_Click(sender As Object, e As RoutedEventArgs)
        Try
            prgExportLog.IsActive = True
            ErrBlock.Hide()
            Dim simu = SimulatorViewModel.Current
            Await simu.SaveLogAsync
        Catch ex As Exception
            ErrBlock.Show(ex.Message)
        Finally
            prgExportLog.IsActive = False
        End Try
    End Sub
End Class