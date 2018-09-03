#!/usr/bin/env groovy

pipeline {
    agent { label 'jenkins-slave-windows'}
    options { 
        timestamps() 
    }

    stages {
        stage('Publish') {
            steps {
                powershell '''
                    bundle install
                    bundle exec rake build_release
                '''
            }
        }

        stage('tests') {
            steps {
                powershell '.\\packages\\NUnit.Runners.2.6.4\\tools\\nunit-console.exe .\\Tests\\bin\\Release\\Tests.dll --result="nunit-result.xml"'
            }
            post {
                always {
                    nunit testResultsPattern: 'nunit-result.xml'
                }
            }
        }

        stage('Create nuget') {
            steps {
                echo "Not done yet :-("
            }
        }
    }

    post {
        cleanup {
            step([$class: 'WsCleanup'])
        }
    }
}
