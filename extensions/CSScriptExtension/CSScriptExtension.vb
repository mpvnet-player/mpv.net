'this extension implements the C# scripting feature of mpv.net,
'it is based on CS-Script: https://www.cs-script.net/

Imports System.ComponentModel.Composition
Imports System.IO
Imports System.Windows.Forms

Imports mpvnet

Imports CSScriptLibrary

<Export(GetType(IExtension))>
Public Class CSScriptExtension
    Implements IExtension

    Sub New()
        Dim scriptFiles As New List(Of String)

        If Directory.Exists(mp.ConfigFolder + "scripts") Then
            scriptFiles.AddRange(Directory.GetFiles(mp.ConfigFolder + "scripts", "*.cs"))
        End If

        If Directory.Exists(Application.StartupPath + "\scripts") Then
            scriptFiles.AddRange(Directory.GetFiles(Application.StartupPath + "\scripts", "*.cs"))
        End If

        If scriptFiles.Count = 0 Then Return
        CSScriptLibrary.CSScript.EvaluatorConfig.Engine = EvaluatorEngine.CodeDom

        For Each i In scriptFiles
            Try
                CSScriptLibrary.CSScript.Evaluator.LoadCode(File.ReadAllText(i))
            Catch ex As Exception
                Msg.ShowException(ex)
            End Try
        Next
    End Sub
End Class