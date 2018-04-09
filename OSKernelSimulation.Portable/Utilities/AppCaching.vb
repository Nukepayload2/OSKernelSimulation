Imports Newtonsoft.Json.JsonConvert

Public Class AppCaching
    ''' <summary>
    ''' 存档为UTF-16格式的文本，并且写入流中。
    ''' </summary>
    ''' <param name="output">要写入的流</param>
    Public Shared Async Function SaveAsync(output As Stream, obj As Object) As Task
        Dim content = Await Task.Run(Function() SerializeObject(obj))
        output.SetLength(0)
        Using writer As New StreamWriter(output, Text.Encoding.Unicode)
            Await writer.WriteAsync(content)
        End Using
    End Function
    ''' <summary>
    ''' 从流中以UTF-16格式读取状态并恢复。
    ''' </summary>
    ''' <param name="input">要读取的流</param>
    Public Shared Async Function LoadAsync(Of T)(input As Stream, obj As T) As Task
        Using reader As New StreamReader(input, Text.Encoding.Unicode)
            PopulateObject(Await reader.ReadToEndAsync, obj)
        End Using
    End Function
End Class
