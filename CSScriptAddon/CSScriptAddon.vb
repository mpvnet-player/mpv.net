' mpv.net
' Copyright(C) 2017 stax76
' 
' This program Is free software: you can redistribute it And/Or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, Or
' (at your option) any later version.
' 
' This program Is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY Or FITNESS FOR A PARTICULAR PURPOSE.See the
' GNU General Public License for more details.
' 
' You should have received a copy of the GNU General Public License
' along with this program. If Not, see http://www.gnu.org/licenses/.

Imports System.ComponentModel.Composition
Imports System.IO

Imports vbnet
Imports mpvnet
Imports vbnet.UI.MainModule
Imports CSScriptLibrary

<Export(GetType(IAddon))>
Public Class CSScriptAddon
    Implements IAddon

    Sub New()
        Dim scriptDir = Folder.AppDataRoaming + "mpv\scripts"
        If Not Directory.Exists(scriptDir) Then Return
        Dim csxFiles = Directory.GetFiles(scriptDir, "*.cs")
        If csxFiles.Count = 0 Then Return
        CSScriptLibrary.CSScript.EvaluatorConfig.Engine = EvaluatorEngine.CodeDom
        CSScriptLibrary.CSScript.Evaluator.ReferenceDomainAssemblies()

        For Each i In csxFiles
            Try
                CSScriptLibrary.CSScript.Evaluator.LoadCode(File.ReadAllText(i))
            Catch ex As Exception
                MsgException(ex)
            End Try
        Next
    End Sub
End Class