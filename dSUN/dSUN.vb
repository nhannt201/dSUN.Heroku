Imports System.IO
Public Class dSUN

    Private docker_path1 As String = ""
    Private docker_path2 As String = ""
    Private project_name As String = ""
    Private net_ver As String = ""

    Private Sub dSUN_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        txtPathProject.ReadOnly = True
        txtNetVersion.ReadOnly = True
        txtOutput.ReadOnly = True
    End Sub



    Private Sub btnDeploy_Click(sender As Object, e As EventArgs) Handles btnDeploy.Click
        clickDeploy()

    End Sub

    Sub clickDeploy()
        txtOutput.Clear()
        tientrinh.Value = 0
        txtAppName.Enabled = False
        If (txtAppName.Text.Trim.Length > 2) Then
            buildPublish(txtPathProject.Text, docker_path1, docker_path2, project_name, net_ver, txtOutput, txtAppName, btnDeploy, tientrinh, btnLog)
            btnDeploy.Enabled = False
        Else
            MsgBox("Vui lòng nhập tên app Heroku của bạn!")
        End If
    End Sub
    Private Sub ExcuteDeploy()
        'txtOutput.Text += RunScript("cd '" + txtPathProject.Text + "'
        '                            dotnet publish -c Release")
        MsgBox("Deployment completed!")
    End Sub

    Private Sub btnOpen_Click(sender As Object, e As EventArgs) Handles btnOpen.Click
        Dim slcFolder As New FolderBrowserDialog
        If slcFolder.ShowDialog() = DialogResult.OK Then
            txtPathProject.Text = slcFolder.SelectedPath

            'Tim file du an sln
            Dim files() As String
            files = Directory.GetFiles(slcFolder.SelectedPath, "*.sln", SearchOption.TopDirectoryOnly)

            Dim filename As String

            If (files.Length > 0) Then

                filename = Path.GetFileName(files(0)).Replace(".sln", "")
                project_name = filename
                'Lay file name xong, gio lay phien ban netcore
                'Boi vi sln nam chung thu muc hoac ben ngoai nen se check duong dan tep .csp 2 lan

                Dim path1 As String = slcFolder.SelectedPath.ToString() + "\" + filename + "\" + filename + ".csproj"
                Dim path2 As String = slcFolder.SelectedPath.ToString() + "\" + filename + ".csproj"
                Dim pathCs As String = ""

                If My.Computer.FileSystem.FileExists(path1) Then
                    pathCs = path1
                ElseIf (My.Computer.FileSystem.FileExists(path2)) Then
                    pathCs = path2
                Else
                    MsgBox("Không tìm thấy tệp .csproj của dự án!")
                End If

                'Kiem  tra csproj ton tai moi tiep tuc

                If (pathCs.Trim.Length > 1) Then


                    'Doc file va lay phien ban

                    Dim value_netcore As String = File.ReadAllText(pathCs)

                    Dim nameNetcore As String = SplitString(value_netcore, "<TargetFramework>", "</TargetFramework>")
                    ' Dim outPut As String = SplitString(value_netcore, "<OutputType>", "</OutputType>")

                    'check co phai la asp.net ko
                    If (value_netcore.Contains("Microsoft.NET.Sdk.Web")) Then

                        'check co lay duoc phien ban ko

                        If (nameNetcore.Trim.Contains("netcore")) Then
                            txtNetVersion.Text = nameNetcore
                            net_ver = nameNetcore.Replace("netcoreapp", "").Trim
                            'neu kiem tra ok thi make path cua debug de tao file Docker

                            ' tao 2 path boi vi co 2 truong hop xay ra

                            docker_path1 = slcFolder.SelectedPath.ToString() + "\bin\Release\" + txtNetVersion.Text + "\publish"
                            docker_path2 = slcFolder.SelectedPath.ToString() + "\" + filename + "\bin\Release\" + txtNetVersion.Text + "\publish"
                            '   Console.WriteLine(docker_path1)
                            '   Console.WriteLine(docker_path2)
                            'thanh cong
                            'If (txtAppName.Text.Length > 0) Then
                            btnDeploy.Enabled = True
                            '   End If
                        Else
                            MsgBox("Không tìm thấy Net Core của Project này!")
                        End If
                    Else
                        MsgBox("Project này không được hỗ trợ!")
                    End If
                End If

                Else
                txtPathProject.Clear()
                MsgBox("Không tìm thấy tệp .sln của dự án!")
            End If
        End If
    End Sub

    Private Sub btnLog_Click(sender As Object, e As EventArgs) Handles btnLog.Click
        Process.Start("cmd", "/k heroku login")
        btnLog.Visible = False
    End Sub
End Class
