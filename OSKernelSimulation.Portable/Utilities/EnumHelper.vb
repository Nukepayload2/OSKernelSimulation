Module EnumHelper
    Function GetOrderedEnumValues(Of T As Structure)() As T()
        Return [Enum].GetValues(GetType(T)).OfType(Of T).OrderBy(Function(i) i).ToArray
    End Function
End Module
