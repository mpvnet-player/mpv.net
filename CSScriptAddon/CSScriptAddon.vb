Imports System.ComponentModel.Composition
Imports System.IO
Imports System.Windows.Forms

Imports mpvnet

Imports CSScriptLibrary

<Export(GetType(IAddon))>
Public Class CSScriptAddon
    Implements IAddon

    Sub New()
        Dim scriptDir = mp.MpvConfFolder + "scripts"
        If Not Directory.Exists(scriptDir) Then Return
        Dim csFiles = Directory.GetFiles(scriptDir, "*.cs").ToList
        csFiles.AddRange(Directory.GetFiles(Application.StartupPath + "\\Scripts", "*.cs"))
        If csFiles.Count = 0 Then Return
        CSScriptLibrary.CSScript.EvaluatorConfig.Engine = EvaluatorEngine.CodeDom

        For Each i In csFiles
            Try
                CSScriptLibrary.CSScript.Evaluator.LoadCode(File.ReadAllText(i))
            Catch ex As Exception
                MainForm.Instance.ShowMsgBox(ex.ToString(), MessageBoxIcon.Error)
            End Try
        Next
    End Sub
End Class