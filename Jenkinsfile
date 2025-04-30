properties([pipelineTriggers([githubPush()])])

pipeline {
    agent any

    stages {
        stage('hello') {
            steps {
                // Get some code from a GitHub repository
                echo 'hello'
            }
        }
        stage('deleteWorkspace') {
            steps {
                deleteDir()
            }
        }
        stage('clone') {
            steps {
                // Get some code from a GitHub repository
                git branch: 'main',
                url: 'https://github.com/ProductivityTools-FileSharing/ProductivityTools.FileSharing.Api.git'
            }
        }
        stage('Build PTTrFileSharingips') {
            steps {
                bat(script: "dotnet publish ProductivityTools.FileSharing.Api.sln -c Release ", returnStdout: true)
            }
        }
        

        stage('Create PTFileSharing IIS Page') {
            steps {
                powershell('''
                function CheckIfExist($Name){
                    cd $env:SystemRoot\\system32\\inetsrv
                    $exists = (.\\appcmd.exe list sites /name:$Name) -ne $null
                    Write-Host $exists
                    return  $exists
                }
                
                function Create($Name,$HttpbBnding,$PhysicalPath){
                    $exists=CheckIfExist $Name
                    if ($exists){
                        write-host "Web page already existing"
                    }
                    else
                    {
                        write-host "Creating app pool"
                        .\\appcmd.exe add apppool /name:$Name /managedRuntimeVersion:"v4.0" /managedPipelineMode:"Integrated"
                        write-host "Creating webage"
                        .\\appcmd.exe add site /name:$Name /bindings:http://$HttpbBnding /physicalpath:$PhysicalPath
                        write-host "assign app pool to the website"
                        .\\appcmd.exe set app "$Name/" /applicationPool:"$Name"


                    }
                }
                Create "PTFileSharing" "*:8004"  "C:\\Bin\\IIS\\PTFileSharing\\"                
                ''')
            }
        }

        stage('Stop PTTrips on IIS') {
            steps {
                bat('%windir%\\system32\\inetsrv\\appcmd stop site /site.name:PTFileSharing')
            }
        }

        stage('Delete PTFileSharing IIS directory') {
            steps {
              powershell('''
                if ( Test-Path "C:\\Bin\\IIS\\PTFileSharing")
                {
                    while($true) {
                        if ( (Remove-Item "C:\\Bin\\IIS\\PTFileSharing" -Recurse *>&1) -ne $null)
                        {  
                            write-output "removing faild we should wait"
                        }
                        else 
                        {
                            break 
                        } 
                    }
                  }
              ''')

            }
        }

        // stage('deleteIisDir') {
        //     steps {
                
        //         retry(5) {
        //             bat('if exist "C:\\Bin\\IIS\\PTTrips" RMDIR /Q/S "C:\\Bin\\IIS\\PTTrips"')
        //         }

        //     }
        // }
        stage('Copy PTFileSharing Data') {
            steps {
                bat('xcopy "ProductivityTools.FileSharing.Api\\bin\\Release\\net9.0\\publish" "C:\\Bin\\IIS\\PTFileSharing\\" /O /X /E /H /K')
				                      
            }
        }

        stage('Start PT Trips site on IIS') {
            steps {
                bat('%windir%\\system32\\inetsrv\\appcmd start site /site.name:PTFileSharing')
            }
        }

		
        stage('byebye') {
            steps {
                // Get some code from a GitHub repository
                //				#Add-SqlLogin -ServerInstance ".\\sql2022" -LoginName "IIS APPPOOL\\PTTrips" -LoginType "WindowsUser" -DefaultDatabase "PTTrips"
                //	If(-not(Get-InstalledModule SQLServer -ErrorAction silentlycontinue)){
				//	Install-Module SQLServer -Confirm:$False -Force -AllowClobber 
				//}
                echo 'byebye1'
            }
        }
    }
	post {
		always {
            emailext body: "${currentBuild.currentResult}: Job ${env.JOB_NAME} build ${env.BUILD_NUMBER}\n More info at: ${env.BUILD_URL}",
                recipientProviders: [[$class: 'DevelopersRecipientProvider'], [$class: 'RequesterRecipientProvider']],
                subject: "Jenkins Build ${currentBuild.currentResult}: Job ${env.JOB_NAME}"
		}
	}
}
