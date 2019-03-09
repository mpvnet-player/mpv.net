Imports System.ComponentModel.Composition
Imports System.IO

Imports mpvnet
Imports mpvnet.StaticUsing

Imports CSScriptLibrary

<Export(GetType(IAddon))>
Public Class CSScriptAddon
    Implements IAddon

    Sub New()
        Dim scriptDir = mpv.mpvConfFolderPath + "scripts"
        If Not Directory.Exists(scriptDir) Then Return
        Dim csFiles = Directory.GetFiles(scriptDir, "*.cs")
        If csFiles.Count = 0 Then Return
        CSScriptLibrary.CSScript.EvaluatorConfig.Engine = EvaluatorEngine.CodeDom

        For Each i In csFiles
            Try
                CSScriptLibrary.CSScript.Evaluator.LoadCode(File.ReadAllText(i))
            Catch ex As Exception
                MsgError(ex.ToString)
            End Try
        Next
    End Sub
End Class