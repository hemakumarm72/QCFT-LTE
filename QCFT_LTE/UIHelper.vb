Module UIHelper
    Public Sub InvokeOnUIifRequired(control As Control, code As MethodInvoker)
        If control.InvokeRequired AndAlso control.Visible Then
            control.Invoke(code)
            Return
        End If
        code()
    End Sub
End Module
